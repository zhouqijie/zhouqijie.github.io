using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CameraSphere : MonoBehaviour
{
    //UI Control
    private UICanvas uiCanvas;
    private float dragSensitivity = 0.05f;
    private float minPitch = -20f;
    private float maxPitch = 20;

    //Base
    [HideInInspector] public GameObject objFollow;
    private Vector3 chasePos;


    [HideInInspector] public Camera cam;
    [HideInInspector] public GameObject oneMeterPos;
    [HideInInspector] public GameObject defaultPos;
    private float defaultPosDistance;

    private float defaultFOV;
    private float camScale;
    private float input_x;
    private float input_y;
    private bool isFollowing;   public bool IsFollowing { get { return this.isFollowing; } set { this.isFollowing = value; } }
    private bool isAiming;

    public bool IsAiming { get { return isAiming; } }

    

    //Ray hit
    private RaycastHit hit;
    private Ray ray;
    [HideInInspector] public Vector3 hitpoint;

    private RaycastHit hitBack;
    private Ray rayBack;


    //tmp
    private Vector3 dir;



    private void Awake()
    {
        cam = this.GetComponentInChildren<Camera>();
        oneMeterPos = this.transform.Find("OneMeterPos").gameObject;
        defaultPos = this.transform.Find("DefaultPos").gameObject;
        defaultPosDistance = (defaultPos.transform.position - this.transform.position).magnitude;
    }


    private void Start()
    {
        //UI
        uiCanvas = FindObjectOfType<UICanvas>();

        //Base
        objFollow = GameObject.FindGameObjectWithTag("Player");
        this.transform.position = new Vector3(objFollow.transform.position.x, 50f, objFollow.transform.position.z);
        this.transform.rotation = Quaternion.LookRotation(new Vector3(objFollow.transform.forward.x, 0, objFollow.transform.forward.z), Vector3.up);
        isFollowing = true;
        camScale = 1f;
        defaultFOV = cam.fieldOfView;

        //ray hit
        hitpoint = new Vector3(0, 0, 0);
    }


    void Update()
    {
        //跟随
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    isFollowing = !isFollowing;
        //}

        Vector2 canvasOutPut = uiCanvas.OutPut;

        //旋转视角球
        input_x = canvasOutPut.x * dragSensitivity;//input_x = Input.GetAxis("Mouse X");
        float rotateSpeedX = input_x * (cam.fieldOfView / 60f) * 500f;
        this.transform.Rotate(new Vector3(0, 1f, 0), rotateSpeedX * Time.deltaTime);
        
        //相机俯仰
        input_y = canvasOutPut.y * dragSensitivity;// input_y = Input.GetAxis("Mouse Y");
        float rotateSpeedY = input_y * (cam.fieldOfView / 60f);

        Vector3 prjOnPlane = Vector3.ProjectOnPlane(cam.transform.forward, Vector3.up);
        float angle = Vector3.Angle(cam.transform.forward, prjOnPlane);
        angle *= Mathf.Sign(Vector3.Dot(Vector3.Cross(cam.transform.forward, prjOnPlane), cam.transform.right));


        if (angle < maxPitch - Mathf.Abs(rotateSpeedY) && angle > minPitch + Mathf.Abs(rotateSpeedY))
        {
            cam.transform.Rotate(new Vector3(1f, 0, 0), -rotateSpeedY, Space.Self);
        }
        else if (angle >= maxPitch - Mathf.Abs(rotateSpeedY))
        {
            cam.transform.localEulerAngles = new Vector3(-maxPitch, 0, 0);
            cam.transform.Rotate(new Vector3(1f, 0, 0), Mathf.Clamp(-rotateSpeedY * 1.1f, 0, 99f), Space.Self);
        }
        else
        {
            cam.transform.localEulerAngles = new Vector3(-minPitch, 0, 0);
            cam.transform.Rotate(new Vector3(1f, 0, 0), Mathf.Clamp(-rotateSpeedY * 1.1f, -99f, 0), Space.Self);
        }


        ////Ray
        //ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        //if (Physics.Raycast(ray, out hit, 10000f, (1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Character") | 0 << LayerMask.NameToLayer("Player"))))
        //{
        //    hitpoint = hit.point;
        //}
        //else
        //{
        //    hitpoint = cam.transform.position + cam.transform.forward * 10000f;
        //}


        //摄像机防止穿模
        rayBack = new Ray(oneMeterPos.transform.position, defaultPos.transform.position - oneMeterPos.transform.position);
        if (Physics.Raycast(rayBack, out hitBack, defaultPosDistance, (1 << LayerMask.NameToLayer("Default") | 0 << LayerMask.NameToLayer("SafeArea") | 0 << LayerMask.NameToLayer("Player") | 0 << LayerMask.NameToLayer("Charactor"))))
        {
            cam.transform.position = hitBack.point - rayBack.direction * 0.25f;
        }
        else
        {
            cam.transform.position = defaultPos.transform.position;
        }
    }








    private void FixedUpdate()
    {
        if (objFollow != null)
        {
            chasePos = objFollow.transform.position;
        }


        if (isFollowing)
        {
            dir = chasePos - this.transform.position;

            Vector3 dirNormalized = (dir + new Vector3(0f, 0.1f, 0f)).normalized;
            float sqrMagnitude = dir.sqrMagnitude;

            
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
    
}
