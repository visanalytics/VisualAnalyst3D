using UnityEngine;
using System.Collections;

public abstract class Controller {
	public bool enabled;
	abstract public void SetEnabled(bool val);
	abstract public void Update();
	abstract public void Destroy();
}
