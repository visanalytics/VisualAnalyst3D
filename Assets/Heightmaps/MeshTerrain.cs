using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;
public class MeshTerrain
{
	protected Texture2D heightMap;
	protected Texture2D colorMap;
	protected double minX, minZ, maxX, maxZ;
	
	protected double lowerBound, upperBound;
	protected String filename;
	double hmapWidth;
	double hmapHeight;
	double cmapWidth;
	double cmapHeight;
	
	public static int HILLS = 0; 
	public static int PEAKS = 1;

	// Variables Holder
	Variables Vars;

	/// <summary>
	/// Initializes a new instance of the <see cref="MeshTerrain"/> class.
	/// </summary>
	/// <param name="Vars">Variables instance.</param>
	/// <param name="filename">Filename.</param>
	/// <param name="minX">Minimum X coord.</param>
	/// <param name="minZ">Minimum Z coord.</param>
	/// <param name="maxX">Max X coord.</param>
	/// <param name="maxZ">Max Z coord.</param>
	public MeshTerrain (Variables Vars, String filename,
	                    double minX, double minZ, double maxX, double maxZ)
	{
		this.Vars = Vars;
		this.filename = filename;
		this.minX = minX;
		this.minZ = minZ;
		this.maxX = maxX;
		this.maxZ = maxZ;
//		this.minX = minZ;
//		this.minZ = maxX;
//		this.maxX = maxZ;
//		this.maxZ = minX;
	}

	/// <summary>
	/// Gets the height and color map.
	/// </summary>
	/// <returns>The heightmap texture generated.</returns>
	/// <param name="heightMapWidth">Height map width.</param>
	/// <param name="heightMapHeight">Height map height.</param>
	/// <param name="spectrum">Spectrum.</param>
	public virtual Texture2D GetHeightmap(double heightMapWidth,
	                                                double heightMapHeight,
	                                                ColorSpectrumObj spectrum){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		
		double[] bnds= dataHandler.GetBounds();
		lowerBound = 0d;
		upperBound = bnds[1];

		this.hmapWidth = heightMapWidth;
		this.hmapHeight = heightMapHeight;
		heightMap = new Texture2D((int)hmapWidth, (int)hmapHeight);
		FillInColor(heightMap,Color.black);

		CreateHeightMap(dataHandler.GetData());
		return heightMap;
	}

	/// <summary>
	/// Fills the entire bitmap with a given color.
	/// </summary>
	/// <param name="c">Color with which to fill.</param>
	protected void FillInColor(Texture2D tx, Color c){
		for(int i = 0; i < tx.width; i++) {
			for(int j = 0; j < tx.height; j++) {
				tx.SetPixel(i,j, c);
			}
		}
	}

	/// <summary>
	/// Saves heightmap bitmap texture to disk memory.
	/// </summary>
	public virtual void SaveHeightToDisk(Texture2D tx) {
		byte[] bytes = tx.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/Heightmaps/Images/GA_" + Vars.TERRAIN_NAME + ".png", bytes);
	}

	/// <summary>
	/// Saves colormap bitmap texture to disk memory.
	/// </summary>
	public virtual void SaveColorToDisk(Texture2D tx) {
		byte[] bytes = tx.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/Heightmaps/Images/GA_Color_" + Vars.TERRAIN_NAME + ".png", bytes);
	}

	
	double[,] map;
	bool[,] map_pump;
	bool[,] map_nonzero;
	bool[,] map_nonzero_temp;
	double[,] map_temp;

