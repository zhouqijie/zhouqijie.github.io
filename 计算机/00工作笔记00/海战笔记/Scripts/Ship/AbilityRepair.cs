using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Ability
{
    int Index { get; }

    string Name { get; }

    float CD { get; }

    bool CanUse{ get; }

    MonoBehaviour _This { get; }

    //Texture2D Icon { get; }
}

public class AbilityRepair : MonoBehaviour, Ability
{
    public int Index { get { return 1; } }

    public string Name { get { return "维修"; } }

    private IShipController controller;

    private float maxCD = 30f;
    private float remainTime;
    public float CD { get { return Mathf.Clamp01( remainTime / maxCD ); } }
    

    public MonoBehaviour _This { get { return this as MonoBehaviour; } }

    private bool canUse = false;
    public bool CanUse { get { return this.canUse; } }

    private void Awake()
    {
    }

    void Start()
    {
        this.controller = this.GetComponent<IShipController>();
    }
    
    void Update()
    {
        if(remainTime > 0f)
        {
            canUse = false;
            remainTime -= Time.deltaTime;
        }
        else
        {
            canUse = true;
        }
        

        if(canUse && (Index == 1 ? controller.Ability1Order : controller.Ability2Order))
        {
            Do();
        }
    }
    
    void Do()
    {
        remainTime = maxCD;

        Hull hull = this.GetComponent<Hull>();
        hull.HP += (hull.MaxHP - hull.HP) * 0.25f;
    }
}
