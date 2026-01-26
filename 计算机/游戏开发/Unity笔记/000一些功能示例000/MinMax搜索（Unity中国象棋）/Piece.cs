using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType {
	JIANG,
	SHI,
	XIANG,
	MA,
	JU,
	PAO,
	BING

}
public class Piece : MonoBehaviour {

	[SerializeField]
	private PieceType pieceType;

	[HideInInspector]
	public float value;

	[SerializeField]
	private bool isPlayer;

	public PieceType PieceType { get { return this.pieceType; } }
	public bool IsPlayer { get { return this.isPlayer; } }

	// Use this for initialization
	void Start () {
		Texture2D tex = Resources.Load ("Models/Textures/" + this.pieceType.ToString () + (isPlayer ? "1" : "2")) as Texture2D;
		this.GetComponent<MeshRenderer> ().materials[0].SetTexture ("_MainTex", tex);

		switch(pieceType){
			case PieceType.JIANG : value = 999f; break;
			case PieceType.SHI : value = 1.5f; break;
			case PieceType.XIANG : value = 1.5f; break;
			case PieceType.MA : value = 4f; break;
			case PieceType.JU : value = 10f; break;
			case PieceType.PAO : value = 5f; break;
			case PieceType.BING : value = 1f; break;
			default : break;
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public void MoveTo (Chunk targetChunk) {
		if (targetChunk.transform.childCount > 0) {
			DestroyImmediate (targetChunk.transform.GetChild (0).gameObject);
		}
		this.transform.parent = targetChunk.transform;
		this.transform.localPosition = new Vector3 ();
	}
}