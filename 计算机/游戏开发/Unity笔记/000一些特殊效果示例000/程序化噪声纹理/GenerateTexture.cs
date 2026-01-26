using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class GenerateTexture : MonoBehaviour {

	public Material material = null;

	[SerializeField]
	private int textureWidth = 512;


	[SerializeField]
	private Color bgcolor = Color.white;
	

	


	private Texture2D Generate(){
		int size = Mathf.Clamp(textureWidth, 0, 1024);
		Texture2D tex = new Texture2D(size, size);
		for(int w = 0; w < size; w++){
			for(int h = 0; h < size; h++){
				Color color = bgcolor;

				color = new Color(Random.value, Random.value, Random.value, Random.value);

				tex.SetPixel(w, h, color);
			}
		}
		tex.Apply();
		return tex;
	}

	void UpdateMaterial(){
		if(material != null){
			Texture2D tex = Generate();
			material.SetTexture("_NormalTex", tex);
		}
	}

	void Start(){
		if(material == null){
			if(gameObject.GetComponent<Renderer>() != null){
				material = gameObject.GetComponent<Renderer>().sharedMaterial;
			}
		}
		UpdateMaterial();
	}

	void OnValidate(){
		UpdateMaterial();
	}
}
