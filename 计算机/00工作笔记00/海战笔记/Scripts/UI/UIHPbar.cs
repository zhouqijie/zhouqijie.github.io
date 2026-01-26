using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHPbar : MonoBehaviour
{
    private  PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float hp01 = player.GetComponent<Hull>().HP/player.GetComponent<Hull>().MaxHP;
        this.transform.GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(hp01, 1f);
    }
}
