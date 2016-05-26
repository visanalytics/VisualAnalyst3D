using UnityEngine;
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class HeightmapInflater : MonoBehaviour {
	
	public Terrain theterrain;
	public bool CreateTerrain = false;
	public bool CreateColourMap = false;
	SplatPrototype[] fatsplat;
	double alphamulti;
	
	public Camera MainCamera;
	public GameObject Flag;
	public bool TerrainVisible;
	public Terrain Map;
	public bool MapVisible;

	public GameObject GridObject;
	public Material GridTransparentMat;
	public Material GridSolidMat;

	public GameObject DataPoint;
	public GameObject AxisLabel;
	public GameObject AxisGrid;
	public GameObject PlayerObject;

	public Variables Vars;
	public DataHandler dH;

	static float TerrainHeightDefault = 150f;

	void Start () {
		InitTerrain();
		TerrainVisible = true;
		MapVisible = true;
	}

	void Awake(){
		TerrainVisible = true;
		MapVisible = true;
	}

	/// <summary>
	/// Initialises the terrain.
	/// </summary>
	private void InitTerrain(){
		//Initialize MultiDimensional array for setting heights based on data values
		GenerateHeightMapMesh("Victoria","Smooth","Age","Peaks", false, 0.4f);
	}

	/// <summary>
	/// Generates the height map with terrain type Granular.
	/// </summary>
	/// <param name="source">Data Source to be found in presets (e.g. Victoria, Time...).</param>
	/// <param name="terrainType">Terrain type (i.e. Granular).</param>
	/// <param name="dataname">Dataname (e.g. Totals, Age ...).</param>
	/// <param name="preset">Preset (i.e. Peaks, Hills).</param>
	/// <param name="RegenerateTerrain">If set to <c>true</c>, terrain will be generated from scratch
	/// rather than simply loaded.</param>
	/// <param name="ScaleY">Height of the terrain to be generated (max height).</param>
	public void GenerateHeightMapHills(String source, String terrainType, String dataname, String preset, bool RegenerateTerrain, float ScaleY){

		Vars = VariablesPresets.VariablePreset(source, terrainType, dataname, preset);
		Vars.SCALE_Y = 1/ScaleY;
		
		dH = new DataHandler(Application.dataPath + Vars.FILENAME, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, Vars.MIN_Z, Vars.MIN_X, Vars.MAX_Z, Vars.MAX_X, Vars);

		Texture2D heightMap;
		Texture2D colorMap;
		heightMap = new Texture2D((int)Vars.HEIGHTMAP_SIZE, (int)Vars.HEIGHTMAP_SIZE);
		colorMap = new Texture2D((int)Vars.COLORMAP_SIZE, (int)Vars.COLORMAP_SIZE);
		
		// Set background of Map 
		Texture2D MapTex = new Texture2D((int)2, (int)2);
		byte[] mTextBytes = File.ReadAllBytes(Application.dataPath + Vars.MAP_FILENAME);
		MapTex.LoadImage(mTextBytes);
		Texture2D NewTex = ConvertToSquareTex(MapTex,2048,2048);
		Map.terrainData.splatPrototypes = null;
		SplatPrototype[] MapSplat = new SplatPrototype[1];
		MapSplat[0] = new SplatPrototype();
		MapSplat[0].texture = NewTex;
		MapSplat[0].tileSize = new Vector2((int)Map.terrainData.size.x, (int)Map.terrainData.size.z);
		Map.terrainData.splatPrototypes = MapSplat;

		if(!RegenerateTerrain){
			try{
				//Load heightmaps into textures
				byte[] hMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/heightmap_" + Vars.TERRAIN_NAME + ".png");
				byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/colormap_" + Vars.TERRAIN_NAME + ".png");
				heightMap.LoadImage(hMapBytes);
				colorMap.LoadImage(cMapBytes);
			}catch(Exception e){
				if(dataname.Contains("Sex") || dataname.Contains("Gender")){
					heightMap = GetHillsSexHeightmap(Vars);
					colorMap = GetHillsSexColormap(Vars);
				}else{
					heightMap = GetHillsHeightmap(Vars);
					colorMap = GetHillsColormap(Vars, heightMap);
				}
			}
		}else{
			if(dataname.Contains("Sex") || dataname.Contains("Gender")){
				heightMap = GetHillsSexHeightmap(Vars);
				colorMap = GetHillsSexColormap(Vars);
			}else{
				heightMap = GetHillsHeightmap(Vars);
				colorMap = GetHillsColormap(Vars, heightMap);
			}
		}

		//Initialize MultiDimensional array for setting heights based on data values 
		float[,] terrainheight = new float[heightMap.width,heightMap.height];
		
		//Terrain Variable
		TerrainData td = theterrain.terrainData;
		td.heightmapResolution = (new int[]{heightMap.width, heightMap.height}).Max();

		//Colour image background
		for (int i = heightMap.width-1; i >= 0; i--){
			for (int j = heightMap.height-1; j >= 0; j--){
				//Insert corresponding value into heights array at specific points for x,y
				Color32 col = heightMap.GetPixel(j, i);
				Int32 mColor = col.r * 256*256 + col.g * 256 + col.b;
				float heightVal = (float)mColor / (float)Math.Pow(2, 24);

				terrainheight[i, j] = heightVal;
			}
		}
		
		//Resize terrain to fit map
		Vector3 newSize = new Vector3(Map.terrainData.size.x*(MapTex.width/2048f), TerrainHeightDefault / Vars.SCALE_Y, Map.terrainData.size.z*(MapTex.height/2048f));
		theterrain.terrainData.size = newSize;
		
		// Add Texture to terrain
		td.splatPrototypes = null;
		SplatPrototype[] terrainTexture = new SplatPrototype[1];
		terrainTexture[0] = new SplatPrototype();
		terrainTexture[0].texture = colorMap;
		terrainTexture[0].tileSize = new Vector2(Map.terrainData.size.x*(MapTex.width/2048f), Map.terrainData.size.z*(MapTex.height/2048f));
		td.splatPrototypes = terrainTexture;
		
		//Alter Terrain heights to values set in the MD array 
		if(CreateTerrain)
			td.SetHeights(0, 0, terrainheight);

		//Garbage Collection
		System.GC.Collect();
		Texture2D[] unusedTextures = FindObjectsOfType<Texture2D>();
		for(int i=0; i<unusedTextures.Length; i++){
			Destroy(unusedTextures[i]);
		}
	}

	/// <summary>
	/// Generates the height map with terrain type Smooth.
	/// </summary>
	/// <param name="source">Data Source to be found in presets (e.g. Victoria, Time...).</param>
	/// <param name="terrainType">Terrain type (i.e. Smooth).</param>
	/// <param name="dataname">Dataname (e.g. Totals, Age ...).</param>
	/// <param name="preset">Preset (i.e. Peaks, Hills).</param>
	/// <param name="RegenerateTerrain">If set to <c>true</c>, terrain will be generated from scratch
	/// rather than simply loaded.</param>
	/// <param name="ScaleY">Height of the terrain to be generated (max height).</param>
	public void GenerateHeightMapMesh(String source, String terrainType, String dataname, String preset, bool RegenerateTerrain, float ScaleY){
		
		Vars = VariablesPresets.VariablePreset(source, terrainType, dataname, preset);
		Vars.SCALE_Y = 1/ScaleY;
		
		dH = new DataHandler(Application.dataPath + Vars.FILENAME, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, Vars.MIN_Z, Vars.MIN_X, Vars.MAX_Z, Vars.MAX_X, Vars);

		Texture2D heightMap;
		Texture2D colorMap;
		heightMap = new Texture2D((int)Vars.HEIGHTMAP_SIZE, (int)Vars.HEIGHTMAP_SIZE);
		colorMap = new Texture2D((int)Vars.COLORMAP_SIZE, (int)Vars.COLORMAP_SIZE);

		// Set background of Map
		Texture2D MapTex = new Texture2D((int)2, (int)2);
		byte[] mTextBytes = File.ReadAllBytes(Application.dataPath + Vars.MAP_FILENAME);
		MapTex.LoadImage(mTextBytes);
		Texture2D NewTex = ConvertToSquareTex(MapTex,2048,2048);
		Map.terrainData.splatPrototypes = null;
		SplatPrototype[] MapSplat = new SplatPrototype[1];
		MapSplat[0] = new SplatPrototype();
		MapSplat[0].texture = NewTex;
		MapSplat[0].tileSize = new Vector2((int)Map.terrainData.size.x, (int)Map.terrainData.size.z);
		Map.terrainData.splatPrototypes = MapSplat;

		if(!RegenerateTerrain){
			try{
				byte[] hMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/GA_" + Vars.TERRAIN_NAME + ".png");
				byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/GA_Color_" + Vars.TERRAIN_NAME + ".png");
				heightMap.LoadImage(hMapBytes);
				colorMap.LoadImage(cMapBytes);
			}catch(Exception e){
				if(dataname.Contains("Sex") || dataname.Contains("Gender")){
					heightMap = GetMeshSexHeightmap(Vars);
					colorMap = GetMeshSexColormap(Vars);
				}else{
					heightMap = GetMeshHeightmap(Vars);
					colorMap = GetMeshColormap(Vars,heightMap);
				}
			}
		}else{
			if(dataname.Contains("Sex") || dataname.Contains("Gender")){
				heightMap = GetMeshSexHeightmap(Vars);
				colorMap = GetMeshSexColormap(Vars);
			}else{
				heightMap = GetMeshHeightmap(Vars);
				colorMap = GetMeshColormap(Vars,heightMap);
			}
		}

		//Initialize MultiDimensional array for setting heights based on data values 
		float[,] terrainheight = new float[heightMap.width,heightMap.height];
		
		//Terrain Variable
		TerrainData td = theterrain.terrainData;
		td.heightmapResolution = (new int[]{heightMap.width, heightMap.height}).Max();
		
		//Colour image background
		for (int i = heightMap.width-1; i >= 0; i--){
			for (int j = heightMap.height-1; j >= 0; j--){
				//Insert corresponding value into heights array at specific points for x,y
				Color32 col = heightMap.GetPixel(j, i);
				Int32 mColor = col.r * 256*256 + col.g * 256 + col.b;
				float heightVal = (float)mColor / (float)Math.Pow(2, 24);

				terrainheight[i, j] = heightVal;
			}
		}

		//Resize terrain to fit map
		Vector3 newSize = new Vector3(Map.terrainData.size.x*(MapTex.width/2048f), TerrainHeightDefault / Vars.SCALE_Y, Map.terrainData.size.z*(MapTex.height/2048f));
		theterrain.terrainData.size = newSize;
		
		// Add Texture to terrain 
		td.splatPrototypes = null;
		SplatPrototype[] terrainTexture = new SplatPrototype[1];
		terrainTexture[0] = new SplatPrototype();
		terrainTexture[0].texture = colorMap;
		terrainTexture[0].tileSize = new Vector2(Map.terrainData.size.x*(MapTex.width/2048f), Map.terrainData.size.z*(MapTex.height/2048f));
		td.splatPrototypes = terrainTexture;
		
		//Alter Terrain heights to values set in the MD array
		if(CreateTerrain)
			td.SetHeights(0, 0, terrainheight);

		//Garbage Collection
		System.GC.Collect();
		Texture2D[] unusedTextures = FindObjectsOfType<Texture2D>();
		for(int i=0; i<unusedTextures.Length; i++){
			Destroy(unusedTextures[i]);
		}
	}

	/// <summary>
	/// Generates the height map with terrain type Cylindrical.
	/// </summary>
	/// <param name="source">Data Source to be found in presets (e.g. Victoria, Time...).</param>
	/// <param name="terrainType">Terrain type (i.e. Cylindrical).</param>
	/// <param name="dataname">Dataname (e.g. Totals, Age ...).</param>
	/// <param name="preset">Preset (i.e. Cylindrical).</param>
	/// <param name="RegenerateTerrain">If set to <c>true</c>, terrain will be generated from scratch
	/// rather than simply loaded.</param>
	/// <param name="ScaleY">Height of the terrain to be generated (max height).</param>
	public void GenerateHeightMapCylinders(String source, String terrainType, String dataname, String preset, bool RegenerateTerrain, float ScaleY){
		Vars = VariablesPresets.VariablePreset(source, terrainType, dataname, preset);
		Vars.SCALE_Y = 1/ScaleY;
		
		dH = new DataHandler(Application.dataPath + Vars.FILENAME, Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z, Vars.MIN_Z, Vars.MIN_X, Vars.MAX_Z, Vars.MAX_X, Vars);
		
		Texture2D heightMap;
		Texture2D colorMap;
		heightMap = new Texture2D((int)Vars.HEIGHTMAP_SIZE, (int)Vars.HEIGHTMAP_SIZE);
		colorMap = new Texture2D((int)Vars.COLORMAP_SIZE, (int)Vars.COLORMAP_SIZE);
		
		// Set background of Map 
		Texture2D MapTex = new Texture2D((int)2, (int)2);
		byte[] mTextBytes = File.ReadAllBytes(Application.dataPath + Vars.MAP_FILENAME);
		MapTex.LoadImage(mTextBytes);
		Texture2D NewTex = ConvertToSquareTex(MapTex,2048,2048);
		Map.terrainData.splatPrototypes = null;
		SplatPrototype[] MapSplat = new SplatPrototype[1];
		MapSplat[0] = new SplatPrototype();
		MapSplat[0].texture = NewTex;
		MapSplat[0].tileSize = new Vector2((int)Map.terrainData.size.x, (int)Map.terrainData.size.z);
		Map.terrainData.splatPrototypes = MapSplat;
		
		if(!RegenerateTerrain){
			try{
				//Load heightmaps into textures
				byte[] hMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/heightmap_" + Vars.TERRAIN_NAME + ".png");
				byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/colormap_" + Vars.TERRAIN_NAME + ".png");
				heightMap.LoadImage(hMapBytes);
				colorMap.LoadImage(cMapBytes);
			}catch(Exception e){
				if(dataname.Contains("Sex") || dataname.Contains("Gender")){
					heightMap = GetHillsSexHeightmapCylinder(Vars);
					colorMap = GetHillsSexColormapCylinder(Vars);
				}else{
					heightMap = GetHillsHeightmapCylinder(Vars);
					colorMap = GetHillsColormap(Vars, heightMap);
				}
			}
		}else{
			if(dataname.Contains("Sex") || dataname.Contains("Gender")){
				heightMap = GetHillsSexHeightmapCylinder(Vars);
				colorMap = GetHillsSexColormapCylinder(Vars);
			}else{
				heightMap = GetHillsHeightmapCylinder(Vars);
				colorMap = GetHillsColormap(Vars, heightMap);
			}
		}
		
		//Initialize MultiDimensional array for setting heights based on data values 
		float[,] terrainheight = new float[heightMap.width,heightMap.height];
		
		//Terrain Variable
		TerrainData td = theterrain.terrainData;
		td.heightmapResolution = (new int[]{heightMap.width, heightMap.height}).Max();
		
		//Colour image background
		for (int i = heightMap.width-1; i >= 0; i--){
			for (int j = heightMap.height-1; j >= 0; j--){
				//Insert corresponding value into heights array at specific points for x,y
				Color32 col = heightMap.GetPixel(j, i);
				Int32 mColor = col.r * 256*256 + col.g * 256 + col.b;
				float heightVal = (float)mColor / (float)Math.Pow(2, 24);
				
				terrainheight[i, j] = heightVal;
			}
		}
		
		//Resize terrain to fit map
		Vector3 newSize = new Vector3(Map.terrainData.size.x*(MapTex.width/2048f), TerrainHeightDefault / Vars.SCALE_Y, Map.terrainData.size.z*(MapTex.height/2048f));
		theterrain.terrainData.size = newSize;
		
		// Add Texture to terrain
		td.splatPrototypes = null;
		SplatPrototype[] terrainTexture = new SplatPrototype[1];
		terrainTexture[0] = new SplatPrototype();
		terrainTexture[0].texture = colorMap;
		terrainTexture[0].tileSize = new Vector2(Map.terrainData.size.x*(MapTex.width/2048f), Map.terrainData.size.z*(MapTex.height/2048f));
		td.splatPrototypes = terrainTexture;
		
		//Alter Terrain heights to values set in the MD array 
		if(CreateTerrain)
			td.SetHeights(0, 0, terrainheight);
		
		//Garbage Collection
		System.GC.Collect();
		Texture2D[] unusedTextures = FindObjectsOfType<Texture2D>();
		for(int i=0; i<unusedTextures.Length; i++){
			Destroy(unusedTextures[i]);
		}
	}

	#region Heightmap/colormap generation functions

	private Texture2D GetMeshHeightmap(Variables Vars){
		MeshTerrain mt = new MeshTerrain(Vars, Application.dataPath + Vars.FILENAME,
		                                 Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z); 
		return mt.GetHeightmap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE,
		                            new ColorSpectrumObj(Vars.COLOR_PRESET));
	}

	private Texture2D GetHillsHeightmap(Variables Vars){
		HeightmapGenerator heightGenerator = new HeightmapGenerator(Vars,
		                                                            Application.dataPath + Vars.FILENAME,
		                                                            Vars.MAX_RADIUS,
		                                                            Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return heightGenerator.GetHeightmap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE, false);
	}

	private Texture2D GetHillsHeightmapCylinder(Variables Vars){
		HeightmapGenerator heightGenerator = new HeightmapGenerator(Vars,
		                                                            Application.dataPath + Vars.FILENAME,
		                                                            Vars.MAX_RADIUS,
		                                                            Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return heightGenerator.GetHeightmapCylinder(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE, false);
	}

	private Texture2D GetMeshColormap(Variables Vars, Texture2D heightmap){
		ColormapGen cGen = new ColormapGen(heightmap, new ColorSpectrumObj(Vars.COLOR_PRESET), Application.dataPath + "/Heightmaps/Images/GA_Color_" + Vars.TERRAIN_NAME + ".png");
		return cGen.GetColormap();
	}

	private Texture2D GetHillsColormap(Variables Vars, Texture2D heightmap){
		ColormapGen cGen = new ColormapGen(heightmap, new ColorSpectrumObj(Vars.COLOR_PRESET), Application.dataPath + "/Heightmaps/Images/colormap_" + Vars.TERRAIN_NAME + ".png");
		return cGen.GetColormap();
	}

	#region Sex/gender terrain generation
	private Texture2D GetMeshSexHeightmap(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetMeshHeightmap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE);
	}

	private Texture2D GetMeshSexColormap(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetMeshColormap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE, new ColorSpectrumObj(Vars.COLOR_PRESET));
	}

	private Texture2D GetHillsSexHeightmap(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetHillsHeightmap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE);
	}

	private Texture2D GetHillsSexColormap(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetHillsColormap(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE, new ColorSpectrumObj(Vars.COLOR_PRESET));
	}

	private Texture2D GetHillsSexHeightmapCylinder(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetHillsSexHeightmapCylinder(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE);
	}

	private Texture2D GetHillsSexColormapCylinder(Variables Vars){
		SexHeightmapGenerator shg = new SexHeightmapGenerator(Vars, Application.dataPath + Vars.FILENAME, Vars.MAX_RADIUS,
		                                                      Vars.MIN_X, Vars.MIN_Z, Vars.MAX_X, Vars.MAX_Z);
		return shg.GetHillsColormapCylinder(Vars.HEIGHTMAP_SIZE, Vars.HEIGHTMAP_SIZE, new ColorSpectrumObj(Vars.COLOR_PRESET));
	}
	#endregion

	#endregion

	public void HideTerrain(){
		theterrain.enabled = false;
		theterrain.collider.enabled = false;
		TerrainVisible = false;
	}
	public void UnhideTerrain(){
		theterrain.enabled = true;
		theterrain.collider.enabled = true;
		TerrainVisible = true;
	}

	public void HideMap(){
		Map.enabled = false;
		Map.collider.enabled = false;
		MapVisible = false;
	}
	public void UnhideMap(){
		Map.enabled = true;
		Map.collider.enabled = true;
		MapVisible = true;
	}

	/// <summary>
	/// Returns the associated [X,Y,Z] coordinates in the
	/// bounds of the data associated with given coordinates
	/// in the Unity world.
	/// </summary>
	/// <returns>The position in the bounds of the data.</returns>
	/// <param name="worldCoords">World coords.</param>
	public Vector3 DataPosFromWorld(Vector3 worldCoords){
		double worldMaxX = theterrain.terrainData.size.x;
		double worldMaxZ = theterrain.terrainData.size.z;
		double ratioX = worldCoords.x/worldMaxX;
		double ratioZ = (worldMaxZ-worldCoords.z)/worldMaxZ;
		double dataX = (ratioX*(Vars.MAX_Z-Vars.MIN_Z)) + Vars.MIN_Z;
		double dataZ = (ratioZ*(Vars.MAX_X-Vars.MIN_X)) + Vars.MIN_X;
		double lower = Vars.TERRAIN_NAME.Contains("Sex") ? 0f : 0f;
		double ratioY = (double)(dH.GetMaxY()-lower)/theterrain.terrainData.size.y;
		double dataY = ratioY*theterrain.SampleHeight(worldCoords) + lower;
		return new Vector3((float)dataX,(float)dataY,(float)dataZ);
	}

	/// <summary>
	/// Given squareWidth and squareHeight are greater than the 
	/// respect sizes of the src texture, it will draw the texture
	/// on to a larger square texture for application to terrain.
	/// </summary>
	/// <returns>A square texture with the N x M texture on it..</returns>
	/// <param name="src">Source texture.</param>
	/// <param name="squareWidth">Square texture width.</param>
	/// <param name="squareHeight">Square texture height.</param>
	private Texture2D ConvertToSquareTex(Texture2D src, int squareWidth, int squareHeight){
		Texture2D newImage = new Texture2D(squareWidth, squareHeight);
		for(int x=0; x<squareWidth; x++){
			for(int y=0; y<squareHeight; y++){
				newImage.SetPixel(x,y, UnityEngine.Color.black);
			}
		}
		newImage.Apply();
		for(int x=0; x<src.width; x++){
			for(int y=0; y<src.height; y++){
				newImage.SetPixel(x,y,src.GetPixel(x,y));
			}
		}
		newImage.Apply();
		return newImage;
	}

	/// <summary>
	/// Used by the button in 'Settings' to regenerate all terrains.
	/// Should only be used when terrain generation algorithms are changed.
	/// </summary>
	public void GenerateAllTerrains(){
		StartCoroutine("GenerateAllTerrainsCoroutine");
	}

	/// <summary>
	/// Generates all terrains.
	/// </summary>
	private IEnumerator GenerateAllTerrainsCoroutine(){
		TimeSpan diff;
		DateTime before = DateTime.Now;
		DateTime after;
		
		DataTable table = VariablesPresets.GetTableFromCSV();
		foreach(DataRow row in table.Rows){
			DateTime beforeEach = DateTime.Now;
			if((string)row["TerrainType"] == "Granular"){
				GenerateHeightMapHills((string)row["Source"],
				                       (string)row["TerrainType"],
				                       (string)row["Data"],
				                       (string)row["Preset"],
				                       true,
				                       1);
			}
			else if((string)row["TerrainType"] == "Smooth"){
				GenerateHeightMapMesh((string)row["Source"],
				                      (string)row["TerrainType"],
				                      (string)row["Data"],
				                      (string)row["Preset"],
				                      true,
				                      1);
			}
			else if((string)row["TerrainType"] == "Cylindrical"){
				GenerateHeightMapCylinders((string)row["Source"],
				                    (string)row["TerrainType"],
				                    (string)row["Data"],
				                    (string)row["Preset"],
				                    true,
				                    1);
			}
			DateTime afterEach = DateTime.Now;
			diff = afterEach.Subtract(beforeEach);
			Debug.Log("Time Taken for " + (string)row["Source"] + 
			          (string)row["TerrainType"] + 
			          (string)row["Data"] +
			          (string)row["Preset"]  + ": " + diff.TotalSeconds.ToString());
			yield return new WaitForSeconds(2f);
		}
		
		after = DateTime.Now;
		diff = after.Subtract(before);
		Debug.Log("Time Taken for Mesh Gen: " + diff.TotalSeconds.ToString());
	}
}