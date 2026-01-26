using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Game : MonoBehaviour
{
    private static Game inst = null;
    public static Game Instance => inst;

    //coms
    private UICanvas uiCanvas;
    private WaypointNav nav;
    private PlayerController playerCtrl;


    //status
    private bool isEnd = false;

    private void Awake()
    {
        inst = this;

        InitPlayerAndCanvas();

        InitBGM();
    }
    

    void InitPlayerAndCanvas()
    {

        //Start Pos
        Vector3 startpos = new Vector3();
        GameObject startpoint = GameObject.FindGameObjectWithTag("Start");
        if (startpoint != null) startpos = startpoint.transform.position;

        //player and canvas
        GameObject player = null;
        GameObject canvas = null;
        if (!(FindObjectOfType<PlayerController>() != null))
        {
            player = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), startpos, new Quaternion(), null);
        }
        else
        {
            player = FindObjectOfType<PlayerController>().gameObject; player.transform.position = startpos;
        }

        if (!(FindObjectOfType<UICanvas>() != null))
        {
            canvas = Instantiate(Resources.Load<GameObject>("Prefabs/UICanvas"));
        }
        else
        {
            canvas = FindObjectOfType<UICanvas>().gameObject;
        }

        
        //coms pointer
        nav = player.GetComponent<WaypointNav>();
        playerCtrl = player.GetComponent<PlayerController>();
        uiCanvas = canvas.GetComponent<UICanvas>();
    }

    void InitBGM()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/BGM"), null);
    }

    void Start()
    {
        //音量应用
        SoundPlayer.ApplyVolume();

        //角色设置 Characters
        int level = Infomanager.Instance.userData.currentLevel;
        CharacterRoot[] characters = FindObjectsOfType<CharacterRoot>();
        foreach(var chara in characters)
        {
            //体积大判定为Boss，不可抓取
            bool isBigSize = chara.transform.localScale.y > 1.25f;
            chara.GetComponentInChildren<DollController>().isBoss = isBigSize;
            //体积血量增幅
            float sizeHpMultip = chara.transform.localScale.y  * chara.transform.localScale.y;
            
            chara.GetComponentInChildren<DamageableDoll>().HpMultip = (1f + Mathf.Clamp(level * 0.1f, 0f, 1f)) * sizeHpMultip;
            chara.GetComponentInChildren<DollController>().attackSpeed = 1f + Mathf.Clamp(level * 0.1f, 0f, 1f);
        }

        //开始游戏暂停
        StartGame1();


        ////DEBUG
        //Invoke("Win", 2f);
    }

    void Update()
    {
        
    }

    
    public void StartGame1()
    {
        nav.IsPause = true;

        uiCanvas.ShowWindowStart(true);
    }
    public void StartGame2()
    {
        nav.IsPause = false;

        //record
        Recorder.StartRecord(() => { }, (code, msg) => { }, 300);
    }

    public void Win()
    {
        if (isEnd) return;
        isEnd = true;

        nav.IsPause = true;

        //record
        Recorder.StopRecord((path) => { }, (code, msg) => { });

        //Event
        playerCtrl.onFinishLevel.Invoke();

        uiCanvas.ShowWindowWin(true);
    }

    public void Lose(int dieReason = 0) //0 - normal 1 - drill over heat
    {
        if (isEnd) return;

        nav.IsPause = true;

        uiCanvas.ShowWindowLose(true);
    }

    public void Continue()
    {
        if (isEnd) return;

        nav.IsPause = false;
        
        playerCtrl.Recovery();
    }


    //----------------STATIC----------------
    public static void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public static void LoadNextLevel()
    {
        //Next Level
        Infomanager.Instance.userData.currentLevel++;

        string sceneName = "Level_" + Infomanager.Instance.userData.currentLevel % Infomanager.levelSceneCount;

        Debug.Log("=====Load Scene: " + sceneName + "======");

        SceneManager.LoadScene(sceneName);
    }


    // ----------- Utils -------------
    public static void RandomEquipNotHave(out bool canGet, out bool drillOrHook, out int equipId)
    {
        canGet = false;
        drillOrHook = Random.value < 0.5f;
        equipId = 0;

        if (drillOrHook)
        {
            DrillInfo[] drillsNotHave = Infomanager.Instance.drillInfos.Where(d => !Infomanager.Instance.userData.myDrills.Contains(d.id)).ToArray();
            if (drillsNotHave.Length > 0)
            {
                canGet = true;
                equipId = drillsNotHave[Random.Range(0, drillsNotHave.Length)].id;
            }
        }
        else
        {
            HookInfo[] hooksNotHave = Infomanager.Instance.hookInfos.Where(d => !Infomanager.Instance.userData.myHooks.Contains(d.id)).ToArray();
            if (hooksNotHave.Length > 0)
            {
                canGet = true;
                equipId = hooksNotHave[Random.Range(0, hooksNotHave.Length)].id;
            }
        }
    }
}
