using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum AIMode
{
    Idle,
    Hunt,
    MoveTo,
    Guard,
}

public class AIController : MonoBehaviour, IShipController
{
    private string name = "匿名";
    public string Name { get { return this.name; } set { this.name = value; } }

    //interfase impl
    public int team;

    public AIMode aiMode = AIMode.Idle;
    [HideInInspector] public bool mad = false;

    public float primarydistance = 3000f;
    public float secondarydistance = 1500f;

    private float throttle;
    private float rudder;
    private Vector3 target;
    private bool primaryFiringOrder;
    private bool secondaryFiringOrder;
    private bool ability1Order;
    private bool ability2Order;

    public GameObject GameObj { get { return this.gameObject; } }

    public float Throttle { get { return this.throttle; } }

    public float Rudder { get { return this.rudder; } }

    public Vector3 Target { get { return this.target; } }

    public int Team { get { return team; } }

    public bool PrimaryFiringOrder { get { return this.primaryFiringOrder; } }
    public bool SecondaryFiringOrder { get { return this.secondaryFiringOrder; } }
    public bool Ability1Order { get { return this.ability1Order; } }
    public bool Ability2Order { get { return this.ability2Order; } }

    //Enabled
    public bool Enabled { get { return this.enabled; } set { this.enabled = value; } }


    //Targeting
    public GameObject targetObjHunt = null;//搜寻目标
    public GameObject targetLock = null;

    public GameObject TargetLock { get { return this.targetLock; } }

