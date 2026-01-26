using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationAcc : MonoBehaviour
{
    private Vector3 startEulerAnglesL;
    private Vector3 startEulerAnglesR;
    // Start is called before the first frame update
    void Awake()
    {
        startEulerAnglesL = this.transform.Find("LeftPanel").localEulerAngles;
        startEulerAnglesR = this.transform.Find("RightPanel").localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(0, 0, Input.acceleration.x * 15f), 0.05f); //Input.acceleration.x * 15f

        this.transform.Find("LeftPanel").localRotation = Quaternion.Lerp(this.transform.Find("LeftPanel").localRotation, Quaternion.Euler(Input.acceleration.y * -30f, startEulerAnglesL.y, startEulerAnglesL.z), 0.05f);

        this.transform.Find("RightPanel").localRotation = Quaternion.Lerp(this.transform.Find("RightPanel").localRotation, Quaternion.Euler(Input.acceleration.y * -30f, startEulerAnglesR.y, startEulerAnglesR.z), 0.05f);
    }

    
}
