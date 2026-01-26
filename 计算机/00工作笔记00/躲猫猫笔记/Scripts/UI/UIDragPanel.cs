using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDragPanel : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public static bool enableDrag = true;

    private UICanvas canvas;

    private Vector2 startPoint;
    private Vector2 currentPoint;

    [HideInInspector] public UnityEngine.Events.UnityEvent onBeginDrag;

    void Start()
    {
        canvas = this.GetComponentInParent<UICanvas>();
    }
    //-----------------------------------------------------View Drag-----------------------------------------------------
    public void OnDrag(PointerEventData eventData)
    {
        if (!UIDragPanel.enableDrag)
        {
            canvas.OutPut = new Vector2();
            return;
        }
        canvas.OutPut = eventData.position - currentPoint;
        currentPoint = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag.Invoke();

        startPoint = eventData.position;
        currentPoint = startPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!UIDragPanel.enableDrag)
        {
            canvas.OutPut = new Vector2();
            return;
        }
        canvas.OutPut = new Vector2();
    }
}
