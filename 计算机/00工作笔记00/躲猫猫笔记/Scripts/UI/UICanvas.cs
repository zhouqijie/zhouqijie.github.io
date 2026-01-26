using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class UICanvas : MonoBehaviour
{
    [HideInInspector] public PlayerInput player;

    private IGame game;

    private CameraSphere camSphere;

    [HideInInspector] public Vector2 moveVec;

    private Vector2 outPut;

    [HideInInspector]
    public Vector2 InspectOutPut { get { return this.outPut; } }
    [HideInInspector]
    public Vector2 OutPut
    {
        get
        {
            Vector2 tmp = this.outPut;
            this.outPut = new Vector2();
            return tmp;
        }
        set
        {
            this.outPut = value;
        }
    }

    private Ability ab;



    //----windows------
    private Transform windowDisguise;
    private Transform windowWinLose;
    private Transform windowRank;
    private Transform windowShare;

    //----panels------
    private Transform panelUserStuff;
    private Transform panelAim;
    private Transform panelObserverStuff;
    private Transform hiderStuff;
    private Transform hunterStuff;
    private Transform statisticStuff;

    //---InGame eles--------
    private RectTransform hpInd;
    private RectTransform timerInd;
    private RectTransform noticeKill;

    private Transform timerStatus;

    private Text textAmmo;
    private Text textTimer;
    private Text textHidders;
    private Text textHunters;

    private Button btnAbility;

    private Image imgAbMask;
    private Image imgPosion;
    private Image[] aimImgs;


    //-----windowEles--------
    private PreviewController charactorPreivew = null;









    private void Awake()
    {
        windowDisguise = this.transform.Find("Windows").Find("WindowDisguise");
        windowWinLose = this.transform.Find("Windows").Find("WindowWinLose");
        windowRank = this.transform.Find("Windows").Find("WindowRank");
        windowShare = this.transform.Find("Windows").Find("WindowShare");

        panelUserStuff = this.transform.Find("UserStuff");
        panelAim = panelUserStuff.Find("PanelAim");
        panelObserverStuff = this.transform.Find("ObserverStuff");
        hiderStuff = this.transform.Find("UserStuff").Find("HidderStuff");
        hunterStuff = this.transform.Find("UserStuff").Find("HunterStuff");
        statisticStuff = this.transform.Find("StatisticsStuff");

        hpInd = this.transform.Find("UserStuff").Find("ProgressBarHp").GetChild(0).GetComponent<RectTransform>() ;
        timerInd = statisticStuff.Find("ProgressBarTimer").GetChild(0).GetComponent<RectTransform>();

        noticeKill = this.transform.Find("NoticesStuff").Find("ImageNoticeKill").GetComponent<RectTransform>();
        textTimer = statisticStuff.Find("TextTimer").GetComponent<Text>();
        textAmmo = this.transform.Find("UserStuff").Find("TextAmmo").GetComponent<Text>();
        textHidders = statisticStuff.Find("TextHidders").GetComponent<Text>();
        textHunters = statisticStuff.Find("TextHunters").GetComponent<Text>();

        timerStatus = statisticStuff.Find("ImageTimerBg").Find("ImagesStatus");

        btnAbility = panelUserStuff.Find("ButtonAbility").GetComponent<Button>();


        imgAbMask = btnAbility.transform.Find("Mask").GetComponent<Image>();
        imgPosion = panelUserStuff.Find("ImagePoison").GetComponent<Image>();
        aimImgs = panelAim.Find("ImageAim").GetComponentsInChildren<Image>();
    }

    void Start()
    {
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().planeDistance = 1f;

        player = FindObjectOfType<PlayerInput>();

        game = FindObjectOfType<GameStarter>().GetComponent<IGame>();

        camSphere = FindObjectOfType<CameraSphere>();

        this.GetComponent<Canvas>().worldCamera = camSphere.cam;

        //classic ui
        if(game is ClassicGame)
        {
            StartCoroutine(CoStartClassicUI());
        }
        if(game is BattleGame)
        {
            StartCoroutine(CoStartBattleUI());
        }

        //ability actived?
        ab = player.GetComponent<CharactorController>().GetActiveAbility();
        if (ab != null)
        {
            btnAbility.gameObject.SetActive(true);
            btnAbility.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Abilities/" + ab.info.name);
        }
        else
        {
            btnAbility.gameObject.SetActive(false);
        }


        //ApplySoundVolumes
        SoundPlayer.ApplyVolume();

        //startup refresh
        RefreshHpBar(1f);
        Invoke("RefreshAmmo", 0.2f);
        InvokeRepeating("RefreshTimer", 0.1f, 1f);
    }

    IEnumerator CoStartClassicUI()
    {
        //----Base set----

        //set notices
        RectTransform goal;
        RectTransform notice;
        if(player.Team == 0)
        {
            goal = this.transform.Find("NoticesStuff").Find("ImageGoalHidder").GetComponent<RectTransform>();
            notice = this.transform.Find("NoticesStuff").Find("ImageNoticeHidder").GetComponent<RectTransform>();
        }
        else
        {
            goal = this.transform.Find("NoticesStuff").Find("ImageGoalHunter").GetComponent<RectTransform>();
            notice = this.transform.Find("NoticesStuff").Find("ImageNoticeHunter").GetComponent<RectTransform>();
        }

        //set statistics
        statisticStuff.Find("ImageHidders").gameObject.SetActive(true);
        statisticStuff.Find("ImageHunters").gameObject.SetActive(true);
        textHidders.gameObject.SetActive(true);
        textHunters.gameObject.SetActive(true);
        RefreshStatistics();


        //----ANIMATE----
        goal.gameObject.SetActive(true);
        notice.gameObject.SetActive(true);

        notice.anchoredPosition = new Vector2(-1000f, 0);
        notice.DOAnchorPos(new Vector2(), 0.5f);
        
        yield return new WaitForSeconds(3f);

        notice.DOAnchorPos(new Vector2(1000f, 0), 0.5f).OnComplete(() => { notice.gameObject.SetActive(false); });


        yield return new WaitForSeconds(ClassicGame.hideTime);

        goal.GetComponent<Image>().DOFade(0f, 1f).OnComplete(() => { goal.gameObject.SetActive(false); });

    }

    IEnumerator CoStartBattleUI()
    {
        //set statistics
        statisticStuff.Find("ImageHunters").gameObject.SetActive(true);
        textHunters.gameObject.SetActive(true);
        RefreshStatistics();

        yield return null;
    }


    void Update()
    {
        //Cheat
        if(Input.GetKeyDown(KeyCode.K))
        {

            Debug.Log("Kill ALL AI");
            foreach(var chara in FindObjectsOfType<CharactorController>().Where(c => !(c.gameInput is PlayerInput)).ToArray())
            {
                chara.Damage(1000f, player.GetComponent<CharactorController>());

                Debug.Log("Kill: " + chara.gameInput.UserName);
            }
        }



        //Ability
        if (ab != null && ab.cdRemain > 0f)
        {
            imgAbMask.fillAmount = (ab.cdRemain / ab.cd) * 0.6f + 0.18f;
        }
    }




    //------------------------------------- buttons ------------------------------------

    /// <summary>
    /// MovePanel
    /// </summary>
    /// <param name="vec"></param>
    public void PlayerSetMoveVec(Vector2 vec)
    {
        moveVec = vec;
    }

    /// <summary>
    /// 按键回调开火
    /// </summary>
    /// <param name="enableOrDisable"></param>
    public void PlayerSetContantFire(bool enableOrDisable)
    {
        player.GetComponent<CharactorController>().SetConstaneFire(enableOrDisable);
    }

    /// <summary>
    /// 按键回调伪装
    /// </summary>
    public void PlayerDisguise()
    {
        if (!player.GetComponent<CharactorController>().IsDisguised)
        {
            ShowHideWindowDisguise(true);
        }
        else
        {
            player.GetComponent<CharactorController>().SwitchDisguise();
        }
    }

    /// <summary>
    /// 按键回调跳跃
    /// </summary>
    public void PlayerJump()
    {
        player.SetJump();
    }

    /// <summary>
    /// 按键回调技能
    /// </summary>
    public void PlayerUseAbility()
    {
        player.GetComponent<CharactorController>().UseAbility();
    }

    /// <summary>
    /// 按钮广告跑动
    /// </summary>
    public void PlayerRun()
    {
        AdManager.VideoAd(() => {
            player.GetComponent<CharactorController>().AdSpeedUp();
        }, () => { });
    }
    /// <summary>
    /// 按钮广告渐隐
    /// </summary>
    public void PlayerFade()
    {
        AdManager.VideoAd(() => {
            player.GetComponent<CharactorController>().AdSkinFade();
        }, () => { });
    }







    /// <summary>
    /// 按键回调观战下一个
    /// </summary>
    public void PlayerObservePrevOrNext(int i) //prev = -1 next = 1
    {
        List<GameObject> allies = FindObjectsOfType<CharactorController>().Where(c => c.gameInput.Team == player.Team && c.enabled).Select(c => c.gameObject).ToList();
        

        if (allies.Count < 1)
        {
            Debug.Log("CHASE:     <1");
            return;
        }

        if (camSphere.objFollow == null || camSphere.objFollow == player.gameObject)
        {
            Debug.Log("CHASE:     first");
            camSphere.objFollow = allies[0];
            return;
        }

        if (camSphere.objFollow != null && allies.Contains(camSphere.objFollow) && camSphere.objFollow.GetComponent<CharactorController>() != null)
        {
            Debug.Log("CHASE:     prevOrNext");

            int index = allies.IndexOf(camSphere.objFollow);
            int nextIdx = (index + i) % allies.Count;
            if (nextIdx < 0) nextIdx += allies.Count;

            camSphere.objFollow = (allies[nextIdx]);
        }
    }

    /// <summary>
    /// 按键回调强制退出
    /// </summary>
    public void PlayerExit()
    {
        game.ForceEnd();
    }

    //-------------------------------------states------------------------------------

    public void ShowHiderUI()
    {
        panelObserverStuff.gameObject.SetActive(false);
        panelUserStuff.gameObject.SetActive(true);

        hiderStuff.gameObject.SetActive(true);
        hunterStuff.gameObject.SetActive(false);
    }

    public void ShowHunterUI()
    {
        panelObserverStuff.gameObject.SetActive(false);
        panelUserStuff.gameObject.SetActive(true);

        hunterStuff.gameObject.SetActive(true);
        hiderStuff.gameObject.SetActive(false);
    }

    public void ShowObserverUI()
    {
        panelObserverStuff.gameObject.SetActive(true);
        panelUserStuff.gameObject.SetActive(false);
    }

    public void ShowShooterUI()
    {
        panelObserverStuff.gameObject.SetActive(false);
        panelUserStuff.gameObject.SetActive(true);

        hunterStuff.gameObject.SetActive(true);
        hiderStuff.gameObject.SetActive(false);
    }

    

    //------------------------------------windows------------------------------------

    /// <summary>
    /// 变身窗口
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowDisguise(bool show)
    {
        windowDisguise.gameObject.SetActive(show);

        //refresh..
        if (show)
        {
            //插屏
            AdManager.InterstitialOrBannerAd(null);
            //刷新
            RefreshWindowDisguise();
        }
    }
    private void RefreshWindowDisguise()
    {
        //---变身选项1---

        //preview
        windowDisguise.Find("PanelSelection").Find("PanelDisguise1").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + Infomanager.GetInstance().userData.activeDisguiseObj.name);
        //btn
        windowDisguise.Find("PanelSelection").Find("ButtonDisguise1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowDisguise.Find("PanelSelection").Find("ButtonDisguise1").GetComponent<Button>().onClick.AddListener(() =>
        {
            player.GetComponent<CharactorController>().SwitchDisguise(Infomanager.GetInstance().userData.activeDisguiseObj.name);

            ShowHideWindowDisguise(false);
        });


        //---变身选项2---
        DisguiseObjectInfo[] disguiseObjs2 = Infomanager.GetInstance().userData.myDisguiseObjects.Where(d => d != Infomanager.GetInstance().userData.activeDisguiseObj).ToArray();
        DisguiseObjectInfo disguise2 = disguiseObjs2[Random.Range(0, disguiseObjs2.Length)];

        //preview
        windowDisguise.Find("PanelSelection").Find("PanelDisguise2").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + disguise2.name);

        //btn
        windowDisguise.Find("PanelSelection").Find("ButtonDisguise2").GetComponent<Button>().onClick.RemoveAllListeners();
        if (Infomanager.GetInstance().userData.unlocks[0] > 0)
        {
            windowDisguise.Find("PanelSelection").Find("ButtonDisguise2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/选择");
            
            windowDisguise.Find("PanelSelection").Find("ButtonDisguise2").GetComponent<Button>().onClick.AddListener(() =>
            {

                player.GetComponent<CharactorController>().SwitchDisguise(disguise2.name);

                ShowHideWindowDisguise(false);
            });
        }
        else
        {
            windowDisguise.Find("PanelSelection").Find("ButtonDisguise2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/视频永久解锁");
            
            windowDisguise.Find("PanelSelection").Find("ButtonDisguise2").GetComponent<Button>().onClick.AddListener(() =>
            {
                //埋点
                MaiDian.Mai("number", 5, "version", Infomanager.Version, "startVideo");

                AdManager.VideoAd(() =>
                {
                    //埋点
                    MaiDian.Mai("number", 5, "version", Infomanager.Version, "endVideo");

                    Infomanager.GetInstance().userData.unlocks[0] = 1;

                    player.GetComponent<CharactorController>().SwitchDisguise(disguise2.name);

                    ShowHideWindowDisguise(false);
                }, () => { });
            });
        }


        //---变身选项3---
        DisguiseObjectInfo[] disguiseObjs3 = Infomanager.GetInstance().disguiseObjInfos.Where(d => d != Infomanager.GetInstance().userData.activeDisguiseObj).ToArray();
        DisguiseObjectInfo disguise3 = disguiseObjs3[Random.Range(0, disguiseObjs3.Length)];

        //preview
        windowDisguise.Find("PanelSelection").Find("PanelDisguise3").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + disguise3.name);

        //btn
        windowDisguise.Find("PanelSelection").Find("ButtonDisguise3").GetComponent<Button>().onClick.RemoveAllListeners();
        windowDisguise.Find("PanelSelection").Find("ButtonDisguise3").GetComponent<Button>().onClick.AddListener(() =>
        {
            //埋点
            MaiDian.Mai("number", 6, "version", Infomanager.Version, "startVideo");

            AdManager.VideoAd(() =>
            {
                //埋点
                MaiDian.Mai("number", 6, "version", Infomanager.Version, "endVideo");

                player.GetComponent<CharactorController>().SwitchDisguise(disguise3.name);

                ShowHideWindowDisguise(false);
            }, () => { });

        });
    }

    /// <summary>
    /// 结算窗口
    /// </summary>
    public void ShowWindowWinLose(bool win)
    {
        //远离战场防止出现各种音效
        camSphere.objFollow = null;
        camSphere.transform.position = new Vector3(0f, 1000f, 0f);
        camSphere.IsFollowing = false;

        //弹窗
        windowWinLose.gameObject.SetActive(true);
        RefreshWindowWinLose(win);
    }
    private void RefreshWindowWinLose(bool win)
    {
        if (win)
        {
            windowWinLose.Find("ImagesWin").gameObject.SetActive(true);
            windowWinLose.Find("ImagesLose").gameObject.SetActive(false);
            SoundPlayer.PlaySound2D("win");
        }
        else
        {
            windowWinLose.Find("ImagesWin").gameObject.SetActive(false);
            windowWinLose.Find("ImagesLose").gameObject.SetActive(true);
            SoundPlayer.PlaySound2D("lose");
        }

        //coin /diamond / effect
        int moneyAmount = game.Bonuses.FirstOrDefault(b => b.type == 0) != null ? game.Bonuses.FirstOrDefault(b => b.type == 0).amount : 0;
        windowWinLose.Find("TextCoin").GetComponent<Text>().text = "+" + moneyAmount;
        int diamondAmount = game.Bonuses.FirstOrDefault(b => b.type == 4) != null ? game.Bonuses.FirstOrDefault(b => b.type == 4).amount : 0;
        windowWinLose.Find("TextDiamond").GetComponent<Text>().text = "+" + diamondAmount;


        //text content
        if (win)
        {
            if(player.Team == 0)
            {
                windowWinLose.Find("ContentsHidderWin").GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Transform transContents = windowWinLose.Find("ContentsWin");
                transContents.GetChild(Random.Range(0, transContents.childCount)).gameObject.SetActive(true);
                transContents.GetComponentInChildren<Text>().text = player.GetComponent<CharactorController>().Kills.ToString();
            }
        }
        else
        {
            if (player.Team == 0)
            {
                windowWinLose.Find("ContentsHidderLose").GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                Transform transContents = windowWinLose.Find("ContentsLose");
                transContents.GetChild(Random.Range(0, transContents.childCount)).gameObject.SetActive(true);
                transContents.GetComponentInChildren<Text>().text = player.GetComponent<CharactorController>().Kills.ToString();
            }
        }


        //evaluations 
        if (win)
        {
            if(player.Team == 0)
            {
                Transform transEvaluations = windowWinLose.transform.Find("EvaluationsHidderWin");
                transEvaluations.GetChild(Random.Range(0, transEvaluations.childCount)).gameObject.SetActive(true);
            }
            else
            {
                Transform transEvaluations = windowWinLose.transform.Find("EvaluationsWin");
                transEvaluations.GetChild(Random.Range(0, transEvaluations.childCount)).gameObject.SetActive(true);
            }
        }
        else
        {
            Transform transEvaluations = windowWinLose.transform.Find("EvaluationsLose");
            transEvaluations.GetChild(Random.Range(0, transEvaluations.childCount)).gameObject.SetActive(true);
        }

        //preview charactor
        PreviewController charactor = PreviewController.Create(windowWinLose.Find("PanelPreview"));
        charactorPreivew = charactor;
        charactor.ResetCharactor();
        charactor.transform.localEulerAngles = new Vector3(0f, 210f, 0f);



        //badge
        RectTransform badge = windowWinLose.Find("PanelBadge").GetComponent<RectTransform>();
        
        Badge.Set(badge, Badge.levelPrevious);//左侧段位动画
        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => badge.DOAnchorPos(new Vector2(0, 0), 1f)).AppendInterval(1f).AppendCallback(() => Badge.Anim(badge)).AppendInterval(3f).AppendCallback(() => badge.DOAnchorPos(new Vector2(-400, 0), 1f));
        
        if (Badge.UserLevelGetRank(Badge.levelCurrent) > Badge.UserLevelGetRank(Badge.levelPrevious))//段位晋升窗口
        {
            ShowHideWindowRank(true);
        }



        //btn listener
        windowWinLose.Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWinLose.Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() =>
        {
            game.GetBonus(1);

            //share窗口弹出
            ShowHideWindowShare(true);
        });

        windowWinLose.Find("ButtonReturn").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWinLose.Find("ButtonReturn").GetComponent<Button>().onClick.AddListener(() =>
        {
            game.GetBonus(1);

            //share窗口弹出
            ShowHideWindowShare(true);
        });

        windowWinLose.Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWinLose.Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() =>
        {
            //埋点
            MaiDian.Mai("number", 9, "version", Infomanager.Version, "startVideo");

            AdManager.VideoAd(() => {
                //埋点
                MaiDian.Mai("number", 9, "version", Infomanager.Version, "endVideo");

                game.GetBonus(2);//2倍领取

                //share窗口弹出
                ShowHideWindowShare(true);

            }, () => { });
        });



        //Remove Try Stuff(移除试用)
        if (!Infomanager.GetInstance().userData.mySkins.Contains(Infomanager.GetInstance().userData.activeSkin))
        {
            Infomanager.GetInstance().userData.activeSkin = Infomanager.GetInstance().userData.mySkins[0];
        }
        if (Infomanager.GetInstance().userData.activeDecoration0 != null    &&     !Infomanager.GetInstance().userData.myDecorations.Where(d => d.group == "head").Contains(Infomanager.GetInstance().userData.activeDecoration0))
        {
            Infomanager.GetInstance().userData.activeDecoration0 = null;
        }
        if (Infomanager.GetInstance().userData.activeDecoration1!= null     &&     !Infomanager.GetInstance().userData.myDecorations.Where(d => d.group == "back").Contains(Infomanager.GetInstance().userData.activeDecoration1))
        {
            Infomanager.GetInstance().userData.activeDecoration1 = null;
        }
        if (Infomanager.GetInstance().userData.activeDecoration2 != null    &&     !Infomanager.GetInstance().userData.myDecorations.Where(d => d.group == "weapon").Contains(Infomanager.GetInstance().userData.activeDecoration2))
        {
            Infomanager.GetInstance().userData.activeDecoration2 = null;
        }
    }

    /// <summary>
    /// 晋级窗口
    /// </summary>
    public void ShowHideWindowRank(bool show)
    {
        if (show)
        {
            windowRank.gameObject.SetActive(true);
            windowRank.GetChild(0).localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowRank.GetChild(0).DOScale(Vector3.one, 0.5f);

            charactorPreivew.transform.localPosition = new Vector3(0, 1000f, 0);

            StartCoroutine(CoWindowRank());
        }
        else
        {
            windowRank.GetChild(0).DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f).OnComplete(() => {
                windowRank.gameObject.SetActive(false);
                charactorPreivew.transform.localPosition = new Vector3(0, 0, 0);
            });
        }
    }
    public IEnumerator CoWindowRank()
    {
        windowRank.GetChild(0).Find("ButtonOk").gameObject.SetActive(false);
        Badge.Set(windowRank.GetChild(0).Find("Badge"), Badge.levelPrevious);
        
        yield return new WaitForSeconds(1f);

        Badge.Anim(windowRank.GetChild(0).Find("Badge"));

        yield return new WaitForSeconds(0.5f);

        //fireworks
        Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Ribbons"), windowRank.transform.position, windowRank.transform.rotation, windowRank.transform);

        windowRank.GetChild(0).Find("ButtonOk").gameObject.SetActive(true);
        windowRank.GetChild(0).Find("ButtonOk").GetComponent<Button>().onClick.RemoveAllListeners();
        windowRank.GetChild(0).Find("ButtonOk").GetComponent<Button>().onClick.AddListener(() => {
            ShowHideWindowRank(false);
        });
    }


    /// <summary>
    /// 分享窗口
    /// </summary>
    public void ShowHideWindowShare(bool show)
    {
        if (show)
        {
            windowShare.gameObject.SetActive(true);

            //插屏
            AdManager.InterstitialOrBannerAd(null);

            //隐藏角色
            charactorPreivew.transform.localPosition = new Vector3(0, 1000f, 0);

            //刷新分享窗口
            StartCoroutine(CoWindowShare());
        }
        else
        {
            windowShare.gameObject.SetActive(false);
        }
    }
    private IEnumerator CoWindowShare()
    {
        windowShare.Find("ButtonShare").GetComponent<Button>().onClick.RemoveAllListeners();
        windowShare.Find("ButtonCancel").GetComponent<Button>().onClick.RemoveAllListeners();
        windowShare.Find("ButtonShare").gameObject.SetActive(false);
        windowShare.Find("ButtonCancel").gameObject.SetActive(false);

        windowShare.Find("ImagePicture").localScale = new Vector3(0.1f, 0.1f, 0.1f);
        windowShare.Find("ImagePicture").DOScale(Vector3.one, 0.5f);

        yield return new WaitForSeconds(0.5f);

        windowShare.Find("ButtonShare").gameObject.SetActive(true);
        windowShare.Find("ButtonCancel").gameObject.SetActive(true);
        windowShare.Find("ButtonShare").localScale = new Vector3(0.1f, 0.1f, 0.1f);
        windowShare.Find("ButtonCancel").localScale = new Vector3(0.1f, 0.1f, 0.1f);
        windowShare.Find("ButtonShare").DOScale(Vector3.one, 0.5f);
        windowShare.Find("ButtonCancel").DOScale(Vector3.one, 0.5f);

        yield return new WaitForSeconds(0.5f);
        

        windowShare.Find("ButtonShare").GetComponent<Button>().onClick.AddListener(() => {
            //埋点
            MaiDian.Mai("number", 10, "version", Infomanager.Version, "startVideo");

            Recorder.ShareVideo(() => {

                //埋点
                MaiDian.Mai("number", 10, "version", Infomanager.Version, "endVideo");

                //奖励
                Infomanager.GetInstance().userData.diamonds += 5;
                //返回
                game.ReturnMainMenu();//ShowHideWindowShare(false);
            }, (msg) => {
                //返回
                game.ReturnMainMenu();//ShowHideWindowShare(false);
            }, () => {
                //返回
                game.ReturnMainMenu();//ShowHideWindowShare(false);
            });

        });
        windowShare.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(() => {
            //返回
            game.ReturnMainMenu();//ShowHideWindowShare(false);
        });
    }




    //-------------------------------------Refresh----------------------------------

    public void RefreshHpBar(float value)
    {
        hpInd.anchorMax = new Vector2(value, 1f);
    }

    public void RefreshTimer()
    {
        Vector2 statusTime = game.StatusTime;
        textTimer.text = Mathf.FloorToInt(statusTime.x).ToString();

        //status
        timerStatus.GetChild(0).gameObject.SetActive(false);
        timerStatus.GetChild(1).gameObject.SetActive(false);
        timerStatus.GetChild(2).gameObject.SetActive(false);
        timerStatus.GetChild(3).gameObject.SetActive(false);
        switch (game.Status)
        {
            case 0:
                timerStatus.GetChild(0).gameObject.SetActive(true);
                break;
            case 1:
                timerStatus.GetChild(1).gameObject.SetActive(true);
                break;
            case 2:
                timerStatus.GetChild(2).gameObject.SetActive(true);
                break;
            case 3:
                timerStatus.GetChild(3).gameObject.SetActive(true);
                break;
            default:
                break;
        }

        timerInd.anchorMin = new Vector2(0.5f - (statusTime.x / statusTime.y) * 0.5f, 0);
        timerInd.anchorMax = new Vector2(0.5f + (statusTime.x / statusTime.y) * 0.5f, 1);
    }

    public void RefreshAmmo()
    {
        textAmmo.text = player.GetComponent<CharactorController>().AmmoCountStr;
    }

    public void RefreshStatistics()
    {
        if(game is ClassicGame)
        {
            int hidderCount = 0;
            int hunterCount = 0;
            hidderCount += FindObjectsOfType<AIInput0>().Where(ai => ai.Team == 0).Where(ai => ai.enabled).Count();
            hunterCount += FindObjectsOfType<AIInput0>().Where(ai => ai.Team == 1).Where(ai => ai.enabled).Count();

            if (player.Team == 0 && player.enabled) hidderCount++;
            if (player.Team == 1 && player.enabled) hunterCount++;

            textHidders.text = hidderCount.ToString();
            textHunters.text = hunterCount.ToString();
        }
        if(game is BattleGame)
        {
            int hunterCount = FindObjectsOfType<AIInput1>().Where(ai => ai.enabled).Count();
            if (player.enabled) hunterCount++;

            textHunters.text = hunterCount.ToString();
        }
    }



    //-----------------------------ANIM------------------------------------------

    public void SetPositionEffect(bool show)
    {
        Debug.Log("Player in poison");

        if (show)
        {
            imgPosion.gameObject.SetActive(true);
        }
        else
        {
            imgPosion.gameObject.SetActive(false);
        }
    }

    public void ShowKillInfo(string name)
    {
        StartCoroutine(CoShowKillInfo(name));
    }
    private IEnumerator CoShowKillInfo(string name)
    {
        if (noticeKill.gameObject.activeInHierarchy) yield break;

        noticeKill.gameObject.SetActive(true);
        noticeKill.anchoredPosition = new Vector2(0, 100f);
        noticeKill.GetComponentInChildren<Text>().text = name;

        yield return new WaitForSeconds(1f);

        noticeKill.DOAnchorPos(new Vector2(1200f, 100f), 0.5f);

        yield return new WaitForSeconds(1f);

        noticeKill.gameObject.SetActive(false);
    }


    public void ShowReloadImage(float time)
    {
        StartCoroutine(CoShowReload(time));
    }
    private IEnumerator CoShowReload(float time)
    {
        panelAim.Find("ImageAim").gameObject.SetActive(false);
        panelAim.Find("ImageReload").gameObject.SetActive(true);

        panelAim.Find("ImageReload").DOBlendableLocalRotateBy(new Vector3(0, 0, 360), time, RotateMode.LocalAxisAdd);

        yield return new WaitForSeconds(time);

        panelAim.Find("ImageAim").gameObject.SetActive(true);
        panelAim.Find("ImageReload").gameObject.SetActive(false);
    }

    public void ShowHit()
    {
        foreach(Image img in aimImgs)
        {
            img.DOKill();
            img.color = Color.black;
            img.DOColor(Color.white, 0.5f);
        }
    }
}
