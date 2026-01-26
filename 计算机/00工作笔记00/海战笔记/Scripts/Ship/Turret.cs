using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    private Radar radar;
    private IShipController controller;
    private Hull hull;
    private GameObject[] bars;

    //Turret Type
    public int turretType;

    //Ammo Type
    private GameObject shell;
    private float shellDamage;
    private float shellPiercingPercentage;
    private int[] ammoCounts;


    //Temp
    [HideInInspector] public float flyingTime;
    private Vector3 target;

    private float v0;
    private float length;
    private float height;
    private Vector3 targetDir;
    private Vector3 targetDir_hor;
    private Vector3 targetFireDir;
    private Vector3 targetFireDir_OnhullPlane;
    private Vector3 hullBack;
    private Vector3 hullForward;

    private Vector3 baseNormal;// = this.transform.up;
    private Vector3 baseRight;// = this.transform.parent.right;


    private float currentAngleX;
    private float targetAngleX;
    private float realTargetAngleX;
    private float currentAngleY;//Multiple ==> foreach set
    private float targetAngleY;
    private float realTargetAngleY;



    //Firing
    private float reloadTime;
    [HideInInspector] public float reloadProgress = 1.0f;
    [HideInInspector] public bool isLoaded;
    private bool reloading = false;
    private bool isFiringOrder;
    [HideInInspector] public bool isRightAngle;

    //Angle Limit
    public Vector2 minMaxAngleX;
    public Vector2 minMaxAngleY;

    private void Awake()
    {
    }

    void Start()
    {
        //bars init
        isLoaded = true;
        int barCount = this.transform.childCount;
        bars = new GameObject[barCount];
        for(int i = 0; i < barCount; i++)
        {
            bars[i] = this.transform.GetChild(i).gameObject;
        }

        //统一预制体
        switch (turretType)
        {
            case 0:
                shell = Resources.Load<GameObject>("Prefabs/Shells/ShellDefault");
                break;
            case 1:
                shell = Resources.Load<GameObject>("Prefabs/Shells/ShellDefaultSmall");
                break;
            default:
                shell = Resources.Load<GameObject>("Prefabs/Shells/ShellDefaultSmall");
                break;
        }

        //radar
        radar = this.GetComponentInParent<Radar>();
        
        //default angle
        targetAngleX = 0f;
        targetAngleY = 0f;

        //curve
        InvokeRepeating("CalFireDir", Random.Range(0.1f, 0.6f), 0.5f);
    }

    public void TurretInit(float damageMultip)
    {
        //init
        controller = GetComponentInParent<IShipController>();
        hull = GetComponentInParent<Hull>();

        ShipConfig shipConfig = GetComponentInParent<Hull>().shipConfig; 
        ShipWeaponCom weaponCom = shipConfig.activeWeaponCom;

        //Init With User Data 
        switch (turretType)
        {
            case 0:
                {
                    if(weaponCom != null)
                    {
                        WeaponInfo weaponInfo = weaponCom.weaponInfo;
                        v0 = weaponInfo.baseV0 * 3f;//???三倍飞行速度
                        reloadTime = weaponInfo.reloadTime * 0.4f;//??? 2.5倍装填速度
                        shellDamage = shipConfig.shipInfo.attack * Utils.CalFac(shipConfig.lvl);
                        shellDamage += weaponInfo.attack * Utils.CalFac(weaponCom.lvl);

                        shellDamage *= 1f;//??? 1倍原伤害

                       shellPiercingPercentage = weaponInfo.piercingPercentage;
                    }
                    else
                    {
                        v0 = GetComponentInParent<Hull>().secondaryV0 * 3f;//???三倍飞行速度
                        reloadTime = 10f * 0.4f;//??? 2.5倍装填速度
                        shellDamage = shipConfig.shipInfo.attack * Utils.CalFac(shipConfig.lvl);
                        shellPiercingPercentage = 0f;
                    }
                }
                break;
            case 1:
                {
                    v0 = GetComponentInParent<Hull>().secondaryV0 * 3f;//??? 副炮三倍飞行速度
                    reloadTime = GetComponentInParent<Hull>().secondaryBaseReloadTime;//??? 副炮装填不变

                    shellDamage = shipConfig.shipInfo.attack * Utils.CalFac(shipConfig.lvl);
                    shellDamage *= 0.33f * 0.5f;//??? 副炮0.5倍原伤害

                    shellPiercingPercentage = 0f;
                }
                break;
            default:
                break;
        }
        

        //舰长攻击加成
        if(hull.shipConfig.crew != null)
        {
            shellDamage *= Utils.CalFacCrew(hull.shipConfig.crew.crewInfo.attack, hull.shipConfig.crew.lvl);
        }

        //damageMultip
        shellDamage *= damageMultip;
    }




    void Update()
    {
        //LoadProgress
        if (reloading)
        {
            reloadProgress += (Time.deltaTime / reloadTime);
        }
        //Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 100 * reloadProgress, 0), Color.blue);

        
    }

    private void CalFireDir()
    {

        //controller目标获取
        if (controller.TargetLock != null)
        {
            if (!float.IsNaN(flyingTime))
            {
                target = (controller.TargetLock.transform.position + controller.TargetLock.GetComponent<Rigidbody>().velocity * (flyingTime + Random.Range(0f, 1f))).OnOceanPlane();
                
            }
            else
            {
                target = ((controller.TargetLock.transform.position + controller.TargetLock.GetComponent<Rigidbody>().velocity * 30f)).OnOceanPlane();
            }
        }
        else
        {
            target = (controller.Target).OnOceanPlane();
        }


        

        if (turretType == 1 && (target - this.transform.position).sqrMagnitude > Mathf.Pow(1000f, 2))
        {
            GetParacurveFromRadar();
            return;
        }
        
        CalParacurveSelf();
    }

    private void CalParacurveSelf()
    {

        //计算发射角
        Vector3 firePos = this.transform.GetChild(0).GetChild(0).transform.position;
        targetDir = target - firePos;
        targetDir_hor = new Vector3(targetDir.x, 0, targetDir.z);
        length = (new Vector3(target.x - firePos.x, 0, target.z - firePos.z)).magnitude;
        height = target.y - firePos.y;
        targetFireDir = targetDir_hor.magnitude * Mathf.Tan(Utils.CalculateAngle(length, height, v0, out flyingTime, true, 98f)) * (new Vector3(0, 1f, 0)) + targetDir_hor;

        if(float.IsNaN(targetFireDir.x) || float.IsNaN(targetFireDir.y) || float.IsNaN(targetFireDir.z))
        {
            targetFireDir = targetDir_hor.normalized + new Vector3(0f, 10f, 0f);
        }


    }

    private void GetParacurveFromRadar()
    {
        targetFireDir = radar.targetFireDir;
    }


    private void FixedUpdate()
    {
        //-----------------------------计算local fire angle------------------------------------------

        //炮塔平面法线
        baseNormal = this.transform.parent.up;
        baseRight = this.transform.parent.right;

        //投影到炮塔
        targetFireDir_OnhullPlane = Vector3.ProjectOnPlane(targetFireDir, baseNormal);

        //炮塔角和俯仰角
        realTargetAngleX = -Mathf.Sign(Vector3.Angle(targetFireDir, baseRight) - 90f) * Vector3.Angle(targetFireDir_OnhullPlane, this.transform.parent.forward);
        realTargetAngleY = -Mathf.Sign(Vector3.Angle(targetFireDir, baseNormal) - 90f) * Vector3.Angle(targetFireDir_OnhullPlane, targetFireDir);//!!!!!
        

        if (!float.IsNaN(realTargetAngleX))
        {
            targetAngleX = Mathf.Clamp(realTargetAngleX, minMaxAngleX.x, minMaxAngleX.y);
        }
        else
        {
            realTargetAngleX = 180f;
            targetAngleX = 0f;
        }

        if (!float.IsNaN(realTargetAngleY))
        {
            targetAngleY = Mathf.Clamp(realTargetAngleY, minMaxAngleY.x, minMaxAngleY.y);
        }
        else
        {
            targetAngleY = minMaxAngleY.y;
        }
        
        //--------------------------------------------------------------------------------------------



        //转炮
        currentAngleX = -Vector3.SignedAngle(this.transform.forward, this.transform.parent.forward, this.transform.parent.up);


        if (realTargetAngleX < 175f && realTargetAngleX > -175f)
        {
            if (currentAngleX < targetAngleX - 1f)
            {
                this.transform.Rotate(new Vector3(0f, 1f, 0f), Space.Self);
            }
            else if (currentAngleX > targetAngleX + 1f)
            {
                this.transform.Rotate(new Vector3(0f, -1f, 0f), Space.Self);
            }
            else
            {
                this.transform.localEulerAngles = new Vector3(0f, targetAngleX, 0f);
            }
        }


        //俯仰
        for (int i = 0; i < bars.Length; i++)
        {
            currentAngleY = Vector3.SignedAngle(bars[i].transform.forward, this.transform.forward, this.transform.right);


            if (currentAngleY < targetAngleY - 1f)
            {
                bars[i].transform.Rotate(new Vector3(-1f, 0f, 0f), Space.Self);
                continue;
            }

            if (currentAngleY > targetAngleY + 1f)
            {
                bars[i].transform.Rotate(new Vector3(1f, 0f, 0f), Space.Self);
                continue;
            }

            bars[i].transform.localEulerAngles = new Vector3(-targetAngleY, 0f, 0f);
        }

        
        //isRightAngle
        isRightAngle = Mathf.Abs(currentAngleX - realTargetAngleX) < 1f && Mathf.Abs(currentAngleY - realTargetAngleY) < 1f;

        //controller开火指令获取
        //isFiringOrder = false;//
        switch (turretType)
        {
            case 0:
                isFiringOrder = controller.PrimaryFiringOrder;
                break;
            case 1:
                isFiringOrder = controller.SecondaryFiringOrder;
                break;
            default:
                break;
        }

        //开火
        if (isFiringOrder && isLoaded && isRightAngle)
        {
            StartCoroutine(CoFire());
        }
        
    }

    private IEnumerator CoFire()
    {
        isLoaded = false;

        //准确度accuracy
        float accuracy = 1f / GetComponentInParent<Hull>().accuracyMultipiler; //Debug.Log("发射准确度" + accuracy);

        if (turretType != 0)
        {
            yield return new WaitForSeconds(Random.Range(0f, 0.5f));
        }

        reloadProgress = 0f;

        for (int i = 0; i < bars.Length; i++)
        {
            //后座
            if(turretType == 0 && i == bars.Length - 1)
            {
                this.GetComponentInParent<Rigidbody>().AddRelativeTorque(new Vector3(0, 0, Vector3.Dot(this.transform.forward * this.GetComponentInParent<Rigidbody>().mass * 200f, hull.transform.right)));
            }
            //发射
            Vector3 fireDir = Vector3.Normalize(bars[i].transform.forward + Random.insideUnitSphere * accuracy * 0.0025f);//  + Random.insideUnitSphere * accuracy * 0.01f

            //音效
            if (true)//all bar sounds
            {
                switch (turretType)
                {
                    case 0:
                        SoundPlayer.PlaySound3D("主炮1", this.transform.position);
                        break;
                    case 1:
                        SoundPlayer.PlaySound3D("炮击" + Random.Range(1, 4), this.transform.position);
                        break;
                    default:
                        break;

                }
            }

            //炮口效果
            GameObject gunfire;
            if (turretType == 0)
            {
                gunfire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/GunFire"), bars[i].transform.GetChild(0).position, bars[i].transform.rotation, bars[i].transform);
                Destroy(gunfire.GetComponent<Light>(), 0.05f);
                Destroy(gunfire, 3f);
            }
            else
            {
                gunfire = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/GunFireSmall"), bars[i].transform.GetChild(0).position, bars[i].transform.rotation, bars[i].transform);
                Destroy(gunfire, 3f);
            }

            //炮弹生成
            GameObject o = Instantiate(shell, bars[i].transform.GetChild(0).position, bars[i].transform.rotation);
            o.GetComponent<Rigidbody>().velocity = fireDir * v0;
            o.GetComponent<Shell>().shooter = this.GetComponentInParent<IShipController>();
            o.GetComponent<Shell>().isSecondary = turretType == 0 ? false : true;
            o.GetComponent<Shell>().damage = shellDamage;
            o.GetComponent<Shell>().piercingPercentage = shellPiercingPercentage;
            Destroy(o, 30f);

            yield return new WaitForSeconds(0.1f);
        }


        reloading = true;

        if (turretType == 0)
        {
            yield return new WaitForSeconds(reloadTime);
        }
        else
        {
            float rdmTime = Random.Range(0.75f, 1f) * reloadTime;
            reloadProgress = (reloadTime - rdmTime) / reloadTime;
            yield return new WaitForSeconds(rdmTime);
        }

        isLoaded = true;
        reloadProgress = 1.0f;
        reloading = false;

        //Invoke("LoadShell", reloadTime);
    }

    //private void LoadShell()
    //{
    //    isLoaded = true;
    //}

    private void OnGUI()
    {
        //GUI.TextArea(new Rect(0f, 600f, 100f, 100f), "currentX: " + currentAngleX + "\ncurrentY:" + currentAngleY);
        //GUI.TextArea(new Rect(0f, 600f, 100f, 100f), "target:" + target + "\ntargetX: " + targetAngleX + "\ntargetY:" + targetAngleY);
    }
}
