using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum AIModeHunter
{
    Normal,
    LocSearching,
    Hunting,
    Obtaing
}

public enum AIModeHider
{
    MoveTo,
    Hiding
}

public class AIInput0 : MonoBehaviour, IGameInput
{
    private IGame game;

    //---status---
    private AIModeHunter modeHunter;
    private AIModeHider modeHider;

    //------------
    private string userName;
    public string UserName { get { return this.userName; } set { this.userName = value; } }

    [HideInInspector] public int team;
    public int Team { get { return this.team; } }

    private Vector2 moveVec;
    public Vector2 MoveVec { get { return this.moveVec; } }

    private Vector3 viewDir;
    public Vector3 ViewDir { get { return this.viewDir; } }

    public bool Jump { get { return false; } }



    private NavMeshAgent agent;
    private CharactorController charactorController;


    //-------tmp blockcheck----
    private Vector3 blockPos = new Vector3();


    //-------tmp disguise-----
    private bool isDisguiseing = false;


    //-------tmp hunt-----
    private Coroutine huntCorouting = null;
    private CharactorController huntTarget = null;
    private float huntTargetSqrDistance = 0f;
    private bool intelligentFiring = false;

    
    private float aimLerpSpeed = 4f;
    private float aimBiasFac = 0f;
    private float biasLerpSpeed = 5f;
    private Vector3 biasUnitVec = new Vector3();
    private Vector3 targetBiasUnitVec = new Vector3();


    //--------tmp obtain---------
    private Coroutine obtainCorouting = null;

    private GameObject objObtain = null;





    //debug
    [HideInInspector] public bool debug = false;








    void Start()
    {
        game = GameObject.FindObjectOfType<GameStarter>().GetComponent<IGame>(); ;

        charactorController = this.GetComponent<CharactorController>();
        agent = this.GetComponent<NavMeshAgent>();
        agent.updatePosition = false;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        switch (team)
        {
            case 0://Hidder
                {
                    Invoke("SetRandomLocation", Random.Range(0.1f, 0.5f));//to hide
                    InvokeRepeating("ResetSimPosition", Random.Range(0.6f, 1.5f), 0.9f);
                    
                    InvokeRepeating("RepeatingValidateDst", Random.Range(0.5f, 4.5f), 4f);
                }
                break;
            default://Searcher
                {
                    InvokeRepeating("RepeatSetBias", Random.Range(0.5f, 1.5f), 1f);

                    Invoke("SetRandomLocation", Random.Range(0.1f, 0.5f));//to search
                    InvokeRepeating("ResetSimPosition", Random.Range(0.6f, 1.5f), 0.9f);

                    InvokeRepeating("RepeatingValidateDst", Random.Range(0.5f, 4.5f), 4f);
                    InvokeRepeating("RepeatingSearchForward", Random.Range(0.5f, 1.5f), 1f);
                    InvokeRepeating("RepeatingSearchAmmoForward", Random.Range(0.5f, 1.5f), 1f);
                    InvokeRepeating("RepeatingBlockCheck", Random.Range(0.5f, 2.5f), 2f);
                }
                break;
        }
        

    }
    

