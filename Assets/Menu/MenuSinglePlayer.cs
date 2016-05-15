using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class MenuSinglePlayer
{
	MenuHandler Handler;
	float w = Screen.width;
	float h = Screen.height;
	// "tile" width and height, for easy GUI creation 
	float tw = 100f;
	float th = 20f;

	#region Combo Boxes
	GUIStyle presetsStyle;
	GUIStyle fNameStyle;
	GUIStyle tTypesStyle;
	GUIStyle dSourceStyle;
	
	GUIContent[] dSourceList;
	GUIContent[] tTypesList;
	GUIContent[] fNameList;
	GUIContent[] presetsList;
	
	// Combo boxes for terrain selectioon
	ComboBox datasourcesCombo; // e.g. VicAll, CitySurr, etc.
	ComboBox terraintypesCombo; // e.g. granular (was hills), smooth (was mesh)
	ComboBox filenamesCombo; // e.g. age, totals, fatalities, etc.
	ComboBox presetsCombo; // e..g peaks (was needles), landscape (was hills), bars

	string CurrentDataSource,
	CurrentTerrainType,
	CurrentFilename,
	CurrentPreset;

	#endregion

	//Variables for Spectrum Slider
	Terrain currentterrain;
	Color[,] splatmapcolours;
	Texture2D currenttexture;

	// Button for regeneration of terrain is created in OnGUI
	//bool RegenerateTerrainToggle = false;
	float ScaleYSliderVal = 0.5f;
	
	//Slider Colouring of Map
	float ScaleYSliderValcolour = 1f;
	float[, ,] tspmpdata; 

	public MenuSinglePlayer (MenuHandler handler)
	{
		this.Handler = handler;
		this.currentterrain = Handler.Inflater.theterrain;
		InitGui();
	}

	public void OnGUI(){
		
		GUI.Box(new Rect(0,0, w, th*5),"");
		GUI.Box(new Rect(0,0, w, th*5),"");
		
		GUI.Label(new Rect(tw*0.5f, th*0.5f, tw, th), "Data Set");
		datasourcesCombo.Show();
		GUI.Label(new Rect(tw*1.5f, th*0.5f, tw*1.5f, th), "Property");
		filenamesCombo.Show();
		GUI.Label(new Rect(tw*3f, th*0.5f, tw, th), "Terrain Type");
		terraintypesCombo.Show();
		GUI.Label(new Rect(tw*4f, th*0.5f, tw, th), "View");
		presetsCombo.Show();
		//RegenerateTerrainToggle = GUI.Toggle(new Rect(tw*6.65f, th*1.5f, tw*3f, th), RegenerateTerrainToggle, "Regenerate Terrain? Could take a long time");
		
		
		GUI.Label(new Rect(tw*5.25f, th*2.75f, tw, th), "Max Height: ");
		GUI.Label(new Rect(tw*6f, th*3.75f, tw, th), "0.0");
		GUI.Label(new Rect(tw*7f, th*3.75f, tw, th), "1.0");
		MenuHandler.GUIDrawRect(new Rect(tw*6f, th*2.9f, tw*1.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		ScaleYSliderVal = GUI.HorizontalSlider(new Rect(tw*6.1f, th*3f, tw*1, th), ScaleYSliderVal, 0.01f, 0.8f);

		if(GUI.Button(new Rect(tw*7f, th*1.5f, tw*1.5f, th), "Reset Colour Shift")){
			ScaleYSliderValcolour = 1f;
		}
		GUI.Label(new Rect(tw*7.50f, th*2.75f, tw, th), "Colour Shift: ");
		GUI.Label(new Rect(tw*8.25f, th*3.75f, tw, th), "0.0");
		GUI.Label(new Rect(tw*9.25f, th*3.75f, tw, th), "2.0");
		MenuHandler.GUIDrawRect(new Rect(tw*8.25f, th*2.9f, tw*1.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		ScaleYSliderValcolour = GUI.HorizontalSlider(new Rect(tw*8.35f, th*3f, tw*1, th), ScaleYSliderValcolour, 0.01f, 2.0f);

		if(GUI.Button(new Rect(tw*5.35f, th*1.5f, tw*1.5f, th), "Generate Terrain")){
			if(Handler.GetState() == MenuHandler.State.StateMultiplayer){
				Handler.Multiplayer.CallChangeTerrain(dSourceList[datasourcesCombo.SelectedItemIndex].text,
				                                      tTypesList[terraintypesCombo.SelectedItemIndex].text,
				                                      fNameList[filenamesCombo.SelectedItemIndex].text,
				                                      presetsList[presetsCombo.SelectedItemIndex].text,
				                                      false,
				                                      ScaleYSliderVal,
				                                      ScaleYSliderValcolour);
			}else{
				
				if(CurrentDataSource != dSourceList[datasourcesCombo.SelectedItemIndex].text
				   || CurrentFilename != fNameList[filenamesCombo.SelectedItemIndex].text){
					Handler.DeleteAllFlags();
					Handler.DeleteAllGrids();
				}

				if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Granular"){
					GenerateGranular(dSourceList[datasourcesCombo.SelectedItemIndex].text,
		                                tTypesList[terraintypesCombo.SelectedItemIndex].text,
		                                fNameList[filenamesCombo.SelectedItemIndex].text,
		                                presetsList[presetsCombo.SelectedItemIndex].text,
		                                false,
		                                ScaleYSliderVal,
		         						ScaleYSliderValcolour);
				}
				else if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Smooth"){
					GenerateSmooth(dSourceList[datasourcesCombo.SelectedItemIndex].text,
					                 tTypesList[terraintypesCombo.SelectedItemIndex].text,
					                 fNameList[filenamesCombo.SelectedItemIndex].text,
					                 presetsList[presetsCombo.SelectedItemIndex].text,
					                 false,
					                 ScaleYSliderVal,
					                 ScaleYSliderValcolour);
				}
				else if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Cylindrical"){
					GenerateCylindrical(dSourceList[datasourcesCombo.SelectedItemIndex].text,
					               tTypesList[terraintypesCombo.SelectedItemIndex].text,
					               fNameList[filenamesCombo.SelectedItemIndex].text,
					               presetsList[presetsCombo.SelectedItemIndex].text,
					               false,
					               ScaleYSliderVal,
					               ScaleYSliderValcolour);
				}

				CurrentDataSource = dSourceList[datasourcesCombo.SelectedItemIndex].text;
				CurrentTerrainType = tTypesList[terraintypesCombo.SelectedItemIndex].text;
				CurrentFilename = fNameList[filenamesCombo.SelectedItemIndex].text;
				CurrentPreset = presetsList[presetsCombo.SelectedItemIndex].text;

				Handler.UpdateFlags();
				Handler.UpdateGrids();
				Handler.MoveCamHome();
				
				//Variables for Spectrum Slider
				currentterrain = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
				StoreSplatColours(currentterrain, ref splatmapcolours, ref currenttexture);
			}
		}
	}

	public void RegenerateCurrentTerrainMulti(){
		Handler.Multiplayer.CallChangeTerrain(dSourceList[datasourcesCombo.SelectedItemIndex].text,
		                                      tTypesList[terraintypesCombo.SelectedItemIndex].text,
		                                      fNameList[filenamesCombo.SelectedItemIndex].text,
		                                      presetsList[presetsCombo.SelectedItemIndex].text,
		                                      true,
		                                      ScaleYSliderVal,
		                                      ScaleYSliderValcolour);
	}
	public void RegenerateCurrentTerrainSingle(){
		if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Granular"){
			GenerateGranular(dSourceList[datasourcesCombo.SelectedItemIndex].text,
			                 tTypesList[terraintypesCombo.SelectedItemIndex].text,
			                 fNameList[filenamesCombo.SelectedItemIndex].text,
			                 presetsList[presetsCombo.SelectedItemIndex].text,
			                 true,
			                 ScaleYSliderVal,
			                 ScaleYSliderValcolour);
		}
		else if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Smooth"){
			GenerateSmooth(dSourceList[datasourcesCombo.SelectedItemIndex].text,
			               tTypesList[terraintypesCombo.SelectedItemIndex].text,
			               fNameList[filenamesCombo.SelectedItemIndex].text,
			               presetsList[presetsCombo.SelectedItemIndex].text,
			               true,
			               ScaleYSliderVal,
			               ScaleYSliderValcolour);
		}
		else if(tTypesList[terraintypesCombo.SelectedItemIndex].text == "Cylindrical"){
			GenerateCylindrical(dSourceList[datasourcesCombo.SelectedItemIndex].text,
			                    tTypesList[terraintypesCombo.SelectedItemIndex].text,
			                    fNameList[filenamesCombo.SelectedItemIndex].text,
			                    presetsList[presetsCombo.SelectedItemIndex].text,
			                    true,
			                    ScaleYSliderVal,
			                    ScaleYSliderValcolour);
		}
		
		CurrentDataSource = dSourceList[datasourcesCombo.SelectedItemIndex].text;
		CurrentTerrainType = tTypesList[terraintypesCombo.SelectedItemIndex].text;
		CurrentFilename = fNameList[filenamesCombo.SelectedItemIndex].text;
		CurrentPreset = presetsList[presetsCombo.SelectedItemIndex].text;
		
		Handler.UpdateFlags();
		Handler.UpdateGrids();
		Handler.MoveCamHome();
		
		//Variables for Spectrum Slider
		currentterrain = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		StoreSplatColours(currentterrain, ref splatmapcolours, ref currenttexture);
	}
	
	public void GenerateGranular(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		Handler.Inflater.GenerateHeightMapHills(DataSource,
		                                        TerrainType,
		                                        Filename,
		                                        Preset,
		                                        regen,
		                                        scale);
		GenerateSharedCalls(DataSource, TerrainType, Filename, Preset, scale, colorScale);
	}
	public void GenerateSmooth(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		Handler.Inflater.GenerateHeightMapMesh(DataSource,
		                                       TerrainType,
		                                       Filename,
		                                       Preset,
		                                       regen,
		                                       scale);
		GenerateSharedCalls(DataSource, TerrainType, Filename, Preset, scale, colorScale);
	}
	public void GenerateCylindrical(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		Handler.Inflater.GenerateHeightMapCylinders(DataSource,
		                                       TerrainType,
		                                       Filename,
		                                       Preset,
		                                       regen,
		                                       scale);
		GenerateSharedCalls(DataSource, TerrainType, Filename, Preset, scale, colorScale);
	}

	public void RegenerateColorSplat(){
		if(ScaleYSliderValcolour == 1.0f)
			ResetTerrainSplatToOriginal(ref currentterrain,CurrentDataSource,CurrentTerrainType,CurrentFilename,CurrentPreset);
		else{
			Terrain terr = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
			SplatPrototype[] splat = new SplatPrototype[1];
			splat[0] = new SplatPrototype();
			splat[0].texture = LoadColourMap(CurrentDataSource,CurrentTerrainType,CurrentFilename,CurrentPreset);
			splat[0].tileOffset = new Vector2(0, 0);
			splat[0].tileSize = new Vector2(terr.terrainData.size.x, terr.terrainData.size.z);
			terr.terrainData.splatPrototypes = null;
			terr.terrainData.splatPrototypes = splat;
			StoreSplatColours(currentterrain, ref splatmapcolours, ref currenttexture);
			SetSliderSplatColours(ScaleYSliderValcolour, ref currentterrain, splatmapcolours,CurrentDataSource,CurrentTerrainType,CurrentFilename,CurrentPreset);
		}
	}
	private void GenerateColorSplat(float colorScale, string DataSource, string TerrainType, string Filename, string Preset){
		if(colorScale == 1.0f)
			ResetTerrainSplatToOriginal(ref currentterrain,DataSource,TerrainType,Filename,Preset);
		else
			SetSliderSplatColours(colorScale, ref currentterrain, splatmapcolours,DataSource,TerrainType,Filename,Preset);
	}

	private void GenerateSharedCalls(string DataSource, string TerrainType, string Filename, string Preset, float scale, float colorScale){
		CurrentDataSource = DataSource;
		CurrentTerrainType = TerrainType;
		CurrentFilename = Filename;
		CurrentPreset = Preset;
		ScaleYSliderVal = scale;
		ScaleYSliderValcolour = colorScale;
		///
		for(int i=0; i<dSourceList.Length; i++){
			if(dSourceList[i].text == CurrentDataSource){
				datasourcesCombo.SelectedItemIndex = i;
				datasourcesCombo.SetMainString(CurrentDataSource);
			}
		}
		
		UpdateAllAfterSource();

		for(int i=0; i<tTypesList.Length; i++){
			if(tTypesList[i].text == CurrentTerrainType){
				terraintypesCombo.SelectedItemIndex = i;
				terraintypesCombo.SetMainString(CurrentTerrainType);
			}
		}
		for(int i=0; i<fNameList.Length; i++){
			if(fNameList[i].text == CurrentFilename){
				filenamesCombo.SelectedItemIndex = i;
				filenamesCombo.SetMainString(CurrentFilename);
			}
		}
		UpdatePresets();

		for(int i=0; i<presetsList.Length; i++){
			if(presetsList[i].text == CurrentPreset){
				presetsCombo.SelectedItemIndex = i;
				presetsCombo.SetMainString(CurrentPreset);
			}
		}
		///
		StoreSplatColours(currentterrain, ref splatmapcolours, ref currenttexture);
		GenerateColorSplat(colorScale,DataSource,TerrainType,Filename,Preset);
		Handler.Tabs.ResetDataPoints();
		Handler.Tabs.ResetAxisGrid();
		Handler.UpdateFlags();
		Handler.UpdateGrids();
		if(Network.peerType == NetworkPeerType.Client){
			Handler.Multiplayer.CallExternalAxisReset(Handler.Tabs.AxisGridSizeXRounded, Handler.Tabs.AxisGridSizeYRounded, Handler.Tabs.AxisGridSizeZRounded);
		}
	}

	private void InitGui(){
		InitComboBoxLists();
		
		// Create data source dropbox, e.g. vic, city, cbd
		dSourceStyle = new GUIStyle();
		dSourceStyle.normal.textColor = Color.white;
		dSourceStyle.onHover.background = 
			dSourceStyle.hover.background = new Texture2D(2, 2);
		dSourceStyle.padding.left = 
			dSourceStyle.padding.right = 
				dSourceStyle.padding.top = 
				dSourceStyle.padding.bottom = 4;
		datasourcesCombo = new ComboBox(new Rect(tw*0.5f, th*1.5f, tw, th),
		                                dSourceList[0],
		                                dSourceList,
		                                "button",
		                                "box",
		                                dSourceStyle);
		datasourcesCombo.SetOnClick(Handler.ReadyCamHome, UpdateAllAfterSource);
		
		// Data property dropbox, e.g. age, totals, fatalities
		fNameStyle = new GUIStyle();
		fNameStyle.normal.textColor = Color.white;
		fNameStyle.onHover.background = 
			fNameStyle.hover.background = new Texture2D(2, 2);
		fNameStyle.padding.left = 
			fNameStyle.padding.right = 
				fNameStyle.padding.top = 
				fNameStyle.padding.bottom = 4;
		filenamesCombo = new ComboBox(new Rect(tw*1.5f, th*1.5f, tw*1.5f, th),
		                              fNameList[0],
		                              fNameList,
		                              "button",
		                              "box",
		                              fNameStyle);
//		filenamesCombo.SetOnClick();
		
		// Terrain type, e.g. granular, smooth, etc.
		tTypesStyle = new GUIStyle();
		tTypesStyle.normal.textColor = Color.white;
		tTypesStyle.onHover.background = 
			tTypesStyle.hover.background = new Texture2D(2, 2);
		tTypesStyle.padding.left = 
			tTypesStyle.padding.right = 
				tTypesStyle.padding.top = 
				tTypesStyle.padding.bottom = 4;
		terraintypesCombo = new ComboBox(new Rect(tw*3f, th*1.5f, tw, th),
		                                 tTypesList[0],
		                                 tTypesList,
		                                 "button",
		                                 "box",
		                                 tTypesStyle);
		// terraintypesCombo.SetOnClick(); 
		
		// Terrain view style, e.g. peaks, lanscape, etc.
		presetsStyle = new GUIStyle();
		presetsStyle.normal.textColor = Color.white;
		presetsStyle.onHover.background = 
			presetsStyle.hover.background = new Texture2D(2, 2);
		presetsStyle.padding.left = 
			presetsStyle.padding.right = 
				presetsStyle.padding.top = 
				presetsStyle.padding.bottom = 4;
		
		
		CurrentDataSource = dSourceList[datasourcesCombo.SelectedItemIndex].text;
		CurrentTerrainType = tTypesList[terraintypesCombo.SelectedItemIndex].text;
		CurrentFilename = fNameList[filenamesCombo.SelectedItemIndex].text;
		UpdatePresets();
		CurrentPreset = presetsList[presetsCombo.SelectedItemIndex].text;

		// Update the View combo box
		
		// Updates all combo box lists to ensure that all
		UpdateAllAfterSource();
	}
	
	// Gets lists of names for combo boxes from CSV file
	public void InitComboBoxLists(){
		DataTable table = VariablesPresets.GetTableFromCSV();
		List<String> datasources = new List<String>(table.Rows.Count);
		foreach(DataRow row in table.Rows){
			String temp = (String)row["Source"];
			if(!datasources.Contains(temp))
				datasources.Add(temp);
		}
		
		dSourceList = new GUIContent[datasources.Count];
		for(int i=0; i<datasources.Count; i++){
			dSourceList[i] = new GUIContent(datasources[i]);
		}
		
		List<String> terraintypes = new List<String>(table.Rows.Count);
		foreach(DataRow row in table.Rows){
			String temp = (String)row["TerrainType"];
			if(!terraintypes.Contains(temp))
				terraintypes.Add(temp);
		}
		
		tTypesList = new GUIContent[terraintypes.Count];
		for(int i=0; i<terraintypes.Count; i++){
			tTypesList[i] = new GUIContent(terraintypes[i]);
		}
		
		List<String> datatypes = new List<String>(table.Rows.Count);
		foreach(DataRow row in table.Rows){
			String temp = (String)row["Data"];
			if(!datatypes.Contains(temp))
				datatypes.Add(temp);
		}
		
		fNameList = new GUIContent[datatypes.Count];
		for(int i=0; i<datatypes.Count; i++){
			fNameList[i] = new GUIContent(datatypes[i]);
		}
	}

	// Alters the View combo box list when needed
	public void UpdatePresets(){
		DataTable table = VariablesPresets.GetTableFromCSV();
		DataRow[] preset_rows = table.Select("Source = '"+dSourceList[datasourcesCombo.SelectedItemIndex].text+
		                                     "' AND TerrainType = '"+tTypesList[terraintypesCombo.SelectedItemIndex].text+
		                                     "' AND Data = '"+fNameList[filenamesCombo.SelectedItemIndex].text+"'");
		presetsList = new GUIContent[preset_rows.Length];
		for(int i=0; i<preset_rows.Length; i++){
			presetsList[i] = new GUIContent((String)preset_rows[i]["Preset"]);
		}
		presetsCombo = new ComboBox(new Rect(tw*4f, th*1.5f, tw, th),
		                            presetsList[0],
		                            presetsList,
		                            "button",
		                            "box",
		                            presetsStyle);
	}

	// Changes the combo box lists Property, Type and View after the Data Set is changed
	private void UpdateAllAfterSource(){
		
		// Load the presets
		DataTable table = VariablesPresets.GetTableFromCSV();
		DataRow[] source_rows = table.Select("Source = '"+dSourceList[datasourcesCombo.SelectedItemIndex].text+"'");
		List<String> terraintypes = new List<String>(table.Rows.Count);
		foreach(DataRow row in source_rows){
			String temp = (String)row["TerrainType"];
			if(!terraintypes.Contains(temp))
				terraintypes.Add(temp);
		}
		
		// Create a list of terrain types and update the terrain type combo list
		tTypesList = new GUIContent[terraintypes.Count];
		for(int i=0; i<terraintypes.Count; i++){
			tTypesList[i] = new GUIContent(terraintypes[i]);
		} 
		terraintypesCombo = new ComboBox(new Rect(tw*3f, th*1.5f, tw, th),
		                                 tTypesList[0],
		                                 tTypesList,
		                                 "button",
		                                 "box",
		                                 tTypesStyle);
		terraintypesCombo.SetOnClick(UpdatePresets);
		
		// Update the list of data types (properties?)
		List<String> datatypes = new List<String>(table.Rows.Count);
		foreach(DataRow row in source_rows){
			String temp = (String)row["Data"];
			if(!datatypes.Contains(temp))
				datatypes.Add(temp);
		}
		
		fNameList = new GUIContent[datatypes.Count];
		for(int i=0; i<datatypes.Count; i++){
			fNameList[i] = new GUIContent(datatypes[i]);
		}
		filenamesCombo = new ComboBox(new Rect(tw*1.5f, th*1.5f, tw*1.5f, th),
		                              fNameList[0],
		                              fNameList,
		                              "button",
		                              "box",
		                              fNameStyle);
		filenamesCombo.SetOnClick(UpdatePresets); 	
		
		// Update the view presets 
		UpdatePresets();
	} 
	
	public void StoreSplatColours(Terrain t, ref Color[,] c, ref Texture2D origi)
	{
		t = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		Texture2D splatmap = t.terrainData.splatPrototypes[0].texture;
		origi = splatmap;
		c = new Color[splatmap.width, splatmap.height];
		
		for(int i = 0; i < splatmap.width; i++)
			for(int j = 0; j < splatmap.height; j++)
		{
			c[i, j] = splatmap.GetPixel(i, j); //Store splatmap colours into the array
		}
	}

	public void ResetTerrainSplatToOriginal(ref Terrain t)
	{
		ResetTerrainSplatToOriginal(ref t,CurrentDataSource,CurrentTerrainType,CurrentFilename,CurrentPreset);
	}
	public void ResetTerrainSplatToOriginal(ref Terrain t, string DataSource, string TerrainType, string Filename, string Preset)
	{
		Terrain terr = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		
		SplatPrototype[] splat = new SplatPrototype[1];
		splat[0] = new SplatPrototype();
		splat[0].texture = LoadColourMap(DataSource,TerrainType,Filename,Preset);
		splat[0].tileOffset = new Vector2(0, 0);
		splat[0].tileSize = new Vector2(terr.terrainData.size.x, terr.terrainData.size.z);
		terr.terrainData.splatPrototypes = null;
		terr.terrainData.splatPrototypes = splat;
		
		t = terr;
		
		ScaleYSliderValcolour = 1.0f;
	}
	
	public void SetSliderSplatColours(float slider, ref Terrain t, Color[,] c, string DataSource, string TerrainType, string Filename, string Preset)
	{
		if(Filename.Contains("Gender") || Filename.Contains("Sex"))
			return;

		Color[,] temp = c;
		Texture2D tempsplat = LoadColourMap(DataSource,TerrainType,Filename,Preset);
		
		//Contains the colour for the corresponding map
		Handler.Vars = VariablesPresets.VariablePreset(DataSource,//dSourceList[datasourcesCombo.SelectedItemIndex].text, 
		                                       TerrainType, //tTypesList[terraintypesCombo.SelectedItemIndex].text,
		                                       Filename, //fNameList[filenamesCombo.SelectedItemIndex].text,
		                                       Preset);//presetsList[presetsCombo.SelectedItemIndex].text);
		
		ColorSpectrumObj cso = new ColorSpectrumObj(Handler.Vars.COLOR_PRESET);
		
		for(int i = 0; i < temp.GetUpperBound(0); i++)
			for(int j = 0; j < temp.GetUpperBound(1); j++)
		{
			Color32 col = new Color(temp[i, j].r, temp[i, j].g, temp[i, j].b, temp[i, j].a);
			Int32 mColor = col.r * 256*256 + col.g * 256 + col.b; //Returns the colour as an int
			float HeightVal = (float)mColor / (float)Math.Pow(2, 24);
			//ALL VALUES SET FROM 0.0 to 1.0
			Color ColorVal = cso.GetColorFromVal((HeightVal*ScaleYSliderValcolour));
			tempsplat.SetPixel(i,j,ColorVal);
		}
		
		tempsplat.Apply();
		
		byte[] bytes = tempsplat.EncodeToPNG();
		Terrain terr = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		Texture2D convertedsplat = new Texture2D((int)terr.terrainData.size.x, (int)terr.terrainData.size.z);
		convertedsplat.LoadImage(bytes);
		
		SplatPrototype[] splatAp = new SplatPrototype[1];
		splatAp[0] = new SplatPrototype();
		splatAp[0].texture = convertedsplat;
		splatAp[0].tileOffset = new Vector2(0, 0);
		splatAp[0].tileSize = new Vector2(terr.terrainData.size.x, terr.terrainData.size.z);

		terr.terrainData.splatPrototypes = null;
		terr.terrainData.splatPrototypes = splatAp;
		t = terr;
	}
	
	private Texture2D LoadColourMap(string DataSource, string TerrainType, string Filename, string Preset)
	{
		Terrain t = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		
		Texture2D corrmap = new Texture2D((int)t.terrainData.size.x, (int)t.terrainData.size.z);
		string colourfile = DataSource + TerrainType + Filename + Preset;//dSourceList[datasourcesCombo.SelectedItemIndex].text + 
//			tTypesList[terraintypesCombo.SelectedItemIndex].text +
//				fNameList[filenamesCombo.SelectedItemIndex].text +
//				presetsList[presetsCombo.SelectedItemIndex].text;
		
		if(TerrainType == "Smooth")
		{
			byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/GA_Color_" + colourfile + ".png");
			corrmap.LoadImage(cMapBytes);
		}
		else if(TerrainType == "Granular")
		{
			byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/colormap_" + colourfile + ".png");
			corrmap.LoadImage(cMapBytes);
		}
		else if(TerrainType == "Cylindrical")
		{
			byte[] cMapBytes = File.ReadAllBytes(Application.dataPath + "/Heightmaps/Images/colormap_" + colourfile + ".png");
			corrmap.LoadImage(cMapBytes);
		}
		
		return corrmap;
	}
}
