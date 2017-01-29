using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class Main : MonoBehaviour {

	public Terrain InflatedTerrain;
	public Camera MainCamera;

	HeightmapInflater Inflater;
	MenuHandler Menu;

	void Start () {
		Inflater = InflatedTerrain.GetComponent<HeightmapInflater>();
		Menu = new MenuHandler(Inflater, MainCamera, networkView);
	}

	void Update () {
		Menu.Update();
	}

	void OnGUI(){
		Menu.OnGUI();
	}

	#region RPC Functions/Networking

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Menu.SetState(MenuHandler.State.StateInitial);
		if (info == NetworkDisconnection.LostConnection){
			Menu.ShowMessage("Lost connection to the server");
		}else{
			Menu.ShowMessage("Disconnected from the server");
			Menu.TabsMulti.RemoveAxisGrids();
			Menu.TabsMulti.RemoveAxisGridLabels();
			Menu.TabsMulti.RemoveDataPointsExternal();
			Menu.TabsMulti.RemoveSurfaceGrid();
			Menu.DeleteAllFlags();
			Menu.DeleteAllGrids();
			Menu.ControlHandler.SetEnabled(false);
		}
	}

	void OnFailedToConnect(NetworkConnectionError info){
		switch(info){
		case NetworkConnectionError.InvalidPassword:
			Menu.StateInitial.InvalidPasswordFlag();
			break;
		}
	}

	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		Menu.Multiplayer.MasterServerFailedToConnect = true;
		Network.Disconnect();
	}

	[RPC]
	void ChangeTerrain(NetworkPlayer player, string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		switch(TerrainType){
		case "Granular":
			Menu.GenerateGranular(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
			break;
			
			case "Smooth":
			Menu.GenerateSmooth(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
			break;
			
			case "Cylindrical":
			Menu.GenerateCylindrical(DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
			break;
		}
	}
	
	[RPC]
	void CreateFlag(NetworkPlayer player, int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation, string Col, int Tex){
		GameObject temp = Menu.CreateFlag(Annotation, WorldPosition);
		Color c;
		switch(Col){
		case MenuHandler.FC_BLUE:
			c = Color.blue;
			break;
		case MenuHandler.FC_RED:
			c = Color.red;
			break;
		case MenuHandler.FC_GREEN:
			c = Color.green;
			break;
		case MenuHandler.FC_YELLOW:
			c = Color.yellow;
			break;
		case MenuHandler.FC_WHITE:
			c = Color.white;
			break;
		default:
			c = Color.white;
			break;
		}
		temp.transform.Find("Cube").renderer.material.SetColor ("_Color", c);
		temp.transform.Find("Cube").renderer.material.mainTexture = (Texture2D)Resources.Load(Menu.flaghandler.FLAG_PATH[Tex] + "/" + Menu.flaghandler.FLAG_IMAGE[Tex]);
		
		temp.GetComponent<FlagData>().Init(ID, WorldPosition, DataPosition, Annotation);
	}
	[RPC]
	void DeleteFlag(int ID){
		for(int i=0; i<Menu.Flags.Count; i++){
			if(Menu.Flags[i].GetComponent<FlagData>().GetID() == ID){
				Destroy(Menu.Flags[i]);
				Menu.Flags.RemoveAt(i);
				break;
			}
		}
	}
	
	[RPC]
	void CreateGrid(NetworkPlayer player, int ID, int Orientation, string Name, int PositionIndex){
		// Orientation and Name are set in the INIT method of CreateGrid
		GameObject tempGridObj = Menu.CreateGrid(Name, Orientation);
		GridData tempData = tempGridObj.GetComponent<GridData>();
		
		tempData.SetID(ID);
		tempData.SetWorldPosIndex(PositionIndex);
		tempData.SetDataPosIndex(PositionIndex);
		tempData.SetName(Name);
	}
	[RPC]
	void DeleteGrid(int ID){
		for(int i=0; i<Menu.Grids.Count; i++){
			if(Menu.Grids[i].GetComponent<GridData>().GetID() == ID){
				Destroy(Menu.Grids[i]);
				Menu.Grids.RemoveAt(i);
				break;
			}
		}
	}
	
	[RPC]
	void UpdateGrid(NetworkPlayer player, int ID, int Index){
		for(int i=0; i<Menu.Grids.Count; i++){
			GridData g = Menu.Grids[i].GetComponent<GridData>();
			if(g.GetID() == ID){
				g.SetWorldPosIndex(Index);
				g.SetDataPosIndex(Index);
			}
		}
	}

	[RPC]
	void SetPlayer(NetworkPlayer player, float playerColorR, float playerColorG, float playerColorB)
	{
		if(player == Network.player){
			Menu.Multiplayer.MyPlayerObject = (GameObject)Network.Instantiate(Inflater.PlayerObject, MainCamera.transform.position, Quaternion.identity, 0);
			Menu.Multiplayer.MyPlayerObject.
				GetComponent<MultiplayerObject>().
					SetColor(playerColorR,playerColorG,playerColorB);
			Menu.Multiplayer.MyPlayerObject.renderer.enabled = false;
		}else{
			Menu.Multiplayer.MyPlayerObject.
				GetComponent<MultiplayerObject>().
					BroadcastColor();
		}
	}

	[RPC]
	void ToggleAxisOn(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		Menu.TabsMulti.ShowAxisGrid = true;
		Menu.TabsMulti.AddAxisValues(AxisValueX, AxisValueY, AxisValueZ);
		Menu.TabsMulti.ApplySurfaceGridValues(AxisValueX, AxisValueZ);
	}
	[RPC]
	void ToggleAxisOff(){
		Menu.TabsMulti.ShowAxisGrid = false;
		Menu.TabsMulti.RemoveAxisGrids();
		Menu.TabsMulti.RemoveSurfaceGrid();
	}
	[RPC]
	void ToggleAxisLabelsOn(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		Menu.TabsMulti.ShowAxisGridLabels = true;
		Menu.TabsMulti.AddAxisLabelsValues(AxisValueX, AxisValueY, AxisValueZ);
	}
	[RPC]
	void ToggleAxisLabelsOff(){
		Menu.TabsMulti.ShowAxisGridLabels = false;
		Menu.TabsMulti.RemoveAxisGridLabels();
	}

	[RPC]
	void ResetAxis(NetworkPlayer player, float AxisValueX, float AxisValueY, float AxisValueZ){
		Menu.TabsMulti.ResetAxisValues(AxisValueX,AxisValueY,AxisValueZ);
	}
	
	[RPC]
	void AddDataPoints(NetworkPlayer player, int NumDataPoints){
		Menu.TabsMulti.AddDataPointsValue(NumDataPoints);
	}
	
	[RPC]
	void RemoveDataPoints(){
		Menu.TabsMulti.RemoveDataPointsExternal();
	}
	
	[RPC]
	void ResetDataPoints(NetworkPlayer player, int NumDataPoints){
		Menu.TabsMulti.ResetDataPointsValue(NumDataPoints);
	}
	
	[RPC]
	void RemovePlayer(NetworkPlayer player){}
	
	#endregion

	#region Phone Controller RPC Calls

	[RPC]
	void UpdatePhoneData(Vector3 acc){
		((PhoneController)Menu.ControlHandler.CurControl).UpdatePhoneData(acc);
	}
	
	[RPC]
	void SetSpeed(float speed){
		((PhoneController)Menu.ControlHandler.CurControl).SetSpeed(speed);
	}

	[RPC]
	void SetEnabled(bool val){
		((PhoneController)Menu.ControlHandler.CurControl).SetEnabled(val);
	}

	[RPC]
	void DropFlag(NetworkPlayer player, string annotation){
		
		RaycastHit hit; 
		Ray ray = MainCamera.ScreenPointToRay(new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f));
		if(Physics.Raycast(ray, out hit, Mathf.Infinity)){
			Vector3 flagPos = hit.point;
			Vector3 DataPosition = Inflater.DataPosFromWorld(flagPos);
			Vector3 WorldPosition = flagPos + Vector3.down * 1.0F;
			GameObject temp = Menu.CreateFlag(annotation, WorldPosition);
			Color c = Color.white;
			temp.transform.Find("Cube").renderer.material.SetColor ("_Color", c);
			temp.transform.Find("Cube").renderer.material.mainTexture = (Texture2D)Resources.Load(Menu.flaghandler.FLAG_PATH[1] + "/" + Menu.flaghandler.FLAG_IMAGE[1]);
			
			temp.GetComponent<FlagData>().Init(Menu.R.Next(1,25000), WorldPosition, DataPosition, annotation);
		}
	}

	#region Client Calls (DISREGARD):

	[RPC]
	void DisconnectFromServer(string message){}

	#endregion

	#endregion
}
