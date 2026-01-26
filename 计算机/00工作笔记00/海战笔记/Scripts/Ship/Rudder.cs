using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour
{
    public float turnForce;

    public GameObject[] rudders;

    private Rigidbody rigid;

    private float currentAngle;

    private float targetAngle;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        targetAngle = -GetComponent<IShipController>().Rudder * 30f;

        currentAngle += (targetAngle - currentAngle) * Time.deltaTime;

        foreach(var item in rudders)
        {
            item.transform.localEulerAngles = new Vector3(0f, currentAngle, 0f);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.transform.position.y < 5f)
        {
            rigid.AddForceAtPosition(rigid.transform.right * Vector3.Dot(rigid.velocity, rigid.transform.forward.normalized) * (currentAngle / 45f) * 0.1f * rigid.mass * turnForce, rigid.transform.position + rigid.transform.forward * (rudders[0].transform.localPosition.z) + rigid.transform.up * (-1f));
        }
    }
}
