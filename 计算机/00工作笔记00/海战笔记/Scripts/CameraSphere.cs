using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public class CameraSphere : MonoBehaviour
{

    //UI Control
    private UICanvas uiCanvas;
    private float dragSensitivity = 0.05f;

    //Base
    [HideInInspector] public GameObject objFollow;
    private Vector3 chasePos;


    [HideInInspector] public Camera cam;
    [HideInInspector] public GameObject aimPos;
    [HideInInspector] public GameObject defaultPos;
    [HideInInspector] public GameObject dirIndicator;
    private float defaultFOV;
    private float camScale;
    private float input_x;
    private float input_y;
    private bool isFollowing;
    private bool isAiming;

    public bool IsAiming { get { return isAiming; } }


    //speed effects
    private ParticleSystem rainEffect;
    private GameObject speedEffectBase;
    private ParticleSystem speedEffect;

    private AudioSource auWind;



    //Ray hit
    private RaycastHit hit;
    private Ray ray;
    [HideInInspector] public Vector3 hitpoint;


    //tmp
    private Vector3 dir;


    public void Awake()
    {
        cam = this.GetComponentInChildren<Camera>();
        aimPos = this.transform.Find("AimPos").gameObject;
        defaultPos = this.transform.Find("DefaultPos").gameObject;
        dirIndicator = this.transform.Find("Dir").gameObject;

        rainEffect = this.transform.GetComponentInChildren<Camera>().transform.Find("RainEffect").GetComponent<ParticleSystem>();
        speedEffectBase = this.transform.GetComponentInChildren<Camera>().transform.Find("SpeedEffectBase").gameObject;
        speedEffect = speedEffectBase.GetComponentInChildren<ParticleSystem>();

        auWind = this.transform.GetComponentInChildren<Camera>().transform.Find("WindSound").GetComponent<AudioSource>();
    }


    // Use this for initialization
    void Start()
    {
        //UI
        uiCanvas = FindObjectOfType<UICanvas>();

        //Base
        objFollow = GameObject.FindGameObjectWithTag("Player");
        this.transform.position = new Vector3(objFollow.transform.position.x, 0, objFollow.transform.position.z);
        this.transform.rotation = Quaternion.LookRotation(objFollow.transform.forward.OnOceanPlane(), Vector3.up);
        isFollowing = true;
        camScale = 1f;
        defaultFOV = cam.fieldOfView;

        //ray hit
        hitpoint = new Vector3(0, 0, 0);


        //cam init
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(2f)
        .Append(
                cam.transform.DOLocalMove(defaultPos.transform.localPosition, 1f).OnComplete(() => { uiCanvas.EnableControl(); })
        );
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    Focus(1f);
        //}

        //跟随
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFollowing = !isFollowing;
        }

        Vector2 canvasOutPut = uiCanvas.OutPut;

        //旋转视角球
        input_x = canvasOutPut.x * dragSensitivity;//input_x = Input.GetAxis("Mouse X");
        float rotateSpeedX = input_x * (cam.fieldOfView / 60f) * 500f;
        this.transform.Rotate(new Vector3(0, 1f, 0), rotateSpeedX * Time.deltaTime);

        ////缩放
        //if (!isAiming)
        //{
        //    camScale = Mathf.Clamp01(camScale - 0.5f * Input.GetAxis("Mouse ScrollWheel"));
        //    cam.fieldOfView = defaultFOV * camScale;
        //}

        //相机俯仰
        input_y = canvasOutPut.y * dragSensitivity;// input_y = Input.GetAxis("Mouse Y");
        float rotateSpeedY = input_y * (cam.fieldOfView / 60f);

        Vector3 prjOnPlane = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float angle = Vector3.Angle(cam.transform.forward, prjOnPlane);
        angle *= Mathf.Sign(Vector3.Dot(Vector3.Cross(cam.transform.forward, prjOnPlane), cam.transform.right));


        if (angle < 20f - Mathf.Abs(rotateSpeedY) && angle > -8f + Mathf.Abs(rotateSpeedY))
        {
            cam.transform.Rotate(new Vector3(1f, 0, 0), -rotateSpeedY, Space.Self);
        }
        else if (angle >= 20f - Mathf.Abs(rotateSpeedY))
        {
            cam.transform.localEulerAngles = new Vector3(-20f, 0, 0);
            cam.transform.Rotate(new Vector3(1f, 0, 0), Mathf.Clamp(-rotateSpeedY * 1.1f, 0, 99f), Space.Self);
        }
        else
        {
            cam.transform.localEulerAngles = new Vector3(8f, 0, 0);
            cam.transform.Rotate(new Vector3(1f, 0, 0), Mathf.Clamp(-rotateSpeedY * 1.1f, -99f, 0), Space.Self);
        }


        //Ray
        ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out hit, 10000, (0 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Water")), QueryTriggerInteraction.Collide))
        {
            if ((hit.point - this.transform.position).sqrMagnitude < 40000f)
            {
                Vector3 dir = (hit.point - this.transform.position).normalized;
                hitpoint = new Vector3(this.transform.position.x, 0, this.transform.position.z) + new Vector3(dir.x * 200f, 0, dir.z * 200f);
            }
            else
            {
                hitpoint = hit.point;
            }

        }

        if (ray.direction.y > 0f)
        {
            hitpoint = (this.transform.position + ray.direction.OnOceanPlane().normalized * 20000f).OnOceanPlane();
        }
        
    }


    void FixedUpdate()
    {

        if (objFollow != null)
        {
            chasePos = objFollow.transform.position;

            if (objFollow.GetComponent<IShipController>() != null)
            {
                chasePos.y = 0;
            }
        }


        if (isFollowing)
        {
            dir = chasePos - this.transform.position;

            Vector3 dirNormalized = (dir + new Vector3(0f, 0.1f, 0f)).normalized;
            float sqrMagnitude = dir.sqrMagnitude;


            //speed effect
            speedEffectBase.transform.rotation = Quaternion.LookRotation(dirNormalized);
            var trail = speedEffect.trails;
            trail.colorOverTrail = new ParticleSystem.MinMaxGradient(new Color(1f, 1f, 1f, sqrMagnitude / 90000f));

            //sound
            auWind.volume = SoundPlayer.Volume * sqrMagnitude / 90000f;

            //move
            if (sqrMagnitude > 0.001f)
            {
                if (!dirIndicator.activeInHierarchy)
                {
                    dirIndicator.SetActive(true);
                }
                
                dirIndicator.transform.rotation = Quaternion.LookRotation(dirNormalized.OnOceanPlane(), Vector3.up);//dirInd
            }

            if (sqrMagnitude < 0.001f && dirIndicator.activeInHierarchy)
            {
                dirIndicator.SetActive(false);
            }



            if (sqrMagnitude > 0.001f)
            {
                this.transform.position += dir * Time.fixedDeltaTime * 4f;
            }
            if (sqrMagnitude <= 0.001f)
            {
                this.transform.position = chasePos;
            }
        }
    }

    //--
    public void DisableRain()
    {
        var em = rainEffect.emission;
        em.enabled = false;
    }

    //---------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------

    public void SwitchAim()
    {
        if (objFollow != null && objFollow.GetComponent<PlayerController>() != null)
        {
            //开镜
            if (!isAiming)
            {
                uiCanvas.maskAim.SetActive(true);
                cam.transform.position = aimPos.transform.position;
                cam.fieldOfView = 5f;
                cam.transform.rotation = Quaternion.LookRotation(this.hitpoint - cam.transform.position);
                isAiming = true;
            }
            else
            {
                uiCanvas.maskAim.SetActive(false);
                cam.transform.position = defaultPos.transform.position;
                cam.fieldOfView = defaultFOV * camScale;
                cam.transform.rotation = Quaternion.LookRotation(this.hitpoint - cam.transform.position);//???
                isAiming = false;
            }
        }
        
    }


    //----------------------------------视角Dotween相关----------------------------------------
    private object targetTmp = null;
    public void Focus()
    {
        if (targetTmp != null) DOTween.Kill(targetTmp);
        targetTmp = DOTween.To(() => cam.fieldOfView, (x) => { cam.fieldOfView = x; }, defaultFOV * 0.5f, 0.5f).target;
    }

    public void CancelFocus()
    {
        if (targetTmp != null) DOTween.Kill(targetTmp);
        targetTmp = DOTween.To(() => cam.fieldOfView, (x) => { cam.fieldOfView = x; }, defaultFOV * 1f, 0.5f).target;
    }

    public void Focus(float timeToUnFocus)
    {
        Focus();
        Invoke("CancelFocus", timeToUnFocus);
    }

    public void RotateTo(Vector3 dir)
    {
        this.transform.DOComplete();

        uiCanvas.panelDrag.SetActive(false);
        this.transform.DORotateQuaternion(Quaternion.LookRotation(dir, Vector3.up), 0.5f).OnComplete(() => { uiCanvas.panelDrag.SetActive(true); });

        Vector3 dir2 = Vector3.Dot(dir, cam.transform.forward) > 0f ? dir : -dir;

        Vector3 prjDir = Vector3.ProjectOnPlane(dir2, cam.transform.right);

        float angle = Vector3.SignedAngle(prjDir, cam.transform.forward, cam.transform.right);

        this.cam.transform.DOLocalRotate(new Vector3(cam.transform.localEulerAngles.x - angle, 0f, 0f), 0.45f);

    }

    /*
    public void RotateTo2(Vector3 dir)
    {
        this.transform.DOComplete();

        uiCanvas.panelDrag.SetActive(false);
        this.transform.DORotateQuaternion(Quaternion.LookRotation(dir, Vector3.up), 0.5f).OnComplete(() => { uiCanvas.panelDrag.SetActive(true); });
        this.cam.transform.DOLocalRotate(new Vector3(Vector3.SignedAngle(Vector3.ProjectOnPlane(dir, this.transform.right), this.transform.forward, this.transform.right), 0f, 0f), 0.45f);
    }
    */

    //---------------------------------------------------------------------------------

    /// <summary>
    /// 改变追踪目标
    /// </summary>
    /// <param name="obj"></param>
    public void ChangeFollow(GameObject obj)
    {
        objFollow = obj;
    }


    public void ResetFollow()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            objFollow = player.gameObject;
        }
    }


    //------------------------------------------Debug---------------------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, 100f);
    }
}
