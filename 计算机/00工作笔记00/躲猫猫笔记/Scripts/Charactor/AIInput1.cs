using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;



public enum AIMode
{
    Stay,

    MoveToDst,

    AccessAmmo,

    Hunt,

    FoolChase
}




public class AIInput1 : MonoBehaviour, IGameInput
{
    private IGame game;

    private AIMode mode;
    public AIMode Mode { get { return this.mode; } }


    private string userName;
    public string UserName { get { return this.userName; } set { this.userName = value; } }

    [HideInInspector] public int team;
    public int Team { get { return 1; } }

    private Vector2 moveVec;
    public Vector2 MoveVec { get { return moveVec; } }

    private Vector3 viewDir;
    public Vector3 ViewDir { get { return this.viewDir; } }

    public bool Jump { get { return false; } }


    private NavMeshAgent agent;
    private CharactorController charactorController;

    //=====Stay==========
    private bool isStaying = false;
    private float maxStaytime = 99f;
    private float currentStayTime = 0f;


    //======MoveTo========
    private Vector3 globalDestination = new Vector3();

    //======HUNT==========
    private CharactorController huntTarget = null;
    private Vector3 huntTargetDir = new Vector3();
    private float huntTargetSqrDistance;
    private Collider huntTargetCollider = null;
    private bool huntTargetInView = false;

    private CharactorController attacker = null;

    //====Fool hunt=======
    private CharactorController foolChaseTarget = null;

    //=====Obtain=========
    private SupplyAmmo ammoTarget = null;




    //=====AI shoot======
    private float aimLerpSpeed = 4f;
    private float aimBiasFac = 0;
    private float biasLerpSpeed = 5f;
    private Vector3 biasUnitVec = new Vector3();
    private Vector3 targetBiasUnitVec = new Vector3();
    private float angleCanFire = 10f;

    private bool intelligentFiring = false;









    void Start()
    {
        game = GameObject.FindObjectOfType<GameStarter>().GetComponent<IGame>(); ;

        charactorController = this.GetComponent<CharactorController>();
        agent = this.GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;



        //Invoke
        InvokeRepeating("RepeatSetBias", Random.Range(0.5f, 1.5f), 1f);

        Invoke("SetRandomLocation", Random.Range(1f, 2f));
        InvokeRepeating("RepeatingLookForward", Random.Range(0.5f, 1.5f), 1f);
        InvokeRepeating("RepeatingValidateDst", Random.Range(0.5f, 4.5f), 4f);
        InvokeRepeating("ResetSimPosition", Random.Range(0.5f, 2.5f), 2f);
    }
    


    void Update()
    {
        // ---Time Cal---
        if (isStaying)
        {
            currentStayTime += Time.deltaTime;
        }
        else
        {
            currentStayTime = 0f;
        }


        //---AI Update---
        switch (mode)
        {
            case AIMode.Stay:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.white);

                    //move vec
                    moveVec = new Vector2();

                    //look around
                    viewDir = this.transform.forward;
                    
                    //auto transition to ...
                    if(currentStayTime > maxStaytime)
                    {
                        isStaying = false;
                        SetRandomLocation();
                    }
                }
                break;



