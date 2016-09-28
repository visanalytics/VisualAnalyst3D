using System;
using UnityEngine;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class MenuTabsSingleplayer : MenuTabs
{
	
	bool KeyboardSelected = true;
	bool PhoneSelected = false;
	bool ImportingData = false;

	public MenuTabsSingleplayer (MenuHandler handler) : base(handler)
	{
		this.Handler = handler;
		CurrentTabState = TabState.TabDefault;
	}
	
	protected override void GUISettings(){
		
		if(ImportingData){

			ImportDataGUI();

		}else{
			if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
				SetTabState(TabState.TabDefault);
			}
			GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*7f), "");
			
			
			if(Handler.Inflater.TerrainVisible)
				MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*1.5f, tw, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
			if(GUI.Button(new Rect(w - tw*2f, h/2 - th*1.5f, tw, th), "Terrain On/Off")){
				if(Handler.Inflater.TerrainVisible){
					Handler.Inflater.HideTerrain();
				}else if(!Handler.Inflater.TerrainVisible){
					Handler.Inflater.UnhideTerrain();
				}
				Handler.UpdateFlags();
			}
			
			if(Handler.Inflater.MapVisible)
				MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*0.5f, tw, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
			if(GUI.Button(new Rect(w - tw*2f, h/2 - th*0.5f, tw, th), "Map On/Off")){
				if(Handler.Inflater.MapVisible)
					Handler.Inflater.HideMap();
				else if(!Handler.Inflater.MapVisible)
					Handler.Inflater.UnhideMap();
			}
			if(Handler.GridsVisible)
				MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 + th*0.5f, tw, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
			if(GUI.Button(new Rect(w - tw*2f, h/2 + th*0.5f, tw, th), "Grids On/Off")){
				if(Handler.GridsVisible)
					Handler.HideGrids();
				else if(!Handler.GridsVisible)
					Handler.UnhideGrids();
			}

			#region Controller Selection
			GUI.Label(new Rect(w - tw*2f, h/2 + th*2.5f, tw, th), "Controller: ");
			if(KeyboardSelected)
				MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 + th*3.5f, tw, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
			if(GUI.Button(new Rect(w - tw*2f, h/2 + th*3.5f, tw, th), "Keyboard")){
				if(!KeyboardSelected){
					PhoneSelected = false;
					Handler.ControlHandler.SetControl(ControllerHandler.ControllerType.Keyboard);
					KeyboardSelected = true;
				}
			}
			if(PhoneSelected)
				MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 + th*4.5f, tw, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
			if(GUI.Button(new Rect(w - tw*2f, h/2 + th*4.5f, tw, th), "Phone")){
				if(!PhoneSelected){
					KeyboardSelected = false;
					Handler.ControlHandler.SetControl(ControllerHandler.ControllerType.Phone);
					PhoneSelected = true;
				}
			}

			#endregion
			
			MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*5.75f, tw*1.5f, th), Color.green);
			if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*5.75f, tw*1.5f, th), "Import Data")){
				ImportingData = true;
				ImportDataPresetList = DataInterpreter.GetPresetList();
			}

			MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*7.5f, tw*1.5f, th), Color.red);
			if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*7.5f, tw*1.5f, th), "Regenerate Terrain")){
				Handler.StateSingleplayer.RegenerateCurrentTerrainSingle();
			}
			
			// Developer tool to regenerate every map 
			MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*9.5f, tw*1.5f, th), Color.red);
			if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*9.5f, tw*1.5f, th), "Regenerate ALL")){
				Handler.Inflater.GenerateAllTerrains();
			}
		}
	}

	string ImportDataFilename = "";
	string ImportDataMapFilename = "";
	string[] ImportDataColumnNames;
	int[] ImportDataColumnSelected;
	string[] ImportDataColumnAlias;
	Vector2 ImportDataScrollPos = new Vector2();
	string ImportDataErrorMessage = "Import Data";
	string ImportDataPresetName = "";
	bool ImportDataShowAdvanced = false;
	double ImportDataMinX, ImportDataMinZ, ImportDataMaxX, ImportDataMaxZ;
	private enum ImportDataState {Default=1, Add=2, Edit=3, Delete=4};
	ImportDataState ImportDataStateCurrent = ImportDataState.Default;
	List<string[]> ImportDataPresetList = new List<string[]>();
	string ImportDataDeleteDataName = "";
	string ImportDataDeletePresetName = "";

	private void ImportDataGUI(){
		//
		switch(ImportDataStateCurrent){
		case ImportDataState.Default:
			ImportDataDefaultGUI();
			break;
		case ImportDataState.Add:
			ImportDataAddGUI(false);
			break;
		case ImportDataState.Edit:
			ImportDataAddGUI(true);
			break;
		case ImportDataState.Delete:
			ImportDataDeleteGUI();
			break;
		}
	}

	private void ImportDataDefaultGUI(){
		
		float boxX = w*0.15f, boxY = th*6f, boxW = w*0.7f, boxH = h - th*8f;
		GUI.Box(new Rect(boxX, boxY, boxW, boxH), "");
		if(GUI.Button(new Rect(boxX + boxW*0.5f - tw*0.6f, boxY + th*1f, tw*1.2f, th), "Import New Data")){
			ImportDataFilename = "";
			ImportDataMapFilename = "";
			ImportDataScrollPos = new Vector2();
			ImportDataErrorMessage = "Import Data";
			ImportDataPresetName = "";
			ImportDataShowAdvanced = false;
			ImportDataMinX = 0;
			ImportDataMinZ = 0;
			ImportDataMaxX = 0; 
			ImportDataMaxZ = 0;
			ImportDataStateCurrent = ImportDataState.Add;
		}

		GUI.Box(new Rect(boxX + tw/2f, boxY + th*2.5f, boxW - tw, th), "");
		GUI.Label(new Rect(boxX + tw/2f, boxY + th*2.5f, tw*1.5f, th), "Source File");
		GUI.Label(new Rect(boxX + tw/2f + tw*1.6f, boxY + th*2.5f, tw*1.5f, th), "Preset Name");
		if(ImportDataPresetList.Count * th * 1.1f < boxH - th*5f){
			
			ImportDataShowPresets(boxX + tw/2f, boxY + th*3.5f, boxW - tw, boxH - th*5f);
			
		}else{
			Rect scrollRect = new Rect(boxX + tw/2f, boxY + th*3.5f, boxW - tw, boxH - th*5f);
			Rect viewRect = new Rect(0f, 0f, boxW - tw*1.5f, ImportDataPresetList.Count * th * 1.1f);
			ImportDataScrollPos = GUI.BeginScrollView(
				scrollRect,
				ImportDataScrollPos,
				viewRect,
				false,
				true);
			ImportDataShowPresets(0, 0, boxW - tw*1.5f, ImportDataPresetList.Count * th * 1.1f);
			GUI.EndScrollView();
		}

		if(GUI.Button(new Rect(boxX + boxW*0.5f - tw*0.5f, boxY+boxH - th*1.5f, tw, th), "Close")){
			ImportingData = false;
		}
	}

	private void ImportDataShowPresets(float boxX, float boxY, float boxW, float boxH){
		for(int i=0; i<ImportDataPresetList.Count; i++){
			GUI.Box(new Rect(boxX, boxY + (th*1.1f*i), (boxW - tw*1.8f)*0.5f - tw*0.05f, th), "");
			GUI.Label(new Rect(boxX, boxY + (th*1.1f*i), (boxW - tw*1.8f)*0.5f - tw*0.05f, th), ImportDataPresetList[i][0]);
			GUI.Box(new Rect(boxX + (boxW - tw*1.8f)*0.5f + tw*0.05f, boxY + (th*1.1f*i), (boxW - tw*1.8f)*0.5f - tw*0.05f, th), "");
			GUI.Label(new Rect(boxX + (boxW - tw*1.8f)*0.5f + tw*0.05f, boxY + (th*1.1f*i), (boxW - tw*1.8f)*0.5f - tw*0.05f, th), ImportDataPresetList[i][1]);
			if(GUI.Button(new Rect(boxX + boxW - tw*1.7f, boxY + (th*1.1f*i), tw*0.75f, th), "Edit")){
				// Edit listing
				Variables Vars = VariablesPresets.VariablePreset(ImportDataPresetList[i][0],
				                                "Cylindrical",
				                                ImportDataPresetList[i][1],
				                                "Cylindrical"); // Using cylindrical to simply get data for the preset
				ImportDataFilename = UnityEngine.Application.dataPath + Vars.FILENAME;
				ImportDataMapFilename = UnityEngine.Application.dataPath + Vars.MAP_FILENAME;
				ImportDataColumnNames = DataInterpreter.ImportHeaderList(ImportDataFilename);
				ImportDataColumnSelected = new int[]{Vars.COLUMN_X, Vars.COLUMN_Y, Vars.COLUMN_Z};
				ImportDataColumnAlias = new string[]{Vars.COLUMN_X_ALIAS, Vars.COLUMN_Y_ALIAS, Vars.COLUMN_Z_ALIAS};
				ImportDataPresetName = ImportDataPresetList[i][1];
				ImportDataShowAdvanced = true;
				ImportDataMinX = Vars.MIN_X;
				ImportDataMinZ = Vars.MIN_Z;
				ImportDataMaxX = Vars.MAX_X;
				ImportDataMaxZ = Vars.MAX_Z;
				ImportDataStateCurrent = ImportDataState.Edit;
			}
			if(GUI.Button(new Rect(boxX +boxW - tw*0.85f, boxY + (th*1.1f*i), tw*0.75f, th), "Delete")){
				// Delete listing
				ImportDataDeleteDataName = ImportDataPresetList[i][0];
				ImportDataDeletePresetName = ImportDataPresetList[i][1];
				ImportDataStateCurrent = ImportDataState.Delete;
			}
		}
	}

	private void ImportDataDeleteGUI(){
		float boxX = w/2f - tw*1.5f, boxY = h/2f - th*2f, boxW = tw*3f, boxH = th*5.25f;
		GUI.Box(new Rect(boxX, boxY, boxW, boxH), "Do you want to delete...");
		GUI.Label(new Rect(boxX + boxW/2f - tw*1.25f, boxY + th*1.5f, tw*2.5f, th), "Data: " + ImportDataDeleteDataName);
		GUI.Label(new Rect(boxX + boxW/2f - tw*1.25f, boxY + th*2.5f, tw*2.5f, th), "Preset: " + ImportDataDeletePresetName);
		if(GUI.Button(new Rect(boxX + boxW/2f - tw*0.75f, boxY + th*4f, tw*0.5f, th), "Yes")){
			DataInterpreter.DeletePreset(ImportDataDeleteDataName, ImportDataDeletePresetName);
			ImportDataPresetList = DataInterpreter.GetPresetList();
			Handler.StateSingleplayer.ReinitGui();
			ImportDataStateCurrent = ImportDataState.Default;
		}
		if(GUI.Button(new Rect(boxX + boxW/2f + tw*0.25f, boxY + th*4f, tw*0.5f, th), "No")){
			ImportDataStateCurrent = ImportDataState.Default;
		}
	}

	private void ImportDataAddGUI(bool editing){
		float boxX = w/4f, boxY = th*6f, boxW = w/2f, boxH = h - th*8f;
		GUI.Box(new Rect(boxX, boxY, boxW, boxH), ImportDataErrorMessage);
		GUI.Box (new Rect(boxX + tw/2f, boxY + th*1f, boxW - tw*2f, th), "");
		GUI.Label(new Rect(boxX + tw/2f, boxY + th*1f, boxW - tw*2f, th), ImportDataFilename);
		if(!editing){
			if(GUI.Button(new Rect(boxX + boxW - tw*1.5f, boxY + th*1f, tw*1.2f, th), "Open File")){
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = 
					"CSV files (*.csv)|*.csv|All files (*.*)|*.*";
				dialog.DefaultExt = "csv";
				DialogResult result = dialog.ShowDialog();
				if(result == DialogResult.OK){
					ImportDataFilename = dialog.FileName.Replace('\\', '/'); //EditorUtility.OpenFilePanel("Open File", "", "csv");
					if(ImportDataFilename != "" && 
					   ImportDataFilename.EndsWith(".csv")){
						ImportDataColumnNames = DataInterpreter.ImportHeaderList(ImportDataFilename);
						if(ImportDataColumnNames.Length < 3){
							ImportDataErrorMessage = "Not enough data.";
							ImportDataFilename = "";
						}else{
							ImportDataColumnSelected = new int[]{0,1,2};
							ImportDataColumnAlias = new string[]{"","",""};
						}
					}
				}
			}
		}

		// File selected
		if(ImportDataFilename != "" && 
			   ImportDataFilename.EndsWith(".csv")){

			// Map Filename selection
			GUI.Box (new Rect(boxX + tw/2f, boxY + th*2f, boxW - tw*2f, th), "");
			GUI.Label(new Rect(boxX + tw/2f, boxY + th*2f, boxW - tw*2f, th), ImportDataMapFilename);
			if(GUI.Button(new Rect(boxX + boxW - tw*1.5f, boxY + th*2f, tw*1.2f, th), "Select Map Image")){
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.DefaultExt = "png";
				DialogResult result = dialog.ShowDialog();
				if(result == DialogResult.OK)
					ImportDataMapFilename = dialog.FileName.Replace('\\', '/');//EditorUtility.OpenFilePanel("Select Map Image", "", "png");
			}

			if(ImportDataShowAdvanced){
				MenuHandler.GUIDrawRect(new Rect(boxX + boxW, boxY + th*2f, tw*1.2f, th), Color.gray);
				ImportDataAdvanced(boxX, boxY, boxW, boxH);
			}
			if(GUI.Button(new Rect(boxX + boxW, boxY + th*2f, tw*1.2f, th), "Advanced Settings")){
				ImportDataShowAdvanced = !ImportDataShowAdvanced;
			}

			GUI.Box(new Rect(boxX + tw/2f, boxY + th*3.5f, boxW - tw, th), "");
			GUI.Label(new Rect(boxX + tw/2f, boxY + th*3.5f, tw/3f, th), "X");
			GUI.Label(new Rect(boxX + tw/2f + tw/3f, boxY + th*3.5f, tw/3f, th), "Y");
			GUI.Label(new Rect(boxX + tw/2f + tw*2f/3f, boxY + th*3.5f, tw/3f, th), "Z");
			GUI.Label(new Rect(boxX + tw*1.5f, boxY + th*3.5f, tw, th), "Column Name");
			if(ImportDataColumnNames.Length * th * 1.1f < boxH - th*5.5f){

				ImportDataFromOrigin(boxX + tw/2f, boxY + th*4.5f, boxW - tw, boxH - th*8f);

			}else{
				Rect scrollRect = new Rect(boxX + tw/2f, boxY + th*4.5f, boxW - tw, boxH - th*8f);
				Rect viewRect = new Rect(0f, 0f, boxW - tw*1.5f, ImportDataColumnNames.Length * th * 1.1f);
				ImportDataScrollPos = GUI.BeginScrollView(
					scrollRect,
					ImportDataScrollPos,
					viewRect,
					false,
					true);
				ImportDataFromOrigin(0, 0, boxW - tw*1.5f, ImportDataColumnNames.Length * th * 1.1f);
				GUI.EndScrollView();
			}
		}

		GUI.Label(new Rect(boxX + boxW*0.5f - tw*1.75f, boxY+boxH - th*3f, tw*1f, th), "Preset Name: ");
		if(!editing){
			ImportDataPresetName = GUI.TextField(new Rect(boxX + boxW*0.5f - tw*0.75f, boxY+boxH - th*3f, tw*2.5f, th), ImportDataPresetName);
		}else{
			GUI.Label(new Rect(boxX + boxW*0.5f - tw*0.75f, boxY+boxH - th*3f, tw*2.5f, th), ImportDataPresetName);
		}

		if(GUI.Button(new Rect(boxX + boxW*0.5f - tw*1.25f, boxY+boxH - th*1.5f, tw, th), "Import")){
			// Test all necessary inputs have been chosen
			string[] filename_parts = ImportDataFilename.Split('/');
			string data_name = filename_parts[filename_parts.Length-1].Replace(".csv", "");
			if(ImportDataFilename != "" && 
			   ImportDataFilename.EndsWith(".csv")){
				// Map filename
				if(ImportDataMapFilename == ""){
					ImportDataMapFilename = UnityEngine.Application.dataPath + "/Heightmaps/Maps/Time.png";
				}

				// if advanced settings is toggled
				if(ImportDataShowAdvanced){
					// Delete preset if it exists
					DataInterpreter.DeletePreset(data_name, ImportDataPresetName);
					DataInterpreter.ImportPreset(ImportDataPresetName, 
					                             data_name,
					                             ImportDataFilename, 
					                             ImportDataMapFilename, 
					                             ImportDataColumnSelected[0],
					                             ImportDataColumnSelected[1],
					                             ImportDataColumnSelected[2],
					                             ImportDataColumnAlias[0],
					                             ImportDataColumnAlias[1],
					                             ImportDataColumnAlias[2],
					                             ImportDataMinX,
					                             ImportDataMinZ,
					                             ImportDataMaxX,
					                             ImportDataMaxZ);
				}else{
					// Delete preset if it exists
					DataInterpreter.DeletePreset(data_name, ImportDataPresetName);
					DataInterpreter.ImportPreset(ImportDataPresetName, 
					                             data_name,
					                             ImportDataFilename, 
					                             ImportDataMapFilename, 
					                             ImportDataColumnSelected[0],
					                             ImportDataColumnSelected[1],
					                             ImportDataColumnSelected[2],
					                             ImportDataColumnAlias[0],
					                             ImportDataColumnAlias[1],
					                             ImportDataColumnAlias[2]);
				}

				Handler.StateSingleplayer.ReinitGui();
				// Reset all variables and exit import gui.
				ImportDataFilename = "";
				ImportDataMapFilename = "";
				ImportDataScrollPos = new Vector2();
				ImportDataErrorMessage = "Import Data";
				ImportDataPresetName = "";
				ImportDataPresetList = DataInterpreter.GetPresetList();
				ImportDataStateCurrent = ImportDataState.Default;
			}
		}
		if(GUI.Button(new Rect(boxX + boxW*0.5f + tw*0.25f, boxY+boxH - th*1.5f, tw, th), "Cancel")){
			ImportDataStateCurrent = ImportDataState.Default;
		}
	}

	private void ImportDataAdvanced(float boxX, float boxY, float boxW, float boxH){
		float tBoxX = boxX + boxW, tBoxY = boxY + th*3f, tBoxW = tw*1.5f, tBoxH = boxH - th*3f;
		GUI.Box(new Rect(tBoxX, tBoxY, tBoxW, tBoxH), "");
		
		GUI.Label(new Rect(tBoxX, tBoxY, tw, th), "Minimum X");
		ImportDataMinX = double.Parse(GUI.TextField(new Rect(tBoxX + tw*0.25f, tBoxY + th, tw, th), ImportDataMinX.ToString()));
		GUI.Label(new Rect(tBoxX, tBoxY + th*2f, tw, th), "Minimum Z");
		ImportDataMinZ = double.Parse(GUI.TextField(new Rect(tBoxX + tw*0.25f, tBoxY + th*3f, tw, th), ImportDataMinZ.ToString()));
		GUI.Label(new Rect(tBoxX, tBoxY + th*4f, tw, th), "Maximum X");
		ImportDataMaxX = double.Parse(GUI.TextField(new Rect(tBoxX + tw*0.25f, tBoxY + th*5f, tw, th), ImportDataMaxX.ToString()));
		GUI.Label(new Rect(tBoxX, tBoxY + th*6f, tw, th), "Maximum Z");
		ImportDataMaxZ = double.Parse(GUI.TextField(new Rect(tBoxX + tw*0.25f, tBoxY + th*7f, tw, th), ImportDataMaxZ.ToString()));
	}

	private void ImportDataFromOrigin(float boxX, float boxY, float boxW, float boxH){
		for(int i=0; i<ImportDataColumnNames.Length; i++){
			for(int col = 0; col < ImportDataColumnSelected.Length; col++){
				bool toggled = ImportDataColumnSelected[col] == i;
				if(GUI.Toggle(new Rect(boxX + (tw/3f)*col, boxY + (th*1.1f*i), tw/3f, th),
				              toggled,
				              "")){
					bool collision = false;
					for(int j=0; j<ImportDataColumnSelected.Length; j++){
						if(ImportDataColumnSelected[j] == i){
							collision = true;
							//Swap the two selected columns
							ImportDataColumnSelected[j] = ImportDataColumnSelected[col];
							ImportDataColumnSelected[col] = i;
						}
					}
					if(!collision){
						ImportDataColumnSelected[col] = i;
					}
				}
			}
			bool selected = false;
			int columnIndex = -1;
			for(int j=0; j<ImportDataColumnSelected.Length; j++){
				if(ImportDataColumnSelected[j] == i){
					selected = true;
					columnIndex = j;
				}
			}
			GUI.Label(new Rect(boxX + tw*1.1f, boxY + (th*1.1f*i), tw*1f, th), ImportDataColumnNames[i]);
			if(selected && ImportDataColumnSelected[columnIndex] == i){
				float size = tw*1.25f; //tw*3.35f > boxW ? boxW - tw*2.1f : tw*1.25f;
				ImportDataColumnAlias[columnIndex] = GUI.TextField(new Rect(boxX + tw*2.1f, boxY + (th*1.1f*i), size, th), ImportDataColumnAlias[columnIndex]);
			}
		}
	}
	
	protected override void GUIGrids(){
		
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		
		
		
		if(SelectingGridOrientation)
		{
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Select Orientation");
			
			if(GUI.Button(new Rect(w/2 -tw*1f, h/2 - th*3.0f, tw*2f, th), "Left - Right"))
			{
				SelectingGridOrientation = false;
				GridData dTemp = ((GridData)GridBeingNamed.GetComponent(typeof(GridData)));
				dTemp.SetOrientation(GridData.Orientation.UP_X);
				NamingGrid = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*1f, h/2 - th*1.5f, tw*2f, th), "Up - Down"))
			{
				SelectingGridOrientation = false;
				GridData dTemp = ((GridData)GridBeingNamed.GetComponent(typeof(GridData)));
				dTemp.SetOrientation(GridData.Orientation.UP_Y);
				NamingGrid = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*1f, h/2 - th*0.0f, tw*2f, th), "Forwards - Backwards"))
			{
				SelectingGridOrientation = false;
				GridData dTemp = ((GridData)GridBeingNamed.GetComponent(typeof(GridData)));
				dTemp.SetOrientation(GridData.Orientation.UP_Z);
				NamingGrid = true;
			}
		}
		
		if(NamingGrid){
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Name Grid");
			tempGridName = GUI.TextArea (new Rect (w/2 - tw*1.5f, h/2 - th*3f, tw*3f, th*6f), tempGridName);
			
			if(GUI.Button(new Rect(w/2 - tw/2, h/2 + th*3.05f, tw, th), "Done")){
				GridData dTemp = ((GridData)GridBeingNamed.GetComponent(typeof(GridData)));
				if(tempGridName != ""){
					dTemp.SetName(tempGridName);
				}else{
					dTemp.SetName("Grid");
				}
				if(SelectedGrid != null){
					SelectedGrid.GetComponent<GridData>().SetGridSelected(false);
				}
				switch(dTemp.GetOrientation()){
				case GridData.Orientation.UP_X:
					SelectedGridHeight = (dTemp.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.x) * 100;
					break;
				case GridData.Orientation.UP_Y:
					SelectedGridHeight = (dTemp.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.y) * 100;
					break;
				case GridData.Orientation.UP_Z:
					SelectedGridHeight = (dTemp.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.z) * 100;
					break;
				}
				SelectedGrid = GridBeingNamed;
				SelectedGridData = (GridData)SelectedGrid.GetComponent(typeof(GridData));
				SelectedGridData.SetGridSelected(true);
				tempGridName = "";
				NamingGrid = false;
			}
		}
		
		// New Grid Button
		if(GUI.Button(new Rect(w-tw*2, th*5.5f, tw, th), "New Grid")){
			SelectingGridOrientation = true;
			GridBeingNamed = Handler.CreateGrid("", GridData.Orientation.UP_Y);
		}
		
		// Viewing Grid:  
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*7f), "");
		if(Handler.Grids.Count > 0){
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th), Color.red);
			if(GUI.Button(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th),"Delete ALL Grids")){
				Handler.DeleteAllGrids();
			}
			for(int i=0; i<Handler.Grids.Count; i++){
				GridData dTemp = (GridData)Handler.Grids[i].GetComponent(typeof(GridData)); 
				float curX = w - tw*3f,curY = th*7f + th*2f*i;
				float width = tw*3f, height = th*2f;
				
				if(dTemp.IsSelected()){MenuHandler.GUIDrawRect(new Rect(curX, curY, width, height), new Color(1.0f, 0.55f, 0.55f, 0.65f));}
				
				//Select Grid Button 
				if(GUI.Button(new Rect(curX, curY, width, height), "")){
					if(SelectedGrid == Handler.Grids[i]){
						SelectedGrid = null;
						dTemp.SetGridSelected(false);
					}else{
						if(SelectedGrid != null){
							SelectedGrid.GetComponent<GridData>().SetGridSelected(false);
						}
						SelectedGrid = Handler.Grids[i];
						SelectedGridData = (GridData)SelectedGrid.GetComponent(typeof(GridData));

						switch(SelectedGridData.GetOrientation()){
						case GridData.Orientation.UP_X:
							SelectedGridHeight = (SelectedGridData.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.x) * 100 + 1;
							break;
						case GridData.Orientation.UP_Y:
							SelectedGridHeight = (SelectedGridData.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.y) * 100 + 1;
							break;
						case GridData.Orientation.UP_Z:
							SelectedGridHeight = (SelectedGridData.GetWorldValue()/Handler.Inflater.theterrain.terrainData.size.z) * 100 + 1;
							break;
						}
						SelectedGridData.SetGridSelected(true);
					}
				}
				GUI.Label(new Rect(curX + width*0.05f, curY, width*0.9f, height/2), dTemp.GetName());
				string PosString = "";
				switch(dTemp.GetOrientation()){
				case GridData.Orientation.UP_X:
					PosString = "Lat: " + Math.Round(dTemp.GetDataValue(), 2).ToString();
					break;
				case GridData.Orientation.UP_Y:
					PosString = "Height: " + Math.Round(dTemp.GetDataValue(), 2).ToString();
					break;
				case GridData.Orientation.UP_Z:
					PosString = "Long: " + Math.Round(dTemp.GetDataValue(), 2).ToString();
					break;
				default:
					PosString = "Value: " + Math.Round(dTemp.GetDataValue(), 2).ToString();
					break;
				}
				GUI.Label(new Rect(curX + width*0.05f, curY + height/2, width*0.9f, height/2), PosString);
			}
		}
		
		// Grid Data for SELECTED Grid
		if (SelectedGrid != null) {
			float boxX = w - tw * 3f, boxY = h - th * 6.1f, boxWidth = tw * 3f, boxHeight = th * 6.1f;
			GUI.Box (new Rect (boxX, boxY - th, boxWidth, boxHeight + th * 0.9f), "Selected Grid");				
			
			// Grid Height Slider
			GUI.Label (new Rect (w - th * 2f, h - th * 6f, th * 1.5f, th), "");
			GUI.Label (new Rect (w - th * 2f, h - th * 1.1f, th * 1.5f, th), "");
			MenuHandler.GUIDrawRect (new Rect (w - th * 1.2f, h - th * 5.3f, th, th * 4.5f), new Color (1f, 1f, 1f, 0.25f));
			SelectedGridHeight = GUI.VerticalSlider (new Rect (w - th, h - th * 5f, th, th * 3.9f), SelectedGridHeight, 100.0f, 0.0f);
			SelectedGridData.SetWorldPosIndex ((int)SelectedGridHeight);
			SelectedGridData.SetDataPosIndex ((int)SelectedGridHeight);
			string selectedGridPosString = "Value: " + Math.Round (SelectedGridData.GetDataValue(), 4).ToString ();
			
			bool defaultWordWrap = GUI.skin.label.wordWrap;
			GUI.skin.label.wordWrap = true;
			GUI.Label (new Rect (boxX + boxWidth * 0.05f, boxY, boxWidth * 0.9f, th * 2f), SelectedGridData.GetName ());
			GUI.skin.label.wordWrap = defaultWordWrap;
			GUI.Label (new Rect (boxX + boxWidth * 0.05f, boxY + th * 2.5f, boxWidth * 0.9f, th * 1f), selectedGridPosString);
			
			if (SelectedGrid.renderer.material == Handler.Inflater.GridSolidMat)
				MenuHandler.GUIDrawRect (new Rect (boxX + boxWidth * 0.1f, boxY + th * 3.55f, boxWidth * 0.33f, th), new Color (0.78f, 0.73f, 0.93f, 0.55f));
			if (GUI.Button (new Rect (boxX + boxWidth * 0.1f, boxY + th * 3.55f, boxWidth * 0.33f, th), "Solid")) {
				SelectedGrid.renderer.material = Handler.Inflater.GridSolidMat;
				Handler.UpdateGrids ();
			}
			if (SelectedGrid.renderer.material == Handler.Inflater.GridTransparentMat)
				MenuHandler.GUIDrawRect (new Rect (boxX + boxWidth * 0.57f, boxY + th * 3.55f, boxWidth * 0.33f, th), new Color (0.78f, 0.73f, 0.93f, 0.55f));
			if (GUI.Button (new Rect (boxX + boxWidth * 0.57f, boxY + th * 3.55f, boxWidth * 0.33f, th), "Transparent")) {
				SelectedGrid.renderer.material = Handler.Inflater.GridTransparentMat;
				Handler.UpdateGrids ();
			}
			
			
			// Delete Grid Button
			if (GUI.Button (new Rect (boxX + boxWidth * 0.33f, boxY + th * 4.65f, boxWidth * 0.33f, th), "Delete Grid")) {
				Handler.Grids.Remove (SelectedGrid);
				Main.Destroy (SelectedGrid);
				SelectedGrid = null;
			}
		}
	}
	
	protected override void GUIFlags(){
		
		if(!PlacingFlag && !SelectingFlag && !FlagColour && ! AnnotatingFlag){
			if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
				SetTabState(TabState.TabDefault);
			}
			
			if(GUI.Button(new Rect(w-tw*2, th*5.5f, tw, th), "New Flag")){
				if(!PlacingFlag){
					FlagBeingAnnotated = Handler.CreateFlag("", Vector3.zero);
					PlacingFlag = true;
				}
			}
		}
		
		// Viewing Flags: 
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*5f), "");
		if(Handler.Flags.Count > 0){
			if(!PlacingFlag && !SelectingFlag && !FlagColour && ! AnnotatingFlag){
				MenuHandler.GUIDrawRect(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th), Color.red);
				if(GUI.Button(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th),"Delete ALL Flags")){
					Handler.DeleteAllFlags();
				}
			}
			for(int i=0; i<Handler.Flags.Count; i++){
				FlagData fTemp = (FlagData)Handler.Flags[i].GetComponent(typeof(FlagData));
				if(fTemp.GetAnnotation() != "" && fTemp.GetDataPos() != Vector3.zero){
					float curX = w - tw*3f,curY = th*7f + th*2f*i;
					float width = tw*3f, height = th*2f;
					
					if(fTemp.IsSelected()){MenuHandler.GUIDrawRect(new Rect(curX, curY, width, height), new Color(1.0f, 0.55f, 0.55f, 0.65f));}
					
					//Select Flag Button  
					if(GUI.Button(new Rect(curX, curY, width, height), "")){
						if(SelectedFlag == Handler.Flags[i]){
							SelectedFlag = null;
							fTemp.SetFlagSelected(false);
						}else{
							if(SelectedFlag != null){
								SelectedFlag.GetComponent<FlagData>().SetFlagSelected(false);
							}
							SelectedFlag = Handler.Flags[i];
							fTemp.SetFlagSelected(true);
						}
					}
					GUI.Label(new Rect(curX + width*0.05f, curY, width*0.9f, height/2), fTemp.GetAnnotation());
					string PosString = "Lat: " + Math.Round(fTemp.GetDataPos().x, 2).ToString() +
						", Long: " + Math.Round(fTemp.GetDataPos().z, 2).ToString() +
							", Value: " + Math.Round(fTemp.GetDataPos().y, 2).ToString();
					GUI.Label(new Rect(curX + width*0.05f, curY + height/2, width*0.9f, height/2), PosString);
				}
			}
		}
		
		// Flag Data for SELECTED Flag  
		if(SelectedFlag != null){
			float boxX = w - tw*3f, boxY = h - th*6.1f, boxWidth = tw*3f, boxHeight = th*6.1f;
			GUI.Box(new Rect(boxX, boxY-th, boxWidth, boxHeight+th*0.9f), "Selected Flag");
			
			FlagData selectedFlagData = (FlagData)SelectedFlag.GetComponent(typeof(FlagData));
			string selectedFlagPosString = "Lat: " + Math.Round(selectedFlagData.GetDataPos().x, 2).ToString() +
				", Long: " + Math.Round(selectedFlagData.GetDataPos().z, 2).ToString() +
					", Value: " + Math.Round(selectedFlagData.GetDataPos().y, 2).ToString();
			
			bool defaultWordWrap = GUI.skin.label.wordWrap;
			GUI.skin.label.wordWrap = true;
			GUI.Label(new Rect(boxX + boxWidth*0.05f, boxY, boxWidth*0.9f, th*3f), selectedFlagData.GetAnnotation());
			GUI.skin.label.wordWrap = defaultWordWrap;
			GUI.Label(new Rect(boxX + boxWidth*0.05f, boxY + th*3.5f, boxWidth*0.9f, th*1f), selectedFlagPosString);
			// Delete Flag Button
			if(!PlacingFlag && !SelectingFlag && !FlagColour && ! AnnotatingFlag){
				if(GUI.Button(new Rect(boxX + boxWidth*0.33f, boxY + th*4.65f, boxWidth*0.33f, th), "Delete Flag")){
					Handler.Flags.Remove(SelectedFlag);
					Main.Destroy(SelectedFlag);
					SelectedFlag = null;
				}
			}
		}
		
		if(SelectingFlag)
		{
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Select Image");
			
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*3.0f, tw, th), Handler.flaghandler.FLAG_TYPE[1]))
			{
				Handler.SetFlagImage(1, Handler.flaghandler);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*1.5f, tw, th), Handler.flaghandler.FLAG_TYPE[2]))
			{
				Handler.SetFlagImage(2, Handler.flaghandler);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*0.0f, tw, th), Handler.flaghandler.FLAG_TYPE[3]))
			{
				Handler.SetFlagImage(3, Handler.flaghandler);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*-1.5f, tw, th), Handler.flaghandler.FLAG_TYPE[4]))
			{
				Handler.SetFlagImage(4, Handler.flaghandler);
				SelectingFlag = false;
				FlagColour = true;
			}
		}
		
		if(FlagColour)
		{
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Select Flag Colour");
			
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*3.0f, tw, th), MenuHandler.FC_WHITE))
			{
				Handler.SetFlagColour(Color.white,MenuHandler.FC_WHITE);
				AnnotatingFlag = true;
				FlagColour = false;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*1.5f, tw, th), MenuHandler.FC_RED))
			{
				Handler.SetFlagColour(Color.red,MenuHandler.FC_RED);
				AnnotatingFlag = true;
				FlagColour = false;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*0.0f, tw, th), MenuHandler.FC_YELLOW))
			{
				Handler.SetFlagColour(Color.yellow,MenuHandler.FC_YELLOW);
				AnnotatingFlag = true;
				FlagColour = false;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*-1.5f, tw, th), MenuHandler.FC_GREEN))
			{
				Handler.SetFlagColour(Color.green,MenuHandler.FC_GREEN);
				AnnotatingFlag = true;
				FlagColour = false;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*-3.0f, tw, th), MenuHandler.FC_BLUE))
			{
				Handler.SetFlagColour(Color.blue,MenuHandler.FC_BLUE);
				AnnotatingFlag = true;
				FlagColour = false;
			}
		}
		
		// Flag Mode  
		if(AnnotatingFlag){
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Annotate Flag");
			tempAnnotation = GUI.TextArea (new Rect (w/2 - tw*1.5f, h/2 - th*3f, tw*3f, th*6f), tempAnnotation);
			
			if(GUI.Button(new Rect(w/2 - tw/2, h/2 + th*3.05f, tw, th), "Annotate")){ //User clicks "Annotate"
				if(tempAnnotation != ""){
					((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetAnnotation(tempAnnotation);
					AnnotatingFlag = true;
				}else{
					((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetAnnotation("Flag");
					AnnotatingFlag = true;
				}
				tempAnnotation = "";
				AnnotatingFlag = false;
			}
		}
	}
}