using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileNav : MonoBehaviour
{
    [HideInInspector] public IShipController targetObj;

    private float speed = 10f;

    private Vector3 initVelocity;

    private float timer = 0f;


    //Shell Stuff

    private Ray ray;
    private RaycastHit hit;


    [HideInInspector] public IShipController shooter;
    [HideInInspector] public float damage;
    [HideInInspector] public float piercingPercentage;



    void Start()
    {
        //速度继承
        initVelocity = this.shooter.GameObj.GetComponent<Rigidbody>().velocity;
        this.GetComponent<Rigidbody>().velocity = initVelocity;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //点火时间
        timer += Time.fixedDeltaTime;

        //加速
        speed = Mathf.Clamp(speed + 0.5f, 0f, 340f);

        //继承速度衰减
        if(timer > 2f)
        {
            this.GetComponent<Rigidbody>().velocity = Mathf.Clamp01((4f - (timer - 2f)) / 4f) * initVelocity;
        }

        //导航
        if(targetObj != null && timer > 2f)
        {
            Vector3 dir = (targetObj.GameObj.transform.position.OnOceanPlane(2f) - this.transform.position).normalized;
            Vector3 right = Vector3.Cross(Vector3.up, dir);
            Vector3 up = Vector3.Cross(dir, right);

            this.transform.rotation = Quaternion.Lerp(this.transform.rotation , Quaternion.LookRotation(dir, up), 0.02f);
        }

        //推进
        this.transform.position += this.transform.forward * 0.02f * speed;

        //撞击Ray
        ray = new Ray(this.transform.position, this.transform.forward);

        if (Physics.Raycast(ray, out hit, speed * 0.02f + 5f, (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water")), QueryTriggerInteraction.Collide))
        {
            Destroy(this.gameObject);

            Impact(hit.collider, hit.point);
        }

    }





    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);

        Impact(collision.collider, collision.contacts[0].point);
    }


    private void Impact(Collider collider, Vector3 pos)
    {
        if (collider.GetComponentInParent<Hull>() != null)
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


            //Sound
            SoundPlayer.PlaySound3D("炮弹撞击", pos, 1f);

            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/" + effectName), pos, effectRotation);
            Destroy(effect, 3.0f);

            collider.GetComponentInParent<Hull>().Damage(Utils.CalDamage(damage, collider.GetComponentInParent<Hull>().Armor * (1f - piercingPercentage)), this.shooter);
            
        }
        else if (collider.gameObject.CompareTag("WaterCollider"))
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/WaterSpray"), pos, Quaternion.LookRotation(Vector3.up));
            Destroy(effect, 3.0f);
        }
        else
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HitEffect"), pos, Quaternion.LookRotation(-this.GetComponent<Rigidbody>().velocity));
            Destroy(effect, 2.0f);
        }
    }

}
