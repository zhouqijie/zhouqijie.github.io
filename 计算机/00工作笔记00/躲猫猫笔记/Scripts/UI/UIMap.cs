using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : MonoBehaviour
{
    private PlayerInput player;
    private Collider safeArea;

    private RectTransform rotatorUI;
    private RectTransform safeAreaUI;

    //-----TMP------
    private Vector3 dir;
    private float viewAngle;




    
    void Start()
    {
        player = FindObjectOfType<PlayerInput>();
        safeArea = FindObjectOfType<GameStarter>().Game.SafeArea;

        rotatorUI = this.transform.Find("Mask").Find("Rotator").GetComponent<RectTransform>();
        safeAreaUI = rotatorUI.Find("ImageSafeArea").GetComponent<RectTransform>();

        //repeat
        InvokeRepeating("RepeatRefreshMap", 0.2f, 0.5f);
    }


    private void Update()
    {
        viewAngle = Vector3.SignedAngle(player.transform.forward, Vector3.forward, -Vector3.up);
        rotatorUI.transform.localEulerAngles = new Vector3(0, 0, viewAngle);
    }



    void RepeatRefreshMap()
    {
        dir = (safeArea.transform.position - player.transform.position);

        safeAreaUI.anchoredPosition = new Vector2(dir.x, dir.z);
        safeAreaUI.sizeDelta = new Vector2(safeArea.transform.localScale.x, safeArea.transform.localScale.z);
    }
}
