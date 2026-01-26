using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabableBoom : MonoBehaviour, IGrabable
{
    public GameObject mainObj;
    public Transform brokenHolder;
    public Transform vFX;
    public float explodeForce;
    public float killRadius;

    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => null;

    public bool CanGrab => true;
    

    //---status----
    private bool explodeSersorEnabled = false;



    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void OnGrab(Transform grab)
    {
        this.transform.parent = grab;

        this.GetComponent<Rigidbody>().isKinematic = true;

        explodeSersorEnabled = false;
    }

    public void OnDrop(Vector3 dir)
    {
        this.transform.parent = null;

        this.GetComponent<Rigidbody>().isKinematic = false;

        this.GetComponent<Rigidbody>().AddForce(dir.normalized * 30f, ForceMode.VelocityChange);// = false;;

        explodeSersorEnabled = true;
    }

    private void BreakThis()
    {
        //fix this.transfrom
        if(this.GetComponent<Rigidbody>()!= null) this.GetComponent<Rigidbody>().isKinematic = true;
        if (this.GetComponent<Collider>() != null) this.GetComponent<Collider>().enabled = false;

        //main
        mainObj.gameObject.SetActive(false);

        //vfx
        vFX.GetComponent<ParticleSystem>().Play();

        //broken parts
        brokenHolder.gameObject.SetActive(true);
        foreach(Transform t in brokenHolder)
        {
            t.gameObject.SetActive(true);
            t.GetComponent<MeshRenderer>().enabled = true;
            t.GetComponent<Collider>().enabled = true;
            t.GetComponent<Rigidbody>().isKinematic = false;
            t.GetComponent<Rigidbody>().useGravity = true;
            t.GetComponent<Rigidbody>().AddForce(explodeForce * Random.onUnitSphere, ForceMode.VelocityChange);
            t.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);

            Destroy(t.gameObject, 10f);
        }

        //Kill All In Radius
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, killRadius);
        foreach (var col in colliders)
        {
            var doll = col.GetComponentInParent<DamageableDoll>();
            if (doll != null)
            {
                doll.GetDamage(10f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(explodeSersorEnabled && collision.relativeVelocity.sqrMagnitude > Mathf.Pow(10f, 2f))
        {
            BreakThis();
        }
    }


}
