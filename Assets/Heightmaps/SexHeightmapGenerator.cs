using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;
public class SexHeightmapGenerator
{
	
	protected Texture2D heightMap;
	double hmapWidth, hmapHeight;
	protected double minX, minZ, maxX, maxZ;
	
	protected double lowerBound, upperBound;
	protected double maxRadius;
	protected String filename;

	ColorSpectrumObj spectrum;
	
	// Variables Holder
	Variables Vars;

	/// <summary>
	/// Initializes a new instance of the <see cref="SexHeightmapGenerator"/> class.
	/// </summary>
	/// <param name="Vars">Variables instane.</param>
	/// <param name="filename">Filename.</param>
	/// <param name="maxRadius">Max radius.</param>
	/// <param name="minX">Minimum X coord.</param>
	/// <param name="minZ">Minimum Z coord.</param>
	/// <param name="maxX">Max X coord.</param>
	/// <param name="maxZ">Max Z coord.</param>
	public SexHeightmapGenerator (Variables Vars, String filename, double maxRadius,
	                              double minX, double minZ, double maxX, double maxZ)
	{
		this.Vars = Vars;
		this.filename = filename;
		this.maxRadius = maxRadius;
		this.minX = minZ;
		this.minZ = maxX;
		this.maxX = maxZ;
		this.maxZ = minX;
	}

	/// <summary>
	/// Gets the mesh heightmap.
	/// </summary>
	/// <returns>The mesh heightmap texture.</returns>
	/// <param name="heightMapWidth">Height map texture width.</param>
	/// <param name="heightMapHeight">Height map texture height.</param>
	public Texture2D GetMeshHeightmap(double heightMapWidth,
	                                  double heightMapHeight){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		hmapWidth = heightMapWidth;
		hmapHeight = heightMapHeight;
		double[] bnds= dataHandler.GetBounds();
		lowerBound = 0f;
		upperBound = bnds[1];

		heightMap = new Texture2D((int)heightMapWidth, (int)heightMapHeight);
		heightMap = CreateHeightMap(dataHandler.GetData());
		SaveToDisk(heightMap, "GA_" + Vars.TERRAIN_NAME);

		return heightMap;
	}

