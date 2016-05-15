using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
public class VariablesPresets
{
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

//	public static DataTable GetTable(){
//		DataTable table = new DataTable();
//		table.Columns.Add("Data", typeof(string));
//		table.Columns.Add("Source", typeof(string));
//		table.Columns.Add("TerrainType", typeof(string));
//		table.Columns.Add("Preset", typeof(string));
//		table.Columns.Add("HeightmapSize", typeof(float));
//		table.Columns.Add("ColormapSize", typeof(float)); 
//		table.Columns.Add("ScaleY", typeof(float));
//		table.Columns.Add("Filename", typeof(string));
//		table.Columns.Add("MapFilename", typeof(string));
//		table.Columns.Add("MinX", typeof(float));
//		table.Columns.Add("MinZ", typeof(float));
//		table.Columns.Add("MaxX", typeof(float));
//		table.Columns.Add("MaxZ", typeof(float));
//		table.Columns.Add("ColorPreset", typeof(string));
//		table.Columns.Add("ColumnX", typeof(int));
//		table.Columns.Add("ColumnY", typeof(int));
//		table.Columns.Add("ColumnZ", typeof(int));
//		table.Columns.Add("HillsMaxRadius", typeof(float));
//		table.Columns.Add("HillsIterationNumber", typeof(float));
//		table.Columns.Add("GeneticMaxIteration", typeof(int));
//		table.Columns.Add("GeneticSmoothIteration", typeof(int));
//
//		// Age
//		table.Rows.Add("Age", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_age_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.05f, 100f, 0, 0);
//		table.Rows.Add("Age", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_age_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.5f, 100f, 0, 0);
//		table.Rows.Add("Age", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_age_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Age", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_age_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);     
//		table.Rows.Add("Age", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_age_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Age", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_age_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.005f, 100f, 0, 0);
//		table.Rows.Add("Age", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_age_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Age", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_age_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Age", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_age_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Age", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_age_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Age", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_age_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Age", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_age_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Age", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_age_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Age", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_age_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Age", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_age_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Age", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_age_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//
//		// Sum
//		table.Rows.Add("Totals", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_VicAll-128x128.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.03f, 100f, 0, 0);
//		table.Rows.Add("Totals", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sum_grid_VicAll-128x128.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.25f, 100f, 0, 0);
//		table.Rows.Add("Totals", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sum_grid_VicAll-128x128.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Totals", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_VicAll-128x128.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);     
//		table.Rows.Add("Totals", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Totals", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sum_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.005f, 100f, 0, 0);
//		table.Rows.Add("Totals", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sum_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Totals", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Totals", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Totals", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sum_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Totals", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sum_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Totals", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Totals", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Totals", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sum_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Totals", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sum_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Totals", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sum_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//
//		// Fatalities
//		table.Rows.Add("Fatalities", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_VicAll-256x256.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.03f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_fat_grid_VicAll-256x256.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.25f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_fat_grid_VicAll-256x256.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Fatalities", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_VicAll-256x256.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);     
//		table.Rows.Add("Fatalities", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_fat_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.005f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_fat_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Fatalities", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Fatalities", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_fat_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_fat_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Fatalities", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Fatalities", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_fat_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Fatalities", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_fat_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Fatalities", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_fat_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//
//		// Local Gov. Areas 
//		table.Rows.Add("Local Areas", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.03f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.25f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Local Areas", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);     
//		table.Rows.Add("Local Areas", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.005f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Local Areas", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Local Areas", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Local Areas", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Local Areas", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Local Areas", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Local Areas", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_lgas_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//
//		// Severity 
//		table.Rows.Add("Severity", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.03f, 100f, 0, 0);
//		table.Rows.Add("Severity", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sev_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.25f, 100f, 0, 0);
//		table.Rows.Add("Severity", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sev_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Severity", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);     
//		table.Rows.Add("Severity", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Severity", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sev_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.005f, 100f, 0, 0);
//		table.Rows.Add("Severity", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sev_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Severity", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Severity", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Severity", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sev_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Severity", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sev_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Severity", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Severity", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Severity", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sev_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Severity", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sev_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Severity", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sev_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		
//		// Sex
//		table.Rows.Add("Sex", "Victoria", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.03f, 100f, 0, 0);
//		table.Rows.Add("Sex", "Victoria", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sex_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.25f, 100f, 0, 0);
//		table.Rows.Add("Sex", "Victoria", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sex_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Sex", "Victoria", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_VicAll-512x512.csv", "/Heightmaps/Maps/VicAll.png", -34f, 141f, -39f, 150f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);      
//		table.Rows.Add("Sex", "City", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Sex", "City", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sex_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Sex", "City", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sex_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Sex", "City", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_CitySurr-128x128.csv", "/Heightmaps/Maps/CitySurr.png", -37.78f, 144.88f, -37.86f, 145.03f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Sex", "Larger Melb.", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.002f, 100f, 0, 0);
//		table.Rows.Add("Sex", "Larger Melb.", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sex_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.01f, 100f, 0, 0);
//		table.Rows.Add("Sex", "Larger Melb.", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sex_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f,  ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Sex", "Larger Melb.", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_LargerMelb-128x128.csv", "/Heightmaps/Maps/LargerMelb.png", -37.65f, 144.7f, -38f, 145.3f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//		table.Rows.Add("Sex", "CBD", "Granular", "Peaks", 512f, 512f, 5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.0002f, 100f, 0, 0);
//		table.Rows.Add("Sex", "CBD", "Granular", "Landscape", 512f, 512f, 1f, "/Heightmaps/Vic_Data/r_acc_sex_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0.001f, 100f, 0, 0);
//		table.Rows.Add("Sex", "CBD", "Smooth", "Peaks", 512f, 512f, 3f, "/Heightmaps/Vic_Data/r_acc_sex_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW_NEEDLES, 3, 1, 2, 0f, 0f, 200, 0);
//		table.Rows.Add("Sex", "CBD", "Smooth", "Landscape", 512f, 512f, 2.5f, "/Heightmaps/Vic_Data/r_acc_sex_grid_MelbCity-64x64.csv", "/Heightmaps/Maps/MelbCity.png", -37.805f, 144.950f, -37.825f, 144.986f, ColorSpectrumObj.PRESET_BLUE_TO_YELLOW, 3, 1, 2, 0f, 0f, 200, 7);    
//
//		return table;
//	}

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
			               int.Parse(vals[16]), float.Parse(vals[17]), float.Parse(vals[18]), int.Parse(vals[19]), int.Parse(vals[20]));
		}
		return table;
	}
}