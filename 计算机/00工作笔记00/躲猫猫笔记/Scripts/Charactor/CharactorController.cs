
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

using DG.Tweening;

public class CharactorController : MonoBehaviour
{
    //Game
    private IGame game;

    //COM
    [HideInInspector] public IGameInput gameInput;
    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rigid;

    private AudioSource auFoot;


    private GameObject charactor;
    private GameObject disguised;

    [HideInInspector] public SkinnedMeshRenderer targetSmr;

    public Transform headHold;
    public Transform backHold;
    public Transform weaponHold;

    //Info
    [HideInInspector] public CharactorInstance charaInstance;


    ///----------------------------Talents----------------------------

    private float attackTalentMultip;
    private float attackStayMultip = 1f;
    private float hpTalentMultip;
    private float speedTalentMultip;


    private List<TalentNodeInfo> talents;
    private void ReCalAddTalents()
    {
        attackTalentMultip = 1f;
        hpTalentMultip = 1f;
        speedTalentMultip = 1f;
        //Cal Properties
        UtilsGame.CalPropertyAdd(this.talents, ref attackTalentMultip, ref hpTalentMultip, ref speedTalentMultip);
        float ammoMultip = HasTalent(9) ? 1.2f : 1f;


        //---HP----
        this.maxhp = maxhpBase * hpTalentMultip;
        //---AMMO----
        this.maxAmmo = Mathf.RoundToInt(maxAmmoBase * ammoMultip);

    }
    private void ReSetStatusTalent()
    {
        //探测器状态
        if(gameInput is PlayerInput)
        {
            if (!HasTalent(11) && !HasTalent(23))
            {
                FindObjectOfType<UICanvas>().GetComponentInChildren<UIScaner>(true).gameObject.SetActive(false);
            }
            else
            {
                FindObjectOfType<UICanvas>().GetComponentInChildren<UIScaner>(true).gameObject.SetActive(true);
            }
        }


        //脚步声
        this.hasFootSound = !(HasTalent(6) || HasTalent(18));
    }
    public void SetTalents(List<TalentNodeInfo> talentNodes)
    {
        this.talents = talentNodes;

        //ReCal and ReSet
        ReCalAddTalents();
        ReSetStatusTalent();

        //---ABILITIES---(Usually set in ReCalTalent)
        if (HasTalent(20))
        {
            Ability ab = new AbilityJoke(null);
            AddAbility(ab);
        }


        if(HasTalent(4))//team 1
        {
            AddAbility(Utils.GetAbility(charaInstance, 1));
        }

        if (HasTalent(16))//team 0
        {
            AddAbility(Utils.GetAbility(charaInstance, 0));
        }
    }
    public void AddTalent(TalentNodeInfo talent)
    {
        this.talents.Add(talent);

        if (talent.group == "add")
        {
            ReCalAddTalents();
        }

        if(talent.group == "status")
        {
            ReSetStatusTalent();
        }
    }
    public bool HasTalent(int id)
    {
        if (!(talents != null)) return false;

        return talents.Any(t => t.id == id);
    }
    ///-------------------------- BUFF --------------------------------

    private float speedBuffMultip = 1f;

