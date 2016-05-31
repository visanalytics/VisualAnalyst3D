using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class MenuTabs
{
	
	protected MenuHandler Handler;
	protected float w = Screen.width;
	protected float h = Screen.height;
	// "tile" width and height, for easy GUI creation 
	protected float tw = 100f;
	protected float th = 20f;
	
	#region Tab States
	
	protected enum TabState {TabFlag=0,TabGrid=1,TabDataPoints=2,TabSurfaceGrid=3,TabAxisGrid=4,TabSettings=5,TabDefault=6};
	protected TabState CurrentTabState;
	
	#endregion
	
	protected GameObject SelectedFlag = null;
	protected GameObject SelectedGrid = null;
	protected bool NamingGrid = false;
	protected bool SelectingGridOrientation = false;
	protected String tempGridName = "";
	protected GameObject GridBeingNamed;
	protected float SelectedGridHeight = 0.3f;
	protected float SelectedGridWidth = 0.3f;
	protected GridData SelectedGridData;
	protected Vector3[] GridPositions;
	protected Vector3[] GridDataPositions;
	
	protected bool PlacingFlag = false;
	protected bool AnnotatingFlag = false;
	protected bool SelectingFlag = false;
	protected bool FlagColour = false;
	protected String tempAnnotation = "";
	protected GameObject FlagBeingAnnotated;

	public MenuTabs (MenuHandler handler)
	{
		this.Handler = handler;
		CurrentTabState = TabState.TabDefault;

		DataHandler dH = Handler.Inflater.dH;
		List<double[]> data = dH.GetData();
		MaxNumDataPoints = data.Count;
		NumDataPoints = (int)(MaxNumDataPoints/1000f) + 1;
		AxisMinGridX = Math.Abs(Handler.Inflater.Vars.MIN_Z);
		AxisMinGridZ = Math.Abs(Handler.Inflater.Vars.MIN_X);
		AxisMaxGridX = Math.Abs(Handler.Inflater.Vars.MAX_Z);
		AxisMaxGridZ = Math.Abs(Handler.Inflater.Vars.MAX_X);
		MinY = 0f;
		MaxY = (float)dH.GetMaxY();
		AxisMinGridY = 0f;
		AxisMaxGridY = (float)Math.Abs(dH.GetMaxY());
		AxisGridSizeX = (AxisMaxGridX-AxisMinGridX)/10f;
		AxisGridSizeY = (AxisMaxGridY-AxisMinGridY)/10f;
		AxisGridSizeZ = AxisGridSizeX;
		AxisGridSizeXIT = GetGridIterator(AxisMaxGridX-AxisMinGridX);
		AxisGridSizeYIT = GetGridIterator(AxisMaxGridY-AxisMinGridY);
		AxisGridSizeZIT = GetGridIterator(AxisMaxGridZ-AxisMinGridZ);
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		AxisGridSizeXString = String.Format("{0:0.00}", AxisGridSizeX);
		AxisGridSizeYString = String.Format("{0:0.00}", AxisGridSizeY);
		AxisGridSizeZString = String.Format("{0:0.00}", AxisGridSizeZ);
	}
	
	public void Update(){
		// Moving the flag with a mouse
		if(PlacingFlag){
			RaycastHit hit; 
			Ray ray = Handler.MainCamera.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
				Vector3 flagPos = hit.point;
				FlagBeingAnnotated.transform.position = flagPos + Vector3.down * 1.0F;
				Vector3 DataPos = Handler.Inflater.DataPosFromWorld(flagPos);
				((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetDataPos(DataPos);
				((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetWorldPos(flagPos + Vector3.down * 1.0F);
				if(Input.GetKeyDown(KeyCode.Mouse0)){
					((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetDataPos(Handler.Inflater.DataPosFromWorld(flagPos));
					((FlagData)FlagBeingAnnotated.GetComponent(typeof(FlagData))).SetWorldPos(flagPos + Vector3.down * 1.0F);
					PlacingFlag = false;
					SelectingFlag = true;
				}
			}
		}
	}

	public void DeleteAllFlags(){
		PlacingFlag = false;
		AnnotatingFlag = false;
		SelectingFlag = false;
		SelectedFlag = null;
	}
	public void DeleteAllGrids(){
		NamingGrid = false;
		SelectedGrid = null;
	}
	
	public void OnGUI(){
		// Home Button
		MenuHandler.GUIDrawRect(new Rect(tw*0.1f, h - th*6.5f, tw*1.1f, th*1.5f), new Color(0,0,0,0.65f));
		if(GUI.Button(new Rect(tw*0.1f, h - th*6.5f, tw*1.1f, th*1.5f), "Home")){
			Handler.ReadyCamHome();
			Handler.MoveCamHome();
		}
		
		// Help Button
		MenuHandler.GUIDrawRect(new Rect(tw*0.1f, h - th*4.2f, tw*1.1f, th*1.5f), new Color(Color.red.r,Color.red.g,Color.red.b,0.65f));
		if(Handler.GetState() == MenuHandler.State.StateMultiplayer){
			if(GUI.Button(new Rect(tw*0.1f, h - th*4.2f, tw*1.1f, th*1.5f), "Disconnect")){
				Handler.Multiplayer.Disconnect();
				Handler.TabsMulti.RemoveAxisGrids();
				Handler.TabsMulti.RemoveAxisGridLabels();
				Handler.TabsMulti.RemoveDataPointsExternal();
				Handler.TabsMulti.RemoveSurfaceGrid();
				Handler.DeleteAllFlags();
				Handler.DeleteAllGrids();
				Handler.SetState(MenuHandler.State.StateInitial);
				Handler.ControlHandler.SetEnabled(false);
			}
		}else{
			if(GUI.Button(new Rect(tw*0.1f, h - th*4.2f, tw*1.1f, th*1.5f), "Help")){
				Handler.SetState(MenuHandler.State.StateHelp);
				Handler.ControlHandler.SetEnabled(false);
			}
		}

		// Exit Button
		MenuHandler.GUIDrawRect(new Rect(tw*0.1f, h - th*2f, tw*1.1f, th*1.5f), new Color(Color.red.r,Color.red.g,Color.red.b,0.65f));
		if(GUI.Button(new Rect(tw*0.1f, h - th*2f, tw*1.1f, th*1.5f), "Exit to Desktop")){
			Application.Quit();
		}
		
		switch (CurrentTabState){
		case TabState.TabFlag:
			GUIFlags();
			break;
			
		case TabState.TabGrid:
			GUIGrids();
			break;

		case TabState.TabDataPoints:
			GUIDataPoints();
			break;

		case TabState.TabAxisGrid:
			GUIExternalAxis();
			break;

		case TabState.TabSettings:
			GUISettings();
			break;
			
		case TabState.TabDefault:
			GUITabs();
			break;
		}
	}
	
	protected void SetTabState(TabState state){this.CurrentTabState = state;}
	
	protected void GUITabs(){
		if(GUI.Button(new Rect(w - tw*1.2f, h/2 - th*1.5f, tw*1.2f, th), "<< Flags")){
			SetTabState(TabState.TabFlag);
		}
		if(GUI.Button(new Rect(w - tw*1.2f, h/2 - th*0.5f, tw*1.2f, th), "<< Grids")){
			SetTabState(TabState.TabGrid);
		}
		if(GUI.Button(new Rect(w - tw*1.2f, h/2 + th*0.5f, tw*1.2f, th), "<< Data Points")){
			SetTabState(TabState.TabDataPoints);
		}
		if(GUI.Button(new Rect(w - tw*1.2f, h/2 + th*1.5f, tw*1.2f, th), "<< Axis")){
			SetTabState(TabState.TabAxisGrid);
		}
		if(GUI.Button(new Rect(w - tw*1.2f, h/2 + th*2.5f, tw*1.2f, th), "<< Settings")){
			SetTabState(TabState.TabSettings);
		}
	}
	
	protected virtual void GUISettings(){}
	
	protected virtual void GUIGrids(){}
	
	protected virtual void GUIFlags(){}

	// Data Points on/off
	protected bool ShowDataPoints = false;
	protected int MaxNumDataPoints = 200;
	protected int NumDataPoints = 100;

	protected virtual void GUIDataPoints(){
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
				AddDataPoints(NumDataPoints);
			ShowDataPoints = true;
		}
		// Button NO
		if(!ShowDataPoints)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), "No")){
			if(ShowDataPoints)
				RemoveDataPoints();
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
			ResetDataPoints();
		}
	}
	#region Data Point Functions
	/// <summary>
	/// Adds the data points.
	/// </summary>
	/// <param name="x">number of data points to show.</param>
	public void AddDataPoints(float x){
		RemoveDataPoints();
		DataHandler dH = Handler.Inflater.dH;
		List<GameObject> toAddPoints = new List<GameObject>();
		List<double[]> data = dH.GetData();

		int iterator = x > 2000 ? (int)(data.Count/1000) : (int)(data.Count/x);
		for(int i=0; i<data.Count; i+= iterator){
			float Y = (float)(data[i][1]/dH.GetMaxY());
			Vector3 pos = new Vector3((float)(data[i][0])*Handler.Inflater.theterrain.terrainData.size.x,
			                          (float)(Y)*Handler.Inflater.theterrain.terrainData.size.y,
			                          (float)(1-data[i][2])*Handler.Inflater.theterrain.terrainData.size.z);
			Debug.Log((float)(data[i][0])*Handler.Inflater.theterrain.terrainData.size.x + "    " +
			          (float)(Y)*Handler.Inflater.theterrain.terrainData.size.y + "    " +
			          (float)(data[i][2])*Handler.Inflater.theterrain.terrainData.size.z);
			Debug.Log((float)(data[i][0]) + "     " + 
			          (float)(Y) + "      " + 
			          (float)(data[i][2]));
			GameObject temp = (GameObject) Main.Instantiate (Handler.Inflater.DataPoint, pos, Quaternion.identity);
			toAddPoints.Add(temp);
		}
		Handler.DataPoints = toAddPoints;
	}
	
	public void RemoveDataPoints(){
		for(int i=0; i<Handler.DataPoints.Count; i++){
			Main.Destroy(Handler.DataPoints[i]);
		}
		Handler.DataPoints = new List<GameObject>();
	}

	public void ResetDataPoints(){
		DataHandler dH = Handler.Inflater.dH;
		float tempRatio = ((float)NumDataPoints)/((float)MaxNumDataPoints);
		MaxNumDataPoints = dH.GetData().Count;
		NumDataPoints = (int)(MaxNumDataPoints*tempRatio);

		if(ShowDataPoints){
			RemoveDataPoints();
			AddDataPoints(NumDataPoints);
		}else{
			RemoveDataPoints();
		}
	}

	#endregion

	//Surface Grid on/off
	protected bool ShowSurfaceGrid = false;
	protected float MinGridX = 0f;
	protected float MaxGridX = 1f;
	protected float MinGridZ = 0f;
	protected float MaxGridZ = 1f;
	protected float GridSizeX = 1f;
	protected float GridSizeZ = 1f;
	protected float MinY = 0f, MaxY = 1f;
	protected string GridSizeXString = "";
	protected string GridSizeZString = "";

	protected virtual void GUISurfaceGrid(){
		if(GUI.Button(new Rect(w - tw*3.5f, h*0.5f - th*0.5f, tw*0.5f, th), " >> ")){
			SetTabState(TabState.TabDefault);
		}
		GUI.Box(new Rect(w - tw*3f, th*7f, tw*3f, h - th*7f), "");
		
		GUI.Label(new Rect(w - tw*2f, h/2 - th*1.5f, tw, th), "Show Surface Grid?");
		// Button YES
		if(ShowSurfaceGrid)
			MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), "Yes")){
			if(!ShowSurfaceGrid)
				ApplySurfaceGridValues(GridSizeX, GridSizeZ);
			ShowSurfaceGrid = true;
		}
		// Button NO
		if(!ShowSurfaceGrid)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), "No")){
			if(ShowSurfaceGrid)
				RemoveSurfaceGrid();
			ShowSurfaceGrid = false;
		}
		// Grid Size Z:
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(GridSizeZString)).x/2f, h/2f + th, tw*2f, th),
		          "Grid Size Latitude: " + GridSizeZString);
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*2.75f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*2.75f, tw, th), String.Format("{0:0.000}", (MaxGridZ-MinGridZ)));
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*1.9f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		GridSizeZ = GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*2f, tw*2, th), GridSizeZ, 0, MaxGridZ-MinGridZ);
		GridSizeZString = String.Format("{0:0.000}", GridSizeZ);

		// Grid Size X:
		GUI.Label(new Rect(w - tw*2f - GUI.skin.GetStyle("Label").CalcSize(new GUIContent(GridSizeXString)).x/2f, h/2f + th*3.5f, tw*2f, th),
		          "Grid Size Longitude: " + GridSizeXString);
		GUI.Label(new Rect(w - tw*2.5f, h/2f + th*5.25f, tw, th), "0");
		GUI.Label(new Rect(w - tw*0.6f, h/2f + th*5.25f, tw, th), String.Format("{0:0.000}", (MaxGridX-MinGridX)));
		MenuHandler.GUIDrawRect(new Rect(w - tw*2.6f, h/2 + th*4.4f, tw*2.2f, th*0.75f), new Color(1f,1f,1f,0.25f));
		GridSizeX = GUI.HorizontalSlider(new Rect(w - tw*2.5f, h/2 + th*4.5f, tw*2, th), GridSizeX, 0, MaxGridX-MinGridX);
		GridSizeXString = String.Format("{0:0.000}", GridSizeX);
		
		if(GUI.Button(new Rect(w-tw*2.25f, h/2 + th*5.75f, tw*1.5f, th*1.5f), "Regenerate Grid")){
			ResetSurfaceGrid();
		}
	}

	#region Surface Grid Functions
	public void ApplySurfaceGridValues(float gridSizeValueX, float gridSizeValueZ){
		float proportionX = (float)((gridSizeValueX)/(AxisMaxGridX - AxisMinGridX));
		float proportionZ = (float)((gridSizeValueZ)/(AxisMaxGridZ - AxisMinGridZ));
		if(gridSizeValueX == 0)
			proportionX = 1f;
		if(gridSizeValueZ == 0)
			proportionZ = 1f;
		ApplySurfaceGrid(proportionX, proportionZ);
	}
	
	public void ApplySurfaceGrid(float gridSizeProportionX, float gridSizeProportionZ){
		RemoveSurfaceGrid();
		Terrain t = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		SplatPrototype[] splat = new SplatPrototype[1];
		splat[0] = new SplatPrototype();
		splat[0].texture = GenerateSurfaceGridTex(gridSizeProportionX,gridSizeProportionZ);
		splat[0].tileOffset = new Vector2(0, 0);
		splat[0].tileSize = new Vector2(t.terrainData.size.x, t.terrainData.size.z);
		t.terrainData.splatPrototypes = null;
		t.terrainData.splatPrototypes = splat;
		
	}

	private Texture2D GenerateSurfaceGridTex(float gridSizeProportionX, float gridSizeProportionZ){
		Terrain t = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		Texture2D cMapTex = t.terrainData.splatPrototypes[0].texture;
		
		Texture2D surfaceTex = new Texture2D(cMapTex.width, cMapTex.height);
		surfaceTex.SetPixels(cMapTex.GetPixels());
		float separatorX = (float)(cMapTex.width*gridSizeProportionX);
		float separatorZ = (float)(cMapTex.height*gridSizeProportionZ);
		
		for(float x=0; x<surfaceTex.width; x+= separatorX){
			for(float y=0; y<surfaceTex.height; y++){
				surfaceTex.SetPixel((int)x,(int)y,Color.white);
			}
		}
		for(float z=surfaceTex.width-1; z>=0; z-= separatorZ){
			for(float y=0; y<surfaceTex.height; y++){
				surfaceTex.SetPixel((int)y,(int)z,Color.white);
			}
		}
		
		return surfaceTex;
	}
	public void ResetSurfaceGrid(){
		float tempRatioX = (AxisGridSizeX)/(AxisMaxGridX-AxisMinGridX);
		float tempRatioZ = (AxisGridSizeZ)/(AxisMaxGridZ-AxisMinGridZ);
		MinGridX = Math.Abs(Handler.Inflater.Vars.MIN_Z);
		MinGridZ = Math.Abs(Handler.Inflater.Vars.MIN_X);
		MaxGridX = Math.Abs(Handler.Inflater.Vars.MAX_Z);
		MaxGridZ = Math.Abs(Handler.Inflater.Vars.MAX_X);
		GridSizeX = tempRatioX*(MaxGridX-MinGridX);
		GridSizeZ = tempRatioZ*(MaxGridZ-MinGridZ);
		GridSizeXString = String.Format("{0:0.00}", GridSizeX);
		GridSizeZString = String.Format("{0:0.00}", GridSizeZ);
		if(ShowSurfaceGrid){
			RemoveSurfaceGrid();
			ApplySurfaceGridValues(GridSizeX, GridSizeZ);
		}else{
			RemoveSurfaceGrid();
		}
	}

	public void RemoveSurfaceGrid(){
		Handler.StateSingleplayer.RegenerateColorSplat();
	}
	#endregion
	
	public bool ShowAxisGrid = false;
	public bool ShowAxisGridLabels = false;
	public float AxisGridSizeX=1f, AxisGridSizeZ=1f,AxisGridSizeY=1f,
	AxisGridSizeXIT=1f,AxisGridSizeZIT=1f,AxisGridSizeYIT=1f,
	AxisGridSizeXRounded=1f, AxisGridSizeZRounded=1f,AxisGridSizeYRounded=1f;
	protected float AxisMinGridX = 0f;
	protected float AxisMaxGridX = 1f;
	protected float AxisMinGridZ = 0f;
	protected float AxisMaxGridZ = 1f;
	protected float AxisMinGridY = 0f;
	protected float AxisMaxGridY = 1f;
	protected string AxisGridSizeXString = "";
	protected string AxisGridSizeZString = "";
	protected string AxisGridSizeYString = "";

	protected virtual void GUIExternalAxis(){
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
			if(!ShowAxisGrid){
				AddAxisGridsValues(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
				ApplySurfaceGridValues(AxisGridSizeXRounded, AxisGridSizeZRounded);
			}
			ShowAxisGrid = true;
		}
		// Button NO
		if(!ShowAxisGrid)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*3f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*3f, tw*0.45f, th), "No")){
			if(ShowAxisGrid){
				RemoveAxisGrids();
				RemoveSurfaceGrid();
			}
			ShowAxisGrid = false;
		}
		#endregion

		#region Axis Grid LABELS Yes/No
		GUI.Label(new Rect(w - tw*2.25f, h/2 - th*1.5f, tw*1.5f, th), "Show Axis Labels?");
		// Button YES
		if(ShowAxisGridLabels)
			MenuHandler.GUIDrawRect(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*2f, h/2 - th*0.5f, tw*0.45f, th), "Yes")){
			if(!ShowAxisGridLabels)
				AddAxisGridLabelsValues(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
			ShowAxisGridLabels = true;
		}
		// Button NO
		if(!ShowAxisGridLabels)
			MenuHandler.GUIDrawRect(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), new Color(0.38f, 0.57f, 0.72f, 0.55f));
		if(GUI.Button(new Rect(w - tw*1.55f, h/2f - th*0.5f, tw*0.45f, th), "No")){
			if(ShowAxisGridLabels){
				RemoveAxisGridLabels();
			}
			ShowAxisGridLabels = false;
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
			ResetAxisGrid();
		}
	}

	#region External Axis Grids Functions

	protected GameObject AxisGridLeftWall;
	protected GameObject AxisGridBackWall;
	protected GameObject AxisGridBottomWall;

	#region Networking Calls

	public void AddAxisValues(float x, float y, float z){
		AxisGridSizeX = x;
		AxisGridSizeY = y;
		AxisGridSizeZ = z;
		ResetAxisStatics();
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		AxisGridSizeXString = String.Format("{0:0.000}", AxisGridSizeXRounded);
		AxisGridSizeYString = String.Format("{0:0.000}", AxisGridSizeYRounded);
		AxisGridSizeZString = String.Format("{0:0.000}", AxisGridSizeZRounded);
		AddAxisGridsValues(x,y,z);
	}

	public void AddAxisLabelsValues(float x, float y, float z){
		AxisGridSizeX = x;
		AxisGridSizeY = y;
		AxisGridSizeZ = z;
		ResetAxisStatics();
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		AxisGridSizeXString = String.Format("{0:0.000}", AxisGridSizeXRounded);
		AxisGridSizeYString = String.Format("{0:0.000}", AxisGridSizeYRounded);
		AxisGridSizeZString = String.Format("{0:0.000}", AxisGridSizeZRounded);
		AddAxisGridLabelsValues(x,y,z);
	}

	public void ResetAxisValues(float x, float y, float z){
		AxisGridSizeX = x;
		AxisGridSizeY = y;
		AxisGridSizeZ = z;
		ResetAxisStatics();
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		AxisGridSizeXString = String.Format("{0:0.000}", AxisGridSizeXRounded);
		AxisGridSizeYString = String.Format("{0:0.000}", AxisGridSizeYRounded);
		AxisGridSizeZString = String.Format("{0:0.000}", AxisGridSizeZRounded);
		ResetAxisGrid();
	}

	public void AddDataPointsValue(int x){
		ShowDataPoints = true;
		NumDataPoints = x;
		AddDataPoints(x);
	}

	public void RemoveDataPointsExternal(){
		ShowDataPoints = false;
		RemoveDataPoints();
	}

	public void ResetDataPointsValue(int x){
		NumDataPoints = x;
		ResetDataPoints();
	}

	#endregion

	protected void AddAxisGridsValues(float valueX, float valueY, float valueZ){
		float proportionX = (float)((valueX)/(AxisMaxGridX - AxisMinGridX));
		float proportionY = (float)((valueY)/(AxisMaxGridY - AxisMinGridY));
		float proportionZ = (float)((valueZ)/(AxisMaxGridZ - AxisMinGridZ));
		if(valueX == 0)
			proportionX = 1f;
		if(valueY == 0)
			proportionY = 1f;
		if(valueZ == 0)
			proportionZ = 1f;
		AddAxisGrids(proportionX, proportionY, proportionZ);
	}

	protected void AddAxisGridLabelsValues(float valueX, float valueY, float valueZ){
		float proportionX = (float)((valueX)/(AxisMaxGridX - AxisMinGridX));
		float proportionY = (float)((valueY)/(AxisMaxGridY - AxisMinGridY));
		float proportionZ = (float)((valueZ)/(AxisMaxGridZ - AxisMinGridZ));
		if(valueX == 0)
			proportionX = 1f;
		if(valueY == 0)
			proportionY = 1f;
		if(valueZ == 0)
			proportionZ = 1f;
		AddAxisGridLabels(proportionX, proportionY, proportionZ);
	}

	protected void AddAxisGrids(float proportionX, float proportionY, float proportionZ){

		#region Attempt using Line Renderer
		Terrain ter = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		Vector3 tSize = ter.terrainData.size;
		Vector3 tPos = ter.transform.position;
		GameObject t = GameObject.Find("Terrain");
		int lineRendLength = 999;
		LineRenderer lineRend;
		lineRend = t.GetComponent<LineRenderer>();
		if(lineRend == null)
			lineRend = t.AddComponent<LineRenderer>();
		lineRend.material = new Material(Shader.Find("Particles/Additive"));
		lineRend.SetColors(new Color(0.0f,1.0f,0.0f,0.30f), new Color(0.0f,1.0f,0.0f,0.30f));
		lineRend.SetWidth(1f, 1f);
		lineRend.SetVertexCount(lineRendLength);

		// Inital Box (cardinal axis) Surrounding Terrain
		lineRend.SetPosition(0, tPos);
		lineRend.SetPosition(1, new Vector3(tPos.x+tSize.x,tPos.y,tPos.z)); //bottom right down
		lineRend.SetPosition(2, new Vector3(tPos.x+tSize.x,tPos.y,tPos.z+tSize.z));// top right down
		lineRend.SetPosition(3, new Vector3(tPos.x,tPos.y,tPos.z+tSize.z)); // top left down
		lineRend.SetPosition(4, new Vector3(tPos.x,tPos.y,tPos.z)); // bottom left down
		lineRend.SetPosition(5, new Vector3(tPos.x,tPos.y+tSize.y,tPos.z)); // bottom left up
		lineRend.SetPosition(6, new Vector3(tPos.x,tPos.y+tSize.y,tPos.z+tSize.z)); // top left up
		lineRend.SetPosition(7, new Vector3(tPos.x,tPos.y,tPos.z+tSize.z)); // top left down
		lineRend.SetPosition(8, new Vector3(tPos.x,tPos.y+tSize.y,tPos.z+tSize.z)); // top left up
		lineRend.SetPosition(9, new Vector3(tPos.x+tSize.x,tPos.y+tSize.y,tPos.z+tSize.z)); // top right up
		lineRend.SetPosition(10, new Vector3(tPos.x+tSize.x,tPos.y,tPos.z+tSize.z)); // top right down
		lineRend.SetPosition(11, new Vector3(tPos.x+tSize.x,tPos.y+tSize.y,tPos.z+tSize.z)); // top right up
		lineRend.SetPosition(12, new Vector3(tPos.x,tPos.y+tSize.y,tPos.z+tSize.z)); // top left up
		lineRend.SetPosition(13, new Vector3(tPos.x,tPos.y,tPos.z+tSize.z)); // top left down

		int n = 14; // last index of LineRenderer
		Vector3 xStart = new Vector3(tPos.x,tPos.y,tPos.z + tSize.z),
		xDir = new Vector3(1f,0,0),
		yStart = new Vector3(tPos.x,tPos.y,tPos.z+tSize.z),
		yDir = new Vector3(0,1f,0),
		zStart = new Vector3(tPos.x,tPos.y,tPos.z+tSize.z),
		zDir = new Vector3(0,0,-1f);

		// X Lines
		float iteratorX = proportionX*tSize.x;
		if(iteratorX == 0)
			iteratorX = 8f;
		for(float i=0; i<tSize.x; i+=iteratorX){
			Vector3 tempPos1 = xStart + xDir*i;
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos2 = tempPos1 + new Vector3(0,tSize.y,0);
			lineRend.SetPosition(++n,tempPos2);
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos3 = tempPos1 + new Vector3(0,0,-tSize.z);
			lineRend.SetPosition(++n,tempPos3);
			lineRend.SetPosition(++n,tempPos1);
		}

		// Y Lines
		float iteratorY = proportionY*tSize.y;
		if(iteratorY == 0)
			iteratorY = 8f;
		for(float i=0; i<tSize.y; i+=iteratorY){
			Vector3 tempPos1 = yStart + yDir*i;
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos2 = tempPos1 + new Vector3(0,0,-tSize.z);
			lineRend.SetPosition(++n,tempPos2);
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos3 = tempPos1 + new Vector3(tSize.x,0,0);
			lineRend.SetPosition(++n,tempPos3);
			lineRend.SetPosition(++n,tempPos1);
		}

		// Z Lines
		float iteratorZ = proportionZ*tSize.z;
		if(iteratorZ == 0)
			iteratorZ = 8f;
		for(float i=0; i<tSize.z; i+=iteratorZ){
			Vector3 tempPos1 = zStart + zDir*i;
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos2 = tempPos1 + new Vector3(0,tSize.y,0);
			lineRend.SetPosition(++n,tempPos2);
			lineRend.SetPosition(++n,tempPos1);
			Vector3 tempPos3 = tempPos1 + new Vector3(tSize.x,0,0);
			lineRend.SetPosition(++n,tempPos3);
			lineRend.SetPosition(++n,tempPos1);
		}

		#endregion
	}

	public void AddAxisGridLabels(float proportionX, float proportionY, float proportionZ){
		Terrain ter = (Terrain)GameObject.Find("Terrain").GetComponent<Terrain>();
		Vector3 tSize = ter.terrainData.size;
		Vector3 tPos = ter.transform.position;
		float iteratorX = proportionX*tSize.x;
		float iteratorY = proportionY*tSize.y;
		float iteratorZ = proportionZ*tSize.z;
		Vector3 xStart = new Vector3(tPos.x,tPos.y+(iteratorY/2f)+tSize.y,tPos.z + tSize.z),
		xDir = new Vector3(1f,0,0),
		yStart1 = new Vector3(tPos.x,tPos.y+(iteratorY/2f),tPos.z+tSize.z),
		yStart2 = new Vector3(tPos.x+tSize.x,tPos.y+(iteratorY/2f),tPos.z+tSize.z),
		yStart3 = new Vector3(tPos.x,tPos.y+(iteratorY/2f),tPos.z),
		yDir = new Vector3(0,1f,0),
		zStart = new Vector3(tPos.x,tPos.y+(iteratorY/2f)+tSize.y,tPos.z+tSize.z),
		zDir = new Vector3(0,0,-1f);

		
		float DataXStart = Handler.Inflater.Vars.MIN_Z,
		DataXIt = (Handler.Inflater.Vars.MAX_Z - Handler.Inflater.Vars.MIN_Z)*(iteratorX/tSize.x),
		DataYStart = AxisMinGridY,
		DataYIt = (AxisMaxGridY - AxisMinGridY)*(iteratorY/tSize.y),
		DataZStart = Handler.Inflater.Vars.MIN_X,
		DataZIt = (Handler.Inflater.Vars.MAX_X - Handler.Inflater.Vars.MIN_X)*(iteratorZ/tSize.z);

		float scale = 4f;
		Vector3 offsetX = new Vector3(0,0,scale*scale),
		offsetY1 = new Vector3(-scale*scale,0,0),
		offsetY2 = new Vector3(scale*scale,0,0),
		offsetY3 = new Vector3(0,0,-scale*scale),
		offsetZ = new Vector3(-scale*scale,0,0);

		// X Lines
		for(float i=iteratorX; i<tSize.x; i+=iteratorX){
			Vector3 tempPos = xStart + xDir*i + offsetX;
			float tempDataPos = DataXStart + DataXIt*(i/iteratorX);
			GameObject temp = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,tempPos, Handler.MainCamera.transform.rotation);
			temp.GetComponent<TextMesh>().text = String.Format("{0:0.00}", tempDataPos);
			temp.transform.localScale = new Vector3(scale,scale,scale);
			Handler.AxisLabels.Add(temp);
		}
		
		// Y Lines
		for(float i=iteratorY; i<tSize.y; i+=iteratorY){
			Vector3 tempPos1 = yStart1 + yDir*i + offsetY1;
			Vector3 tempPos2 = yStart2 + yDir*i + offsetY2;
			Vector3 tempPos3 = yStart3 + yDir*i + offsetY3;
			float tempDataPos = DataYStart + DataYIt*(i/iteratorY);
			GameObject temp1 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,tempPos1, Handler.MainCamera.transform.rotation);
			GameObject temp2 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,tempPos2, Handler.MainCamera.transform.rotation);
			GameObject temp3 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,tempPos3, Handler.MainCamera.transform.rotation);
			temp1.GetComponent<TextMesh>().text = String.Format("{0:0.00}", tempDataPos);
			temp1.transform.localScale = new Vector3(scale,scale,scale);
			Handler.AxisLabels.Add(temp1);
			temp2.GetComponent<TextMesh>().text = String.Format("{0:0.00}", tempDataPos);
			temp2.transform.localScale = new Vector3(scale,scale,scale);
			Handler.AxisLabels.Add(temp2);
			temp3.GetComponent<TextMesh>().text = String.Format("{0:0.00}", tempDataPos);
			temp3.transform.localScale = new Vector3(scale,scale,scale);
			Handler.AxisLabels.Add(temp3);
		}
		
		// Z Lines
		for(float i=iteratorZ; i<tSize.z; i+=iteratorZ){
			Vector3 tempPos = zStart + zDir*i + offsetZ;
			float tempDataPos = DataZStart + DataZIt*(i/iteratorZ);
			GameObject temp = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,tempPos, Handler.MainCamera.transform.rotation);
			if(Handler.Inflater.dH.GetColumnZName().ToLower().Contains("date")){
				temp.GetComponent<TextMesh>().text = GetDateString(tempDataPos);
			}else{
				temp.GetComponent<TextMesh>().text = String.Format("{0:0.00}", tempDataPos);
			}
			temp.transform.localScale = new Vector3(scale,scale,scale);
			Handler.AxisLabels.Add(temp);
		}

		// Axis Names:
		Vector3 XColumnPos1 = new Vector3(tPos.x + tSize.x/2f, tPos.y + tSize.y/4f, tPos.z - 2.5f*scale*scale);
		Vector3 XColumnPos2 = new Vector3(tPos.x + tSize.x/2f, tPos.y + tSize.y + 2.5f*scale*scale, tPos.z + tSize.z);
		Vector3 YColumnPos1 = new Vector3(tPos.x + tSize.x + scale*scale, tPos.y + tSize.y + scale*scale, tPos.z + tSize.z);
		Vector3 YColumnPos2 = new Vector3(tPos.x, tPos.y + tSize.y + scale*scale, tPos.z - scale*scale);
		Vector3 ZColumnPos1 = new Vector3(tPos.x, tPos.y + tSize.y + 2.5f*scale*scale, tPos.z + tSize.z/2f);
		Vector3 ZColumnPos2 = new Vector3(tPos.x + tSize.x + 2*scale*scale, tPos.y + tSize.y/4f, tPos.z + tSize.z/2f);
		GameObject tempXName1 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,XColumnPos1, Handler.MainCamera.transform.rotation);
		tempXName1.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnXName();
		tempXName1.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);
		GameObject tempXName2 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,XColumnPos2, Handler.MainCamera.transform.rotation);
		tempXName2.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnXName();
		tempXName2.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);

		GameObject tempYName1 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,YColumnPos1, Handler.MainCamera.transform.rotation);
		tempYName1.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnYName();
		tempYName1.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);

		GameObject tempYName2 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,YColumnPos2, Handler.MainCamera.transform.rotation);
		tempYName2.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnYName();
		tempYName2.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);
		
		GameObject tempZName1 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,ZColumnPos1, Handler.MainCamera.transform.rotation);
		tempZName1.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnZName();
		tempZName1.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);
		GameObject tempZName2 = (GameObject)Main.Instantiate(Handler.Inflater.AxisLabel,ZColumnPos2, Handler.MainCamera.transform.rotation);
		tempZName2.GetComponent<TextMesh>().text = Handler.Inflater.dH.GetColumnZName();
		tempZName2.transform.localScale = new Vector3(scale*1.2f,scale*1.2f,scale*1.2f);
		
		Handler.AxisLabels.Add(tempXName1);
		Handler.AxisLabels.Add(tempXName2);
		Handler.AxisLabels.Add(tempYName1);
		Handler.AxisLabels.Add(tempYName2);
		Handler.AxisLabels.Add(tempZName1);
		Handler.AxisLabels.Add(tempZName2);
	}

	private string GetDateString(float dateValue){
		string month = ((int)(dateValue % 12)+1).ToString();
		string year = Math.Floor(((double)dateValue)/12d).ToString();
		string dateString = "15/" + month + "/" + year;
		return dateString;
	}

	public void ResetAxisGrid(){
		DataHandler dH = Handler.Inflater.dH;
		float tempRatioX = (AxisGridSizeX)/(AxisMaxGridX-AxisMinGridX);
		float tempRatioZ = (AxisGridSizeZ)/(AxisMaxGridZ-AxisMinGridZ);
		float tempRatioY = (AxisGridSizeY)/(AxisMaxGridY-AxisMinGridY);
		AxisMinGridX = Math.Abs(Handler.Inflater.Vars.MIN_Z);
		AxisMinGridZ = Math.Abs(Handler.Inflater.Vars.MIN_X);
		AxisMinGridY = 0f;
		AxisMaxGridX = Math.Abs(Handler.Inflater.Vars.MAX_Z);
		AxisMaxGridZ = Math.Abs(Handler.Inflater.Vars.MAX_X);
		AxisMaxGridY = (float)Math.Abs(dH.GetMaxY());
		AxisGridSizeX = tempRatioX*(AxisMaxGridX-AxisMinGridX);
		AxisGridSizeY = tempRatioY*(AxisMaxGridY-AxisMinGridY);
		AxisGridSizeZ = tempRatioZ*(AxisMaxGridZ-AxisMinGridZ);
		AxisGridSizeXIT = GetGridIterator(AxisMaxGridX-AxisMinGridX);
		AxisGridSizeYIT = GetGridIterator(AxisMaxGridY-AxisMinGridY);
		AxisGridSizeZIT = GetGridIterator(AxisMaxGridZ-AxisMinGridZ);
		AxisGridSizeXRounded = RoundToNearest(AxisGridSizeX, AxisGridSizeXIT);
		AxisGridSizeYRounded = RoundToNearest(AxisGridSizeY, AxisGridSizeYIT);
		AxisGridSizeZRounded = RoundToNearest(AxisGridSizeZ, AxisGridSizeZIT);
		AxisGridSizeXString = String.Format("{0:0.000}", AxisGridSizeXRounded);
		AxisGridSizeYString = String.Format("{0:0.000}", AxisGridSizeYRounded);
		AxisGridSizeZString = String.Format("{0:0.000}", AxisGridSizeZRounded);
		MinY = 0f;
		MaxY = (float)dH.GetMaxY();

		if(ShowAxisGrid){
			RemoveAxisGrids();
			AddAxisGridsValues(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
			RemoveSurfaceGrid();
			ApplySurfaceGridValues(AxisGridSizeXRounded, AxisGridSizeZRounded);
		}
		if(ShowAxisGridLabels){
			RemoveAxisGridLabels();
			AddAxisGridLabelsValues(AxisGridSizeXRounded, AxisGridSizeYRounded, AxisGridSizeZRounded);
		}
	}

	private void ResetAxisStatics(){
		DataHandler dH = Handler.Inflater.dH;
		AxisMinGridX = Math.Abs(Handler.Inflater.Vars.MIN_Z);
		AxisMinGridZ = Math.Abs(Handler.Inflater.Vars.MIN_X);
		AxisMinGridY = 0f;
		AxisMaxGridX = Math.Abs(Handler.Inflater.Vars.MAX_Z);
		AxisMaxGridZ = Math.Abs(Handler.Inflater.Vars.MAX_X);
		AxisMaxGridY = (float)Math.Abs(dH.GetMaxY());
		AxisGridSizeXIT = GetGridIterator(AxisMaxGridX-AxisMinGridX);
		AxisGridSizeYIT = GetGridIterator(AxisMaxGridY-AxisMinGridY);
		AxisGridSizeZIT = GetGridIterator(AxisMaxGridZ-AxisMinGridZ);
		MinY = 0f;
		MaxY = (float)dH.GetMaxY();
	}
	
	public void RemoveAxisGrids(){
		GameObject t = GameObject.Find("Terrain");
		Main.DestroyImmediate(t.GetComponent<LineRenderer>());
	}

	public void RemoveAxisGridLabels(){
		for(int i=0; i<Handler.AxisLabels.Count; i++){
			Main.DestroyImmediate(Handler.AxisLabels[i]);
		}
		Handler.AxisLabels = new List<GameObject>();
	}

	#endregion

	public static float GetGridIterator(float max){
		float r = max/50f;
		if(r<0.002f)
			return 0.001f;
		else if(r<0.02f)
			return 0.005f;
		else if(r<=0.2f)
			return 0.05f;
		else if(r<=2f)
			return 0.5f;
		else if(r<=20f)
			return 1f;
		else if(r<=100)
			return 10f;
		else
			return 100f;
	}

	public static float RoundToNearest(float passednumber, float roundto)
	{
		
		// 105.5 up to nearest 1 = 106
		// 105.5 up to nearest 10 = 110
		// 105.5 up to nearest 7 = 112
		// 105.5 up to nearest 100 = 200
		// 105.5 up to nearest 0.2 = 105.6
		// 105.5 up to nearest 0.3 = 105.6
		
		//if no rounto then just pass original number back
		if (roundto == 0)
		{
			return passednumber;
		}
		else
		{
			return (float)(Math.Round((double)(passednumber / roundto))) * roundto;
		}
	}
}