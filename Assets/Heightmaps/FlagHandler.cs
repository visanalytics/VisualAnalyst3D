using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class FlagHandler : MonoBehaviour {

	private List<string> FLAGTYPE;
	private List<string> FLAGIMAGE;
	private List<string> FLAGPATH;

	public List<string> FLAG_TYPE {
		get { return FLAGTYPE;}
	}

	public List<string> FLAG_IMAGE {
		get { return FLAGIMAGE;}
	}

	public List<string> FLAG_PATH {
		get { return FLAGPATH;}
	}

	public FlagHandler()
	{

		this.FLAGTYPE = new List<string>();
		this.FLAGIMAGE = new List<string>();
		this.FLAGPATH = new List<string>();
		ReadCSV();
	}

	private void ReadCSV()
	{
		using(StreamReader sr = new StreamReader(Application.dataPath + "/Heightmaps/FlagData.csv"))
		{
			while(!sr.EndOfStream)
			{
				string line = sr.ReadLine();
				string[] value = line.Split(',');
				
				this.FLAGTYPE.Add(value[0].Replace("'",""));
				this.FLAGIMAGE.Add(value[1].Replace("'",""));
				this.FLAGPATH.Add(value[2].Replace("'",""));
			}
		}
	}
}
