using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageablePart : MonoBehaviour, IDamageable
{
    //Impl
    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => this.breakPos;

    public bool IsBroken => this.isBroken;

    //coms
    public Transform breakPos;


    //Hp
    public float maxHp;
    private float hp;

    //part debris
    public Rigidbody[] parts;


    //status
    private float currentDrillDps = 0f;
    private bool isDamaging = false;
    private bool isBroken = false;




    void Start()
    {
        hp = maxHp;
    }
    

    void Update()
    {
        if (isDamaging && !isBroken)
        {
            Damage( Time.deltaTime * currentDrillDps);
        }
        
    }

    public void Damage(float dmg)
    {
        hp -= dmg;

        if (!isBroken && hp < 0f)
        {
            isBroken = true;
            OnBreak();
        }
    }


    public void StartDamage(Collider collider, float dps)
    {
        isDamaging = true;
        currentDrillDps = dps;
    }

    public void EndDamage()
    {
        isDamaging = false;
    }

    public void BrokenImmidiately()
    {
        Damage(this.hp + 0.1f);
    }

    public void OnBreak()
    {
        //this collider
        this.GetComponent<Collider>().enabled = false;
        

        //parts debris
        foreach(var part in parts)
        {
            part.transform.parent = null;
            part.isKinematic = false;
            part.useGravity = true;
            part.AddForce(Random.insideUnitSphere * Random.Range(2f, 4f), ForceMode.VelocityChange);
            part.GetComponent<Collider>().enabled = true;

            Destroy(part.gameObject, 10f);
        }

        //anounce
        var door = this.GetComponentInParent<DamageableDoor>();
        if(door != null) 


        //event invoke
        FindObjectOfType<PlayerController>().onBreakTarget.Invoke(this);
    }

}
