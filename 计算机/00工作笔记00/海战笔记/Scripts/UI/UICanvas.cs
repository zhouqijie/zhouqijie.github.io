using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

public class UICanvas : MonoBehaviour
{
    private Game game;

    private GameObject player;
    private Rigidbody playerRigid;

    private CameraSphere camSphere;

    private Camera cam;

    private RectTransform canvasRect;

    private Vector2 outPut;


    #region Windows
    private GameObject windowLoading;
    private GameObject windowStatistics;
    private GameObject windowWinLose;
    private GameObject windowReward;
    #endregion

    #region Items
    [HideInInspector] public GameObject panelDrag;

    private GameObject panelTime;

    private GameObject panelSpeed;

    private GameObject indSecondary;


    private GameObject btnAbility1;
    private GameObject btnAbility2;
    private GameObject btnSupport;
    private GameObject btnFire;
    private GameObject btnFire2;
    private GameObject btnLock;
    private GameObject btnAim;

    private GameObject effectHurt;
    #endregion

    [HideInInspector] public GameObject maskAim;
    private RectTransform indAim;//瞄准刻度尺

    [HideInInspector] public GameObject panelLock;
    [HideInInspector] public GameObject panelInfo;


    //tmp
    [HideInInspector] public bool isChasingShell;

    [HideInInspector] public bool canfire;

    [HideInInspector] public bool CanFireAll { get { return (turretsPrimary).All(t => t.isLoaded && t.isRightAngle); } }

    private Turret[] turretsPrimary;
    private Turret[] turretsSecondary;

    private Ability ability1 = null;
    private Ability ability2 = null;


    //fillImage
    private Image imgFillPrimary;
    private Image imgFillPrimary2;
    private Image imgFillSeconard;
    private Image imgFillAb1;
    private Image imgFillAb2;
    private Image imgFillSupport;


    public Vector2 OutPut
    {
        get
        {
            Vector2 outPutTemp = outPut;
            outPut = new Vector2();
            return outPutTemp;
        }

        set
        {
            outPut = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Game>();
        player = FindObjectOfType<PlayerController>().gameObject;
        playerRigid = player.GetComponent<Rigidbody>();

        //cam
        camSphere = FindObjectOfType<CameraSphere>();
        cam = camSphere.GetComponentInChildren<Camera>();

        //canvasScalar
        canvasRect = this.GetComponent<RectTransform>();

        //windows
        windowLoading = this.transform.Find("LOADING").gameObject;
        windowStatistics = this.transform.Find("Windows").Find("WindowStatistics").gameObject;
        windowWinLose = this.transform.Find("Windows").Find("WindowWinLose").gameObject;
        windowReward = this.transform.Find("Windows").Find("WindowReward").gameObject;

        //items
        panelDrag = this.transform.Find("DragPanel").gameObject;

        panelTime = this.transform.Find("UI_Time").gameObject;

        panelSpeed = this.transform.Find("UI_UserControls").Find("SpeedPanel").gameObject;

        indSecondary = this.transform.Find("UI_UserControls").Find("IndSecondary").gameObject;

        btnAbility1 = this.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonAbility1").gameObject;
        btnAbility2 = this.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonAbility2").gameObject;
        btnSupport = this.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonSupport").gameObject;
        btnFire = this.transform.Find("UI_UserControls").Find("ButtonFire").gameObject;
        btnFire2 = this.transform.Find("UI_UserControls").Find("ButtonFire2").gameObject;
        btnLock = this.transform.Find("UI_UserControls").Find("ButtonLock").gameObject;
        btnAim = this.transform.Find("UI_UserControls").Find("ButtonAim").gameObject;
        

        maskAim = this.transform.Find("UI_UserControls").Find("MaskAim").gameObject;
        indAim = maskAim.transform.Find("IndicatorMask").Find("Indicator").GetComponent<RectTransform>() ;

        panelLock = this.transform.Find("PanelLock").gameObject; panelLock.SetActive(false);
        panelInfo = this.transform.Find("PanelInfo").gameObject; panelLock.SetActive(false);

        effectHurt = this.transform.Find("EFFECT_HURT").gameObject;

        //tmp
        turretsPrimary = player.GetComponentsInChildren<Turret>().Where(t => t.turretType == 0).ToArray();
        turretsSecondary = player.GetComponentsInChildren<Turret>().Where(t => t.turretType != 0).ToArray();

        ability1 = player.GetComponents<Ability>().FirstOrDefault(a => a.Index == 1);
        ability2 = player.GetComponents<Ability>().FirstOrDefault(a => a.Index == 2);

        //fill images
        imgFillPrimary = btnFire.transform.Find("Mask").GetComponent<Image>();
        imgFillPrimary2 = btnFire2.transform.Find("Mask").GetComponent<Image>();
        imgFillSeconard = indSecondary.transform.Find("Mask").GetComponent<Image>();
        imgFillAb1 = btnAbility1.transform.Find("Mask").GetComponent<Image>();
        imgFillAb2 = btnAbility2.transform.Find("Mask").GetComponent<Image>();
        imgFillSupport = btnSupport.transform.Find("Mask").GetComponent<Image>();

        //displayerName/Icon
        if (ability1 != null)
        {
            btnAbility1.GetComponentInChildren<Text>().text = ability1.Name;
            btnAbility1.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + ability1.Name);
        }

        if (ability2 != null)
        {
            btnAbility2.GetComponentInChildren<Text>().text = ability2.Name;
            btnAbility2.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + ability2.Name);
        }

