using UnityEngine;
using System.Collections;
using System;

public class GridData : MonoBehaviour {
	
	private string Name;
	private Vector3 DataPosition;
	private Vector3[] GridPositions;
	private int GridPositionIndex;
	private Vector3[] GridDataPositions;
	private int GridDataPositionIndex;
	private int ID;
	public enum Orientation {UP_Y=0, UP_X=1, UP_Z=2};
	private GridData.Orientation orientation;
	private bool Selected;
	
	private Vector3 WorldPosition;
	Vector3 oscillateFrom, oscillateTo;

	float[][] dataBounds;
	float[][] worldBounds;

	void Start () {}

	void Update () {}

	public void Init(int ID, string Name, float[][] dataBounds,
	                 float[][] worldBounds, bool selected,
	                 GridData.Orientation orientation){
		Init(ID, Name, dataBounds, worldBounds, selected, orientation, false);
	}

	public void Init(int ID, string Name, float[][] dataBounds,
	                 float[][] worldBounds, bool selected,
	                 GridData.Orientation orientation, bool maintainPosition){
		this.Name = Name;
		this.Selected = selected;
		this.orientation = orientation;
		this.dataBounds = dataBounds;
		this.worldBounds = worldBounds;
		this.ID = ID;

		//Initialise positions arrays
		GridPositions = new Vector3[101];
		GridDataPositions = new Vector3[101];
		int dI = 0; // the index for the dimension (X,Y,Z) for the orientation
		switch(orientation){
		case Orientation.UP_X:
			dI = 0;
			break;
			
		case Orientation.UP_Y:
			dI = 1;
			break;
			
		case Orientation.UP_Z:
			dI = 2;
			break;
		}
		double lower = dataBounds[dI][0];
		double upper = dataBounds[dI][1];
		float min = worldBounds[dI][0];
		float max = worldBounds[dI][1];
		float incWorldFloat = (max - min)/GridPositions.Length;
		float ratio = (float)(upper - lower)/(max - min);
		
		Vector3 incBaseData = new Vector3();
		Vector3 incBaseWorld = new Vector3();
		Vector3 incWorld = new Vector3();

		float dataX = (dataBounds[0][1] + dataBounds[0][0])/2;
		float dataY = (dataBounds[1][1] + dataBounds[1][0])/2;
		float dataZ = (dataBounds[2][1] + dataBounds[2][0])/2;
		if(!maintainPosition)
			this.DataPosition = new Vector3(dataX, dataY, dataZ);
		float worldX = (worldBounds[0][1] + worldBounds[0][0])/2;
		float worldY = (worldBounds[1][1] + worldBounds[1][0])/2;
		float worldZ = (worldBounds[2][1] + worldBounds[2][0])/2;
		
		Bounds bounds = GetComponent<MeshFilter>().mesh.bounds;
		switch(orientation){
		case Orientation.UP_X:
			incBaseData = new Vector3((float)lower, 0, 0);
			incBaseWorld = new Vector3(min, worldY, worldZ);
			incWorld = new Vector3(incWorldFloat, 0, 0);
			if(!maintainPosition)
				this.WorldPosition = new Vector3(worldX, worldY, worldZ);
			else{
				this.WorldPosition = new Vector3((float)(DataPosition.x-lower)/ ratio, WorldPosition.y, WorldPosition.z);
			}
			this.transform.position = this.WorldPosition;

			transform.rotation = Quaternion.identity;
			this.transform.localScale = new Vector3((worldBounds[1][1] - worldBounds[1][0])/(bounds.size.x), 1f, (worldBounds[2][1] - worldBounds[2][0])/(bounds.size.z));
			this.renderer.material.mainTextureScale = new Vector2((worldBounds[1][1] - worldBounds[1][0])/10f, (worldBounds[1][1] - worldBounds[1][0])/(10f*(worldY/worldZ)));
			transform.rotation = Quaternion.Euler(0,0,90);
			break;
			
		case Orientation.UP_Y:
			incBaseData = new Vector3(0, (float)lower, 0);
			incBaseWorld = new Vector3(worldX, min, worldZ);
			incWorld = new Vector3(0, incWorldFloat, 0);
			if(!maintainPosition)
				this.WorldPosition = new Vector3(worldX, worldY, worldZ);
			else{
				this.WorldPosition = new Vector3(WorldPosition.x, (float)(DataPosition.y-lower)/ ratio, WorldPosition.z);
			}
			this.transform.position = this.WorldPosition;
			
			transform.localScale = new Vector3((worldBounds[0][1] - worldBounds[0][0])/(bounds.size.x), 1f, (worldBounds[2][1] - worldBounds[2][0])/(bounds.size.z));
			renderer.material.mainTextureScale = new Vector2((worldBounds[0][1] - worldBounds[0][0])/10f, (worldBounds[2][1] - worldBounds[2][0])/10f);
			break;
			
		case Orientation.UP_Z:
			incBaseData = new Vector3(0, 0, (float)lower);
			incBaseWorld = new Vector3(worldX, worldY, min);
			incWorld = new Vector3(0, 0, incWorldFloat);
			if(!maintainPosition)
				this.WorldPosition = new Vector3(worldX, worldY, worldZ);
			else{
				this.WorldPosition = new Vector3(WorldPosition.x, WorldPosition.y, (float)(DataPosition.z-lower)/ ratio);
			}
			this.transform.position = this.WorldPosition;
			transform.rotation = Quaternion.identity;
			this.transform.localScale = new Vector3((worldBounds[0][1] - worldBounds[0][0])/(bounds.size.x), 1f, (worldBounds[1][1] - worldBounds[1][0])/(bounds.size.z));
			this.renderer.material.mainTextureScale = new Vector2((worldBounds[1][1] - worldBounds[1][0])/10f, (worldBounds[1][1] - worldBounds[1][0])/(10f*(worldX/worldY)));
			transform.rotation = Quaternion.Euler(90,0,0);
			break;
		}
		for(int m=0; m<GridPositions.Length; m++){
			GridPositions[m] = incBaseWorld + incWorld*(float)m;
			GridDataPositions[m] = incBaseData + ratio*GridPositions[m];
		}
		if(!maintainPosition){
			SetWorldPosIndex(50);
			SetDataPosIndex(50);
		}
	}