    private List<Buff> buffs = new List<Buff>();
    private void ReCalBuffs()
    {
        //speed.....
        List<SpeedBuff> speedBuffs = buffs.Where(b => b is SpeedBuff).Select(b => b as SpeedBuff).ToList();
        speedBuffMultip = 1f;
        foreach(var buff in speedBuffs)
        {
            speedBuffMultip *= buff.multip;
        }

        //attack
        //...

        //...
    }
    private void UpdateBuffs()
    {
        for(int i = buffs.Count - 1; i > -1; i--)
        {
            buffs[i].BuffUpdate();
            buffs[i].time -= Time.deltaTime;
            if (buffs[i].time < 0f)
            {
                buffs[i].BuffDel();
                buffs.Remove(buffs[i]);
                ReCalBuffs();
            }
        }
    }
    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);

        ReCalBuffs();

        buff.BuffStart();
    }



    ///------------------------技能-----------------------------------

    private List<Ability> abilities = new List<Ability>();

    public bool HasAbility<T>() where T : Ability
    {
        foreach(Ability ab in abilities)
        {
            if (ab is T) return true;
        }

        return false;
    }
    public Ability GetAbility<T>() where T : Ability
    {
        foreach (Ability ab in abilities)
        {
            if (ab is T) return ab;
        }

        return null;
    }
    public void AddAbility(Ability ab)
    {
        abilities.Add(ab);
        ab.target = this;

        ab.Init();
    }
    public void UpdateAbilities()
    {
        foreach(Ability ab in abilities)
        {
            ab.cdRemain -= Time.deltaTime;
        }
    }


    

    ///-----------------------------------------------------------------


    //Input
    private Vector2 moveVec;
    private Vector3 viewDir;



    //weapon / ammo
    private GameObject weapon = null;
    private bool isReloading = false;
    private const int maxAmmoBase = 100;
    private int maxAmmo = 100;
    private int ammo;
    private int ammoMag = 30;
    public int AmmoCount { get { return (this.ammo + this.ammoMag); } }
    public string AmmoCountStr { get { return ammoMag + "/" + ammo; } }



    //main status
    [HideInInspector] public bool freezed = false;
    [HideInInspector] public bool aiInvisiable = false;
    private bool dead = false;
    private const float maxhpBase = 100f;
    private float maxhp = 100f;
    private float hp = 100f;
    public float Hp { get { return this.hp; } }

    //other status
    private float currentSpeed;
    private float movespeedBase = 2.5f;
    private bool isGround = false;   public bool IsGround { get { return this.isGround; } }
    private bool isJumped = false;
    private float targetCAngle;
    private float currentCAngle;
    private float targetCAnlgePitch;
    private float currentCAnlgePitch;

    private float stayTime = 0f;
    private bool isStay = false;

    private float injuryingTime = 0f;
    private float speedInjuryMultip = 1f;

    private bool inPoison = false;
    [HideInInspector] public float posionMultip = 1f;
    private float speedPoisonMultip = 1f;

    // underAttack/chase target
    private CharactorController offender1 = null;
    private CharactorController chaseTarget = null;  public CharactorController ChaseTarget { get { return chaseTarget; } }
    private float speedChaseMultip = 1f;


    //ad speed up
    private float speedAdMultip = 1f;

    //fire stuff
    private bool isFiring = false;
    private bool constantFiring = false;  public bool ConstantFiring { get { return this.constantFiring; } }


    //disguise stuff
    private bool isDisguised = false;
    public bool IsDisguised { get { return this.isDisguised; } }


    //-------------------temp status----------------------------
    private bool hasFootSound = true;





    //-------------------events--------------------------------
    public class DamageEvent<CharactorController> : UnityEvent<CharactorController>
    {

    }
    private DamageEvent<CharactorController> onAttacked = new DamageEvent<CharactorController>();




    //-------------------Statistics------------------------------
    private int kills = 0;
    public int Kills
    {
        get
        { return this.kills; }
        set
        {
            if (value > this.kills && HasTalent(10)) this.ammo += Mathf.RoundToInt(this.maxAmmo * 0.2f);
            this.kills = value;
        }
    }




    void Start()
    {
        game = FindObjectOfType<GameStarter>().GetComponent<IGame>();

        gameInput = this.GetComponent<IGameInput>();
        animator = this.GetComponentInChildren<Animator>(true);
        agent = this.GetComponent<NavMeshAgent>();
        rigid = this.GetComponent<Rigidbody>();

        auFoot = this.GetComponentsInChildren<AudioSource>().FirstOrDefault(a => a.name == "foot");

        charactor = animator.gameObject;
        disguised = this.transform.Find("Disguised").gameObject;

        weapon = this.GetComponentInChildren<Weapon>(true).gameObject;

        //WeaponType_int
        ResetAniamtorWeaponStatus();


        //hp ammo
        this.hp = this.maxhp;
        this.ammo = this.maxAmmo;

        //events
        if(gameInput is AIInput1)
        {
            onAttacked.AddListener((attacker) => {
                (gameInput as AIInput1).UnderAttack(attacker);
            });
        }
        
        //invoke
        InvokeRepeating("RepeatCheckStatus", Random.Range(0.5f, 1.5f), 1f);
        InvokeRepeating("RepeatCheckChase", Random.Range(0.5f, 1.5f), 1f);
    }

    
    
    void Update()
    {
        //Input
        moveVec = gameInput.MoveVec;
        viewDir = gameInput.ViewDir;
        



        //---On Ground---
        isGround = false;
        RaycastHit hit;
        if (Physics.SphereCast(this.transform.position + new Vector3(0, 1f, 0), 0.295f, -this.transform.up, out hit, 0.99f))
        {
            isGround = true;
        }


        //----Stay Time(静止时长)---
        if(Mathf.Abs(moveVec.x) < 0.001f && Mathf.Abs(moveVec.y) < 0.001f)//静止中
        {
            stayTime += Time.deltaTime;
        }
        else//移动中
        {
            stayTime = 0f;
            isStay = false;
            attackStayMultip = 1f;
        }
        if(stayTime > 3f)
        {
            //##天赋22自动回血##
            if (Mathf.FloorToInt(stayTime) != Mathf.FloorToInt(stayTime - Time.deltaTime) && HasTalent(22))
            {
                this.hp = Mathf.Clamp(this.hp + 2f, 0f, this.maxhp);
            }

            
            if (!isStay)
            {
                isStay = true;
                //##TALENT##
                if (HasTalent(3))
                {
                    attackStayMultip = 1.5f;
                }
            }
        }


        //---Injurying Time---
        injuryingTime -= Time.deltaTime;
        
        speedInjuryMultip = 1f;
        if (injuryingTime < 0f)
        {
            //##TALENT##
            if (HasTalent(13))
            {
                speedInjuryMultip = 1.5f;
            }
        }



        //---Rotation---
        if (!freezed && !isDisguised) this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(new Vector3(viewDir.x, 0, viewDir.z), Vector3.up), 0.15f);

        //---Move And Speed---
        float finalSpeed = movespeedBase;
        finalSpeed *= speedTalentMultip;
        finalSpeed *= speedChaseMultip;
        finalSpeed *= speedInjuryMultip;
        finalSpeed *= speedPoisonMultip;
        finalSpeed *= speedBuffMultip;
        finalSpeed *= speedAdMultip;

        Vector3 trueMoveVec = (moveVec.x * this.transform.right + moveVec.y * this.transform.forward) * finalSpeed;

        if (!freezed && !isDisguised) rigid.velocity = new Vector3(trueMoveVec.x, rigid.velocity.y, trueMoveVec.z);//apply speed
        
        agent.speed = finalSpeed;//agent speed apply


        //---AutoJump---
        if (isGround && moveVec.y > 0.9f && !Physics.Raycast(this.transform.position + new Vector3(0, 1.4f, 0), this.transform.forward, 0.6f))
        {
            if(Physics.Raycast(this.transform.position + new Vector3(0, 0.25f, 0), this.transform.forward, 0.6f) || Physics.Raycast(this.transform.position + new Vector3(0, 0.75f, 0), this.transform.forward, 0.6f))
            {
                StartCoroutine(Jump());
            }
        }


        //---Jump---
        if (!freezed && gameInput.Jump && isGround)
        {
            StartCoroutine(Jump());
        }



        //---constant firing---
        if (constantFiring)
        {
            StartCoroutine(CoFire());
        }




        //---(Animation / Sound)Speed---
        currentSpeed = 0f;
        if (!freezed)
        {
            currentSpeed = trueMoveVec.magnitude;
        }


        //---(sound)-----
        if (hasFootSound)
        {
            auFoot.volume = SoundPlayer.Volume * (currentSpeed / 3f);
        }
        else
        {
            auFoot.volume = 0;
        }

        //---(Animation)Charactor angle---
        targetCAngle = 0;
        if (!freezed)
        {
            if (currentSpeed > 0.01f)
            {
                targetCAngle = Vector3.SignedAngle(new Vector3(trueMoveVec.x, 0f, trueMoveVec.z), this.transform.forward, this.transform.up);
            }
            if (targetCAngle < -90f) targetCAngle += 180f;
            if (targetCAngle > 90f) targetCAngle -= 180f;
        }
        currentCAngle = Mathf.Lerp(currentCAngle, targetCAngle, Time.deltaTime * 10f);

        targetCAnlgePitch = -Mathf.Clamp(Vector3.Angle(Vector3.up, viewDir), 70f, 110f) + 90f;
        currentCAnlgePitch = Mathf.Lerp(currentCAnlgePitch, targetCAnlgePitch, Time.deltaTime * 10f);

        //----------------Animation----------------
        if (!isDisguised)
        {
            animator.SetFloat("Speed_f", currentSpeed / 3f);


            if (isGround && !animator.GetBool("Grounded"))
            {
                animator.SetBool("Grounded", true);
            }
            if (!isGround && animator.GetBool("Grounded"))
            {
                animator.SetBool("Grounded", false);
            }

            charactor.transform.localEulerAngles = new Vector3(0, -currentCAngle, 0);

            switch (animator.GetInteger("WeaponType_int"))
            {
                case 0:
                    animator.SetFloat("Body_Vertical_f", (currentCAnlgePitch / 90f));
                    break;
                default:
                    animator.SetFloat("Body_Vertical_f", (currentCAnlgePitch / 90f));
                    animator.SetFloat("Body_Horizontal_f", 0.6f + (currentCAngle / 90f));
                    break;
            }

        }



        //--------------Buff Update------------------
        UpdateBuffs();

        //--------------Ability Update---------------
        UpdateAbilities();
    }

    private void FixedUpdate()
    {
    }


    /// <summary>
    /// 跳跃
    /// </summary>
    /// <returns></returns>
    private IEnumerator Jump()
    {
        if (isJumped == true) yield break;

        //jump
        rigid.velocity = new Vector3(rigid.velocity.x, 4.5f, rigid.velocity.z);

        //sourd
        SoundPlayer.PlaySound3D("jump", this.transform.position);

        isJumped = true;

        yield return new WaitForSeconds(1f);

        isJumped = false;
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    public Ability GetActiveAbility()
    {
        return abilities.FirstOrDefault(a => a.activeAbility);
    }
    public void UseAbility()
    {
        abilities.FirstOrDefault(a => a.activeAbility)?.TryUse();
    }

    /// <summary>
    /// 持续开火
    /// </summary>
    /// <param name="enableOrDisable"></param>
    public void SetConstaneFire(bool enableOrDisable)
    {
        constantFiring = enableOrDisable;
    }

    /// <summary>
    /// 开火
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoFire()
    {
        if (isFiring) yield break;
        
        //---
        isFiring = true;


        //---
        if (ammoMag < 1)
        {
            if(ammo < 1)
            {
                SoundPlayer.PlaySound3D("dry-fire", this.transform.position);

                yield return new WaitForSeconds(0.5f);

                isFiring = false;

                yield break;
            }

            if (!isReloading)//isReloading
            {
                StartCoroutine(CoReload());
                isFiring = false;
                yield break;
            }

            //reloading..
            isFiring = false;
            yield break;
        }
        ammoMag--;
        //player?
        if (gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.RefreshAmmo();
        }

        //sound
        SoundPlayer.PlaySound3D("fire2", this.transform.position);

        //---
        GameObject gunfire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/GunFire"), weapon.transform.GetChild(0).position, Quaternion.LookRotation(weapon.transform.forward), weapon.transform);
        Destroy(gunfire, 3f);


        //ray cast
        //Physics.SyncTransforms();
        yield return new WaitForFixedUpdate();
        RaycastHit hit;
        if(Physics.Raycast(new Ray(UtilsGame.GetFirePos(this.transform.position), viewDir.normalized + Random.insideUnitSphere * 0.02f), out hit, 1000f, (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Charactor") | 1 << LayerMask.NameToLayer("Player"))))//, (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Charactor") | 1 << LayerMask.NameToLayer("Player"))
        {

            //damage charactor
            CharactorController charactorhit = hit.collider.GetComponentInParent<CharactorController>();
            

            if (charactorhit != null)
            {

                //伤害计算
                float dmg = 10f;
                dmg *= attackTalentMultip;
                dmg *= attackStayMultip;

                //造成伤害
                charactorhit.Damage(dmg, this);

                //add buff
                if (HasTalent(8))
                {
                    charactorhit.AddBuff(new FootmarkBuff(charactorhit, 10f));
                }



                if (!charactorhit.IsDisguised)
                {
                    //音效
                    SoundPlayer.PlaySound3D(SoundPlayer.collectionManHit[Random.Range(0, SoundPlayer.collectionManHit.Length)], hit.point);

                    //hit effect
                    GameObject hiteffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HitEffectBody"), hit.point, Quaternion.LookRotation(-viewDir), null);
                    Destroy(hiteffect, 3f);
                }
                else
                {
                    //hit effect
                    GameObject hiteffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HitEffect"), hit.point, Quaternion.LookRotation(-viewDir), null);
                    Destroy(hiteffect, 3f);
                }
            }
            else
            {
                //音效
                SoundPlayer.PlaySound3D(SoundPlayer.collectionConcrate[Random.Range(0, SoundPlayer.collectionConcrate.Length)], hit.point);

                //hit effect
                GameObject hiteffect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/HitEffect"), hit.point, Quaternion.LookRotation(-viewDir), null);
                Destroy(hiteffect, 3f);
            }

        }


        yield return new WaitForFixedUpdate();

        gunfire.GetComponent<Light>().enabled = false;

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();


        isFiring = false;
        //---
    }

    public IEnumerator CoReload()
    {
        isReloading = true;

        //player? uiAnim
        if (gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.ShowReloadImage(1.5f);
        }

        //reload aim
        animator.SetBool("Reload_b", true);

        //...
        SoundPlayer.PlaySound3D("magout", weapon.transform.position);

        yield return new WaitForSeconds(0.75f);
        
        SoundPlayer.PlaySound3D("magin", weapon.transform.position);

        yield return new WaitForSeconds(0.75f);

        SoundPlayer.PlaySound3D("pump", weapon.transform.position);

        if (ammo > 30)
        {
            ammoMag = 30;
            ammo -= 30;
        }
        else
        {
            ammoMag = ammo;
            ammo = 0;
        }

        //player? refresh ammo display
        if (gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.RefreshAmmo();
        }

        //end anim
        animator.SetBool("Reload_b", false);

        isReloading = false;
    }

    /// <summary>
    /// 伪装
    /// </summary>
    public void SwitchDisguise(string prefabName = null)
    {
        if (!isDisguised)
        {
            //伪装效果
            GameObject effect = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/DisguiseSmoke"), this.transform.position, Quaternion.LookRotation(Vector3.up), null);
            Destroy(effect, 5f);

            rigid.isKinematic = true;
            this.transform.position = UtilsGame.GetCellCenterPos(this.transform.position);
            this.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            disguised.SetActive(true);

            {
                if (disguised.transform.childCount > 0)
                {
                    Destroy(disguised.transform.GetChild(0).gameObject);
                }
                string disguiseName = prefabName != null ? prefabName : Infomanager.GetInstance().disguiseObjInfos[Random.Range(0, Infomanager.GetInstance().disguiseObjInfos.Length)].name;
                GameObject disguiseObj = Instantiate(Resources.Load<GameObject>("Prefabs/Disguises/" + disguiseName), disguised.transform);
                disguiseObj.transform.localScale = Vector3.one;
                disguiseObj.transform.localPosition = new Vector3();
                disguiseObj.transform.localRotation = new Quaternion();

                var collider = disguiseObj.GetComponentInChildren<Collider>();
                if(collider != null) collider.enabled = false;
            }

            charactor.SetActive(false);
            isDisguised = true;
        }
        else
        {
            rigid.isKinematic = false;
            disguised.SetActive(false);
            charactor.SetActive(true);

            ResetAniamtorWeaponStatus();

            isDisguised = false;
        }
    }
    private void ResetAniamtorWeaponStatus()
    {
        if (weapon != null && weapon.activeInHierarchy)
        {
            animator.SetInteger("WeaponType_int", 2);
        }
        else
        {
            animator.SetInteger("WeaponType_int", 0);
        }
    }


    /// <summary>
    /// 获取物资（弹药）
    /// </summary>
    /// <param name="count"></param>
    public void GetAmmo(int count)
    {
        SoundPlayer.PlaySound3D("pickup", this.transform.position);

        this.ammo = Mathf.Clamp(this.ammo + count, 0, this.maxAmmo);

        if(gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.RefreshAmmo();
        }
    }






    //毒圈/水下状态检测
    public void RepeatCheckStatus()
    {
        if(!game.IsSafeArea(this.transform.position))
        {
            if(!inPoison && gameInput is PlayerInput)
            {
                (gameInput as PlayerInput).uicanvas.SetPositionEffect(true);
            }

            inPoison = true;
            speedPoisonMultip = HasTalent(15) ? 1.2f : 1f;

            Damage(5f * this.posionMultip, null);
        }
        else
        {
            if (inPoison && gameInput is PlayerInput)
            {
                (gameInput as PlayerInput).uicanvas.SetPositionEffect(false);
            }

            inPoison = false;
            speedPoisonMultip = 1f;
        }

        if (this.transform.position.y < -1.1f)
        {
            Damage(5f, null);
        }
    }

    //目标追逐检测
    public void RepeatCheckChase()
    {
        chaseTarget = null;
        foreach (CharactorController chara in FindObjectsOfType<CharactorController>().Where(c => c.IsDisguised == false && c.gameInput.Team != this.gameInput.Team))
        {
            Vector3 dir = (chara.transform.position - this.transform.position);
            if (dir.sqrMagnitude < 900f)//30米追踪距离
            {
                if (Vector3.Angle(dir, this.transform.forward) < 45f)
                {
                    chaseTarget = chara;
                    break;
                }
            }
        }

        //chase speed fac
        if (chaseTarget != null && HasTalent(1))
        {
            speedChaseMultip = 2f;
        }
        else
        {
            speedChaseMultip = 1f;
        }    
    }

    //加血
    public void Heal(float healAmount)
    {
        //heal
        this.hp = Mathf.Clamp(this.hp + healAmount, 0, this.maxhp);
        
        //player?
        if (gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.RefreshHpBar(this.hp / this.maxhp);
        }
    }

    //收到伤害
    public void Damage(float dmg, CharactorController shooter)
    {
        //---avoid---
        if (Random.value < 0.05f && HasTalent(19))
        {
            return;
        }

        //---joke---
        if (this.hp - dmg < 0 && HasAbility<AbilityJoke>())
        {
            Ability abJoke = GetAbility<AbilityJoke>();

            if (abJoke.Ready)
            {
                abJoke.TryUse();
                return;
            }

        }

        //---respawn---
        if(this.hp - dmg < 0 && HasAbility<AbilityRespawn>() && GetAbility<AbilityRespawn>().Ready)
        {
            GetAbility<AbilityRespawn>().TryUse();
            return;
        }


        //---damage---
        this.hp -= dmg;
        
        this.injuryingTime = 3f;

        //---shooter---
        if (shooter != null)
        {
            this.offender1 = shooter;
            this.onAttacked.Invoke(shooter);

            if(shooter.gameInput is PlayerInput)
            {
                (shooter.gameInput as PlayerInput).uicanvas.ShowHit();
            }
        }


        //player?
        if (gameInput is PlayerInput)
        {
            (gameInput as PlayerInput).uicanvas.RefreshHpBar(this.hp / this.maxhp);
        }


        //DEATH
        if (hp < 0 && !dead)//死亡程序
        {
            DieProgram();
        }
    }

    //广告加速
    public void AdSpeedUp()
    {
        speedAdMultip = 1.5f;
        Invoke("AdSpeedDown", 120f);
    }
    private void AdSpeedDown()
    {
        speedAdMultip = 1f;
    }

    /// <summary>
    /// 半透明隐身
    /// </summary>
    private bool forceFade = false;
    public void AdSkinFade()
    {
        SkinFade();

        forceFade = true;
        
        Invoke("AdSkinFadeOut", 120f);
    }
    private void AdSkinFadeOut()
    {
        this.targetSmr.material.DOFade(1f, 3f);

        if (weapon != null)
        {
            foreach (var mr in weapon.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.DOFade(1f, 3f);
            }
        }
    }
    public void SkinFade()
    {
        this.targetSmr.material.DOFade(0.1f, 0.5f);

        if (weapon != null)
        {
            foreach (var mr in weapon.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.DOFade(0.1f, 0.5f);
            }
        }
    }

    public void SkinFadeOut()
    {
        if (forceFade) return;

        this.targetSmr.material.DOFade(1f, 3f);

        if (weapon != null)
        {
            foreach (var mr in weapon.GetComponentsInChildren<MeshRenderer>())
            {
                mr.material.DOFade(1f, 3f);
            }
        }
    }

    //死亡过程
    private void DieProgram()
    {
        if (isDisguised) SwitchDisguise();
        
        rigid.velocity = new Vector3();
        rigid.constraints = (RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ);

        auFoot.volume = 0;

        animator.SetBool("Grounded", true);//死亡动画
        animator.SetBool("Death_b", true);//死亡动画
        this.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Dead");
        this.dead = true;
        this.enabled = false;
        this.gameInput.Disable();

        //agent.enabled = false;
        

        DieBoardcast();
    }

    /// <summary>
    /// 死亡广播
    /// </summary>
    public void DieBoardcast()
    {
        //击杀者击杀数++
        if(offender1 != null)
        {
            offender1.Kills += 1;
        }
        
        //（玩家）
        if(gameInput is PlayerInput)
        {
            game.PlayerDie();
        }

        //UI显示击杀信息
        UICanvas canvas = FindObjectOfType<UICanvas>();
        canvas.ShowKillInfo(gameInput.UserName);

        //刷新计分板
        canvas.RefreshStatistics();

        //游戏胜负判断
        game.Judge();
    }


    public void OnGUI()
    {
    }
}
