using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GuideMask : MonoBehaviour, ICanvasRaycastFilter
{
    [HideInInspector] public RectTransform target = null;

    private GameObject mask;

    [HideInInspector] public bool isShowing = false;







    private void Awake()
    {
        mask = this.transform.parent.Find("Mask").gameObject;
    }

    bool ICanvasRaycastFilter.IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        if (target == null) return true;
        return !RectTransformUtility.RectangleContainsScreenPoint(target, screenPos, eventCamera);
    }

    public void SetText(string txt)
    {
        this.transform.Find("TextGuide").GetComponent<Text>().text = txt;
    }


    public void SetTarget(RectTransform rectt, bool useDefaultImage = false, int radius = 100)//
    {
        this.gameObject.SetActive(true);

        isShowing = true;

        mask.transform.position = rectt.position;

        if(rectt.GetComponent<Image>() != null && !useDefaultImage)
        {
            mask.GetComponent<Image>().sprite = rectt.GetComponent<Image>().sprite;
            mask.GetComponent<RectTransform>().sizeDelta = rectt.sizeDelta;
        }
        else
        {
            mask.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/GuideMask");
            mask.GetComponent<RectTransform>().sizeDelta = new Vector2(radius * 2, radius * 2);
        }

        target = rectt;
    }


    public void Close()
    {
        this.gameObject.SetActive(false);

        isShowing = false;
    }
}
