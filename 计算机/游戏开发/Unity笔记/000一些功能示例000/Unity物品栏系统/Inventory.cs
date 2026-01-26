using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour, IEnumerable
{
	[SerializeField]public GameObject obj_charactor;
	[SerializeField]public GameObject obj_bag;
	[SerializeField]public MyCharactorController character;
	private int length;
    private IItem[] items;
	[SerializeField]private ItemGrid[] grids; 
	[SerializeField]private UnityEngine.UI.Text[] texts;

	public int Length{get{return length;}}
    public IItem this[int index] { get { return items[index]; } set { items[index] = value; } }
	public IEnumerator GetEnumerator(){
		for(int i = 0; i < length; i++){
			yield return items[i];
		}
	}
	
	public void Add(IItem item){
		if(item.Type == ItemType.Consume){
			bool hasSame = false;
			for (int i = 0; i < length; i++)
            {
                if (items[i] != null && items[i].Name == item.Name)
                {
                    items[i].Count += item.Count;
                    Destroy(item.GameObj);
					hasSame = true;
                    break;
                }
            }
            if (!hasSame)
            {
                for (int i = 0; i < length; i++)
                {
                    if (items[i] == null)
                    {
                        item.SetHeld();
                        items[i] = item;
                        break;
                    }
                }
				item.GameObj.transform.SetParent(obj_bag.transform);
				item.GameObj.transform.localPosition = Vector3.zero;
				item.GameObj.transform.localRotation = new Quaternion();
            }
		}
		else if(item.Type == ItemType.Default){
			for (int i = 0; i < length; i++)
            {
                if (items[i] == null)
                {
                    item.SetHeld();
                	items[i] = item;
                    break;
                }
            }
			item.GameObj.transform.SetParent(obj_bag.transform);
			item.GameObj.transform.localPosition = Vector3.zero;
			item.GameObj.transform.localRotation = new Quaternion();
		}
		
		DisplayGrids();
	}
	public void ChooseCurrent(int index){
		if(character.currentItem != null){
			StoreCurrent();
		}
		character.currentItem = this[index].GameObj;
		this[index].GameObj.transform.SetParent(obj_charactor.GetComponent<MyCharactorController>().Hand.transform);
		this[index].GameObj.transform.position = obj_charactor.GetComponent<MyCharactorController>().Hand.transform.position;
		this[index].GameObj.transform.rotation = obj_charactor.GetComponent<MyCharactorController>().Hand.transform.rotation;
	}
	public void StoreCurrent(){
		if(character.currentItem != null){
			if(character.currentItem.GetComponent<IItem>().Type == ItemType.Large){
				character.AbandonCurrent();
			}
			else{
				character.currentItem.transform.SetParent(obj_bag.transform);
				character.currentItem.transform.localPosition = Vector3.zero;
				character.currentItem.transform.localRotation = new Quaternion();
				character.currentItem = null;
			}
		}
	}
	public void Abandon(int index){
		GameObject obj_abandon;
		obj_abandon = this[index].GameObj;
		this[index] = null;
		obj_abandon.transform.parent = null;
		obj_abandon.transform.position = Camera.main.transform.position + Camera.main.transform.forward;
		obj_abandon.GetComponent<IItem>().SetFree();
		obj_abandon.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 250f * obj_abandon.GetComponent<Rigidbody>().mass);
	}
	public void CheckCount(){
		for(int i = 0; i < length; i ++)
        {
            if (this[i] != null)
            {
                if (this[i].Count < 1)
                {
                    Destroy(this[i].GameObj);
                    this[i] = null;
                }
            }
		}
		DisplayGrids();
	}
	public void DisplayGrids(){
		for(int i = 0; i < length; i ++){
			if(grids[i].transform.childCount > 0){
				Destroy(grids[i].transform.GetChild(0).gameObject);
			}
			if(this[i] != null){
				if(this[i].Count > 0){
					GameObject obj_display = Instantiate(this[i].ObjDisplay);
					obj_display.transform.SetParent(grids[i].transform);
					obj_display.transform.SetParent(grids[i].transform);
					obj_display.transform.localPosition = Vector3.zero;
					obj_display.transform.localRotation = new Quaternion();
                    if (this[i].Count > 1)
                    {
                        texts[i].GetComponentInChildren<UnityEngine.UI.Text>().text = this[i].Name + ":" + this[i].Count;
                    }
                    else
                    {
                        texts[i].text = this[i].Name;
                    }
				}
				else{
					Destroy(this[i].GameObj);
					this[i] = null;
				}
				
			}
			else{
				texts[i].GetComponentInChildren<UnityEngine.UI.Text>().text = "Empty";
			}
		}
	}

	public void Hide(){
		this.GetComponent<RectTransform>().anchoredPosition = new Vector2(210, 0);
	}
	public void Show(){
		this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
	}
	void Start()
    {
		length = 6;
		items = new IItem[6];
		DisplayGrids();
    }
	





	void OnGUI(){
		GUILayout.Box("Grid1 : " + (grids[0].transform.childCount > 0 ? grids[0].transform.GetChild(0).gameObject.name : "无") + 
		"\nGrid2 : " + (grids[1].transform.childCount > 0 ? grids[1].transform.GetChild(0).gameObject.name : "无") + 
		"\nGrid3 : " + (grids[2].transform.childCount > 0 ? grids[2].transform.GetChild(0).gameObject.name : "无") + 
		"\nGrid4 : " + (grids[3].transform.childCount > 0 ? grids[3].transform.GetChild(0).gameObject.name : "无") +
		"\nGrid5 : " + (grids[4].transform.childCount > 0 ? grids[4].transform.GetChild(0).gameObject.name : "无") + 
		"\nGrid6 : " + (grids[5].transform.childCount > 0 ? grids[5].transform.GetChild(0).gameObject.name : "无"));

		GUILayout.Box("inevntory[0] : " + (this[0] != null ? this[0].GameObj.name : "无") + 
		"\ninevntory[1] : " + (this[1] != null ? this[1].GameObj.name : "无") + 
		"\ninevntory[2] : " + (this[2] != null ? this[2].GameObj.name : "无") + 
		"\ninevntory[3] : " + (this[3] != null ? this[3].GameObj.name : "无") +
		"\ninevntory[4] : " + (this[4] != null ? this[4].GameObj.name : "无") + 
		"\ninevntory[5] : " + (this[5] != null ? this[5].GameObj.name : "无"));
	}
}
