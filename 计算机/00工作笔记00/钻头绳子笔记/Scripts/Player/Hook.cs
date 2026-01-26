using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;
using Obi;


public enum HookStatus
{
    Idle,
    Grab,
    Throw,
    FlyingTo,
    FlyingBack
}



public class Hook : MonoBehaviour
{
    //coms
    private Rigidbody grabRigid;
    private Transform grabBase;
    private RopeController ropeCtrl;

    public Transform clawUp;
    public Transform clawRight;
    public Transform clawDown;
    public Transform clawLeft;


    //args
    private float flySpeed = 10f;       public float FlySpeed { set { this.flySpeed = value; } }
    private float maxDistance = 20f;
    private int layermaskHook;



    //Status and tmp
    private HookStatus hookStatus = HookStatus.Idle; //! read to pick
    public HookStatus HookStatus => this.hookStatus;
    private Vector3 flyDir;
    private IGrabable grabed = null; public IGrabable Grabed => this.grabed;






    void Start()
    {
        grabRigid = this.GetComponentsInChildren<Rigidbody>().FirstOrDefault(r => r.gameObject.name == "Grab");
        grabBase = grabRigid.transform.parent;
        ropeCtrl = this.GetComponentInChildren<RopeController>();

        layermaskHook = (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Environment") | 1 << LayerMask.NameToLayer("Grabable") | 1 << LayerMask.NameToLayer("GrabableDoll"));
    }
    

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        switch (hookStatus)
        {
            case HookStatus.FlyingTo:
                {
                    RaycastHit hit;
                    if (Physics.SphereCast(new Ray(grabRigid.transform.position, flyDir), 0.25f, out hit, flySpeed * Time.fixedDeltaTime * 1.2f, layermaskHook))
                    {
                        var pickable = hit.collider.GetComponentInParent<IGrabable>();
                        if (pickable != null && pickable.CanGrab)
                        {
                            pickable.OnGrab(grabRigid.transform);

                            grabed = pickable;
                        }
                        GrabBack();
                    }
                    if ((grabRigid.transform.position - this.transform.position).magnitude > maxDistance)
                    {
                        GrabBack();
                    }

                    //fly
                    grabRigid.transform.position += (flyDir.normalized) * flySpeed * Time.fixedDeltaTime;

                    ////rope length
                    //ropeCursor.ChangeLength(); //Control in the rope controller.cs
                }
                break;
            case HookStatus.FlyingBack:
                {
                    
                }
                break;
            default:
                break;
        }



        Vector3 startPos = grabRigid.transform.parent.position;
        Vector3 currentPos = grabRigid.transform.position;
        switch (hookStatus)
        {
            case HookStatus.FlyingTo:
            case HookStatus.FlyingBack:
                {
                    //......Draw Line Segs......
                }
                break;
            default:
                break;
        }
        
    }

    




    public void ShootGrab(Vector3 pos)
    {
        if (hookStatus != HookStatus.Idle) return;

        //SOUND
        SoundPlayer.PlaySound2D("rope");

        StartCoroutine(CoShootGrab(pos));

    }
    private IEnumerator CoShootGrab(Vector3 pos)
    {
        hookStatus = HookStatus.FlyingTo;

        //claw open
        ClawOpen();

        //hand out
        bool aimFinish = false;

        this.transform.DOLocalMove(new Vector3(0, 0, 0.3f), 0.1f);
        this.transform.DOLocalRotate(new Vector3(0, -20f, 0), 0.1f).OnComplete(() => { aimFinish = true; });

        yield return new WaitUntil(() => aimFinish);

        grabRigid.isKinematic = true;

        //DIr
        flyDir = (pos - grabRigid.transform.position);

        //ROpe
        ropeCtrl.ThrowOut();

        //grab look at
        grabRigid.transform.rotation = Quaternion.LookRotation(-Vector3.up, flyDir);

        yield return null;
    }





    public void GrabBack()
    {
        if (hookStatus == HookStatus.FlyingBack) return;

        hookStatus = HookStatus.FlyingBack;

        StartCoroutine(CoGrabBack());
    }
    private IEnumerator CoGrabBack()
    {
        grabRigid.isKinematic = true;

        //ROpe
        ropeCtrl.PullBack();

        //back tweener
        bool isbacked = false;
        grabRigid.transform.DOLocalMove(Vector3.zero, 0.4f).SetEase(Ease.Linear).OnComplete(() => { isbacked = true; });

        yield return new WaitUntil(() => isbacked);
        
        if(grabed != null)
        {
            hookStatus = HookStatus.Grab;
        }
        else
        {
            //hand back
            bool aimFinish = false;
            //hoot rot
            bool hootFinish = false;


            grabRigid.transform.DOLocalRotate( Vector3.zero, 0.2f).OnComplete(() => { hootFinish = true; });

            this.transform.DOLocalRotate(Vector3.zero, 0.2f);
            this.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => { aimFinish = true; });

            yield return new WaitUntil(() => aimFinish && hootFinish);


            //claw close
            ClawClose();

            hookStatus = HookStatus.Idle;
        }
    }






    public void GrabThrow(Ray ray)
    {
        if (hookStatus != HookStatus.Grab && hookStatus != HookStatus.FlyingBack) return;

        StartCoroutine(CoThrowGrab(ray));
    }
    private IEnumerator CoThrowGrab(Ray ray)
    {
        hookStatus = HookStatus.Throw;

        grabed.OnDrop(ray.direction);
        

        grabed = null;


        //hand back
        bool aimFinish = false;
        //hoot rot
        bool hootFinish = false;


        grabRigid.transform.DOLocalRotate(Vector3.zero, 0.2f).OnComplete(() => { hootFinish = true; });

        this.transform.DOLocalRotate(Vector3.zero, 0.2f);
        this.transform.DOLocalMove(Vector3.zero, 0.2f).OnComplete(() => { aimFinish = true; });

        yield return new WaitUntil(() => aimFinish && hootFinish);


        //claw close
        ClawClose();

        hookStatus = HookStatus.Idle;
    }




    //Claw
    private void ClawOpen()
    {
        clawUp.DOKill();
        clawRight.DOKill();
        clawDown.DOKill();
        clawLeft.DOKill();

        clawUp.DOLocalRotate(new Vector3(-60f, 0, 0), 0.5f);
        clawRight.DOLocalRotate(new Vector3(0, 0, -60f), 0.5f);
        clawDown.DOLocalRotate(new Vector3(60f, 0, 0), 0.5f);
        clawLeft.DOLocalRotate(new Vector3(0, 0, 60f), 0.5f);
    }
    private void ClawClose()
    {
        clawUp.DOKill();
        clawRight.DOKill();
        clawDown.DOKill();
        clawLeft.DOKill();

        clawUp.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        clawRight.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        clawDown.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
        clawLeft.DOLocalRotate(new Vector3(0, 0, 0), 0.5f);
    }
}
