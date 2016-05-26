using System;
using UnityEngine;
public class KeyboardController : Controller
{
	MouseLook look;

	/// <summary>
	/// Initializes a new instance of the <see cref="KeyboardController"/> class.
	/// Initially control is disabled.
	/// </summary>
	/// <param name="MainCamera">The main instance of Camera in the scene given it has component <see cref="MouseLook"/>.</param>
	public KeyboardController (Camera MainCamera)
	{
		look = MainCamera.GetComponent<MouseLook>();
		enabled = false;
		look.enabled = false;
	}

	/// <summary>
	/// Update method for KeyboardController.
	/// </summary>
	public override void Update(){
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(!enabled)
			{
				look.enabled = true;
				enabled = true;
			}
			else
			{
				look.enabled = false;
				enabled = false;
			}
		}
	}

	/// <summary>
	/// Enables/disables the controller.
	/// </summary>
	/// <param name="val">Enabled if <c>true</c>, Disabled if <c>false</c>.</param>
	public override void SetEnabled(bool val){
		look.enabled = val;
		enabled = val;
	}

	/// <summary>
	/// Destroy this instance of Controller.
	/// </summary>
	public override void Destroy(){
		look.enabled = false;
	}
}