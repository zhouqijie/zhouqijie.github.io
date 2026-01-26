using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using RootMotion.Dynamics;


public enum DollStatus
{
    Idle,
    MoveToDest,
    Attacking,
    Hit,
    Grabing,
    Dead,
}


public class DollController : MonoBehaviour
{
    //coms
    private IDamageable damageableThis;
    private IGrabable grabableThis;


    private CharacterController characterCtrl;
    private Rigidbody charaRigid;
    private Animator animator;
    private PuppetMaster puppetMaster;
    private BehaviourPuppet bp;


    //public 
    [HideInInspector] public bool isBoss = false;
    public GameObject weaponObj;
    public Transform destination;
    public int weaponType = 0;
    public int attackType = 1;

    //runtime animator ctrlrs
    public RuntimeAnimatorController normalController;

    //attack
    [HideInInspector] public float attackSpeed = 1f;
    private float[] intervalLength = new float[3] { 0f, 3f, 3f };
    private float[] hurtLength = new float[3] { 0f, 1.35f, 1.35f };

    //status
    private bool isDead = false;
    public bool IsDead { get { return this.isDead; } }
    private bool damaging = false;
    private DollStatus dollStatus = DollStatus.Idle;
    private bool loseBalance = false;
    private float loseBalanceTime = 0f;

    //speed
    private const float maxmovespeed = 5f;
    private float targetmoveSpeed = 5f;
    private float currentmoveSpeed = 5f;

    //state tmp
    private bool destChanged = false;
    private PlayerController attackTarget = null;




    void Start()
    {
        damageableThis = this.transform.GetComponentInParent<IDamageable>();
        grabableThis = this.transform.GetComponentInParent<IGrabable>();

        characterCtrl = this.GetComponent<CharacterController>();
        charaRigid = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();
        puppetMaster = this.transform.GetComponentInParent<CharacterRoot>().GetComponentInChildren<PuppetMaster>();
        bp = this.transform.GetComponentInParent<CharacterRoot>().GetComponentInChildren<BehaviourPuppet>();

        //puppet event listen
        bp.onLoseBalance.unityEvent.AddListener(() => {
            loseBalance = true;
            if (weaponType != 0) { Debug.Log("LoseBalence Drop"); } CheckResetType();
        });
        bp.onRegainBalance.unityEvent.AddListener(() => {
            loseBalance = false;
        });

        //tmp
        attackTarget = FindObjectOfType<PlayerController>();
        
    }
    
