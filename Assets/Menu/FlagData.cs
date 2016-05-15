using UnityEngine;
using System.Collections;

public class FlagData : MonoBehaviour {

	private string Annotation;
	private int ID;
	private Vector3 DataPosition;
	private bool Selected;
	private GameObject FlagCloth;
	private int FlagImageIndex;
	private string FlagColorString;

	private Vector3 WorldPosition;
	Vector3 oscillateFrom, oscillateTo;
	float startOscillate;
	float speed = 1f;

	// Use this for initialization
	void Start () {
		//Assigns individual Shader for each flag
		this.FlagCloth = GameObject.Find ("Cube");
		//Material flagmat = new Material(Shader.Find("Transparent/Cutout/Specular"));
//		Material flagmat = new Material(Shader.Find("Nature/Tree Soft Occlusion Leaves"));
//		FlagCloth.renderer.material = flagmat;

//		this.Annotation = "";
//		this.DataPosition = new Vector3();
//		this.WorldPosition = new Vector3();
//		this.Selected = false;
//
//		//oscillation
//		this.oscillateFrom = WorldPosition;
//		this.oscillateTo = WorldPosition + (Vector3.up*5f);
	}

	public void Init(int ID, Vector3 WorldPosition, Vector3 DataPosition, string Annotation){
		this.ID = ID;
		this.Annotation = Annotation;
		this.DataPosition = DataPosition;
		this.WorldPosition = WorldPosition;
		this.Selected = false;
		
		//oscillation
		this.oscillateFrom = WorldPosition;
		this.oscillateTo = WorldPosition + (Vector3.up*5f);
	}

	// Update is called once per frame
	void Update () {
		if(Selected){
			float t = (Mathf.Sin ((Time.time-startOscillate) * speed * Mathf.PI * 2.0f  - Mathf.PI/2) + 1.0f) / 2.0f;
			transform.position = Vector3.Lerp (oscillateFrom, oscillateTo, t);
		}
	}

	// For flag selection
	public void SetFlagSelected(bool val){
		startOscillate = Time.time;
		this.Selected = val;
		if(!Selected){
			this.transform.position = WorldPosition;
		}
	}
	public bool IsSelected(){return Selected;}

	// Getters/setters
	public void SetImage(Texture val){this.FlagCloth.GetComponent<MeshRenderer>().renderer.material.mainTexture = val;}
	public void SetAnnotation(string val){this.Annotation = val;}
	public string GetAnnotation(){return this.Annotation;}
	public void SetID(int val){this.ID = val;}
	public int GetID(){return ID;}
	public void SetDataPos(Vector3 pos){this.DataPosition = pos;}
	public Vector3 GetDataPos(){return this.DataPosition;}
	public void SetWorldPos(Vector3 pos){
		this.WorldPosition = pos;
		
		//oscillation
		this.oscillateFrom = WorldPosition;
		this.oscillateTo = WorldPosition + (Vector3.up*5f);
	}
	public Vector3 GetWorldPos(){return this.WorldPosition;}
	public void SetFlagImageIndex(int index){this.FlagImageIndex = index;}
	public int GetFlagImageIndex(){return this.FlagImageIndex;}
	public void SetFlagColorString(string val){this.FlagColorString = val;}
	public string GetFlagColorString(){return this.FlagColorString;}
}
