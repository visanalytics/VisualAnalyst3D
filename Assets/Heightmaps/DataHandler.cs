using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using UnityEngine;
public class DataHandler
{
	private List<double[]> Data;
	private double minX,minY,minZ,maxX,maxY,maxZ, maxRadius;
	private int ColumnX,ColumnY,ColumnZ;
	private string ColumnXName,ColumnYName,ColumnZName;
	private double lowerBound,upperBound;
	private Variables Vars;

	/// <summary>
	/// Initializes a new instance of the <see cref="DataHandler"/> class.
	/// </summary>
	/// <param name="filename">Filename.</param>
	/// <param name="ColumnX">Index of column to be used as X coord in file.</param>
	/// <param name="ColumnY">Index of column to be used as Y coord in file.</param>
	/// <param name="ColumnZ">Index of column to be used as Z coord in file.</param>
	/// <param name="minX">Minimum X coord.</param>
	/// <param name="minZ">Minimum Z coord.</param>
	/// <param name="maxX">Max X coord.</param>
	/// <param name="maxZ">Max Z coord.</param>
	public DataHandler (String filename, int ColumnX, int ColumnY, int ColumnZ, double minX, double minZ, double maxX, double maxZ)
	{
		this.ColumnX = ColumnX;
		this.ColumnY = ColumnY;
		this.ColumnZ = ColumnZ;
		this.minX = minX;
		this.minZ = minZ;
		this.maxX = maxX;
		this.maxZ = maxZ;
		this.Data = GenerateData(filename);
		double[] bnds = GetBounds(Data);
		lowerBound = bnds[0];
		upperBound = bnds[1];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DataHandler"/> class
	/// without min and max X and Z values.
	/// </summary>
	/// <param name="filename">Filename.</param>
	/// <param name="ColumnX">Column X.</param>
	/// <param name="ColumnY">Column Y.</param>
	/// <param name="ColumnZ">Column Z.</param>
	public DataHandler (String filename, int ColumnX, int ColumnY, int ColumnZ)
	{
		this.ColumnX = ColumnX;
		this.ColumnY = ColumnY;
		this.ColumnZ = ColumnZ;
		this.minX = 0;
		this.minZ = 0;
		this.maxX = 0;
		this.maxZ = 0;
		this.Data = GenerateData(filename);
		double[] bnds = GetBounds(Data);
		lowerBound = bnds[0];
		upperBound = bnds[1];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DataHandler"/> class.
	/// </summary>
	/// <param name="filename">Filename.</param>
	/// <param name="ColumnX">Index of column to be used as X coord in file.</param>
	/// <param name="ColumnY">Index of column to be used as Y coord in file.</param>
	/// <param name="ColumnZ">Index of column to be used as Z coord in file.</param>
	/// <param name="minX">Minimum X coord.</param>
	/// <param name="minZ">Minimum Z coord.</param>
	/// <param name="maxX">Max X coord.</param>
	/// <param name="maxZ">Max Z coord.</param>
	/// <param name="Vars">Variables.</param>
	public DataHandler (String filename, int ColumnX, int ColumnY, int ColumnZ, double minX, double minZ, double maxX, double maxZ, Variables Vars)
	{
		this.ColumnX = ColumnX;
		this.ColumnY = ColumnY;
		this.ColumnZ = ColumnZ;
		this.minX = minX;
		this.minZ = minZ;
		this.maxX = maxX;
		this.maxZ = maxZ;
		this.Vars = Vars;
		this.Data = GenerateData(filename);
		double[] bnds = GetBounds(Data);
		lowerBound = bnds[0];
		upperBound = bnds[1];
	}

	public List<double[]> GetData(){return Data;}
	public double GetMinX(){return minX;}
	public double GetMinY(){return minY;}
	public double GetMinZ(){return minZ;}
	public double GetMaxX(){return maxX;}
	public double GetMaxY(){return maxY;}
	public double GetMaxZ(){return maxZ;}
	public double GetMaxRadius(){return maxRadius;}
	public string GetColumnXName(){return ColumnXName;}
	public string GetColumnYName(){return ColumnYName;}
	public string GetColumnZName(){return ColumnZName;}

	/// <summary>
	/// Reads the data from the file specified and returns
	/// a list of data points associated with columns X, Y
	/// and Z.
	/// </summary>
	/// <returns>The data in the form [X, Y, Z]</returns>
	/// <param name="filename">Filename.</param>
	protected virtual List<double[]> GenerateData(String filename){
		List<double> x = new List<double>();
		List<double> y = new List<double>();
		List<double> z = new List<double>();
		
		var reader = new StreamReader(File.OpenRead(filename));
		List<double[]> data = new List<double[]>();

		// Find the names of the columns
		string ColumnNamesLine = reader.ReadLine();
		SaveColumnNames(ColumnNamesLine, filename);

		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine();
			var values = line.Split(',');
			double[] point = new double[]{0d,0d,0d};
			double tempX, tempY, tempZ;
			try{
				tempX = double.Parse(values[ColumnX].Replace("\"", ""));
				tempY = double.Parse(values[ColumnY].Replace("\"", ""));
				tempZ = double.Parse(values[ColumnZ].Replace("\"", ""));
			}catch(Exception e){
				// Date value is in ColumnX
				if(values[ColumnX].Replace("\"", "").Contains("/")){
					string[] parts = values[ColumnX].Replace("\"", "").Split('/');
					int month = int.Parse(parts[1]);
					int year = int.Parse(parts[2]);
					int value = month+(year*12);
					tempX = value;
					tempY = double.Parse(values[ColumnY].Replace("\"", ""));
					tempZ = double.Parse(values[ColumnZ].Replace("\"", ""));
				// Date value is in ColumnY
				}else if(values[ColumnY].Replace("\"", "").Contains("/")){
					string[] parts = values[ColumnY].Replace("\"", "").Split('/');
					int month = int.Parse(parts[1]);
					int year = int.Parse(parts[2]);
					int value = month+(year*12);
					tempX = double.Parse(values[ColumnX].Replace("\"", ""));
					tempY = value;
					tempZ = double.Parse(values[ColumnZ].Replace("\"", ""));
				// Date value is in ColumnZ
				}else{
					string[] parts = values[ColumnZ].Replace("\"", "").Split('/');
					int month = int.Parse(parts[1]);
					int year = int.Parse(parts[2]);
					int value = month+(year*12);
					tempX = double.Parse(values[ColumnX].Replace("\"", ""));
					tempY = double.Parse(values[ColumnY].Replace("\"", ""));
					tempZ = value;
				}
			}
			point = new double[]{tempX,tempY,tempZ};
			x.Add(tempX);
			y.Add(tempY);
			z.Add(tempZ);
			data.Add(point);
		}
		foreach( double[] point in data){
			point[0] -= (minX);
			point[2] -= (minZ);
			point[0] /= (maxX - minX);
			point[2] /= (maxZ - minZ);
		}
		minY = (double)y.Min();
		maxY = (double)y.Max();
		maxRadius /= (maxX - minX);
		return data;
	}

	/// <summary>
	/// Save the headers of the columns used for axes names.
	/// </summary>
	/// <param name="ColumnNamesLine">Line from the file denoting the headers.</param>
	/// <param name="filename">Filename.</param>
	private void SaveColumnNames(string ColumnNamesLine, string filename){
		string[] ColumnNames = ColumnNamesLine.Replace("\"","").Split(',');

		Dictionary<string,string> filenameToDataName = new Dictionary<string, string>();
		filenameToDataName.Add("Age", "Age");
		filenameToDataName.Add("Fatalities", "Fatalities");
		filenameToDataName.Add("Local Areas", "Accidents");
		filenameToDataName.Add("Severity", "Severity");
		filenameToDataName.Add("Sex", "Gender Balance");
		filenameToDataName.Add("Totals", "Total");
		string CurrentDataName = "";
		try{
			CurrentDataName = filenameToDataName[Vars.DATA];
		}catch(Exception e){}

		Dictionary<string,string> columnNameToName = new Dictionary<string, string>();
		columnNameToName.Add("acc_nos", "Accidents");
		columnNameToName.Add("acc_vehicles", "Vehicles involved");
		columnNameToName.Add("acc_persons", "People involved");
		columnNameToName.Add("acc_cyclists", "Cyclists");
		columnNameToName.Add("acc_females", "Females");
		columnNameToName.Add("acc_males", "Males");
		columnNameToName.Add("acc_deaths", "Deaths");
		columnNameToName.Add("acc_injs", "Injuries");
		columnNameToName.Add("acc_avgage", "Age");
		columnNameToName.Add("acc_avgsev", "Severity");
		columnNameToName.Add("acc_citydist", "Distance from city");
		columnNameToName.Add("acc_date", "Date");
		columnNameToName.Add("acc_genderbal", "Gender Balance");
		columnNameToName.Add("acc_long", "Longitude");
		columnNameToName.Add("acc_lat", "Latitude");
		columnNameToName.Add("point_value", CurrentDataName);
		ColumnXName = columnNameToName[ColumnNames[ColumnX].Replace("\"","")];
		ColumnYName = columnNameToName[ColumnNames[ColumnY].Replace("\"","")];
		ColumnZName = columnNameToName[ColumnNames[ColumnZ].Replace("\"","")];
	}
	
	/// <summary>
	/// Gets the bounds.
	/// </summary>
	/// <returns> Returns [lower_bound, upper_bound] </returns>
	public double[] GetBounds(){
		return new double[]{lowerBound, upperBound};
	}
	
	/// <summary>
	/// Gets the bounds.
	/// </summary>
	/// <returns>The bounds in the form [upper, lower].</returns> 
	/// <param name="dat">Data.</param>
	protected double[] GetBounds(List<double[]> dat){
		
		List<Vector3> points = new List<Vector3>();
		for(int i=0; i<dat.Count; i++){
			double[] t = dat[i];
			points.Add(new Vector3((float)t[0], (float)t[1], (float)t[2]));
		}
		
		IOrderedEnumerable<Vector3> ioe = points.OrderBy(f => f.y);
		double upper = ioe.Last().y;
		double lower = ioe.First().y;
		return new double[]{lower, upper};
	}
}