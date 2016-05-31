using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerManager {
	int ServerPort = 25061;
	MenuHandler Handler;
	NetworkView nView;
	public NetworkPlayer ObjectOwner;
	public GameObject MyPlayerObject;
	public List<GameObject> PlayerObjects;
	public bool MasterServerFailedToConnect = false;

	public MultiplayerManager(MenuHandler Handler, NetworkView nView){
		this.Handler = Handler;
		this.nView = nView;
	}

	public void Update () {
		MyPlayerObject.GetComponent<MultiplayerObject>().SetPosition(Handler.MainCamera.transform.position);
	}

	#region Server Data Polling
	private const string TypeName = "DeakinDataVis";
	private HostData[] hostList;
	
	public void RefreshHostList()
	{
		MasterServer.RequestHostList(TypeName);
	}
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if( msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}
	public HostData[] GetHostList(){return hostList;}
	public void ConnectAsClient(HostData hostData){
		Network.Connect(hostData);
	}
	public void ConnectAsClient(HostData hostData, string password){
		Network.Connect(hostData, password);
	}
	public void ConnectManual(string hostIP){
		Network.Connect(hostIP, ServerPort);
	}

	public void Disconnect(){
		Network.Disconnect();
	}
	
	void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
		MasterServerFailedToConnect = true;
		Network.Disconnect();
	}

	#endregion

	#region RPC Calls to server

	public void CallChangeTerrain(string DataSource, string TerrainType, string Filename, string Preset, bool regen, float scale, float colorScale){
		nView.RPC("ChangeTerrain", RPCMode.Server, Network.player, DataSource, TerrainType, Filename, Preset, regen, scale, colorScale);
	}

	public void CallCreateFlag(int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation, string Col, int Tex){
		nView.RPC("CreateFlag", RPCMode.Others, Network.player, ID, WorldPosition, DataPosition, Annotation, Col, Tex);
	}
	public void CallDeleteFlag(int ID){
		nView.RPC ("DeleteFlag", RPCMode.Others, ID);
	}
	public void CallCreateGrid(int ID, int Orientation, string Name, int PositionIndex){
		nView.RPC("CreateGrid", RPCMode.Others, Network.player, ID, Orientation, Name, PositionIndex);
	}
	public void CallDeleteGrid(int ID){
		nView.RPC ("DeleteGrid", RPCMode.Others, ID);
	}

	public void CallUpdateGrid(int ID, int Index){
		nView.RPC("UpdateGrid", RPCMode.Others, Network.player, ID, Index);
	}

	public void CallExternalAxisOn(float AxisValueX, float AxisValueY, float AxisValueZ){
		nView.RPC("ToggleAxisOn", RPCMode.Others, Network.player, AxisValueX, AxisValueY, AxisValueZ);
	}

	public void CallExternalAxisOff(){
		nView.RPC("ToggleAxisOff", RPCMode.Others);
	}
	public void CallExternalAxisReset(float AxisValueX, float AxisValueY, float AxisValueZ){
		nView.RPC("ResetAxis", RPCMode.Others, Network.player, AxisValueX, AxisValueY, AxisValueZ);
	}

	public void CallExternalAxisLabelsOn(float AxisValueX, float AxisValueY, float AxisValueZ){
		nView.RPC("ToggleAxisLabelsOn", RPCMode.Others, Network.player, AxisValueX, AxisValueY, AxisValueZ);
	}

	public void CallExternalAxisLabelsOff(){
		nView.RPC("ToggleAxisLabelsOff", RPCMode.Others);
	}

	public void CallAddDataPoints(int NumDataPoints){
		nView.RPC("AddDataPoints", RPCMode.Others, Network.player, NumDataPoints);
	}

	public void CallRemoveDataPoints(){
		nView.RPC("RemoveDataPoints", RPCMode.Others);
	}

	public void CallResetDataPoints(int NumDataPoints){
		nView.RPC("ResetDataPoints", RPCMode.Others, Network.player, NumDataPoints);
	}

	#endregion
}
