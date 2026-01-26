using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : MonoBehaviour
{
    private float speed = 2f; // rps
    public Transform[] rotators;
    public Transform firePos;

    void Start()
    {
        
    }

    public void SpeedUp()
    {
        speed = 12f;
    }

    public void SpeedDown()
    {
        speed = 2f;
    }

    void Update()
    {
        for(int i = 0; i < rotators.Length; i++)
        {
            if(i % 2 == 0)
            {
                rotators[i].Rotate(new Vector3(0, Time.deltaTime * 360f * speed, 0), Space.Self);
            }
            else
            {
                rotators[i].Rotate(new Vector3(0, -Time.deltaTime * 180f * speed, 0), Space.Self);
            }
        }


    }
}