    public void RepeatingLock()
    {
        this.targetLock = null;

        if (targetObjHunt != null)
        {
            if ((targetObjHunt.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(FindObjectOfType<Game>().viewDistance * Game.identityDistanceFac, 2))
            {
                this.targetLock = this.targetObjHunt;
            }
        }
    }

    private GameObject target01 = null;
    private GameObject target02 = null;
    public GameObject Target01 { get { return this.target01; } set { this.target01 = value; } }
    public GameObject Target02 { get { return this.target02; } set { this.target02 = value; } }


    //kill
    private int kills;
    private int assists;
    public int KillCount { get { return this.kills; } set { this.kills = value; } }
    public int AssistCount { get { return this.assists; } set { this.assists = value; } }
    private float totalDamage;
    public float TotalDamage { get { return this.totalDamage; } set { this.totalDamage = value; } }


    //Nav
    private Vector3 destination;



    //time counter tmp
    private const float timer1Max = 3f;//索敌计时器
    private float timer1 = timer1Max;
    private const float timer2max = 10f;//航行计时器
    private float timer2 = timer2max;
    private const float timer3max = 2f;//开火及技能计时器
    private float timer3 = timer3max;



    // Start is called before the first frame update
    void Start()
    {
        target = new Vector3();
        //hull
        Hull hull = this.gameObject.GetComponent<Hull>();
        

        

        switch (aiMode)
        {
            case AIMode.Idle:
                {
                    throttle = 0.0f;
                }
                break;
            case AIMode.Guard:
                {
                    throttle = 0.0f;
                }
                break;
            case AIMode.Hunt:
                {
                    throttle = 0.9f;
                }
                break;
            case AIMode.MoveTo:
                {
                    throttle = 0.2f;
                }
                break;
            default:
                throw new UnityException("No AI Mode");
                break;
        }

        //循环锁定
        InvokeRepeating("RepeatingLock", Random.Range(0.5f, 1f), 0.9f);
    }

    // Update is called once per frame
    void Update()
    {
        //Graphics.DrawTexture(new Rect(new Vector2(), new Vector2(100, 100)), Resources.Load<Texture2D>("Textures/AIInfo"));

        switch (aiMode)
        {
            case AIMode.Idle:
                {
                    timer1 -= Time.deltaTime;
                    if (timer1 < 0f)
                    {
                        SearchingTargetInDistance();
                        if (targetObjHunt != null && targetObjHunt.GetComponent<IShipController>().Enabled == true)
                        {
                            this.aiMode = AIMode.Hunt;
                        }

                        if(targetObjHunt != null && targetObjHunt.GetComponent<IShipController>().Enabled == true)
                        {
                            this.aiMode = AIMode.Hunt;
                            targetObjHunt = this.target01;
                        }
                    }
                }
                break;
            case AIMode.Hunt:
                {
                    //---索敌---
                    timer1 -= Time.deltaTime;
                    if (timer1 < 0f)
                    {
                        SearchingTarget();
                        timer1 = timer1Max;
                    }

                    //---设置目标---
                    timer2 -= Time.deltaTime;
                    if(timer2 < 0f)
                    {
                        if(targetObjHunt != null)
                        {
                            //Set dest
                            if (!mad)
                            {
                                float distance = Mathf.Clamp((targetObjHunt.transform.position - this.transform.position).magnitude, 4000f, 10000f);
                                destination = targetObjHunt.transform.position + new Vector3(Random.Range(-distance / 2f, distance / 2f), 0, Random.Range(-distance / 2f, distance / 2f));
                            }
                            else
                            {
                                destination = targetObjHunt.transform.position;
                            }
                        }
                        timer2 = Random.Range(0.8f, 1f) * timer2max;
                    }


                    //---航行---
                    NavDestination();

                    //---开火---
                    timer3 -= Time.deltaTime;
                    if(timer3 < 0f)
                    {
                        RepeatingSetFireAndAbility();
                        timer3 = timer3max * Random.Range(0.75f, 1f);
                    }
                }
                break;
            case AIMode.MoveTo:
                {
                    //---索敌---
                    timer1 -= Time.deltaTime;
                    if (timer1 < 0f)
                    {
                        SearchingTarget();
                        timer1 = timer1Max;
                    }
                    
                    //---航行---
                    if (targetObjHunt != null && targetObjHunt.GetComponent<IShipController>().Enabled == true)
                    {
                        NavDestination();
                        throttle = 0.9f;
                    }
                    else
                    {
                        throttle = 0.0f;
                    }

                    //---开火---
                    timer3 -= Time.deltaTime;
                    if (timer3 < 0f)
                    {
                        RepeatingSetFireAndAbility();
                        timer3 = timer3max * Random.Range(0.75f, 1f);
                    }
                }
                break;
            default:
                break;
        }
        
    }

    #region 开火指令延迟
    private void RepeatingSetFireAndAbility()
    {
        //---攻击---
        if (targetObjHunt != null && targetObjHunt.GetComponent<IShipController>().Enabled == true)//有目标
        {
            //AI预瞄
            target = targetObjHunt.transform.position;

            //主武器开火：
            if ((targetObjHunt.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(primarydistance, 2))
            {
                TryToFirePrimary();
            }
            else
            {
                primaryFiringOrder = false;
            }

            //副武器开火：
            if ((targetObjHunt.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(secondarydistance, 2))
            {
                TryToFireSecondary();
            }
            else
            {
                secondaryFiringOrder = false;
            }

            //技能
            Ability ab = this.GetComponentsInChildren<Ability>().FirstOrDefault(a => a.Index == 2 && a.CanUse == true);
            if (ab != null)
            {
                ability2Order = true;
            }
            else
            {
                ability2Order = false;
            }
        }
        else
        {
            //主武器开火：
            primaryFiringOrder = false;
            //副武器开火：
            secondaryFiringOrder = false;

            //技能
            ability2Order = false;
        }
    }

    private bool isInvokingFireP = false;
    private bool isInvokingFireS = false;
    void TryToFirePrimary()
    {
        if (isInvokingFireP) return;

        if (!primaryFiringOrder)
        {
            isInvokingFireP = true;
            Invoke("FirePrimary", Random.Range(1f, 4f));
        }
    }
    void TryToFireSecondary()
    {
        if (isInvokingFireS) return;

        if (!secondaryFiringOrder)
        {
            isInvokingFireS = true;
            Invoke("FireSecondary", Random.Range(1f, 4f));
        }
    }
    void FirePrimary()
    {
        isInvokingFireP = false;
        primaryFiringOrder = true;
    }
    void FireSecondary()
    {
        isInvokingFireS = false;
        secondaryFiringOrder = true;
    }
    #endregion


    void SearchingTarget()
    {
        List<IShipController> shipControllers = new List<IShipController>();
        shipControllers.AddRange(FindObjectsOfType<AIController>());
        shipControllers.Add(FindObjectOfType<PlayerController>());

        targetObjHunt = null;
        //foreach(var item in shipControllers)
        //{
        //    if(item.Team != team && item.Enabled == true)
        //    {
        //        targetObj = item.GameObj;
        //    }
        //}

        IShipController[] controllers = shipControllers.Where(c => c.Team != team && c.Enabled == true).ToArray();//[]
        if(controllers.Length > 0)
        {
            targetObjHunt = controllers[Random.Range(0, controllers.Length)].GameObj;
        }
    }

    void SearchingTargetInDistance()
    {
        List<IShipController> shipControllers = new List<IShipController>();
        shipControllers.AddRange(FindObjectsOfType<AIController>());
        shipControllers.Add(FindObjectOfType<PlayerController>());

        targetObjHunt = null;

        IShipController[] controllers = shipControllers.Where(c => c.Team != team && c.Enabled == true).Where(c => (c.GameObj.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(FindObjectOfType<Game>().viewDistance, 2)).ToArray();//[]
        if (controllers.Length > 0)
        {
            targetObjHunt = controllers[Random.Range(0, controllers.Length)].GameObj;
        }
    }

    void NavDestination()
    {
        float angle = -Vector3.SignedAngle(destination - this.transform.position, this.transform.forward, this.transform.up);
        angle = angle > 180f ? (angle - 360f) : angle;

        rudder = Mathf.Clamp((angle / 60f), -1f, 1f); //60角度满舵

        throttle = 0.8f;
    }

    public void DieBoardcast()
    {
        if(this.Target01 != null)
        {
            this.Target01.GetComponent<IShipController>().KillCount += 1;
        }

        if (this.Target02 != null)
        {
            this.Target02.GetComponent<IShipController>().AssistCount += 1;
        }

        
        FindObjectOfType<Game>().Judge();

        //刷新计分版
        FindObjectOfType<UICanvas>().RefreshStatistics();
    }
}
