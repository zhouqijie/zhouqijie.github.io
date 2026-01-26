using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMovePanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private UICanvas uiCanvas;

    private float radius;

    private GameObject indicator;

    private Vector2 center;

    private Vector2 currentPos;

    public Vector2 offset;

    private Vector2 factor;

    void Awake()
    {
        center = this.GetComponent<RectTransform>().anchoredPosition;
        uiCanvas = FindObjectOfType<UICanvas>();
        indicator = this.transform.Find("Indicator").gameObject;
        radius = this.GetComponent<RectTransform>().sizeDelta.x / 2f;
    }
    // Start is called before the first frame update
    void Start()
    {
        factor = uiCanvas.GetComponent<RectTransform>().sizeDelta / new Vector2(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        
    }





    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }


    public void OnDrag(PointerEventData eventData)
    {
        currentPos = eventData.position * factor;
        offset = Vector2.ClampMagnitude(currentPos - center, radius);
        indicator.GetComponent<RectTransform>().anchoredPosition = offset;
        uiCanvas.PlayerSetMoveVec(offset / radius);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        currentPos = center;
        offset = Vector2.ClampMagnitude(currentPos - center, radius);
        indicator.GetComponent<RectTransform>().anchoredPosition = offset;
        uiCanvas.PlayerSetMoveVec(offset / radius);
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }
}
