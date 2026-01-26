using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableDefault : MonoBehaviour, IGrabable
{
    //custom field
    public string specialTag;


    //impl
    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => this.transform;

    public bool CanGrab => true;

    public void OnDrop(Vector3 dir)
    {
        try
        {
            this.transform.parent = null;

            this.GetComponent<Rigidbody>().isKinematic = false;

            this.GetComponent<Rigidbody>().AddForce(dir.normalized * 30f, ForceMode.VelocityChange);// = false;
        }
        catch
        {

        }
    }

    public void OnGrab(Transform grab)
    {
        this.transform.parent = grab;

        //this.transform.localPosition = Vector3.zero;

        this.GetComponent<Rigidbody>().isKinematic = true;

        //event
        if(specialTag == "Rocket")
        {
            FindObjectOfType<PlayerController>().onGrabRocket.Invoke();
        }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }
}
