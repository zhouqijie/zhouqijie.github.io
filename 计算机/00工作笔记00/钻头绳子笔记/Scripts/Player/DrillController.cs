using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;


public class DrillController : MonoBehaviour
{
    //coms
    public Transform drillskinHolder;
    public Transform drillroot;
    public Transform rotateTransform;
    public AudioSource audioDrill;
    private Drill drill;
    private FxDrill fxdrill;
    private UIDrill uiDrill;

    //status
    private IDamageable current; public IDamageable Current => this.current;
    private bool isRolling = false;

    //drill status
    private float dps = 50f;                public float DPS { set { this.dps = value; } }
    private float maxHeat = 100f;           public float MaxHeat { set { this.maxHeat = value; } }
    private float currentHeat = 100f;
    public float HeatPercentage => currentHeat / maxHeat;





    void Start()
    {
        drill = this.GetComponentInChildren<Drill>(false);
        fxdrill = this.GetComponentInChildren<FxDrill>();
        uiDrill = this.GetComponentInChildren<UIDrill>();

        //set volume
        audioDrill.volume = 0f;

        //startup set
        uiDrill.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if (isRolling)
        {
            currentHeat = Mathf.Clamp(currentHeat - Time.deltaTime * 50f, -0.1f, maxHeat);
            if(currentHeat < 0f)
            {
                EndDamage();
                Game.Instance.Lose(1);
            }
        }
        else
        {
            currentHeat = Mathf.Clamp(currentHeat + Time.deltaTime * 150f, 0, maxHeat);
        }
    }

    public void OnTargetBroken(IDamageable dmg)
    {
        if (isRolling && dmg == current)
        {
            EndDamage();
        }
    }

    public void StartDamage(IDamageable damageobj, Collider collider, Vector3 damagePos)
    {
        isRolling = true;


        //sound 
        audioDrill.DOKill();
        audioDrill.pitch = 0.2f;
        audioDrill.DOFade(1f * SoundPlayer.Volume, 0.25f);
        audioDrill.DOPitch(1f, 0.5f);

        //speed up
        drill.SpeedUp();

        //UI
        uiDrill.gameObject.SetActive(true);

        //fx
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ragdoll"))
        {
            fxdrill.bloodFx.gameObject.SetActive(true);
            fxdrill.bloodFx.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            fxdrill.metalFx.gameObject.SetActive(true);
            fxdrill.metalFx.GetComponent<ParticleSystem>().Play();
        }

        //rotate
        drillroot.DOLocalRotate(rotateTransform.localEulerAngles, 0.25f);

        //move drill
        drillroot.DOMove(damagePos, 0.25f);
        //var localPos = drillroot.parent.worldToLocalMatrix * damagePos;
        //var localPosFire = drillroot.parent.worldToLocalMatrix * drill.firePos.position;
        //drillroot.transform.localPosition = localPos - localPosFire;

        //OnStartDamage()
        current = damageobj;
        damageobj.StartDamage(collider, this.dps);
    }

    public void EndDamage()
    {
        if (!isRolling) return;
        isRolling = false;

        //sound 
        audioDrill.DOKill();
        audioDrill.DOFade(0f * SoundPlayer.Volume, 0.25f);
        audioDrill.DOPitch(0.2f, 0.1f);

        //speed
        drill.SpeedDown();

        //UI
        uiDrill.gameObject.SetActive(false);

        //fx
        fxdrill.bloodFx.gameObject.SetActive(false);
        fxdrill.metalFx.gameObject.SetActive(false);

        //move rotate drill
        drillroot.DOLocalMove(Vector3.zero, 0.25f);
        drillroot.DOLocalRotate(Vector3.zero, 0.25f);

        //OnEndDamage()
        current.EndDamage();
    }
    
}