	/// <summary>
	/// Creates the height map.
	/// </summary>  
	/// <param name="data">Data with each object containing the following structure:    
	/// [ peak_height, peak_X, peak_Y ]
	/// </param> 
	protected void CreateHeightMap(List<double[]> data){
		/// Parallel Processing
		int numCores = SystemInfo.processorCount;

		// convert list of double[] to list of Vector3's and sort it. 
		List<Vector3> points = new List<Vector3>();
		for(int i=0; i<data.Count; i++){
			double[] t = data[i];
			points.Add(new Vector3((float)t[0], (float)t[1], (float)t[2]));
		}

		//Genetic Algorithm for mesh construction
		map = new double[(int)hmapWidth, (int)hmapHeight];
		map_pump = new bool[(int)hmapWidth, (int)hmapHeight];
		map_nonzero = new bool[(int)hmapWidth, (int)hmapHeight];
		map_nonzero_temp = new bool[(int)hmapWidth, (int)hmapHeight];
		map_temp = new double[(int)hmapWidth, (int)hmapHeight];
		for(int i=0; i<points.Count; i++){
			Vector3 p = points[i];
			int x = (int)(p.x*(hmapWidth-1));
			int z = (int)(p.z*(hmapHeight-1));
			map[x, z] = p.y;
			map_pump[x, z] = true;
			heightMap.SetPixel(x,z,GrayscaleFromHeight(p.y, upperBound, lowerBound));
		}

		for(int a=0; a<hmapWidth; a++){
			for (int b=0; b<hmapHeight; b++){
				if(!map_pump[a,b])
					map[a,b] = 0f;
				map_nonzero[a,b] = true;
			}
		}
		heightMap.Apply();

		Array.Copy (map, map_temp,map.Length);
		Array.Copy (map_nonzero, map_nonzero_temp, map_nonzero.Length);
		int iterationMax = Vars.GENETIC_MAX_ITERATION;	//Variable in CSV
		//BEGIN GENETIC ALGORITHM 
		for(int i=0; i<iterationMax; i++){
			for(int n=0; n<numCores; n++){
				System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(GenerateMesh);
				System.Threading.Thread worker = new System.Threading.Thread(pts);
				worker.Start(new int[]{(int)(hmapHeight/numCores)*n, (int)((hmapHeight/numCores)*(n+1))});
			}
			Array.Copy (map_temp, map, map_temp.Length);
			Array.Copy (map_nonzero_temp, map_nonzero, map_nonzero_temp.Length);
		}
		heightMap.Apply();

		// adjust pumps 
		for(int i=0; i<Vars.GENETIC_SMOOTH_ITERATION; i++){
			for(int n=0; n<numCores; n++){
				System.Threading.ParameterizedThreadStart pts = new System.Threading.ParameterizedThreadStart(SmoothMesh);
				System.Threading.Thread worker = new System.Threading.Thread(pts);
				worker.Start(new int[]{(int)(hmapHeight/numCores)*n, (int)((hmapHeight/numCores)*(n+1))});
			}
			Array.Copy (map_temp, map, map_temp.Length);
			Array.Copy (map_nonzero_temp, map_nonzero, map_nonzero_temp.Length);
		}
		heightMap.Apply(); 

		// Scale values back to original bounds:
		double minC = map.Cast<double>().Min(), maxC = map.Cast<double>().Max ();
		for(int x=0; x<hmapWidth; x++){
			for (int z=0; z<hmapHeight; z++){
				heightMap.SetPixel(x,z,GrayscaleFromHeight(map[x, z], maxC, minC)); 
			}
		}

		heightMap.Apply(); 
		SaveHeightToDisk(heightMap);
	}