	/// <summary>
	/// Gets the mesh colormap texture.
	/// </summary>
	/// <returns>The mesh colormap texture.</returns>
	/// <param name="heightMapWidth">Height map texture width.</param>
	/// <param name="heightMapHeight">Height map texture height.</param>
	/// <param name="spectrum">ColorSpectrumObj.</param>
	public Texture2D GetMeshColormap(double heightMapWidth,
	                                 double heightMapHeight,
	                                 ColorSpectrumObj spectrum){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		hmapWidth = heightMapWidth;
		hmapHeight = heightMapHeight;
		double[] bnds= dataHandler.GetBounds();
		lowerBound = bnds[0];
		upperBound = bnds[1];
		
		UnityEngine.Color[] colors = new UnityEngine.Color[]{UnityEngine.Color.red,
			UnityEngine.Color.red,
			UnityEngine.Color.magenta,
			UnityEngine.Color.black,
			UnityEngine.Color.cyan,
			UnityEngine.Color.blue,
			UnityEngine.Color.blue};

		float[] proportions = new float[]{0.0f,
			(float)((-lowerBound)/(upperBound-lowerBound))/2f,
			(float)((-lowerBound)/(upperBound-lowerBound))-0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound)),
			(float)((-lowerBound)/(upperBound-lowerBound))+0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound))+(1-(float)((-lowerBound)/(upperBound-lowerBound)))/2f,
			1.0f};
		
		this.spectrum = new ColorSpectrumObj(colors, proportions);
		heightMap = new Texture2D((int)heightMapWidth, (int)heightMapHeight);
		heightMap = CreateColorMap(dataHandler.GetData());
		SaveToDisk(heightMap, "GA_Color_" + Vars.TERRAIN_NAME);
		
		return heightMap;
	} 

	/// <summary>
	/// Gets the hills heightmap texture.
	/// </summary>
	/// <returns>The hills heightmap texture.</returns>
	/// <param name="heightMapWidth">Height map texture width.</param>
	/// <param name="heightMapHeight">Height map texture height.</param>
	public Texture2D GetHillsHeightmap(double heightMapWidth,
	                                   double heightMapHeight){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		
		double[] bnds= dataHandler.GetBounds();
		lowerBound = 0f;
		upperBound = bnds[1];

		HeightmapGenerator heightGen = new HeightmapGenerator(Vars, filename, Vars.MAX_RADIUS, Vars.MIN_X,Vars.MIN_Z,Vars.MAX_X,Vars.MAX_Z);
		heightMap = heightGen.GetHeightmap(heightMapWidth, heightMapHeight, true);
		SaveToDisk(heightMap, "heightmap_" + Vars.TERRAIN_NAME);
		return heightMap;
	}

	/// <summary>
	/// Gets the heightmap with terrain type Cylindrical.
	/// </summary>
	/// <returns>The heightmap texture.</returns>
	/// <param name="bWidth">Texture width.</param>
	/// <param name="bHeight">Texture height.</param>
	public virtual Texture2D GetHillsSexHeightmapCylinder(double bWidth, double bHeight){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		List<double[]> data = dataHandler.GetData();
		heightMap = new Texture2D((int)bWidth, (int)bHeight);
		FillInColor(UnityEngine.Color.black);
		
		double[] bnds= dataHandler.GetBounds();
		lowerBound = 0;
		upperBound = bnds[1];

		double it = (upperBound-lowerBound)/Vars.ITERATION_NUMBER;
		if(it == 0)
			it = 1f;

		System.Drawing.Bitmap bit = new System.Drawing.Bitmap(heightMap.width, heightMap.height);
		System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bit);
		
		double h = lowerBound;
		while(data.Count != 0){
			h += it;
			List<int> toRemove = new List<int>();
			for(int i=0; i<data.Count; i++){
				if(Math.Abs(data[i][1]) < h){ //If middle of the values is less than the lower bounds, then add it "toRemove" List
					toRemove.Add(i);
					double height = Math.Abs(data[i][1]);
					System.Drawing.Color col = ColourConvert(height);
					Brush brush = new SolidBrush(col);
					Rectangle rect = new Rectangle((int)(data[i][0] * bWidth) - 5, (int)(data[i][2] * bHeight) - 5, 10 , 10);
					g.FillEllipse(brush, rect);
				}
			}
			for(int i=0; i<toRemove.Count; i++){
				data.RemoveAt(toRemove[i]-i);
			}
		}
		bit.RotateFlip(RotateFlipType.Rotate180FlipX);
		ImageConverter imgconv = new ImageConverter();
		byte[] hmapbytes = (byte[])imgconv.ConvertTo(bit, typeof(byte[]));
		heightMap.LoadImage(hmapbytes);
		heightMap.Apply();
		SaveToDisk(heightMap, "heightmap_" + Vars.TERRAIN_NAME);
		return heightMap;
	}

	/// <summary>
	/// Saves texture to disk memory.
	/// </summary>
	/// <param name="img">Image.</param>
	/// <param name="filename">Filename.</param>
	public void SaveToDisk(Texture2D img, string filename) {
		byte[] bytes = img.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/Heightmaps/Images/" + filename + ".png", bytes);
	}
	
	public Texture2D GetHillsColormap(double heightMapWidth,
	                                 double heightMapHeight,
	                                 ColorSpectrumObj spectrum){
		
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		hmapWidth = heightMapWidth;
		hmapHeight = heightMapHeight;
		double[] bnds= dataHandler.GetBounds();
		lowerBound = bnds[0];
		upperBound = bnds[1];

		UnityEngine.Color[] colors = new UnityEngine.Color[]{UnityEngine.Color.red,
			UnityEngine.Color.red,
			UnityEngine.Color.magenta,
			UnityEngine.Color.black,
			UnityEngine.Color.cyan,
			UnityEngine.Color.blue,
			UnityEngine.Color.blue};
		float[] proportions = new float[]{0.0f,
			(float)((-lowerBound)/(upperBound-lowerBound))/2f,
			(float)((-lowerBound)/(upperBound-lowerBound))-0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound)),
			(float)((-lowerBound)/(upperBound-lowerBound))+0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound))+(1-(float)((-lowerBound)/(upperBound-lowerBound)))/2f,
			1.0f};
		
		this.spectrum = new ColorSpectrumObj(colors, proportions);
		Texture2D colorMap = new Texture2D((int)heightMapWidth, (int)heightMapHeight);
		ColormapGenerator colorGen = new ColormapGenerator(Vars, filename, Vars.MAX_RADIUS, Vars.MIN_X,Vars.MIN_Z,Vars.MAX_X,Vars.MAX_Z, this.spectrum);
		colorMap = colorGen.GetHeightmap(heightMapWidth, heightMapHeight, dataHandler.GetData(), lowerBound, upperBound);
		SaveToDisk(colorMap, "colormap_" + Vars.TERRAIN_NAME);
		
		return colorMap;
	}

	public Texture2D GetHillsColormapCylinder(double heightMapWidth,
	                                  double heightMapHeight,
	                                          ColorSpectrumObj spectrum){
		DataHandler dataHandler = new DataHandler(filename, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, minX, minZ, maxX, maxZ);
		hmapWidth = heightMapWidth;
		hmapHeight = heightMapHeight;
		double[] bnds= dataHandler.GetBounds();
		lowerBound = bnds[0];
		upperBound = bnds[1];

		UnityEngine.Color[] colors = new UnityEngine.Color[]{UnityEngine.Color.red,
			UnityEngine.Color.red,
			UnityEngine.Color.magenta,
			UnityEngine.Color.black,
			UnityEngine.Color.cyan,
			UnityEngine.Color.blue,
			UnityEngine.Color.blue};
		float[] proportions = new float[]{0.0f,
			(float)((-lowerBound)/(upperBound-lowerBound))/2f,
			(float)((-lowerBound)/(upperBound-lowerBound))-0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound)),
			(float)((-lowerBound)/(upperBound-lowerBound))+0.005f,
			(float)((-lowerBound)/(upperBound-lowerBound))+(1-(float)((-lowerBound)/(upperBound-lowerBound)))/2f,
			1.0f};
		
		this.spectrum = new ColorSpectrumObj(colors, proportions);
		Texture2D colorMap = new Texture2D((int)heightMapWidth, (int)heightMapHeight);
		ColormapGenerator colorGen = new ColormapGenerator(Vars, filename, Vars.MAX_RADIUS, Vars.MIN_X,Vars.MIN_Z,Vars.MAX_X,Vars.MAX_Z, this.spectrum);
		colorMap = colorGen.GetHeightmapCylinder(heightMapWidth, heightMapHeight, dataHandler.GetData(), lowerBound, upperBound);
		SaveToDisk(colorMap, "colormap_" + Vars.TERRAIN_NAME);
		
		return colorMap;
	}

	protected void FillInColor(UnityEngine.Color c){
		for(int i = 0; i < heightMap.width; i++) {
			for(int j = 0; j < heightMap.height; j++) {
				heightMap.SetPixel(i,j, c);
			}
		}
	}

	/// <summary>
	/// Performs the same function as <see cref="ColorFromHeight"/> 
	/// however returns a System.Drawing.Color instead of a Unity
	/// Color object.
	/// </summary>
	/// <returns>The color associated with the height from 
	/// the designated ColorSpectrumObj.</returns>
	/// <param name="val">Height value.</param>
	public System.Drawing.Color ColourConvert(double val)
	{
		UnityEngine.Color unityC = GrayscaleFromHeight(val, upperBound, lowerBound);
		UnityEngine.Color32 unicolour = new UnityEngine.Color(unityC.r, unityC.g, unityC.b, unityC.a);
		System.Drawing.Color wincolour = new System.Drawing.Color();
		wincolour = System.Drawing.Color.FromArgb(unicolour.a, unicolour.r, unicolour.g, unicolour.b);
		return wincolour;
	}

	double[,] map;
	bool[,] map_pump;
	bool[,] map_nonzero;
	bool[,] map_nonzero_temp;
	double[,] map_temp;
	protected Texture2D CreateHeightMap(List<double[]> data){ 
		/// Parallel Processing
		int numCores = SystemInfo.processorCount;
		
		// convert list of double[] to list of Vector3's and sort it. 
		List<Vector3> points = new List<Vector3>();
		for(int i=0; i<data.Count; i++){
			double[] t = data[i];
			points.Add(new Vector3((float)t[0], (float)Math.Abs(t[1]), (float)t[2]));
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
					map[a,b] = 0f;//(-lowerBound)/(upperBound-lowerBound);
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
		return heightMap;
	}
	protected Texture2D CreateColorMap(List<double[]> data){ 
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
			heightMap.SetPixel(x,z,ColorFromHeight(p.y, upperBound, lowerBound));
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
				heightMap.SetPixel(x,z,ColorFromHeight(map[x, z], maxC, minC)); 
			}
		}
		
		heightMap.Apply(); 
		return heightMap;
	}
	
	protected UnityEngine.Color ColorFromHeight(double height, double maxHeight, double minHeight) 
	{ 
		double scalar = (height-minHeight) / (maxHeight-minHeight);
		UnityEngine.Color val = spectrum.GetColorFromVal((float)scalar);
		return val;
	}
	
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
				
				avg = (div != 0) ? avg/div : 0f;
				map_temp[x,z] = avg;
				map_nonzero_temp[x, z] = (avg != 0f) ? true : false;

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
					
					avg = (div != 0) ? avg/div : 0f;
					map_temp[x,z] = avg;
					map_nonzero_temp[x, z] = (avg != 0f) ? true : false;
				}
			}
		}
	}
}