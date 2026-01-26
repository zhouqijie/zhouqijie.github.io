using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Shell : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;

    private Rigidbody rigid;
    private Vector3 velocity;
    

    [HideInInspector] public IShipController shooter;
    [HideInInspector] public float damage;
    [HideInInspector] public float piercingPercentage;
    [HideInInspector] public bool isSecondary = false;
    [HideInInspector] public bool isTorpedo = false;
    //minAngle..
    //radius...
    //...

    private void Awake()
    {
        rigid = this.GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (!isTorpedo)
        {
            //重力
            rigid.AddForce(-98f * Vector3.up, ForceMode.Acceleration);

            velocity = this.GetComponent<Rigidbody>().velocity;

            this.transform.rotation = Quaternion.LookRotation(velocity);


            //Ray//鱼雷不使用射线
            ray = new Ray(this.transform.position, this.GetComponent<Rigidbody>().velocity);
            if (Physics.Raycast(ray, out hit, velocity.magnitude * 0.02f + 5f, (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water")), QueryTriggerInteraction.Collide))
            {
                Destroy(this.gameObject);
                
                Impact(hit.collider, hit.point);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);

        Impact(collision.collider, collision.contacts[0].point);
    }


    private void Impact(Collider collider, Vector3 pos)
    {
        Hull hull = collider.GetComponentInParent<Hull>();
        if (hull != null)
        {
            Quaternion effectRotation = Quaternion.LookRotation(-this.GetComponent<Rigidbody>().velocity);

            string effectName = "HitEffectHull";
            if (GetComponent<MissileNav>() != null)
            {
                effectName = "HitEffectMissile";
            } 
            if (GetComponent<TorpedoNav>() != null)
            {
                effectName = "HitEffectTorpedo";
                effectRotation = Quaternion.LookRotation(Vector3.up);
            } 


            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + effectName), pos, effectRotation);
            Destroy(effect, 3.0f);

            float trueDamage = Utils.CalDamage(damage, collider.GetComponentInParent<Hull>().Armor * (1f - piercingPercentage));

            //伤害
            hull.Damage(trueDamage, this.shooter);

            //Sound
            SoundPlayer.PlaySound3D("撞击1", pos, 1f);
            

            //Notice
            if (this.shooter is PlayerController)
            {
                if (isTorpedo)
                {
                    GameNotice.Notice(new Color(0.1f, 0.1f, 0.65f, 0.75f), "null", "鱼雷命中目标 × ", 3);
                }
                else if (isSecondary)
                {
                    GameNotice.Notice(new Color(0.75f, 0.75f, 0.2f, 0.75f), "null", "副炮命中目标 × ", 2);
                }
                else
                {
                    GameNotice.Notice(new Color(0.75f, 0.2f, 0.2f, 0.75f), "null", "主炮命中目标 × ", 1);
                }

                //伤害数字
                if(!isSecondary)
                {
                    hull.DamageTxtEnQueue(trueDamage);
                }
            }
        }
        else if (collider.gameObject.CompareTag("WaterCollider"))
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/WaterSpray"), pos, Quaternion.LookRotation(Vector3.up));
            Destroy(effect, 3.0f);

            //Sound
            SoundPlayer.PlaySound3D("水花1", pos, 0.5f);
        }
        else
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HitEffect"), pos, Quaternion.LookRotation(-this.GetComponent<Rigidbody>().velocity));
            Destroy(effect, 2.0f);
        }



        //AOE
        
        Collider[] colliders = Physics.OverlapSphere(pos, 10f, (1 << LayerMask.NameToLayer("Default")));
        foreach(var col in colliders)
        {
            Hull h = col.GetComponentInParent<Hull>();
            if (h != null)
            {
                h.Damage(Utils.CalDamage(damage * 0.25f, h.Armor * (1f - piercingPercentage)), this.shooter);//溅射伤害0.25f倍
            }
        }
    }
}