	/// <summary>
	/// Smooths the array representing the mesh texture.
	/// </summary>
	/// <param name="rangeArray">Array containing the data
	/// in the form.</param>
	private void SmoothMesh(object rangeArray){
		for(int x=0; x<hmapWidth; x++){
			for (int z=0; z<hmapHeight; z++){
				double avg = 0f;
				double div = 0f;
				if(x > 0){
					avg += map[x-1, z];
					div += map_nonzero[x-1, z]? 1 : 0;
				}if(z > 0){
					avg += map[x, z-1];
					div += map_nonzero[x, z-1]? 1 : 0;
				}if(x < hmapWidth-1){
					avg += map[x+1, z];
					div += map_nonzero[x+1, z]? 1 : 0;
				}if( z < hmapHeight-1){
					avg += map[x, z+1];
					div += map_nonzero[x, z+1]? 1 : 0;
				}if(x > 0 && z > 0){
					avg += map[x-1, z-1];
					div += map_nonzero[x-1, z-1]? 1 : 0;
				}if(x < hmapWidth-1 && z > 0){
					avg += map[x+1, z-1];
					div += map_nonzero[x+1, z-1]? 1 : 0;
				}if(x > 0 && z < hmapHeight-1){
					avg += map[x-1, z+1];
					div += map_nonzero[x-1, z+1]? 1 : 0;
				}if(x < hmapWidth-1 && z < hmapHeight-1){
					avg += map[x+1, z+1];
					div += map_nonzero[x+1, z+1]? 1 : 0;
				}
				
				avg = (div != 0) ? avg/div : 0;
				map_temp[x,z] = avg;
				map_nonzero_temp[x, z] = (avg != 0) ? true : false;
			}
		}
	}

	/// <summary>
	/// Generates the initial terrain data.
	/// </summary>
	/// <param name="rangeArray">Array containing the data
	/// in the form.</param>
	private void GenerateMesh(object rangeArray){
		int start = ((int[])rangeArray)[0];
		int stop = ((int[])rangeArray)[1];
		for(int x=0; x<hmapWidth; x++){
			for(int z=start; z<stop; z++){
				if(!map_pump[x,z]){
					double avg = 0f;
					double div = 0f;
					if(x > 0){
						avg += map[x-1, z];
						div += map_nonzero[x-1, z]? 1 : 0;
					}if(z > 0){
						avg += map[x, z-1];
						div += map_nonzero[x, z-1]? 1 : 0;
					}if(x < hmapWidth-1){
						avg += map[x+1, z];
						div += map_nonzero[x+1, z]? 1 : 0;
					}if( z < hmapHeight-1){
						avg += map[x, z+1];
						div += map_nonzero[x, z+1]? 1 : 0;
					}if(x > 0 && z > 0){
						avg += map[x-1, z-1];
						div += map_nonzero[x-1, z-1]? 1 : 0;
					}if(x < hmapWidth-1 && z > 0){
						avg += map[x+1, z-1];
						div += map_nonzero[x+1, z-1]? 1 : 0;
					}if(x > 0 && z < hmapHeight-1){
						avg += map[x-1, z+1];
						div += map_nonzero[x-1, z+1]? 1 : 0;
					}if(x < hmapWidth-1 && z < hmapHeight-1){
						avg += map[x+1, z+1];
						div += map_nonzero[x+1, z+1]? 1 : 0;
					}
					
					avg = (div != 0) ? avg/div : 0;
					map_temp[x,z] = avg;
					map_nonzero_temp[x, z] = (avg != 0) ? true : false;
				}
			}
		}
	}

	/// <summary>
	/// Returns the color associated with the height of the data value
	/// for the heightmap texture.
	/// </summary>
	/// <returns>The color associated with the height
	/// of the data point.</returns>
	/// <param name="height">Height to convert to color.</param>
	/// <param name="maxHeight">Max height.</param>
	/// <param name="minHeight">Minimum height.</param>
	protected virtual UnityEngine.Color GrayscaleFromHeight(double height, double maxHeight, double minHeight) 
	{ 
		UnityEngine.Color color = new UnityEngine.Color(0,0,0,1);
		double scalar = (height-minHeight) / (maxHeight-minHeight);
		Int32 mColor = (Int32)Math.Floor(scalar * Math.Pow(2, 24));
		Int32 b = mColor%256;
		Int32 g = mColor%(256*256);
		g /= 256;
		Int32 r = mColor;
		r /= 256*256;
		color.r = (float)((float)r/256f);
		color.g = (float)((float)g/256f);
		color.b = (float)((float)b/256f);
		return color;
	}
}