using UnityEngine;
using System.Collections;

public class ControllerHandler {

	public enum ControllerType {Keyboard=1,Phone=2};
	public Controller CurControl;

	Camera MainCamera;

	/// <summary>
	/// Initializes a new instance of the <see cref="ControllerHandler"/> class.
	/// </summary>
	/// <param name="initialControl">The controller type to use initially.</param>
	/// <param name="MainCamera">Main camera used by the program.</param>
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

	/// <summary>
	/// Sets the controller type to be used. Methods changing Controller.
	/// </summary>
	/// <param name="controlType">Control type. (Phone, Keyboard)</param>
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

	/// <summary>
	/// Enables/disables the current controller.
	/// </summary>
	/// <param name="val">Enabled if <c>true</c>, Disabled if <c>false</c>.</param>
	public void SetEnabled(bool val){CurControl.SetEnabled(val);}

	/// <summary>
	/// Update the current controller.
	/// </summary>
	public void Update () {
		//Handle input from current controller
		CurControl.Update();
	}
}
