using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Pos{
	public int x;
	public int y;
}
public class Chunk : MonoBehaviour {

	[SerializeField]
	private Pos position;

	public Pos Position {get {return this.position;}}


	// Use this for initialization
	void Awake () {
		position.x = Mathf.RoundToInt(this.transform.localPosition.z);
		position.y = Mathf.RoundToInt(this.transform.localPosition.x);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