	public void SetWorldPosIndex(int index){
		SetWorldPos(GridPositions[index]);
		GridPositionIndex = index;
	}
	public int GetWorldPosIndex(){
		return GridPositionIndex;
	}
	public void SetDataPosIndex(int index){
		SetDataPos(GridDataPositions[index]);
		GridDataPositionIndex = index;
	}
	public int GetDataPosIndex(){
		return GridDataPositionIndex;
	}
	
	// For flag selection
	public void SetGridSelected(bool val){
		this.Selected = val;
		if(!Selected){
			this.transform.position = WorldPosition;
		}
	}
	public bool IsSelected(){return Selected;}
	
	// Getters/setters
	public void SetName(string val){this.Name = val;}
	public string GetName(){return this.Name;}
	public void SetID(int val){this.ID = val;}
	public int GetID(){return ID;}
	public void SetDataPos(Vector3 pos){this.DataPosition = pos;}
	public Vector3 GetDataPos(){return this.DataPosition;}
	public void SetDataValue(float height){
		this.DataPosition = new Vector3(DataPosition.x, height, DataPosition.z);
	}
	public float GetDataValue(){
		switch(orientation){
		case GridData.Orientation.UP_X:
			return DataPosition.x;
		case GridData.Orientation.UP_Y:
			return DataPosition.y;
		case GridData.Orientation.UP_Z:
			return DataPosition.z;
		default:
			return DataPosition.y;
		}
	}
	public void SetWorldPos(Vector3 pos){
		this.WorldPosition = pos;
		this.transform.position = this.WorldPosition;
	}
	public void SetWorldValue(float height){
		this.WorldPosition = new Vector3(WorldPosition.x, height, WorldPosition.z);
		this.transform.position = WorldPosition;
	}
	public float GetWorldValue(){
		switch(orientation){
		case GridData.Orientation.UP_X:
			return this.WorldPosition.x;
		case GridData.Orientation.UP_Y:
			return this.WorldPosition.y;
		case GridData.Orientation.UP_Z:
			return this.WorldPosition.z;
		default:
			return this.WorldPosition.y;
		}
	}
	public Vector3 GetWorldPos(){return this.WorldPosition;}

	public void SetOrientation(GridData.Orientation val){
		Init(ID, Name, dataBounds, worldBounds, Selected, val);
	}
	public GridData.Orientation GetOrientation(){
		return this.orientation;
	}
}