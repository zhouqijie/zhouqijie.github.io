using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_Default : MonoBehaviour, IItem {

    [SerializeField] public ItemType type;
    [SerializeField] public string itemName;
    [SerializeField] public GameObject display;
    [SerializeField] public int count;


    public GameObject GameObj { get { return this.gameObject; } }
    public ItemType Type { get { return type; } }
    public string Name { get { return itemName; } }
    public int Count { get { return count; } set { count = value; } }
    public GameObject ObjDisplay { get { return display; } }

    public void SetFree(){
		if(this.GetComponent<Rigidbody>() != null){
			this.GetComponent<Rigidbody>().isKinematic = false;
		}
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = true;
		}
		foreach(Transform child in transform){
			if(child.GetComponent<Collider>() != null){
				child.GetComponent<Collider>().enabled = true;
			}
		}
	}
	public void SetHeld(){
		if(this.GetComponent<Rigidbody>() != null){
			this.GetComponent<Rigidbody>().isKinematic = true;
		}
		if(this.GetComponent<Collider>() != null){
			this.GetComponent<Collider>().enabled = false;
		}
		foreach(Transform child in transform){
			if(child.GetComponent<Collider>() != null){
				child.GetComponent<Collider>().enabled = false;
			}
		}
	}

    public void Action0()
    {
    }
    public void Action1()
    {
    }
}
