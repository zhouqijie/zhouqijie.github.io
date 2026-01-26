using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameInput
{
    string UserName { get; set; }

    int Team { get; }

    Vector2 MoveVec { get; }

    Vector3 ViewDir { get; }

    bool Jump { get; }

    void Disable();
}


public class PlayerInput : MonoBehaviour, IGameInput
{
    private string userName;
    public string UserName { get { return this.userName; } set { this.userName = value; } }

    public int team;
    public int Team { get { return this.team; } }

    private Vector2 moveVec;
    public Vector2 MoveVec { get { return this.moveVec; } }

    private Vector3 viewDir;
    public Vector3 ViewDir { get { return this.viewDir; } }

    private bool jump;
    public bool Jump
    {
        get
        {
            bool tmp = this.jump;
            this.jump = false;
            return tmp;
        }
        
    }



    //player
    [HideInInspector] public CameraSphere camSphere;
    [HideInInspector] public UICanvas uicanvas;


    //cam ray
    private RaycastHit hit;


    //aim stuff tmp
    private Vector3 hitDir;// = (hit.point - UtilsGame.GetFirePos(this.transform.position));
    private float hor;// = new Vector3(hitDir.x, 0, hitDir.z).magnitude;
    private float ver;// = Vector3.Dot(hitDir, Vector3.up);













    void Start()
    {
        //agent disable
        var agent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.enabled = false;

        //player stuff
        camSphere = FindObjectOfType<CameraSphere>();
        uicanvas = FindObjectOfType<UICanvas>();
    }




    void Update()
    {
        //Rotation
        viewDir = camSphere.cam.transform.forward;
        if (Physics.Raycast(camSphere.cam.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f)), out hit, 1000f, (0 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Charactor") | 1 << LayerMask.NameToLayer("Default"))))
        {
            hitDir = (hit.point - UtilsGame.GetFirePos(this.transform.position));
            hor = new Vector3(hitDir.x, 0, hitDir.z).magnitude;
            ver = Vector3.Dot(hitDir, Vector3.up);
            viewDir = hor * camSphere.transform.forward + ver * Vector3.up;
        }
        
        

        //RUN
        moveVec = uicanvas.moveVec;

        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        //Fire
        if (Input.GetKey(KeyCode.F))
        {
            var charactor = GetComponent<CharactorController>();
            charactor.StartCoroutine(charactor.CoFire());
        }

        //disguise
        if (Input.GetKeyDown(KeyCode.H))
        {
            var charactor = GetComponent<CharactorController>();
            charactor.SwitchDisguise(Infomanager.GetInstance().userData.activeDisguiseObj.name); //Debug.Log("Disguise: " + Infomanager.GetInstance().userData.activeDisguiseObj.name);
        }

        #region EditorInput
        if (Input.GetKey(KeyCode.W))
        {
            moveVec.y = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVec.y = -1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveVec.x = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveVec.x = 1f;
        }
        #endregion
    }
    

    public void SetJump()
    {
        jump = true;
    }


    /// <summary>
    /// 死亡
    /// </summary>
    public void Disable()
    {
        this.enabled = false;
    }



    //void OnGUI()
    //{
    //    GUILayout.TextArea("HP:" + GetComponent<CharactorController>().Hp);
    //}
}