        //---Disable Controls---
        //btnFire.SetActive(false);
        //btnLock.SetActive(false);
        //btnAim.SetActive(false);
        //panelDrag.SetActive(false);
        this.transform.Find("UI_UserControls").gameObject.SetActive(false);
        this.transform.Find("UI_UserControls").GetComponent<RectTransform>().anchorMin = new Vector2(0, -1);
        this.transform.Find("UI_UserControls").GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);

        //---Match Stuff---
        if (game != null && game.gameType != GameType.Match)
        {
            this.transform.Find("UI_MatchStuff").gameObject.SetActive(false);
        }
        else
        {
            this.transform.Find("UI_MatchStuff").gameObject.SetActive(true);
            //refresh statistics
            RefreshStatistics();
        }

        //---Support Stuff---
        if (game != null && game.gameType != GameType.Tutorial && game.gameType != GameType.Match)
        {
            this.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonSupport").gameObject.SetActive(true);
            this.transform.Find("UI_UserControls").Find("AbilityPanel").GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 100);
        }
        else
        {
            this.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonSupport").gameObject.SetActive(false);
            this.transform.Find("UI_UserControls").Find("AbilityPanel").GetComponent<RectTransform>().anchoredPosition = new Vector2(-60, 100);
        }


        //loading hide
        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.5f).AppendCallback(() =>
        {
            windowLoading.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(() => { windowLoading.SetActive(false); });
        });


        //Invoke 
        InvokeRepeating("RepeatingIdentityAlly", 0.2f, 0.2f);

        InvokeRepeating("RefreshSpeedDisplay", 0.5f, 0.5f);
    }

    public void EnableControl()
    {
        //btnFire.SetActive(true);
        //btnLock.SetActive(true);
        //btnAim.SetActive(true);
        //panelDrag.SetActive(true);
        this.transform.Find("UI_UserControls").gameObject.SetActive(true);
        this.transform.Find("UI_UserControls").GetComponent<RectTransform>().DOAnchorMin(new Vector2(0, 0), 0.5f);
        this.transform.Find("UI_UserControls").GetComponent<RectTransform>().DOAnchorMax(new Vector2(1, 1), 0.5f);

        this.transform.Find("MISSION").GetComponent<RectTransform>().DOAnchorMin(new Vector2(0.13f, 0.8f), 0.5f);
        this.transform.Find("MISSION").GetComponent<RectTransform>().DOAnchorMax(new Vector2(0.13f, 0.8f), 0.5f);
        this.transform.Find("MISSION").GetComponent<RectTransform>().DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
    }

    //--------------------航速显示-----------------------
    //--------------------航速显示-----------------------

    /// <summary>
    /// 航速显示
    /// </summary>
    public void RefreshSpeedDisplay()
    {
        float throttle = player.GetComponent<PlayerController>().Throttle;
        float velocity = Mathf.RoundToInt(Vector3.Dot(player.transform.forward, playerRigid.velocity) * 1.94f);

        //border
        panelSpeed.GetComponent<Image>().color = new Color(1f, 1f, 1f, throttle * 0.5f);

        //txt
        string speedStr = "全速前进";
        if (throttle < 0.95f)
        {
            speedStr = "前进";
        }
        if(velocity < 0.5f)
        {
            speedStr = "停止";
        }
        if (velocity < -0.5f)
        {
            speedStr = "后退";
        }
        panelSpeed.GetComponentInChildren<Text>().text = "<b>" + speedStr + "\n航速" + velocity.ToString() + "</b>";
        panelSpeed.GetComponentInChildren<Text>().color = new Color(0f, 1f, 0f, Mathf.Abs(throttle));

        //ind pos
        panelSpeed.transform.Find("Indicator").GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f + throttle * 0.35f);
        panelSpeed.transform.Find("Indicator").GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f + throttle * 0.35f);

        //Ind Dir
        if (throttle < 0f)
        {
            panelSpeed.transform.Find("Indicator").GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            panelSpeed.transform.Find("Indicator").GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, 0);
        }
        panelSpeed.transform.Find("Indicator").GetComponent<Image>().color = new Color(1f, 1f, 1f, Mathf.Abs(throttle));
    }


        


    void Update()
    {
        //时间显示
        if(game != null)
        {
            panelTime.GetComponentInChildren<Text>().text = "<b>" + (Mathf.RoundToInt(game.timeCountDown) / 60).ToString() + " : " + (Mathf.RoundToInt(game.timeCountDown) % 60) + "</b>";
        }


        //按钮冷却
        if(turretsPrimary.Length > 0)
        {
            float value1 = turretsPrimary.Average(t => t.reloadProgress);
            imgFillPrimary.fillAmount = 1f - value1;
            imgFillPrimary2.fillAmount = 1f - value1;
        }

        if(turretsSecondary.Length > 0)
        {
            float value2 = turretsSecondary.Average(t => t.reloadProgress);
            imgFillSeconard.fillAmount = 1f - value2;
        }

        if (turretsPrimary.Any(t => t.isLoaded && t.isRightAngle))
        {
            canfire = true;
            btnFire.GetComponent<Image>().color = Color.white;
            btnFire2.GetComponent<Image>().color = Color.white;
        }
        else
        {
            canfire = false;
            btnFire.GetComponent<Image>().color = Color.gray;
            btnFire2.GetComponent<Image>().color = Color.gray;
        }

        imgFillAb1.fillAmount = ability1.CD;
        imgFillAb2.fillAmount = ability2.CD;
        imgFillSupport.fillAmount = (game.CanCallSupport && !game.isCallingSupport) ? 0f : 1f;

        //瞄准刻度尺
        if (camSphere.IsAiming)
        {
            indAim.anchoredPosition = new Vector2(-1340f * Vector3.SignedAngle(camSphere.transform.forward, Vector3.forward, -Vector3.up) / 180f, 0f);
        }


        //lock图标跟随
        if (targetLock != null)
        {
            panelLock.GetComponent<RectTransform>().anchoredPosition = cam.WorldToViewportPoint(targetLock.transform.position) * canvasRect.sizeDelta;
            panelLock.GetComponentInChildren<Text>(true).text = targetLock.GetComponent<IShipController>().Name + "\nHP:" + Mathf.CeilToInt(targetLock.GetComponent<Hull>().HPpercent * 100) + "%";
        }


        //友军识别
        panelInfo.SetActive(false);
        if (targetAlly != null)
        {
            Vector2 screenPos = cam.WorldToViewportPoint(targetAlly.transform.position) * canvasRect.sizeDelta;
            if ((screenPos - (canvasRect.sizeDelta / 2f)).sqrMagnitude < 32400f)
            {
                panelInfo.SetActive(true);
                panelInfo.GetComponent<RectTransform>().anchoredPosition = screenPos;
                panelInfo.GetComponentInChildren<Text>().text = targetAllyName + "\nHP:" + targetAllyHpDisplay + "%";
            }
        }
    }

    //---------------------------------------------------------------------Button Callback-------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// test
    /// </summary>
    public void SetShipPos()
    {
        FindObjectOfType<Rigidbody>().transform.position = new Vector3(0, 10, 0);
    }

    //---------------------fire----------------------------------------------------
    

    /// <summary>
    /// 按键回调：玩家开火
    /// </summary>
    public void PlayerEnableFiringCurrent()
    {
        if (player != null)
        {
            player.GetComponent<PlayerController>().EnablePrimaryFiringOrder();
        }

        if (canfire)
        {
            //聚焦1秒
            //camSphere.Focus(1f);
            //开火缩圈
            LockFire();

            //震动
            Vibrate.DoVibrateArr(new long[4] { 50, 100, 50, 100 });


            if (!camSphere.IsAiming)
            {
                //镜头指向
                camSphere.RotateTo(player.GetComponent<PlayerController>().Target - player.transform.position);
                //自动跟随
                Invoke("ChaseShells", 0.1f);

                CancelInvoke("ResetChase");
                Invoke("ResetChase", 5f);
            }
        }
    }

    public void PlayerDisableFiringCurrent()
    {
        if (player != null)
        {
            player.GetComponent<PlayerController>().DisablePrimaryFiringOrder();
        }
        //camSphere.CancelFocus();
    }

    /// <summary>
    /// 按键回调： 手动瞄准
    /// </summary>
    public void PlayerSwitchAim()
    {
        camSphere.SwitchAim();//摄像机瞄准
        player.GetComponent<PlayerController>().SwitchAim();//用户瞄准（影响是否自动锁定）
    }


    //-------------------------move--------------------------------------------------
    

    /// <summary>
    /// 触控回调：前后左右
    /// </summary>
    public void PlayerSetMoveVec(Vector2 moveVec)
    {
        player.GetComponent<PlayerController>().MoveVec = moveVec;
    }


    //---------------------------item------------------------------------------------
    
    /// <summary>
    /// 按键回调：技能一
    /// </summary>
    public void PlayerAbilityOne()
    {
        //音效
        SoundPlayer.PlaySound2D("按钮");
        
        //提示
        if (ability1.CanUse)
        {
            string strAb = Utils.AbilityUse(ability1.Name);
            Displayer.Display(strAb, Color.yellow);
        }

        player.GetComponent<PlayerController>().StartCoroutine(player.GetComponent<PlayerController>().CoUseAbility(1));
    }
    /// <summary>
    /// 按键回调：技能二
    /// </summary>
    public void PlayerAbilityTwo()
    {
        //音效
        SoundPlayer.PlaySound2D("按钮");

        //提示
        if (ability2.CanUse)
        {
            string strAb = Utils.AbilityUse(ability2.Name);
            Displayer.Display(strAb, Color.yellow);
        }

        player.GetComponent<PlayerController>().StartCoroutine(player.GetComponent<PlayerController>().CoUseAbility(2));
    }

    /// <summary>
    /// 按键回调：呼叫支援
    /// </summary>
    public void PlayerCallSupport()
    {
        if (game.CanCallSupport && !game.isCallingSupport)
        {
            game.isCallingSupport = true;

            //埋点
            MaiDian.Mai("number", 5, "version", InfoManager.Version, "startVideo");

            AdManager.VideoAd(() =>
            {
                game.isCallingSupport = false;

                //埋点
                MaiDian.Mai("number", 5, "version", InfoManager.Version, "endVideo");

                game.StartCoroutine(game.CoSpawnSupport());

            }, () => {
                game.isCallingSupport = false;
            });
        }
    }


    //-----------------------Settting-----------------------------------------

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ButtonExit()
    {
        game.ExitGame();
    }


    //-----------------------Chase Ally and Leave--------------------------------------
    /// <summary>
    /// 立即结算
    /// </summary>
    public void ButtonLeave()
    {
        game.Leave();
    }




    //-------------------------------追踪--------------------------------------------------
    private AIController allyFlw = null;
    public void ChaseAnotherAllly(int i)
    {
        List<AIController> allies = FindObjectsOfType<AIController>().Where(s => s.Team == 0 && s.Enabled).ToList();

        if(allies.Count < 1)
        {
            return;
        }

        if(allyFlw == null)
        {
            ChaseAlly(allies[0]);
            return;
        }

        if (allyFlw != null && allies.Contains(allyFlw))
        {
            int index = allies.IndexOf(allyFlw);
            int nextIdx = (index + i) % allies.Count;
            if (nextIdx < 0) nextIdx += allies.Count;
            ChaseAlly(allies[nextIdx]);
        }
    }

    public void ChaseAlly(AIController ally)
    {
        if (ally != null) FindObjectOfType<CameraSphere>().ChangeFollow(ally.GameObj);
        allyFlw = ally;
    }

    public void ChaseShells()
    {
        List<GameObject> playerShells = new List<GameObject>();
        //playerShells.AddRange(FindObjectsOfType<MissileNav>().Where(s => s.shooter == player.GetComponent<IShipController>()).OrderBy(s => (s.transform.position - cam.transform.position).sqrMagnitude).Select(s => s.gameObject));
        playerShells.AddRange(FindObjectsOfType<Shell>().Where(s => s.shooter == player.GetComponent<IShipController>() && s.isSecondary == false).OrderBy(s => (s.transform.position - cam.transform.position).sqrMagnitude).Select(s => s.gameObject));

        if (playerShells.Count > 0)
        {
            StartCoroutine(CoChaseShell(playerShells[0]));
        }
    }

    public void ResetChase()
    {
        camSphere.ResetFollow();
        isChasingShell = false;
        //---
        btnLock.SetActive(false);
    }

    private IEnumerator CoChaseShell(GameObject shell)
    {
        camSphere.ChangeFollow(shell.gameObject);
        isChasingShell = true;
        //---
        btnLock.SetActive(true);


        yield return new WaitUntil(() => !(shell != null));

        yield return new WaitForSeconds(2f);

        if (isChasingShell)
        {
            camSphere.ResetFollow();
            isChasingShell = false;
            //---
            btnLock.SetActive(false);
        }
    }

    //-------------------------------UI LOCK ON-------------------------------------------
    //-------------------------------UI LOCK ON-------------------------------------------

    [HideInInspector] public GameObject targetLock = null;

    /// <summary>
    /// 锁定
    /// </summary>
    /// <param name="obj"></param>
    public void LockOn(GameObject obj)
    {
        //结束进行中动画
        panelLock.transform.DOComplete();
        panelLock.GetComponent<Image>().DOComplete();

        targetLock = obj;
        panelLock.SetActive(true);
        panelLock.transform.GetChild(0).gameObject.SetActive(false);

        panelLock.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        panelLock.GetComponent<Image>().DOColor(Color.white, 0.5f);
        panelLock.transform.localScale = new Vector3(2f, 2f, 2f);
        panelLock.transform.DOScale(Vector3.one, 0.5f).OnComplete(() => {
            panelLock.transform.GetChild(0).gameObject.SetActive(true);
        });
    }
    /// <summary>
    /// 取消锁定
    /// </summary>
    public void LockOff()
    {
        //结束进行中动画
        panelLock.transform.DOComplete();
        panelLock.GetComponent<Image>().DOComplete();

        targetLock = null;
        panelLock.transform.GetChild(0).gameObject.SetActive(false);
        panelLock.GetComponent<Image>().DOColor(new Color(1f, 1f, 1f, 0f), 0.5f);
        panelLock.transform.DOScale(new Vector3(2f, 2f, 2f), 0.5f).OnComplete(() => {
            panelLock.SetActive(false);
        });
    }
    /// <summary>
    /// 开火缩圈
    /// </summary>
    public void LockFire()
    {
        //ImageFire
        panelLock.transform.Find("ImageFire").GetComponent<RectTransform>().DOKill();
        panelLock.transform.Find("ImageFire").GetComponent<Image>().color = Color.red;
        panelLock.transform.Find("ImageFire").GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        panelLock.transform.Find("ImageFire").GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
        panelLock.transform.Find("ImageFire").GetComponent<Image>().DOFade(0f, 1.5f);
        panelLock.transform.Find("ImageFire").GetComponent<RectTransform>().DOScale(new Vector3(), 1.5f);
        panelLock.transform.Find("ImageFire").GetComponent<RectTransform>().DOLocalRotate(new Vector3(0, 0, 180f), 1.5f, RotateMode.FastBeyond360);
    }

    private GameObject targetAlly = null;
    private string targetAllyName = "";
    private string targetAllyHpDisplay = "";

    public void RepeatingIdentityAlly()
    {
        targetAlly = null;

        var controllers = FindObjectsOfType<AIController>().Where(c => c.Team == 0).OrderBy(c => Vector3.Angle(camSphere.transform.forward, (c.transform.position - camSphere.transform.position))).ToArray();
        if (controllers.Length > 0)
        {
            targetAlly = controllers[0].gameObject;
            targetAllyName = targetAlly.GetComponent<IShipController>().Name;
            targetAllyHpDisplay = Mathf.CeilToInt(targetAlly.GetComponent<Hull>().HPpercent * 100).ToString();
        }
    }

    //-----------------------------------------------------------------玩家死亡------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    public void PlayerDie()
    {
        if (this.transform.Find("UI_UserControls").gameObject.activeInHierarchy)
        {
            this.transform.Find("UI_UserControls").gameObject.SetActive(false);
        }

        this.transform.Find("UI_Die").gameObject.SetActive(true);
        this.transform.Find("UI_Map").gameObject.SetActive(false);
        this.transform.Find("ButtonSettings").gameObject.SetActive(false);
    }




    //-----------------------------------------------------------------------窗口----------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 窗口：统计窗口
    /// </summary>
    public void ShowWindowStatistics()
    {
        //音效
        SoundPlayer.PlaySound2D("按钮");


        //埋点
        MaiDian.Mai("number", 25, "version", InfoManager.Version, "goInterface");

        windowStatistics.SetActive(true);
    }
    public void HideWindowStatistics()
    {
        windowStatistics.SetActive(false);
    }

    /// <summary>
    /// 窗口：结束界面
    /// </summary>
    public void ShowWindowWinLose(int result)//win lose leave
    {
        windowWinLose.SetActive(true);

        //插屏广告
        AdManager.InterstitialOrBannerAd(null);


        //音效
        SoundPlayer.PlaySound2D("结算");

        PlayerController playerCtrlr = FindObjectOfType<PlayerController>();

        //文字
        //windowWinLose.transform.Find("TextStatistics").GetComponent<Text>().text = "<b>" + playerCtrlr.KillCount + "\n" + playerCtrlr.AssistCount + "\n" + playerCtrlr.TotalDamage.ToString("f2") + "</b>";


        //图片
        if (result == 0)
        {
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().enabled = true;
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/ImageWin");
            windowWinLose.transform.Find("ImageBottom").Find("Image00").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image00Win");
            windowWinLose.transform.Find("ImageBottom").Find("Image01").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image01Win");
            windowWinLose.transform.Find("ImageBottom").Find("Image02").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image02Win");
        }
        else if(result == 1)
        {
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().enabled = true;
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/ImageLose");
            windowWinLose.transform.Find("ImageBottom").Find("Image00").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image00Lose");
            windowWinLose.transform.Find("ImageBottom").Find("Image01").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image01Lose");
            windowWinLose.transform.Find("ImageBottom").Find("Image02").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image02Lose");
        }
        else
        {
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().enabled = false;
            windowWinLose.transform.Find("ImageWinLose").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/ImageLose");
            windowWinLose.transform.Find("ImageBottom").Find("Image00").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image00Lose");
            windowWinLose.transform.Find("ImageBottom").Find("Image01").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image01Lose");
            windowWinLose.transform.Find("ImageBottom").Find("Image02").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Settlement/Image02Lose");
        }

        //Ui usercontrols UI die 全部关闭
        this.transform.Find("UI_UserControls").gameObject.SetActive(false);
        this.transform.Find("UI_Die").gameObject.SetActive(false);
        this.transform.Find("UI_Map").gameObject.SetActive(false);
        this.transform.Find("ButtonSettings").gameObject.SetActive(false);

        //动画
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => {
            windowWinLose.transform.Find("ImageBottom").GetComponent<RectTransform>().DOAnchorMin(new Vector2(0, 0), 0.5f);
            windowWinLose.transform.Find("ImageBottom").GetComponent<RectTransform>().DOAnchorMax(new Vector2(1, 1), 0.5f);
        }).AppendInterval(0.5f).AppendCallback(() => {
            windowWinLose.transform.Find("ImageWinLose").GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);
            windowWinLose.transform.Find("ImageWinLose").GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f);//TextStatistics
        }).AppendInterval(0.5f).AppendCallback(() => {
            windowWinLose.transform.Find("TextStatistics").GetComponent<RectTransform>().DOAnchorMin(new Vector2(0.7f, 0.3f), 0.75f);
            windowWinLose.transform.Find("TextStatistics").GetComponent<RectTransform>().DOAnchorMax(new Vector2(1f, 0.6f), 0.75f).OnComplete(() => {

                DOTween.To(value => { windowWinLose.transform.Find("TextStatistics").Find("TextKill").GetComponent<Text>().text = Mathf.Floor(value).ToString(); }, startValue: 0, endValue: playerCtrlr.KillCount, duration: 1f);
                DOTween.To(value => { windowWinLose.transform.Find("TextStatistics").Find("TextAssist").GetComponent<Text>().text = Mathf.Floor(value).ToString(); }, startValue: 0, endValue: playerCtrlr.AssistCount, duration: 1f);
                DOTween.To(value => { windowWinLose.transform.Find("TextStatistics").Find("TextDamage").GetComponent<Text>().text = Mathf.Floor(value).ToString(); }, startValue: 0, endValue: Mathf.RoundToInt(playerCtrlr.TotalDamage), duration: 1f);


            });
        });

        //继续
        windowWinLose.transform.Find("ButtonContinue").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWinLose.transform.Find("ButtonContinue").GetComponent<Button>().onClick.AddListener(() =>
        {
            windowWinLose.SetActive(false);
            ShowWindowReward();
        });


        //分享
        windowWinLose.transform.Find("ButtonShare").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWinLose.transform.Find("ButtonShare").GetComponent<Button>().onClick.AddListener(() => {

            //埋点
            MaiDian.Mai("number", 6, "version", InfoManager.Version, "startVideo");

            Recorder.StopRecord((path) => { }, (code, msg) => { });
            Recorder.ShareVideo(() => {
                //埋点
                MaiDian.Mai("number", 6, "version", InfoManager.Version, "endVideo");
            }, (msg) => { }, () => { });
        });
    }

    /// <summary>
    /// 窗口：获取道具
    /// </summary>
    public void ShowWindowReward()
    {
        //音效
        SoundPlayer.PlaySound2D("奖励");


        windowReward.SetActive(true);

        Transform transPanel = windowReward.transform.Find("PanelReward").GetChild(0);
        transPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-60 * (game.rewards.Count - 1), 0);

        for(int i = 0; i < transPanel.childCount; i++)
        {
            if(i < game.rewards.Count)
            {
                transPanel.GetChild(i).gameObject.SetActive(true);
                transPanel.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + game.rewards[i].name);
                if (game.rewards[i].name == "碎片")
                {
                    transPanel.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + InfoManager.GetInstance().shipInfos[game.rewards[i].index].displayName);
                    transPanel.GetChild(i).GetComponentInChildren<Text>().text = "碎片：" + InfoManager.GetInstance().shipInfos[game.rewards[i].index].displayName;
                }
                else
                {
                    transPanel.GetChild(i).GetComponentInChildren<Text>().text = game.rewards[i].name + "×" + game.rewards[i].count;
                }
            }
            else
            {
                transPanel.GetChild(i).gameObject.SetActive(false);
            }
        }

        windowReward.transform.Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowReward.transform.Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
            game.GetAssetsReward();
            FindObjectOfType<Game>().ReturnMainMenuScene();
        });
        windowReward.transform.Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
        windowReward.transform.Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {

            //埋点
            MaiDian.Mai("number", 7, "version", InfoManager.Version, "startVideo");

            AdManager.VideoAd(() => {

                //埋点
                MaiDian.Mai("number", 7, "version", InfoManager.Version, "endVideo");

                game.GetAssetsReward();
                game.GetAssetsReward();//双倍
                FindObjectOfType<Game>().ReturnMainMenuScene();
            },() => {
                //donothing
            });
        });
    }



    //-----------------------------------------------------------------------刷新----------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 刷新：统计
    /// </summary>
    public void RefreshStatistics()
    {
        //计分板
        List<IShipController> shiplist0 = new List<IShipController>();
        shiplist0.Add(FindObjectOfType<PlayerController>());
        shiplist0.AddRange(FindObjectsOfType<AIController>().Where(s => s.Team == 0));

        List<IShipController> shiplist1 = new List<IShipController>();
        shiplist1.AddRange(FindObjectsOfType<AIController>().Where(s => s.Team == 1));

        for(int i = 0; i < windowStatistics.transform.Find("PanelTeam0").childCount; i++)
        {
            if (i < shiplist0.Count)
            {
                windowStatistics.transform.Find("PanelTeam0").GetChild(i).Find("TextName").GetComponent<Text>().text = shiplist0[i].Name;
                windowStatistics.transform.Find("PanelTeam0").GetChild(i).Find("TextKills").GetComponent<Text>().text = shiplist0[i].KillCount.ToString();
                windowStatistics.transform.Find("PanelTeam0").GetChild(i).Find("TextAssists").GetComponent<Text>().text = shiplist0[i].AssistCount.ToString();
                windowStatistics.transform.Find("PanelTeam0").GetChild(i).Find("TextStatus").GetComponent<Text>().text = shiplist0[i].Enabled ? "<color=#00ff00>存活</color>" : "<color=#ff0000>沉没</color>";
            }
            else
            {
                windowStatistics.transform.Find("PanelTeam0").GetChild(i).GetComponentsInChildren<Text>().ToList().ForEach(s => { s.text = ""; });
            }
        }
        for(int i = 0; i < windowStatistics.transform.Find("PanelTeam1").childCount; i++)
        {
            if (i < shiplist1.Count)
            {
                windowStatistics.transform.Find("PanelTeam1").GetChild(i).Find("TextName").GetComponent<Text>().text = shiplist1[i].Name;
                windowStatistics.transform.Find("PanelTeam1").GetChild(i).Find("TextKills").GetComponent<Text>().text = shiplist1[i].KillCount.ToString();
                windowStatistics.transform.Find("PanelTeam1").GetChild(i).Find("TextAssists").GetComponent<Text>().text = shiplist1[i].AssistCount.ToString();
                windowStatistics.transform.Find("PanelTeam1").GetChild(i).Find("TextStatus").GetComponent<Text>().text = shiplist1[i].Enabled ? "<color=#00ff00>存活</color>" : "<color=#ff0000>沉没</color>";
            }
            else
            {
                windowStatistics.transform.Find("PanelTeam1").GetChild(i).GetComponentsInChildren<Text>().ToList().ForEach(s => { s.text = ""; });
            }
        }

        //上方面板

        for(int i = 0; i < 5; i++)
        {
            if(i < shiplist0.Count)
            {
                if (shiplist0[i].Enabled)
                {
                    this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(0).GetChild(i).GetComponent<Image>().color = Color.white;
                }
                else
                {
                    this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(0).GetChild(i).GetComponent<Image>().color = Color.grey;
                }
            }
            else
            {
                this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(0).GetChild(i).GetComponent<Image>().color = Color.grey;
            }


            if(i < shiplist1.Count)
            {
                if (shiplist1[i].Enabled)
                {
                    this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(1).GetChild(i).GetComponent<Image>().color = Color.white;
                }
                else
                {
                    this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(1).GetChild(i).GetComponent<Image>().color = Color.grey;
                }
            }
            else
            {
                this.transform.Find("UI_MatchStuff").Find("PanelShips").GetChild(1).GetChild(i).GetComponent<Image>().color = Color.grey;
            }


        }
    }

    //-----------------------------------------------------------------------声音调节----------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// 设置声音
    /// </summary>
    /// <param name="newVolume"></param>
    public void ChangeVolume(float newVolume)
    {
        SoundPlayer.ChangeVolume(newVolume);
    }

    //==========================================Effects=======================================================

    public void HurtEffect(float value)
    {
        //Image img = effectHurt.GetComponent<Image>();
        //img.DOKill();
        //img.color = new Color(1f, 0f, 0f, Mathf.Clamp01(value * 5f) * 0.5f);
        //img.DOColor(new Color(1f, 0f, 0f, 0f), 2f);

        if(value > 0.02f)
        {
            cam.transform.DOShakeRotation(0.5f, 1f);
        }

        if(value > 0.02f)
        {
            SoundPlayer.PlaySound2D("警告音");

            //震动
            Vibrate.DoVibrate(600);
        }

        
    }
}
