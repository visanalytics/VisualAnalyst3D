using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

public class MenuHandler
{

	public const String FC_BLUE = "Blue";
	public const String FC_RED = "Red";
	public const String FC_GREEN = "Green";
	public const String FC_YELLOW = "Yellow";
	public const String FC_WHITE = "White";
	public HeightmapInflater Inflater;
	public MultiplayerManager Multiplayer;

	public Camera MainCamera;
	bool camOrgReadyToChange = true; // True is camera is to be homed
	Vector3 camOrgPos;	// Camera's initial position
	Quaternion camOrgRot;	// Camera's initial rotation
	public ControllerHandler ControlHandler;

	public List<GameObject> Flags;
	public List<GameObject> Grids;
	public List<GameObject> DataPoints;
	public List<GameObject> AxisLabels;
	public List<GameObject> AxisGrids;
	public bool GridsVisible;

	public Variables Vars;
	
	public FlagHandler flaghandler;
	System.Random R;

	public MenuHandler (HeightmapInflater Inflater, Camera MainCamera, NetworkView nView)
	{
		this.Inflater = Inflater;
		this.MainCamera = MainCamera;
		this.Multiplayer = new MultiplayerManager(this, nView);
		this.R = new System.Random();
		this.ControlHandler = new ControllerHandler(ControllerHandler.ControllerType.Keyboard, MainCamera);
		Flags = new List<GameObject>();
		Grids = new List<GameObject>();
		DataPoints = new List<GameObject>();
		AxisLabels = new List<GameObject>();
		AxisGrids = new List<GameObject>();
		GridsVisible = true;
		SaveCamHome(MainCamera.transform.position, MainCamera.transform.rotation);
		flaghandler = new FlagHandler();
		InitGUI();
	}

	public enum State {StateInitial=0,StateMultiplayer=1,StateSingleplayer=2,StateHelp=3};
	private MenuHandler.State CurrentState;
	public MenuInitial StateInitial;
	public MenuHelp StateHelp;
	public MenuSinglePlayer StateSingleplayer;
	public MenuTabsSingleplayer TabsSingle;
	public MenuTabsMultiplayer TabsMulti;
	public MenuTabs Tabs;

	public MenuHandler.State GetState(){return this.CurrentState;}
	public void SetState(MenuHandler.State state){
		this.CurrentState = state;
	}

	// Initialises the GUI
	private void InitGUI(){
		StateInitial = new MenuInitial(this);
		StateHelp = new MenuHelp(this);
		StateSingleplayer = new MenuSinglePlayer(this);
		TabsSingle = new MenuTabsSingleplayer(this);
		TabsMulti = new MenuTabsMultiplayer(this);
		Tabs = TabsSingle;
		SetState(MenuHandler.State.StateHelp);
	}

	public void InitMultiplayer(){
		Tabs = TabsMulti;
		//Destroy all TabsSingle objects
		TabsSingle.RemoveAxisGrids();
		TabsSingle.RemoveAxisGridLabels();
		TabsSingle.RemoveDataPoints();
		TabsSingle = new MenuTabsSingleplayer(this);
	}
	public void InitSingleplayer(){
		Tabs = TabsSingle;
		//Destroy all TabsMulti objects
		TabsMulti = new MenuTabsMultiplayer(this);
	}
	
	public void OnGUI(){

		switch (CurrentState) {
			case State.StateInitial:
			StateInitial.OnGUI();
			break;

			case State.StateHelp:
			StateHelp.OnGUI();
			break;

			case State.StateSingleplayer:
			StateSingleplayer.OnGUI();
			Tabs.OnGUI();
			break;

			case State.StateMultiplayer:
			StateSingleplayer.OnGUI();
			Tabs.OnGUI();
			break;
		}

		if(ShowingMessage){
			GUIStyle style = new GUIStyle();
			style.fontSize = 18;
			double TimeSinceMessageStart = DateTime.Now.Subtract(MessageStart).TotalSeconds;
			if(TimeSinceMessageStart < MessageFadeTime){
				style.normal.textColor = Color.red;
				GUI.Label(new Rect(Screen.width/2f - style.CalcSize(new GUIContent(Message)).x/2f,
				                   Screen.height/4f - style.CalcSize(new GUIContent(Message)).y/2f,
				                   style.CalcSize(new GUIContent(Message)).x,
				                   style.CalcSize(new GUIContent(Message)).y),
				          Message,
				          style);
			}else if(TimeSinceMessageStart < MessageFadeTime + MessageStopTime){
				float alpha = (float)(1.0d - ((TimeSinceMessageStart - MessageFadeTime)/MessageStopTime));
				style.normal.textColor = new Color(1.0f,0.0f,0.0f,alpha);
				GUI.Label(new Rect(Screen.width/2f - style.CalcSize(new GUIContent(Message)).x/2f,
				                   Screen.height/4f - style.CalcSize(new GUIContent(Message)).y/2f,
				                   style.CalcSize(new GUIContent(Message)).x,
				                   style.CalcSize(new GUIContent(Message)).y), 
				          Message,
				          style);
			}else{
				ShowingMessage = false;
			}
		}
	}

