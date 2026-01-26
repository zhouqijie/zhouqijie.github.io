using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxDrill : MonoBehaviour
{
    [HideInInspector] public  Transform bloodFx;
    [HideInInspector] public Transform metalFx;



    void Start()
    {
        bloodFx = this.transform.GetChild(0);
        metalFx = this.transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
