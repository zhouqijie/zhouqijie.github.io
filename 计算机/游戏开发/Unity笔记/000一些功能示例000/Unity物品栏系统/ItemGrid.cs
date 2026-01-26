using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemGrid : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler, IPointerClickHandler, ICanvasRaycastFilter {

	private Inventory inventory;
	public int index;
	private Canvas canvas;
	


	void Start () {
		canvas = GetComponentInParent<Canvas>();
		inventory = canvas.GetComponentInChildren<Inventory>();
	}
	

	public void OnPointerClick(PointerEventData data){
		if(inventory[data.pointerPress.GetComponent<ItemGrid>().index] != null){
			inventory.ChooseCurrent(data.pointerPress.GetComponent<ItemGrid>().index);
		}
	}

	public void OnBeginDrag(PointerEventData data){
	}
	public void OnDrag(PointerEventData data){
		
		if(inventory[data.pointerDrag.GetComponent<ItemGrid>().index] != null){
			Vector3 worldPoint;
			RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.transform.GetChild(0).GetComponent<RectTransform>(), data.position, canvas.worldCamera, out worldPoint);
			data.pointerDrag.transform.GetChild(0).position = worldPoint;
		}
	}
	public void OnDrop(PointerEventData data){
		if(this.GetComponent<ItemGrid>() != null){
			if(inventory[data.pointerDrag.GetComponent<ItemGrid>().index] != null){
				if(inventory[this.GetComponent<ItemGrid>().index] == null){
					IItem item = inventory[data.pointerDrag.GetComponent<ItemGrid>().index];
					inventory[data.pointerDrag.GetComponent<ItemGrid>().index] = null;
					inventory[this.GetComponent<ItemGrid>().index] = item;
				}
				else if(inventory[this.GetComponent<ItemGrid>().index] != null){
					IItem item1 = inventory[data.pointerDrag.GetComponent<ItemGrid>().index];
					IItem item2 = inventory[this.GetComponent<ItemGrid>().index];

					inventory[data.pointerDrag.GetComponent<ItemGrid>().index] = item2;
					inventory[this.GetComponent<ItemGrid>().index] = item1;
				}
				
			}
			
		}
	}
	public void OnEndDrag(PointerEventData data){
		if(data.pointerCurrentRaycast.gameObject == null){
			inventory.Abandon(data.pointerDrag.GetComponent<ItemGrid>().index);
		}
		inventory.DisplayGrids();
	}

	public bool IsRaycastLocationValid(Vector2 vec, Camera cam){
		return true;
	}

	void OnGUI(){
	}
}
