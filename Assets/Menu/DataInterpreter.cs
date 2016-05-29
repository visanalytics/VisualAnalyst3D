using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;

/// <summary>
/// Data interpreter USER STORIES
/// As a user with my own data set, I want to
/// specify the filename of this data set in 
/// the form of a CSV and specify columns to plot.
/// I want to do this by utilising a tab in the
/// program's settings menu in Singleplayer or the
/// initial menu scene.
/// 
/// I want to be able to add presets for multiple 
/// combinations and permutations of the data.
/// 
/// In multiplayer mode, I want to import the data
/// and have it shared with other users on the
/// network.
/// </summary>

public class DataInterpreter
{
	private static String[][] PresetStrings = new String[][]{new String[]{"Smooth,Peaks", "BlueToYellowNeedles","0,0,200,0"},
	new String[]{"Smooth,Landscape", "BlueToYellow","0,0,200,7"},
	new String[]{"Cylindrical,Cylindrical", "BlueToYellowNeedles","0.5,100,0,0"},
	new String[]{"Granular,Peaks", "BlueToYellowNeedles","0.005,100,0,0"},
	new String[]{"Granular,Landscape", "BlueToYellowNeedles","0.05,100,0,0"}};

	public DataInterpreter ()
	{
	}
	
	public static string[] ImportHeaderList(string filename){
		StreamReader dataStream = new StreamReader(File.OpenRead(filename));
		string headers_line = dataStream.ReadLine();
		string[] headers_arr = headers_line.Split(',');
		return headers_arr;
	}

	/// <summary>
	/// Imports the file to the local filesystem and
	/// generates the data presets for terrain
	/// generation.
	/// </summary>
	/// <param name="preset_name">Preset_name.</param>
	/// <param name="data_name">Data_name.</param>
	/// <param name="filename">Filename.</param>
	/// <param name="map_filename">Map_filename.</param>
	/// <param name="columnXIndex">Column X index.</param>
	/// <param name="columnYIndex">Column Y index.</param>
	/// <param name="columnZIndex">Column Z index.</param>
	public static void ImportPreset(string preset_name, string data_name, string filename, string map_filename, int columnXIndex, int columnYIndex, int columnZIndex){
		StreamReader dataStream = new StreamReader(File.OpenRead(filename));
		string headers_line = dataStream.ReadLine();
		string[] headers = headers_line.Split(',');
		for(int i=0; i<headers.Length; i++){
			headers[i] = headers[i].Replace("\"", "").Replace("\n", "");
		}

		// Find Minimum/Maximum values of all data dimensions.
		int[] ColumnIndexes = {columnXIndex, columnYIndex, columnZIndex};
		double[] MinI = new double[ColumnIndexes.Length];
		double[] MaxI = new double[ColumnIndexes.Length];
		// Set first Min/Max
		string[] first_line = dataStream.ReadLine ().Split(',');
		for(int i=0; i<ColumnIndexes.Length; i++){
			MinI[i] = double.Parse(first_line[ColumnIndexes[i]]);
       		MaxI[i] = double.Parse(first_line[ColumnIndexes[i]]);
		}

		// Search through entire file to find Min/Max values for each dimension
		while(!dataStream.EndOfStream){
			string[] line_vals = dataStream.ReadLine().Split(',');
			for(int i=0; i<ColumnIndexes.Length; i++){
				MinI[i] = double.Parse(first_line[ColumnIndexes[i]]) < MinI[i] ? double.Parse(first_line[ColumnIndexes[i]]) : MinI[i];
				MaxI[i] = double.Parse(first_line[ColumnIndexes[i]]) < MaxI[i] ? double.Parse(first_line[ColumnIndexes[i]]) : MaxI[i];
			}
		}

		// Define preset strings
		string PresetOutputString = "";
		string Comma = ",";
		for(int i=0; i<PresetStrings.Length; i++){
			string temp = "";
			temp += preset_name + Comma; // Preset name
			temp += data_name + Comma; // Source name
			temp += PresetStrings[i][0] + Comma; // Terrain type & preset
			temp += "512,512" + Comma; // Heightmap size. colormap size
			temp += "1" + Comma; // Scale Y
			temp += filename + Comma; // Data filename
			temp += map_filename + Comma; // Map filename
			temp += MinI[0].ToString() + Comma; // Min X
			temp += MinI[2].ToString() + Comma; // Min Z
			temp += MaxI[0].ToString() + Comma; // Max X
			temp += MaxI[2].ToString() + Comma; // Max Z
			temp += PresetStrings[i][1] + Comma; // Color Preset
			temp += columnXIndex.ToString() + Comma; // Column X Index
			temp += columnYIndex.ToString() + Comma; // Column Y Index
			temp += columnZIndex.ToString() + Comma; // Column Z Index
			temp += PresetStrings[i][2]; // HillsMaxRadius,HillsIterationNumber,GeneticMaxIteration,GeneticSmoothIteration
		}
	}
}