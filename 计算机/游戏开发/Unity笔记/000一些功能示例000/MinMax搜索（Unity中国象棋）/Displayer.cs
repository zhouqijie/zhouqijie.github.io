using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displayer : MonoBehaviour {

	private UnityEngine.UI.Text displayer;
	// Use this for initialization
	void Awake () {
		
	}
	public static void Display(string newText){
		var displayer = GameObject.Find("UI_Text").GetComponent<UnityEngine.UI.Text>();
		displayer.text = newText;
	}
}
