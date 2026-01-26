using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IShipController
{
    string Name { get; set; }
    GameObject GameObj { get; }

    int Team { get; }
    float Throttle { get; }
    float Rudder { get; }
    Vector3 Target { get; }

    int KillCount { get; set; }
    int AssistCount { get; set; }
    float TotalDamage { get; set; }

    GameObject Target01 { get; set; }
    GameObject Target02 { get; set; }
    GameObject TargetLock { get; }
    
    bool Enabled { get; set; }

    bool PrimaryFiringOrder { get; }
    bool SecondaryFiringOrder { get; }
    bool Ability1Order { get; }
    bool Ability2Order { get; }

    void DieBoardcast();
}

public class PlayerController : MonoBehaviour, IShipController
{
    public string Name { get { return InfoManager.GetInstance().userData.name; } set { } }

    //Hull
    private Hull hull;

    //Cam Sphere
    private CameraSphere camSphere;

    //Input/OutPut
    //private int horInt; // -1-L    0-mid      1-R
    //private int verInt; // -1-D    0-mid      1-U
    //public int HorInt { set { horInt = value; } }
    //public int VerInt { set { verInt = value; } }

    private Vector2 moveVec;
    public Vector2 MoveVec { set { this.moveVec = value; } }

    private float throttle;
    private float rudder;
    private Vector3 target;
    private bool primaryFiringOrder;
    private bool secondaryFiringOrder;
    private bool ability1Order;
    private bool ability2Order;

    public GameObject GameObj { get { return this.gameObject; } }

    public float Throttle { get { return this.throttle; } }
    public float Rudder { get { return this.rudder; } }
    public Vector3 Target { get { return target; } }
    public int Team { get { return 0; } }

    public bool PrimaryFiringOrder { get { return primaryFiringOrder; } }
    public bool SecondaryFiringOrder { get { return secondaryFiringOrder; } }
    public bool Ability1Order { get { return this.ability1Order; } }
    public bool Ability2Order { get { return this.ability2Order; } }

    //Enabled
    public bool Enabled { get { return this.enabled; }set { this.enabled = value; } }


    //Lock
    private bool isAiming = false;
    private bool isLock = false;
    private GameObject targetLock = null;
    public GameObject TargetLock { get { return this.targetLock; } }
    private GameObject target01 = null;
    private GameObject target02 = null;
    public GameObject Target01 { get { return this.target01; } set { this.target01 = value; } }
    public GameObject Target02 { get { return this.target02; } set { this.target02 = value; } }
    private List<GameObject> nearTargets = new List<GameObject>();
    public List<GameObject> NearTargets { get { return this.nearTargets; } }
    


    //killCount
    private int kills;
    private int assists;
    public int KillCount
    {
        get { return this.kills; }
        set { if (value > this.kills) { GameNotice.NoticeCenter("击沉");  } this.kills = value; }//SoundPlayer.PlaySound2D("奖励");
    }
    public int AssistCount
    {
        get { return this.assists; }
        set { if (value > this.assists) { GameNotice.NoticeCenter("助攻"); } this.assists = value; }//SoundPlayer.PlaySound2D("奖励"); 
    }
    private float totalDamage;
    public float TotalDamage { get { return this.totalDamage; } set { this.totalDamage = value; } }



    private float secondaryDistance;

    void Start()
    {
        //cam sphere
        camSphere = FindObjectOfType<CameraSphere>();
        //hull
        hull = GetComponent<Hull>();

        //autofire distance
        secondaryDistance = FindObjectOfType<Game>().viewDistance * Game.identityDistanceFac * 0.5f;

        InvokeRepeating("RepeatTryLock", 1f, 1f);
    }





    void Update()
    {
        //-----------------------------测试-------------------------------



        //------------------------------移动------------------------------

        Vector2 moveVecTrue = new Vector2(Mathf.Clamp(moveVec.x * 2f, -1f, 1f), Mathf.Clamp(moveVec.y * 2f, -1f, 1f));

        rudder = Mathf.Clamp(moveVecTrue.x, -1f, 1f);
        //throttle = Mathf.Clamp(throttle + Mathf.Sign(moveVecTrue.y - throttle) * Time.deltaTime * 0.2f, -1f, 1f);
        throttle = moveVecTrue.y;
        //-------------------------------瞄准-------------------------------
        if (!isLock)//瞄准目标(未锁定)
        {
            target = camSphere.hitpoint;
        }
        else//（锁定）
        {
            if (targetLock.GetComponent<IShipController>().Enabled != false)
            {
                target = targetLock.transform.position;
            }
            else
            {
                CancelLock();
                target = camSphere.hitpoint;
            }
            //设置相机跟踪？？ no
            //。。。
        }

        //------------------------------------------------------------------
        
    }



