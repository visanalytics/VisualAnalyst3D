using UnityEngine;
using System.Collections;

public class ControllerHandler {

	public enum ControllerType {Keyboard=1,Phone=2};
	public Controller CurControl;

	Camera MainCamera;

	public ControllerHandler(ControllerHandler.ControllerType initialControl, Camera MainCamera){
		this.MainCamera = MainCamera;
		switch(initialControl){
			case ControllerType.Keyboard:
				this.CurControl = new KeyboardController(MainCamera);
				break;

			case ControllerType.Phone:
				this.CurControl = new PhoneController(MainCamera);
				break;
		}
	}

	public void SetControl(ControllerHandler.ControllerType controlType){
		this.CurControl.Destroy();
		switch(controlType){
			case ControllerType.Keyboard:
				this.CurControl = new KeyboardController(MainCamera);
				break;
				
			case ControllerType.Phone:
				this.CurControl = new PhoneController(MainCamera);
				break;
		}
	}

	public void SetEnabled(bool val){CurControl.SetEnabled(val);}

	public void Update () {
		//Handle input from current controller
		CurControl.Update();
	}

	private void EnableMenuGUI(){

	}
	private void DisableMenuGUI(){

	}
}
