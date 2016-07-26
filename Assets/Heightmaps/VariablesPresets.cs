using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class VariablesPresets
{
	/// <summary>
	/// Instantiates a new instance of <see cref="VariablePreset"/>
	/// using the source, terrain type, filename and terrain preset
	/// to find its preset values in the data table.
	/// </summary>
	/// <returns>The preset.</returns>
	/// <param name="source">Source.</param>
	/// <param name="terrainType">Terrain type.</param>
	/// <param name="filename">Filename.</param>
	/// <param name="terrainPreset">Terrain preset.</param>
	public static Variables VariablePreset(String source,
	                                       String terrainType,
	                                       String filename,
	                                       String terrainPreset)
	{


		Variables Vars = new Variables();
		DataRow[] result = GetTableFromCSV().Select("Source = '"+source+"' AND Data = '"+filename+"' AND TerrainType = '"+terrainType+"' AND Preset = '"+terrainPreset+"'");
		if(result != null){
			// Shared Variables
			Vars.TERRAIN_NAME = source + terrainType + filename + terrainPreset;
			Vars.SOURCE = source;
			Vars.TERRAIN_TYPE = terrainType;
			Vars.DATA = filename;
			Vars.PRESET = terrainPreset;
			Vars.HEIGHTMAP_SIZE = (float)result[0]["HeightmapSize"];
			Vars.COLORMAP_SIZE = (float)result[0]["HeightmapSize"];
			Vars.SCALE_Y = (float)result[0]["ScaleY"];
			Vars.FILENAME = (string)result[0]["Filename"];
			Vars.MAP_FILENAME = (string)result[0]["MapFilename"];
			Vars.COLOR_PRESET = (string)result[0]["ColorPreset"];
			Vars.COLUMN_X = (int)result[0]["ColumnX"];
			Vars.COLUMN_Y = (int)result[0]["ColumnY"];
			Vars.COLUMN_Z = (int)result[0]["ColumnZ"];
			Vars.COLUMN_X_ALIAS = (string)result[0]["ColumnXAlias"];
			Vars.COLUMN_Y_ALIAS = (string)result[0]["ColumnYAlias"];
			Vars.COLUMN_Z_ALIAS = (string)result[0]["ColumnZAlias"];
			Vars.MIN_X = (float)result[0]["MinX"];
			Vars.MIN_Z = (float)result[0]["MinZ"];
			Vars.MAX_X = (float)result[0]["MaxX"];
			Vars.MAX_Z = (float)result[0]["MaxZ"];
			
			// Hills Variables ONLY
			Vars.MAX_RADIUS = (float)result[0]["HillsMaxRadius"];
			Vars.ITERATION_NUMBER = (float)result[0]["HillsIterationNumber"];
			
			//Genetic Algorithm Variables ONLY 
			Vars.GENETIC_MAX_ITERATION = (int)result[0]["GeneticMaxIteration"];
			Vars.GENETIC_SMOOTH_ITERATION = (int)result[0]["GeneticSmoothIteration"];
			
			return Vars;
		}else{
			return Vars; 
		}
	}

	/// <summary>
	/// Generates the data table from the CSV file of 
	/// data presets.
	/// </summary>
	/// <returns>The populated data table.</returns>
	public static DataTable GetTableFromCSV(){
		String filename = Application.dataPath + "/Heightmaps/DataPresets.csv";
		DataTable table = new DataTable();
		table.Columns.Add("Data", typeof(string));
		table.Columns.Add("Source", typeof(string));
		table.Columns.Add("TerrainType", typeof(string));
		table.Columns.Add("Preset", typeof(string));
		table.Columns.Add("HeightmapSize", typeof(float));
		table.Columns.Add("ColormapSize", typeof(float)); 
		table.Columns.Add("ScaleY", typeof(float));
		table.Columns.Add("Filename", typeof(string));
		table.Columns.Add("MapFilename", typeof(string));
		table.Columns.Add("MinX", typeof(float));
		table.Columns.Add("MinZ", typeof(float));
		table.Columns.Add("MaxX", typeof(float));
		table.Columns.Add("MaxZ", typeof(float));
		table.Columns.Add("ColorPreset", typeof(string));
		table.Columns.Add("ColumnX", typeof(int));
		table.Columns.Add("ColumnY", typeof(int));
		table.Columns.Add("ColumnZ", typeof(int));
		table.Columns.Add("ColumnXAlias", typeof(string));
		table.Columns.Add("ColumnYAlias", typeof(string));
		table.Columns.Add("ColumnZAlias", typeof(string));
		table.Columns.Add("HillsMaxRadius", typeof(float));
		table.Columns.Add("HillsIterationNumber", typeof(float));
		table.Columns.Add("GeneticMaxIteration", typeof(int));
		table.Columns.Add("GeneticSmoothIteration", typeof(int));
		
		var reader = new StreamReader(File.OpenRead(filename));
		reader.ReadLine();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			var vals = line.Split(',');
			
			table.Rows.Add((string)vals[0], (string)vals[1], (string)vals[2], (string)vals[3], float.Parse(vals[4]), float.Parse(vals[5]),
			               float.Parse(vals[6]), (string)vals[7], (string)vals[8], float.Parse(vals[9]), float.Parse(vals[10]),
			               float.Parse(vals[11]), float.Parse(vals[12]), (string)vals[13], int.Parse(vals[14]), int.Parse(vals[15]),
			               int.Parse(vals[16]), (string)vals[17], (string)vals[18], (string)vals[19], float.Parse(vals[20]),
			               float.Parse(vals[21]), int.Parse(vals[22]), int.Parse(vals[23]));
		}
		return table;
	}
}