using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
public class PhoneController : Controller
{
	Camera MainCamera;
	Transform T;
	float Speed;
	Vector3 CurAcc;
	Quaternion FacingDirection;
	private NetworkController NetControl;
	
	public PhoneController (Camera MainCamera)
	{
		this.MainCamera = MainCamera;
		this.T = MainCamera.transform;
		this.NetControl = new NetworkController();
		NetControl.StartServer();
	}

	public override void Update(){
		if(enabled){
			T.position = T.position + (T.forward * Speed/10);
			T.RotateAround(T.position, Vector3.up, CurAcc.x*2f);
			T.RotateAround(T.position, T.right, CurAcc.y*2f);
		}
	}
	
	public override void SetEnabled(bool val){
		enabled = val;
	}

	public override void Destroy(){
		NetControl.CloseServer();
		enabled = false;
	}

	#region RPC Calls propogated through networkView (Main.cs):

	public void UpdatePhoneData(Vector3 acc){
		//T.localEulerAngles = new Vector3(-acc.y*90f, acc.x*90f, 0);
		CurAcc = acc;
	}

	public void SetSpeed(float speed){
		this.Speed = speed;
	}

	#endregion

	#region Network Control
	public class NetworkController {
		public string ConnectingMessage{ get; set; }
		int ServerPort = 25002;
		string server_ip;
		private const string TypeName = "DeakinDataVis_PhoneControl";
		private string RoomName; 
			
		public void StartServer(){
			NetworkConnectionError e = Network.InitializeServer (20, ServerPort, !Network.HavePublicAddress());

			RoomName = System.Environment.UserName;
			MasterServer.RegisterHost(TypeName,RoomName);

			Debug.Log("Multiplayer Server return: " + e);
		}

		public void CloseServer()
		{
			ConnectingMessage = "";
			Network.Disconnect(	250 );
		}
	}
	#endregion
}