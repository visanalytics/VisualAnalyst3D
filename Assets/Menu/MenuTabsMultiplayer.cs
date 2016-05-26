using System;
using UnityEngine;
public class MenuTabsMultiplayer : MenuTabs
{
	
	bool KeyboardSelected = true;

	public MenuTabsMultiplayer (MenuHandler handler) : base(handler)
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
				Handler.ControlHandler.SetControl(ControllerHandler.ControllerType.Keyboard);
				KeyboardSelected = true;
			}
		}

		#endregion
		
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.25f, h/2 + th*6.5f, tw*1.5f, th), Color.red);
		if(GUI.Button(new Rect(w - tw*2.25f, h/2 + th*6.5f, tw*1.5f, th), "Regenerate Terrain")){
			Handler.StateSingleplayer.RegenerateCurrentTerrainMulti();
		}
	}
	
	protected override void GUIGrids(){
		
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		
		// New Grid Button
		if(GUI.Button(new Rect(w-tw*2, th*5.5f, tw, th), "New Grid")){
			SelectingGridOrientation = true;
			GridBeingNamed = Handler.CreateGrid("", GridData.Orientation.UP_Y);
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
				RPCGrid(GridBeingNamed);
			}
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
			// Send RPC if grid has changed position
			if((int)SelectedGridHeight != SelectedGridData.GetWorldPosIndex()){
				Handler.Multiplayer.CallUpdateGrid(SelectedGridData.GetID (), (int)SelectedGridHeight);
			}
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
				Handler.Multiplayer.CallDeleteGrid(SelectedGridData.GetID());
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
		
		// Viewing Flags: 
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*5f), "");
		if(Handler.Flags.Count > 0){
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th), Color.red);
			if(GUI.Button(new Rect(w - tw*1.5f, th*3.5f, tw*1.2f, th),"Delete ALL Flags")){
				Handler.DeleteAllFlags();
			}
			for(int i=0; i<Handler.Flags.Count; i++){
				FlagData fTemp = Handler.Flags[i].GetComponent<FlagData>();
				float curX = w - tw*3f, curY = th*7f + th*2f*i;
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
				Handler.Multiplayer.CallDeleteFlag(selectedFlagData.GetID());
				Handler.Flags.Remove(SelectedFlag);
				Main.Destroy(SelectedFlag);
				SelectedFlag = null;
			}
		}

		/*******	Flag Creation  	*******/

		if(GUI.Button(new Rect(w-tw*2, th*5.5f, tw, th), "New Flag")){
			if(!PlacingFlag){
				FlagBeingAnnotated = Handler.CreateFlag("", Vector3.zero);
				PlacingFlag = true;
			}
		}
		
		if(SelectingFlag)
		{
			GUI.Box(new Rect(w/2 - tw*1.55f, h/2 - th*4f, tw*3.1f, th*8.1f), "Select Image");
			
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*3.0f, tw, th), Handler.flaghandler.FLAG_TYPE[1]))
			{
				Handler.SetFlagImage(1, Handler.flaghandler);
				FlagBeingAnnotated.GetComponent<FlagData>().SetFlagImageIndex(1);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*1.5f, tw, th), Handler.flaghandler.FLAG_TYPE[2]))
			{
				Handler.SetFlagImage(2, Handler.flaghandler);
				FlagBeingAnnotated.GetComponent<FlagData>().SetFlagImageIndex(2);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*0.0f, tw, th), Handler.flaghandler.FLAG_TYPE[3]))
			{
				Handler.SetFlagImage(3, Handler.flaghandler);
				FlagBeingAnnotated.GetComponent<FlagData>().SetFlagImageIndex(3);
				SelectingFlag = false;
				FlagColour = true;
			}
			if(GUI.Button(new Rect(w/2 -tw*0.5f, h/2 - th*-1.5f, tw, th), Handler.flaghandler.FLAG_TYPE[4]))
			{
				Handler.SetFlagImage(4, Handler.flaghandler);
				FlagBeingAnnotated.GetComponent<FlagData>().SetFlagImageIndex(4);
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

				//Final step of Flag Creation, call server to create flag for every other user.
				RPCFlag(FlagBeingAnnotated);
			}
		}
	}

	protected override void GUIDataPoints(){
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*7f), "");
		
		GUI.Label(new Rect(w - tw*2f, h/2 - th*1.5f, tw, th), "Show Data Points?");
		// Button YES
		if(ShowDataPoints)
			MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), "Yes")){
			if(!ShowDataPoints)
				RPCAddDataPoints(NumDataPoints);
			ShowDataPoints = true;
		}
		// Button NO
		if(!ShowDataPoints)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), "No")){
			if(ShowDataPoints)
				RPCRemoveDataPoints();
			ShowDataPoints = false;
		}
		
		// Number of Data Points to be shown
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(NumDataPoints.ToString())).x/2f, h/2f + th, tw*1.5f, th),
		          "# of Data Points: " + NumDataPoints.ToString());
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*2.75f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*2.75f, tw, th), MaxNumDataPoints.ToString());
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*1.9f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		NumDataPoints = (int)GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*2f, tw*2, th), NumDataPoints, 0, MaxNumDataPoints);
		
		if(GUI.Button(new Rect(w-tw*2.25f, h/2 + th*3.25f, tw*1.5f, th*1.5f), "Regenerate Points")){
			RPCResetDataPoints();
		}
	}
	
	protected override void GUIExternalAxis(){
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*7f), "");
		
		#region Axis Grid Yes/No
		GUI.Label(new Rect(w - tw*2f, h/2 - th*4f, tw, th), "Show Axis Grid?");
		// Button YES
		if(ShowAxisGrid)
			MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*3f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*2f, h/2 - th*3f, tw*0.45f, th), "Yes")){
			/// Make Networkable
			if(!ShowAxisGrid){
				RPCAxisOn(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
				ShowAxisGrid = true;
			}
		}
		// Button NO
		if(!ShowAxisGrid)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*3f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*3f, tw*0.45f, th), "No")){
			if(ShowAxisGrid){
				RPCAxisOff();
				ShowAxisGrid = false;
			}
		}
		#endregion
		
		#region Axis Grid LABELS Yes/No
		GUI.Label(new Rect(w - tw*2.25f, h/2 - th*1.5f, tw*1.5f, th), "Show Axis Labels?");
		// Button YES
		if(ShowAxisGridLabels)
			MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), "Yes")){
			if(!ShowAxisGridLabels){
				RPCAxisLabelsOn(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
				ShowAxisGridLabels = true;
			}
		}
		// Button NO
		if(!ShowAxisGridLabels)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), "No")){
			if(ShowAxisGridLabels){
				RPCAxisLabelsOff();
				ShowAxisGridLabels = false;
			}
		}
		#endregion
		
		// Axis Grid Size Z:
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(AxisGridSizeZString)).x/2f, h/2f + th, tw*2f, th),
		          "Grid Size Latitude: " + AxisGridSizeZString);
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*2.75f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*2.75f, tw, th), String.Format("{0:0.000}", (AxisMaxGridZ-AxisMinGridZ)));
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*1.9f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		AxisGridSizeZ = GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*2f, tw*2, th), AxisGridSizeZ, 0, AxisMaxGridZ-AxisMinGridZ);
		AxisGridSizeZString = String.Format("{0:0.000}", AxisGridSizeZRounded);
		
		// Axis Grid Size X:
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(AxisGridSizeXString)).x/2f, h/2f + th*3.5f, tw*2f, th),
		          "Grid Size Longitude: " + AxisGridSizeXString);
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*5.25f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*5.25f, tw, th), String.Format("{0:0.000}", (AxisMaxGridX-AxisMinGridX)));
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*4.4f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		AxisGridSizeX = GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*4.5f, tw*2, th), AxisGridSizeX, 0, AxisMaxGridX-AxisMinGridX);
		AxisGridSizeXString = String.Format("{0:0.000}", AxisGridSizeXRounded);
		
		// Axis Grid Size Y:
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(AxisGridSizeYString)).x/2f, h/2f + th*6f, tw*2f, th),
		          "Grid Size Height: " + AxisGridSizeYString);
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*7.75f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*7.75f, tw, th), String.Format("{0:0.000}", (AxisMaxGridY-AxisMinGridY)));
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*6.9f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		AxisGridSizeY = GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*7f, tw*2, th), AxisGridSizeY, 0, AxisMaxGridY-AxisMinGridY);
		AxisGridSizeYString = String.Format("{0:0.000}", AxisGridSizeYRounded);
		
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		if(GUI.Button(new Rect(w-tw*2.25f, h/2 + th*8.25f, tw*1.5f, th*1.5f), "Regenerate Axis Grid")){
			RPCAxisReset();
			ResetAxisGrid();
		}
	}

	private void RPCAxisOn(float AxisGridSizeX, float AxisGridSizeY, float AxisGridSizeZ){
		Handler.Multiplayer.CallExternalAxisOn(AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
		AddAxisGridsValues(AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
		ApplySurfaceGridValues(AxisGridSizeX, AxisGridSizeZ);
	}
	private void RPCAxisOff(){
		Handler.Multiplayer.CallExternalAxisOff();
		RemoveAxisGrids();
		RemoveSurfaceGrid();
	}

	private void RPCAxisReset(){
		Handler.Multiplayer.CallExternalAxisReset(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
		ResetAxisGrid();
	}

	private void RPCAxisLabelsOn(float AxisGridSizeX, float AxisGridSizeY, float AxisGridSizeZ){
		Handler.Multiplayer.CallExternalAxisLabelsOn(AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
		AddAxisGridLabelsValues(AxisGridSizeX, AxisGridSizeY, AxisGridSizeZ);
	}
	private void RPCAxisLabelsOff(){
		Handler.Multiplayer.CallExternalAxisLabelsOff();
		RemoveAxisGridLabels();
	}

	private void RPCAddDataPoints(int NumDataPoints){
		Handler.Multiplayer.CallAddDataPoints(NumDataPoints);
		AddDataPoints(NumDataPoints);
	}

	private void RPCRemoveDataPoints(){
		Handler.Multiplayer.CallRemoveDataPoints();
		RemoveDataPoints();
	}

	private void RPCResetDataPoints(){
		Handler.Multiplayer.CallResetDataPoints(NumDataPoints);
		ResetDataPoints();
	}

	private void RPCGrid(GameObject GridObject){
		GridData data = GridObject.GetComponent<GridData>();

		int ID = data.GetID();
		int Orientation;
		switch(data.GetOrientation()){
		case GridData.Orientation.UP_Y:
			Orientation = 0;
			break;
			
		case GridData.Orientation.UP_X:
			Orientation = 1;
			break;
			
		case GridData.Orientation.UP_Z:
			Orientation = 2;
			break;
			
		default:
			Orientation = 0;
			break;
		}
		string Name = data.GetName();
		// World Index and Data Index are the same value
		int PositionIndex = data.GetWorldPosIndex();
		
		/***	Make call to server to instantiate grid on every client 	***/
		Handler.Multiplayer.CallCreateGrid(ID, Orientation, Name, PositionIndex);
	}

	private void RPCFlag(GameObject FlagObject){
		FlagData data = FlagObject.GetComponent<FlagData>();

		int ID = data.GetID();
		int Tex = data.GetFlagImageIndex();
		string Col = data.GetFlagColorString();
		String Annotation = data.GetAnnotation();
		Vector3 WorldPosition = data.GetWorldPos();
		Vector3 DataPosition = data.GetDataPos();

		/***	Make call to server to instantiate flag on every client 	***/
		Handler.Multiplayer.CallCreateFlag(ID, WorldPosition, DataPosition, Annotation, Col, Tex);
	}
}
