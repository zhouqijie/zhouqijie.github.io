using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Hull : MonoBehaviour
{
    private bool sunk = false;

    private static UICanvas uiCanvas;

    private IShipController controller;
    // lvl and secondary
    
    public GameObject seconaryShell;
    public float secondaryBaseReloadTime;
    public float secondaryV0;

    //HP ARMOR
    private float maxhp;
    private float hp;
    private float armor;

    [HideInInspector] public float armorMultipiler = 1.0f;
    [HideInInspector] public float accuracyMultipiler = 1.0f;


    [HideInInspector] public float MaxHP { get { return this.maxhp; } set { this.maxhp = value; } }
    [HideInInspector] public float HP { get { return this.hp; } set { this.hp = value; } }
    [HideInInspector] public float HPpercent { get { return HP / MaxHP; } }
    [HideInInspector] public float Armor { get { return this.armor; } set { this.armor = value; } }

    //Bank factor
    [InspectorName("Bank Factor")] public float forceDepth;


    //ShipConfig
    [HideInInspector] public float shipPowerMultip;

    [HideInInspector] public ShipConfig shipConfig;

    //tmp damage queue--
    private Queue<float> dmgQueue = new Queue<float>();
    public void DamageTxtEnQueue(float dmg)
    {
        dmgQueue.Enqueue(dmg);
        if(dmgTxtNumCurrent == 0)
        {
            StartCoroutine(CoShowDmg());
        }
    }

    //tmp damage---
    private int dmgTxtNumCurrent = 0;
    private IEnumerator CoShowDmg()
    {
        float tmpfac = -1f;

        while(dmgQueue.Count > 0)
        {
            float dmg = dmgQueue.Dequeue();
            
            tmpfac = BillboardText.ShowNumber(Mathf.RoundToInt(dmg), this, dmgTxtNumCurrent, Color.red, tmpfac);

            dmgTxtNumCurrent += 1;

            yield return new WaitForSeconds(0.1f);
        }

        dmgTxtNumCurrent = 0;
    }


    private void Awake()
    {
        uiCanvas = FindObjectOfType<UICanvas>();
    }

    void Start()
    {
        ////角阻力/阻力
        this.GetComponent<Rigidbody>().angularDrag = 0.25f;//best: 0.25f
        this.GetComponent<Rigidbody>().drag = 0.5f;

        ////旋转张量
        //this.GetComponent<Rigidbody>().inertiaTensor = new Vector3(30000000f, 30000000f, 1800000f);
        //Debug.Log("Tensor:" + GetComponent<Rigidbody>().inertiaTensor);//best: 30M, 30M, 1.8M

        ////重心
        this.GetComponent<Rigidbody>().centerOfMass = this.transform.Find("MASS_CENTER").localPosition;

        //CTRLer
        controller = this.GetComponentInParent<IShipController>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(this.GetComponent<Rigidbody>().centerOfMass, this.GetComponent<Rigidbody>().centerOfMass + new Vector3(0f, 50f, 0f));
        

        ////垂直阻尼
        //float speed_verti = Vector3.Dot(this.GetComponent<Rigidbody>().velocity, this.transform.up.normalized);
        //this.GetComponent<Rigidbody>().AddForce(this.transform.up * Mathf.Sign(speed_verti) * -0.5f, ForceMode.Force);
        

        ////防侧滑
        if (!sunk && this.transform.position.y < 4.5f)
        {
            Vector3 force_Sideway = -Vector3.Dot(this.GetComponent<Rigidbody>().velocity, this.transform.right.normalized) * this.GetComponent<Rigidbody>().mass * 2f * this.transform.right.normalized;
            this.GetComponent<Rigidbody>().AddForceAtPosition(force_Sideway, (transform.position + transform.up * -forceDepth), ForceMode.Force);
        }

        

        //生命//沉没

        if (hp < 0f && this.GetComponent<IShipController>().Enabled == true)
        {

            this.GetComponent<IShipController>().Enabled = false;



            //死亡广播
            this.GetComponentInParent<IShipController>().DieBoardcast();

            //禁用炮塔
            foreach (var turret in this.GetComponentsInChildren<Turret>())
            {
                turret.enabled = false;
            }

            //烟囱效果去除???Clean??
            foreach(var chimney in this.GetComponentsInChildren<ChimneyParticles>(true))
            {
                var em = chimney.GetComponent<ParticleSystem>().emission;
                em.enabled = false;
            }
            //水波效果去除
            WaterParticles wp = this.GetComponentInChildren<WaterParticles>(true);
            if (wp != null) { wp.Clean(); }

            //爆炸声
            SoundPlayer.PlaySound3D("爆炸大", this.transform.position, 1f, 50000f);

            //爆炸特效
            GameObject explode = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), this.transform.position, new Quaternion(), this.transform);

            //消失程序
            if (FindObjectsOfType<AIController>().ToArray().Where(ship => ship.Team == 1 && ship.Enabled).ToArray().Length == 0)
            {
                explode.GetComponent<Explode>().ExecuteProgram(2f);
                StartCoroutine(DieRemoveMesh(0f));
            }
            else if(GetComponentInParent<PlayerController>() != null)
            {
                explode.GetComponent<Explode>().ExecuteProgram(2f);
                StartCoroutine(DieRemoveMesh(0f));
            }
            else
            {
                explode.GetComponent<Explode>().ExecuteProgram(3f);
                StartCoroutine(DieRemoveMesh(3f));
            }
        }
    }



    public IEnumerator DieRemoveMesh(float delay = 5f)
    {
        yield return new WaitForSeconds(delay);

        //下沉
        this.GetComponentInChildren<Float>().Sunk();
        this.sunk = true;

        yield return new WaitForSeconds(5f);

        foreach (var mr in this.GetComponentsInChildren<MeshRenderer>())
        {
            mr.enabled = false;
        }
        foreach (var mf in this.GetComponentsInChildren<MeshFilter>())
        {
            mf.mesh = null;
        }
    }




    //---------------------------------------------------------------------------------------------------------
    public void Damage(float damage, IShipController shooter)
    {
        if (shooter.Team == controller.Team) return;//忽略友方伤害

        hp -= damage;

        //是敌对攻击
        if (this.GetComponent<IShipController>().Team != shooter.Team)
        {
            if (this.GetComponentInParent<IShipController>().Target01 != null)
            {
                if (this.GetComponentInParent<IShipController>().Target01 != shooter.GameObj)
                {
                    this.GetComponentInParent<IShipController>().Target02 = this.GetComponentInParent<IShipController>().Target01;
                }
            }
            this.GetComponentInParent<IShipController>().Target01 = shooter.GameObj;

            shooter.TotalDamage += damage;
        }

        //玩家收到伤害特效
        if (this.GetComponent<IShipController>() is PlayerController)
        {
            uiCanvas.HurtEffect(damage / this.maxhp);
        }
        
    }
}
