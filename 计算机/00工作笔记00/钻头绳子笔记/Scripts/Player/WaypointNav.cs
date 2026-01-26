
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public enum Status
{
    WalkTo,
    JumpTo,
    Activate,
    Aim,
    End,
}

public class WaypointNav : MonoBehaviour
{
    //coms
    private PlayerController playerCtrl;
    private Camera cam;


    //targets
    private IDamageable[] targets = null;
    private List<IDamageable> targetsAlive = null;



    //status
    private Status state = Status.WalkTo;
    private Waypoint loc;                    public Waypoint Loc => this.loc;
    public bool stopped { get { return Vector3.Angle(this.transform.forward, loc.transform.forward) < 1f && (this.transform.position - loc.transform.position).sqrMagnitude < 0.01f; } }

    //pause
    private bool isPause = false;            public bool IsPause { get { return this.isPause; } set { isPause = value; } }


    //-------------------------------------
    private bool isMoving = false;
    private const float movespeed = 3f;
    private float timer = 0f;



    private void Awake()
    {
        playerCtrl = this.GetComponent<PlayerController>();
        cam = this.GetComponentInChildren<Camera>();
    }

    void Start()
    {
        //event listen
        playerCtrl.onBreakTarget.AddListener((o) => {
            this.OnSomeObjectBreak(o);
        });

        //State Init
        loc = FindObjectsOfType<Waypoint>().OrderBy(l => (l.transform.position - this.transform.position).sqrMagnitude).First();
        this.transform.position = loc.transform.position;
        this.transform.rotation = loc.transform.rotation;

        state = Status.WalkTo;
    }


    void Update()
    {
        if (isPause) return;

        //Timer
        timer -= Time.deltaTime;


        //States

        switch (state)
        {
            case Status.WalkTo:
                {
                    if (!isMoving)
                    {
                        isMoving = true;

                        float duration = (loc.nextLoc.transform.position - this.transform.position).magnitude / movespeed;

                        this.transform.DORotateQuaternion(loc.nextLoc.transform.rotation, 2f);
                        this.transform.DOMove(loc.nextLoc.transform.position, duration).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            isMoving = false;
                            TryTransition();
                        });
                    }
                }
                break;
            case Status.JumpTo:
                {
                    if (!isMoving)
                    {
                        isMoving = true;
                        this.transform.DOJump(loc.nextLoc.transform.position, 5f, 1, 2f).OnComplete(() =>
                        {
                            isMoving = false;
                            TryTransition();
                        });
                    }
                }
                break;
            case Status.Activate:
                {


                    if (!isMoving && timer < 0f)
                    {
                        isMoving = true;

                        float duration = (loc.nextLoc.transform.position - this.transform.position).magnitude / movespeed;

                        this.transform.DORotateQuaternion(loc.nextLoc.transform.rotation, 2f);
                        this.transform.DOMove(loc.nextLoc.transform.position, duration).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            isMoving = false;
                            TryTransition();
                        });
                    }
                }
                break;
            case Status.Aim:
                {
                    if (targetsAlive != null && targetsAlive.Count > 0)
                    {
                        //No Cam-Following !!
                        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, loc.transform.rotation, Time.deltaTime);
                    }
                    else
                    {
                        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(Vector3.forward, Vector3.up), Time.deltaTime);
                        
                        if (!isMoving)
                        {
                            isMoving = true;

                            float duration = (loc.nextLoc.transform.position - this.transform.position).magnitude / movespeed;

                            this.transform.DORotateQuaternion(loc.nextLoc.transform.rotation, 2f);
                            this.transform.DOMove(loc.nextLoc.transform.position, duration).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                isMoving = false;
                                TryTransition();
                            });
                        }
                    }
                }
                break;

            case Status.End:
                {
                    //do nothing
                }
                break;
            default:
                break;
        }



    }


    public void OnSomeObjectBreak(IDamageable o)
    {
        switch (state)
        {
            case Status.Aim:
                {
                    if (targetsAlive.Contains(o))
                    {
                        targetsAlive.Remove(o);
                    }
                }
                break;
            default:
                break;
        }
    }



    public void TryTransition()  // reset loc when transition
    {
        loc = loc.nextLoc;
        switch (loc.locType)
        {
            case WaypointType.Walk:
                {
                    state = Status.WalkTo;
                }
                break;
            case WaypointType.Jump:
                {
                    state = Status.JumpTo;
                }
                break;
            case WaypointType.Activate:
                {
                    state = Status.Activate;
                    if (loc.GetComponent<IActicvatable>() != null)
                    {
                        loc.GetComponent<IActicvatable>().Activate();
                        timer = loc.GetComponent<IActicvatable>().WaitTime;
                    }
                    else
                    {
                        throw new UnityException("No Action....");
                    }
                }
                break;
            case WaypointType.Aim:
                {
                    IDamageable[] locTargets = loc.targets;
                    if (locTargets != null)
                    {
                        state = Status.Aim;
                        targets = locTargets;
                        targetsAlive = targets.Where(t => !t.IsBroken).ToList(); 

                        foreach(var target in targets)
                        {
                            if (target is DamageableDoll) { (target as DamageableDoll).DollCtrl.StartAction(); }
                        }
                    }
                    else
                    {
                        throw new UnityException("no target on this waypoint!");
                    }
                }
                break;
            case WaypointType.End:
                {
                    state = Status.End;

                    //end
                    FindObjectOfType<Game>().Win();
                }
                break;
            default:
                break;
        }
    }
    

}

