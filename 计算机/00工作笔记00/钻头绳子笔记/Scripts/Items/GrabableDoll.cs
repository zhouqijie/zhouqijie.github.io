using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RootMotion.Dynamics;

public class GrabableDoll : MonoBehaviour, IGrabable
{
    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => null;

    public bool CanGrab => (behaviourPuppet.state == BehaviourPuppet.State.Puppet && !dollCtrl.isBoss);

    private Transform root;
    private CharacterRoot charaRoot;
    private DollController dollCtrl;
    private PuppetMaster puppetMaster;
    private BehaviourPuppet behaviourPuppet;

    public void OnDrop(Vector3 dir)
    {
        this.transform.parent = root;
        
        dollCtrl.CancelGrab();

        StartCoroutine(CoThrow(dir));

        //Face
        charaRoot.FaceGrabing = false;
    }

    public void OnGrab(Transform grab)
    {
        if (dollCtrl.weaponType != 0) { Debug.Log("OnGrab Drop"); }

        //丢失武器
        dollCtrl.CheckResetType();


        this.transform.parent = grab;
        
        dollCtrl.EnterGrab();

        //Face
        charaRoot.FaceGrabing = true;
    }

    // Start is called before the first frame update
    void Awake()
    {
        charaRoot = this.GetComponentInParent<CharacterRoot>();
        root = this.GetComponentInParent<CharacterRoot>().transform;
        dollCtrl = root.GetComponentInChildren<DollController>();
        puppetMaster = root.GetComponentInChildren<PuppetMaster>();
        behaviourPuppet = root.GetComponentInChildren<BehaviourPuppet>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoThrow(Vector3 dir)
    {
        //  抛出方法
        puppetMaster.state = PuppetMaster.State.Frozen;

        yield return new WaitForSeconds(0.01f);

        foreach (var rigidPart in puppetMaster.GetComponentsInChildren<Rigidbody>())
        {
            rigidPart.AddForce(dir.normalized * 25f, ForceMode.VelocityChange);
        }

        //  如果抛出完成时刻  => 角色未死亡就从Freeze转换到Alive。死亡则保持Dead
        yield return new WaitForSeconds(0.5f);
        if(puppetMaster.state != PuppetMaster.State.Dead)
        {
            puppetMaster.state = PuppetMaster.State.Alive;
        }
    }
}