            case AIMode.MoveToDst:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.green);

                    if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                    {
                        viewDir = this.transform.forward ;
                        moveVec = new Vector2(0, 0);

                        if (!isStaying) { Stay(Random.Range(2f, 4f)); }

                        break;
                    }

                    viewDir = agent.pathPending ? this.transform.forward : new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                    moveVec = new Vector2(0f, 1f);
                }
                break;



            case AIMode.Hunt:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.red);
                    Debug.DrawLine(this.transform.position + new Vector3(0, 1f, 0), this.transform.position + new Vector3(0, 1f, 0) + (viewDir * 1000f), Color.red);

                    //自己或者目标死亡退出狩猎模式（目标隐身）
                    if (!huntTarget.enabled || !this.enabled || huntTarget.aiInvisiable)
                    {
                        EndHunt();
                        break;
                    }
                    //没弹药退出狩猎模式
                    if(charactorController.AmmoCount <= 0)
                    {
                        EndHunt();
                        break;
                    }

                    //-------distance-----
                    huntTargetSqrDistance = (huntTarget.transform.position - this.transform.position).sqrMagnitude;



                    //------aim bias------
                    biasUnitVec = Vector3.Lerp(biasUnitVec, targetBiasUnitVec, Time.deltaTime * biasLerpSpeed);



                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(UtilsGame.GetFirePos(this.transform.position), huntTarget.transform.position - this.transform.position), out hit, 100f) && hit.collider == huntTargetCollider)
                    {
                        //视野内
                        huntTargetInView = true;

                        huntTargetDir = (huntTarget.transform.position - this.transform.position).normalized;

                        viewDir = Vector3.Lerp(viewDir, huntTargetDir + (biasUnitVec * aimBiasFac),    Time.deltaTime * aimLerpSpeed).normalized;//射击偏差
                        moveVec = new Vector2();

                        //SetIntelligentFire();
                        if (Vector3.Angle(huntTargetDir, viewDir) < angleCanFire)
                        {
                            SetIntelligentFire(Mathf.Clamp((2500f - huntTargetSqrDistance) / 2500f, 0.1f, 1f), Mathf.Clamp(huntTargetSqrDistance / 2500f, 0f, 1f)); //距离极限值：50m
                        }
                    }
                    else
                    {
                        //视野外
                        huntTargetInView = false;
                        
                        viewDir = new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                        agent.SetDestination(huntTarget.transform.position);
                        moveVec = new Vector2(0, 1f);

                        charactorController.SetConstaneFire(false);
                            
                            //目标丢失
                            if(huntTargetSqrDistance > Mathf.Pow(30f, 2f))
                            {
                                Stay(Random.Range(1f, 2f));
                            }
                    }
                }
                break;


            case AIMode.AccessAmmo:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.blue);

                    if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                    {
                        viewDir = this.transform.forward;
                        moveVec = new Vector2(0, 0);

                        if (!isStaying) { Stay(Random.Range(2f, 4f)); }
                        break;
                    }


                    viewDir = new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                    agent.SetDestination(huntTarget.transform.position);
                    moveVec = new Vector2(0, 1f);
                }
                break;


            case AIMode.FoolChase:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.black);

                    agent.SetDestination(foolChaseTarget.transform.position);

                    if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance))
                    {
                        viewDir = this.transform.forward;
                        moveVec = new Vector2(0, 0);

                        if (!isStaying) { Stay(Random.Range(2f, 4f)); }

                        break;
                    }

                    viewDir = agent.pathPending ? this.transform.forward : new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                    moveVec = new Vector2(0f, 1f);
                }
                break;

            default:
                {
                    Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 10f, 0), Color.black);

                    moveVec = new Vector2();
                    viewDir = this.transform.forward;
                }
                break;
        }


    }


    //------------------------------------------------------------------------------------

    /// <summary>
    /// 设置随机目的地
    /// </summary>
    private void SetRandomLocation()
    {
        //查找目标点(第一次查找最近的)
        NavLocation[] locations = FindObjectsOfType<NavLocation>().Where(loc => game.IsSafeArea(loc.transform.position) && (loc.transform.position - this.transform.position).sqrMagnitude > 5f).OrderBy(loc => (loc.transform.position - this.transform.position).sqrMagnitude).ToArray();

        if (locations.Length > 0)
        {
            if(globalDestination == new Vector3())
            {
                agent.SetDestination(locations[0].transform.position);
                this.globalDestination = locations[0].transform.position;
            }

            else
            {
                NavLocation rdmLoc = locations[Random.Range(0, locations.Length)];
                agent.SetDestination(rdmLoc.transform.position);
                this.globalDestination = rdmLoc.transform.position;
            }
            mode = AIMode.MoveToDst;
        }
    }

    /// <summary>
    /// 被攻击反击
    /// </summary>
    /// <param name="attacker"></param>
    public void UnderAttack(CharactorController attacker)
    {
        if (mode == AIMode.FoolChase) return;

        if (this.huntTarget == attacker) return;
        if (this.charactorController.AmmoCount <= 0) return;
        if (this.attacker == attacker) return;



        this.attacker = attacker;

        if(Random.value < 0.4f)
        {
            Stay(Random.Range(1f, 3f));
        }
    }



    /// <summary>
    /// 设置瞄准误差
    /// </summary>
    private void RepeatSetBias()
    {
        if(huntTarget != null)
        {
            //set rdm unitVec
            targetBiasUnitVec = Random.insideUnitSphere;


            //set Fac
            if(huntTarget.gameInput is PlayerInput)
            {
                aimBiasFac = 0.1f * Mathf.Clamp((50f * 50f) / huntTargetSqrDistance, 0f, 1f);
            }
            else
            {
                aimBiasFac = 0.025f * Mathf.Clamp((50f * 50f) / huntTargetSqrDistance, 0f, 1f);
            }
        }
    }

    /// <summary>
    /// 正前方查找敌人(正前方200米)
    /// </summary>
    private void RepeatingLookForward()
    {
        if (mode == AIMode.FoolChase) return;

        if (mode == AIMode.Hunt && huntTargetInView) return;

        if (this.charactorController.AmmoCount <= 0) return;

        Collider[] colliders = Physics.OverlapBox(this.transform.position + this.transform.forward * 100f, new Vector3(100f, 100f, 100f));
        
        foreach (Collider col in colliders)
        {
            //目标角色
            var charactor = col.GetComponentInParent<CharactorController>();


            //目标存在 && 目标未死亡 && 目标在75度视角内 && 目标未被遮挡
            if (charactor != null && charactor != this.charactorController && charactor.enabled == true && Vector3.Angle(charactor.transform.position - this.transform.position, this.transform.forward) < 75f && NoCover(col))
            {
                //如果目标AI隐身则略过
                if (charactor.aiInvisiable) continue;

                //距离
                float sqrDistance = (charactor.transform.position - this.transform.position).sqrMagnitude;

                
                if(sqrDistance < 900f)//绝对发现30m
                {
                    HuntTarget(charactor);
                    break;
                }
                else//低几率发现
                {
                    if(Random.value < 0.1f)
                    {
                        HuntTarget(charactor);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 正前方查找弹药
    /// </summary>
    private void RepeatingSearchAmmo()
    {
        if (mode == AIMode.FoolChase) return;

        if (mode == AIMode.AccessAmmo) return;//不会中途改变捡起目标。
        if (mode == AIMode.Hunt) return;//不会从追击模式直接变为捡拾模式。（追击模式弹药耗尽，先进入EndHunt停留模式）

        SupplyAmmo[] ammos = Physics.OverlapBox(this.transform.position + this.transform.forward * 100f, new Vector3(100f, 100f, 100f)).Select(c => c.GetComponent<SupplyAmmo>()).Where(su => su != null).ToArray();

        foreach( SupplyAmmo ammo in ammos)
        {
            AccessAmmoTarget(ammo);
            break;
        }
    }


    /// <summary>
    /// 目的地在安全区外
    /// </summary>
    private void RepeatingValidateDst()
    {
        if (mode == AIMode.MoveToDst)
        {
            if (!game.IsSafeArea(agent.destination))
            {
                Debug.Log(userName + ": 目的地现在在安全区外，重新设置目的地");
                SetRandomLocation();
            }
        }
    }


    //------------------------------------------------------------------------------------

    /// <summary>
    /// 停留
    /// </summary>
    private void Stay(float time)
    {
        isStaying = true;// start timer

        maxStaytime = time;

        mode = AIMode.Stay;
    }

    /// <summary>
    /// 追击目标/取消
    /// </summary>
    /// <param name="target"></param>
    private void HuntTarget(CharactorController target)
    {
        this.huntTarget = target;
        this.huntTargetCollider = target.GetComponentInChildren<Collider>();

        mode = AIMode.Hunt;
    }
    private void EndHunt()
    {
        this.huntTarget = null;
        this.huntTargetCollider = null;


        charactorController.SetConstaneFire(false);

        //mode = AIMode.MoveToDst;
        Stay(Random.Range(0.5f, 1f));
    }

    /// <summary>
    /// 进入捡拾弹药模式
    /// </summary>
    /// <param name="ammo"></param>
    private void AccessAmmoTarget(SupplyAmmo ammo)
    {
        this.ammoTarget = ammo;

        mode = AIMode.AccessAmmo;
    }
    private void EndAccess()
    {
        this.ammoTarget = null;
        Stay(Random.Range(0.5f, 1f));
    }

    /// <summary>
    /// 傻AI追踪
    /// </summary>
    public void FoolChase(CharactorController target)
    {
        foolChaseTarget = target;
        mode = AIMode.FoolChase;
    }

    /// <summary>
    /// 点射模式
    /// </summary>
    /// <param name="fireOrNot"></param>
    private void SetIntelligentFire(float intervalFire, float intervalStop)
    {
        if (!intelligentFiring)
        {
            StartCoroutine(CoIntelligentFire(intervalFire, intervalStop));
        }
    }
    private IEnumerator CoIntelligentFire(float intervalFire, float intervalStop)
    {
        intelligentFiring = true;

        this.charactorController.SetConstaneFire(true);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f) * intervalFire);
        this.charactorController.SetConstaneFire(false);
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f) * intervalStop);

        intelligentFiring = false;
    }

    /// <summary>
    /// 是否无阻隔
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    private bool NoCover(Collider col)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(UtilsGame.GetFirePos(this.transform.position), (col.transform.position - this.transform.position)), out hit, 10000f))
        {
            if (hit.collider == col)
            {
                return true;
            }
        }

        return false;
    }

    
    /// <summary>
    /// 更新模拟位置
    /// </summary>
    private void ResetSimPosition()
    {
        var tmp = agent.destination;
        agent.Warp(this.transform.position);
        agent.SetDestination(tmp);

        //会使remainDistance变为0

        if ((agent.nextPosition - this.transform.position).sqrMagnitude > 0.01f)//重设模拟位置过后仍有偏差则重启agent
        {
            //Debug.Log("Restart Agent");

            //可以重新计算路径
            //agent.enabled = false;
            //Vector3 destinationTmp = agent.destination;

            //agent.enabled = true;
            //agent.SetDestination(destinationTmp);
        }
    }







    //-----------------Die-------------------------------------------------------------------

    /// <summary>
    /// 死亡
    /// </summary>
    public void Disable()
    {
        this.enabled = false;
    }


    
}
