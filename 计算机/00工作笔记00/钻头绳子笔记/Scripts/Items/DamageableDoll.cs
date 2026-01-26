using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageableDoll : MonoBehaviour, IDamageable
{
    private CharacterRoot charaRoot;
    private DollController dollCtrl;    public DollController DollCtrl => this.dollCtrl;
    private GrabableDoll dollGrab;    public GrabableDoll DollGrab => this.dollGrab;

    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => null;

    public bool IsBroken => dollCtrl.IsDead;


    //coms
    public Collider helmet;
    public Collider armor;
    public SkinnedMeshRenderer smrHelmet;
    public SkinnedMeshRenderer smrArmor;

    //equip
    public int WeaponType => dollCtrl.weaponType;
    public bool hasHelmet;
    public bool hasArmor;
    public bool hasBar;

    //hp
    private float maxBodyHp = 100f;
    private float maxHelmetHp = 100f;
    private float maxArmorHp = 100f;
    private float bodyHp;
    private float helmetHp;
    private float armorHp;
    public float HpMultip
    {
        set
        {
            maxBodyHp *= value; maxHelmetHp *= value; maxArmorHp *= value;
            bodyHp *= value; helmetHp *= value; armorHp *= value;
        }
    }


    //self dmg
    private float currentDrillDPS = 0f;

    //status
    private bool bodyDead = false;
    private bool helmetBroken = false;
    private bool armorBroken = false;

    private bool isDamaging = false;
    private int damagingPart; // 0- body 1-helmet 2-armor






    void Start()
    {
        //coms
        charaRoot = this.GetComponentInParent<CharacterRoot>();
        dollCtrl = this.GetComponentInChildren<DollController>();
        dollGrab = this.GetComponentInChildren<GrabableDoll>();

        //init armors
        if (hasHelmet)
        {
            smrHelmet.enabled = true;
            helmet.gameObject.SetActive(true);
        }
        if (hasArmor)
        {
            smrArmor.enabled = true;
            armor.gameObject.SetActive(true);
        }

        //inti weapon
        var tagWeapon = this.GetComponentInChildren<TagWeapon>(true);
        if (tagWeapon != null)
        {
            if (hasBar)
            {
                tagWeapon.gameObject.SetActive(true);
            }
            else
            {
                tagWeapon.gameObject.SetActive(false);
            }
        }

        //hp
        bodyHp = maxBodyHp;
        helmetHp = maxHelmetHp;
        armorHp = maxArmorHp;
    }
    void Update()
    {
        if (isDamaging)
        {
            GetDamage(Time.deltaTime * 400f);  //KILL IMMEDIATLY
        }
    }




    public void DieImmidiate()
    {
        GetDamage(9999999f, null, true);
    }


    public void StartDamage(Collider collider, float dps)
    {
        dollCtrl.DollDamageStart();

        isDamaging = true;
        currentDrillDPS = dps;

        if(collider == helmet)
        {
            damagingPart = 1;
        }
        else if(collider == armor)
        {
            damagingPart = 2;
        }
        else
        {
            damagingPart = 0;
        }


        //Face
        charaRoot.FaceDamaging = true;
    }

    public void EndDamage()
    {
        dollCtrl.DollDamageEnd();

        isDamaging = false;

        //Face
        charaRoot.FaceDamaging = false;
    }

    public void GetDamage(float dmg, string info = null, bool directlyDamageBody = false)
    {
        //丢失武器
        if(dmg > 2f)
        {
            if (dollCtrl.weaponType != 0) { Debug.Log("Damage Drop. collider: " + info); }
            dollCtrl.CheckResetType();
        }

        if (directlyDamageBody)
        {
            bodyHp -= dmg;
        }
        else
        {
            switch (damagingPart)
            {
                case 0:
                    bodyHp -= dmg;
                    break;
                case 1:
                    helmetHp -= dmg;
                    break;
                case 2:
                    armorHp -= dmg;
                    break;

            }
        }

        if (bodyHp < 0f && !bodyDead) // add property isBroken
        {
            //Die();
            dollCtrl.DollDie();
            bodyDead = true;
            OnBreak();
        }
        if(helmetHp < 0f && !helmetBroken)
        {
            HelmetBreakFx();
            helmetBroken = true;
            OnArmorBreak();
        }
        if (armorHp < 0f && !armorBroken)
        {
            ArmorBreakFx();
            armorBroken = true;
            OnArmorBreak();
        }
    }

    private void HelmetBreakFx()
    {
        smrHelmet.enabled = false;
        helmet.transform.GetComponentsInChildren<Rigidbody>(true).ToList().ForEach(r => {  r.isKinematic = false; r.transform.parent = null; });
        Destroy(helmet.gameObject);
    }
    private void ArmorBreakFx()
    {
        smrArmor.enabled = false;
        armor.transform.GetComponentsInChildren<Rigidbody>(true).ToList().ForEach(r => { r.isKinematic = false; r.transform.parent = null; });
        Destroy(armor.gameObject);
    }



    public void OnBreak()
    {
        FindObjectOfType<PlayerController>().onBreakTarget.Invoke(this);

        FindObjectOfType<PlayerController>().onKill.Invoke();
    }

    public void OnArmorBreak()
    {
        FindObjectOfType<PlayerController>().onSubpartBreakTarget.Invoke(this);
    }
}
