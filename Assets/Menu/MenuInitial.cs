using System;
using System.Collections.Generic;
using UnityEngine;
public class MenuInitial
{
	MenuHandler Handler;
	float w = Screen.width;
	float h = Screen.height;
	// "tile" width and height, for easy GUI creation 
	float tw = 100f;
	float th = 20f;

	private enum State {StateInitial=0,
		StateMultiplayerSelection=1,
		StateMasterServerInput=2,
		StateConnecting=3,
		StateManualConnect=4,
		StateMultiplayerPassword=5};
	State CurrentState;

	/** Multiplayer **/
	HostData[] hostList;
	HostData HostConnecting;
	string ServerIPFieldText = "";
	string ServerPasswordFieldText = "";
	string ServerPasswordBoxText = "";
	private readonly string[] PasswordBox = /*= new string[]*/{"",
		"Enter Password", "Invalid Password"};
	Vector2 scrollPosition;
	string ServerSearchFieldText = "";

	public MenuInitial (MenuHandler Handler)
	{
		this.Handler = Handler;
		this.CurrentState = State.StateInitial;
		this.scrollPosition = Vector2.zero;
		hostList = GetHostList();
	}

	public void OnGUI(){
		MenuHandler.GUIDrawRect(new Rect(0,0,w,h), new Color(0,0,0,0.5f));
		
		GUI.Box(new Rect(w/2f - h*0.45f - th, h*0.05f - th, h*0.9f + th*2f, h*0.9f + th*2f), "");
		float boxX = w/2f - h*0.45f - th,
		boxY = h*0.05f - th,
		boxWidth = h*0.9f + th*2f,
		boxHeight = h*0.9f + th*2f;

		switch(CurrentState){
			case State.StateInitial:
			#region Initial State

			if(GUI.Button(new Rect(boxX + (boxWidth/2)-tw*1.5f,boxY + boxHeight/2 - th/2, tw*1.25f, th*1.5f), "Singleplayer")){
				CurrentState = State.StateInitial;
				Handler.SetState(MenuHandler.State.StateSingleplayer);
				Handler.InitSingleplayer();
			}
			if(GUI.Button(new Rect(boxX + (boxWidth/2)+tw*0.25f,boxY + boxHeight/2 - th/2, tw*1.25f, th*1.5f), "Multiplayer")){
				CurrentState = State.StateMultiplayerSelection;
				Handler.Multiplayer.RefreshHostList();
				Handler.TabsSingle.RemoveAxisGrids();
				Handler.TabsSingle.RemoveAxisGridLabels();
				Handler.TabsSingle.RemoveDataPointsExternal();
				Handler.TabsSingle.RemoveSurfaceGrid();
				Handler.DeleteAllFlags();
				Handler.DeleteAllGrids();
				hostList = GetHostList();
			}
			if(GUI.Button(new Rect(w/2 - tw*1.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Back To Help")){
				CurrentState = State.StateInitial;
				Handler.SetState(MenuHandler.State.StateHelp);
			}
			#endregion
			break;

			case State.StateMultiplayerPassword:
			GUI.Box(new Rect(boxX + boxWidth*0.5f + tw*1f, boxY + boxHeight*0.5f - th*3f, tw*3f, th*6f), ServerPasswordBoxText);
			ServerPasswordFieldText = GUI.TextField(new Rect(boxX + boxWidth*0.5f + tw*1.2f, boxY + boxHeight*0.5f - th*2f, tw*2.6f, th*3.5f), ServerPasswordFieldText, 32);

			if(GUI.Button(new Rect(boxX + boxWidth*0.5f + tw*2.75f, boxY + boxHeight*0.5f + th*1.9f, tw*1f, th), "Enter")){
				Handler.Multiplayer.ConnectAsClient(HostConnecting, ServerPasswordFieldText);
				CurrentState = State.StateConnecting;
			}
			if(GUI.Button(new Rect(boxX + boxWidth*0.5f + tw*1.25f, boxY + boxHeight*0.5f + th*1.9f, tw*1f, th), "Cancel")){
				ResetPasswordField();
				CurrentState = State.StateMultiplayerSelection;
			}
			goto case State.StateMultiplayerSelection;

			case State.StateMultiplayerSelection:
			#region Multiplayer Server Selection
			if (GUI.Button(new Rect(boxX+(boxWidth/2) - tw*1.1f, boxY + th/2, tw, th*1.5f), "Refresh Hosts")){
				hostList = GetHostList();
			}if (GUI.Button(new Rect(boxX+(boxWidth/2), boxY + th/2, tw*1.5f, th*1.5f), "Connect Manually")){
				CurrentState = State.StateManualConnect;
			}
			ServerSearchFieldText = GUI.TextField(new Rect(boxX+(boxWidth/2) - tw*1.1f, boxY + th*2.2f, tw*1.5f, th*1f), ServerSearchFieldText);
			if (GUI.Button(new Rect(boxX+(boxWidth/2) + tw*0.5f, boxY + th*2.2f, tw*1f, th*1f), "Search")){
				hostList = GetHostList();
				hostList = RefineHostList(hostList, ServerSearchFieldText);
			}
			if(GUI.Button(new Rect(boxX+(boxWidth/2) + tw*1.55f, boxY + th*2.2f, tw*0.5f, th*1f), "Clear")){
				ServerSearchFieldText = "";
				hostList = GetHostList();
			}
			if (hostList != null)
			{
				if(hostList.Length * th * 1.1f < boxHeight - th*6f){
					for (int i = 0; i < hostList.Length; i++)
					{
						if(CurrentState == State.StateMultiplayerPassword){
							MenuHandler.GUIDrawRect(new Rect(boxX + boxWidth/2f - tw*0.75f, boxY + th*4f + (th * 1.1f * i), tw*1.5f, th), Color.red);
						}
						if (GUI.Button(new Rect(boxX + boxWidth/2f - tw*0.75f, boxY + th*4f + (th * 1.1f * i), tw*1.5f, th), hostList[i].gameName)){
							if(CurrentState != State.StateMultiplayerPassword){
								HostConnecting = hostList[i];
								Handler.Multiplayer.ConnectAsClient(hostList[i]);
								CurrentState = State.StateConnecting;
							}
						}
					}
				}else{
					float scrollHeight = hostList.Length * th * 1.1f < boxHeight - th*6f ? boxHeight - th*6f : hostList.Length * th * 1.1f;
					scrollPosition = GUI.BeginScrollView(
						new Rect(boxX+(boxWidth/2f) - tw, boxY + th*4f, tw*2f, boxHeight - th*6f),
						scrollPosition,
						new Rect(0, 0, tw*1.75f, scrollHeight),
						false,
						true);
					for (int i = 0; i < hostList.Length; i++)
					{
						if(CurrentState == State.StateMultiplayerPassword){
							MenuHandler.GUIDrawRect(new Rect(tw*0.25f, th * 1.1f * i, tw*1.5f, th), Color.red);
						}
						if (GUI.Button(new Rect(tw*0.25f, th * 1.1f * i, tw*1.5f, th), hostList[i].gameName)){
							if(CurrentState != State.StateMultiplayerPassword){
								HostConnecting = hostList[i];
								Handler.Multiplayer.ConnectAsClient(hostList[i]);
								CurrentState = State.StateConnecting;
							}
						}
					}
					GUI.EndScrollView();
				}
			}
			if(Handler.Multiplayer.MasterServerFailedToConnect){
				GUI.Label(new Rect(boxX+boxWidth*0.5f - tw*1f, boxY+boxHeight*0.5f - th, tw*2.5f, th), "Failed to connect to master server.");
				if(GUI.Button(new Rect(boxX+boxWidth*0.5f - tw*0.75f, boxY+boxHeight*0.5f, tw*1.5f, th), "Connect Manually")){
					CurrentState = State.StateManualConnect;
				}
			}
			if(GUI.Button(new Rect(w/2 - tw*1.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Back")){
				CurrentState = State.StateInitial;
			}
			#endregion
			break;

			case State.StateManualConnect:
			#region Multiplayer Server Connect Manually
			GUI.Label(new Rect(boxX+boxWidth*0.5f - tw*0.75f, boxY+boxHeight*0.5f - th*1.5f, tw*2, th), "Enter Server IP:");
			ServerIPFieldText = GUI.TextField(new Rect(boxX+boxWidth*0.5f - tw*0.75f, boxY+boxHeight*0.5f - th*0.5f, tw*1.5f, th), ServerIPFieldText, 32);
			if(GUI.Button(new Rect(boxX+boxWidth*0.5f - tw*0.5f, boxY+boxHeight*0.5f + th*0.75f, tw, th), "Connect")){
				Handler.Multiplayer.ConnectManual(ServerIPFieldText);
				CurrentState = State.StateConnecting;
			}
			if(GUI.Button(new Rect(w/2 - tw*1.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Back")){
				CurrentState = State.StateMultiplayerSelection;
			}
			#endregion
			break;

			case State.StateConnecting:
			#region Connecting to server
			if(Network.peerType == NetworkPeerType.Disconnected){
				GUI.Label(new Rect(boxX + boxWidth/2 - tw/2, boxY + boxHeight/2 - th*1.1f, tw, th*1.5f), "Connecting...");
				if(GUI.Button(new Rect(boxX + boxWidth/2 - tw/2, boxY + boxHeight/2 + th*0.1f, tw, th*1.15f), "Cancel")){
					Handler.Multiplayer.Disconnect();
					CurrentState = State.StateMultiplayerSelection;
				}
			}else{
				CurrentState = State.StateInitial;
				Handler.InitMultiplayer();
				Handler.SetState(MenuHandler.State.StateMultiplayer);
			}
			if(GUI.Button(new Rect(w/2 - tw*1.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Back")){
				CurrentState = State.StateMultiplayerSelection;
			}
			#endregion
			break;
		}

		// Exit Button
		if(GUI.Button(new Rect(w/2 + tw*0.25f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Exit to Desktop")){
			Application.Quit();
		}
	}

	public void InvalidPasswordFlag(){
		if(CurrentState == State.StateConnecting){
			if(ServerPasswordBoxText == PasswordBox[0]){
				ServerPasswordBoxText = PasswordBox[1];
			}else if(ServerPasswordBoxText == PasswordBox[1]){
				ServerPasswordBoxText = PasswordBox[2];
			}else{
				ServerPasswordBoxText = PasswordBox[2];
			}
			CurrentState = State.StateMultiplayerPassword;
		}
	}

	private void ResetPasswordField(){
		ServerPasswordBoxText = PasswordBox[0];
	}

	private HostData[] GetHostList(){
		Handler.Multiplayer.RefreshHostList();
		return MasterServer.PollHostList();

		// Test Multiple Server list
//		HostData[] temp = new HostData[70];
//		for(int i=0; i<70; i++){
//			HostData t = new HostData();
//			t.gameType = "DeakinDataVis";
//			t.gameName = "test_" + i.ToString();
//			t.ip = new string[]{"127.0.0.1"};
//			t.port = 25061;
//			temp[i] = t;
//		}
//		return temp;
	}

	private HostData[] RefineHostList(HostData[] hostList, string searchString){
		List<HostData> refinedHostListEqual = new List<HostData>();
		List<HostData> refinedHostListStarts = new List<HostData>();
		List<HostData> refinedHostListContains = new List<HostData>();
		for(int i=0; i<hostList.Length; i++){
			if(hostList[i].gameName.Equals(searchString)){
				refinedHostListEqual.Add(hostList[i]);
			}else if(hostList[i].gameName.StartsWith(searchString)){
				refinedHostListStarts.Add(hostList[i]);
			}else if(hostList[i].gameName.Contains(searchString)){
				refinedHostListContains.Add(hostList[i]);
			}
		}
		int resultNum = refinedHostListEqual.Count +
			refinedHostListStarts.Count +
				refinedHostListContains.Count;
		HostData[] refinedHostList = new HostData[resultNum];
		for(int i=0; i<refinedHostListEqual.Count; i++){
			refinedHostList[i] = refinedHostListEqual[i];
		}
		for(int i=0; i<refinedHostListStarts.Count; i++){
			refinedHostList[i + refinedHostListEqual.Count] = refinedHostListStarts[i];
		}
		for(int i=0; i<refinedHostListContains.Count; i++){
			refinedHostList[i + refinedHostListEqual.Count + refinedHostListStarts.Count] = refinedHostListContains[i];
		}
		return refinedHostList;
	}
}