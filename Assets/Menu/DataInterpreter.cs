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
		dataStream.Close();
		return headers_arr;
	}

	// Parse values from double format or date format to double value
	private static double ParseValue(string item){
		double value;
		try{
			value = double.Parse(item.Replace("\"", ""));
		}catch(Exception e){
			// item is a Date value
			if(item.Replace("\"", "").Contains("/")){
				string[] parts = item.Replace("\"", "").Split('/');
				int month = int.Parse(parts[1]);
				int year = int.Parse(parts[2]);
				value = month+(year*12);
			}else{
				throw new InvalidDataException("Invalid data selected.");
			}
		}
		return value;
	}
	
	public static void ImportPreset(string preset_name, string data_name, string filename, string map_filename,
	                                int columnXIndex, int columnYIndex, int columnZIndex,
	                                string columnXAlias, string columnYAlias, string columnZAlias){
		string[] map_split = map_filename.Split('.');
		string map_filename_suffix = map_split[map_split.Length-1];
		string map_filename_reduced = "/Heightmaps/Maps/" + data_name + "." + map_filename_suffix;
		if(!File.Exists(Application.dataPath + map_filename_reduced))
			File.Copy(map_filename, Application.dataPath + map_filename_reduced, true);
		// Copy data file
		string local_filename = "/Heightmaps/Import_Data/" + data_name + ".csv";
		if(!File.Exists(Application.dataPath + local_filename))
			File.Copy(filename, Application.dataPath + local_filename, true);

		FileStream readStream = File.OpenRead(filename);
		StreamReader dataStream = new StreamReader(readStream);
		// Find Minimum/Maximum values of all data dimensions.
		int[] ColumnIndexes = {columnXIndex, columnYIndex, columnZIndex};
		double[] MinI = new double[ColumnIndexes.Length];
		double[] MaxI = new double[ColumnIndexes.Length];
		// Set first Min/Max
		string[] headers = dataStream.ReadLine().Split(',');
		string[] first_line = dataStream.ReadLine().Split(',');
		
		for(int i=0; i<ColumnIndexes.Length; i++){
			try{
				MinI[i] = ParseValue(first_line[ColumnIndexes[i]]);//double.Parse(first_line[ColumnIndexes[i]].Replace("\"", ""));
				MaxI[i] = ParseValue(first_line[ColumnIndexes[i]]);//double.Parse(first_line[ColumnIndexes[i]].Replace("\"", ""));
			}catch(InvalidDataException e){
				return;
			}
		}
		
		// Search through entire file to find Min/Max values for each dimension
		while(!dataStream.EndOfStream){
			string[] line_vals = dataStream.ReadLine().Split(',');
			for(int i=0; i<ColumnIndexes.Length; i++){
				double temp = ParseValue(line_vals[ColumnIndexes[i]]);
				MinI[i] = temp < MinI[i] ? temp : MinI[i];//double.Parse(line_vals[ColumnIndexes[i]].Replace("\"", "")) < MinI[i] ? double.Parse(line_vals[ColumnIndexes[i]].Replace("\"", "")) : MinI[i];
				MaxI[i] = temp > MaxI[i] ? temp : MaxI[i];//double.Parse(line_vals[ColumnIndexes[i]].Replace("\"", "")) > MaxI[i] ? double.Parse(line_vals[ColumnIndexes[i]].Replace("\"", "")) : MaxI[i];
			}
		}
		dataStream.Close();
		readStream.Close();

		ImportPreset(preset_name, data_name, filename, map_filename, 
		             columnXIndex, columnYIndex, columnZIndex,
		             columnXAlias, columnYAlias, columnZAlias,
		             MinI[0], MinI[2], MaxI[0], MaxI[2]);
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
	public static void ImportPreset(string preset_name, string data_name, string filename, string map_filename,
		                                int columnXIndex, int columnYIndex, int columnZIndex, string columnXAlias,
	                                	string columnYAlias, string columnZAlias, double minX, double minZ, 
	                                	double maxX, double maxZ){
		// Move all files into working directory
		// Copy Map image:
		string[] map_split = map_filename.Split('.');
		string map_filename_suffix = map_split[map_split.Length-1];
		string map_filename_reduced = "/Heightmaps/Maps/" + data_name + "." + map_filename_suffix;
		if(!File.Exists(Application.dataPath + map_filename_reduced))
			File.Copy(map_filename, Application.dataPath + map_filename_reduced, true);
		// Copy data file
		string local_filename = "/Heightmaps/Import_Data/" + data_name + ".csv";
		if(!File.Exists(Application.dataPath + local_filename))
			File.Copy(filename, Application.dataPath + local_filename, true);

		// Define preset strings
		String presetFilename = Application.dataPath + "/Heightmaps/DataPresets.csv";
		StreamWriter sw = File.AppendText(presetFilename);

		string PresetOutputString = "";
		string Comma = ",";
		for(int i=0; i<PresetStrings.Length; i++){
			string temp = "";
			temp += preset_name + Comma; // Preset name
			temp += data_name + Comma; // Source name
			temp += PresetStrings[i][0] + Comma; // Terrain type & preset
			temp += "512,512" + Comma; // Heightmap size. colormap size
			temp += "1" + Comma; // Scale Y
			temp += local_filename + Comma; // Data filename
			temp += map_filename_reduced + Comma; // Map filename
			temp += minX.ToString() + Comma; // Min X
			temp += minZ.ToString() + Comma; // Min Z
			temp += maxX.ToString() + Comma; // Max X
			temp += maxZ.ToString() + Comma; // Max Z
			temp += PresetStrings[i][1] + Comma; // Color Preset
			temp += columnXIndex.ToString() + Comma; // Column X Index
			temp += columnYIndex.ToString() + Comma; // Column Y Index
			temp += columnZIndex.ToString() + Comma; // Column Z Index
			temp += columnXAlias + Comma; // Column X Alias
			temp += columnYAlias + Comma; // Column Y Alias
			temp += columnZAlias + Comma; // Column Z Alias
			temp += PresetStrings[i][2]; // HillsMaxRadius,HillsIterationNumber,GeneticMaxIteration,GeneticSmoothIteration
			
			sw.WriteLine(temp);
		}
		sw.Close();
	}

	public static List<string[]> GetPresetList(){
		List<string[]> output = new List<string[]>();

		String presetFilename = Application.dataPath + "/Heightmaps/DataPresets.csv";
		FileStream fileStream = File.OpenRead(presetFilename);
		StreamReader dataStream = new StreamReader(fileStream);
		// Append each preset to the array
		while(!dataStream.EndOfStream){
			string[] line_vals = dataStream.ReadLine().Split(',');
			// index 1 is data source
			// index 0 is the preset name
			bool exists = false;
			string[] out_t = {line_vals[1], line_vals[0]};
			for(int i=0; i<output.Count; i++){
				if(output[i][0] == out_t[0] && output[i][1] == out_t[1]){
					exists = true;
					break;
				}
			}
			if(!exists){
				output.Add(out_t);
			}
		}
		// Sort output by data (string at index 0)
		List<string[]> output_refined = new List<string[]>();
		while(output.Count != 0){
			string data_name = output[0][0];
			List<string[]> output_replacement = new List<string[]>();
			for(int i=0; i<output.Count; i++){
				if(output[i][0] == data_name){
					output_refined.Add(output[i]);
				}else{
					output_replacement.Add(output[i]);
				}
			}
			output = output_replacement;
		}
		dataStream.Close();
		fileStream.Close();
		return output_refined;
	}

	public static void DeletePreset(string data_name, string preset_name){
		string temp_filepath = Path.GetTempFileName();
		String presetFilename = Application.dataPath + "/Heightmaps/DataPresets.csv";
		StreamWriter tempWriter = new StreamWriter(temp_filepath);
		FileStream fs = File.OpenRead(presetFilename);
		StreamReader presetsFileReader = new StreamReader(fs);

		while(!presetsFileReader.EndOfStream){
			string line = presetsFileReader.ReadLine();
			string[] line_vals = line.Split(',');
			// index 1 is data name
			// index 0 is preset name
			// if the line does not correspond to the data/preset given
			if(line_vals[1] != data_name ||  line_vals[0] != preset_name){
				tempWriter.WriteLine(line);
			}
		}
		tempWriter.Close();
		presetsFileReader.Close();
		fs.Close();
		bool file_copy_success = false;
		while(!file_copy_success){
			try{
				File.Delete(presetFilename);
				File.Move(temp_filepath, presetFilename);
				file_copy_success = true;
			}catch(Exception e){ }
		}
	}
}