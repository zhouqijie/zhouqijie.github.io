using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableDoor : MonoBehaviour, IDamageable
{
    //Impl
    public GameObject gameObj => this.gameObject;
    public Transform breakPos; 
    public Transform defaultPos => breakPos;

    public bool IsBroken => this.isBroken;

    //Sub parts
    public DamageablePart[] subParts;
    private float damageMultip;

    //Hp
    public float maxHp;
    private float hp;

    //type
    public bool isSkinnedMesh;

    //(for skinned mesh renderers type)
    public SkinnedMeshRenderer[] smrs;


    //(for mesh renderers type)
    public MeshRenderer[] mrs;
    public Transform brokenPartHolder;
    private int currentMr = 0;


    //VFX
    public ParticleSystem vfx;


    //status
    private float currentDrillDps = 0f;
    private bool isDamaging = false;
    private bool isBroken = false;




    void Start()
    {
        hp = maxHp;
        damageMultip = 1f - (0.1f * subParts.Length);
    }

    void Update()
    {
        if (isDamaging && !isBroken)
        {
            Damage(Time.deltaTime * currentDrillDps * damageMultip);
        }
    }

    public void Damage(float dmg)
    {
        hp -= dmg;


        if (isSkinnedMesh)
        {
            RefreshSMR();
        }
        else
        {
            if (CalMrIndex() != currentMr)
            {
                currentMr = CalMrIndex();
                ResetMr(currentMr);
            }
        }

        if (hp < 0f)
        {
            isBroken = true;
            OnBreak();
        }
    }
    


    private int CalMrIndex()
    {
        if (hp <= 0f) return -1;

        int mrCount = mrs.Length;
        int id = Mathf.Clamp(Mathf.FloorToInt(mrCount * ((maxHp - hp) / maxHp)), 0, mrCount);
        return id;
    }

    private void ResetMr()
    {
        int idx = CalMrIndex();
        ResetMr(idx);
    }
    private void ResetMr(int idx)  // reverse?
    {
        for(int i = 0;i < mrs.Length; i++)
        {
            if (i != idx) mrs[i].enabled = false;
            else mrs[i].enabled = true;
        }
    }

    private void RefreshSMR()
    {
        foreach(var smr in smrs)
        {
            smr.SetBlendShapeWeight(0, ((maxHp - hp) / maxHp) * 100f);
        }
    }

    public void StartDamage(Collider collider, float dps)
    {
        currentDrillDps = dps;
        isDamaging = true;
    }

    public void EndDamage()
    {
        isDamaging = false;
    }


    public void OnSubpartBreak()
    {
        damageMultip += 0.1f;
    }
    
    
    public void OnBreak()
    {
        //vfx
        if(vfx != null) vfx.Play();

        //this collider
        this.GetComponent<Collider>().enabled = false;

        //sub parts
        foreach(var part in subParts)
        {
            part.BrokenImmidiately();
        }

        //effects
        if(isSkinnedMesh)
        {
            foreach(var smr in smrs)
            {
                smr.GetComponent<Collider>().enabled = true;
                smr.GetComponent<Rigidbody>().isKinematic = false;
                smr.GetComponent<Rigidbody>().useGravity = true;
                smr.GetComponent<Rigidbody>().AddForce((this.transform.position - FindObjectOfType<PlayerController>().transform.position).normalized * 10f, ForceMode.VelocityChange);
                smr.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere, ForceMode.VelocityChange);

                Destroy(smr.gameObject, 10f);
            }
        }
        else
        {
            ResetMr();

            foreach(Transform t in brokenPartHolder)
            {
                t.gameObject.SetActive(true);
                t.parent = null;
                t.GetComponent<Collider>().enabled = true;
                t.GetComponent<Rigidbody>().isKinematic = false;
                t.GetComponent<Rigidbody>().useGravity = true;
                t.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 2f, ForceMode.VelocityChange);
                t.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere, ForceMode.VelocityChange);

                Destroy(t.gameObject, 10f);
            }
        }



        //event invoke
        FindObjectOfType<PlayerController>().onBreakTarget.Invoke(this);
    }

    
}
