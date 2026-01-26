using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Ability
{
    public CharactorController target;

    public AbilityInfo info = null;

    public bool activeAbility = false;

    public float cd;

    public float cdRemain;

    public virtual void Init() { }

    public abstract bool Ready { get; }

    public abstract void TryUse();
}




public class AbilityTest : Ability
{
    private int teamTest;

    public AbilityTest(AbilityInfo info, int presetTeam)
    {
        this.teamTest = presetTeam;
        this.info = info;
        this.activeAbility = true;
        cd = 0f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get { return true; }
    }

    public int Team
    {
        get { return teamTest; }
    }

    public override void TryUse()
    {

        if (!Ready) return;

        Debug.Log("技能使用测试: 名称：" + info.name + "  阵营：" + Team);
        
    }
}






/// <summary>
/// 主动技能
/// </summary>
public class AbilityRun : Ability
{
    public AbilityRun(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = true;
        cd = 30f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get { return cdRemain < 0f; }
    }
    

    public override void TryUse()
    {
        if (!Ready) return;

        this.target.AddBuff(new SpeedBuff(this.target, 1.5f, 6f));

        SoundPlayer.PlaySound3D("land", this.target.transform.position);

        cdRemain = cd;
    }
}

/// <summary>
/// 主动技能
/// </summary>
public class AbilityHeal : Ability
{
    public AbilityHeal(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = true;
        cd = 30f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get { return cdRemain < 0f; }
    }


    public override void TryUse()
    {
        if (!Ready) return;

        var allies = GameObject.FindObjectsOfType<CharactorController>().Where(c => (c.transform.position - target.transform.position).sqrMagnitude < 25f);

        foreach(var ally in allies)
        {
            ally.Heal(30f);
        }

        cdRemain = cd;
    }
}



public class AbilityMask: Ability
{
    public AbilityMask(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = false;
    }

    public override bool Ready
    {
        get { return false; }
    }

    public override void Init()
    {
        target.posionMultip = 0.5f;
    }


    public override void TryUse()
    {
        if (!Ready) return;

        //do nothing

        cdRemain = cd;
    }
}


/// <summary>
/// 被动使用 主动技能
/// </summary>
public class AbilityRespawn : Ability
{
    public AbilityRespawn(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = false;
        cd = 999f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get { return cdRemain < 0f; }
    }


    public override void TryUse()
    {
        if (!Ready) return;


        var game = GameObject.FindObjectOfType<GameStarter>().GetComponent<IGame>();
        Vector3 newPosSpawn = UtilsGame.ScanRandomPos(game, true, Mathf.RoundToInt(ClassicGame.Instance.SafeRadius), ClassicGame.Instance.SafeCenter, new Vector2(0f, 6f));

        if(newPosSpawn != new Vector3())
        {
            target.transform.position = newPosSpawn;
            GameObject effect = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/DisguiseSmoke"), target.transform.position, new Quaternion(), null);
            GameObject.Destroy(effect, 3f);
            //hp
            target.Heal(100f);
        }

        cdRemain = cd;
    }
}



/// <summary>
/// 被动使用 主动技能
/// </summary>
public class AbilityJoke : Ability
{
    public AbilityJoke(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = false;
        cd = 30f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get { return cdRemain < 0f; }
    }
    

    public override void TryUse()
    {
        if (!Ready) return;


        var game = GameObject.FindObjectOfType<GameStarter>().GetComponent<IGame>();
        Vector3 newPosSpawn = UtilsGame.ScanRandomPos(game, false, 20, target.transform.position, new Vector2(0f, 6f));

        if (newPosSpawn != new Vector3())
        {
            target.transform.position = newPosSpawn + new Vector3(0f, 1f, 0f);
            GameObject effect = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/DisguiseSmoke"), target.transform.position, new Quaternion(), null);
            GameObject.Destroy(effect, 3f);
        }


        cdRemain = cd;
    }
}






/// <summary>
/// 主动技能（两段技能）
/// </summary>
public class AbilityGuard : Ability
{
    private bool guardHasSet = false;
    private GameObject guard = null;

    public AbilityGuard(AbilityInfo info)
    {
        this.info = info;
        this.activeAbility = true;
        cd = 30f;
        cdRemain = 0f;
    }

    public override bool Ready
    {
        get
        {
            if (guardHasSet)
            {
                return true;
            }
            else
            {
                return cdRemain < 0f;
            }
        }
    }

    

    public override void TryUse()
    {
        if (!Ready) return;

        if (!guardHasSet)
        {
            Vector3 pos = UtilsGame.GetCellCenterPos(target.transform.position);
            guard = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Guard"), pos, new Quaternion(), null);

            SoundPlayer.PlaySound3D("warning", guard.transform.position);

            guardHasSet = true;

            //不进入CD，等待再次使用技能
        }
        else
        {
            target.transform.position = guard.transform.position;
            
            SoundPlayer.PlaySound3D("deploy", guard.transform.position);

            GameObject.Destroy(guard);

            guardHasSet = false;

            //进入CD
            cdRemain = cd;
        }
    }
}