    //--------------------------------------------------------武器选择相关------------------------------------------------------------
    
    //--------------------------------------------------------开火指令相关------------------------------------------------------------

    public void EnablePrimaryFiringOrder()
    {
        primaryFiringOrder = true;
    }
    public void DisablePrimaryFiringOrder()
    {
        primaryFiringOrder = false;
    }
    
    private void EnableSeconaryFiringOrder()
    {
        this.secondaryFiringOrder = true;
    }
    private void DisableSecondaryFiringOrder()
    {
        this.secondaryFiringOrder = false;
    }



    //------------------------------------------------------------锁定相关------------------------------------------------------------

    public void SwitchAim()
    {
        isAiming = !isAiming;
    }
    

    private void RepeatTryLock()
    {
        //if (isAiming)
        //{
        //    CancelLock();
        //    return;
        //}

        //默认关闭副炮
        secondaryFiringOrder = false;



        AIController[] controllers = FindObjectsOfType<AIController>()
            .Where(c => c.Enabled == true && Vector3.Angle(c.transform.position - this.transform.position, camSphere.transform.forward) < 30f)
            .OrderBy(c => Vector3.Angle(c.transform.position - this.transform.position, camSphere.transform.forward))
            .ToArray();
        
        foreach (var ctrl in controllers)
        {
            float distanceSqr = (ctrl.transform.position - this.transform.position).sqrMagnitude;

            if (ctrl.Team != 0 && distanceSqr < Mathf.Pow(Game.instance.viewDistance * Game.identityDistanceFac, 2))
            {
                if (ctrl.gameObject != targetLock)//锁定目标
                {
                    Lock(ctrl);
                }
                else//未锁定目标
                {
                }

                //锁定目标在范围内，自动副炮
                if (distanceSqr < Mathf.Pow(secondaryDistance, 2))
                {
                    secondaryFiringOrder = true;
                }
                return;
            }
        }

        CancelLock();
    }

    public void Lock(IShipController ship)
    {
        targetLock = ship.GameObj;
        isLock = true;
        FindObjectOfType<UICanvas>().LockOn(targetLock);
        
    }

    public void CancelLock()
    {
        targetLock = null;
        isLock = false;
        FindObjectOfType<UICanvas>().LockOff();
        
    }

    //-------------------------------------------------------------技能--------------------------------------------------------------

    public IEnumerator CoUseAbility(int index)
    {

        switch (index)
        {
            case 1:
                this.ability1Order = true;

                yield return null;
                yield return null;

                this.ability1Order = false;
                break;
            case 2:
                this.ability2Order = true;

                yield return null;
                yield return null;

                this.ability2Order = false;
                break;
            default:
                break;
        }
    }

    //-------------------------------------------------------------死亡--------------------------------------------------------------
    public void DieBoardcast()
    {
        if(this.Target01 != null)
        {
            this.Target01.GetComponent<IShipController>().KillCount += 1;
        }
        
        if(this.Target02 != null)
        {
            this.Target02.GetComponent<IShipController>().AssistCount += 1;
        }
        //UI变化
        FindObjectOfType<UICanvas>().PlayerDie();
        //刷新计分版
        FindObjectOfType<UICanvas>().RefreshStatistics();

        FindObjectOfType<Game>().Judge();

    }

    //------------------------------------------------------------GUI DEBUG------------------------------------------------------------

    void OnGUI()
    {
        //if(GetComponent<Rigidbody>() != null && GetComponent<Hull>() != null)
        //{
        //    GUILayout.TextArea("Throttle: " + Throttle);
        //    GUILayout.TextArea("Rudder: " + Rudder);
        //    GUILayout.TextArea("Speed: " + GetComponent<Rigidbody>().velocity.magnitude / 0.5144444f);

        //    string str = "";
        //    foreach (var item in FindObjectsOfType<AIController>().Select(c => c.GetComponent<Hull>()))
        //    {
        //        str += "\n" + "ai name:" + item.name + " HP: " + item.HP;
        //    }

        //    GUILayout.TextArea("UserHP: " + GetComponent<Hull>().HP + str);

        //}
    }
}
