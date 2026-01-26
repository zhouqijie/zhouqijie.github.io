using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody rigid;


    private bool enabledSensor = false;
    private bool exploded = false;

    private void Awake()
    {
        rigid = this.GetComponent<Rigidbody>();
    }
    


    void Start()
    {
        Invoke("EnableSensor", 0.2f);
    }


    private void EnableSensor()
    {
        this.GetComponent<Collider>().enabled = true;
        enabledSensor = true;
    }

    private void FixedUpdate()
    {
    }

    public void ForceExplode()
    {
        if (exploded) return;
        exploded = true;

        //Mesh renderer Disable
        this.GetComponentInChildren<MeshRenderer>().enabled = false;

        //Explode
        StartCoroutine(CoExplode());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (exploded) return;
        if (! enabledSensor) return;
        exploded = true;

        if (collision.collider.CompareTag("PlayerCharacter"))
        {
            var playerCtrl = collision.collider.GetComponentInParent<PlayerController>();
            if (playerCtrl != null)
            {
                playerCtrl.Damage(100f);
            }
        }

        //Mesh renderer Disable
        this.GetComponentInChildren<MeshRenderer>().enabled = false;

        //Explode
        StartCoroutine(CoExplode());
    }

    private IEnumerator CoExplode()
    {
        //Overlap Colliders
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1.5f);
        
        //Kill All In Radius
        foreach(var col in colliders)
        {
            var doll = col.GetComponentInParent<DamageableDoll>();
            var rocket = col.GetComponentInParent<Rocket>();

            if (doll != null)
            {
                doll.GetDamage(10f);
            }
            if(rocket != null && rocket != this)
            {
                rocket.ForceExplode();
            }
        }

        //fx
        var fx = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), this.transform.position, new Quaternion(), null);
        Destroy(fx, 3f);

        //Explode Wave
        yield return new WaitForSeconds(0.1f);
        foreach(var col in colliders)
        {
            if (col != null &&col.attachedRigidbody != null)
            {
                col.attachedRigidbody.AddForce((col.bounds.center - this.transform.position).normalized * 3f, ForceMode.VelocityChange);
            }
        }


        //Destory this
        Destroy(this.gameObject, 1f);

    }
}