    void Update()
    {
        //Timer
        if (!loseBalance)
        {
            loseBalanceTime = 0f;
        }
        else
        {
            loseBalanceTime += Time.deltaTime;
        }

        //Lose Balance Auto Kill
        if(loseBalanceTime > 5f)
        {
            (damageableThis as DamageableDoll).DieImmidiate();
        }


        //（Anim）移动动画
        UpdateRunningAnim();

        //Status Update
        switch (dollStatus)
        {
            case DollStatus.Idle:
                {
                    Debug.DrawRay(this.transform.position, Vector3.up, Color.gray, Time.deltaTime);
                    UpdateCheckGround();
                }
                break;
            case DollStatus.Grabing:
                {
                    Debug.DrawRay(this.transform.position, Vector3.up, Color.blue, Time.deltaTime);
                }
                break;
            case DollStatus.MoveToDest:
                {
                    Debug.DrawRay(this.transform.position, Vector3.up, Color.green, Time.deltaTime);
                    UpdateCheckGround();
                    UpdateMoveToDes();
                }
                break;
            case DollStatus.Attacking:
                {
                    Debug.DrawRay(this.transform.position, Vector3.up, Color.red, Time.deltaTime);

                    this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(attackTarget.transform.position - this.transform.position, Vector3.up), Vector3.up), Time.deltaTime * 3f);

                    UpdateCheckDistanceToDest();
                }
                break;
            case DollStatus.Hit:
                {
                    Debug.DrawRay(this.transform.position, Vector3.up, Color.yellow, Time.deltaTime);
                }
                break;
            default:
                Debug.DrawRay(this.transform.position, Vector3.up, Color.black, Time.deltaTime);
                break;
        }

    }

    private void FixedUpdate()
    {
        if(Mathf.Abs(currentmoveSpeed - targetmoveSpeed) < (5f * Time.fixedDeltaTime))
        {
            currentmoveSpeed = targetmoveSpeed;
        }
        else
        {
            currentmoveSpeed += Mathf.Sign(targetmoveSpeed - currentmoveSpeed) * 5f * Time.fixedDeltaTime;
        }
    }

    private void UpdateCheckGround()
    {
        if (characterCtrl != null && characterCtrl.isGrounded)
        {
            Debug.DrawLine(this.transform.position, this.transform.position  - new Vector3(0, 10f, 0),Color.green);
            //return;
        }

        RaycastHit hit;
        if(Physics.Raycast(new Ray(this.transform.position, -Vector3.up), out hit, 1000f))
        {
            float height = this.transform.position.y - hit.point.y;
            this.transform.Translate(new Vector3(0, -height * Time.deltaTime * 2f, 0));
        }
    }


    private void UpdateCheckDistanceToDest()
    {
        if (weaponType == 1) return;

        if ((destination.transform.position - this.transform.position).sqrMagnitude > 0.02f)
        {
            Transition(DollStatus.MoveToDest);
        }
    }

    private void UpdateMoveToDes()
    {
        if (weaponType == 1) return; 

        if (bp.state == BehaviourPuppet.State.GetUp || bp.state == BehaviourPuppet.State.Unpinned) return;

        if (destination != null)
        {
            Vector3 dir = destination.position - this.transform.position;
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(dir, Vector3.up), Vector3.up), Time.deltaTime * 3f);
            characterCtrl.SimpleMove(currentmoveSpeed * dir.normalized);
            //Vector3 v = 3f * dir.normalized;
            //charaRigid.velocity = new Vector3(v.x, charaRigid.velocity.y, v.z);

            if((this.transform.position - destination.transform.position).sqrMagnitude < 0.01f)
            {
                StartToAttack();
            }
        }
    }




    //------------------------------------------------Action  / Type-----------------------------------------------------


    public void StartAction()
    {
        if (dollStatus != DollStatus.Idle) return;


        if(weaponType == 0)
        {
            Transition(DollStatus.MoveToDest);
        }
        else
        {
            StartToAttack();
        }
    }

    public void CheckResetType()
    {
        if (weaponType == 0) return;

        //detach weapons
        if(weaponObj != null)
        {
            weaponObj.transform.parent = null;
            weaponObj.GetComponent<Rigidbody>().isKinematic = false;
        }

        //stop attack
        //if (dollStatus == DollStatus.Attacking) stopAttack = true; // ??

        //weapon type
        weaponType = 0;
        this.animator.runtimeAnimatorController = normalController;

        //isKinematic   (auto kinematic if has behaviour puppet)
        puppetMaster.mode = PuppetMaster.Mode.Active;
    }

    public void EnterGrab()
    {
        Transition(DollStatus.Grabing);
    }
    public void CancelGrab()
    {
        if(dollStatus == DollStatus.Grabing)
        {
            Transition(DollStatus.MoveToDest);
        }
    }
    public void TransToIdle()
    {
        Transition(DollStatus.Idle);
    }

    public void DollDamageStart()
    {
        damaging = true;

        if (dollStatus != DollStatus.Grabing)
        {
            Transition(DollStatus.Hit);
        }

    }
    public void DollDamageEnd()
    {
        damaging = false;

        if (dollStatus != DollStatus.Grabing && dollStatus != DollStatus.Dead) // ??? If Die Do Nothing
        {
            Transition(DollStatus.Attacking);
        }
    }


    public void DollDie()
    {
        Debug.Log("Doll Die");
        isDead = true;

        if (dollStatus == DollStatus.Dead) return;

        Destroy(characterCtrl);

        Destroy(damageableThis as DamageableDoll);

        DieChangeColor();

        Transition(DollStatus.Dead);
    }

    //--------------------------------------------Attack-----------------------------------------------------
    private bool stopAttack = false;
    public void StartToAttack()
    {
        Transition(DollStatus.Attacking);

        //StartCoroutine(CoAttack(attackTarget));  // move to enter state
    }
    private IEnumerator CoAttack(PlayerController player)
    {
        float timer;// = intervalLength[this.attackType] + 0.2f;

        switch(weaponType)
        {
            case 0:
                {
                    float timeTotal = intervalLength[this.attackType] * (1f / attackSpeed);
                    float timeHurt = (intervalLength[this.attackType] - hurtLength[this.attackType]) * (1f / attackSpeed);

                    timer = timeTotal;

                    animator.SetFloat("AttackSpeed", attackSpeed);

                    while (!stopAttack)
                    {
                        animator.SetTrigger("PerformAttack");

                        bool isHit = false;

                        yield return new WaitUntil(() => {
                            if (stopAttack)
                            {
                                return true;
                            }

                            timer -= Time.deltaTime;

                            if(!isHit && timer < timeHurt)
                            {
                                //玩家被击中
                                isHit = true;
                                player.Damage(100f);

                            }

                            if (timer < 0f)
                            {
                                timer = timeTotal;

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        });
                    }
                    
                }
                break;
            case 1:
                {
                    timer = 3f;

                    while (!stopAttack)
                    {
                        bool launched = false;

                        animator.SetTrigger("ShootAttack");

                        yield return new WaitUntil(() =>
                        {
                            if (stopAttack)
                            {
                                return true;
                            }

                            timer -= Time.deltaTime;

                            if(timer < 2.2f && !launched)
                            {
                                ////发射火箭弹
                                GameObject rkt = Instantiate(Resources.Load<GameObject>("Prefabs/Rocket"), weaponObj.transform.position, weaponObj.transform.rotation, null);
                                Vector3 fireDir = (attackTarget.Cam.transform.position - weaponObj.transform.position + new Vector3(0, -0.5f, 0)).normalized;
                                //Vector3 fireDir = (attackTarget.transform.position - this.transform.position).normalized;
                                rkt.transform.rotation = Quaternion.LookRotation(fireDir, Vector3.up);
                                rkt.GetComponent<Rigidbody>().AddForce(fireDir * 4f, ForceMode.VelocityChange);

                                launched = true;
                            }

                            if (timer < 0f)
                            {
                                timer = 3f;
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        });
                    }
                    
                }
                break;
        }

        
    }


    //------------------------------------------------Transition---------------------------------------------------
    private void Transition(DollStatus s)
    {
        OnExitStatus(this.dollStatus);
        this.dollStatus = s;
        OnEnterStatus(this.dollStatus);
    }
    private void OnExitStatus(DollStatus s)
    {
        switch (s)
        {
            case DollStatus.Idle:
                break;
            case DollStatus.MoveToDest:
                break;
            case DollStatus.Attacking:
                {
                    stopAttack = true;
                }
                break;
            case DollStatus.Grabing:
                {
                    animator.SetBool("Grab", false);


                    puppetMaster.mappingWeight = 1f;
                    bp.enabled = true;

                    characterCtrl.enabled = true;
                }
                break;
            case DollStatus.Hit:
                {
                    animator.SetBool("GetDamage", false);
                }
                break;
            default:
                break;
        }
    }
    private void OnEnterStatus(DollStatus s)
    {
        switch (s)
        {
            case DollStatus.Idle:
                break;
            case DollStatus.MoveToDest:
                {
                    currentmoveSpeed = 0f;
                    targetmoveSpeed = maxmovespeed;
                    CheckDestination();
                }
                break;
            case DollStatus.Attacking:
                {
                    characterCtrl.SimpleMove(Vector3.zero);

                    stopAttack = false;

                    StartCoroutine(CoAttack(attackTarget));
                }
                break;
            case DollStatus.Hit:
                {
                    animator.SetBool("GetDamage", true);
                    animator.SetTrigger("StartGetDamage");
                }
                break;
            case DollStatus.Grabing:
                {
                    animator.SetBool("Grab", true);


                    puppetMaster.mappingWeight = 0f;
                    bp.enabled = false;

                    characterCtrl.enabled = false;
                }
                break;
            case DollStatus.Dead:
                {
                    bp.enabled = false;
                    puppetMaster.state = PuppetMaster.State.Dead;
                    puppetMaster.mappingWeight = 1f;
                    puppetMaster.pinWeight = 0f;
                    puppetMaster.muscleWeight = 0f;
                }
                break;
            default:
                break;
        }
    }

    //------------------------------------------------Anim-----------------------------------------------------------
    private void UpdateRunningAnim()
    {
        if (characterCtrl != null && dollStatus == DollStatus.MoveToDest && currentmoveSpeed > 0.5f)
        {
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
    }

    //--------------------------------------------------Effects-----------------------------------------------------

    private void DieChangeColor()
    {
        foreach(var mat in this.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            mat.material.color = Color.gray;
            mat.material.SetColor("_EmissionColor", Color.black);
        }
    }

    //------------------------------------------------Dest-----------------------------------------------------------

    public void CheckDestination()
    {
        //check distance
        Debug.Log("Reset Dest1: " + this.transform.root.gameObject.name);
        if (!destChanged && (attackTarget.transform.position - this.destination.position).sqrMagnitude > Mathf.Pow(3.5f, 2f))
        {
            destChanged = true;

            //change dest
            TagDest[] dests = FindObjectsOfType<CharacterRoot>().Select(cr => cr.GetComponentInChildren<TagDest>()).OrderBy(x => (x.transform.position - attackTarget.transform.position).sqrMagnitude).ToArray();
            this.destination.position = dests[0].transform.position;
        }

        //check view in view
        float signAngle = Vector3.SignedAngle(attackTarget.CurrentWaypoint.transform.forward, destination.transform.position - attackTarget.CurrentWaypoint.transform.position, Vector3.up);
        Debug.Log("Reset Dest2: old angle:" + signAngle.ToString("f2"));
        if (signAngle < -26f)
        {
            destination.transform.RotateAround(attackTarget.CurrentWaypoint.transform.position, Vector3.up, (-26f - signAngle));
        }
        if(signAngle > 26f)
        {
            destination.transform.RotateAround(attackTarget.CurrentWaypoint.transform.position, Vector3.up, (26f - signAngle));
        }
    }


    //---------------------------------------------------Debug---------------------------------------------------------
   
}
