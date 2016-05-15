using UnityEngine;
using System.Collections;
using System.IO;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseLook : MonoBehaviour {

	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityX = 15F;
	public float sensitivityY = 15F;

	public float minimumX = -360F;
	public float maximumX = 360F;

	public float minimumY = -60F;
	public float maximumY = 60F;

	float rotationY = 0F;

	bool menu = false;
	bool menuterrain = false;
	bool menucolour = false;
	public Terrain heights;
	public Terrain google;

	SplatPrototype[] tex1;
	SplatPrototype[] tex2;
	SplatPrototype[] tex3;

	void Update ()
	{

		if(enabled)
		{
			if (axes == RotationAxes.MouseXAndY)
			{
				float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
				
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
			}
			else if (axes == RotationAxes.MouseX)
			{
				transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			}
			else
			{
				rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
				rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
				
				transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
			}

			if(Input.GetKey(KeyCode.Mouse0))
			{
				this.transform.Translate(Vector3.forward * 2f);
			}

			if(Input.GetKey(KeyCode.Mouse1))
			{
				this.transform.Translate(Vector3.back * 2f);
			}
		}
	}
	
	void Start ()
	{
		// Make the rigid body not change rotation
		if (rigidbody)
			rigidbody.freezeRotation = true;

		tex1 = new SplatPrototype[1];
		tex2 = new SplatPrototype[1];
		tex3 = new SplatPrototype[1];
		
		tex1[0] = new SplatPrototype();
		tex1[0].texture = (Texture2D)Resources.Load("VicColourFat",typeof(Texture2D));
		tex1[0].tileOffset = new Vector2(0, 0);
		tex1[0].tileSize = new Vector2(512, 512);
		
		tex2[0] = new SplatPrototype();
		tex2[0].texture = (Texture2D)Resources.Load("VicColourAge",typeof(Texture2D));
		tex2[0].tileOffset = new Vector2(0, 0);
		tex2[0].tileSize = new Vector2(512, 512);
		
		tex3[0] = new SplatPrototype();
		tex3[0].texture = (Texture2D)Resources.Load("VicColourSum",typeof(Texture2D));
		tex3[0].tileOffset = new Vector2(0, 0);
		tex3[0].tileSize = new Vector2(512, 512);
	}
	
	void OnGUI()
	{
		if(menu)
		{
			if(GUI.Button(new Rect(Screen.width/2.5f, 20, 200, 50), "Turn Terrain On/Off"))
			{
				if(heights.enabled)
					heights.enabled = false;
				else
					heights.enabled = true;
			}

			if(GUI.Button(new Rect(Screen.width/2.5f, 70, 200, 50), "Turn Google Map On/Off"))
			{
				if(google.enabled)
					google.enabled = false;
				else
					google.enabled = true;
			}

			if(GUI.Button(new Rect(Screen.width/2.5f, 120, 200, 50), "Change Terrain"))
			{
				menuterrain = true;
				menu = false;
			}

			if(GUI.Button(new Rect(Screen.width/2.5f, 170, 200, 50), "Change Colour Map"))
			{
				//Change overlay
				menucolour = true;
				menu = false;
			}

			if(GUI.Button(new Rect(Screen.width/2.5f, 220, 200, 50), "Exit Menu"))
			{
			
				menu = false;
			}

			
		} 
		if(menuterrain)
		{
			if(GUI.Button(new Rect(Screen.width/2.5f, 20, 200, 50), "Fatalities"))
			{ 
				Application.LoadLevel(2);
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 70, 200, 50), "Average Age"))
			{ 
				Application.LoadLevel(1);
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 120, 200, 50), "Accidents Summary"))
			{ 
				Application.LoadLevel(0);
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 170, 200, 50), "Back"))
			{ 
				menuterrain = false; 
				menu = true; 
			} 
			if(GUI.Button(new Rect(Screen.width/2.5f, 220, 200, 50), "Exit Menu"))
			{
				
				menuterrain = false;
			}
		} 
		if(menucolour)
		{
			if(GUI.Button(new Rect(Screen.width/2.5f, 20, 200, 50), "Fatalities"))
			{
				heights.terrainData.splatPrototypes = null;
				heights.terrainData.splatPrototypes = tex1;

				//Interface output of what the colours represent
				/*GUI.Label(new Rect(0,0,200,30), "Green: 0 to " + (int)(190/alphamulti));
				GUI.Label(new Rect(0,30,200,30), "Yellow: " + (int)(191/alphamulti) + " to " + (int)(220/alphamulti));
				GUI.Label(new Rect(0,60,200,30), "Orange: " + (int)(221/alphamulti) + " to " + (int)(230/alphamulti));
				GUI.Label(new Rect(0,90,200,30), "Red: " + (int)(231/alphamulti) + " to " + (int)(255/alphamulti));*/
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 70, 200, 50), "Average Age"))
			{
				heights.terrainData.splatPrototypes = null;
				heights.terrainData.splatPrototypes = tex2;

				//Interface output of what the colours represent
				/*GUI.Label(new Rect(0,0,200,30), "Green: 0 to " + (int)(100/alphamulti));
				GUI.Label(new Rect(0,30,200,30), "Yellow: " + (int)(101/alphamulti) + " to " + (int)(150/alphamulti2));
				GUI.Label(new Rect(0,60,200,30), "Orange: " + (int)(151/alphamulti) + " to " + (int)(199/alphamulti));
				GUI.Label(new Rect(0,90,200,30), "Red: " + (int)(200/alphamulti) + " to " + (int)(255/alphamulti));*/
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 120, 200, 50), "Accidents Summary"))
			{
				heights.terrainData.splatPrototypes = null;
				heights.terrainData.splatPrototypes = tex3;

				//Interface output of what the colours represent
				/*GUI.Label(new Rect(0,0,200,30), "Green: 0 to 1");
				GUI.Label(new Rect(0,30,200,30), "Yellow: 1 to " + (int)(10/alphamulti));
				GUI.Label(new Rect(0,60,200,30), "Orange: " + (int)(11/alphamulti) + " to " + (int)(50/alphamulti3));
				GUI.Label(new Rect(0,90,200,30), "Red: " + (int)(51/alphamulti) + " to " + (int)(255/alphamulti));*/
			}
			
			if(GUI.Button(new Rect(Screen.width/2.5f, 170, 200, 50), "Back"))
			{ 
				menucolour = false; 
				menu = true; 
			} 
			if(GUI.Button(new Rect(Screen.width/2.5f, 220, 200, 50), "Exit Menu"))
			{
				
				menucolour = false;
			}
		}
	}
}