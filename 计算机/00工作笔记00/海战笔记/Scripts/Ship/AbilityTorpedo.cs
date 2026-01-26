using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LauncherType
{
    OneSide,
    BothSide,
    Forward
}

public class AbilityTorpedo : MonoBehaviour, Ability
{
    public int Index { get { return 2; } }

    public string Name { get { return "发射鱼雷"; } }

    private IShipController controller;

    public float maxCD;
    private float remainTime;
    private Vector3 dir;

    [HideInInspector] public float CD { get { return Mathf.Clamp01(remainTime / maxCD); } }

    public GameObject[] launchers;
    public LauncherType type;


    public MonoBehaviour _This { get { return this as MonoBehaviour; } }

    private bool canUse = false;
    public bool CanUse { get {return this.canUse; } }


    private void Awake()
    {

    }

    void Start()
    {
        this.controller = this.GetComponent<IShipController>();
    }

    void Update()
    {

        if (remainTime > 0f)
        {
            canUse = false;
            remainTime -= Time.deltaTime;
        }
        else
        {
            if (controller is AIController && controller.TargetLock != null)
            {
                dir = controller.TargetLock.gameObject.transform.position - this.transform.position; //没有预瞄     //角度出现偏差的原因是一次限制和二次限制不一样
            }
            else
            {
                dir = controller.Target - this.transform.position;
            }

            switch (type)
            {
                case LauncherType.OneSide:
                    {
                        if (Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(this.transform.forward, Vector3.up), Vector3.ProjectOnPlane(dir, Vector3.up).normalized)) < 0.85f)//一次限制
                        {
                            canUse = true;
                        }
                        else
                        {
                            canUse = false;
                        }
                    }
                    break;
                case LauncherType.BothSide:
                    {
                        if (Mathf.Abs(Vector3.Dot(Vector3.ProjectOnPlane(this.transform.forward, Vector3.up), Vector3.ProjectOnPlane(dir, Vector3.up).normalized)) < 0.85f)//一次限制
                        {
                            canUse = true;
                        }
                        else
                        {
                            canUse = false;
                        }
                    }
                    break;
                case LauncherType.Forward:
                    {
                        if (Vector3.Dot(Vector3.ProjectOnPlane(this.transform.forward, Vector3.up), Vector3.ProjectOnPlane(dir, Vector3.up).normalized) > 0.85f)//一次限制
                        {
                            canUse = true;
                        }
                        else
                        {
                            canUse = false;
                        }
                    }
                    break;
                default:
                    break;
            }
        }





        if (canUse && (Index == 1 ? controller.Ability1Order : controller.Ability2Order))
        {
            Do();
        }
    }

    void Do()
    {
        remainTime = maxCD;

        switch (type)
        {
            case LauncherType.OneSide:
                {
                    GameObject launcher = launchers.FirstOrDefault(l => Vector3.Dot(l.transform.position - this.transform.position, dir) > 0);
                    float baseEuler = Vector3.Dot(launcher.transform.position - this.transform.position, this.transform.right) > 0 ? 90f : -90f;

                    launcher.transform.localEulerAngles = new Vector3(0f, Mathf.Clamp(-Vector3.SignedAngle(dir, this.transform.forward, Vector3.up), baseEuler - 45f, baseEuler + 45f), 0f);//二次限制

                    for (int i = 0; i < 4; i++)
                    {
                        LaunchTorpedo(launcher.transform.Find("FirePos").position, Quaternion.Euler(launcher.transform.eulerAngles + new Vector3(0, (-1.5f + i) * 3f, 0f)));
                    }
                }
                break;
            case LauncherType.BothSide:
                {
                    GameObject launcher = launchers[0];

                    float signAngle = -Vector3.SignedAngle(dir, this.transform.forward, Vector3.up);

                    if(signAngle > 0)
                    {
                        signAngle = Mathf.Clamp(signAngle, 45f, 135f);
                    }
                    else
                    {
                        signAngle = Mathf.Clamp(signAngle, -135f, -45f);
                    }

                    launcher.transform.localEulerAngles = new Vector3(0f, signAngle, 0f);//二次限制

                    for (int i = 0; i < 4; i++)
                    {
                        LaunchTorpedo(launcher.transform.Find("FirePos").position, Quaternion.Euler(launcher.transform.eulerAngles + new Vector3(0, (-1.5f + i) * 3f, 0f)));
                    }
                }
                break;
            case LauncherType.Forward:
                {
                    for (int i = 0; i < launchers.Length; i++)
                    {
                        LaunchTorpedo(launchers[i].transform.Find("FirePos").position, launchers[i].transform.rotation);
                    }
                }
                break;
            default:
                break;
        }
        
    }


    public void LaunchTorpedo(Vector3 pos, Quaternion rot)
    {
        GameObject torpedo = Instantiate(Resources.Load<GameObject>("Prefabs/Shells/Torpedo"), pos, rot, null);
        torpedo.GetComponent<Rigidbody>().velocity = this.GetComponentInParent<Rigidbody>().velocity + torpedo.transform.forward * 20f;

        var hull = this.GetComponent<Hull>();
        var shipConfig = hull.shipConfig;
        torpedo.GetComponent<Shell>().damage = hull.shipPowerMultip * shipConfig.shipInfo.attack * Utils.CalFac(shipConfig.lvl) * 4f;

        torpedo.GetComponent<Shell>().piercingPercentage = 0f;
        torpedo.GetComponent<Shell>().shooter = this.GetComponent<IShipController>();
        torpedo.GetComponent<Shell>().isTorpedo = true;
        torpedo.GetComponent<TorpedoNav>();
    }
}
