using System;
using UnityEngine;
public class MenuHelp
{
	MenuHandler Handler;
	float w = Screen.width;
	float h = Screen.height;
	// "tile" width and height, for easy GUI creation 
	float tw = 100f;
	float th = 20f;

	Texture2D Logo;

	public MenuHelp (MenuHandler handler)
	{
		this.Handler = handler;
		Logo = Resources.Load<Texture2D>("help_image");
	}

	public void OnGUI(){
		MenuHandler.GUIDrawRect(new Rect(0,0,w,h), new Color(0,0,0,0.5f));
		float imgWidth = h*0.9f;
		float WtoHratioLogo = (float)Logo.height/(float)Logo.width;
		float imgHeight = WtoHratioLogo * imgWidth;
		
		GUI.Box(new Rect(w/2f - h*0.45f - th, h*0.05f - th, h*0.9f + th*2f, h*0.9f + th*2f), "");
		float boxY = h*0.05f - th;
		float boxHeight = h*0.9f + th*2f;
		
		GUI.DrawTexture(new Rect(w/2f - h*0.45f, h*0.05f, imgWidth, imgHeight), Logo);
		
		String URLString = "http://visanalytics.org/va3d/";
		Rect URLRect = new Rect(w/2f - h*0.45f + imgWidth*0.27f, h*0.05f + imgHeight*0.58f, imgWidth*0.45f, imgHeight*0.05f);
		// URL Rect proportions:
		// X: imgX + 526/1920 * imgWidth
		// Y: imgY + 1118/1920 * imgHeight
		// Width: 872/1920 * imgWidth
		// Height: 98/1920 * imgHeight 
		if(Event.current.type == EventType.MouseUp && URLRect.Contains(Event.current.mousePosition))
			Application.OpenURL(URLString);
		
		// Return Button & Exit Button
		if(GUI.Button(new Rect(w/2 - tw*0.75f, boxY + boxHeight - th*1.5f, tw*1.5f, th*1.2f), "Continue")){
			Handler.SetState(MenuHandler.State.StateInitial);
			Handler.ControlHandler.SetEnabled(false);
		}
	}
}

