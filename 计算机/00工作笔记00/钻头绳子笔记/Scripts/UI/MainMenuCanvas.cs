
#define SCRIPT_OPTION_SAVE
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuCanvas : MonoBehaviour
{
    private List<Transform> windows = new List<Transform>();
    //w1
    private Transform windowMain;
    private Transform windowShop;
    //w2
    private Transform panelUserAssets;
    //w3
    private Transform windowLogin;
    private Transform windowCheckins;
    private Transform windowRank;
    private Transform windowMissions;
    private Transform windowGot;
    private Transform windowSettings;
    //w4
    private Transform tabDrills;
    private Transform tabHooks;

    //coms
    private Transform btnBonus;

    //---- Events -----
    //event uiRx
    public UnityEvent onMoneyChange = new UnityEvent();
    public UnityEvent onCheckin = new UnityEvent();
    public UnityEvent onGetMissionBonus = new UnityEvent();
    //event statistics
    public UnityEvent onBuy = new UnityEvent();
    public UnityEvent onGetLogin = new UnityEvent();





    private void Awake()
    {
        Transform tranWindows = this.transform.Find("Windows");

        windowMain = tranWindows.Find("WindowMain");
        windowShop = tranWindows.Find("WindowShop");
        windows.Add(windowMain);
        windows.Add(windowShop);


        panelUserAssets = tranWindows.Find("PanelUserAssets");

        windowLogin = tranWindows.Find("WindowLogin");
        windowCheckins = tranWindows.Find("WindowCheckins");
        windowRank = tranWindows.Find("WindowRank");
        windowMissions = tranWindows.Find("WindowMissions");
        windowGot = tranWindows.Find("WindowGot");
        windowSettings = tranWindows.Find("WindowSettings");

        tabDrills = windowShop.Find("PanelDrills");
        tabHooks = windowShop.Find("PanelHooks");

        btnBonus = windowMain.Find("ButtonBonus");
    }
    
    void Start()
    {
        //音量应用
        SoundPlayer.ApplyVolume();

        //event listeners
        onCheckin.AddListener(() => 
        {
            RefreshCheckinCurrent();
        });

        onMoneyChange.AddListener(() =>
        {
            RefreshPanelUserAssets();

            if(windowShop.gameObject.activeInHierarchy && tabDrills.gameObject.activeInHierarchy)
            {
                RefreshTabDrills();
            }
            if (windowShop.gameObject.activeInHierarchy && tabHooks.gameObject.activeInHierarchy)
            {
                RefreshTabHooks();
            }
        });

        onGetMissionBonus.AddListener(() => {
            if (windowMissions.gameObject.activeInHierarchy)
            {
                RefreshWindowMission();
            }
        });


        //event statistics
        onBuy.AddListener(() => { Infomanager.Instance.userData.todayBuys++; });
        onGetLogin.AddListener(() => { Infomanager.Instance.userData.todayLoginBonus++; });

        //user asset set
        GetMoney(0);


        //invokes
        InvokeRepeating("RefreshBonusBtn", 0.1f, 1f);


        ToggleWindowMain();
    }

    void Update()
    {
        
    }




    ///------------------------------Toggle Window--------------------------------------
    
    private void ToggleWindow(Transform window)
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");


        foreach (Transform w in windows)
        {
            if(w != window)
            {
                w.gameObject.SetActive(false);
            }
            else
            {
                w.gameObject.SetActive(true);
            }
        }
    }

    public void ToggleWindowMain()
    {
        ToggleWindow(windowMain);

        RefreshWindowMain();

#if SCRIPT_OPTION_SAVE
        //自动存档
        Infomanager.Save();
#endif

        //弹出每日登录
        if (Infomanager.newDay)
        {
            Infomanager.newDay = false;
            ShowHideWindowLogin(true);
        }
    }
    
    public void ToggleWindowShop()
    {
        //AD
        AdManager.InterstitialOrBannerAd(null);

        ToggleWindow(windowShop);

        RefreshWindowShop();
    }

    //-------------------------------Tabs------------------------------------------------

    /// <summary>
    /// 选项卡 商店
    /// </summary>
    public void TabDrills()
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");


        tabDrills.gameObject.SetActive(true);
        tabHooks.gameObject.SetActive(false);

        RefreshTabDrills();
    }
    public void TabHooks()
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");


        tabDrills.gameObject.SetActive(false);
        tabHooks.gameObject.SetActive(true);

        RefreshTabHooks();
    }

    //------------------------------FLoat WIndows----------------------------------------------

    /// <summary>
    /// 每日登录
    /// </summary>
    public void ShowHideWindowLogin(bool show)
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {
            windowLogin.gameObject.PopActive(true);

            //refresh
            windowLogin.Find<Button>("ButtonGet1").onClick.RemoveAllListeners();
            windowLogin.Find<Button>("ButtonGet1").onClick.AddListener(() => {
                //b1
                GetMoney(200, true);

                //close
                ShowHideWindowLogin(false);

                //event
                onGetLogin.Invoke();
            });

            bool canGet = false;
            bool drillOrHook = Random.value < 0.5f;
            int equipId = 0;
            if (drillOrHook)
            {
                DrillInfo[] drillsNotHave = Infomanager.Instance.drillInfos.Where(d => !Infomanager.Instance.userData.myDrills.Contains(d.id)).ToArray();
                if(drillsNotHave.Length > 0)
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
            

            windowLogin.Find<Button>("ButtonGet2").onClick.RemoveAllListeners();
            windowLogin.Find<Button>("ButtonGet2").onClick.AddListener(() => {
                AdManager.VideoAd(() => {
                    //b1 b2
                    GetMoney(200, true);
                    if (canGet)
                    {
                        if (drillOrHook) GetDrill(equipId);
                        if (!drillOrHook) GetHook(equipId);
                    }

                    //close
                    ShowHideWindowLogin(false);

                    //event
                    onGetLogin.Invoke();
                }, () => { });
            });
        }
        else
        {
            windowLogin.gameObject.PopActive(false);
        }
    }
    
    /// <summary>
    /// 签到窗口
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowCheckins(bool show)
    {
        //AD
        AdManager.InterstitialOrBannerAd(null);

        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {
            windowCheckins.gameObject.PopActive(true);

            int dayIdx = UtilsCheckin.GetTodayIndex(Infomanager.Instance.userData.checkins1, Infomanager.Instance.userData.checkins1);
            RefreshWindowCheckins(dayIdx);

            windowCheckins.Find<Button>("ButtonLeft").onClick.RemoveAllListeners();
            windowCheckins.Find<Button>("ButtonLeft").onClick.AddListener(() => {
                RefreshCheckinPrevious();
            });
            windowCheckins.Find<Button>("ButtonRight").onClick.RemoveAllListeners();
            windowCheckins.Find<Button>("ButtonRight").onClick.AddListener(() => {
                RefreshCheckinNext();
            });


        }
        else
        {
            windowCheckins.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 荣誉窗口
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowRank(bool show)
    {
        //AD
        AdManager.InterstitialOrBannerAd(null);

        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {

            windowRank.gameObject.PopActive(true);

            RefreshWindowRank();
        }
        else
        {
            windowRank.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// 任务窗口
    /// </summary>
    public void ShowHideWindowMissions(bool show)
    {
        //AD
        AdManager.InterstitialOrBannerAd(null);

        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {

            windowMissions.gameObject.PopActive(true);

            RefreshWindowMission();
        }
        else
        {
            windowMissions.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 恭喜获得
    /// </summary>
    public void ShowHideWindowGot(bool show)
    {
        ShowHideWindowGot(show, "");
    }
    public void ShowHideWindowGot(bool show, string ItemSpritePath = null)
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");


        if (show)
        {
            windowGot.gameObject.PopActive(true);

            //Img
            if(ItemSpritePath != null)
                windowGot.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>(ItemSpritePath);
        }
        else
        {
            windowGot.gameObject.PopActive(false);
        }
    }

    /// <summary>
    /// 设置窗口
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowSettings(bool show)
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {
            windowSettings.gameObject.PopActive(true);

            RefreshSetting();
        }
        else
        {
            windowSettings.gameObject.PopActive(false);
        }
    }


    ////----------------------------Refresh---------------------------------------------------
    
    /// <summary>
    /// 刷新 主窗口
    /// </summary>
    public void RefreshWindowMain()
    {
        Debug.Log(windowMain.name);
        Debug.Log("Try Get: " + windowMain.Find<Button>("ButtonStart").name);


        //Game Start
        windowMain.Find<Button>("ButtonStart").onClick.RemoveAllListeners();
        windowMain.Find<Button>("ButtonStart").onClick.AddListener(() => {
            SceneManager.LoadScene("Level_" + Infomanager.Instance.userData.currentLevel % Infomanager.levelSceneCount);
        });

        //Checkin
        windowMain.Find<Button>("ButtonCheckin").onClick.RemoveAllListeners();
        windowMain.Find<Button>("ButtonCheckin").onClick.AddListener(() => {
            ShowHideWindowCheckins(true);
        });
        //Rank
        windowMain.Find<Button>("ButtonRank").onClick.RemoveAllListeners();
        windowMain.Find<Button>("ButtonRank").onClick.AddListener(() => {
            ShowHideWindowRank(true);
        });



        //BonusBTN
        RefreshBonusImg();
    }

    /// <summary>
    /// 刷新 资源面板
    /// </summary>
    public void RefreshPanelUserAssets()
    {
        panelUserAssets.Find<Text>("TextMoney").text = "<b>" + Infomanager.Instance.userData.money + "</b>";
    }

    /// <summary>
    /// 刷新 签到窗口
    /// </summary>
    private int currentCheckinShow = 0;
    public void RefreshCheckinPrevious()
    {
        int targetIdx = Mathf.Clamp(currentCheckinShow - 1, 0, 6);
        RefreshWindowCheckins(targetIdx);
    }
    public void RefreshCheckinNext()
    {
        int targetIdx = Mathf.Clamp(currentCheckinShow + 1, 0, 6);
        RefreshWindowCheckins(targetIdx);
    }
    public void RefreshCheckinCurrent()
    {
        RefreshWindowCheckins(currentCheckinShow);
    }
    public void RefreshWindowCheckins(int dayIdx)
    {
        currentCheckinShow = dayIdx;

        BonusInfo b1 = Infomanager.Instance.checkinBonus1[dayIdx];
        BonusInfo b2 = Infomanager.Instance.checkinBonus2[dayIdx];

        //Image day
        windowCheckins.Find<Image>("ImageDay").sprite = Resources.Load<Sprite>("Sprites/Days/" + (dayIdx + 1));

        //Image bonus
        switch (b1.type)
        {
            default:
                windowCheckins.Find("PanelBonus").Find<Image>("ImageBonus1").sprite = Resources.Load<Sprite>("Sprites/Items/coins2");
                break;
        }
        switch (b2.type)
        {
            case 1:
                windowCheckins.Find("PanelBonus").Find<Image>("ImageBonus2").sprite = Resources.Load<Sprite>("Sprites/Drills/" + b2.name);
                break;
            case 2:
                windowCheckins.Find("PanelBonus").Find<Image>("ImageBonus2").sprite = Resources.Load<Sprite>("Sprites/Hooks/" + b2.name);
                break;
            default:
                break;
        }
        //Image Got
        int getStatus = UtilsCheckin.DayIdChecked(Infomanager.Instance.userData.checkins1, Infomanager.Instance.userData.checkins2, dayIdx);
        if (getStatus > 0)
        {
            windowCheckins.Find("PanelBonus").Find("ImageBonus1").Find<Image>("ImageGot").gameObject.SetActive(true);
        }
        else
        {
            windowCheckins.Find("PanelBonus").Find("ImageBonus1").Find<Image>("ImageGot").gameObject.SetActive(false);
        }
        if (getStatus > 1)
        {
            windowCheckins.Find("PanelBonus").Find("ImageBonus2").Find<Image>("ImageGot").gameObject.SetActive(true);
        }
        else
        {
            windowCheckins.Find("PanelBonus").Find("ImageBonus2").Find<Image>("ImageGot").gameObject.SetActive(false);
        }

        //BTNs
        windowCheckins.Find<Button>("ButtonGet1").gameObject.SetActive(false);
        windowCheckins.Find<Button>("ButtonGet2").gameObject.SetActive(false);
        windowCheckins.Find<Button>("ButtonGet3").gameObject.SetActive(false);
        windowCheckins.Find<Button>("ButtonGet4").gameObject.SetActive(false);

        int today = UtilsCheckin.GetTodayIndex(Infomanager.Instance.userData.checkins1, Infomanager.Instance.userData.checkins1);
        if(today > dayIdx) //错过补领
        {
            if (getStatus == 0)
            {
                windowCheckins.Find<Button>("ButtonGet3").gameObject.SetActive(true);
                windowCheckins.Find<Button>("ButtonGet3").onClick.RemoveAllListeners();
                windowCheckins.Find<Button>("ButtonGet3").onClick.AddListener(() => {

                    //SOUND
                    SoundPlayer.PlaySound2D("click");

                    AdManager.VideoAd(() => {

                        this.GetBonus(b1);
                        this.GetBonus(b2);
                        this.CheckIn(dayIdx, true);

                    }, () => { });
                });
            }
            if (getStatus == 1)
            {
                windowCheckins.Find<Button>("ButtonGet3").gameObject.SetActive(true);
                windowCheckins.Find<Button>("ButtonGet3").onClick.RemoveAllListeners();
                windowCheckins.Find<Button>("ButtonGet3").onClick.AddListener(() => {

                    //SOUND
                    SoundPlayer.PlaySound2D("click");

                    AdManager.VideoAd(() => {
                        
                        this.GetBonus(b2);
                        this.CheckIn(dayIdx, true);

                    }, () => { });
                });
            }
        }
        if(today == dayIdx)//今天
        {
            if(getStatus == 0)
            {
                windowCheckins.Find<Button>("ButtonGet1").gameObject.SetActive(true);
                windowCheckins.Find<Button>("ButtonGet1").onClick.RemoveAllListeners();
                windowCheckins.Find<Button>("ButtonGet1").onClick.AddListener(() =>
                {
                    //SOUND
                    SoundPlayer.PlaySound2D("click");


                    this.GetBonus(b1);
                    this.CheckIn(dayIdx, false);
                });
                windowCheckins.Find<Button>("ButtonGet2").gameObject.SetActive(true);
                windowCheckins.Find<Button>("ButtonGet2").onClick.RemoveAllListeners();
                windowCheckins.Find<Button>("ButtonGet2").onClick.AddListener(() =>
                {

                    //SOUND
                    SoundPlayer.PlaySound2D("click");

                    AdManager.VideoAd(() => {
                        this.GetBonus(b1);
                        this.GetBonus(b2);
                        this.CheckIn(dayIdx, true);
                    }, () => { });
                });
            }
            if(getStatus == 1)
            {
                windowCheckins.Find<Button>("ButtonGet2").gameObject.SetActive(true);
                windowCheckins.Find<Button>("ButtonGet2").onClick.RemoveAllListeners();
                windowCheckins.Find<Button>("ButtonGet2").onClick.AddListener(() =>
                {
                    AdManager.VideoAd(() => {
                        this.GetBonus(b2);
                        this.CheckIn(dayIdx, true);
                    }, () => { });
                    
                });
            }
        }
        if(today < dayIdx) //未到时间
        {
            windowCheckins.Find<Button>("ButtonGet4").gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// 刷新 商店窗口
    /// </summary>
    public void RefreshWindowShop()
    {
        TabDrills();
        //
        windowShop.GetComponentInChildren<ToggleGroup>().transform.Find<Toggle>("ToggleDrills").onValueChanged.RemoveAllListeners();
        windowShop.GetComponentInChildren<ToggleGroup>().transform.Find<Toggle>("ToggleDrills").onValueChanged.AddListener((ck) => {
            if (ck) TabDrills();
        });
        //
        windowShop.GetComponentInChildren<ToggleGroup>().transform.Find<Toggle>("ToggleHooks").onValueChanged.RemoveAllListeners();
        windowShop.GetComponentInChildren<ToggleGroup>().transform.Find<Toggle>("ToggleHooks").onValueChanged.AddListener((ck) => {
            if (ck) TabHooks();
        });
    }
    public void RefreshTabDrills()
    {
        RectTransform transContent = tabDrills.GetComponentInChildren<ScrollRect>().content;

        //Instantiate Cells
        if(transContent.childCount == 1)
        {
            int drillCount = Infomanager.Instance.drillInfos.Length;
            transContent.sizeDelta = new Vector2(drillCount * transContent.sizeDelta.x, Mathf.CeilToInt(drillCount / 2f) * transContent.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
            for(int i = 1; i < drillCount; i++)
            {
                GameObject newCell = Instantiate(transContent.GetChild(0).gameObject, transContent.GetChild(0).transform.position, transContent.GetChild(0).transform.rotation, transContent);
                RectTransform rectCell = newCell.GetComponent<RectTransform>();
                rectCell.anchoredPosition = new Vector2((i % 2) * rectCell.sizeDelta.x, -(i / 2) * rectCell.sizeDelta.y);
            }
        }


        //Ceils
        float maxDps = Infomanager.Instance.drillInfos.Max(x => x.dps);
        float maxMaxHeat = Infomanager.Instance.drillInfos.Max(x => x.maxHeat);

        for (int i = 0; i < Infomanager.Instance.drillInfos.Length; i++)
        {
            int iTmp = i;
            DrillInfo drillInfo = Infomanager.Instance.drillInfos[iTmp];


            //Image Item
            transContent.GetChild(i).Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Drills/" + iTmp);

            //bars
            float value1 = drillInfo.dps / maxDps;
            float value2 = drillInfo.maxHeat / maxMaxHeat;
            transContent.GetChild(i).Find("BarDamage").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(value1, 1f);
            transContent.GetChild(i).Find("BarHeat").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(value2, 1f);


            //Cell BTN
            transContent.GetChild(i).Find("ButtonBuy").gameObject.SetActive(false);
            transContent.GetChild(i).Find("ButtonEquip").gameObject.SetActive(false);
            transContent.GetChild(i).Find("ButtonEquiping").gameObject.SetActive(false);
            if (Infomanager.Instance.userData.myDrills.Contains(i))
            {
                if(Infomanager.Instance.userData.activeDrill == i)
                {
                    transContent.GetChild(i).Find("ButtonEquiping").gameObject.SetActive(true);
                }
                else
                {
                    transContent.GetChild(i).Find("ButtonEquip").gameObject.SetActive(true);
                    transContent.GetChild(i).Find<Button>("ButtonEquip").onClick.RemoveAllListeners();
                    transContent.GetChild(i).Find<Button>("ButtonEquip").onClick.AddListener(() => {

                        //SOUND
                        SoundPlayer.PlaySound2D("click");

                        Infomanager.Instance.userData.activeDrill = iTmp;
                        RefreshTabDrills();
                    });
                }
            }
            else
            {
                transContent.GetChild(i).Find("ButtonBuy").gameObject.SetActive(true);
                transContent.GetChild(i).Find("ButtonBuy").GetComponentInChildren<Text>().text = "<b>" + drillInfo.price + "</b>";
                transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.RemoveAllListeners();
                if (MoneyEnough(drillInfo.price))
                {
                    transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.AddListener(() => {

                        //SOUND
                        SoundPlayer.PlaySound2D("click");


                        SpeedMoney(drillInfo.price);
                        GetDrill(iTmp);
                        //event
                        onBuy.Invoke();
                        //auto equip
                        Infomanager.Instance.userData.activeDrill = iTmp;
                        RefreshTabDrills();
                    });
                }
                else
                {
                    transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.AddListener(() => {

                        Displayer.Display("金币不足！");
                    });
                }
                    
            }
        }
    }
    public void RefreshTabHooks()
    {
        RectTransform transContent = tabHooks.GetComponentInChildren<ScrollRect>().content;

        //Instantiate Cells
        if (transContent.childCount == 1)
        {
            int hookCount = Infomanager.Instance.hookInfos.Length;
            transContent.sizeDelta = new Vector2(hookCount * transContent.sizeDelta.x, Mathf.CeilToInt(hookCount / 2f) * transContent.GetChild(0).GetComponent<RectTransform>().sizeDelta.y);
            for (int i = 1; i < hookCount; i++)
            {
                GameObject newCell = Instantiate(transContent.GetChild(0).gameObject, transContent.GetChild(0).transform.position, transContent.GetChild(0).transform.rotation, transContent);
                RectTransform rectCell = newCell.GetComponent<RectTransform>();
                rectCell.anchoredPosition = new Vector2((i % 2) * rectCell.sizeDelta.x, -(i / 2) * rectCell.sizeDelta.y);
            }
        }


        //Ceils
        float maxSpeed = Infomanager.Instance.hookInfos.Max(x => x.speed);

        for (int i = 0; i < Infomanager.Instance.drillInfos.Length; i++)
        {
            int iTmp = i;
            HookInfo hookInfo = Infomanager.Instance.hookInfos[iTmp];


            //Image Item
            transContent.GetChild(i).Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Hooks/" + iTmp);

            //bars
            float value1 = hookInfo.speed / maxSpeed;
            transContent.GetChild(i).Find("BarSpeed").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(value1, 1f);


            //Cell BTN
            transContent.GetChild(i).Find("ButtonBuy").gameObject.SetActive(false);
            transContent.GetChild(i).Find("ButtonEquip").gameObject.SetActive(false);
            transContent.GetChild(i).Find("ButtonEquiping").gameObject.SetActive(false);
            if (Infomanager.Instance.userData.myHooks.Contains(i))
            {
                if (Infomanager.Instance.userData.activeHook == i)
                {
                    transContent.GetChild(i).Find("ButtonEquiping").gameObject.SetActive(true);
                }
                else
                {
                    transContent.GetChild(i).Find("ButtonEquip").gameObject.SetActive(true);
                    transContent.GetChild(i).Find<Button>("ButtonEquip").onClick.RemoveAllListeners();
                    transContent.GetChild(i).Find<Button>("ButtonEquip").onClick.AddListener(() =>
                    {

                        //SOUND
                        SoundPlayer.PlaySound2D("click");

                        Infomanager.Instance.userData.activeHook = iTmp;
                        RefreshTabHooks();
                    });
                }
            }
            else
            {
                transContent.GetChild(i).Find("ButtonBuy").gameObject.SetActive(true);
                transContent.GetChild(i).Find("ButtonBuy").GetComponentInChildren<Text>().text = "<b>" + hookInfo.price + "</b>";
                transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.RemoveAllListeners();
                if (MoneyEnough(hookInfo.price))
                {
                    transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.AddListener(() =>
                    {
                        //SOUND
                        SoundPlayer.PlaySound2D("click");


                        SpeedMoney(hookInfo.price);
                        GetHook(iTmp);
                        //event
                        onBuy.Invoke();
                        //auto equip
                        Infomanager.Instance.userData.activeHook = iTmp;
                        RefreshTabHooks();
                    });
                }
                else
                {
                    transContent.GetChild(i).Find<Button>("ButtonBuy").onClick.AddListener(() =>
                    {
                        Displayer.Display("金币不足！");
                    });
                }

            }
        }
    }


    /// <summary>
    /// 刷新 荣誉窗口
    /// </summary>
    public void RefreshWindowRank()
    {
        //windowRank.Find<Image>("");
        int currentRank = Infomanager.Instance.userData.rank;


        //Image Rank
        windowRank.Find<Image>("ImageRank").sprite = Resources.Load<Sprite>("Sprites/Ranks/rank" + (currentRank + 1));
        windowRank.Find<Image>("ImageRankText").sprite = Resources.Load<Sprite>("Sprites/Ranks/ranktxt" + (currentRank + 1));

        //Bonus stuff
        if (currentRank < 6) //可以晋升
        {
            BonusInfo b1NextRank = Infomanager.Instance.rankBonus1[currentRank + 1];
            BonusInfo b2NextRank = Infomanager.Instance.rankBonus2[currentRank + 1];


            //Image items
            if (b2NextRank.type == 1)
            {
                windowRank.Find("PanelBonus").Find<Image>("ImageBonus2").sprite = Resources.Load<Sprite>("Sprites/Drills/" + b2NextRank.name);
            }
            else if (b2NextRank.type == 2)
            {
                windowRank.Find("PanelBonus").Find<Image>("ImageBonus2").sprite = Resources.Load<Sprite>("Sprites/Hooks/" + b2NextRank.name);
            }

            //Btn 
            windowRank.Find("ButtonUpgrade").gameObject.SetActive(true);
            windowRank.Find<Button>("ButtonUpgrade").onClick.RemoveAllListeners();
            windowRank.Find<Button>("ButtonUpgrade").onClick.AddListener(() => {

                //SOUND
                SoundPlayer.PlaySound2D("click");


                AdManager.VideoAd(() => {
                    GetBonus(b1NextRank);
                    GetBonus(b2NextRank);
                    Upgrade();
                    RefreshWindowRank();
                }, () => { });

            });

        }
        else // 最高等级
        {
            windowRank.Find("ButtonUpgrade").gameObject.SetActive(true);
            windowRank.Find("PanelBonus").Find("ImageMax").gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 刷新 任务窗口
    /// </summary>
    public void RefreshWindowMission()
    {
        Transform transContainer = windowMissions.Find("PanelContent");

        //instantiate
        if(transContainer.childCount == 1)
        {
            GameObject original = transContainer.GetChild(0).gameObject;
            for (int i = 1; i < Infomanager.Instance.missionInfos.Length; i++)
            {
                GameObject newo = Instantiate(original, original.transform.position, original.transform.rotation, original.transform.parent);
                newo.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -i * original.GetComponent<RectTransform>().sizeDelta.y);
            }
        }

        //Cell Set
        for(int i = 0; i < Infomanager.Instance.missionInfos.Length; i++)
        {
            int iTmp = i;
            Transform cell = transContainer.GetChild(i);
            MissionInfo mInfo = Infomanager.Instance.missionInfos[i];

            //text
            cell.GetComponentInChildren<Text>().text = mInfo.description;

            //BTN
            cell.Find("ButtonGet").gameObject.SetActive(false);
            cell.Find("ButtonGet").gameObject.SetActive(false);
            cell.Find("ButtonGet").gameObject.SetActive(false);
            if (Infomanager.Instance.userData.missionFinished[i] == 0)
            {
                
                if (MissionAchieved(iTmp))//领取
                {
                    cell.Find("ButtonGet").gameObject.SetActive(true);
                    cell.Find<Button>("ButtonGet").onClick.RemoveAllListeners();
                    cell.Find<Button>("ButtonGet").onClick.AddListener(() => {
                        //SOUND
                        SoundPlayer.PlaySound2D("click");

                        FinishAndGetMissionBonus(iTmp);
                    });
                }
                else//未达成
                {
                    cell.Find("ButtonNo").gameObject.SetActive(true);
                    cell.Find<Button>("ButtonNo").onClick.RemoveAllListeners();
                }
                
            }
            else//已领取
            {
                cell.Find("ButtonGot").gameObject.SetActive(true);
                cell.Find<Button>("ButtonGot").onClick.RemoveAllListeners();
            }
        }
    }
    
    /// <summary>
    /// 刷新 设置
    /// </summary>
    public void RefreshSetting()
    {
        if(SoundPlayer.Volume > 0.9)
        {
            windowSettings.Find("ButtonSound").GetChild(0).gameObject.SetActive(false);
            windowSettings.Find("ButtonSound").GetChild(1).gameObject.SetActive(true);
            windowSettings.Find("ImagesStatusSound").GetChild(0).gameObject.SetActive(false);
            windowSettings.Find("ImagesStatusSound").GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            windowSettings.Find("ButtonSound").GetChild(0).gameObject.SetActive(true);
            windowSettings.Find("ButtonSound").GetChild(1).gameObject.SetActive(false);
            windowSettings.Find("ImagesStatusSound").GetChild(0).gameObject.SetActive(true);
            windowSettings.Find("ImagesStatusSound").GetChild(1).gameObject.SetActive(false);
        }


        if (Vibrate.VibrateOn)
        {
            windowSettings.Find("ButtonVib").GetChild(0).gameObject.SetActive(false);
            windowSettings.Find("ButtonVib").GetChild(1).gameObject.SetActive(true);
            windowSettings.Find("ImagesStatusVib").GetChild(0).gameObject.SetActive(false);
            windowSettings.Find("ImagesStatusVib").GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            windowSettings.Find("ButtonVib").GetChild(0).gameObject.SetActive(true);
            windowSettings.Find("ButtonVib").GetChild(1).gameObject.SetActive(false);
            windowSettings.Find("ImagesStatusVib").GetChild(0).gameObject.SetActive(true);
            windowSettings.Find("ImagesStatusVib").GetChild(1).gameObject.SetActive(false);
        }
    }


    //-------------------------------Bonus ---------------------------------------------
    public void ReGenerateBonus()
    {
        //DEBUG
        Debug.Log("ReGen....");

        BonusTimer.inst.bonusCurrent = GenRandomBonus();

        RefreshBonusImg();
    }
    public void RefreshBonusImg()
    {
        if (BonusTimer.inst.bonusCurrent == null) return;

        if (BonusTimer.inst.bonusCurrent.type == 1)
        {
            btnBonus.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Drills/" + BonusTimer.inst.bonusCurrent.name);
        }
        else if (BonusTimer.inst.bonusCurrent.type == 2)
        {
            btnBonus.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Hooks/" + BonusTimer.inst.bonusCurrent.name);
        }
    }
    public void RefreshBonusBtn()
    {
        if(BonusTimer.inst.bonusCurrent == null)
        {
            ReGenerateBonus();
        }



        if (!Infomanager.Instance.bonusTimer.IsAvaliable) // repeating disable
        {
            btnBonus.GetChild(0).gameObject.SetActive(true);
            btnBonus.GetChild(1).gameObject.SetActive(false);

            btnBonus.GetComponent<Button>().onClick.RemoveAllListeners();

            float timerClamped = Mathf.Clamp(Infomanager.Instance.bonusTimer.RemainTime, 0f, 999f);
            btnBonus.GetComponentInChildren<Text>().text = "<b>" + Mathf.FloorToInt(timerClamped / 60f) + ": " + Mathf.FloorToInt(timerClamped % 60f) + "</b>";
        }
        else if (Infomanager.Instance.bonusTimer.IsAvaliable && !btnBonus.GetChild(1).gameObject.activeInHierarchy)  // once enable
        {
            btnBonus.GetChild(0).gameObject.SetActive(false);
            btnBonus.GetChild(1).gameObject.SetActive(true);

            btnBonus.GetComponent<Button>().onClick.RemoveAllListeners();
            btnBonus.GetComponent<Button>().onClick.AddListener(() => {

                SoundPlayer.PlaySound2D("click");

                AdManager.VideoAd(() => {
                    GetBonus(BonusTimer.inst.bonusCurrent);
                    Infomanager.Instance.bonusTimer.ResetTimer();
                    ReGenerateBonus();
                }, () => { });
            });
        }
    }


    // ----------------------------------Button Callback--------------------------------------------

    public void ButtonGetMoney()
    {
        AdManager.VideoAd(() => {
            GetMoney(500, true);
        }, () => { });
    }

    public void ButtonSwitchSound()
    {
        if(SoundPlayer.Volume > 0.9f)
        {
            SoundPlayer.ChangeVolume(0f);
            //disp
            RefreshSetting();
        }
        else
        {
            SoundPlayer.ChangeVolume(1f);
            //disp
            RefreshSetting();
        }
    }

    public void ButtonSwitchVib()
    {
        if (Vibrate.VibrateOn)
        {
            Vibrate.VibrateOn = false;

            RefreshSetting();
        }
        else
        {
            Vibrate.VibrateOn = true;

            RefreshSetting();
        }
    }

    //-------------------------------------UserAsset Operations -----------------------------------------------
    

    public void GetBonus(BonusInfo b)
    {
        if (b == null) return;


        switch (b.type)
        {
            case 0:
                {
                    GetMoney(b.amount, true);
                }
                break;
            case 1:
                {
                    int equipId;
                    int.TryParse(b.name, out equipId);
                    GetDrill(equipId);
                }
                break;
            case 2:
                {
                    int equipId;
                    int.TryParse(b.name, out equipId);
                    GetHook(equipId);
                }
                break;
            default:
                break;
        }
    }

    public void GetMoney(int amount, bool anim = false)
    {
        if(anim)
        {
            StartCoroutine(CoGetMoneyAnim(amount));
        }
        else
        {
            //Events
            Infomanager.Instance.userData.money += amount;
            onMoneyChange.Invoke();
        }

    }
    private IEnumerator CoGetMoneyAnim(int amount)
    {
        Effects.ImageBoom("Sprites/Items/coin", flyto: panelUserAssets.GetComponent<RectTransform>());

        yield return new WaitForSeconds(1f);

        //Events
        Infomanager.Instance.userData.money += amount;
        onMoneyChange.Invoke();
    }

    public bool MoneyEnough(int amount)
    {
        if (Infomanager.Instance.userData.money >= amount) return true;
        return false;
    }
    public bool SpeedMoney(int amount)
    {
        if(Infomanager.Instance.userData.money >= amount)
        {
            Infomanager.Instance.userData.money -= amount;

            //Events
            onMoneyChange.Invoke();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GetDrill(int id)
    {
        if (Infomanager.Instance.userData.myDrills.Contains(id))
        {
            //已拥有
        }
        else
        {
            Infomanager.Instance.userData.myDrills.Add(id);

            ShowHideWindowGot(true, "Sprites/Drills/" + id);
        }
    }

    public void GetHook(int id)
    {
        if (Infomanager.Instance.userData.myHooks.Contains(id))
        {
            //已拥有
        }
        else
        {
            Infomanager.Instance.userData.myHooks.Add(id);

            ShowHideWindowGot(true, "Sprites/Hooks/" + id);
        }
    }

    public BonusInfo GenRandomBonus()
    {
        BonusInfo bonus = new BonusInfo();

        bool canGet;
        bool isdrillnothook;
        int eid;
        Game.RandomEquipNotHave(out canGet, out isdrillnothook, out eid);

        bonus.id = -999;
        bonus.type = isdrillnothook ? 1 : 2;
        bonus.name = eid.ToString();
        bonus.amount = 1;

        return bonus;
    }

    public void GiveRandomBonus()
    {
        bool canGet;
        bool isdrillnothook;
        int eid;
        Game.RandomEquipNotHave(out canGet, out isdrillnothook, out eid);

        if (canGet)
        {
            if (isdrillnothook)
            {
                GetDrill(eid);
            }
            else
            {
                GetHook(eid);
            }
        }
    }

    public void CheckIn(int dayIdx, bool getAll)
    {
        UtilsCheckin.CheckIn(ref Infomanager.Instance.userData.checkins1, ref Infomanager.Instance.userData.checkins2, dayIdx, getAll);

        //Events
        onCheckin.Invoke();
    }

    public void Upgrade()
    {
        int currentRank = Infomanager.Instance.userData.rank;
        Infomanager.Instance.userData.rank = Mathf.Clamp(currentRank + 1, 0, 6);
    }


    //----------------------------Missions----------------------------

    public bool MissionAchieved(int mId)
    {
        switch (mId)
        {
            case 0:
                {
                    if (Infomanager.Instance.userData.todayKills >= 30) return true;
                    else return false;
                }
            case 1:
                {
                    if (Infomanager.Instance.userData.todayLoginBonus > 0) return true;
                    else return false;
                }
            case 2:
                {
                    if (Infomanager.Instance.userData.todayLevels >= 3) return true;
                    else return false;
                }
            case 3:
                {
                    if (Infomanager.Instance.userData.todayShares > 0) return true;
                    else return false;
                }
            case 4:
                {
                    if (Infomanager.Instance.userData.todayBuys > 0) return true;
                    else return false;
                }
            case 5:
                {
                    if (Infomanager.Instance.userData.todayGrabRockets > 10) return true;
                    else return false;
                }
            default:
                return false;
        }
    }

    public void FinishAndGetMissionBonus(int mId)
    {
        Infomanager.Instance.userData.missionFinished[mId] = 1;

        GetMoney(Infomanager.Instance.missionInfos[mId].bonusMoneyAmount, true);

        onGetMissionBonus.Invoke();
    }
}
