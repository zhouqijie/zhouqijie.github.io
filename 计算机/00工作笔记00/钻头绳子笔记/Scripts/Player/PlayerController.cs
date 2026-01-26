using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerController : MonoBehaviour
{
    //coms
    private Camera cam;   public Camera Cam => this.cam;

    private DrillController drillCtrl;
    private HookController hookCtrl;

    private WaypointNav nav;   public Waypoint CurrentWaypoint => nav.Loc;

    private int layerMaskGrab;
    private int layerMaskDrill;


    //hp
    private float hp = 100f;
    private bool god = false;
    private bool dead = false;


    //Player Event
    public class ObjectBreakEvent<IDamageable> : UnityEvent<IDamageable> { }
    public ObjectBreakEvent<IDamageable> onBreakTarget;
    public ObjectBreakEvent<IDamageable> onSubpartBreakTarget;

    public UnityEvent onKill = new UnityEvent();
    public UnityEvent onFinishLevel = new UnityEvent();
    public UnityEvent onShare = new UnityEvent();
    public UnityEvent onGrabRocket = new UnityEvent();

    //tmp
    public static int moneySum = 0;
    public static int lvlSum = 0;







    private void Awake()
    {
        //coms
        cam = this.GetComponentInChildren<Camera>();
        drillCtrl = this.GetComponentInChildren<DrillController>();
        hookCtrl = this.GetComponentInChildren<HookController>();

        nav = this.GetComponent<WaypointNav>();

        //set weapon from userdata
        SetHookAndDrill();

        //event init
        onBreakTarget = new ObjectBreakEvent<IDamageable>();
        onSubpartBreakTarget = new ObjectBreakEvent<IDamageable>();
    }


    void Start()
    {
        layerMaskGrab = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Grabable") | 1 << LayerMask.NameToLayer("GrabableDoll") | 1 << LayerMask.NameToLayer("Damageable"));   //可破坏层级遮挡可抓取层级。
        layerMaskDrill = (1 << LayerMask.NameToLayer("Ragdoll") | 1 << LayerMask.NameToLayer("Armor") | 1 << LayerMask.NameToLayer("Damageable"));

        //tmp init
        moneySum = 0;


        //event listener(observers)
        onBreakTarget.AddListener((dmgAble) => {
            
            hookCtrl.OnTargetBroken(dmgAble);

            if (dmgAble == drillCtrl.Current)
            {
                drillCtrl.OnTargetBroken(dmgAble);
            }

        });

        onSubpartBreakTarget.AddListener((dmgAble) => {
            if (dmgAble == drillCtrl.Current)
            {
                drillCtrl.OnTargetBroken(dmgAble);
            }
        });

        //other events
        onKill.AddListener(() => {
            //kill count ++
            Infomanager.Instance.userData.todayKills++;

            moneySum += 5;
        });
        onFinishLevel.AddListener(() => {
            Infomanager.Instance.userData.todayLevels++;

            moneySum += 100;
            lvlSum++;
        });
        onShare.AddListener(() =>
        {
            Infomanager.Instance.userData.todayShares++;
        });
        onGrabRocket.AddListener(() => {
            Infomanager.Instance.userData.todayGrabRockets++;
        });

    }


    private void SetHookAndDrill()
    {
        //get idx
        int hookIdx = Infomanager.Instance.userData.activeHook;
        int drillIdx = Infomanager.Instance.userData.activeDrill;


        //set dps maxheat
        drillCtrl.DPS = (float)Infomanager.Instance.drillInfos[drillIdx].dps;
        drillCtrl.MaxHeat = (float)Infomanager.Instance.drillInfos[drillIdx].maxHeat; Debug.Log("SetMaxHeat:" + (float)Infomanager.Instance.drillInfos[drillIdx].maxHeat);


        //model
        var hook = Instantiate(Resources.Load<GameObject>("Prefabs/Hooks/Hook_" + hookIdx), new Vector3(), new Quaternion(), hookCtrl.hookHolder);
        hook.transform.localPosition = new Vector3();
        hook.transform.localRotation = new Quaternion();
        hook.transform.localScale = Vector3.one;



        var drill = Instantiate(Resources.Load<GameObject>("Prefabs/Drills/Drill_" + drillIdx), new Vector3(), new Quaternion(), drillCtrl.drillskinHolder);
        drill.transform.localPosition = new Vector3();
        drill.transform.localRotation = new Quaternion();
        drill.transform.localScale = Vector3.one;


        //hook speed // ????
        hookCtrl.SetHook();
        hookCtrl.Hook.FlySpeed = (float)Infomanager.Instance.hookInfos[hookIdx].speed;
    }


    public void ResetHooksAndDrills(int hookIdx, int drillIdx)
    {
        StartCoroutine(CoResetHooksAndDrills( hookIdx,  drillIdx));
    }
    private IEnumerator CoResetHooksAndDrills(int hookIdx, int drillIdx)
    {
        //del
        if (hookCtrl.hookHolder.childCount > 0)
        {
            DestroyImmediate(hookCtrl.hookHolder.GetChild(0).gameObject);
        }

        if (drillCtrl.drillskinHolder.childCount > 0)
        {
            DestroyImmediate(drillCtrl.drillskinHolder.GetChild(0).gameObject);
        }

        yield return null;
        yield return null;


        //set dps maxheat
        drillCtrl.DPS = (float)Infomanager.Instance.drillInfos[drillIdx].dps;
        drillCtrl.MaxHeat = (float)Infomanager.Instance.drillInfos[drillIdx].maxHeat; Debug.Log("SetMaxHeat:" + (float)Infomanager.Instance.drillInfos[drillIdx].maxHeat);


        //model
        var hook = Instantiate(Resources.Load<GameObject>("Prefabs/Hooks/Hook_" + hookIdx), new Vector3(), new Quaternion(), hookCtrl.hookHolder);
        hook.transform.localPosition = new Vector3();
        hook.transform.localRotation = new Quaternion();
        hook.transform.localScale = Vector3.one;



        var drill = Instantiate(Resources.Load<GameObject>("Prefabs/Drills/Drill_" + drillIdx), new Vector3(), new Quaternion(), drillCtrl.drillskinHolder);
        drill.transform.localPosition = new Vector3();
        drill.transform.localRotation = new Quaternion();
        drill.transform.localScale = Vector3.one;


        //hook speed // ????
        hookCtrl.SetHook();
        hookCtrl.Hook.FlySpeed = (float)Infomanager.Instance.hookInfos[hookIdx].speed;
    }



    public void StartRay(Ray ray)
    {

        //----最高优先级：钻头-------
        RaycastHit hit1;
        if(Physics.Raycast(ray, out hit1, 100f, layerMaskDrill))
        {
            IDamageable damageable = hit1.collider.GetComponentInParent<IDamageable>(); // chara

            float distance = (hit1.point - this.transform.position).magnitude;

            //dmamage stuff (钻头优先)
            if(this.nav.stopped && damageable != null && distance < 4f)
            {
                if(damageable.defaultPos != null)
                {
                    drillCtrl.StartDamage(damageable, hit1.collider, damageable.defaultPos.position);
                }
                else
                {
                    drillCtrl.StartDamage(damageable, hit1.collider, hit1.point);
                }

                return;
            }

            
        }

        //----第二优先级：发射钩爪-------
        RaycastHit hit2;
        if (Physics.Raycast(ray, out hit2, 100f, layerMaskGrab))
        {
            IGrabable grabable = hit2.collider.GetComponentInChildren<IGrabable>();
            
            if(grabable != null && hookCtrl.Hook.HookStatus == HookStatus.Idle)
            {
                hookCtrl.ShootHook(hit2.point);

                return;
            }
        }


        //----最低优先级：抛出-------
        if (hookCtrl.Hook.HookStatus == HookStatus.Grab)
        {
            hookCtrl.ThrowHook(ray);
        }
    }

    public void EndRay(Ray ray)
    {
        //if damaging
        drillCtrl.EndDamage();
    }

    public void Damage(float dmg)
    {
        //无敌
        if (god) return;

        //扣血
        hp -= dmg;

        if(hp <= 0f && !dead)
        {
            Die();
        }
    }

    public void Die()
    {
        dead = true;

        FindObjectOfType<Game>().Lose();
    }

    public void Recovery()
    {
        StartCoroutine(CoRecovery());
    }
    public IEnumerator CoRecovery()
    {
        dead = false;
        hp = 100f;
        god = true;

        yield return new WaitForSeconds(4f);

        god = false;
    }
    
}
