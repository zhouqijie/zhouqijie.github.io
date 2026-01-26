using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class CollisionDetector : MonoBehaviour
{
    private DamageableDoll damageable;
    private BehaviourPuppet bp;

    //layer set
    private int layerDoll;
    private int layerGrabobj;
    private int layerEnvironment;
    private int layerDefault;

    private void Awake()
    {
        layerDoll = LayerMask.NameToLayer("Ragdoll");
        layerGrabobj = LayerMask.NameToLayer("Grabable");
        layerEnvironment = LayerMask.NameToLayer("Environment");
        layerDefault = LayerMask.NameToLayer("Default");
    }

    void Start()
    {
        damageable = this.GetComponentInParent<CharacterRoot>().GetComponentInChildren<DamageableDoll>();
        bp = this.GetComponentInParent<CharacterRoot>().GetComponentInChildren<BehaviourPuppet>();
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.sqrMagnitude < Mathf.Pow(5f, 2f)) return;

        float mass = 0.1f;

        int colliderLayer = collision.collider.gameObject.layer;

        //角色之间碰撞伤害
        if (colliderLayer == layerDoll)
        {
            mass = 0.1f;
        }



        //抓取物体碰撞伤害
        if (colliderLayer == layerGrabobj) mass = 2f;

        //地面和环境碰撞伤害
        if (colliderLayer == layerDefault || colliderLayer == layerEnvironment) mass = 0.1f;


        //////DMG//////
        float dmg = collision.relativeVelocity.magnitude * mass * 2f;
        damageable.GetDamage(dmg, collision.collider.name + " root ; " + collision.collider.transform.root.name);

        //SOUNDS
        if(dmg > 50f)
        {
            SoundPlayer.PlaySound2D("hit");
        }
    }
}
