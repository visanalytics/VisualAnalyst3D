using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;
public class ColormapGen
{
	Texture2D heightMap;
	ColorSpectrumObj cso;
	String filenameToSave;
//	double aspectWidth, aspectHeight; 
	public ColormapGen (Texture2D heightmap, ColorSpectrumObj colorSpectrumObj, String filenameToSave)
	{
		this.heightMap = heightmap;
		this.cso = colorSpectrumObj;
		this.filenameToSave = filenameToSave;
	}

	public Texture2D GetColormap(){
//		this.aspectWidth = bWidth/heightmap.width;
//		this.aspectHeight = bHeight/heightmap.height;
		Texture2D ColorMap = ConvertHeightmapToColormap();
		SaveToDisk(ColorMap);
		return ColorMap;
	}

	private Texture2D ConvertHeightmapToColormap(){
		Texture2D colorMap = new Texture2D(heightMap.width, heightMap.height);
		for(int x=0; x<heightMap.width; x++){
			for(int y=0; y<heightMap.height; y++){
				Color32 col = heightMap.GetPixel(x,y);
				Int32 mColor = col.r * 256*256 + col.g * 256 + col.b; //Returns the colour as an int
				float HeightVal = (float)mColor / (float)Math.Pow(2, 24);
				Color ColorVal = cso.GetColorFromVal(HeightVal);
				colorMap.SetPixel(x,y,ColorVal);
			}
		}
		return colorMap;
	}

	public void SaveToDisk(Texture2D img) {
		byte[] bytes = img.EncodeToPNG();
		File.WriteAllBytes(filenameToSave, bytes);
	}
}