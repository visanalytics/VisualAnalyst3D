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

	/// <summary>
	/// Initializes a new instance of the <see cref="ColormapGen"/> class.
	/// </summary>
	/// <param name="heightmap">Heightmap texture to convert from.</param>
	/// <param name="colorSpectrumObj">Color spectrum object on which to model the colormap.</param>
	/// <param name="filenameToSave">Filename to save image to on disk.</param>
	public ColormapGen (Texture2D heightmap, ColorSpectrumObj colorSpectrumObj, String filenameToSave)
	{
		this.heightMap = heightmap;
		this.cso = colorSpectrumObj;
		this.filenameToSave = filenameToSave;
	}

	/// <summary>
	/// Returns the colormap associated with the heightmap passed in the constructor.
	/// </summary>
	/// <returns>The colormap texture.</returns>
	public Texture2D GetColormap(){
		Texture2D ColorMap = ConvertHeightmapToColormap();
		SaveToDisk(ColorMap);
		return ColorMap;
	}

	/// <summary>
	/// Converts the heightmap to a colormap with associated color spectrum.
	/// </summary>
	/// <returns>The colormap texture.</returns>
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

	/// <summary>
	/// Saves the colormap image to disk memory.
	/// </summary>
	/// <param name="img">Colormap image.</param>
	public void SaveToDisk(Texture2D img) {
		byte[] bytes = img.EncodeToPNG();
		File.WriteAllBytes(filenameToSave, bytes);
	}
}