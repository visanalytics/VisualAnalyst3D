using System;
using UnityEngine;
public class KeyboardController : Controller
{
	MouseLook look;
	Camera MainCamera;

	public KeyboardController (Camera MainCamera)
	{
		this.MainCamera = MainCamera;
		look = MainCamera.GetComponent<MouseLook>();
		enabled = false;
		look.enabled = false;
	}

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

	public override void SetEnabled(bool val){
		look.enabled = val;
		enabled = val;
	}

	public override void Destroy(){
		look.enabled = false;
	}
}