using UnityEngine;
using System.Collections;

public abstract class Controller {
	public bool enabled;
	/// <summary>
	/// Enables/disables the controller.
	/// </summary>
	/// <param name="val">Enabled if <c>true</c>, Disabled if <c>false</c>.</param>
	abstract public void SetEnabled(bool val);
	/// <summary>
	/// Overridable update method for children of Controller.
	/// </summary>
	abstract public void Update();
	/// <summary>
	/// Destroy this instance of Controller.
	/// </summary>
	abstract public void Destroy();
}
