using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    private IShipController controller;

    public GameObject[] fans;

    [HideInInspector] public float thrustPower;

    private Rigidbody rigid;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
        controller = GetComponentInParent<IShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var item in fans)
        {
            item.transform.Rotate(new Vector3(0f, 0f, Time.deltaTime * controller.Throttle * 2345f), Space.Self);
        }
    }



    private void FixedUpdate()
    {
        if (rigid.transform.position.y < 5f)
        {
            rigid.AddForce(this.transform.forward * thrustPower * rigid.mass * controller.Throttle, ForceMode.Force);
        }
    }
}
