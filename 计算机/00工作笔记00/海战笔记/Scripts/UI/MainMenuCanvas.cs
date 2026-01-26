using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using DG.Tweening;

public class MainMenuCanvas : MonoBehaviour
{
    private static MainMenuCanvas inst = null;

    [HideInInspector] private GameObject shipPreview = null;
    //cam pos 
    public GameObject posShipWindow;
    public GameObject posMainWindow;
    public GameObject posCrewWindow;


    private GameObject windowMain;
    private GameObject windowShips;
    private GameObject windowEquipments;
    private GameObject windowMatch;
    private GameObject windowMissions;
    private GameObject windowAchieve;
    private GameObject windowCheckin;
    private GameObject windowCrew;

    private GameObject windowActivity;
    private GameObject windowTryGetShip;
    private GameObject windowGetCrew;
    private GameObject windowDeCom;
    private GameObject windowGetMoney;
    private GameObject windowGetMat;


    private List<GameObject> windows;


    //indicator border 
    private GameObject indSelectedCom;
    private GameObject indSelectedShip;

    //tmp
    [HideInInspector] public bool isAtMainWindow;


    private int tapCountUpgradeShip = 0;
    private int tapCountUpgradeCom = 0;
    private int tapCountUpgradeCrew = 0;

















    private void Awake()
    {
        inst = this;

        InfoManager.GetInstance();

        windows = new List<GameObject>();


        //切换窗口
        windowMain = this.transform.Find("Windows").Find("WindowMain").gameObject;
        windowShips = this.transform.Find("Windows").Find("WindowShips").gameObject;
        windowEquipments = this.transform.Find("Windows").Find("WindowEquipments").gameObject;
        windowMatch = this.transform.Find("Windows").Find("WindowMatch").gameObject;
        windowMissions = this.transform.Find("Windows").Find("WindowMissions").gameObject;
        windowAchieve = this.transform.Find("Windows").Find("WindowAchieve").gameObject;
        windowCheckin = this.transform.Find("Windows").Find("WindowCheckin").gameObject;
        windowCrew = this.transform.Find("Windows").Find("WindowCrew").gameObject;

        //浮动窗口
        windowActivity = this.transform.Find("Windows").Find("WindowActivity").gameObject;
        windowTryGetShip = this.transform.Find("Windows").Find("WindowTryGetShip").gameObject;
        windowGetCrew = this.transform.Find("Windows").Find("WindowGetCrew").gameObject;
        windowDeCom = this.transform.Find("Windows").Find("WindowDeCom").gameObject;
        windowGetMoney = this.transform.Find("Windows").Find("WindowTryGetMoney").gameObject;
        windowGetMat = this.transform.Find("Windows").Find("WindowTryGetMat").gameObject;

        windows.Add(windowMain);
        windows.Add(windowShips);
        windows.Add(windowEquipments);
        windows.Add(windowMatch);
        windows.Add(windowMissions);
        windows.Add(windowAchieve);
        windows.Add(windowCheckin);
        windows.Add(windowCrew);



        //Ind
        indSelectedCom = windowEquipments.transform.Find("ImageIndicator").gameObject;
        indSelectedShip = windowShips.transform.Find("ImageIndicator").gameObject;
    }
    
