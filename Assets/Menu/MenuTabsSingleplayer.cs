using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class MenuTabsSingleplayer : MenuTabs
{
	
	bool KeyboardSelected = true;
	bool PhoneSelected = false;

	public MenuTabsSingleplayer (MenuHandler handler) : base(handler)
	{
		this.Handler = handler;
		CurrentTabState = TabState.TabDefault;
	}
	
	protected override void GUISettings(){
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

		MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*6.5f, tw*1.5f, th), Color.red);
		if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*6.5f, tw*1.5f, th), "Regenerate Terrain")){
			Handler.StateSingleplayer.RegenerateCurrentTerrainSingle();
		}
		
		// Developer tool to regenerate every map 
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*8.5f, tw*1.5f, th), Color.red);
		if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*8.5f, tw*1.5f, th), "Regenerate ALL")){
			Handler.Inflater.GenerateAllTerrains();
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
						DataHandler dH = Handler.Inflater.dH; //new DataHandler(Application.dataPath + Handler.Inflater.Vars.FILENAME, Handler.Inflater.Vars.COLUMN_X, Handler.Inflater.Vars.COLUMN_Y, Handler.Inflater.Vars.COLUMN_Z);
						
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
		
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		
		if(GUI.Button(new Rect(w-tw*2, th*5.5f, tw, th), "New Flag")){
			if(!PlacingFlag){
				FlagBeingAnnotated = Handler.CreateFlag("", Vector3.zero);
				PlacingFlag = true;
			}
		}
		
		// Viewing Flags: 
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*5f), "");
		if(Handler.Flags.Count > 0){
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th), Color.red);
			if(GUI.Button(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th),"Delete ALL Flags")){
				Handler.DeleteAllFlags();
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
			if(GUI.Button(new Rect(boxX + boxWidth*0.33f, boxY + th*4.65f, boxWidth*0.33f, th), "Delete Flag")){
				Handler.Flags.Remove(SelectedFlag);
				Main.Destroy(SelectedFlag);
				SelectedFlag = null;
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