	public void Update(){
		
		// Detecting keyboard actions
		ControlHandler.Update();
		if(GetState() == State.StateSingleplayer || GetState() == State.StateMultiplayer){
			Tabs.Update();
			if(GetState() == State.StateMultiplayer && Network.peerType == NetworkPeerType.Client)
				Multiplayer.Update();
		}

		for(int i=0; i<AxisLabels.Count; i++){
			AxisLabels[i].transform.rotation = MainCamera.transform.rotation;
		}
		for(int i=0; i<Flags.Count; i++){
			Flags[i].transform.LookAt(new Vector3(MainCamera.transform.position.x,
			                                      Flags[i].transform.position.y,
			                                      MainCamera.transform.position.z));
			Flags[i].transform.rotation *= Quaternion.Euler(new Vector3(0,180,0));
		}
	}

	bool ShowingMessage = false;
	string Message;
	DateTime MessageStart;
	double MessageFadeTime = 1.5d;
	double MessageStopTime = 1.5d;
	public void ShowMessage(string message){
		ShowingMessage = true;
		Message = message;
		MessageStart = DateTime.Now;
	}

	public GameObject CreateFlag(String annotation, Vector3 worldPos){
		GameObject temp = (GameObject)Main.Instantiate(Inflater.Flag, worldPos + Vector3.down * 1.0F,
				                                          Quaternion.identity);
		((FlagData)temp.GetComponent(typeof(FlagData))).SetID(R.Next(1,25000));
		Flags.Add(temp);
		UpdateFlags();
		return temp;
	}

	public void CreateFlag(int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation, string Col, int Tex){
		GameObject temp = (GameObject)Main.Instantiate(Inflater.Flag, WorldPosition + Vector3.down * 1.0F,
		                                          Quaternion.identity);
		Flags.Add(temp);
	}

	public GameObject CreateGrid(String name, int orientation){
		switch(orientation){
			case 0:
			return CreateGrid(name, GridData.Orientation.UP_Y);
		
			case 1:
			return CreateGrid(name, GridData.Orientation.UP_X);

			case 2:
			return CreateGrid(name, GridData.Orientation.UP_Z);

			default:
			return CreateGrid(name, GridData.Orientation.UP_Y);
		}
	}

	public GameObject CreateGrid(String name, GridData.Orientation orientation){
		DataHandler dH = Inflater.dH;

		GameObject temp = (GameObject) Main.Instantiate (Inflater.GridObject, new Vector3(), Quaternion.identity);
		float[][] dataBounds = new float[][]{
			new float[]{Inflater.Vars.MIN_X, Inflater.Vars.MAX_X},
			new float[]{(float)dH.GetMinY(), (float)dH.GetMaxY()},
			new float[]{Inflater.Vars.MIN_Z, Inflater.Vars.MAX_Z}
		};
		float[][] worldBounds = new float[][]{
			new float[]{0, Inflater.theterrain.terrainData.size.x}, 
			new float[]{0, Inflater.theterrain.terrainData.size.y},
			new float[]{0, Inflater.theterrain.terrainData.size.z}
		};
		((GridData)temp.GetComponent(typeof(GridData))).Init(R.Next(1,25000), "", dataBounds, worldBounds, false, orientation);
		Grids.Add(temp);
		return temp;
	}
	
	public void HideGrids(){
		for(int i=0; i<Grids.Count; i++){
			Grids[i].renderer.enabled = false;
		}
		GridsVisible = false;
	}

	public void UnhideGrids(){
		for(int i=0; i<Grids.Count; i++){
			Grids[i].renderer.enabled = true;
		}
		GridsVisible = true;
	}

	public void DeleteAllFlags(){
		if(Network.peerType == NetworkPeerType.Client){
			for(int i=0; i<Flags.Count; i++){
				Multiplayer.CallDeleteFlag(Flags[i].GetComponent<FlagData>().GetID());
				Main.Destroy(Flags[i]);
			}
			Flags.Clear();
			Tabs.DeleteAllFlags();
		}else{
			for(int i=0; i<Flags.Count; i++){
				Main.Destroy(Flags[i]);
			}
			Tabs.DeleteAllFlags();
			Flags.Clear();
		}
	}

	public void DeleteAllGrids(){
		if(Network.peerType == NetworkPeerType.Client){
			for(int i=0; i<Grids.Count; i++){
				Multiplayer.CallDeleteGrid(Grids[i].GetComponent<GridData>().GetID());
				Main.Destroy (Grids[i]);
			}
			Grids.Clear();
			Tabs.DeleteAllGrids();
		}else{
			for(int i=0; i<Grids.Count; i++){
				Main.Destroy(Grids[i]);
			}
			Tabs.DeleteAllGrids();
			Grids.Clear();
		}
	}

