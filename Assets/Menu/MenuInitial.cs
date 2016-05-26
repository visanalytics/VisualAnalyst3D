using System;
using UnityEngine;
public class MenuInitial
{
	MenuHandler Handler;
	float w = Screen.width;
	float h = Screen.height;
	// "tile" width and height, for easy GUI creation 
	float tw = 100f;
	float th = 20f;

	private enum State {StateInitial=0,StateMultiplayerSelection=1,StateMasterServerInput=2,StateConnecting=3, StateManualConnect=4};
	State CurrentState;

	/** Multiplayer **/
	HostData[] hostList;
	string ServerIPFieldText = "";

	public MenuInitial (MenuHandler Handler)
	{
		this.Handler = Handler;
		this.CurrentState = State.StateInitial;
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
			}

			#endregion
			break;

			case State.StateMultiplayerSelection:
			#region Multiplayer Server Selection
			if (GUI.Button(new Rect(boxX+(boxWidth/2) - tw*1.1f, boxY + th/2, tw, th*1.5f), "Refresh Hosts")){
				Handler.Multiplayer.RefreshHostList();
			}if (GUI.Button(new Rect(boxX+(boxWidth/2), boxY + th/2, tw*1.5f, th*1.5f), "Connect Manually")){
				CurrentState = State.StateManualConnect;
			}
			hostList = MasterServer.PollHostList();
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(boxX+(boxWidth/2) - tw*0.75f, (boxY + th*3f) + (th* 1.1f * i), tw*1.5f, th), hostList[i].gameName)){
						Handler.Multiplayer.ConnectAsClient(hostList[i]);
						CurrentState = State.StateConnecting;
					}
				}
			}
			if(Handler.Multiplayer.MasterServerFailedToConnect){
				GUI.Label(new Rect(boxX+boxWidth*0.5f - tw*1f, boxY+boxHeight*0.5f - th, tw*2.5f, th), "Failed to connect to master server.");
				if(GUI.Button(new Rect(boxX+boxWidth*0.5f - tw*0.75f, boxY+boxHeight*0.5f, tw*1.5f, th), "Connect Manually")){
					CurrentState = State.StateManualConnect;
				}
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
			#endregion
			break;
		}

		// Return Button & Exit Button
		if(GUI.Button(new Rect(w/2 - tw*1.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Back To Help")){
			CurrentState = State.StateInitial;
			Handler.SetState(MenuHandler.State.StateHelp);
		}

		if(GUI.Button(new Rect(w/2 + tw*0.25f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Exit to Desktop")){
			Application.Quit();
		}
	}
}