    void Update()
    {
        if(team == 1)
        {
            //switch (modeHunter)
            //{
            //    case AIModeHunter.Normal:
            //        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 50f, 0), Color.green);
            //        break;
            //    case AIModeHunter.LocSearching:
            //        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 50f, 0), Color.white);
            //        break;
            //    case AIModeHunter.Hunting:
            //        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 50f, 0), Color.red);
            //        break;
            //    case AIModeHunter.Obtaing:
            //        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 50f, 0), Color.blue);
            //        break;
            //    default:
            //        break;
            //}
        }





        switch (team)
        {
            case 0://---躲藏者---
                {
                    if (ClassicGame.Instance.GameTime < 2f) break;//AI输入开局冻结

                    bool reach = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance);
                    moveVec = reach ? new Vector2() : new Vector2(0, 1f);

                    if (!reach)
                    {
                        //debug
                        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), Color.green);

                        viewDir = new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                    }
                    else
                    {
                        if (isDisguiseing)
                        {
                            //debug
                            Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), Color.blue);
                        }

                        if (charactorController.IsDisguised)
                        {
                            //debug
                            Debug.DrawLine(this.transform.position + new Vector3(0.1f, 0f , 0.1f), this.transform.position + new Vector3(0.2f, 20f, 0.2f), Color.black);
                        }
                    }

                    //once: start hide
                    if (!isDisguiseing && !charactorController.IsDisguised && reach)
                    {
                        //debug
                        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), new Color(1f, 0f, 1f, 1f));

                        StartCoroutine(CoHide());
                    }

                    //once
                    if (charactorController.IsDisguised)
                    {
                        if (!ClassicGame.Instance.IsSafeArea(this.transform.position))
                        {
                            charactorController.SwitchDisguise();//loop??
                            
                            SetRandomLocation();
                        }
                    }
                }
                break;
            default://---搜寻者---
                {
                    if (ClassicGame.Instance.GameTime < 2f) break;//AI输入开局冻结


                    //------aim bias------
                    biasUnitVec = Vector3.Lerp(biasUnitVec, targetBiasUnitVec, Time.deltaTime * biasLerpSpeed);


                    //---------------------
                    if (modeHunter == AIModeHunter.Normal || modeHunter == AIModeHunter.LocSearching)//移动到躲藏点或者搜寻模式 模式
                    {

                        //debug
                        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), Color.green);

                        bool reach = !agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance);
                        moveVec = reach ? new Vector2() : new Vector2(0, 1f);

                        if (!reach && modeHunter != AIModeHunter.Hunting)//正在移动到躲藏点
                        {
                            viewDir = new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                        }

                        if (modeHunter != AIModeHunter.LocSearching && reach)//正在目标点-开始搜寻
                        {
                            StartCoroutine(CoSearch());
                        }
                    }

                    if(modeHunter == AIModeHunter.Hunting)//追击模式
                    {
                        //moveVec = new Vector2();在协程中处理。。
                        //debug
                        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), Color.red);
                    }


                    if (modeHunter == AIModeHunter.Obtaing)
                    {
                        //moveVec = new Vector2(0, 1f);在协程中处理。。
                        //debug
                        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0f, 20f, 0f), Color.blue);
                    }
                }
                break;
        }


        //view dir prevent 0
        if (viewDir.sqrMagnitude < 0.001f) viewDir = this.transform.forward;
    }



    //----------------------------------------------------Invokes----------------------------------------------------------------



    /// <summary>
    /// 设置瞄准误差
    /// </summary>
    private void RepeatSetBias()
    {
        if (huntTarget != null)
        {
            //set rdm unitVec
            targetBiasUnitVec = Random.insideUnitSphere;


            //set Fac
            if (huntTarget.gameInput is PlayerInput)
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
    /// 正前方查找敌人
    /// </summary>
    private void RepeatingSearchForward()
    {
        //...  //可以中途改变目标

        if(modeHunter == AIModeHunter.Obtaing)
        {
            if(charactorController.AmmoCount < 1)
            {
                return;//弹药不足。不能从捡拾状态转换到追逐。
            }
        }

        Collider[] colliders = Physics.OverlapBox(this.transform.position + this.transform.forward * 100f, new Vector3(100f, 100f, 100f));

        foreach (Collider col in colliders)
        {
            var charactor = col.GetComponentInParent<CharactorController>();
            if (charactor != null && charactor.gameInput.Team == 0 && charactor.enabled == true && Vector3.Angle(charactor.transform.position - this.transform.position, this.transform.forward) < 75f && NoCover(col))
            {

                //如果游戏未开始则跳过
                if (ClassicGame.Instance.GameTime < ClassicGame.hideTime + 1f) continue;

                //如果对AI隐身则跳过
                if (charactor.aiInvisiable == true) continue;

                //距离
                float sqrDistance = (charactor.transform.position - this.transform.position).sqrMagnitude;


                if (sqrDistance < 900f)//近距离30m内
                {
                    //判定是否发现设为追击目标
                    if (!charactor.IsDisguised)//未伪装直接发现
                    {
                        StartCoHunt(charactor);
                        break;
                    }
                    else//伪装后有发现可能
                    {
                        //野外或者屋顶极大概率被发现
                        if (Mathf.RoundToInt(charactor.transform.position.y) < 5 || Mathf.RoundToInt(charactor.transform.position.y) > 6)
                        {
                            if (Random.value < 0.6f)
                            {
                                StartCoHunt(charactor);
                                break;
                            }
                        }
                        //其他伪装情况(周围有无预制体)
                        else
                        {
                            int nearPrefabs = 0;//周围预制体数量

                            {
                                Collider[] cols = Physics.OverlapSphere(charactor.transform.position, 5);
                                foreach (var c in cols)
                                {
                                    if (c is BoxCollider)
                                    {
                                        Vector3 sizeVolume = (c as BoxCollider).size;
                                        if (sizeVolume.x * sizeVolume.y * sizeVolume.z < 8f)
                                        {
                                            nearPrefabs++;
                                        }
                                    }
                                }
                            }
                            

                            if(Random.value > nearPrefabs * 0.25f) //周围有大于4个必然不被发现， 没有必然发现
                            {
                                StartCoHunt(charactor);
                                break;
                            }
                        }
                    }
                }
                else//远距离30m外
                {
                    if (!charactor.IsDisguised && charactor.GetComponent<Rigidbody>().velocity.sqrMagnitude > 0.25f)//未伪装并且在移动则很大概率被发现
                    {
                        if (Random.value < 0.5f)
                        {
                            StartCoHunt(charactor);
                            break;
                        }
                    }
                }

            }
        }
    }

    /// <summary>
    /// 正前方查找弹药
    /// </summary>
    private void RepeatingSearchAmmoForward()
    {
        if (modeHunter == AIModeHunter.Obtaing) return;// 不能改变捡拾目标
        if (modeHunter == AIModeHunter.Hunting) return;// 不能直接从追逐状态转换到捡拾状态

        SupplyAmmo[] ammos = Physics.OverlapBox(this.transform.position + this.transform.forward * 20f, new Vector3(20f, 20f, 20f)).Select(c => c.GetComponent<SupplyAmmo>()).Where(su => su != null).ToArray();//向前搜寻距离40m

        foreach (SupplyAmmo ammo in ammos)
        {
            StartCoObtain(ammo.gameObject);
            break;
        }
    }

    /// <summary>
    /// 卡位检测
    /// </summary>
    private void RepeatingBlockCheck()
    {
        if(moveVec.sqrMagnitude > 0.25f)
        {
            if((this.blockPos - this.transform.position).sqrMagnitude < 0.5f)
            {
                Collider[] colliders = Physics.OverlapSphere(this.transform.position, 1.5f);

                foreach(var col in colliders)
                {
                    var charaGet = col.GetComponentInParent<CharactorController>();
                    if (charaGet != null && charaGet.enabled && charaGet != charactorController)
                    {
                        StartCoHunt(charaGet);
                        break;
                    }
                }

            }

            this.blockPos = this.transform.position;
        }
    }

    /// <summary>
    /// 目的地在安全区外
    /// </summary>
    private void RepeatingValidateDst()
    {
        if((team == 0 && modeHider == AIModeHider.MoveTo) || (team == 1 && modeHunter == AIModeHunter.Normal))
        {
            if (!game.IsSafeArea(agent.destination))
            {
                Debug.Log(userName + ": 目的地现在在安全区外，重新设置目的地");
                SetRandomLocation();
            }
        }
    }

    //-----------------------------------------------------Hidder Status Coroutine-------------------------------------------------
    /// <summary>
    /// 在附近躲藏
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoHide()
    {
        //开始躲藏
        isDisguiseing = true;

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));


        //射线查找附近空位
        //Vector3 pos;
        //bool isValidPos = false;

        //do
        //{
        //    pos = UtilsGame.GetCellCenterPos(this.transform.position + new Vector3(Random.Range(-4, 5), 0f, Random.Range(-4, 5)));

        //    RaycastHit hit;

        //    //非悬空
        //    bool isOnGround = Physics.Raycast(new Ray(pos + new Vector3(0, 5f, 0), -Vector3.up), out hit, 5.5f);
        //    //空位
        //    bool isEmpty = !(Physics.Raycast(new Ray(pos + new Vector3(0, 5f, 0), -Vector3.up), out hit, 4.5f));// is onground
        //    //验证通过
        //    isValidPos = isEmpty && isOnGround;

        //    //##应该再判断是否在NavMesh上
        //}
        //while (!isValidPos);
        //agent.SetDestination(pos);



        Vector3 hidePos = UtilsGame.ScanRandomPos(game, true, 6, this.transform.position, new Vector2(this.transform.position.y - 0.5f, this.transform.position.y + 1.5f));

        if(hidePos == new Vector3())
        {
            agent.SetDestination(this.transform.position);
        }
        else
        {
            agent.SetDestination(hidePos);
        }

        yield return new WaitUntil(() => {
            Debug.DrawLine(hidePos, hidePos + new Vector3(0, 2f,0f), Color.yellow);
            Debug.DrawLine(hidePos, this.transform.position, Color.yellow);
            return (this.transform.position - hidePos).sqrMagnitude < Mathf.Pow(0.5f, 2);
        });
        
        ////default prefab//
        charactorController.SwitchDisguise();
        
        //躲藏完成
        isDisguiseing = false;
    }



    //-----------------------------------------------------Hunter Status Coroutine-------------------------------------------------

    
    /// <summary>
    /// 查找该位置附近（未优化）
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoSearch()
    {
        modeHunter = AIModeHunter.LocSearching;

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        viewDir = Random.onUnitSphere;
        
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        viewDir = Random.onUnitSphere;

        yield return new WaitForSeconds(Random.Range(0.5f, 2f));

        foreach (var charactorTarget in FindObjectsOfType<CharactorController>())
        {
            if(charactorTarget != this.GetComponent<CharactorController>() && charactorTarget.gameInput.Team == 0 && (charactorTarget.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(10f, 2) && charactorTarget.enabled == true)
            {
                //不被遮挡
                if (!NoCover(charactorTarget.GetComponentInChildren<Collider>()))
                {
                    continue;
                }

                //伪装60%几率不被发现
                if (charactorTarget.IsDisguised)
                {
                    if (Random.value < 0.6f) continue;
                }


                //设为追击目标
                StartCoHunt(charactorTarget);
                yield break;
            }
        }

        yield return new WaitForSeconds(2f);

        SetRandomLocation();
        modeHunter = AIModeHunter.Normal;
    }


    /// <summary>
    /// 追击目标(站立不动)
    /// </summary>
    /// <returns></returns>
    private void StartCoHunt(CharactorController charactor)
    {
        if (this.huntCorouting != null)
        {
            StopCoroutine(this.huntCorouting);
            huntCorouting = null;
        }
        if(this.obtainCorouting != null)
        {
            StopCoroutine(this.obtainCorouting);
            obtainCorouting = null;
        }

        this.huntCorouting = StartCoroutine(CoHunt(charactor));
    }
    private IEnumerator CoHunt(CharactorController charactor)
    {
        modeHunter = AIModeHunter.Hunting;

        huntTarget = charactor;
        Collider collider = charactor.GetComponentInChildren<Collider>();

        //-------losing-time--------
        float targetLosingTime = 0f;
        

        while(this.enabled && charactor.enabled && charactorController.AmmoCount > 0)//如果目标未死亡且子弹大于0--->每帧运行
        {
            //debug
            Debug.DrawLine(charactor.transform.position + new Vector3(0.1f, 0f, 0.1f), charactor.transform.position + new Vector3(0.3f, 10f, 0.3f), Color.red);

            //----cal distance--
            huntTargetSqrDistance = (charactor.transform.position - this.transform.position).sqrMagnitude;


            //---view and move---
            RaycastHit hit;
            if(Physics.Raycast(new Ray(UtilsGame.GetFirePos(this.transform.position), charactor.transform.position - this.transform.position), out hit, 100f) && hit.collider == collider)
            {
                //瞄准
                viewDir = Vector3.Lerp(ViewDir, (charactor.transform.position - this.transform.position).normalized + (biasUnitVec * aimBiasFac), Time.deltaTime * aimLerpSpeed).normalized;
                moveVec = new Vector2();
                
                //目标能看见-停住攻击
                SetIntelligentFire(Mathf.Clamp((2500f - huntTargetSqrDistance) / 2500f, 0.1f, 1f), Mathf.Clamp(huntTargetSqrDistance / 2500f, 0f, 1f)); //距离极限值：50m
            }
            else
            {
                //停止射击程序
                charactorController.SetConstaneFire(false);

                //如果目标丢失时间超过则放弃追踪
                if(targetLosingTime > 8f)
                {
                    yield return new WaitForSeconds(Random.Range(0.5f, 1f));//目标丢失后停顿0.5 - 1秒
                    SetRandomLocation();
                    modeHunter = AIModeHunter.Normal;
                    yield break;
                }

                //目标离开视野- 目标丢失时间不超过 -追击
                viewDir = agent.pathPending ? this.transform.forward : new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
                agent.SetDestination(charactor.transform.position);
                moveVec = new Vector2(0, 1f);

                targetLosingTime += Time.deltaTime;
            }


            yield return null;
        }

        huntTarget = null;

        yield return new WaitForSeconds(Random.Range(1f, 3f));//杀人后停顿1-3秒

        modeHunter = AIModeHunter.Normal;
    }

    /// <summary>
    /// 获取补给
    /// </summary>
    /// <param name="supply"></param>
    /// <returns></returns>
    private void StartCoObtain(GameObject supply)
    {
        if (this.huntCorouting != null)
        {
            StopCoroutine(this.huntCorouting);
            huntCorouting = null;
        }
        if (this.obtainCorouting != null)
        {
            StopCoroutine(this.obtainCorouting);
            obtainCorouting = null;
        }

        obtainCorouting = StartCoroutine(CoObtain(supply));
    }
    private IEnumerator CoObtain(GameObject supply)
    {
        modeHunter = AIModeHunter.Obtaing;

        while (supply)
        {
            viewDir = agent.pathPending ? this.transform.forward : new Vector3(agent.desiredVelocity.x, 0f, agent.desiredVelocity.z);
            agent.SetDestination(supply.transform.position);
            moveVec = new Vector2(0, 1f);

            yield return null;
        }
        

        SetRandomLocation();
        modeHunter = AIModeHunter.Normal;
    }


    //--------------------------------------------Utils-------------------------------------------------------

    /// <summary>
    /// 重新设置目的地（未判断是否搜查过）
    /// </summary>
    private void SetRandomLocation()
    {
        //查找距离5米以上而且在安全区的目标点
        NavLocation[] locations = FindObjectsOfType<NavLocation>().Where(loc => ClassicGame.Instance.IsSafeArea(loc.transform.position) && (loc.transform.position - this.transform.position).sqrMagnitude > 25f).ToArray();
        
        if(locations.Length > 0)
        {
            agent.SetDestination(locations[Random.Range(0, locations.Length)].transform.position);
        }
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



    //--------------------------------------------Other--------------------------------------------------------


    /// <summary>
    /// 死亡
    /// </summary>
    public void Disable()
    {
        this.enabled = false;
    }



    public void OnGUI()
    {

    }
}