	public void UpdateFlags(){
		for(int i=0; i<Flags.Count; i++){
			GameObject temp = Flags[i];
			if(Inflater.TerrainVisible){
				Vector3 newPos = new Vector3(temp.transform.position.x, Inflater.theterrain.SampleHeight(temp.transform.position) - 1f, temp.transform.position.z);
				temp.transform.position = newPos;
				temp.GetComponent<FlagData>().SetWorldPos(newPos);
			}else{
				Vector3 newPos = new Vector3(temp.transform.position.x, Inflater.Map.SampleHeight(temp.transform.position) - 1f, temp.transform.position.z);
				temp.transform.position = new Vector3(temp.transform.position.x, Inflater.Map.SampleHeight(temp.transform.position) - 1f, temp.transform.position.z);
				temp.GetComponent<FlagData>().SetWorldPos(newPos);
			}
		}
	}

	public void UpdateGrids(){
		DataHandler dH = Inflater.dH;

		float[][] dataBounds = new float[][]{
			new float[]{Inflater.Vars.MIN_X, Inflater.Vars.MAX_X},
			new float[]{(float)dH.GetMinY(), (float)dH.GetMaxY()},
			new float[]{Inflater.Vars.MIN_Z, Inflater.Vars.MAX_Z}
		};
		float[][] worldBounds = new float[][]{
			new float[]{0, Inflater.theterrain.terrainData.size.x}, 
			new float[]{0, Inflater.theterrain.terrainData.size.y},
			new float[]{0, Inflater.theterrain.terrainData.size.z}
		};

		for(int i=0; i<Grids.Count; i++){
			GameObject GridObject = Grids[i];
			
			GridData temp = (GridData)GridObject.GetComponent(typeof(GridData));
			temp.Init(temp.GetID(), temp.GetName(), dataBounds, worldBounds, temp.IsSelected(), temp.GetOrientation(), true);
		}
	}

	private static Texture2D _staticRectTexture;
	private static GUIStyle _staticRectStyle;
	
	// Note that this function is only meant to be called from OnGUI() functions.
	public static void GUIDrawRect( Rect position, Color color )
	{
		if(_staticRectTexture == null)
		{
			_staticRectTexture = new Texture2D(1, 1);
		}
		
		if(_staticRectStyle == null)
		{
			_staticRectStyle = new GUIStyle();
		}
		
		_staticRectTexture.SetPixel(0, 0, color);
		_staticRectTexture.Apply();
		
		_staticRectStyle.normal.background = _staticRectTexture;
		
		GUI.Box(position, GUIContent.none, _staticRectStyle);
	}

	// Remembers that camera is to be placed home the next time terrain is generated
	public void ReadyCamHome() {
		camOrgReadyToChange = true; 
	}

	// Places the camera into its original home position
	public void SaveCamHome(Vector3 camPos, Quaternion camRot) {
		// Change the mpuose position to its home
		this.camOrgPos = camPos;
		this.camOrgRot = camRot;
		camOrgReadyToChange = false;
	}

	/// <summary>
	/// Places the camera into its original home position
	/// </summary>
	public void MoveCamHome() {
		if (camOrgReadyToChange) {
			// Change the mpuose position to its home
			this.MainCamera.transform.position = this.camOrgPos;
			this.MainCamera.transform.rotation = this.camOrgRot;
		}
		camOrgReadyToChange = false;
	}

	public void SetFlagImage(int choice, FlagHandler fh)
	{
		Texture tex = new Texture();
		tex = (Texture2D)Resources.Load(fh.FLAG_PATH[choice] + "/" + fh.FLAG_IMAGE[choice]);

		Flags[Flags.Count-1].transform.Find("Cube").renderer.material.mainTexture = tex;
	}

	public void SetFlagColour(Color c, String colorName)
	{
		Flags[Flags.Count-1].transform.Find("Cube").renderer.material.SetColor("_Color", c);
		Flags[Flags.Count-1].GetComponent<FlagData>().SetFlagColorString(colorName);
	}

	/// <summary>
	/// Used to transmit the message of terrain generation from
	/// <see cref="Main"/> to <see cref="MenuSinglePlayer"/>.
	/// </summary>
	public void GenerateGranular(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		StateSingleplayer.GenerateGranular(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
	}

	/// <summary>
	/// Used to transmit the message of terrain generation from
	/// <see cref="Main"/> to <see cref="MenuSinglePlayer"/>.
	/// </summary>
	public void GenerateSmooth(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		StateSingleplayer.GenerateSmooth(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
	}

	/// <summary>
	/// Used to transmit the message of terrain generation from
	/// <see cref="Main"/> to <see cref="MenuSinglePlayer"/>.
	/// </summary>
	public void GenerateCylindrical(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		StateSingleplayer.GenerateCylindrical(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
	}
}
