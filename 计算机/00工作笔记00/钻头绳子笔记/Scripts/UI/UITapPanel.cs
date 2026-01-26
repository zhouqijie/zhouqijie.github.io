using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;



public class UITapPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private PlayerController player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    
    void Update()
    {
        
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        player.StartRay(Camera.main.ScreenPointToRay(eventData.position));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.EndRay(Camera.main.ScreenPointToRay(eventData.position));
    }

}
