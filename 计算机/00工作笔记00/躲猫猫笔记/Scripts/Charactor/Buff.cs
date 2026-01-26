using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Buff
{
    public string name;

    public float time;

    public CharactorController buffTarget;


    public abstract void BuffStart();

    public abstract void BuffUpdate();

    public abstract void BuffDel();
}

public class FootmarkBuff : Buff
{
    private GameObject effect;


    public FootmarkBuff(CharactorController target, float time)
    {
        this.name = "足迹";

        this.time = time;

        this.buffTarget = target;
    }

    public override void BuffStart()
    {
        effect = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Footmark"), buffTarget.transform);
    }

    public override void BuffUpdate()
    {

    }
    public override void BuffDel()
    {
        GameObject.Destroy(effect);
    }
}


public class SpeedBuff: Buff
{
    public float multip;


    public SpeedBuff(CharactorController target, float multp, float time)
    {
        this.name = "移动速度";

        this.multip = multp;

        this.time = time;

        this.buffTarget = target;
    }

    public override void BuffStart()
    {

    }

    public override void BuffUpdate()
    {

    }
    public override void BuffDel()
    {

    }
}