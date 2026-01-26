using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookController : MonoBehaviour
{
    public Transform hookHolder;

    private Hook hook; public Hook Hook => this.hook;

    private void Awake()
    {
    }

    void Start()
    {
    }
    

    void Update()
    {
        
    }


    public void SetHook()
    {
        hook = this.GetComponentInChildren<Hook>();
    }

    public void OnTargetBroken(IDamageable dmgAble)
    {
        if (dmgAble is DamageableDoll)
        {
            if ((dmgAble as DamageableDoll).DollGrab as IGrabable == hook.Grabed) // null -> current
            {
                if (hook.HookStatus == HookStatus.Grab || hook.HookStatus == HookStatus.FlyingBack)
                {
                    hook.GrabThrow(new Ray(dmgAble.gameObj.transform.position, this.transform.root.forward));
                }
                else
                {

                }
            }
        }
        
    }

    public void ShootHook(Vector3 pos)
    {
        hook.ShootGrab(pos);
    }

    public void ThrowHook(Ray ray)
    {
        hook.GrabThrow(ray);
    }
}