    void Start()
    {
        //音量
        foreach (var asc in FindObjectsOfType<AudioSource>())
        {
            asc.volume = SoundPlayer.GetVolume();
        }

        if (InfoManager.showAchieveOnLoad)
        {
            ToggleWindowAchieve(true);
        }
        else
        {
            ToggleWindowMain();
        }

        //埋点
        MaiDian.Mai("number", 1, "version", InfoManager.Version, "goInterface");

        StartCoroutine(FpsTest());
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //爆炸测试
            //GameObject o = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Explosion"), new Vector3(), new Quaternion(), null);
            //Destroy(o, 5f);

            //获得材料
            //InfoManager.GetInstance().userData.equipmentMat += 5;
            //RefreshEquipmentInfoPanel(selectedCom, false);

            //查看总游戏数
            //Debug.Log(InfoManager.GetInstance().userData.totalGames);

            InfoManager.GetInstance().userData.achieveProgress += 10;
            RefreshWindowAchieve();
        }
    }

    //--------------------------------------FPS Test------------------------------------------------
    public IEnumerator FpsTest()
    {
        float time1 = Time.time;

        for(int i = 0; i < 60; i++)
        {
            yield return null;
        }

        float time2 = Time.time;
        float fps = 60f / (time2 - time1);

        MenuNotice.Notice("null", "FPS:" + fps.ToString("f2"), Color.gray);

        if(fps > 40f)
        {
            InfoManager.devicePerformance = 1;
        }
        else
        {
            InfoManager.devicePerformance = 0;
        }
    }

    //--------------------------------------Toggle Windows------------------------------------------
    public void ToggleWindowMain()
    {
        //---
        SaveUserData();
        //---

        //---
        isAtMainWindow = true;
        //---

        SoundPlayer.PlaySound2D("按钮");

        foreach(var w in windows)
        {
            if(w != windowMain)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        //刷新显示关卡
        windowMain.transform.Find("AccPanel").Find("LeftPanel").Find("ButtonMissions").GetComponentInChildren<Text>().text = "<b>进度：" + InfoManager.GetInstance().userData.currentLvl + " - " + (InfoManager.GetInstance().userData.currentMission + 1) + "</b>";

        //刷新显示资源
        RefreshUserAssetsPanel();

        //船只预览
        if(shipPreview == null)
        {
            RefreshShipPreview(InfoManager.GetInstance().userData.currentShip.shipInfo);
        }
        else if(InfoManager.GetInstance().userData.currentShip.shipInfo != shipPreview.GetComponent<Hull>().shipConfig.shipInfo)
        {
            RefreshShipPreview(InfoManager.GetInstance().userData.currentShip.shipInfo);
        }

        //相机补间动画
        Camera.main.transform.DOMove(posMainWindow.transform.position, 1f);
        Camera.main.transform.DORotateQuaternion(posMainWindow.transform.rotation, 1f);
    }

    public void ToggleWindowShips()
    {
        //---
        isAtMainWindow = false;
        //---


        //埋点
        MaiDian.Mai("number", 4, "version", InfoManager.Version, "goInterface");

        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowShips)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshShipsPanel();
        RefreshShipConfigPanel(InfoManager.GetInstance().userData.currentShip.shipInfo);
        RefreshShipUpgradePanel(InfoManager.GetInstance().userData.currentShip.shipInfo);


        Camera.main.transform.DOMove(posShipWindow.transform.position, 1f);
        Camera.main.transform.DORotateQuaternion(posShipWindow.transform.rotation, 1f);


        //插屏广告
        AdManager.InterstitialOrBannerAd(null);
    }

    public void ToggleWindowEquipments()
    {
        //---
        isAtMainWindow = false;
        //---

        //埋点
        MaiDian.Mai("number", 6, "version", InfoManager.Version, "goInterface");


        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowEquipments)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshEquipmentsPanel();
        RefreshEquipmentInfoPanel(null);//InfoManager.GetInstance().userData.currentShip.activeWeaponCom


        //插屏广告
        AdManager.InterstitialOrBannerAd(null);
    }

    private void ToggleWindowMatch()
    {
        //---
        isAtMainWindow = false;
        //---


        //埋点
        MaiDian.Mai("number", 2, "version", InfoManager.Version, "goInterface");

        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowMatch)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshWindowMatch();
    }

    public void ToggleWindowMissions()
    {
        ////---
        //isAtMainWindow = false;
        ////---


        //SoundPlayer.PlaySound2D("按钮");
        //foreach (var w in windows)
        //{
        //    if (w != windowMissions)
        //    {
        //        w.SetActive(false);
        //    }
        //    else
        //    {
        //        w.SetActive(true);
        //    }
        //}

        //SetMissionType(0);
    }

    public void ToggleWindowAchieve(bool toIndicatePos = false)
    {
        //---
        isAtMainWindow = false;
        //---


        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowAchieve)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshWindowAchieve(toIndicatePos);


        //插屏广告
        AdManager.InterstitialOrBannerAd(null);
    }

    public void ToggleWindowCheckin()
    {
        //---
        isAtMainWindow = false;
        //---


        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowCheckin)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshWindowCheckin();
    }

    public void ToggleWindowCrew()
    {
        //---
        isAtMainWindow = false;
        //---

        //埋点
        MaiDian.Mai("number", 5, "version", InfoManager.Version, "goInterface");

        SoundPlayer.PlaySound2D("按钮");
        foreach (var w in windows)
        {
            if (w != windowCrew)
            {
                w.SetActive(false);
            }
            else
            {
                w.SetActive(true);
            }
        }

        RefreshWindowCrew();


        Camera.main.transform.DOMove(posCrewWindow.transform.position, 1f);
        Camera.main.transform.DORotateQuaternion(posCrewWindow.transform.rotation, 1f);


        //插屏广告
        AdManager.InterstitialOrBannerAd(null);
    }


    //--------------------------------------Show Windows--------------------------------------------

    public void ShowWindowActivity()
    {
        SoundPlayer.PlaySound2D("按钮");
        windowActivity.gameObject.SetActive(true);

        windowActivity.transform.Find("ButtonShowTryGet").GetComponent<Button>().onClick.RemoveAllListeners();
        windowActivity.transform.Find("ButtonShowTryGet").GetComponent<Button>().onClick.AddListener(() => {
            windowActivity.gameObject.SetActive(false);
            ShowWindowTryGetShip();
        });
    }

    public void ShowWindowTryGetShip()
    {
        SoundPlayer.PlaySound2D("按钮");

        windowTryGetShip.gameObject.SetActive(true);

        //领取
        ShipInfo shipinfo = InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.displayName == "波塞冬");

        windowTryGetShip.transform.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();
        windowTryGetShip.transform.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {
            if(!(InfoManager.GetInstance().userData.userShips.FirstOrDefault(s => s.shipInfo == shipinfo) != null))
            {

                //埋点
                MaiDian.Mai("number", 4, "version", InfoManager.Version, "startVideo");

                AdManager.VideoAd(() => {

                    //埋点
                    MaiDian.Mai("number", 4, "version", InfoManager.Version, "endVideo");

                    //音效
                    SoundPlayer.PlaySound2D("奖励");

                    //Fireworks
                    GameObject fireworks = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/Fireworks"), this.transform.position, Quaternion.LookRotation(this.transform.up), this.transform);
                    Destroy(fireworks, 6f);

                    TryUnLockShip(shipinfo);
                    RefreshShipPreview(shipinfo);
                    windowTryGetShip.gameObject.SetActive(false);
                }, () => { });
            }
            else
            {
                MenuNotice.Notice("Error", "已拥有战舰！", Color.red);
            }
        });

        //留给对手
        windowTryGetShip.transform.Find("ButtonCancel").GetComponent<Button>().onClick.RemoveAllListeners();
        windowTryGetShip.transform.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(() => {
            windowTryGetShip.gameObject.SetActive(false);
        });
    }

    public void ShowWindowHire(bool isDec)
    {

        SoundPlayer.PlaySound2D("按钮");

        windowGetCrew.gameObject.SetActive(true);

        if (!isDec)//====单抽====
        {
            windowGetCrew.transform.Find("PanelGet1").gameObject.SetActive(true);
            windowGetCrew.transform.Find("PanelGet10").gameObject.SetActive(false);

            //随机船员
            CrewInfo crewinfo = RandomCrew();


            //头像
            windowGetCrew.transform.Find("PanelGet1").Find("ImagePic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + crewinfo.name);
            windowGetCrew.transform.Find("PanelGet1").Find("ImageBottom").GetComponent<Image>().color = Utils.RankColor(crewinfo.rank);

            if (InfoManager.GetInstance().userData.crews.Any(c => c.crewInfo == crewinfo))
            {
                InfoManager.GetInstance().userData.crews.FirstOrDefault(c => c.crewInfo == crewinfo).lvl = Mathf.Clamp(InfoManager.GetInstance().userData.crews.FirstOrDefault(c => c.crewInfo == crewinfo).lvl + 1, 1, 30);
                //介绍
                windowGetCrew.transform.Find("PanelGet1").Find("TextIntroduce").GetComponent<Text>().text = "获得船员:" + crewinfo.name + "（已拥有）";
                //notice
                MenuNotice.Notice(crewinfo.name, "船员已升级", Color.green);
            }
            else
            {
                Crew crew = new Crew();
                crew.crewInfo = crewinfo;
                crew.lvl = 1;
                InfoManager.GetInstance().userData.crews.Add(crew);
                //介绍
                windowGetCrew.transform.Find("PanelGet1").Find("TextIntroduce").GetComponent<Text>().text = "获得船员:" + crewinfo.name;
                //notice
                MenuNotice.Notice(crewinfo.name, "船员已招募", Color.green);
            }
        }
        else//====十连====
        {
            windowGetCrew.transform.Find("PanelGet1").gameObject.SetActive(false);
            windowGetCrew.transform.Find("PanelGet10").gameObject.SetActive(true);

            for (int i = 0; i < 10; i++)
            {

                //随机船员
                CrewInfo crewinfo;
                if (i == 0)//必出A
                {
                    var rankAcrews = InfoManager.GetInstance().crewInfos.Where(c => Utils.ToRankLvl(c.rank) == 3).ToArray();
                    crewinfo = rankAcrews[Random.Range(0, rankAcrews.Length)];
                }
                else
                {
                    crewinfo = RandomCrew();
                }


                //头像
                windowGetCrew.transform.Find("PanelGet10").GetChild(i).Find("ImagePic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + crewinfo.name);
                windowGetCrew.transform.Find("PanelGet10").GetChild(i).Find("ImageBottom").GetComponent<Image>().color = Utils.RankColor(crewinfo.rank);

                if (InfoManager.GetInstance().userData.crews.Any(c => c.crewInfo == crewinfo))
                {
                    InfoManager.GetInstance().userData.crews.FirstOrDefault(c => c.crewInfo == crewinfo).lvl = Mathf.Clamp(InfoManager.GetInstance().userData.crews.FirstOrDefault(c => c.crewInfo == crewinfo).lvl + 1, 1, 30);
                    //介绍
                    windowGetCrew.transform.Find("PanelGet10").GetChild(i).Find("TextIntroduce").GetComponent<Text>().text = "获得船员:" + crewinfo.name + "（已拥有）";
                    //notice
                    MenuNotice.Notice(crewinfo.name, "船员已升级", Color.green);
                }
                else
                {
                    Crew crew = new Crew();
                    crew.crewInfo = crewinfo;
                    crew.lvl = 1;
                    InfoManager.GetInstance().userData.crews.Add(crew);
                    //介绍
                    windowGetCrew.transform.Find("PanelGet10").GetChild(i).Find("TextIntroduce").GetComponent<Text>().text = "获得船员:" + crewinfo.name;
                    //notice
                    MenuNotice.Notice(crewinfo.name, "船员已招募", Color.green);
                }
            }
        }
        


        //关闭
        windowGetCrew.transform.Find("ButtonClose").GetComponent<Button>().onClick.RemoveAllListeners();
        windowGetCrew.transform.Find("ButtonClose").GetComponent<Button>().onClick.AddListener(() => {
            windowGetCrew.SetActive(false);
            RefreshWindowCrew();
        });


    }
    public CrewInfo RandomCrew()
    {
        float rdm = Random.value;
        int rdmlvl;
        if (rdm < 0.02f)
        {
            rdmlvl = 4;
        }
        else if (rdm < 0.1f)
        {
            rdmlvl = 3;
        }
        else if (rdm < 0.35f)
        {
            rdmlvl = 2;
        }
        else
        {
            rdmlvl = 1;
        }
        CrewInfo[] crewInfos = InfoManager.GetInstance().crewInfos.Where(c => Utils.ToRankLvl(c.rank) == rdmlvl).ToArray();
        CrewInfo crewinfo = crewInfos[Random.Range(0, crewInfos.Length)];
        return crewinfo;
    }

    public void ShowWindowDeCom(ShipCom com)
    {
        windowDeCom.SetActive(true);

        //Button listener
        windowDeCom.transform.Find("ButtonDeCom1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowDeCom.transform.Find("ButtonDeCom1").GetComponent<Button>().onClick.AddListener(() => {
            TryDecomposeCom(com, false);
            RefreshEquipmentInfoPanel(null);
            windowDeCom.SetActive(false);
            RefreshEquipmentsPanel();
        });

        windowDeCom.transform.Find("ButtonDeCom2").GetComponent<Button>().onClick.RemoveAllListeners();
        windowDeCom.transform.Find("ButtonDeCom2").GetComponent<Button>().onClick.AddListener(() => {
            AdManager.VideoAd(() => {
                TryDecomposeCom(com, true);
                RefreshEquipmentInfoPanel(null);
                windowDeCom.SetActive(false);
                RefreshEquipmentsPanel();
            },() => {
                //..donothing
            });
        });
    }

    public void ShowWindowGetMoney()
    {
        windowGetMoney.SetActive(true);

        int amount = Mathf.RoundToInt(Utils.CalFac(InfoManager.GetInstance().userData.userShips.Max(s => s.lvl))) * 500;

        windowGetMoney.transform.Find("TextAmount").GetComponent<Text>().text = amount.ToString();

        windowGetMoney.transform.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();
        windowGetMoney.transform.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {

            //埋点
            MaiDian.Mai("number", 1, "version", InfoManager.Version, "startVideo");

            AdManager.VideoAd(() => {
                //埋点
                MaiDian.Mai("number", 1, "version", InfoManager.Version, "endVideo");

                GiveMoney(amount);
                windowGetMoney.SetActive(false);
            }, () => {

            });
        });
    }

    public void ShowWindowGetMat()
    {
        windowGetMat.SetActive(true);

        int amount = Mathf.RoundToInt(Utils.CalFac(InfoManager.GetInstance().userData.userShips.Max(s => s.lvl))) * 5;

        windowGetMat.transform.Find("TextAmount").GetComponent<Text>().text = amount.ToString();

        windowGetMat.transform.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();
        windowGetMat.transform.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {

            //埋点
            MaiDian.Mai("number", 3, "version", InfoManager.Version, "startVideo");

            AdManager.VideoAd(() => {

                //埋点
                MaiDian.Mai("number", 3, "version", InfoManager.Version, "endVideo");

                GiveMaterial(amount);
                windowGetMat.SetActive(false);
            }, () => {

            });
        });
    }

    //--------------------------------------Refresh Windows----------------------------------------


    /// <summary>
    /// 刷新：用户资产
    /// </summary>
    public void RefreshUserAssetsPanel()
    {
        InfoManager info = InfoManager.GetInstance();
        Transform userAssetsPanel = windowMain.transform.Find("AccPanel").Find("LeftPanel").Find("UserAssetsPanel");

        userAssetsPanel.Find("TextMoney").GetComponent<Text>().text = "" + info.userData.money.ToString();
        userAssetsPanel.Find("TextEquipmentMats").GetComponent<Text>().text = "" + info.userData.equipmentMat.ToString();
    }

    /// <summary>
    /// 刷新：战舰列表
    /// </summary>
    public void RefreshShipsPanel()
    {
        Transform transShipsPanel = windowShips.transform.Find("ShipsPanel");
        List<ShipInfo> shipInfos = InfoManager.GetInstance().shipInfos.Where(s => s.get == 1 || s.get == 2).ToList();
        

        transShipsPanel.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().sizeDelta = new Vector2(transShipsPanel.GetComponent<ScrollRect>().content.GetComponent<RectTransform>().sizeDelta.x, shipInfos.Count * 140);

        for (int i = 0; i < transShipsPanel.GetComponent<ScrollRect>().content.childCount; i++)
        {
            if (i < shipInfos.Count)
            {
                transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).gameObject.SetActive(true);

                //名称显示
                transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponentInChildren<Text>().text = shipInfos[i].displayName;

                //图片显示
                transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + shipInfos[i].displayName);


                //框显示
                if (InfoManager.GetInstance().userData.currentShip.shipInfo == shipInfos[i])
                {
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).Find("ImageBorder").GetComponentInChildren<Image>().enabled = true;
                }
                else
                {
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).Find("ImageBorder").GetComponentInChildren<Image>().enabled = false;
                }

                if (InfoManager.GetInstance().userData.userShips.Select(s => s.shipInfo).Contains(shipInfos[i]))//拥有
                {
                    //颜色
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponentInChildren<Text>().color = Color.white;
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Image>().color = Color.white;
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).Find("Image").GetComponent<Image>().color = Color.white;

                    int indexTmp = i;
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                        //音效
                        SoundPlayer.PlaySound2D("按钮");
                        

                        RefreshShipConfigPanel(shipInfos[indexTmp]);
                        RefreshShipUpgradePanel(shipInfos[indexTmp]);
                        SetCurrent(shipInfos[indexTmp]);
                        RefreshShipsPanel();

                        //弹出提示
                        MenuNotice.Notice(shipInfos[indexTmp].displayName, "出击战舰：" + shipInfos[indexTmp].displayName, Color.green);

                        //预览战舰
                        RefreshShipPreview(shipInfos[indexTmp]);
                    });

                }
                else//未拥有
                {
                    //颜色
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponentInChildren<Text>().color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1f);
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).Find("Image").GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1f);

                    int indexTmp = i;
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                        RefreshShipConfigPanel(shipInfos[indexTmp]);
                        RefreshShipUpgradePanel(shipInfos[indexTmp]);
                        RefreshShipsPanel();

                        //预览战舰
                        RefreshShipPreview(shipInfos[indexTmp]);
                    });
                }
            }
            else
            {
                transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).gameObject.SetActive(false);

                transShipsPanel.GetComponent<ScrollRect>().content.GetChild(i).GetComponentInChildren<Text>().text = "";
            }
            

        }
    }

    private void MoveShipInd(Transform trans)
    {
        indSelectedShip.transform.DOKill();

        indSelectedShip.transform.parent = trans;//可以直接穿过其他
        indSelectedShip.transform.DOLocalMove(new Vector3(), 0.25f).OnComplete(() => { indSelectedShip.transform.localPosition = new Vector3(); });
    }

    /// <summary>
    /// 刷新：战舰配置面板
    /// </summary>
    /// <param name="shipInfo"></param>
    public void RefreshShipConfigPanel(ShipInfo shipInfo)
    {
        ShipConfig userShipConfig = InfoManager.GetInstance().userData.userShips.FirstOrDefault(s => s.shipInfo == shipInfo);

        Transform transShipConfigPanel = windowShips.transform.Find("ShipConfigPanel");
        Transform transNoShipPanel = windowShips.transform.Find("NoShipPanel");

        //移动游标
        int itmp = InfoManager.GetInstance().shipInfos.IndexOf(shipInfo);
        MoveShipInd(windowShips.transform.Find("ShipsPanel").GetComponent<ScrollRect>().content.GetChild(itmp));

        //名称
        transShipConfigPanel.Find("TextTitle").GetComponent<Text>().text = shipInfo.displayName;

        if (userShipConfig != null)
        {
            //未获得战舰黑边
            transNoShipPanel.gameObject.SetActive(false);

            if (userShipConfig.activeWeaponCom != null)
            {
                transShipConfigPanel.Find("ImageWeapon").GetComponentInChildren<Text>().text = userShipConfig.activeWeaponCom.weaponInfo.displayName;
                transShipConfigPanel.Find("ImageWeapon").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userShipConfig.activeWeaponCom.weaponInfo.displayName);
                transShipConfigPanel.Find("ImageWeapon").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            else
            {
                transShipConfigPanel.Find("ImageWeapon").GetComponentInChildren<Text>().text = "空";
                transShipConfigPanel.Find("ImageWeapon").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
                transShipConfigPanel.Find("ImageWeapon").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            if (userShipConfig.activeArmorCom != null)
            {
                transShipConfigPanel.Find("ImageArmor").GetComponentInChildren<Text>().text = userShipConfig.activeArmorCom.armorInfo.displayName;
                transShipConfigPanel.Find("ImageArmor").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userShipConfig.activeArmorCom.armorInfo.displayName);
                transShipConfigPanel.Find("ImageArmor").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            else
            {
                transShipConfigPanel.Find("ImageArmor").GetComponentInChildren<Text>().text = "空";
                transShipConfigPanel.Find("ImageArmor").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
                transShipConfigPanel.Find("ImageArmor").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            if (userShipConfig.activeEngineCom != null)
            {
                transShipConfigPanel.Find("ImageEngine").GetComponentInChildren<Text>().text = userShipConfig.activeEngineCom.engineInfo.displayName;
                transShipConfigPanel.Find("ImageEngine").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userShipConfig.activeEngineCom.engineInfo.displayName);
                transShipConfigPanel.Find("ImageEngine").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            else
            {
                transShipConfigPanel.Find("ImageEngine").GetComponentInChildren<Text>().text = "空";
                transShipConfigPanel.Find("ImageEngine").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
                transShipConfigPanel.Find("ImageEngine").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            if (userShipConfig.crew != null)
            {
                transShipConfigPanel.Find("ImageCrew").GetComponentInChildren<Text>().text = userShipConfig.crew.crewInfo.name;
                transShipConfigPanel.Find("ImageCrew").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userShipConfig.crew.crewInfo.name);
            }
            else
            {
                transShipConfigPanel.Find("ImageCrew").GetComponentInChildren<Text>().text = "空";
                transShipConfigPanel.Find("ImageCrew").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
            }
        }
        else
        {
            //未获得战舰黑边
            transNoShipPanel.gameObject.SetActive(true);

            transShipConfigPanel.Find("ImageWeapon").GetComponentInChildren<Text>().text = "？？？";
            transShipConfigPanel.Find("ImageWeapon").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
            transShipConfigPanel.Find("ImageArmor").GetComponentInChildren<Text>().text = "？？？";
            transShipConfigPanel.Find("ImageArmor").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
            transShipConfigPanel.Find("ImageEngine").GetComponentInChildren<Text>().text = "？？？";
            transShipConfigPanel.Find("ImageEngine").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
            transShipConfigPanel.Find("ImageCrew").GetComponentInChildren<Text>().text = "？？？";
            transShipConfigPanel.Find("ImageCrew").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
        }
    }

    /// <summary>
    /// 刷新：战舰解锁/升级面板
    /// </summary>
    /// <param name="shipInfo"></param>
    public void RefreshShipUpgradePanel(ShipInfo shipInfo)
    {
        ShipConfig userShipConfig = InfoManager.GetInstance().userData.userShips.FirstOrDefault(s => s.shipInfo == shipInfo);
        Transform transShipUpgradePanel = windowShips.transform.Find("ShipUpgradePanel");
        Transform transShipUnlockPanel = windowShips.transform.Find("ShipUnlockPanel");

        //名称
        transShipUpgradePanel.Find("TextTitle").GetComponent<Text>().text = "<b>" + shipInfo.displayName + "</b>";

        //Rank显示
        transShipUpgradePanel.Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rank/" + shipInfo.rank.ToUpper());
        transShipUnlockPanel.Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rank/" + shipInfo.rank.ToUpper());

        //详情（已有船只）
        if (userShipConfig != null)
        {
            transShipUpgradePanel.gameObject.SetActive(true);
            transShipUnlockPanel.gameObject.SetActive(false);

            transShipUpgradePanel.Find("TextLvl").GetComponent<Text>().text = "<size=12>等级</size><size=24>" + userShipConfig.lvl + "</size>";

            //攻击防御速度条
            float maxAttack = InfoManager.GetInstance().shipInfos.Max(s => s.attack) * Utils.CalFac(30);
            float maxDefense = InfoManager.GetInstance().shipInfos.Max(s => s.defense) * Utils.CalFac(30);
            float maxSpeed = InfoManager.GetInstance().shipInfos.Max(s => s.speed) * Utils.CalFac(30);
            float maxHp = InfoManager.GetInstance().shipInfos.Max(s => s.hp) * Utils.CalFac(30);

            float attackCurrent = userShipConfig.shipInfo.attack * Utils.CalFac(userShipConfig.lvl); float attackNext = userShipConfig.shipInfo.attack * Utils.CalFac(userShipConfig.lvl + 1);
            float defenseCurrent = userShipConfig.shipInfo.defense * Utils.CalFac(userShipConfig.lvl); float defenseNext = userShipConfig.shipInfo.defense * Utils.CalFac(userShipConfig.lvl + 1);
            float speedCurrent = userShipConfig.shipInfo.speed * Utils.CalFac(userShipConfig.lvl); float speedNext = userShipConfig.shipInfo.speed * Utils.CalFac(userShipConfig.lvl + 1);
            float hpCurrent = userShipConfig.shipInfo.hp * Utils.CalFac(userShipConfig.lvl); float hpNext = userShipConfig.shipInfo.hp * Utils.CalFac(userShipConfig.lvl + 1);

            transShipUpgradePanel.Find("PanelWeapon").GetComponentInChildren<Text>().text = Mathf.RoundToInt(attackCurrent).ToString();
            transShipUpgradePanel.Find("PanelWeapon").Find("ProgressBar").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2( attackNext / maxAttack, 1f);
            transShipUpgradePanel.Find("PanelWeapon").Find("ProgressBar").GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2( attackCurrent / maxAttack, 1f);

            transShipUpgradePanel.Find("PanelArmor").GetComponentInChildren<Text>().text = Mathf.RoundToInt(defenseCurrent).ToString();
            transShipUpgradePanel.Find("PanelArmor").Find("ProgressBar").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(defenseNext / maxDefense, 1f);
            transShipUpgradePanel.Find("PanelArmor").Find("ProgressBar").GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2(defenseCurrent / maxDefense, 1f);

            transShipUpgradePanel.Find("PanelHp").GetComponentInChildren<Text>().text = Mathf.RoundToInt(hpCurrent).ToString();
            transShipUpgradePanel.Find("PanelHp").Find("ProgressBar").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(hpNext / maxHp, 1f);
            transShipUpgradePanel.Find("PanelHp").Find("ProgressBar").GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2(hpCurrent / maxHp, 1f);

            //技能介绍
            Ability ab1 = Resources.Load<GameObject>("Prefabs/Ships/" + userShipConfig.shipInfo.displayName).GetComponents<Ability>().FirstOrDefault(a => a.Index == 1);
            Ability ab2 = Resources.Load<GameObject>("Prefabs/Ships/" + userShipConfig.shipInfo.displayName).GetComponents<Ability>().FirstOrDefault(a => a.Index == 2);
            string ab1str = ab1.Name;
            string ab2str = ab2.Name;
            transShipUpgradePanel.Find("ImageAbility1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + ab1str);
            transShipUpgradePanel.Find("ImageAbility1").GetComponentInChildren<Text>().text = "<color=#FFFF00>" + ab1str + "</color>(技能一)\n" + Utils.AbilityIntroduce(ab1str);
            transShipUpgradePanel.Find("ImageAbility2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + ab2str);
            if(ab2 is AbilityEmpty)
            {
                transShipUpgradePanel.Find("ImageAbility2").GetComponentInChildren<Text>().text = "";
            }
            else
            {
                transShipUpgradePanel.Find("ImageAbility2").GetComponentInChildren<Text>().text = "<color=#FFFF00>" + ab2str + "</color>(技能二)\n" + Utils.AbilityIntroduce(ab2str);
            }

            //按键
            if (userShipConfig.lvl < 30)
            {
                transShipUpgradePanel.Find("TextStatic").gameObject.SetActive(true);
                transShipUpgradePanel.Find("TextCost").gameObject.SetActive(true);
                transShipUpgradePanel.Find("ImageCost").gameObject.SetActive(true);
            }
            else
            {
                transShipUpgradePanel.Find("TextStatic").gameObject.SetActive(false);
                transShipUpgradePanel.Find("TextCost").gameObject.SetActive(false);
                transShipUpgradePanel.Find("ImageCost").gameObject.SetActive(false);
            }

            if (Utils.CalShipUpgradeCost(userShipConfig.lvl, shipInfo.ranklvl) <= InfoManager.GetInstance().userData.money && userShipConfig.lvl < 30)//金币足够升级且小于30级
            {

                transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
                transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");

                transShipUpgradePanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                transShipUpgradePanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
                    //音效
                    SoundPlayer.PlaySound2D("升级");

                    TryUpgradeCurrentShip();
                    RefreshShipUpgradePanel(shipInfo);
                    //notice
                    MenuNotice.Notice(shipInfo.displayName, "战舰 " + shipInfo.displayName + " 已升级", Color.green);
                });


                //消耗绿色显示
                transShipUpgradePanel.Find("TextCost").GetComponent<Text>().text = "<color=#00FF00>" + Utils.CalShipUpgradeCost(userShipConfig.lvl, shipInfo.ranklvl).ToString() + "</color>/" + InfoManager.GetInstance().userData.money;
            }
            else//金币不足升级或者30级
            {
                transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.gray;
                transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");

                transShipUpgradePanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                if(userShipConfig.lvl < 30)
                {
                    if (tapCountUpgradeShip > 4)
                    {
                        tapCountUpgradeShip = 0;
                        transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
                        transShipUpgradePanel.GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/广告升级");
                        transShipUpgradePanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
                            AdManager.VideoAd(() => {
                                TryUpgradeCurrentShip(true);//true - for free
                                RefreshShipUpgradePanel(shipInfo);
                            }, () => { });
                        });

                    }
                    else
                    {
                        transShipUpgradePanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
                            MenuNotice.Notice("Error", "金币不足！", Color.red);
                            tapCountUpgradeShip += 1;
                            RefreshShipUpgradePanel(shipInfo);
                        });
                    }
                }
                
                //消耗红色显示
                transShipUpgradePanel.Find("TextCost").GetComponent<Text>().text = "<color=#FF0000>" + (userShipConfig.lvl < 30 ? Utils.CalShipUpgradeCost(userShipConfig.lvl, shipInfo.ranklvl).ToString() : "已满级") + "</color>/" + InfoManager.GetInstance().userData.money;
            }
        }
        else if(shipInfo.get == 1)//（未解锁战舰）
        {
            transShipUnlockPanel.gameObject.SetActive(true);
            transShipUpgradePanel.gameObject.SetActive(false);
            transShipUnlockPanel.Find("PanelUnlock").gameObject.SetActive(true);
            transShipUnlockPanel.Find("PanelActivity").gameObject.SetActive(false);

            transShipUnlockPanel.Find("PanelUnlock").Find("ImageDebris").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + shipInfo.displayName);

            transShipUnlockPanel.Find("PanelUnlock").Find("TextInfo").GetComponent<Text>().text = "当前拥有碎片：" + InfoManager.GetInstance().userData.debris[InfoManager.debrisIndexOfShipInfo[shipInfo.id]];
            transShipUnlockPanel.Find("PanelUnlock").Find("TextCost").GetComponent<Text>().text = "<b>×" + Utils.CalShipUnlockCost(shipInfo) + "</b>";


            if (Utils.CalShipUnlockCost(shipInfo) <= InfoManager.GetInstance().userData.debris[InfoManager.debrisIndexOfShipInfo[shipInfo.id]])//可解锁
            {

                transShipUnlockPanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;

                transShipUnlockPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                transShipUnlockPanel.GetComponentInChildren<Button>().onClick.AddListener(() => {
                    //音效
                    SoundPlayer.PlaySound2D("奖励");

                    TryUnLockShip(shipInfo);
                    RefreshShipConfigPanel(shipInfo);
                    RefreshShipUpgradePanel(shipInfo);
                    RefreshShipsPanel();
                });
            }
            else//碎片不足
            {
                transShipUnlockPanel.GetComponentInChildren<Button>().GetComponent<Image>().color = Color.gray;

                transShipUnlockPanel.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            }
        }
        else//(活动战舰)
        {
            transShipUnlockPanel.gameObject.SetActive(true);
            transShipUpgradePanel.gameObject.SetActive(false);
            transShipUnlockPanel.Find("PanelUnlock").gameObject.SetActive(false);
            transShipUnlockPanel.Find("PanelActivity").gameObject.SetActive(true);

        }
    }

    /// <summary>
    /// 刷新：装备页面
    /// </summary>
    private ShipCom selectedCom = null;
    public void RefreshEquipmentsPanel()
    {
        UserData userdata = InfoManager.GetInstance().userData;
        ShipConfig ship = userdata.currentShip;

        //...
        Transform weaponRow = windowEquipments.transform.Find("EquipmentsPanel").GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        Transform armorRow = windowEquipments.transform.Find("EquipmentsPanel").GetChild(1).GetChild(0).GetChild(0).GetChild(0);
        Transform engineRow = windowEquipments.transform.Find("EquipmentsPanel").GetChild(2).GetChild(0).GetChild(0).GetChild(0);

        
        //List<ShipCom> shipComs = userdata.myWeaponComs.Select((c) => { return (c as ShipCom); }).ToList(); //replace userdata.myWeaponComs
        SetRow(weaponRow, userdata.myWeaponComs.Select((c) => { return (c as ShipCom); }).ToList(), 0);
        SetRow(armorRow, userdata.myArmorComs.Select((c) => { return (c as ShipCom); }).ToList(), 1);
        SetRow(engineRow, userdata.myEngineComs.Select((c) => { return (c as ShipCom); }).ToList(), 2);

        #region 复用。。
        //for (int i = 0; i < weaponRow.childCount; i++)
        //{
        //    //Color:Default white
        //    weaponRow.GetChild(i).GetComponentInChildren<Text>().color = Color.black;

        //    if (i < userdata.myWeaponComs.Count)
        //    {
        //        weaponRow.GetChild(i).gameObject.SetActive(true);
        //        if (ship.activeWeaponCom == userdata.myWeaponComs[i])
        //        {
        //            weaponRow.GetChild(i).GetComponentInChildren<Text>().text = userdata.myWeaponComs[i].weaponInfo.displayName + "\n<color=#FFFFFF>(当前)</color>";
        //            int iTmp = i;
        //            weaponRow.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        //            weaponRow.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
        //                //...
        //                ship.activeWeaponCom = userdata.myWeaponComs[iTmp];
        //                RefreshEquipmentsPanel();
        //                RefreshEquipmentInfoPanel(userdata.myWeaponComs[iTmp]);
        //            });
        //        }
        //        else
        //        {
        //            bool isAvailable = true;
        //            foreach (var usership in userdata.userShips)
        //            {
        //                if (usership.activeWeaponCom == userdata.myWeaponComs[i]) isAvailable = false;
        //            }

        //            if (isAvailable)
        //            {
        //                weaponRow.GetChild(i).GetComponentInChildren<Text>().text = userdata.myWeaponComs[i].weaponInfo.displayName + "\n<color=#22FF22>(可用)</color>";
        //                int iTmp = i;
        //                weaponRow.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        //                weaponRow.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
        //                    //...
        //                    ship.activeWeaponCom = userdata.myWeaponComs[iTmp];
        //                    RefreshEquipmentsPanel();
        //                    RefreshEquipmentInfoPanel(userdata.myWeaponComs[iTmp]);
        //                });
        //            }
        //            else
        //            {
        //                weaponRow.GetChild(i).GetComponentInChildren<Text>().text = userdata.myWeaponComs[i].weaponInfo.displayName + "\n<color=#FF0000>(已被装备在其他战舰上)</color>";
        //                weaponRow.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        weaponRow.GetChild(i).gameObject.SetActive(false);
        //    }
        //}
        //weaponRow.GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 120 * userdata.myWeaponComs.Count, 0);
        #endregion
    }
    private void SetRow(Transform row, List<ShipCom> coms, int type)//0-w 1- a 2-e
    {
        UserData userdata = InfoManager.GetInstance().userData;
        ShipConfig ship = userdata.currentShip;
        ShipCom shipActiveCom;
        switch (type)
        {
            case 0:
                shipActiveCom = ship.activeWeaponCom;
                break;
            case 1:
                shipActiveCom = ship.activeArmorCom;
                break;
            case 2:
                shipActiveCom = ship.activeEngineCom;
                break;
            default:
                shipActiveCom = ship.activeWeaponCom;
                break;
        }


        //Instantiate Cells
        if(row.childCount == 1)
        {
            for(int i = 1; i < 30; i++)
            {
                GameObject o = Instantiate(row.GetChild(0).gameObject, row);
                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(70 + i * 120, 0);
            }
        }


        //Set cells
        for (int i = 0; i < row.childCount; i++)
        {
            //关闭边框
            row.GetChild(i).Find("ImageBorder").gameObject.SetActive(false);
            //默认有色图标
            row.GetChild(i).Find("Image").GetComponent<Image>().color = Color.white;
            row.GetChild(i).GetComponentInChildren<Text>().color = Color.white;


            if (i < coms.Count)
            {
                //激活
                row.GetChild(i).gameObject.SetActive(true);
                //显示图片
                row.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + coms[i].EquipmentInfo.displayName); row.GetChild(i).Find("Image").GetComponent<Image>().SetNativeSize();
                //显示名字
                row.GetChild(i).Find("TextName").GetComponent<Text>().text = "<color=#006600><b>" + coms[i].EquipmentInfo.displayName + "</b></color>";
                //默认状态显示
                row.GetChild(i).Find("TextStatus").GetComponent<Text>().text = "";

                if (shipActiveCom == coms[i])//当前
                {
                    //边框
                    row.GetChild(i).Find("ImageBorder").gameObject.SetActive(true);

                    int iTmp = i;

                    row.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                        
                        //移动游标
                        MoveComInd(row.GetChild(iTmp).transform);

                        RefreshEquipmentsPanel();
                        RefreshEquipmentInfoPanel(coms[iTmp], true);
                    });
                }
                else
                {
                    bool isAvailable = true;
                    foreach (var s in userdata.userShips)
                    {
                        //????
                        ShipCom thisShipActiveCom;
                        switch (type)
                        {
                            case 0:
                                thisShipActiveCom = s.activeWeaponCom;
                                break;
                            case 1:
                                thisShipActiveCom = s.activeArmorCom;
                                break;
                            case 2:
                                thisShipActiveCom = s.activeEngineCom;
                                break;
                            default:
                                thisShipActiveCom = s.activeWeaponCom;
                                break;
                        }
                        if (thisShipActiveCom == coms[i]) isAvailable = false;
                    }

                    if (isAvailable)//可用
                    {
                        int iTmp = i;
                        row.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        switch (type)
                        {
                            case 0:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    ship.activeWeaponCom = coms[iTmp] as ShipWeaponCom;
                                });
                                break;
                            case 1:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    ship.activeArmorCom = coms[iTmp] as ShipArmorCom;
                                });
                                break;
                            case 2:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    ship.activeEngineCom = coms[iTmp] as ShipEngineCom;
                                });
                                break;
                            default:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    ship.activeWeaponCom = coms[iTmp] as ShipWeaponCom;
                                });
                                break;
                        }
                        row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                            SoundPlayer.PlaySound2D("按钮");


                            //移动游标
                            MoveComInd(row.GetChild(iTmp).transform);

                            MenuNotice.Notice(coms[iTmp].EquipmentInfo.displayName, "已装备 " + coms[iTmp].EquipmentInfo.displayName, Color.green);
                            RefreshEquipmentsPanel();
                            RefreshEquipmentInfoPanel(coms[iTmp], true);
                        });
                    }
                    else//已被装备在其他战舰上
                    {
                        int iTmp = i;

                        //装备状态显示
                        row.GetChild(i).Find("TextStatus").GetComponent<Text>().text = "<b>" + "其他战舰已装备" + "</b>";

                        row.GetChild(i).Find("Image").GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 1f);
                        row.GetChild(i).GetComponentInChildren<Text>().color = new Color(0.25f, 0.25f, 0.25f, 1f);

                        row.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();

                        switch (type)
                        {
                            case 0:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    TryDetachCom(coms[iTmp]);
                                    ship.activeWeaponCom = coms[iTmp] as ShipWeaponCom;
                                });
                                break;
                            case 1:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    TryDetachCom(coms[iTmp]);
                                    ship.activeArmorCom = coms[iTmp] as ShipArmorCom;
                                });
                                break;
                            case 2:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    TryDetachCom(coms[iTmp]);
                                    ship.activeEngineCom = coms[iTmp] as ShipEngineCom;
                                });
                                break;
                            default:
                                row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                                    TryDetachCom(coms[iTmp]);
                                    ship.activeWeaponCom = coms[iTmp] as ShipWeaponCom;
                                });
                                break;
                        }
                        row.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                            SoundPlayer.PlaySound2D("按钮");


                            //移动游标
                            MoveComInd(row.GetChild(iTmp).transform);

                            MenuNotice.Notice(coms[iTmp].EquipmentInfo.displayName, "已从其他战舰拆除装备", Color.yellow);
                            MenuNotice.Notice(coms[iTmp].EquipmentInfo.displayName, "已装备 " + coms[iTmp].EquipmentInfo.displayName, Color.green);
                            RefreshEquipmentsPanel();
                            RefreshEquipmentInfoPanel(coms[iTmp], true);
                        });
                    }
                }
            }
            else
            {
                row.GetChild(i).gameObject.SetActive(false);
            }
        }
        row.GetComponent<RectTransform>().sizeDelta = new Vector2(10 + 120 * coms.Count, 0);
    }

    private void MoveComInd(Transform targetTrans)
    {
        //Ind
        indSelectedCom.transform.DOKill();

        indSelectedCom.transform.parent = this.transform;//不可以直接穿过其他
        indSelectedCom.transform.DOMove(targetTrans.position, 0.25f).OnComplete(() => { indSelectedCom.transform.parent = targetTrans; indSelectedCom.transform.localPosition = new Vector3(); });
    }

    
    /// <summary>
    /// 刷新：装备详情
    /// </summary>
    public void RefreshEquipmentInfoPanel(ShipCom com, bool reShow = false)
    {
        //--设为当前选择--
        selectedCom = com;
        //----------------

        Transform equipmentPanel = windowEquipments.transform.Find("EquipmentInfoPanel");

        if (reShow)
        {
            equipmentPanel.gameObject.SetActive(true);

            equipmentPanel.DOComplete();
            equipmentPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(250, 50);
            equipmentPanel.GetComponent<RectTransform>().DOAnchorPos(new Vector2(-5, 50), 0.5f);
        }


        indSelectedCom.gameObject.SetActive(true);
        if (com == null)
        {
            equipmentPanel.Find("TextTitle").GetComponent<Text>().text = "已被拆解！";
            equipmentPanel.Find("TextLvl").GetComponent<Text>().text = "";

            //Button onCLick...（REMOVE ALL Listeners）
            equipmentPanel.Find("ButtonUpgrade").gameObject.SetActive(false);
            equipmentPanel.Find("ButtonDecompose").gameObject.SetActive(false);
            

            //直接隐藏
            equipmentPanel.gameObject.SetActive(false);
            indSelectedCom.gameObject.SetActive(false);
            return;
        }

        EquipmentInfo equipInfo = com.EquipmentInfo;

        if(equipInfo != null)
        {
            equipmentPanel.Find("TextTitle").GetComponent<Text>().text = equipInfo.displayName;
            equipmentPanel.Find("TextLvl").GetComponent<Text>().text = com.lvl.ToString();//等级

            //Introduce:
            string str = "";
            if(equipInfo is WeaponInfo)
            {
                str += "攻击力+" + ((equipInfo as WeaponInfo).attack * Utils.CalFac(com.lvl)).ToString("f2") + "\n";
                str += "穿透：" + ((equipInfo as WeaponInfo).piercingPercentage * 100f).ToString("N0") + "%\n";
            }
            if (equipInfo is ArmorInfo)
            {
                str += "防御力+" + ((equipInfo as ArmorInfo).defense * Utils.CalFac(com.lvl)).ToString("f2") + "\n";
                str += "生命值+" + ((equipInfo as ArmorInfo).hp * Utils.CalFac(com.lvl)).ToString("f2") + "\n";
            }
            if (equipInfo is EngineInfo)
            {
                str += "速度+" + ((equipInfo as EngineInfo).speed  * Utils.CalFac(com.lvl) / 10f).ToString("f2") + "%\n";
                str += "生命值+" + ((equipInfo as EngineInfo).hp * Utils.CalFac(com.lvl)).ToString("f2") + "\n";
            }
            equipmentPanel.Find("TextInfo").GetComponent<Text>().text = str;

            //ImageItem
            equipmentPanel.Find("ImageItem").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + equipInfo.displayName);
            equipmentPanel.Find("ImageItem").GetChild(0).GetComponent<Image>().SetNativeSize();



            //Button onCLick...
            //ButtonDecompose
            equipmentPanel.Find("ButtonUpgrade").gameObject.SetActive(true);

            if (Utils.CalComUpgradeCost(com.lvl, equipInfo.ranklvl) < InfoManager.GetInstance().userData.equipmentMat && com.lvl < 30)//可升级
            {
                //Cost text
                string txtCost = Utils.CalComUpgradeCost(com.lvl, equipInfo.ranklvl).ToString();
                equipmentPanel.Find("TextCost").GetComponent<Text>().text = "<color=#00FF00>" + txtCost + "</color>/" + InfoManager.GetInstance().userData.equipmentMat.ToString();

                //btn
                equipmentPanel.Find("ButtonUpgrade").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");
                equipmentPanel.Find("ButtonUpgrade").GetComponent<Image>().color = Color.white;

                equipmentPanel.Find("ButtonUpgrade").GetComponent<Button>().onClick.RemoveAllListeners();
                equipmentPanel.Find("ButtonUpgrade").GetComponent<Button>().onClick.AddListener(() => {
                    //音效
                    SoundPlayer.PlaySound2D("升级");

                    TryUpgradeCom(com);
                    RefreshEquipmentInfoPanel(com);
                });
            }
            else//升级材料不足或者满级
            {
                //Cost text
                string txtCost = com.lvl < 30 ? Utils.CalComUpgradeCost(com.lvl, equipInfo.ranklvl).ToString() : "(已满级)";
                equipmentPanel.Find("TextCost").GetComponent<Text>().text = "<color=#FF0000>" + txtCost + "</color>/" + InfoManager.GetInstance().userData.equipmentMat.ToString();
                //btn
                equipmentPanel.Find("ButtonUpgrade").GetComponent<Image>().color = Color.gray;
                equipmentPanel.Find("ButtonUpgrade").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");

                equipmentPanel.Find("ButtonUpgrade").GetComponent<Button>().onClick.RemoveAllListeners();

                if(com.lvl < 30)
                {
                    if (tapCountUpgradeCom > 4)
                    {
                        tapCountUpgradeCom = 0;
                        equipmentPanel.Find("ButtonUpgrade").GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
                        equipmentPanel.Find("ButtonUpgrade").GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/广告升级");
                        equipmentPanel.Find("ButtonUpgrade").GetComponentInChildren<Button>().onClick.AddListener(() => {
                            AdManager.VideoAd(() => {
                                TryUpgradeCom(com, true);
                                RefreshEquipmentInfoPanel(com);
                            }, () => { });
                        });

                    }
                    else
                    {
                        equipmentPanel.Find("ButtonUpgrade").GetComponentInChildren<Button>().onClick.AddListener(() => {

                            MenuNotice.Notice("Error", "材料不足！", Color.red);
                            tapCountUpgradeCom += 1;
                            RefreshEquipmentInfoPanel(com);
                        });
                    }
                }
            }

            //button down
            equipmentPanel.Find("ButtonDown").gameObject.SetActive(true);
            equipmentPanel.Find("ButtonDown").GetComponent<Button>().onClick.RemoveAllListeners();
            equipmentPanel.Find("ButtonDown").GetComponent<Button>().onClick.AddListener(() => {
                TryDetachCom(com);
                RefreshEquipmentInfoPanel(null);
                RefreshEquipmentsPanel();
            });
            //button decom
            equipmentPanel.Find("ButtonDecompose").gameObject.SetActive(true);
            equipmentPanel.Find("ButtonDecompose").GetComponent<Button>().onClick.RemoveAllListeners();
            equipmentPanel.Find("ButtonDecompose").GetComponent<Button>().onClick.AddListener(() => {
                ShowWindowDeCom(com);
            });

        }
    }

    /// <summary>
    /// 刷新：匹配窗口
    /// </summary>
    public void RefreshWindowMatch()
    {
        InfoManager.RefreshPlayers();

        for (int i = 0; i < windowMatch.transform.Find("PlayerCells").childCount; i++)
        {
            if(i == 0)
            {
                windowMatch.transform.Find("PlayerCells").GetComponentsInChildren<Text>()[i].text = InfoManager.GetInstance().userData.name;
                windowMatch.transform.Find("PlayerCells").GetChild(i).Find("ImagePic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/Wait");
                continue;
            }

            windowMatch.transform.Find("PlayerCells").GetComponentsInChildren<Text>()[i].text = "匹配中...";
            windowMatch.transform.Find("PlayerCells").GetChild(i).Find("ImagePic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/null");
        }
        windowMatch.transform.Find("ImageStatus").gameObject.SetActive(true);
        windowMatch.transform.Find("TextStatus").gameObject.SetActive(false);

        Invoke("ReadyToEnter", 1f);
        Invoke("EnterMatch", 2f);
    }


    private void ReadyToEnter()
    {
        int[] userIds = InfoManager.playerIds;

        windowMatch.transform.Find("ImageStatus").gameObject.SetActive(false);
        windowMatch.transform.Find("TextStatus").gameObject.SetActive(true);

        for (int i = 0; i < windowMatch.transform.Find("PlayerCells").childCount; i++)
        {
            if (i == 0) continue;

            windowMatch.transform.Find("PlayerCells").GetComponentsInChildren<Text>()[i].text = InfoManager.playerNames[i];
            windowMatch.transform.Find("PlayerCells").GetChild(i).Find("ImagePic").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Pic/" + userIds[i]);
        }
    }
    private void EnterMatch()
    {
        InfoManager.currentGameType = 99;

        AddTotalGameCount();
        SceneManager.LoadScene("SceneMatch");
    }

    /// <summary>
    /// 刷新： 关卡界面(弃用)
    /// </summary>
    public void RefreshWindowMissions(int currentTypeID)
    {
    //    for(int i = 0; i < 3; i++)
    //    {

    //        string missionName;
    //        switch (i)
    //        {
    //            case 0:
    //                missionName = "歼灭";
    //                break;
    //            case 1:
    //                missionName = "生存";
    //                break;
    //            case 2:
    //                missionName = "支援";
    //                break;
    //            default:
    //                missionName = "??";
    //                break;
    //        }



    //        windowMissions.transform.Find("PanelMissions").GetChild(i).GetComponent<Button>().enabled = false;

    //        //无文本
    //        windowMissions.transform.Find("PanelMissions").GetChild(i).GetComponentInChildren<Text>().text = "";

    //        if (currentTypeID == i)//当前
    //        {
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").GetComponent<Image>().color = Color.white;
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBTN").GetComponent<Image>().color = Color.white;
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageName").GetComponent<Image>().color = Color.white;
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").localScale = new Vector3(0.1f, 0.1f, 0.1f);
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").DOScale(Vector3.one, 0.5f);
    //        }
    //        else//不是当前
    //        {
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").DOKill();
    //            windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").localScale = new Vector3(0.1f, 0.1f, 0.1f);

    //            if (i > 0 && InfoManager.GetInstance().userData.lvlsProgress[i - 1] < 10)//未解锁
    //            {
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).GetComponentInChildren<Text>().text = "完成前置任务关卡10之后解锁";
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").GetComponent<Image>().color = Color.gray;
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBTN").GetComponent<Image>().color = Color.gray;
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageName").GetComponent<Image>().color = Color.gray;
    //            }
    //            else//已解锁
    //            {
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).GetComponent<Button>().enabled = true;
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBG").GetComponent<Image>().color = Color.gray;
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageBTN").GetComponent<Image>().color = Color.gray;
    //                windowMissions.transform.Find("PanelMissions").GetChild(i).Find("ImageName").GetComponent<Image>().color = Color.gray;
    //            }
    //        }
    //    }

    //    ResetLevelList(currentTypeID);
    //}

    //public void SetMissionType(int typeId)
    //{
    //    SoundPlayer.PlaySound2D("按钮");
    //    InfoManager.currentMissionType = typeId;
    //    RefreshWindowMissions(typeId);
    }
    public void ResetLevelList(int typeid)
    {
        //List<Dropdown.OptionData> list = new List<Dropdown.OptionData>();
        //for (int i = 0; i < InfoManager.GetInstance().userData.lvlsProgress[typeid]; i++)
        //{
        //    Dropdown.OptionData option = new Dropdown.OptionData();
        //    option.text = (i + 1).ToString();
        //    list.Add(option);
        //}
        //windowMissions.transform.Find("Dropdown").GetComponent<Dropdown>().options = list;

        //SetLevel();
    }
    public void SetLevel()
    {
        //SoundPlayer.PlaySound2D("按钮");
        //int level;
        //int.TryParse(windowMissions.transform.Find("Dropdown").GetComponent<Dropdown>().captionText.text, out level);
        //InfoManager.GetInstance().userData.currentLvls[InfoManager.currentMissionType] = level;
    }


    /// <summary>
    /// 刷新： 远征窗口
    /// </summary>
    public void RefreshWindowAchieve(bool toIndicatePos = false)
    {
        //tmp
        Button btnGet1 = null;
        Button btnGet2 = null;

        //..
        Transform transPanelCommon = windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("PanelCommon");
        Transform transPanelSpecial = windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("PanelSpecial");


        //Bar
        float progress = (InfoManager.GetInstance().userData.achieveProgress / 1000f);
        windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("ProgressBar").GetChild(0).GetComponent<RectTransform>().anchorMax = new Vector2(progress, 0.5f);
        windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("ProgressBar").GetChild(0).GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);

        windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("ProgressBar").GetChild(1).GetComponent<RectTransform>().anchorMax = new Vector2(progress, 0.5f);
        windowAchieve.transform.Find("Scroll View").GetChild(0).GetChild(0).Find("ProgressBar").GetChild(1).GetComponent<RectTransform>().anchorMin = new Vector2(progress, 0.5f);



        //instantiate
        if(transPanelCommon.childCount == 1 && transPanelSpecial.childCount == 1)
        {
            for (int i = 0; i < 19; i++)
            {
                Instantiate(transPanelCommon.GetChild(0).gameObject, transPanelCommon);
                Instantiate(transPanelSpecial.GetChild(0).gameObject, transPanelSpecial);
            }
        }

        //Buttons
        for (int i = 0; i < InfoManager.GetInstance().commonBonusList.Count; i++)
        {
            transPanelCommon.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(120 + i * 120 + 60, 0);
            transPanelCommon.GetChild(i).Find("ImageItem").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + InfoManager.GetInstance().commonBonusList[i].name); 
            transPanelCommon.GetChild(i).GetComponentInChildren<Text>().text = InfoManager.GetInstance().commonBonusList[i].name;

            int iTmp = i;


            //已经解锁
            bool unlocked = (i + 1) <= Mathf.FloorToInt(progress * InfoManager.GetInstance().commonBonusList.Count);
            //可以当前领取
            bool isCurrent = (i + 1) == Mathf.FloorToInt(progress * InfoManager.GetInstance().commonBonusList.Count);

            //Bottom Image
            transPanelCommon.GetChild(i).GetComponent<Image>().color = Color.gray;
            transPanelCommon.GetChild(i).Find("ImageItem").GetComponent<Image>().color = Color.gray;

            //LockIcon
            transPanelCommon.GetChild(i).Find("ImageLock").gameObject.SetActive(true);
            if (unlocked)
            {
                transPanelCommon.GetChild(i).Find("ImageLock").gameObject.SetActive(false) ;
            }
            

            //Get Img
            transPanelCommon.GetChild(i).Find("ImageGot").gameObject.SetActive(false);
            if(InfoManager.GetInstance().userData.bonusGet1[iTmp] == true)
            {
                transPanelCommon.GetChild(i).Find("ImageGot").gameObject.SetActive(true);
            }


            //BTN Call And ImgBlink
            transPanelCommon.GetChild(i).Find("ImageCurrent").gameObject.SetActive(false);

            transPanelCommon.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();

            if (isCurrent && InfoManager.GetInstance().userData.bonusGet1[iTmp] == false)
            {
                //闪烁框
                transPanelCommon.GetChild(i).Find("ImageCurrent").gameObject.SetActive(true);
                //颜色
                transPanelCommon.GetChild(i).GetComponent<Image>().color = Color.white;
                transPanelCommon.GetChild(i).Find("ImageItem").GetComponent<Image>().color = Color.white;

                //tmp
                btnGet1 = transPanelCommon.GetChild(i).GetComponent<Button>();

                transPanelCommon.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                    InfoManager.GetInstance().userData.bonusGet1[iTmp] = true;
                    GiveAchieveBonus(InfoManager.GetInstance().commonBonusList[iTmp]);
                    RefreshWindowAchieve();
                });
            }


        }

        for (int i = 0; i < InfoManager.GetInstance().specialBonusList.Count; i++)
        {
            transPanelSpecial.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector2(120 + i * 120 + 60, 0);
            transPanelSpecial.GetChild(i).Find("ImageItem").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + InfoManager.GetInstance().specialBonusList[i].name);
            transPanelSpecial.GetChild(i).GetComponentInChildren<Text>().text = InfoManager.GetInstance().specialBonusList[i].name;

            int iTmp = i;


            //已经解锁
            bool unlocked = (i + 1) <= Mathf.FloorToInt(progress * InfoManager.GetInstance().specialBonusList.Count);
            //可以当前领取
            bool isCurrent = (i + 1) == Mathf.FloorToInt(progress * InfoManager.GetInstance().specialBonusList.Count);

            //Bottom Image
            transPanelSpecial.GetChild(i).GetComponent<Image>().color = Color.gray; 
            transPanelSpecial.GetChild(i).Find("ImageItem").GetComponent<Image>().color = Color.gray; 

            //LockIcon
            transPanelSpecial.GetChild(i).Find("ImageLock").gameObject.SetActive(true);
            if (unlocked)
            {
                transPanelSpecial.GetChild(i).Find("ImageLock").gameObject.SetActive(false);
            }

            //Get Img
            transPanelSpecial.GetChild(i).Find("ImageGot").gameObject.SetActive(false);
            if (InfoManager.GetInstance().userData.bonusGet2[iTmp] == true)
            {
                transPanelSpecial.GetChild(i).Find("ImageGot").gameObject.SetActive(true);
            }


            //BTN Call And ImgBlink

            transPanelSpecial.GetChild(i).Find("ImageCurrent").gameObject.SetActive(false);

            transPanelSpecial.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();

            if (isCurrent && InfoManager.GetInstance().userData.bonusGet2[iTmp] == false)
            {
                //BLink
                transPanelSpecial.GetChild(i).Find("ImageCurrent").gameObject.SetActive(true);
                //color
                transPanelSpecial.GetChild(i).GetComponent<Image>().color = Color.white;
                transPanelSpecial.GetChild(i).Find("ImageItem").GetComponent<Image>().color = Color.white;

                //tmp
                btnGet2 = transPanelSpecial.GetChild(i).GetComponent<Button>();

                transPanelSpecial.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                    //埋点
                    MaiDian.Mai("number", 2, "version", InfoManager.Version, "startVideo");

                    AdManager.VideoAd(() => {

                        //埋点
                        MaiDian.Mai("number", 2, "version", InfoManager.Version, "endVideo");

                        InfoManager.GetInstance().userData.bonusGet2[iTmp] = true;
                        GiveAchieveBonus(InfoManager.GetInstance().specialBonusList[iTmp]);
                        RefreshWindowAchieve();
                    },() => {

                    });
                });
            }
        }



        int rightIdx = Mathf.FloorToInt(progress * InfoManager.GetInstance().commonBonusList.Count) - 1;
        //========领取按钮========
        windowAchieve.transform.Find("ButtonGet1").GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1.0f);
        windowAchieve.transform.Find("ButtonGet2").GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        windowAchieve.transform.Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowAchieve.transform.Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();


        if (rightIdx > -1 && InfoManager.GetInstance().userData.bonusGet1[rightIdx] == false)
        {
            windowAchieve.transform.Find("ButtonGet1").GetComponent<Image>().color = Color.white;
            windowAchieve.transform.Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                btnGet1.onClick?.Invoke();
            });
        }
        if (rightIdx > -1 && InfoManager.GetInstance().userData.bonusGet2[rightIdx] == false)
        {
            windowAchieve.transform.Find("ButtonGet2").GetComponent<Image>().color = Color.white;
            windowAchieve.transform.Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {
                btnGet2.onClick?.Invoke();
            });
        }

        //========补领按钮========

        //是否显示补领按钮
        windowAchieve.transform.Find("ButtonGet3").GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 1.0f);

        for (int i = 0; i < rightIdx; i++)
        {
            if (InfoManager.GetInstance().userData.bonusGet1[i] == false)
            {
                windowAchieve.transform.Find("ButtonGet3").GetComponent<Image>().color = Color.white;
                break;
            }
            if (InfoManager.GetInstance().userData.bonusGet2[i] == false)
            {
                windowAchieve.transform.Find("ButtonGet3").GetComponent<Image>().color = Color.white;
                break;
            }
        }

        //按钮回调
        windowAchieve.transform.Find("ButtonGet3").GetComponent<Button>().onClick.RemoveAllListeners();
        windowAchieve.transform.Find("ButtonGet3").GetComponent<Button>().onClick.AddListener(() => {
            AdManager.VideoAd(() => {


                //--补领

                for (int i = 0; i < rightIdx; i++)
                {
                    if(InfoManager.GetInstance().userData.bonusGet1[i] == false)
                    {
                        InfoManager.GetInstance().userData.bonusGet1[i] = true;
                        GiveAchieveBonus(InfoManager.GetInstance().commonBonusList[i]);
                    }
                    if (InfoManager.GetInstance().userData.bonusGet2[i] == false)
                    {
                        InfoManager.GetInstance().userData.bonusGet2[i] = true;
                        GiveAchieveBonus(InfoManager.GetInstance().specialBonusList[i]);
                    }
                }
                RefreshWindowAchieve();
                //----


            }, () => {

            });
        });


        //Auto scroll
        if (toIndicatePos)
        {
            windowAchieve.transform.Find("Scroll View").GetComponent<ScrollRect>().DOHorizontalNormalizedPos(progress, 1f);
        }
    }

    /// <summary>
    /// 刷新：签到窗口
    /// </summary>
    public void RefreshWindowCheckin()
    {
        Transform transPanelCheckin = windowCheckin.transform.Find("Panel");

        for (int i = 0; i < transPanelCheckin.childCount; i++)
        {
            transPanelCheckin.GetChild(i).GetComponentInChildren<Text>().text = InfoManager.GetInstance().checkinBonusList[i].name;

            transPanelCheckin.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + InfoManager.GetInstance().checkinBonusList[i].name);
            //签到图标显示。。。
            if (InfoManager.GetInstance().userData.checkins[i] == "")
            {
                transPanelCheckin.GetChild(i).Find("ImageGet").gameObject.SetActive(false);
            }
            else
            {
                transPanelCheckin.GetChild(i).Find("ImageGet").gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// 刷新：战舰预览
    /// </summary>
    public void RefreshShipPreview(ShipInfo shipInfo)
    {
        //删除原ship
        if (shipPreview != null)
        {
            shipPreview.GetComponentInChildren<WaterParticles>().CleanImmidiate();
            Destroy(shipPreview);
        }

        shipPreview = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/" + shipInfo.displayName), new Vector3(), new Quaternion(), null);
        shipPreview.transform.position = new Vector3(shipPreview.transform.position.x, -shipPreview.GetComponentInChildren<Float>().BalanceHeight, shipPreview.transform.position.z);
        shipPreview.GetComponent<Hull>().shipConfig = new ShipConfig();
        shipPreview.GetComponent<Hull>().shipConfig.shipInfo = shipInfo;

        //只保留浮力
        foreach (Turret t in shipPreview.GetComponentsInChildren<Turret>())
        {
            t.enabled = false;
        }
        foreach (Ability a in shipPreview.GetComponentsInChildren<Ability>())
        {
            a._This.enabled = false;
        }
        foreach (ChimneyParticles c in shipPreview.GetComponentsInChildren<ChimneyParticles>())
        {
            c.enabled = false;
            Destroy(c.GetComponent<ParticleSystem>());
        }

        shipPreview.GetComponentInChildren<Engine>().enabled = false;
        shipPreview.GetComponentInChildren<Rudder>().enabled = false;
    }

    /// <summary>
    /// 刷新：船员界面
    /// </summary>
    public void RefreshWindowCrew()
    {
        RectTransform transCrewsContent = this.transform.Find("Windows").Find("WindowCrew").Find("CrewsPanel").GetComponentInChildren<ScrollRect>().content;
        UserData userdata = InfoManager.GetInstance().userData;

        transCrewsContent.sizeDelta = new Vector2(transCrewsContent.sizeDelta.x, userdata.crews.Count * 140);

        //列表窗口
        //instantiate列表
        if(transCrewsContent.childCount == 1)
        {
            GameObject prefab = transCrewsContent.GetChild(0).gameObject;
            for (int i = 1; i < 30; i++)
            {
                GameObject o = Instantiate(prefab, transCrewsContent);
                o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -70 - 140 * i);
            }
        }
        transCrewsContent.GetComponent<RectTransform>().sizeDelta = new Vector2(transCrewsContent.GetComponent<RectTransform>().sizeDelta.x, 140 * (userdata.crews.Count + 1));

        //设置列表
        for(int i = 0; i < transCrewsContent.childCount; i++)
        {

            if(i < userdata.crews.Count)
            {
                transCrewsContent.GetChild(i).gameObject.SetActive(true);
                transCrewsContent.GetChild(i).GetComponentInChildren<Text>().text = userdata.crews[i].crewInfo.name;
                transCrewsContent.GetChild(i).GetComponent<Image>().color = Color.white;

                //rank color
                transCrewsContent.GetChild(i).GetComponent<Image>().color = Utils.RankColor(userdata.crews[i].crewInfo.rank);
                //img
                transCrewsContent.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userdata.crews[i].crewInfo.name); transCrewsContent.GetChild(i).Find("Image").GetComponent<Image>().color = Color.white;

                transCrewsContent.GetChild(i).GetComponentInChildren<Text>().color = Color.white;

                if (userdata.crews[i] == userdata.currentShip.crew)
                {
                    transCrewsContent.GetChild(i).Find("ImageBorder").gameObject.SetActive(true);
                }
                else
                {

                    transCrewsContent.GetChild(i).Find("ImageBorder").gameObject.SetActive(false);

                    bool isAvailable = true;
                    foreach(var ship in userdata.userShips)
                    {
                        if(ship.crew == userdata.crews[i])
                        {
                            isAvailable = false;
                        }
                    }

                    if (isAvailable)
                    {
                        int iTmp = i;
                        transCrewsContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        transCrewsContent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                            //音效
                            SoundPlayer.PlaySound2D("按钮");

                            //SetCurrentCrew;
                            userdata.currentShip.crew = userdata.crews[iTmp];
                            RefreshWindowCrew();

                            //notice
                            MenuNotice.Notice(userdata.crews[iTmp].crewInfo.name, "船员 " + userdata.crews[iTmp].crewInfo.name + "已装备", Color.green);
                        });
                    }
                    else
                    {
                        //Grey
                        transCrewsContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        transCrewsContent.GetChild(i).GetComponent<Image>().color = Color.gray;
                        transCrewsContent.GetChild(i).GetComponentInChildren<Text>().color = Color.grey;
                        transCrewsContent.GetChild(i).Find("Image").GetComponent<Image>().color = Color.grey;
                        //Dequip from other ship?? no
                    }
                }
            }
            else
            {
                transCrewsContent.GetChild(i).gameObject.SetActive(false);
            }
        }

        //详情窗口
        RectTransform transCrewInfo = this.transform.Find("Windows").Find("WindowCrew").Find("CrewInfoPanel").GetComponent<RectTransform>();
        RectTransform transNoCrewPanel = this.transform.Find("Windows").Find("WindowCrew").Find("NoCrewPanel").GetComponent<RectTransform>();

        transCrewInfo.gameObject.SetActive(false);
        transNoCrewPanel.gameObject.SetActive(true);

        if (userdata.currentShip.crew != null)
        {
            transCrewInfo.gameObject.SetActive(true);
            transNoCrewPanel.gameObject.SetActive(false);

            //Base introduce
            transCrewInfo.Find("TextTitle").GetComponent<Text>().text = userdata.currentShip.crew.crewInfo.name;
            transCrewInfo.Find("TextLvl").GetComponent<Text>().text = userdata.currentShip.crew.lvl.ToString();

            //pic
            transCrewInfo.Find("ImagePic").GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + userdata.currentShip.crew.crewInfo.name);
            //rank
            transCrewInfo.Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rank/" + userdata.currentShip.crew.crewInfo.rank.ToUpper());
            //rank color
            transCrewInfo.Find("ImagePic").GetComponent<Image>().color = Utils.RankColor(userdata.currentShip.crew.crewInfo.rank);

            //Prop add Introduce
            transCrewInfo.Find("TextIntroduce").GetComponent<Text>().text = "";
            if(userdata.currentShip.crew.crewInfo.attack > 0)
            {
                transCrewInfo.Find("TextIntroduce").GetComponent<Text>().text += "\n战舰攻击力+" + ((float)userdata.currentShip.crew.crewInfo.attack * (1f + userdata.currentShip.crew.lvl * 0.01f)).ToString("f2") + "%";
            }
            if (userdata.currentShip.crew.crewInfo.hp > 0)
            {
                transCrewInfo.Find("TextIntroduce").GetComponent<Text>().text += "\n战舰生命值+" + ((float)userdata.currentShip.crew.crewInfo.hp * (1f + userdata.currentShip.crew.lvl * 0.01f)).ToString("f2") + "%";
            }
            if (userdata.currentShip.crew.crewInfo.defense > 0)
            {
                transCrewInfo.Find("TextIntroduce").GetComponent<Text>().text += "\n战舰防御力+" + ((float)userdata.currentShip.crew.crewInfo.defense * (1f + userdata.currentShip.crew.lvl * 0.01f)).ToString("f2") + "%";
            }
            if (userdata.currentShip.crew.crewInfo.speed > 0)
            {
                transCrewInfo.Find("TextIntroduce").GetComponent<Text>().text += "\n战舰速度+" + ((float)userdata.currentShip.crew.crewInfo.speed * (1f + userdata.currentShip.crew.lvl * 0.01f)).ToString("f2") + "%";
            }



            //UpgradeButton
            int moneyTmp = Utils.CalCrewUpgradeCost(userdata.currentShip.crew);
            if (userdata.currentShip.crew.lvl < 30 && moneyTmp <= userdata.money)//船员可升级
            {
                //Cost
                string txtCost = Utils.CalCrewUpgradeCost(userdata.currentShip.crew).ToString();
                transCrewInfo.Find("TextCost").GetComponent<Text>().text = "<color=#00FF00>" + txtCost + "</color>/" + InfoManager.GetInstance().userData.money;

                //btn
                transCrewInfo.Find("ButtonUpgrade").GetComponent<Image>().color = Color.white;
                transCrewInfo.Find("ButtonUpgrade").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");

                transCrewInfo.Find("ButtonUpgrade").GetComponent<Button>().onClick.RemoveAllListeners();
                transCrewInfo.Find("ButtonUpgrade").GetComponent<Button>().onClick.AddListener(() => {
                    //音效
                    SoundPlayer.PlaySound2D("升级");


                    userdata.currentShip.crew.lvl = Mathf.Clamp(userdata.currentShip.crew.lvl + 1, 1, 30);
                    userdata.money -= moneyTmp;
                    RefreshWindowCrew();

                    //notice
                    MenuNotice.Notice(userdata.currentShip.crew.crewInfo.name, "船员已升级", Color.green);
                });
            }
            else//金币不足升级或者满级
            {
                //Cost
                string txtCost = userdata.currentShip.crew.lvl < 30 ? Utils.CalCrewUpgradeCost(userdata.currentShip.crew).ToString() : "(已满级)";
                transCrewInfo.Find("TextCost").GetComponent<Text>().text = "<color=#FF0000>" + txtCost + "</color>/" + InfoManager.GetInstance().userData.money;

                //btn
                transCrewInfo.Find("ButtonUpgrade").GetComponent<Image>().color = Color.gray;
                transCrewInfo.Find("ButtonUpgrade").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/升级");

                transCrewInfo.Find("ButtonUpgrade").GetComponent<Button>().onClick.RemoveAllListeners();
                

                if(userdata.currentShip.crew.lvl < 30)
                {
                    if (tapCountUpgradeCrew > 4)
                    {
                        tapCountUpgradeCrew = 0;
                        transCrewInfo.Find("ButtonUpgrade").GetComponentInChildren<Button>().GetComponent<Image>().color = Color.white;
                        transCrewInfo.Find("ButtonUpgrade").GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Button/广告升级");
                        transCrewInfo.Find("ButtonUpgrade").GetComponentInChildren<Button>().onClick.AddListener(() => {

                            AdManager.VideoAd(() => {
                                userdata.currentShip.crew.lvl = Mathf.Clamp(userdata.currentShip.crew.lvl + 1, 1, 30);
                                RefreshWindowCrew();
                            }, () => { });
                        });

                    }
                    else
                    {
                        transCrewInfo.Find("ButtonUpgrade").GetComponentInChildren<Button>().onClick.AddListener(() => {
                            MenuNotice.Notice("Error", "金币不足！", Color.red);
                            tapCountUpgradeCrew += 1;
                            RefreshWindowCrew();
                        });
                    }
                }
            }

            //ButtonDetach
            transCrewInfo.Find("ButtonDetach").GetComponent<Button>().onClick.RemoveAllListeners();
            transCrewInfo.Find("ButtonDetach").GetComponent<Button>().onClick.AddListener(() => {
                string nameTmp = userdata.currentShip.crew.crewInfo.name;
                userdata.currentShip.crew = null;
                RefreshWindowCrew();

                //notice
                MenuNotice.Notice(nameTmp, "船员 " + nameTmp + " 已卸任", Color.yellow);
            });

        }
    }

    //-------------------------------------------Load Level----------------------------------------
    public bool IsReady()
    {
        //ShipConfig currentShipConfig = InfoManager.GetInstance().userData.currentShip;
        //if (currentShipConfig.activeWeaponCom != null && currentShipConfig.activeWeaponCom != null && currentShipConfig.activeWeaponCom != null)
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        return true;
    }
    public void LoadMatchLevel()
    {
        if (IsReady())
        {
            //自动保存
            SaveUserData();

            ToggleWindowMatch();
        }
        else
        {
            Debug.Log("未准备好！");
        }
    }
    public void LoadMissionLevel()
    {
        InfoManager.currentGameType = InfoManager.GetInstance().userData.currentMission;

        //自动保存
        SaveUserData();

        Debug.Log("尝试载入关卡：类型：" + InfoManager.GetInstance().userData.currentMission + ", 关卡：" + InfoManager.GetInstance().userData.currentLvl);

        if (IsReady())
        {
            switch (InfoManager.GetInstance().userData.currentMission)
            {
                case 0:
                    {
                        AddTotalGameCount();
                        SceneManager.LoadScene("SceneMissionSunset");
                    }
                    break;
                case 1:
                    {
                        AddTotalGameCount();
                        SceneManager.LoadScene("SceneMissionFog");
                    }
                    break;
                case 2:
                    {
                        AddTotalGameCount();
                        SceneManager.LoadScene("SceneMissionStorm");
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.Log("未准备好！");
        }
    }

    public void LoadTutorialLevel()
    {
        InfoManager.currentGameType = -1;

        //自动保存
        SaveUserData();

        AddTotalGameCount();
        SceneManager.LoadScene("SceneTutorial");
    }

    //--------------------------------------------------------------User Data Operations------------------------------------------------------------------------------


    /// <summary>
    /// 设置当前战舰
    /// </summary>
    /// <param name="shipInfo"></param>
    public void SetCurrent(ShipInfo shipInfo)
    {
        UserData data = InfoManager.GetInstance().userData;
        var usership = data.userShips.FirstOrDefault(s => s.shipInfo == shipInfo);
        if (usership != null)
        {
            data.currentShip = usership;


        }
        else
        {
            Debug.Log("No ship found");
        }
    }

    /// <summary>
    /// 升级战舰
    /// </summary>
    public void TryUpgradeCurrentShip(bool forFree = false)
    {
        if(InfoManager.GetInstance().userData.currentShip.lvl < 30)
        {
            if (!forFree)
            {
                InfoManager.GetInstance().userData.money -= Utils.CalShipUpgradeCost(InfoManager.GetInstance().userData.currentShip.lvl, InfoManager.GetInstance().userData.currentShip.shipInfo.ranklvl);
                InfoManager.GetInstance().userData.currentShip.lvl += 1;
            }
            else
            {
                InfoManager.GetInstance().userData.currentShip.lvl += 1;
            }
        }
    }
    
    /// <summary>
    /// 解锁战舰
    /// </summary>
    /// <param name="shipInfo"></param>
    public void TryUnLockShip(ShipInfo shipInfo)
    {
        if(shipInfo != null)
        {
            UserData userdata = InfoManager.GetInstance().userData;
            ShipConfig shipConfig = new ShipConfig();
            shipConfig.shipConfigId = userdata.userShips.Max(s => s.shipConfigId) + 1;
            shipConfig.lvl = 1;
            shipConfig.activeWeaponCom = GiveNewCom(InfoManager.GetInstance().weaponInfos[0]) as ShipWeaponCom;
            shipConfig.activeArmorCom = GiveNewCom(InfoManager.GetInstance().armorInfos[0]) as ShipArmorCom;
            shipConfig.activeEngineCom = GiveNewCom(InfoManager.GetInstance().engineInfos[0]) as ShipEngineCom;
            shipConfig.shipInfo = shipInfo;

            userdata.userShips.Add(shipConfig);
            userdata.currentShip = shipConfig;

            if(shipInfo.get == 1)//碎片获取方式则扣除碎片
            {
                userdata.debris[InfoManager.debrisIndexOfShipInfo[shipInfo.id]] -= Utils.CalShipUnlockCost(shipInfo);
            }

            //notice
            MenuNotice.Notice(shipInfo.displayName, "获得战舰 " + shipInfo.displayName, Color.green);
        }
    }

    
    /// <summary>
    /// 升级组件
    /// </summary>
    /// <param name="com"></param>
    public void TryUpgradeCom(ShipCom com, bool forFree = false)
    {
        if(com.lvl  < 30)
        {
            if (!forFree)
            {
                InfoManager.GetInstance().userData.equipmentMat -= Utils.CalComUpgradeCost(com.lvl, com.EquipmentInfo.ranklvl);
                com.lvl += 1;
            }
            else
            {
                com.lvl += 1;
            }

            MenuNotice.Notice(com.EquipmentInfo.displayName, "装备已升级", Color.green);
        }
    }

    /// <summary>
    /// 卸载组件
    /// </summary>
    /// <param name="com"></param>
    public void TryDetachCom(ShipCom com)
    {
        foreach(var ship in InfoManager.GetInstance().userData.userShips)
        {
            if(ship.activeWeaponCom == com)
            {
                ship.activeWeaponCom = null;
            }
            if (ship.activeArmorCom == com)
            {
                ship.activeArmorCom = null;
            }
            if (ship.activeEngineCom == com)
            {
                ship.activeEngineCom = null;
            }
        }
    }

    /// <summary>
    /// 分解组件
    /// </summary>
    /// <param name="com"></param>
    public void TryDecomposeCom(ShipCom com, bool getAll)
    {
        if(com is ShipWeaponCom)
        {
            InfoManager.GetInstance().userData.myWeaponComs.Remove(com as ShipWeaponCom);
            InfoManager.GetInstance().userData.currentShip.activeWeaponCom = null;
        }
        else if (com is ShipArmorCom)
        {
            InfoManager.GetInstance().userData.myArmorComs.Remove(com as ShipArmorCom);
            InfoManager.GetInstance().userData.currentShip.activeArmorCom = null;
        }
        else if (com is ShipEngineCom)
        {
            InfoManager.GetInstance().userData.myEngineComs.Remove(com as ShipEngineCom);
            InfoManager.GetInstance().userData.currentShip.activeEngineCom = null;
        }

        if (getAll)
        {
            InfoManager.GetInstance().userData.equipmentMat += Utils.CalComDecomGet(com.lvl, com.EquipmentInfo.ranklvl, 1f);
        }
        else
        {
            InfoManager.GetInstance().userData.equipmentMat += Utils.CalComDecomGet(com.lvl, com.EquipmentInfo.ranklvl, 0.1f);
        }

        MenuNotice.Notice(com.EquipmentInfo.displayName, "装备已经拆解", Color.yellow);
    }

    /// <summary>
    /// 普通招募
    /// </summary>
    public void TryHire(bool isDec)
    {
        if (!isDec)//单
        {
            if (InfoManager.GetInstance().userData.money < 100)
            {
                //???提示金币不足
                ShowWindowGetMoney();
                return;
            }


            InfoManager.GetInstance().userData.money -= 100;
            ShowWindowHire(false);//在窗口刷新中随机概率
        }
        else//十
        {
            if (InfoManager.GetInstance().userData.money < 1000)
            {
                //???提示金币不足
                ShowWindowGetMoney();
                return;
            }
            
            InfoManager.GetInstance().userData.money -= 1000;
            ShowWindowHire(true);//在窗口刷新中随机概率
        }
    }

    /// <summary>
    /// 签到
    /// </summary>
    /// <param name="getDouble"></param>
    public void TryCheckIn(bool getDouble = false)
    {
        string dateNow = System.DateTime.Now.ToShortDateString();
        for(int i = 0; i < 7; i++)
        {
            int iTmp = i;
            if(InfoManager.GetInstance().userData.checkins[i] == dateNow)
            {
                MenuNotice.Notice("Error", "今日已签到过！！", Color.red);
                break;
            }
            if (InfoManager.GetInstance().userData.checkins[i] == "")
            {
                if (!getDouble)
                {
                    GiveAchieveBonus(InfoManager.GetInstance().checkinBonusList[iTmp]);
                    InfoManager.GetInstance().userData.checkins[i] = System.DateTime.Now.ToShortDateString();
                    RefreshWindowCheckin();
                }
                else
                {
                    //埋点
                    MaiDian.Mai("number", 0, "version", InfoManager.Version, "startVideo");

                    AdManager.VideoAd(new UnityAction(() => {
                        //埋点
                        MaiDian.Mai("number", 0, "version", InfoManager.Version, "endVideo");

                        GiveAchieveBonus(InfoManager.GetInstance().checkinBonusList[iTmp]);
                        GiveAchieveBonus(InfoManager.GetInstance().checkinBonusList[iTmp]);//double
                        InfoManager.GetInstance().userData.checkins[iTmp] = System.DateTime.Now.ToShortDateString();
                        RefreshWindowCheckin();
                    }), () => {
                        //donothing
                    });
                }
                break;
            }
        }
    }




    /// <summary>
    /// Give: Money /Mat / Com
    /// </summary>
    public void GiveAchieveBonus(AchieveBonus bonus)//1战舰，2装备，3船员，4金币
    {
        switch (bonus.type)
        {
            case 1:
                TryUnLockShip(InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.displayName == bonus.name));
                break;
            case 2:
                EquipmentInfo equip = InfoManager.GetInstance().allEquipmentInfos.FirstOrDefault(s => s.displayName == bonus.name);
                if(equip != null) GiveNewCom(equip);
                break;
            case 3:
                GetCrew(bonus.name);
                break;
            case 4:
                GiveMoney(bonus.amount);
                break;
            default:
                break;
        }

        MenuNotice.Notice("null", "奖励已领取", Color.green);
    }



    public static void GiveMoney(int count)
    {
        InfoManager.GetInstance().userData.money += count;
        //notice
        MenuNotice.Notice("金币", "获得金币 " + count, Color.green);

        if(inst != null)
        {
            inst.RefreshUserAssetsPanel();
            inst.RefreshShipUpgradePanel(InfoManager.GetInstance().userData.currentShip.shipInfo);
            inst.RefreshWindowCrew();
        }
    }

    public static void GiveMaterial(int count)
    {
        InfoManager.GetInstance().userData.equipmentMat += count;
        //notice
        MenuNotice.Notice("材料", "获得材料 " + count, Color.green);

        if (inst != null)
        {
            inst.RefreshUserAssetsPanel();
            inst.RefreshEquipmentInfoPanel(inst.selectedCom, false);
        }

    }

    public static ShipCom GiveNewComOfRank(string rank)
    {
        List<EquipmentInfo> list = new List<EquipmentInfo>();
        list.AddRange(InfoManager.GetInstance().weaponInfos);
        list.AddRange(InfoManager.GetInstance().armorInfos);
        list.AddRange(InfoManager.GetInstance().engineInfos);

        EquipmentInfo[] infosThisRank = list.Where(i => i.rank == rank).ToArray();

        return GiveNewCom(infosThisRank[Random.Range(0, infosThisRank.Length)]);
    }

    public static ShipCom GiveNewCom(EquipmentInfo equipmentInfo)
    {
        List<ShipCom> userAllComs = new List<ShipCom>();
        userAllComs.AddRange(InfoManager.GetInstance().userData.myWeaponComs);
        userAllComs.AddRange(InfoManager.GetInstance().userData.myArmorComs);
        userAllComs.AddRange(InfoManager.GetInstance().userData.myEngineComs);

        int index = userAllComs.Max(c => c.comId) + 1;

        ShipCom com;
        switch (equipmentInfo.GetType().Name)
        {
            case "WeaponInfo":
                {
                    com = new ShipWeaponCom();
                    (com as ShipWeaponCom).weaponInfo = equipmentInfo as WeaponInfo;
                    InfoManager.GetInstance().userData.myWeaponComs.Add(com as ShipWeaponCom);
                    InfoManager.GetInstance().userData.myWeaponComs = InfoManager.GetInstance().userData.myWeaponComs.OrderBy(c => c.lvl).ToList();
                }
                break;
            case "ArmorInfo":
                {
                    com = new ShipArmorCom();
                    (com as ShipArmorCom).armorInfo = equipmentInfo as ArmorInfo;
                    InfoManager.GetInstance().userData.myArmorComs.Add(com as ShipArmorCom);
                    InfoManager.GetInstance().userData.myArmorComs = InfoManager.GetInstance().userData.myArmorComs.OrderBy(c => c.lvl).ToList();
                }
                break;
            case "EngineInfo":
                {
                    com = new ShipEngineCom();
                    (com as ShipEngineCom).engineInfo = equipmentInfo as EngineInfo;
                    InfoManager.GetInstance().userData.myEngineComs.Add(com as ShipEngineCom);
                    InfoManager.GetInstance().userData.myEngineComs = InfoManager.GetInstance().userData.myEngineComs.OrderBy(c => c.lvl).ToList();
                }
                break;
            default:
                {
                    throw new UnityException("FALSE TYPE");
                    com = new ShipWeaponCom();
                    (com as ShipWeaponCom).weaponInfo = equipmentInfo as WeaponInfo;
                    InfoManager.GetInstance().userData.myWeaponComs.Add(com as ShipWeaponCom);
                    InfoManager.GetInstance().userData.myWeaponComs = InfoManager.GetInstance().userData.myWeaponComs.OrderBy(c => c.lvl).ToList();
                }
                break;
        }
        com.lvl = 1;
        com.comId = index;
        return com;

        //return null;
    }


    public static void GetCrew(string name)
    {
        bool hasCrew = InfoManager.GetInstance().userData.crews.Any(c => c.crewInfo.name == name);
        if (!hasCrew)
        {
            CrewInfo crewInfo = InfoManager.GetInstance().crewInfos.FirstOrDefault(c => c.name == name);
            if (crewInfo != null)
            {
                Crew crew = new Crew();
                crew.crewInfo = crewInfo;
                crew.lvl = 1;
                InfoManager.GetInstance().userData.crews.Add(crew);
            }

        }
        else
        {
        }
    }



    public void AddTotalGameCount()
    {
        InfoManager.GetInstance().userData.totalGames += 1;
        

        if(InfoManager.GetInstance().userData.totalGames < 11)
        {
            MaiDian.Mai("number", InfoManager.GetInstance().userData.totalGames, "version", InfoManager.Version, "startLevel");
        }
    }
    //-----
    

    public void SaveUserData()
    {
        PlayerPrefs.SetString("UserData", UserData.Serialize(InfoManager.GetInstance().userData));

#if UNITY_EDITOR
        //Debug 保存到桌面
        Debug.Log("==SAVE==\n" + UserData.Serialize(InfoManager.GetInstance().userData));
#endif
    }

    public void DelUserData()
    {
        PlayerPrefs.DeleteAll();
        InfoManager.GetInstance().userData = UserData.Deserialize(Resources.Load<TextAsset>("Json/UserDataDefault").text, InfoManager.GetInstance());
        ToggleWindowMain();
    }

    //-----

    public void Cheat()
    {
        InfoManager.GetInstance().userData = UserData.Deserialize(Resources.Load<TextAsset>("Json/UserDataTest").text, InfoManager.GetInstance());
        ToggleWindowMain();
    }



    public void OnGUI()
    {
        //GUILayout.TextArea("当前关卡类型：" + InfoManager.currentMissionType);
        //GUILayout.TextArea("玩家关卡进度: " + InfoManager.GetInstance().userData.lvlsProgress[0] + "," + InfoManager.GetInstance().userData.lvlsProgress[1] + "," + InfoManager.GetInstance().userData.lvlsProgress[2]);
    }
}
