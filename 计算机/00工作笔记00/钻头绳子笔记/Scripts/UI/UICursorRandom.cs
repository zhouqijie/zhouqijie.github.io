using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICursorRandom : MonoBehaviour
{
    public RectTransform cursor;

    public float minMaxAngle;

    private bool running = true;
    private float timer = 0f;
    private float currentAngle; public float CurrentAngle { get { running = false; return this.currentAngle;  } }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            timer += Time.deltaTime;
        }

        currentAngle = Mathf.Sin(timer * 8f) * minMaxAngle;

        cursor.localEulerAngles = new Vector3(cursor.localEulerAngles.x, cursor.localEulerAngles.y, currentAngle );
    }
}
