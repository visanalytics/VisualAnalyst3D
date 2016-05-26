using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
public class PhoneController : Controller
{
	Transform T;
	float Speed;
	Vector3 CurAcc;
	Quaternion FacingDirection;
	private NetworkController NetControl;

	/// <summary>
	/// Initializes a new instance of the <see cref="PhoneController"/> class and starts 
	/// server to which the phone can connect. 
	/// </summary>
	/// <param name="MainCamera">Main camera.</param>
	public PhoneController (Camera MainCamera)
	{
		this.T = MainCamera.transform;
		this.NetControl = new NetworkController();
		NetControl.StartServer();
	}

	/// <summary>
	/// Update method of Phone Controller.
	/// </summary>
	public override void Update(){
		if(enabled){
			T.position = T.position + (T.forward * Speed/10);
			T.RotateAround(T.position, Vector3.up, CurAcc.x*2f);
			T.RotateAround(T.position, T.right, CurAcc.y*2f);
		}
	}

	/// <summary>
	/// Enables/disables the controller.
	/// </summary>
	/// <param name="val">If set to <c>true</c> value.</param>
	public override void SetEnabled(bool val){
		enabled = val;
	}

	/// <summary>
	/// Destroy this instance of Controller.
	/// </summary>
	public override void Destroy(){
		NetControl.CloseServer();
		enabled = false;
	}

	#region RPC Calls propogated through networkView (Main.cs):

	/// <summary>
	/// Updates the phone data.
	/// </summary>
	/// <param name="acc">Acc.</param>
	public void UpdatePhoneData(Vector3 acc){
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
			
		/// <summary>
		/// Starts the server hosting the phone controller.
		/// </summary>
		public void StartServer(){
			NetworkConnectionError e = Network.InitializeServer (20, ServerPort, !Network.HavePublicAddress());

			RoomName = System.Environment.UserName;
			MasterServer.RegisterHost(TypeName,RoomName);

			Debug.Log("Multiplayer Server return: " + e);
		}

		/// <summary>
		/// Closes the server hosting the phone controller.
		/// </summary>
		public void CloseServer()
		{
			ConnectingMessage = "";
			Network.Disconnect(	250 );
		}
	}
	#endregion
}