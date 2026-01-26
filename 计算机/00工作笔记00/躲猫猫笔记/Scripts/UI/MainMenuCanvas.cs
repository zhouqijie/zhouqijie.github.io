using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

using DG.Tweening;


public class MainMenuCanvas : MonoBehaviour
{
    //---------------------------------Info------------------------------------------

    private float cellDistanceSetting = 150f;
    private int cellCountAvatars = 9;
    private int cellCountBorders = 15;

    private float cellDistanceShop = 150f;
    private int cellCountChara = 16;
    private int cellCountDecos = 16;
    private int cellCountDsgs = 16;

    //---------------------------------Coms------------------------------------------

    /// <summary>
    /// 用户资源面板
    /// </summary>
    [HideInInspector] public Transform userAssetsPanel;
    /// <summary>
    /// 主窗口列表
    /// </summary>
    private List<Transform> windows = new List<Transform>();
    /// <summary>
    /// 已打开的浮动窗口列表
    /// </summary>
    private List<Transform> floatWindows = new List<Transform>();


    private Transform windowMain;
    private Transform windowClassic;
    private Transform windowTalents;
    private Transform windowShop;
    private Transform windowActivity;
    private Transform windowLogin;
    private Transform windowCheckin;
    private Transform windowSettings;
    private Transform windowMatch;
    private Transform windowLoading;

    private Transform windowExchange;
    private Transform windowGetMoney;
    private Transform windowGot;
    private Transform windowVip;
    private Transform windowTry;
    private Transform windowChat;
    private Transform windowMissions;

    private Transform tabAvatars;
    private Transform tabBorders;
    private Transform tabSounds;


    private Transform tabCharactors;
    private Transform tabDecorations;
    private Transform tabDisguises;


    private PreviewController charactorPreview = null;










    //-----------------------------------------------------------------------------------------------------

    private void Awake()
    {
        userAssetsPanel = this.transform.Find("Windows").Find("---UserAssetsPanel---");

        windowMain = this.transform.Find("Windows").Find("WindowMain");
        windowClassic = this.transform.Find("Windows").Find("WindowClassic");
        windowTalents = this.transform.Find("Windows").Find("WindowTalents");
        windowShop = this.transform.Find("Windows").Find("WindowShop");
        windowActivity = this.transform.Find("Windows").Find("WindowActivity");
        windowLogin = this.transform.Find("Windows").Find("WindowLogin");
        windowCheckin = this.transform.Find("Windows").Find("WindowCheckin");
        windowSettings = this.transform.Find("Windows").Find("WindowSettings");
        windowMatch = this.transform.Find("Windows").Find("WindowMatch");
        windowLoading  = this.transform.Find("Windows").Find("WindowLoading");
        

        windowExchange = this.transform.Find("Windows").Find("WindowExchange");
        windowGetMoney = this.transform.Find("Windows").Find("WindowGetMoney");
        windowGot = this.transform.Find("Windows").Find("WindowGot");
        windowVip = this.transform.Find("Windows").Find("WindowVip");
        windowTry = this.transform.Find("Windows").Find("WindowTry");
        windowMissions = this.transform.Find("Windows").Find("WindowMissions");
        windowChat = this.transform.Find("Windows").Find("WindowChat");
        

        tabAvatars = windowSettings.Find("PanelSettings").GetChild(0);
        tabBorders = windowSettings.Find("PanelSettings").GetChild(1);
        tabSounds = windowSettings.Find("PanelSettings").GetChild(2);

        tabCharactors = windowShop.Find("CharactorsPanel");
        tabDecorations = windowShop.Find("DecorationsPanel");
        tabDisguises = windowShop.Find("DisguisesPanel");



        windows.Add(windowMain);
        windows.Add(windowClassic);
        windows.Add(windowTalents);
        windows.Add(windowShop);
        windows.Add(windowActivity);
        windows.Add(windowCheckin);
        windows.Add(windowMatch);
        windows.Add(windowLoading);

        if (charactorPreview == null)
        {
            charactorPreview = PreviewController.Create(this.transform);
        }
        //charactorPreview = this.GetComponentInChildren<PreviewController>(true); charactorPreview.Awake();//非active 物体不会自动awake
    }


    void Start()
    {
        GiveMoney(0);//刷新资源面板
        GiveDiamond(0);//刷新资源面板

        ToggleWindowMain();//默认窗口

        SoundPlayer.ApplyVolume();//背景音效
    }


    void Update()
    {
        //Test
        if (Input.GetKeyDown(KeyCode.T))
        {
            Badge.levelPrevious = Infomanager.GetInstance().userData.level;
            Infomanager.GetInstance().userData.level = Mathf.Clamp(Infomanager.GetInstance().userData.level + 1, 0, Infomanager.maxLevel);
            Badge.levelCurrent = Infomanager.GetInstance().userData.level;

            Badge.Anim(windowMain.Find("PanelBadge").Find("Badge"));
        }
    }

    //-----------------------------------------------------------------------------------------------------

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Utils/Userdata/SaveToDesktop")]
    public static void SaveUserdataToDesktop()
    {
        string json = UserData.Serialize(Infomanager.GetInstance().userData);

        System.IO.File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\userdata.json", json);
    }
#endif
    public static void SaveUserdata()
    {
        string json = UserData.Serialize(Infomanager.GetInstance().userData);

        PlayerPrefs.SetString("UserData", json);
    }

    //----------------------------------------ButtomCLick-----------------------------------------------------


    public void ButtonToggleMain()
    {
        ToggleWindowMain();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonOpenClassic()
    {
        ToggleWindowClassic();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonOpenTalent()
    {
        ToggleWindowTalents();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonOpenShop()
    {
        ToggleWindowShop();

        SoundPlayer.PlaySound2D("click");
    }
    public void BottonOpenActivity()
    {
        ToggleWindowActivity();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonOpenCheckin()
    {
        ToggleWindowCheckin();

        SoundPlayer.PlaySound2D("click");
    }



    public void ButtonTabChara()
    {
        TabCharactors();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonTabDeco()
    {
        TabDecorations();

        SoundPlayer.PlaySound2D("click");
    }
    public void ButtonTabDsg()
    {
        TabDisguises();

        SoundPlayer.PlaySound2D("click");
    }

    public void ButtonClickSound()
    {
        SoundPlayer.PlaySound2D("click");
    }

    //-----------------------------------------切换窗口-------------------------------------------------------

    ///主要窗口  切换
    public void ToggleWindowMain()
    {
        //埋点
        MaiDian.Mai("number", 1, "version", Infomanager.Version, "goInterface");

        foreach (var w in windows)
        {
            if(w == windowMain)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshWindowMain();

        charactorPreview.ResetParent(windowMain.Find("PanelPreview"));


        //每日登录弹窗
        if (Infomanager.newDay)
        {
            Infomanager.newDay = false;

            ShowHideWindowLogin(true);
        }

        //保存用户数据
        SaveUserdata();
    }

    public void ToggleWindowClassic()
    {
        foreach (var w in windows)
        {
            if (w == windowClassic)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshWindowClassicChoose();
    }

    public void ToggleWindowTalents()
    {
        //埋点
        MaiDian.Mai("number", 28, "version", Infomanager.Version, "goInterface");


        foreach (var w in windows)
        {
            if (w == windowTalents)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshWindowTalents();
    }

    public void ToggleWindowShop()
    {
        //埋点
        MaiDian.Mai("number", 23, "version", Infomanager.Version, "goInterface");


        foreach (var w in windows)
        {
            if (w == windowShop)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        TabCharactors();//默认切换角色页
    }

    public void ToggleWindowActivity()
    {
        //埋点
        MaiDian.Mai("number", 24, "version", Infomanager.Version, "goInterface");


        foreach (var w in windows)
        {
            if (w == windowActivity)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshActivityWindow();
    }

    public void ToggleWindowCheckin()
    {
        //埋点
        MaiDian.Mai("number", 25, "version", Infomanager.Version, "goInterface");

        foreach (var w in windows)
        {
            if (w == windowCheckin)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshCheckinWindow(true);
    }

    public void ToggleWindowMatch()
    {
        foreach (var w in windows)
        {
            if (w == windowMatch)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshWindowMatch();
    }

    public void ToggleWindowLoading()
    {
        foreach (var w in windows)
        {
            if (w == windowLoading)
            {
                w.gameObject.SetActive(true);
                continue;
            }
            w.gameObject.SetActive(false);
        }

        RefreshWindowLoading();
    }


    //浮动窗口(refresh when show)
    public void ShowHideWindowSettings(bool show)
    {
        if (show)
        {
            windowSettings.gameObject.SetActive(true);
            TabAvatar();//默认角色头像选项卡
        }
        else
        {
            windowSettings.gameObject.SetActive(false);
        }
        
        //float windows list
        if (show)
        {
            floatWindows.Add(windowSettings);
        }
        else
        {
            if(floatWindows.Contains(windowSettings)) floatWindows.Remove(windowSettings);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowChat(bool show)
    {
        //埋点
        if (show)
        {
            MaiDian.Mai("number", 27, "version", Infomanager.Version, "goInterface");
        }

        //如果已经显示，则关闭
        if (show && floatWindows.Contains(windowChat))
        {
            ShowHideWindowChat(false);
            return;
        }

        //开关动画
        windowChat.gameObject.GetComponent<RectTransform>().anchorMin = show ? new Vector2(0, 0) : new Vector2(-1f, 0);
        windowChat.gameObject.GetComponent<RectTransform>().anchorMax = show ? new Vector2(0.6f, 1f) : new Vector2(-0.4f, 1f);

        //float windows list
        if (show)
        {
            floatWindows.Add(windowChat);
        }
        else
        {
            if (floatWindows.Contains(windowChat)) floatWindows.Remove(windowChat);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowLogin(bool show)
    {
        if (show)
        {
            windowLogin.gameObject.SetActive(true);


            //放大
            windowLogin.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowLogin.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);


            //Bonus
            DecorationInfo[] decosNotHave = Infomanager.GetInstance().decorationInfos.Where(d => Infomanager.GetInstance().userData.myDecorations.Contains(d)).ToArray();
            DisguiseObjectInfo[] dsgNotHave = Infomanager.GetInstance().disguiseObjInfos.Where(d => Infomanager.GetInstance().userData.myDisguiseObjects.Contains(d)).ToArray();

            Bonus bonus1 = new Bonus();
            bonus1.type = 0;
            bonus1.name = "money";
            bonus1.amount = 200;

            windowLogin.Find("ImageGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Coins");
            windowLogin.Find("ImageGet1").GetComponentInChildren<Text>().text = "<b>" + "×" + bonus1.amount + "</b>";


            Bonus bonus2 = new Bonus();
            if (Random.value < 0.5f && decosNotHave.Length > 0)
            {
                var info = decosNotHave[Random.Range(0, decosNotHave.Length)];

                bonus2.type = 2;
                bonus2.name = info.name;
                bonus2.amount = 1;


                windowLogin.Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + bonus2.name);

                windowLogin.Find("ImageGet2").Find("Image").gameObject.SetActive(true);
                windowLogin.Find("ImageGet2").Find("Text").gameObject.SetActive(false);
                windowLogin.Find("ImageGet2").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + bonus2.name);
                windowLogin.Find("ImageGet2").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            else if(dsgNotHave.Length > 0)
            {
                var info = dsgNotHave[Random.Range(0, dsgNotHave.Length)];

                bonus2.type = 3;
                bonus2.name = info.name;
                bonus2.amount = 1;


                windowLogin.Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + bonus2.name);

                windowLogin.Find("ImageGet2").Find("Image").gameObject.SetActive(true);
                windowLogin.Find("ImageGet2").Find("Text").gameObject.SetActive(false);
                windowLogin.Find("ImageGet2").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + bonus2.name);
                windowLogin.Find("ImageGet2").Find("Image").GetComponent<Image>().SetNativeSize();
            }
            else
            {
                bonus2.type = 4;
                bonus2.name = "diamond";
                bonus2.amount = 50;


                windowLogin.Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Diamonds");

                windowLogin.Find("ImageGet2").Find("Image").gameObject.SetActive(false);
                windowLogin.Find("ImageGet2").Find("Text").gameObject.SetActive(true);
                windowLogin.Find("ImageGet2").GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 1f);
                windowLogin.Find("ImageGet2").GetComponentInChildren<Text>().text = "<b>" + " ×" + bonus2.amount + "</b>";
            }


            windowLogin.Find("ImageGet1").GetComponent<Image>().SetNativeSize();
            windowLogin.Find("ImageGet2").GetComponent<Image>().SetNativeSize();

            windowLogin.Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
            windowLogin.Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
            windowLogin.Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                GiveBonus(bonus1, true);

                ShowHideWindowLogin(false);
            });
            windowLogin.Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {
                AdManager.VideoAd(() => {

                    GiveBonus(bonus1, false);
                    GiveBonus(bonus2, true);

                    ShowHideWindowLogin(false);

                }, () => { });
            });

        }
        else
        {
            windowLogin.gameObject.SetActive(false);
        }


        //float windows list
        if (show)
        {
            floatWindows.Add(windowLogin);
        }
        else
        {
            if (floatWindows.Contains(windowLogin)) floatWindows.Remove(windowLogin);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowExchange(bool show)
    {
        if (show)
        {
            windowExchange.gameObject.SetActive(true);

            //放大
            windowExchange.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowExchange.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);



            //Btn
            windowExchange.Find("ButtonOk").GetComponent<Button>().onClick.RemoveAllListeners();
            windowExchange.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(() => {
                string text = windowExchange.GetComponentInChildren<InputField>().text;
                GiveBonusViaCode(text);
            });
            windowExchange.Find("ButtonCancel").GetComponent<Button>().onClick.RemoveAllListeners();
            windowExchange.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(() => {
                ShowHideWindowExchange(false);
            });
        }
        else
        {
            windowExchange.gameObject.SetActive(false);
        }





        //float windows list
        if (show)
        {
            floatWindows.Add(windowExchange);
        }
        else
        {
            if (floatWindows.Contains(windowExchange)) floatWindows.Remove(windowExchange);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowMissions(bool show)
    {
        if (show) //dotween anim??
        {
            //埋点
            MaiDian.Mai("number", 26, "version", Infomanager.Version, "goInterface");


            windowMissions.gameObject.SetActive(true);

            RefreshWindowMissions();
        }
        else
        {
            windowMissions.gameObject.SetActive(false);
        }

        //float windows list
        if (show)
        {
            floatWindows.Add(windowMissions);
        }
        else
        {
            if (floatWindows.Contains(windowMissions)) floatWindows.Remove(windowMissions);
        }
        CheckCharacterHide();
    }
    
    public void ShowHideWindowGetMoney(bool show, bool getDiamond)
    {
        windowGetMoney.gameObject.SetActive(show);

        //refresh
        if (show)
        {

            //放大
            windowGetMoney.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowGetMoney.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);


            if (!getDiamond)
            {
                windowGetMoney.Find("ImagesMoney").gameObject.SetActive(true);
                windowGetMoney.Find("ImagesDiamonds").gameObject.SetActive(false);

                windowGetMoney.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();
                windowGetMoney.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {
                    AdManager.VideoAd(() => {
                        GiveMoney(500);
                        ShowHideWindowGetMoney(false, false);

                        //sound
                        SoundPlayer.PlaySound2D("coins");

                        //effects
                        for (int i = 0; i < 10; i++)
                        {
                            ImageEffects.ImageBoomAndFly(this.GetComponent<RectTransform>(), this.userAssetsPanel.Find("ImageCoin").GetComponent<RectTransform>(), "Items/Coin", new Vector2(50, 50), 100f, 0.4f);
                        }

                    }, () => { });
                });
            }
            else
            {
                windowGetMoney.Find("ImagesMoney").gameObject.SetActive(false);
                windowGetMoney.Find("ImagesDiamonds").gameObject.SetActive(true);

                windowGetMoney.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();
                windowGetMoney.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {
                    AdManager.VideoAd(() => {
                        GiveDiamond(100);
                        ShowHideWindowGetMoney(false, true);

                        //sound
                        SoundPlayer.PlaySound2D("coins");

                        //effects
                        for (int i = 0; i < 10; i++)
                        {
                            ImageEffects.ImageBoomAndFly(this.GetComponent<RectTransform>(), this.userAssetsPanel.Find("ImageDiamond").GetComponent<RectTransform>(), "Items/Diamond", new Vector2(50, 50), 100f, 0.4f);
                        }

                    }, () => { });
                });
            }

            windowGetMoney.Find("ButtonReturn").GetComponent<Button>().onClick.RemoveAllListeners();
            windowGetMoney.Find("ButtonReturn").GetComponent<Button>().onClick.AddListener(() => {
                ShowHideWindowGetMoney(false, getDiamond);
            });
        }


        //float windows list
        if (show)
        {
            floatWindows.Add(windowGetMoney);
        }
        else
        {
            if (floatWindows.Contains(windowGetMoney)) floatWindows.Remove(windowGetMoney);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowGot(bool show, Bonus bonusInfo)
    {
        if (show)
        {
            windowGot.gameObject.SetActive(true);

            //放大
            windowGot.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowGot.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);


            SoundPlayer.PlaySound2D("unlock");

            switch (bonusInfo.type)
            {
                case 0:
                    {
                        windowGot.Find("ImageGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Coins");
                        windowGot.Find("ImageGot").GetComponent<Image>().SetNativeSize();

                        windowGot.Find("ImageTextGot").gameObject.SetActive(false);
                        windowGot.Find("TextGot").gameObject.SetActive(true);
                        windowGot.Find("TextGot").GetComponent<Text>().text = "<b>× " + bonusInfo.amount + "</b>";
                    }
                    break;
                case 1:
                    {
                        windowGot.Find("ImageGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterHeads/" + bonusInfo.name);
                        windowGot.Find("ImageGot").GetComponent<Image>().SetNativeSize();


                        windowGot.Find("ImageTextGot").gameObject.SetActive(true);
                        windowGot.Find("TextGot").gameObject.SetActive(false);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterNames/" + bonusInfo.name);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().SetNativeSize();
                    }
                    break;
                case 2:
                    {
                        windowGot.Find("ImageGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + bonusInfo.name);
                        windowGot.Find("ImageGot").GetComponent<Image>().SetNativeSize();


                        windowGot.Find("ImageTextGot").gameObject.SetActive(true);
                        windowGot.Find("TextGot").gameObject.SetActive(false);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + bonusInfo.name);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().SetNativeSize();
                    }
                    break;
                case 3:
                    {
                        windowGot.Find("ImageGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + bonusInfo.name);
                        windowGot.Find("ImageGot").GetComponent<Image>().SetNativeSize();


                        windowGot.Find("ImageTextGot").gameObject.SetActive(true);
                        windowGot.Find("TextGot").gameObject.SetActive(false);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + bonusInfo.name);
                        windowGot.Find("ImageTextGot").GetComponent<Image>().SetNativeSize();
                    }
                    break;
                case 4:
                    {
                        windowGot.Find("ImageGot").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Diamonds");
                        windowGot.Find("ImageGot").GetComponent<Image>().SetNativeSize();

                        windowGot.Find("ImageTextGot").gameObject.SetActive(false);
                        windowGot.Find("TextGot").gameObject.SetActive(true);
                        windowGot.Find("TextGot").GetComponent<Text>().text = "<b>× " + bonusInfo.amount + "</b>";
                    }
                    break;

                default:
                    break;
            }



            //btn
            windowGot.Find("ButtonOk").GetComponent<Button>().onClick.RemoveAllListeners();
            windowGot.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(() => {
                ShowHideWindowGot(false, null);
            });
        }
        else
        {
            windowGot.gameObject.SetActive(false);
        }

        //float windows list
        if (show)
        {
            floatWindows.Add(windowGot);
        }
        else
        {
            if (floatWindows.Contains(windowGot)) floatWindows.Remove(windowGot);
        }
        CheckCharacterHide();
        
    }

    public void ShowHideWindowVip(bool show, int level)
    {
        if (show)
        {

            windowVip.gameObject.SetActive(true);

            //放大
            windowVip.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowVip.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);

            //SOunds
            SoundPlayer.PlaySound2D("unlock");


            windowVip.Find("ImageLevel").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Vips/vip" + level.ToString());
            windowVip.Find("ButtonOk").GetComponent<Button>().onClick.RemoveAllListeners();
            windowVip.Find("ButtonOk").GetComponent<Button>().onClick.AddListener(() => {
                ShowHideWindowVip(false, 0);
            });

        }
        else
        {
            windowVip.gameObject.SetActive(false);
        }

        //float windows list
        if (show)
        {
            floatWindows.Add(windowVip);
        }
        else
        {
            if (floatWindows.Contains(windowVip)) floatWindows.Remove(windowVip);
        }
        CheckCharacterHide();
    }

    public void ShowHideWindowTry(bool show, UnityAction callback, Bonus bonus)
    {
        
        if (show)
        {
            
            windowTry.gameObject.SetActive(true);


            //放大
            windowTry.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            windowTry.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);//; = new Vector3(0f, 0f, 0f);


            //imagessprite and btn-try
            windowTry.Find("ButtonTry").GetComponent<Button>().onClick.RemoveAllListeners();
            switch (bonus.type)
            {
                case 1:
                    {
                        windowTry.Find("ImageTry").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterHeads/" + bonus.name);
                        windowTry.Find("ImageTextTry").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterNames/" + bonus.name);
                        windowTry.Find("ButtonTry").GetComponent<Button>().onClick.AddListener(() => {

                            //埋点
                            MaiDian.Mai("number", 8, "version", Infomanager.Version, "startVideo");

                            AdManager.VideoAd(() => {

                                //埋点
                                MaiDian.Mai("number", 8, "version", Infomanager.Version, "endVideo");

                                //...tmp deco
                                SetSkin(Infomanager.GetInstance().skinInfos.FirstOrDefault(s => s.name == bonus.name), force: true);

                                //hide
                                ShowHideWindowTry(false, null, null);

                                //next operation
                                callback();
                            }, () => {

                            });
                        });
                    }
                    break;
                case 2:
                    {
                        windowTry.Find("ImageTry").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + bonus.name);
                        windowTry.Find("ImageTextTry").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + bonus.name);
                        windowTry.Find("ButtonTry").GetComponent<Button>().onClick.AddListener(() => {

                            //埋点
                            MaiDian.Mai("number", 8, "version", Infomanager.Version, "startVideo");

                            AdManager.VideoAd(() => {

                                //埋点
                                MaiDian.Mai("number", 8, "version", Infomanager.Version, "endVideo");

                                //...tmp deco
                                SetDecoration(Infomanager.GetInstance().decorationInfos.FirstOrDefault(s => s.name == bonus.name), force: true);

                                //hide
                                ShowHideWindowTry(false, null, null);

                                //next operation
                                callback();
                            }, () => {

                            });
                        });
                    }
                    break;
                default:
                    break;
            }
            //set native size
            windowTry.Find("ImageTry").GetComponent<Image>().SetNativeSize();
            windowTry.Find("ImageTextTry").GetComponent<Image>().SetNativeSize();

            //btn-cancel
            windowTry.Find("ButtonCancel").GetComponent<Button>().onClick.RemoveAllListeners();
            windowTry.Find("ButtonCancel").GetComponent<Button>().onClick.AddListener(() => {
                ShowHideWindowTry(false, null, null);

                //next operation
                callback();
            });

        }
        else
        {
            windowTry.gameObject.SetActive(false);
        }





        //float windows list
        if (show)
        {
            floatWindows.Add(windowTry);
        }
        else
        {
            if (floatWindows.Contains(windowTry)) floatWindows.Remove(windowTry);
        }
        CheckCharacterHide();
    }

    //隐藏角色
    private void CheckCharacterHide()
    {
        //hide /show preview
        if (floatWindows.Count > 0)
        {
            charactorPreview.gameObject.SetActive(false);
        }
        else
        {
            charactorPreview.gameObject.SetActive(true);
            charactorPreview.ResetCharactor();
        }
    }


    /// 选项卡切换
    public void TabAvatar()
    {
        tabAvatars.gameObject.SetActive(true);
        tabBorders.gameObject.SetActive(false);
        tabSounds.gameObject.SetActive(false);
        //refresh
        RefreshPanelAvatars();
    }
    public void TabBorders()
    {
        tabAvatars.gameObject.SetActive(false);
        tabBorders.gameObject.SetActive(true);
        tabSounds.gameObject.SetActive(false);
        //refresh
        RefreshPanelBorders();
    }
    public void TabSounds()
    {
        tabAvatars.gameObject.SetActive(false);
        tabBorders.gameObject.SetActive(false);
        tabSounds.gameObject.SetActive(true);
        //refresh
        RefreshPanelSounds();
    }

    /// 选项卡切换
    public void TabCharactors()
    {
        tabCharactors.gameObject.SetActive(true);
        tabDecorations.gameObject.SetActive(false);
        tabDisguises.gameObject.SetActive(false);

        RefreshPanelCharactors();
        charactorPreview.ResetParent(tabCharactors.Find("PanelPreview"));
    }
    public void TabDecorations()
    {
        tabCharactors.gameObject.SetActive(false);
        tabDecorations.gameObject.SetActive(true);
        tabDisguises.gameObject.SetActive(false);

        RefreshPanelDecorations();
        charactorPreview.ResetParent(tabDecorations.Find("PanelPreview"));
    }
    public void TabDisguises()
    {
        tabCharactors.gameObject.SetActive(false);
        tabDecorations.gameObject.SetActive(false);
        tabDisguises.gameObject.SetActive(true);

        RefreshPanelDisguises();
    }

    //-----------------------------------------刷新页面-------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 刷新主界面
    /// </summary>
    public void RefreshWindowMain()
    {
        //用户头像名称
        windowMain.Find("PanelAvatar").Find("ImageAvatar").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Avatars/" + Infomanager.GetInstance().userData.avatarId);
        windowMain.Find("PanelAvatar").Find("ImageBorder").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Borders/" + Infomanager.GetInstance().userData.borderId);
        windowMain.Find("PanelAvatar").Find("TextName").GetComponent<Text>().text = Infomanager.GetInstance().userData.name;

        //当前预览角色皮肤名称
        windowMain.Find("PanelPreview").Find("TextSkinName").GetComponent<Text>().text = Infomanager.GetInstance().userData.activeSkin.name;
        windowMain.Find("PanelPreview").Find("ImageRare").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/" + Infomanager.GetInstance().userData.activeSkin.rank);

        //刷新段位显示
        Badge.Set(windowMain.Find("PanelBadge").Find("Badge"), Infomanager.GetInstance().userData.level);

        //刷新vip显示（晋级自动弹窗）
        if(Vip.GetLevel(Infomanager.GetInstance().userData.vipExp) > Infomanager.GetInstance().userData.vipLvl)
        {
            Infomanager.GetInstance().userData.vipLvl = Vip.GetLevel(Infomanager.GetInstance().userData.vipExp);
            ShowHideWindowVip(true, Vip.GetLevel(Infomanager.GetInstance().userData.vipExp));
        }
        windowMain.Find("PanelAvatar").Find("ImageVip").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Vips/vip" + Infomanager.GetInstance().userData.vipLvl);




        //进入游戏按钮回调（当新手教程未完成时）
        
        if(Infomanager.GetInstance().userData.totalClassicGames < 2)
        {
            windowMain.Find("ButtonBattleMode").GetComponent<Animation>().enabled = false;
            windowMain.Find("ButtonClassicMode").GetComponent<Animation>().enabled = true;

            windowMain.Find("ButtonClassicMode").GetComponent<Button>().onClick.RemoveAllListeners();
            if(Infomanager.GetInstance().userData.totalClassicGames == 0)
            {
                windowMain.Find("ButtonClassicMode").GetComponent<Button>().onClick.AddListener(() => {
                    LoadClassicMode(1);
                });
            }
            else
            {
                windowMain.Find("ButtonClassicMode").GetComponent<Button>().onClick.AddListener(() => {
                    LoadClassicMode(0);
                });
            }
        }
        else if (Infomanager.GetInstance().userData.totalBattleGames < 1)
        {
            windowMain.Find("ButtonBattleMode").GetComponent<Animation>().enabled = true;
            windowMain.Find("ButtonClassicMode").GetComponent<Animation>().enabled = false;

            //use default callback
        }
        else
        {
            //use default callback
        }

    }

    /// <summary>
    /// 刷新设置窗口
    /// </summary>
    public void RefreshPanelAvatars()
    {

        //Instantiate
        RectTransform transScrollContent = tabAvatars.GetComponentInChildren<ScrollRect>().content;

        if (transScrollContent.childCount == 1)
        {
            int xCount = Mathf.CeilToInt(cellCountAvatars / 2f);
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < xCount; j++)
                {
                    if (i == 0 && j == 0) continue;

                    GameObject cell = Instantiate(transScrollContent.GetChild(0).gameObject, transScrollContent.GetChild(0).transform.position, transScrollContent.GetChild(0).transform.rotation, transScrollContent);
                    cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(j * cellDistanceSetting, -i * cellDistanceSetting);
                }
            }

            transScrollContent.sizeDelta = new Vector2(cellDistanceSetting * xCount, transScrollContent.sizeDelta.y);
        }

        //Set
        for(int i = 0; i < transScrollContent.childCount; i++)
        {
            int iTmp = i;

            transScrollContent.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Avatars/" + i.ToString());

            if(i < Infomanager.countAvatars)//被使用格子
            {
                if (Infomanager.GetInstance().userData.avatarId == i)//当前头像
                {
                    transScrollContent.GetChild(i).GetComponent<Image>().color = Color.white;
                    transScrollContent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = true;
                    transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                }
                else//其他头像
                {
                    bool uolocked = (Infomanager.GetInstance().userData.vipLvl / 15f) >= ((float)i / (float)Infomanager.countAvatars);


                    if (uolocked)//已解锁头像
                    {
                        transScrollContent.GetChild(i).GetComponent<Image>().color = Color.white;
                        transScrollContent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                            Infomanager.GetInstance().userData.avatarId = iTmp;
                            RefreshPanelAvatars();
                            RefreshWindowMain();
                        });
                    }
                    else//未解锁头像
                    {
                        transScrollContent.GetChild(i).GetComponent<Image>().color = Color.gray;
                        transScrollContent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    }
                }
            }
            else//未使用
            {
                transScrollContent.GetChild(i).GetComponent<Image>().enabled = false;
                transScrollContent.GetChild(i).GetChild(0).GetComponent<Image>().enabled = false;
                transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
            
        }
    }
    public void RefreshPanelBorders()
    {
        //Instantiate
        RectTransform transScrollContent = tabBorders.GetComponentInChildren<ScrollRect>().content;

        if (transScrollContent.childCount == 1)
        {
            int xCount = Mathf.CeilToInt(cellCountBorders / 2f);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < xCount; j++)
                {
                    if (i == 0 && j == 0) continue;

                    GameObject cell = Instantiate(transScrollContent.GetChild(0).gameObject, transScrollContent.GetChild(0).transform.position, transScrollContent.GetChild(0).transform.rotation, transScrollContent);
                    cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(j * cellDistanceSetting, -i * cellDistanceSetting);
                }
            }
            transScrollContent.sizeDelta = new Vector2(cellDistanceSetting * xCount, transScrollContent.sizeDelta.y);
        }

        //Set
        for (int i = 0; i < transScrollContent.childCount; i++)
        {
            int iTmp = i;


            if(i < Infomanager.countBorders)
            {
                //ACTIVE
                transScrollContent.GetChild(i).gameObject.SetActive(true);
                transScrollContent.GetChild(i).Find("ImageBorder").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Borders/" + i.ToString());

                //DISPLAY AND BTN
                if (Infomanager.GetInstance().userData.borderId == i)
                {
                    transScrollContent.GetChild(i).Find("ImageBorder").GetComponent<Image>().color = Color.white;
                    transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().color = Color.white;

                    transScrollContent.GetChild(i).Find("ImageSelected").GetComponent<Image>().enabled = true;
                    transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().enabled = false;
                    transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                }
                else
                {
                    bool uolocked = (Infomanager.GetInstance().userData.vipLvl / 15f) >= ((float)i / (float)Infomanager.countBorders);


                    if (uolocked)//已解锁头像框
                    {
                        transScrollContent.GetChild(i).Find("ImageBorder").GetComponent<Image>().color = Color.white;
                        transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().color = Color.white;

                        transScrollContent.GetChild(i).Find("ImageSelected").GetComponent<Image>().enabled = false;
                        transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().enabled = true;
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {
                            Infomanager.GetInstance().userData.borderId = iTmp;
                            RefreshPanelBorders();
                            RefreshWindowMain();
                        });
                    }
                    else//未解锁头像框
                    {
                        transScrollContent.GetChild(i).Find("ImageBorder").GetComponent<Image>().color = Color.gray;
                        transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().color = Color.gray;

                        transScrollContent.GetChild(i).Find("ImageSelected").GetComponent<Image>().enabled = false;
                        transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().enabled = true;
                        transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                    }
                }
            }
            else
            {
                //ACTIVE
                transScrollContent.GetChild(i).gameObject.SetActive(false);

                //DISPLAY AND BTN
                transScrollContent.GetChild(i).Find("ImageSelected").GetComponent<Image>().enabled = false;
                transScrollContent.GetChild(i).Find("ImageUnSelected").GetComponent<Image>().enabled = true;
                transScrollContent.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }
    public void RefreshPanelSounds()
    {
        tabSounds.GetComponentInChildren<Slider>().value = SoundPlayer.Volume;
    }

    /// <summary>
    /// 刷新经典模式角色阵营选择界面
    /// </summary>
    public void RefreshWindowClassicChoose()
    {
        // ---btn init---
        windowClassic.Find("ButtonHider").GetComponent<Image>().color = Color.gray;
        windowClassic.Find("ButtonHider").GetComponent<Button>().onClick.RemoveAllListeners();
        windowClassic.Find("ButtonHunter").GetComponent<Image>().color = Color.gray;
        windowClassic.Find("ButtonHunter").GetComponent<Button>().onClick.RemoveAllListeners();



        // ---btn set---
        if (Infomanager.GetInstance().userData.cardA > 0)
        {
            //入场券显示
            windowClassic.Find("TextCardA").GetComponent<Text>().text = "<b>入场券：" + Infomanager.GetInstance().userData.cardA.ToString() + "</b>";


            //领取按钮
            windowClassic.Find("ButtonGetA").gameObject.SetActive(false);


            //进入按钮
            windowClassic.Find("ButtonHider").GetComponent<Image>().color = Color.white;
            windowClassic.Find("ButtonHider").GetComponent<Button>().onClick.AddListener(() => {
                Infomanager.GetInstance().userData.cardA = Mathf.Clamp(Infomanager.GetInstance().userData.cardA - 1, 0, 999);
                
                LoadClassicMode(0);
            });
        }
        else
        {
            //入场券显示
            windowClassic.Find("TextCardA").GetComponent<Text>().text = "<b>入场券不足</b>";

            //领取按钮
            windowClassic.Find("ButtonGetA").gameObject.SetActive(true);
            windowClassic.Find("ButtonGetA").GetComponent<Button>().onClick.RemoveAllListeners();
            windowClassic.Find("ButtonGetA").GetComponent<Button>().onClick.AddListener(() =>
            {
                //埋点
                MaiDian.Mai("number", 11, "version", Infomanager.Version, "startVideo");

                AdManager.VideoAd(() => {

                    //埋点
                    MaiDian.Mai("number", 11, "version", Infomanager.Version, "endVideo");

                    Infomanager.GetInstance().userData.cardA = Mathf.Clamp(Infomanager.GetInstance().userData.cardA + 2, 0, 999);
                    Infomanager.GetInstance().userData.cardA = Mathf.Clamp(Infomanager.GetInstance().userData.cardA - 1, 0, 999);
                    LoadClassicMode(0);
                }, () => { });
            });
        }
        

        if(Infomanager.GetInstance().userData.cardB > 0)
        {
            //入场券显示
            windowClassic.Find("TextCardB").GetComponent<Text>().text = "<b>入场券：" + Infomanager.GetInstance().userData.cardB.ToString() + "</b>";

            //领取按钮
            windowClassic.Find("ButtonGetB").gameObject.SetActive(false);


            //进入按钮
            windowClassic.Find("ButtonHunter").GetComponent<Image>().color = Color.white;
            windowClassic.Find("ButtonHunter").GetComponent<Button>().onClick.AddListener(() => {
                Infomanager.GetInstance().userData.cardB = Mathf.Clamp(Infomanager.GetInstance().userData.cardB - 1, 0, 999);
                
                LoadClassicMode(1);
            });
        }
        else
        {
            //入场券显示
            windowClassic.Find("TextCardB").GetComponent<Text>().text = "<b>入场券不足</b>";

            //领取按钮
            windowClassic.Find("ButtonGetB").gameObject.SetActive(true);
            windowClassic.Find("ButtonGetB").GetComponent<Button>().onClick.RemoveAllListeners();
            windowClassic.Find("ButtonGetB").GetComponent<Button>().onClick.AddListener(() =>
            {
                //埋点
                MaiDian.Mai("number", 12, "version", Infomanager.Version, "startVideo");

                AdManager.VideoAd(() => {
                    //埋点
                    MaiDian.Mai("number", 12, "version", Infomanager.Version, "endVideo");

                    Infomanager.GetInstance().userData.cardB = Mathf.Clamp(Infomanager.GetInstance().userData.cardB + 2, 0, 999);
                    Infomanager.GetInstance().userData.cardB = Mathf.Clamp(Infomanager.GetInstance().userData.cardB - 1, 0, 999);
                    LoadClassicMode(1);
                }, ()=> { });
            });
        }

    }

    /// <summary>
    /// 刷新天赋页面
    /// </summary>
    private UITalentNode tnodeSelect = null;
    public void RefreshWindowTalents()
    {
        var charactorInst = Infomanager.GetInstance().userData.charactors.FirstOrDefault(c => c.info == Infomanager.GetInstance().userData.activeSkin.charaInfo);

        //panel container
        Transform talentPanel = windowTalents.Find("TalentPanel");
        Transform transNodes = talentPanel.Find("Nodes");
        Transform transLines = talentPanel.Find("Lines");

        //textTitle
        windowTalents.Find("TextTitle").GetComponent<Text>().text = charactorInst.info.name + "天赋";

        //infoPanel
        windowTalents.Find("InfoPanel").GetComponent<RectTransform>().anchoredPosition = new Vector2(-800, 0);

        //btns reset
        windowTalents.Find("ButtonGemUnlock").GetComponentInChildren<Text>().text = "";
        windowTalents.Find("ButtonGemUnlock").GetComponent<Image>().color = Color.gray;
        windowTalents.Find("ButtonGemUnlock").GetComponent<Button>().onClick.RemoveAllListeners();
        windowTalents.Find("ButtonAdUnlock").GetComponent<Image>().color = Color.gray;
        windowTalents.Find("ButtonAdUnlock").GetComponent<Button>().onClick.RemoveAllListeners();

        //gridSize（适配网格宽度）
        Vector2 panelSize = talentPanel.GetComponent<RectTransform>().sizeDelta;
        float gridSize = panelSize.y / 6f;

        //destroy all nodes
        for (int i = transNodes.childCount - 1; i > -1; i--) 
        {
            DestroyImmediate(transNodes.GetChild(i).gameObject);
        }
        //instantiate...Nodes
        for (int i = 0; i < charactorInst.info.talentNodes.Count; i++)
        {
            GameObject talentNode = Instantiate(Resources.Load<GameObject>("Prefabs/TalentStuff/TalentNode"), transNodes);
            talentNode.transform.localPosition = new Vector3();
            talentNode.transform.localRotation = new Quaternion();
            talentNode.GetComponent<RectTransform>().anchoredPosition = new Vector2(charactorInst.info.talentNodes[i].pos.x * gridSize, charactorInst.info.talentNodes[i].pos.y * gridSize);

            talentNode.GetComponent<UITalentNode>().id = charactorInst.info.talentNodes[i].id;
            talentNode.GetComponent<UITalentNode>().available = charactorInst.info.talentNodes[i].preNodes.Any(pre => charactorInst.unlockedNodes.Contains(pre.id)) || charactorInst.info.talentNodes[i].preNodes.Count < 1 ? true : false;
            talentNode.GetComponent<UITalentNode>().unlocked = charactorInst.unlockedNodes.Contains(talentNode.GetComponent<UITalentNode>().id);
        }

        //destroy lines
        for (int i = 0; i < transLines.childCount; i++)
        {
            Destroy(transLines.GetChild(i).gameObject);
        }
        //instantiate nodes
        for (int i = 0; i < charactorInst.info.talentNodes.Count; i++)
        {
            TalentNodeInfo node = charactorInst.info.talentNodes[i];
            TalentNodeInfo[] preNodes = charactorInst.info.talentNodes[i].preNodes.ToArray();

            foreach(var prenode in preNodes)
            {
                //instantia line
                GameObject talentLine = Instantiate(Resources.Load<GameObject>("Prefabs/TalentStuff/TalentLine"), transLines);
                talentLine.transform.localPosition = new Vector3();
                talentLine.transform.localRotation = new Quaternion();
                talentLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(gridSize * (node.pos.x + prenode.pos.x) / 2f, gridSize * (node.pos.y + prenode.pos.y) / 2f);
                talentLine.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(transLines.right, (node.pos - prenode.pos), transLines.forward));
                talentLine.GetComponent<RectTransform>().sizeDelta = new Vector2(gridSize * (node.pos - prenode.pos).magnitude, 15f);
            }
        }
        
        RefreshTalentNodesAvailable();
    }
    public void RefreshTalentNodesAvailable()//move to UITalentNode script???
    {
        var charactorInst = Infomanager.GetInstance().userData.charactors.FirstOrDefault(c => c.info == Infomanager.GetInstance().userData.activeSkin.charaInfo);
        

        UITalentNode[] uiTalents = windowTalents.GetComponentsInChildren<UITalentNode>();
        
        foreach(var node in uiTalents)
        {
            TalentNodeInfo talentNode = Infomanager.GetInstance().userData.activeSkin.charaInfo.talentNodes.FirstOrDefault(t => t.id == node.id);
            
            bool isAvailable = talentNode.preNodes.Any(pre => charactorInst.unlockedNodes.Contains(pre.id)) || talentNode.preNodes.Count < 1 ? true : false;
            if (isAvailable) node.SetAvailable();
        }
    }
    public void RefreshTalentSelect(UITalentNode uiTalentNode)
    {
        if (tnodeSelect != null && tnodeSelect.IsUnLocking) return;//避免解锁时选择其他节点

        tnodeSelect = uiTalentNode;//重新设置"当前节点"字段


        var charactorInst = Infomanager.GetInstance().userData.charactors.FirstOrDefault(c => c.info == Infomanager.GetInstance().userData.activeSkin.charaInfo);

        //refresh ui node display
        UITalentNode[] uiTalents = windowTalents.GetComponentsInChildren<UITalentNode>();

        foreach (var node in uiTalents)
        {
            if (node == uiTalentNode)
            {
                node.selected = true;
            }
            else
            {
                node.selected = false;
            }

            node.ShowSelectedOtNot();
        }

        //refresh btns
        windowTalents.Find("ButtonGemUnlock").gameObject.SetActive(true);
        windowTalents.Find("ButtonGemUnlock").GetComponentInChildren<Text>().text = uiTalentNode.TalentInfo.price.ToString();
        windowTalents.Find("ButtonGemUnlock").GetComponent<Image>().color = Color.gray;
        windowTalents.Find("ButtonGemUnlock").GetComponent<Button>().onClick.RemoveAllListeners();
        windowTalents.Find("ButtonAdUnlock").gameObject.SetActive(true);
        windowTalents.Find("ButtonAdUnlock").GetComponent<Image>().color = Color.gray;
        windowTalents.Find("ButtonAdUnlock").GetComponent<Button>().onClick.RemoveAllListeners();

        if (!uiTalentNode.unlocked)
        {
            if (uiTalentNode.available)
            {
                if (MoneyEnough(uiTalentNode.TalentInfo.price))
                {
                    windowTalents.Find("ButtonGemUnlock").GetComponent<Image>().color = Color.white;
                    windowTalents.Find("ButtonGemUnlock").GetComponent<Button>().onClick.AddListener(() => {
                        uiTalentNode.AutoFill(false);
                    });
                }

                windowTalents.Find("ButtonAdUnlock").GetComponent<Image>().color = Color.white;
                windowTalents.Find("ButtonAdUnlock").GetComponent<Button>().onClick.AddListener(() => {

                    //埋点
                    MaiDian.Mai("number", 4, "version", Infomanager.Version, "startVideo");

                    AdManager.VideoAd(() => {

                        //埋点
                        MaiDian.Mai("number", 4, "version", Infomanager.Version, "endVideo");

                        uiTalentNode.AutoFill(true);
                    }, () => { });
                });
            }
        }
        

        //refresh info panel
        var talent = charactorInst.info.talentNodes.FirstOrDefault(t => t.id == uiTalentNode.id);

        float pivotX = talent.pos.x > 0 ? 1.2f : -0.2f;
        float pivotY = talent.pos.y > 0 ? 1.2f : -0.2f;
        windowTalents.Find("InfoPanel").GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);

        windowTalents.Find("InfoPanel").GetComponent<RectTransform>().position = uiTalentNode.transform.position;
        windowTalents.Find("InfoPanel").Find("ImageTitle").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Talents/" + uiTalentNode.id + "title");
        windowTalents.Find("InfoPanel").Find("ImageTitle").GetComponent<Image>().SetNativeSize();
        windowTalents.Find("InfoPanel").GetComponentInChildren<Text>().text = talent.description;

    }
    public void TryToggleTalentToMain()
    {
        // 避免解锁过程中强行返回主界面
        if (tnodeSelect != null && tnodeSelect.IsUnLocking) return;

        //btns hide
        windowTalents.Find("ButtonGemUnlock").gameObject.SetActive(false);
        windowTalents.Find("ButtonAdUnlock").gameObject.SetActive(false);
        //now back to main
        ToggleWindowMain();
    } 

    /// <summary>
    /// 刷新角色面板/详情
    /// </summary>
    private int skinSelect = -1;
    public void RefreshPanelCharactors()
    {
        RefreshCharaSkinInventory();

        //btn left right
        tabCharactors.Find("ButtonLeft").GetComponent<Button>().onClick.RemoveAllListeners();
        tabCharactors.Find("ButtonLeft").GetComponent<Button>().onClick.AddListener(() => {
            tabCharactors.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(450, 0);
        });
        tabCharactors.Find("ButtonRight").GetComponent<Button>().onClick.RemoveAllListeners();
        tabCharactors.Find("ButtonRight").GetComponent<Button>().onClick.AddListener(() => {
            tabCharactors.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(-450, 0);
        });
    }
    public void RefreshCharaSkinInventory()
    {
        Transform transInventory = tabCharactors.Find("ScrollViewInventory").GetComponent<ScrollRect>().content;

        //生成格子
        if (transInventory.childCount == 1)
        {
            int xCount = Mathf.CeilToInt(cellCountChara / 2f);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    GameObject cell = Instantiate(transInventory.GetChild(0).gameObject, transInventory.GetChild(0).position, transInventory.GetChild(0).rotation, transInventory);
                    cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(x * cellDistanceShop, -y * cellDistanceShop);
                }
            }

            (transInventory as RectTransform).sizeDelta = new Vector2(cellDistanceShop * xCount, (transInventory as RectTransform).sizeDelta.y);
        }

        //设置角色标签
        for (int i = 0; i < transInventory.childCount; i++)
        {
            if (i < Infomanager.GetInstance().skinInfos.Length)
            {
                int idTmp = i;

                transInventory.GetChild(i).gameObject.SetActive(true);

                transInventory.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/bg" + Infomanager.GetInstance().skinInfos[i].rank);
                transInventory.GetChild(i).Find("ImageTextBgSelect").gameObject.SetActive(false);
                transInventory.GetChild(i).Find("ImageHead").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterHeads/" + Infomanager.GetInstance().skinInfos[i].name);
                transInventory.GetChild(i).Find("ImageRare").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/" + Infomanager.GetInstance().skinInfos[i].rank);
                transInventory.GetChild(i).GetComponentInChildren<Text>().text = Infomanager.GetInstance().skinInfos[i].name;
                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                transInventory.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                    for(int j = 0; j < transInventory.childCount; j++)
                    {
                        transInventory.GetChild(j).Find("ImageTextBgSelect").gameObject.SetActive(false);
                    }

                    transInventory.GetChild(idTmp).Find("ImageTextBgSelect").gameObject.SetActive(true);

                    RefreshSkinInfo(idTmp);

                    SoundPlayer.PlaySound2D("click");
                });

            }
            else
            {
                transInventory.GetChild(i).gameObject.SetActive(false);

                transInventory.GetChild(i).GetComponentInChildren<Text>().text = "";
                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        //默认角色标签页
        int activeid = Infomanager.GetInstance().userData.activeSkin.charaInfo.id;
        transInventory.GetChild(activeid).GetComponent<Button>().onClick.Invoke();// = true; ;//默认刷新为第一个角色   //改为切换当前装备角色？？
    }
    public void RefreshSkinInfo(int skinid)
    {
        skinSelect = skinid;

        
        SkinInfo skinInfo = Infomanager.GetInstance().skinInfos.FirstOrDefault(s => s.id == skinid);
        CharactorInfo charactorInfo = skinInfo.charaInfo;

        //Base Reset
        if (skinInfo != null)
        {
            //---Info Display---
            tabCharactors.Find("ImageCharacterName").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterNames/" + skinInfo.name);
            tabCharactors.Find("ImageCharacterRare").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/" + skinInfo.rank);

            for (int i = 0; i < charactorInfo.abilities.Count; i++)
            {
                tabCharactors.Find("PanelAbilities").GetChild(i).Find("TextName").GetComponent<Text>().text = charactorInfo.abilities[i].name + ": " + charactorInfo.abilities[i].description;
            }

            //---Reset Equiped or Price Info
            tabCharactors.Find("ImageEquiped").gameObject.SetActive(false);
            tabCharactors.Find("ImagePriceMoney").gameObject.SetActive(false);
            tabCharactors.Find("ImagePriceDiamond").gameObject.SetActive(false);


            //---Try Use Charactor---
            charactorPreview.TryDeco(skinInfo);
        }

        //---different condition---
        if (Infomanager.GetInstance().userData.mySkins.Contains(skinInfo))
        {
            if(Infomanager.GetInstance().userData.activeSkin == skinInfo)
            {
                //status
                tabCharactors.Find("ImageEquiped").gameObject.SetActive(true);
                //btn
                tabCharactors.Find("ButtonConfirm").gameObject.SetActive(false);
            }
            else
            {
                //btn
                tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/装备");// "装备角色";
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Infomanager.GetInstance().userData.activeSkin = skinInfo;
                    RefreshSkinInfo(skinInfo.id);//Refresh

                    SoundPlayer.PlaySound2D("click");

                });
            }
            
        }
        else
        {
            if(skinInfo.get == 0)
            {
                if(skinInfo.rank == "SSR")
                {
                    if (DiamondEnough(skinInfo.price / 5)) //砖石价格为金币0.2f
                    {
                        //status
                        tabCharactors.Find("ImagePriceDiamond").gameObject.SetActive(true);
                        tabCharactors.Find("ImagePriceDiamond").GetComponentInChildren<Text>().text = "<b>" + (skinInfo.price / 5).ToString() + "</b>";
                        //btn
                        tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                        tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");// "钻石购买角色";
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            GiveSkin(skinInfo);//购买角色

                            Infomanager.GetInstance().userData.activeSkin = skinInfo;

                            SpendDiamond(skinInfo.price / 5);//Refresh

                            SoundPlayer.PlaySound2D("unlock");

                        });
                    }
                    else
                    {
                        //status
                        tabCharactors.Find("ImagePriceDiamond").gameObject.SetActive(true);
                        tabCharactors.Find("ImagePriceDiamond").GetComponentInChildren<Text>().text = "<b>" + (skinInfo.price / 5).ToString() + "</b>";
                        //btn
                        tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                        tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");// "钻石不足会弹窗";
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                            ShowHideWindowGetMoney(true, true);
                        });
                    }
                }
                else
                {
                    if (MoneyEnough(skinInfo.price))
                    {
                        //status
                        tabCharactors.Find("ImagePriceMoney").gameObject.SetActive(true);
                        tabCharactors.Find("ImagePriceMoney").GetComponentInChildren<Text>().text = "<b>" + (skinInfo.price).ToString() + "</b>";
                        //btn
                        tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                        tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() =>
                        {
                            GiveSkin(skinInfo);//购买角色

                            Infomanager.GetInstance().userData.activeSkin = skinInfo;

                            SpendMoney(skinInfo.price);//Refresh

                            SoundPlayer.PlaySound2D("unlock");
                        });
                    }
                    else
                    {
                        //status
                        tabCharactors.Find("ImagePriceMoney").gameObject.SetActive(true);
                        tabCharactors.Find("ImagePriceMoney").GetComponentInChildren<Text>().text = "<b>" + (skinInfo.price).ToString() + "</b>";
                        //btn
                        tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                        tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");// "金币不足会弹窗";
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                        tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                            ShowHideWindowGetMoney(true, false);
                        });
                    }
                }
                
            }
            if (skinInfo.get == 1)
            {
                tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/视频领取");
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {

                    //埋点
                    MaiDian.Mai("number", 7, "version", Infomanager.Version, "startVideo");

                    AdManager.VideoAd(() => {
                        //埋点
                        MaiDian.Mai("number", 7, "version", Infomanager.Version, "endVideo");

                        GiveSkin(skinInfo);//视频获取角色
                        Infomanager.GetInstance().userData.activeSkin = skinInfo;

                        RefreshSkinInfo(skinInfo.id);//Refresh


                        SoundPlayer.PlaySound2D("unlock");
                    }, () => { });
                });
            }
            if (skinInfo.get == 2)
            {
                tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/签到获取");
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
            }
            if (skinInfo.get == 3)
            {
                tabCharactors.Find("ButtonConfirm").gameObject.SetActive(true);
                tabCharactors.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/限时活动");
                tabCharactors.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }
    

    /// <summary>
    /// 刷新装饰装备界面
    /// </summary>
    private DecorationInfo decoSelect = null;
    public void RefreshPanelDecorations()
    {
        //preview....//others...
        RefreshDecoInventory();


        //btn left right
        tabDecorations.Find("ButtonLeft").GetComponent<Button>().onClick.RemoveAllListeners();
        tabDecorations.Find("ButtonLeft").GetComponent<Button>().onClick.AddListener(() => {
            tabDecorations.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(450, 0);
        });
        tabDecorations.Find("ButtonRight").GetComponent<Button>().onClick.RemoveAllListeners();
        tabDecorations.Find("ButtonRight").GetComponent<Button>().onClick.AddListener(() => {
            tabDecorations.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(-450, 0);
        });
    }
    public void RefreshDecoInventory(string decoType = "head")
    {
        //PanelInventory
        Transform transInventory = tabDecorations.Find("ScrollViewInventory").GetComponent<ScrollRect>().content;
        
        DecorationInfo[] decos = Infomanager.GetInstance().decorationInfos.Where(deco => deco.group == decoType).ToArray();

        //生成格子
        if (transInventory.childCount == 1)
        {
            int xCount = Mathf.CeilToInt(cellCountDecos / 2f);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    GameObject cell = Instantiate(transInventory.GetChild(0).gameObject, transInventory.GetChild(0).position, transInventory.GetChild(0).rotation, transInventory);
                    cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(x * cellDistanceShop, -y * cellDistanceShop);
                }
            }
            (transInventory as RectTransform).sizeDelta = new Vector2(cellDistanceShop * xCount, (transInventory as RectTransform).sizeDelta.y);
        }


        for (int i = 0; i < transInventory.childCount; i++)
        {
            int iTmp = i;
            //格子有装备
            if(i < decos.Length)
            {
                //名称 / 图标
                transInventory.GetChild(i).gameObject.SetActive(true);

                transInventory.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/bg" + decos[i].rank);
                transInventory.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + decos[i].name);
                transInventory.GetChild(i).Find("Image").GetComponent<Image>().SetNativeSize();
                transInventory.GetChild(i).Find("Image").GetComponent<RectTransform>().sizeDelta *= new Vector2(0.5f, 0.5f);
                transInventory.GetChild(i).Find("ImageTextBgSelect").gameObject.SetActive(false);
                transInventory.GetChild(i).GetComponentInChildren<Text>().text = decos[i].chinese;



                if (Infomanager.GetInstance().userData.myDecorations.Contains(decos[i]))//已拥有
                {
                    transInventory.GetChild(i).GetComponentInChildren<Text>().color = Color.green;
                    //预览。。。
                }
                else//未拥有
                {
                    transInventory.GetChild(i).GetComponentInChildren<Text>().color = Color.red;
                    //预览。。。
                }

                DecorationInfo decoTmp = decos[i];

                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                transInventory.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                    for (int j = 0; j < transInventory.childCount; j++)
                    {
                        transInventory.GetChild(j).Find("ImageTextBgSelect").gameObject.SetActive(false);
                    }

                    transInventory.GetChild(iTmp).Find("ImageTextBgSelect").gameObject.SetActive(true);

                    //Try Deco
                    charactorPreview.TryDeco(decoTmp);

                    //refresh info
                    RefreshDecoInfo(decoTmp);

                    //sound
                    SoundPlayer.PlaySound2D("ber");
                });
            }
            else
            {
                transInventory.GetChild(i).gameObject.SetActive(false);

                transInventory.GetChild(i).GetComponentInChildren<Text>().text = "";
                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        //刷新为用户当前装备详情  避免刷新NULL ??????
        switch (decoType)
        {
            case "head":
                {
                    DecorationInfo decoInfo = Infomanager.GetInstance().userData.activeDecoration0 != null ? Infomanager.GetInstance().userData.activeDecoration0 : Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.group == decoType);
                    int activeid = Infomanager.GetInstance().decorationInfos.Where(d => d.group == "head").ToList().IndexOf(decoInfo);
                    transInventory.GetChild(activeid).GetComponent<Button>().onClick.Invoke();
                }
                break;
            case "back":
                {
                    DecorationInfo decoInfo = Infomanager.GetInstance().userData.activeDecoration1 != null ? Infomanager.GetInstance().userData.activeDecoration1 : Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.group == decoType);
                    int activeid = Infomanager.GetInstance().decorationInfos.Where(d => d.group == "back").ToList().IndexOf(decoInfo);
                    transInventory.GetChild(activeid).GetComponent<Button>().onClick.Invoke();
                    RefreshDecoInfo(decoInfo);
                }
                break;
            case "weapon":
                {
                    DecorationInfo decoInfo = Infomanager.GetInstance().userData.activeDecoration2 != null ? Infomanager.GetInstance().userData.activeDecoration2 : Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.group == decoType);
                    int activeid = Infomanager.GetInstance().decorationInfos.Where(d => d.group == "weapon").ToList().IndexOf(decoInfo);
                    transInventory.GetChild(activeid).GetComponent<Button>().onClick.Invoke();
                    //RefreshDecoInfo(decoInfo);
                }
                break;
        }
    }
    public void RefreshDecoInfo(DecorationInfo deco)
    {
        decoSelect = deco;

        //Info Introduce
        //...


        //Ind
        int index = Infomanager.GetInstance().decorationInfos.Where(d => d.group == deco.group).ToList().IndexOf(deco);
        Transform transInventory = tabDecorations.Find("ScrollViewInventory").GetComponent<ScrollRect>().content;

        //BTN
        if (Infomanager.GetInstance().userData.myDecorations.Contains(deco))
        {
            //text price (disable)
            tabDecorations.Find("ImagePriceMoney").gameObject.SetActive(false);
            tabDecorations.Find("ImagePriceDiamond").gameObject.SetActive(false);


            //btn
            if (Infomanager.GetInstance().userData.activeDecoration0 != deco && Infomanager.GetInstance().userData.activeDecoration1 != deco && Infomanager.GetInstance().userData.activeDecoration2 != deco)//没有装备
            {
                //text equiped
                tabDecorations.Find("ImageEquiped").gameObject.SetActive(false);

                //btn
                tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/装备");
                tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                    SetDecoration(deco);//装备

                    RefreshDecoInventory(deco.group);
                    
                    //sound
                    SoundPlayer.PlaySound2D("click");
                });
            }
            else
            {
                //text equiped
                tabDecorations.Find("ImageEquiped").gameObject.SetActive(true);

                //btn
                tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/卸下");
                tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                    DetachDecoration(deco);//卸下

                    RefreshDecoInventory(deco.group);

                    //sound
                    SoundPlayer.PlaySound2D("click");
                });
            }
        }
        else
        {
            if(deco.rank != "SSR")
            {
                //text price
                tabDecorations.Find("ImageEquiped").gameObject.SetActive(false);
                tabDecorations.Find("ImagePriceMoney").gameObject.SetActive(true);
                tabDecorations.Find("ImagePriceDiamond").gameObject.SetActive(false);
                tabDecorations.Find("ImagePriceMoney").GetComponentInChildren<Text>().text = "<b>" + deco.price.ToString() + "</b>";


                //btn
                if (MoneyEnough(deco.price))
                {
                    tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        GiveDecoration(deco, true);
                        SetDecoration(deco);

                        RefreshDecoInventory(deco.group);

                        //sound
                        SoundPlayer.PlaySound2D("unlock");
                    });
                }
                else
                {
                    tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        ShowHideWindowGetMoney(true, false);
                    });
                }
            }
            else
            {
                //text price
                tabDecorations.Find("ImageEquiped").gameObject.SetActive(false);
                tabDecorations.Find("ImagePriceMoney").gameObject.SetActive(false);
                tabDecorations.Find("ImagePriceDiamond").gameObject.SetActive(true);
                tabDecorations.Find("ImagePriceDiamond").GetComponentInChildren<Text>().text = "<b>" +(deco.price / 5).ToString() + "</b>";


                //btn
                if (DiamondEnough(deco.price / 5))
                {
                    tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        GiveDecoration(deco, false);
                        SpendDiamond(deco.price / 5);

                        SetDecoration(deco);

                        RefreshDecoInventory(deco.group);

                        //sound
                        SoundPlayer.PlaySound2D("unlock");
                    });
                }
                else
                {
                    tabDecorations.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDecorations.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        ShowHideWindowGetMoney(true, true);
                    });
                }
            }

        }
    }

    /// <summary>
    /// 刷新伪装物品界面
    /// </summary>
    private DisguiseObjectInfo dsgSelect = null;
    public void RefreshPanelDisguises()
    {
        RefreshDsgInventory();

        //btn left right
        tabDisguises.Find("ButtonLeft").GetComponent<Button>().onClick.RemoveAllListeners();
        tabDisguises.Find("ButtonLeft").GetComponent<Button>().onClick.AddListener(() => {
            tabDisguises.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(450, 0);
        });
        tabDisguises.Find("ButtonRight").GetComponent<Button>().onClick.RemoveAllListeners();
        tabDisguises.Find("ButtonRight").GetComponent<Button>().onClick.AddListener(() => {
            tabDisguises.Find("ScrollViewInventory").GetComponent<ScrollRect>().content.anchoredPosition += new Vector2(-450, 0);
        });
    }
    public void RefreshDsgInventory()
    {
        Transform transInventory = tabDisguises.Find("ScrollViewInventory").GetComponent<ScrollRect>().content;

        //生成格子
        if (transInventory.childCount == 1)
        {
            int xCount = Mathf.CeilToInt(cellCountDsgs / 2f);
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    if (x == 0 && y == 0) continue;

                    GameObject cell = Instantiate(transInventory.GetChild(0).gameObject, transInventory.GetChild(0).position, transInventory.GetChild(0).rotation, transInventory);
                    cell.GetComponent<RectTransform>().anchoredPosition += new Vector2(x * cellDistanceShop, -y * cellDistanceShop);
                }
            }
            (transInventory as RectTransform).sizeDelta = new Vector2(cellDistanceShop * xCount, (transInventory as RectTransform).sizeDelta.y);
        }


        //SET
        for (int i = 0; i < transInventory.childCount; i++) 
        {
            int iTmp = i;

            if(i < Infomanager.GetInstance().disguiseObjInfos.Length)//有效格子
            {
                DisguiseObjectInfo dsgTmp = Infomanager.GetInstance().disguiseObjInfos[i];

                //active
                transInventory.GetChild(i).gameObject.SetActive(true);

                //名称 / 图标
                transInventory.GetChild(i).GetComponentInChildren<Text>().text = dsgTmp.chinese;
                transInventory.GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Rare/bg" + dsgTmp.rank);
                transInventory.GetChild(i).Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + dsgTmp.name);
                transInventory.GetChild(i).Find("ImageTextBgSelect").gameObject.SetActive(false);


                if (Infomanager.GetInstance().userData.myDisguiseObjects.Contains(dsgTmp))//已拥有
                {
                    transInventory.GetChild(i).GetComponentInChildren<Text>().color = Color.green;
                }
                else//未拥有
                {
                    transInventory.GetChild(i).GetComponentInChildren<Text>().color = Color.red;
                }

                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                transInventory.GetChild(i).GetComponent<Button>().onClick.AddListener(() => {

                    for(int j = 0; j < transInventory.childCount; j++)
                    {
                        transInventory.GetChild(j).Find("ImageTextBgSelect").gameObject.SetActive(false);
                    }

                    transInventory.GetChild(iTmp).Find("ImageTextBgSelect").gameObject.SetActive(true);


                    RefreshDsgInfo(dsgTmp);

                    //sound
                    SoundPlayer.PlaySound2D("ber");
                });
            }
            else
            {
                //active
                transInventory.GetChild(i).gameObject.SetActive(false);


                transInventory.GetChild(i).GetComponentInChildren<Text>().text = "";
                transInventory.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }

        //刷新详情
        DisguiseObjectInfo dsgInfo = Infomanager.GetInstance().userData.activeDisguiseObj != null ? Infomanager.GetInstance().userData.activeDisguiseObj : Infomanager.GetInstance().disguiseObjInfos[0];

        int activeid = Infomanager.GetInstance().disguiseObjInfos.ToList().IndexOf(dsgInfo);
        transInventory.GetChild(activeid).GetComponent<Button>().onClick.Invoke();
        //RefreshDsgInfo(dsgInfo);
    }
    public void RefreshDsgInfo(DisguiseObjectInfo dsg)
    {
        dsgSelect = dsg;

        //Introduce
        //...

        //preview
        ResetDsgPreview(dsg);

        //Ind
        int index = Infomanager.GetInstance().disguiseObjInfos.ToList().IndexOf(dsg);
        Transform transInventory = tabDisguises.Find("ScrollViewInventory").GetComponent<ScrollRect>().content;



        //text price (disable)
        tabDisguises.Find("ImagePriceMoney").gameObject.SetActive(false);
        tabDisguises.Find("ImagePriceDiamond").gameObject.SetActive(false);

        //--Btn And Image---
        if (Infomanager.GetInstance().userData.myDisguiseObjects.Contains(dsg))//已拥有
        {
            if (Infomanager.GetInstance().userData.activeDisguiseObj == dsg)
            {
                //text equiped
                tabDisguises.Find("ImageEquiped").gameObject.SetActive(true);

                //btn 
                tabDisguises.Find("ButtonConfirm").gameObject.SetActive(false);
            }
            else
            {
                //text equiped
                tabDisguises.Find("ImageEquiped").gameObject.SetActive(false);

                //btn
                tabDisguises.Find("ButtonConfirm").gameObject.SetActive(true);
                tabDisguises.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/装备");
                tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() =>
                {
                    SetDisguise(dsg);

                    RefreshDsgInventory();//必要操作。（刷新金币不会刷新物品栏显示。）

                    //sound
                    SoundPlayer.PlaySound2D("click");
                });
            }

        }
        else//未拥有
        {
            if(dsg.rank != "SSR")
            {
                //text price
                tabDisguises.Find("ImageEquiped").gameObject.SetActive(false);
                tabDisguises.Find("ImagePriceMoney").gameObject.SetActive(true);
                tabDisguises.Find("ImagePriceDiamond").gameObject.SetActive(false);

                tabDisguises.Find("ImagePriceMoney").GetComponentInChildren<Text>().text = "<b>" + dsg.price.ToString() + "</b>";

                //btn
                if (MoneyEnough(dsg.price))
                {
                    //btn
                    tabDisguises.Find("ButtonConfirm").gameObject.SetActive(true);
                    tabDisguises.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        GiveDisguise(dsg, true);
                        SetDisguise(dsg);

                        RefreshDsgInventory();//必要操作。（刷新金币不会刷新物品栏显示。）

                        //sound
                        SoundPlayer.PlaySound2D("unlock");
                    });
                }
                else
                {
                    //btn
                    tabDisguises.Find("ButtonConfirm").gameObject.SetActive(true);
                    tabDisguises.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        ShowHideWindowGetMoney(true, false);
                    });
                }
            }
            else
            {
                //text price
                tabDisguises.Find("ImageEquiped").gameObject.SetActive(false);
                tabDisguises.Find("ImagePriceMoney").gameObject.SetActive(false);
                tabDisguises.Find("ImagePriceDiamond").gameObject.SetActive(true);

                tabDisguises.Find("ImagePriceDiamond").GetComponentInChildren<Text>().text = "<b>" + (dsg.price / 5).ToString() + "</b>";

                //btn
                if (DiamondEnough(dsg.price / 5))
                {
                    //btn
                    tabDisguises.Find("ButtonConfirm").gameObject.SetActive(true);
                    tabDisguises.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        GiveDisguise(dsg, false);
                        SpendDiamond(dsg.price / 5);

                        SetDisguise(dsg);

                        RefreshDsgInventory();//必要操作。（刷新金币不会刷新物品栏显示。）

                        //sound
                        SoundPlayer.PlaySound2D("unlock");
                    });
                }
                else
                {
                    //btn
                    tabDisguises.Find("ButtonConfirm").gameObject.SetActive(true);
                    tabDisguises.Find("ButtonConfirm").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/购买");
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.RemoveAllListeners();
                    tabDisguises.Find("ButtonConfirm").GetComponent<Button>().onClick.AddListener(() => {
                        ShowHideWindowGetMoney(true, true);
                    });
                }
            }
        }
    }

    private void ResetDsgPreview(DisguiseObjectInfo disguise)
    {
        Transform tDisguise = tabDisguises.Find("PanelPreview");
        if (tDisguise.childCount > 0) DestroyImmediate(tDisguise.GetChild(0).gameObject);

        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Disguises/" + disguise.name), tDisguise);
        obj.transform.localScale = new Vector3(100, 100, 100);
        obj.transform.localPosition = new Vector3();
        obj.transform.localEulerAngles = new Vector3(0, 45, 0);
    }

    /// <summary>
    /// 刷新限时活动窗口
    /// </summary>
    public void RefreshActivityWindow()
    {
        windowActivity.Find("ButtonGet").GetComponent<Button>().onClick.RemoveAllListeners();

        DecorationInfo deco = Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.name == "wing_angle");
        if (Infomanager.GetInstance().userData.myDecorations.Any(d => d.name == "wing_angle"))
        {
            MenuNotice.Notice("已参加过该活动！", Color.white);
        }
        else
        {
            windowActivity.Find("ButtonGet").GetComponent<Button>().onClick.AddListener(() => {

                //埋点
                MaiDian.Mai("number", 3, "version", Infomanager.Version, "startVideo");

                AdManager.VideoAd(() => {

                    //埋点
                    MaiDian.Mai("number", 3, "version", Infomanager.Version, "endVideo");

                    GiveDecoration(deco, false);
                    SetDecoration(deco);

                    //sound
                    SoundPlayer.PlaySound2D("unlock");

                    ToggleWindowMain();
                }, () => { });
            });
        }
   

            

    }

    /// <summary>
    /// 刷新签到窗口
    /// </summary>
    public void RefreshCheckinWindow(bool autoRollTo = false)
    {
        ////---display---
        RectTransform content = windowCheckin.GetComponentInChildren<ScrollRect>().content;

        //today
        int todayId = Infomanager.GetTodayCheckinId();

        //autoRoll
        if (autoRollTo)
        {
            content.DOAnchorPos(new Vector2(-250 * todayId, content.anchoredPosition.y), 1f);
        }

        //Instantiate
        if(content.childCount == 1)
        {
            for(int i = 1; i < 7; i++)
            {
                GameObject o = Instantiate(content.GetChild(0).gameObject, content.GetChild(0).position, content.GetChild(0).rotation, content);

                o.GetComponent<RectTransform>().anchoredPosition += new Vector2(250 * i, 0);
            }
        }


        //Set
        for (int i = 0; i < 7; i++)
        {
            int iTmp = i;

            int b1Type = Infomanager.GetInstance().checkins1[i].type;
            int b1Id = Infomanager.GetInstance().checkins1[i].id;
            string b1Name = Infomanager.GetInstance().checkins1[i].name;
            int b1Amount = Infomanager.GetInstance().checkins1[i].amount;

            int b2Type = Infomanager.GetInstance().checkins2[i].type;
            int b2Id = Infomanager.GetInstance().checkins2[i].id;
            string b2Name = Infomanager.GetInstance().checkins2[i].name;
            int b2Amount = Infomanager.GetInstance().checkins2[i].amount;

            switch (b1Type)
            {
                case 0:
                    {
                        content.GetChild(i).Find("ImageGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Coins");
                        content.GetChild(i).Find("TextGet1").GetComponent<Text>().text ="<b>× " + b1Amount + "</b>";
                    }
                    break;
                case 1:
                    {
                        content.GetChild(i).Find("ImageGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterHeads/" + b1Name);
                        //...
                    }
                    break;
                case 2:
                case 3:
                    {
                        content.GetChild(i).Find("ImageGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + b1Name);
                        //...
                    }
                    break;
                case 4:
                    {
                        content.GetChild(i).Find("ImageGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Diamonds");
                        //...
                    }
                    break;
                default:
                    break;
            }

            switch (b2Type)
            {
                case 0:
                    {
                        content.GetChild(i).Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Coins");
                        //...
                    }
                    break;
                case 1:
                    {
                        content.GetChild(i).Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterHeads/" +  b2Name);
                        content.GetChild(i).Find("ImageTextGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/CharacterNames/" + b2Name);
                        content.GetChild(i).Find("ImageTextGet2").GetComponent<Image>().SetNativeSize();// = Resources.Load<Sprite>("Sprites/CharacterNames/" + b2Name);
                    }
                    break;
                case 2:
                case 3:
                    {
                        content.GetChild(i).Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/" + b2Name);
                        content.GetChild(i).Find("ImageTextGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/TextItems/" + b2Name);
                        content.GetChild(i).Find("ImageTextGet2").GetComponent<Image>().SetNativeSize();// = Resources.Load<Sprite>("Sprites/CharacterNames/" + b2Name);
                    }
                    break;
                case 4:
                    {
                        content.GetChild(i).Find("ImageGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/Diamonds");
                        //...
                    }
                    break;
                default:
                    break;
            }


            //Got Checked
            if (Infomanager.DayIdChecked(i) == 0)
            {
                content.GetChild(i).Find("ImageGet1").Find("ImageGot").gameObject.SetActive(false);
                content.GetChild(i).Find("ImageGet2").Find("ImageGot").gameObject.SetActive(false);
            }
            else if(Infomanager.DayIdChecked(i) == 1)
            {
                content.GetChild(i).Find("ImageGet1").Find("ImageGot").gameObject.SetActive(true);
                content.GetChild(i).Find("ImageGet2").Find("ImageGot").gameObject.SetActive(false);
            }
            else
            {
                content.GetChild(i).Find("ImageGet1").Find("ImageGot").gameObject.SetActive(true);
                content.GetChild(i).Find("ImageGet2").Find("ImageGot").gameObject.SetActive(true);
            }


            //-----Button Status----
            if (i == todayId) //今日
            {
                if(Infomanager.DayIdChecked(i) == 0)//今日未领取
                {
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/只领宝石");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                        GiveBonus(Infomanager.GetInstance().checkins1[iTmp], true);
                        Infomanager.CheckIn(false);

                        RefreshCheckinWindow();
                    });

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(true);
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/全部领取");
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {

                        //埋点
                        MaiDian.Mai("number", 0, "version", Infomanager.Version, "startVideo");

                        AdManager.VideoAd(() => {
                            //埋点
                            MaiDian.Mai("number", 0, "version", Infomanager.Version, "endVideo");

                            GiveBonus(Infomanager.GetInstance().checkins1[iTmp], false);
                            GiveBonus(Infomanager.GetInstance().checkins2[iTmp], true);
                            Infomanager.CheckIn(true);

                            RefreshCheckinWindow();
                        }, () => { });
                    });
                }
                if (Infomanager.DayIdChecked(i) == 1)//今日只领了宝石
                {
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/已领取");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(true);
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/全部领取");
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {

                        //埋点
                        MaiDian.Mai("number", 0, "version", Infomanager.Version, "startVideo");

                        AdManager.VideoAd(() => {
                            //埋点
                            MaiDian.Mai("number", 0, "version", Infomanager.Version, "endVideo");

                            GiveBonus(Infomanager.GetInstance().checkins2[iTmp], true);
                            Infomanager.CheckIn(true);

                            RefreshCheckinWindow();

                        }, () => { });
                    });
                }
                if (Infomanager.DayIdChecked(i) == 2)//今日已全部领取
                {
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/已领取");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(true);
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/已领取");
                    content.GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
                }
            }
            else if(i < todayId)//前几天
            {
                if(Infomanager.DayIdChecked(i) == 0)
                {
                    //之前未领取（可补领）
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/补签");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                        AdManager.VideoAd(() => {
                            GiveBonus(Infomanager.GetInstance().checkins1[iTmp], false);
                            GiveBonus(Infomanager.GetInstance().checkins2[iTmp], true);
                            Infomanager.CheckIn(iTmp, true);

                            RefreshCheckinWindow();
                        }, () => { });
                    });

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
                }
                if (Infomanager.DayIdChecked(i) == 1)
                {
                    //之前只领取了宝石（可补领）
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/补签");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                        AdManager.VideoAd(() => {
                            GiveBonus(Infomanager.GetInstance().checkins2[iTmp], true);
                            Infomanager.CheckIn(iTmp, true);

                            RefreshCheckinWindow();
                        }, () => { });
                    });

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
                }
                if (Infomanager.DayIdChecked(i) == 2)
                {
                    //之前已领取
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/已领取");
                    content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();

                    content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
                }
            }
            else
            {
                //等待领取
                content.GetChild(i).Find("ButtonGet1").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/等待领取");
                content.GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();

                content.GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// 刷新每日任务窗口
    /// </summary>
    public void RefreshWindowMissions()
    {
        RectTransform transMissions = windowMissions.Find("PanelMissions").GetComponent<RectTransform>();

        //init grids
        if(transMissions.childCount == 1)
        {
            for(int i = 1; i < Infomanager.GetInstance().missions.Count; i++)
            {
                GameObject newGrid = Instantiate(transMissions.GetChild(0).gameObject, transMissions.GetChild(0).position, transMissions.GetChild(0).rotation, transMissions);
                newGrid.GetComponent<RectTransform>().anchoredPosition += new Vector2(-i * 10, -i * 70);
            }
        }

        //set
        for(int i = 0; i < Infomanager.GetInstance().missions.Count; i++)
        {
            int iTmp = i;

            int mid = Infomanager.GetInstance().missions[i].id;

            //text description
            transMissions.GetChild(i).Find("TextDescription").GetComponent<Text>().text = Infomanager.GetInstance().missions[i].description;

            //text bonus
            Bonus bMoney = Infomanager.GetInstance().missions[i].bonuses.FirstOrDefault(b => b.type == 0);
            Bonus bDiamond = Infomanager.GetInstance().missions[i].bonuses.FirstOrDefault(b => b.type == 4);
            Bonus bVipExp = Infomanager.GetInstance().missions[i].bonuses.FirstOrDefault(b => b.type == 5);

            transMissions.GetChild(i).Find("ImageMoney").GetComponent<Image>().sprite = bDiamond != null ? (Resources.Load<Sprite>("Sprites/Items/Diamonds")) : (Resources.Load<Sprite>("Sprites/Items/Coins"));
            transMissions.GetChild(i).Find("TextMoney").GetComponent<Text>().text = bDiamond != null ? bDiamond.amount.ToString() : bMoney.amount.ToString();
            transMissions.GetChild(i).Find("TextVipExp").GetComponent<Text>().text = bVipExp.amount.ToString();


            //已领取
            if (Infomanager.GetInstance().userData.todayMissionFinished.Contains(mid))
            {

                //btn
                transMissions.GetChild(i).GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/已领取");
                transMissions.GetChild(i).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            }
            //未领取
            else
            {
                if (Infomanager.GetInstance().missions[i].Achieved())
                {
                    //btn
                    transMissions.GetChild(i).GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/领取");
                    transMissions.GetChild(i).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                    transMissions.GetChild(i).GetComponentInChildren<Button>().onClick.AddListener(() => {
                        foreach (Bonus b in Infomanager.GetInstance().missions[iTmp].bonuses)
                        {
                            GiveBonus(b, true);
                        }

                        Infomanager.GetInstance().userData.todayMissionFinished.Add(mid);

                        RefreshWindowMissions();
                    });
                }
                else
                {
                    //btn
                    transMissions.GetChild(i).GetComponentInChildren<Button>().GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Buttons/未达成");
                    transMissions.GetChild(i).GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                }
            }
        }
    }

    /// <summary>
    /// 刷新匹配窗口
    /// </summary>
    public void CreateRoom(List<MatchUser> knownUsers)
    {
        Infomanager.currentRoomUsers = new List<MatchUser>();

        //player
        int userRank = Badge.UserLevelGetRank(Infomanager.GetInstance().userData.level);
        MatchUser playerInfo = new MatchUser(Infomanager.GetInstance().userData.name, Infomanager.GetInstance().userData.avatarId, Infomanager.GetInstance().userData.borderId, userRank);
        Infomanager.currentRoomUsers.Add(playerInfo);

        //fake users
        for (int i = 0; i < 9; i++)
        {
            if (i < knownUsers.Count)
            {
                Infomanager.currentRoomUsers.Add(knownUsers[i]);
            }
            else
            {
                MatchUser rdmUser = new MatchUser(Infomanager.GetInstance().nickNames[Random.Range(0, Infomanager.GetInstance().nickNames.Count)], Random.Range(0, Infomanager.countAvatars), Random.Range(0, Infomanager.countBorders), Random.Range(0, 3));
                Infomanager.currentRoomUsers.Add(rdmUser);
            }
        }
    }
    public void RefreshWindowMatch()
    {
        if(Infomanager.currentRoomUsers == null)
        {
            CreateRoom(new List<MatchUser>());
        }

        //type
        if(Infomanager.gameType == 0)
        {
            windowMatch.Find("ImageType0").gameObject.SetActive(true);
            windowMatch.Find("ImageType1").gameObject.SetActive(false); 
        }
        else if (Infomanager.gameType == 1)
        {
            windowMatch.Find("ImageType0").gameObject.SetActive(false);
            windowMatch.Find("ImageType1").gameObject.SetActive(true);
        }


        
        StartCoroutine(CoMatch());
    }
    private IEnumerator CoMatch()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 10; i++)
        {
            windowMatch.Find("PanelUsers").GetChild(i).GetComponentInChildren<Text>().text = Infomanager.currentRoomUsers[i].name;
            windowMatch.Find("PanelUsers").GetChild(i).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Avatars/" + Infomanager.currentRoomUsers[i].avatarId);
            windowMatch.Find("PanelUsers").GetChild(i).Find("ImageBorder").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Borders/" + Infomanager.currentRoomUsers[i].borderId);
            windowMatch.Find("PanelUsers").GetChild(i).Find("ImageRank").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Ranks/rank" + Infomanager.currentRoomUsers[i].rank);

            if(Random.value < 0.5f)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.75f));
            }
        }

        yield return new WaitForSeconds(0.5f);

        ToggleWindowLoading();
    }
    public void RefreshWindowLoading()
    {
        StartCoroutine(CoLoading());
        
    }
    private IEnumerator CoLoading()
    {
        Transform progressBarInd = windowLoading.transform.Find("ProgressBar").GetChild(0);
        
        var op = SceneManager.LoadSceneAsync("Classic"); //通用场景
        op.allowSceneActivation = false;//max 0.9f

        // progress < 0.9f
        while (op.progress < 0.9f)
        {
            progressBarInd.GetComponent<RectTransform>().anchorMax = new Vector2(op.progress, 1f);
            
            yield return null;
        }

        // progress >= 0.9
        progressBarInd.GetComponent<RectTransform>().anchorMax = new Vector2(op.progress, 1f);
        yield return new WaitForSeconds(0.1f);

        // progress == 1f
        op.allowSceneActivation = true;
        yield return op;
    }


    //-----------------------------------------按键回调-------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------

    public void ButtonClickGetMoney()
    {
        //埋点
        MaiDian.Mai("number", 1, "version", Infomanager.Version, "startVideo");

        AdManager.VideoAd(() =>
        {
            //埋点
            MaiDian.Mai("number", 1, "version", Infomanager.Version, "endVideo");

            GiveMoney(500);

            //sound
            SoundPlayer.PlaySound2D("coins");

            //effects
            for (int i = 0; i < 10; i++)
            {
                ImageEffects.ImageBoomAndFly(this.GetComponent<RectTransform>(), this.userAssetsPanel.Find("ImageCoin").GetComponent<RectTransform>(), "Items/Coin", new Vector2(50, 50), 100f, 0.4f);
            }
        }, () => { });
    }
    public void ButtonClickGetDiamond()
    {
        //埋点
        MaiDian.Mai("number", 2, "version", Infomanager.Version, "startVideo");

        AdManager.VideoAd(() =>
        {
            //埋点
            MaiDian.Mai("number", 2, "version", Infomanager.Version, "endVideo");

            GiveDiamond(100);

            //sound
            SoundPlayer.PlaySound2D("coins");

            //effects
            for (int i = 0; i < 10; i++)
            {
                ImageEffects.ImageBoomAndFly(this.GetComponent<RectTransform>(), this.userAssetsPanel.Find("ImageDiamond").GetComponent<RectTransform>(), "Items/Diamond", new Vector2(50, 50), 100f, 0.4f);
            }
        }, () => { });
    }



    //-----------------------------------------载入场景-------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------

    public void LoadClassicMode(int team)// -1 == random
    {
        //埋点
        int gamesNumber = Infomanager.GetInstance().userData.totalClassicGames + Infomanager.GetInstance().userData.totalBattleGames + 1;
        if (gamesNumber <= 10)
        {
            MaiDian.Mai("number", gamesNumber, "version", Infomanager.Version, "startLevel");
        }

        //玩家阵营
        if (team < 0)
        {
            Infomanager.playerTeam = Random.Range(0, 2);
        }
        else
        {
            Infomanager.playerTeam = team;
        }


        SoundPlayer.PlaySound2D("click");

        Infomanager.gameType = 0;

        if (Infomanager.playerTeam == 0)
        {
            ToggleWindowMatch();
        }
        else
        {
            //试用弹窗
            Bonus b = RandomBonusNotHave();
            ShowHideWindowTry(true, () => { ToggleWindowMatch(); }, b);
        }
    }

    public void LoadBattleMode()
    {
        //埋点
        int gamesNumber = Infomanager.GetInstance().userData.totalClassicGames + Infomanager.GetInstance().userData.totalBattleGames + 1;
        if(gamesNumber <= 10)
        {
            MaiDian.Mai("number", gamesNumber, "version", Infomanager.Version, "startLevel");
        }

        //进入前设置
        Infomanager.playerTeam = 1;

        Infomanager.gameType = 1;

        SoundPlayer.PlaySound2D("click");


        //试用弹窗
        Bonus b = RandomBonusNotHave();
        ShowHideWindowTry(true, () => { ToggleWindowMatch(); }, b);
    }



    //---------------------------------------声音---------------------------------------------------------

    public void ChangeVolume(float value)
    {
        SoundPlayer.ChangeVolume(value);
    }



    
    


    
    //-------------------------------------- 购买/获取物品 --------------------------------------------------------

    public void GiveBonus(Bonus bonus, bool pumpWindow)//1战舰，2装备，3船员，4金币
    {
        switch (bonus.type)
        {
            case 0://金币
                GiveMoney(bonus.amount);
                break;
            case 1://角色
                GiveSkin( Infomanager.GetInstance().skinInfos.FirstOrDefault(s => s.name == bonus.name));
                break;
            case 2://装饰品
                GiveDecoration(Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.name == bonus.name), false);
                break;
            case 3://变身伪装
                GiveDisguise(Infomanager.GetInstance().disguiseObjInfos.FirstOrDefault(d => d.name == bonus.name), false);
                break;
            case 4://钻石
                GiveDiamond(bonus.amount);
                break;
            case 5://vip经验
                Infomanager.GetInstance().userData.vipExp += bonus.amount;
                break;
            default:
                break;
        }

        //奖励弹窗
        if (pumpWindow)
        {
            ShowHideWindowGot(true, bonus);
        }
    }

    public void GiveBonusViaCode(string code)
    {
        switch (code)
        {
            case "666666":
                {
                    if (!Infomanager.GetInstance().userData.myDecorations.Any(d => d.name == "wing_butterfly2"))
                    {
                        Bonus bonus = new Bonus();
                        bonus.id = -1;
                        bonus.type = 2;
                        bonus.name = "wing_butterfly2";
                        bonus.amount = 1;

                        GiveBonus(bonus, true);
                    }
                }
                break;
            case "777777":
                {
                    if (!Infomanager.GetInstance().userData.mySkins.Any(d => d.name == "唐纳德"))
                    {
                        Bonus bonus = new Bonus();
                        bonus.id = -1;
                        bonus.type = 1;
                        bonus.name = "唐纳德";
                        bonus.amount = 1;

                        GiveBonus(bonus, true);
                    }
                }
                break;
            default:
                break;
        }
    }

    private Bonus RandomBonusNotHave()
    {
        // => default(halo)
        Bonus bonus = new Bonus();
        bonus.id = -1;
        bonus.type = 2;
        bonus.name = "halo";
        bonus.amount = 1;

        //if something not have  =>  random
        SkinInfo[] skinsNotHave = Infomanager.GetInstance().skinInfos.Where(skin => !Infomanager.GetInstance().userData.mySkins.Contains(skin)).ToArray();
        DecorationInfo[] decosNotHave = Infomanager.GetInstance().decorationInfos.Where(deco => !Infomanager.GetInstance().userData.myDecorations.Contains(deco)).ToArray();
        if (Random.value < 0.5f && skinsNotHave.Length > 0)
        {
            bonus.id = -1;
            bonus.type = 1;
            bonus.name = skinsNotHave[Random.Range(0, skinsNotHave.Length)].name;
            bonus.amount = 1;
        }
        else if (decosNotHave.Length > 0)
        {
            bonus.id = -1;
            bonus.type = 2;
            bonus.name = decosNotHave[Random.Range(0, decosNotHave.Length)].name;
            bonus.amount = 1;
        }

        return bonus;
    }

    public bool MoneyEnough(int count)
    {
        if (Infomanager.GetInstance().userData.money >= count) return true;
        return false;
    }

    public bool DiamondEnough(int count)
    {
        if (Infomanager.GetInstance().userData.diamonds >= count) return true;
        return false;
    }

    public void GiveMoney(int count)
    {
        Infomanager.GetInstance().userData.money += count;

        //Refresh stuff
        userAssetsPanel.Find("TextMoney").GetComponent<Text>().text = Infomanager.GetInstance().userData.money.ToString();

        if (true)
        {
            if (skinSelect > -1) RefreshSkinInfo(skinSelect);
            if (decoSelect != null) RefreshDecoInfo(decoSelect);
            if (dsgSelect != null) RefreshDsgInfo(dsgSelect);
        }

        if (windowTalents.gameObject.activeInHierarchy)
        {
            if(tnodeSelect != null) RefreshTalentSelect(tnodeSelect);
        }
    }

    public void GiveDiamond(int count)
    {
        Infomanager.GetInstance().userData.diamonds += count;

        //Refresh stuff
        userAssetsPanel.Find("TextDiamond").GetComponent<Text>().text = Infomanager.GetInstance().userData.diamonds.ToString();
        if (skinSelect > -1) RefreshSkinInfo(skinSelect);
        if (decoSelect != null) RefreshDecoInfo(decoSelect);
        if (dsgSelect != null) RefreshDsgInfo(dsgSelect);
    }
    

    public void SpendMoney(int count)
    {
        Infomanager.GetInstance().userData.money -= count;

        //Refresh stuff
        userAssetsPanel.Find("TextMoney").GetComponent<Text>().text = Infomanager.GetInstance().userData.money.ToString();
        if (skinSelect > -1) RefreshSkinInfo(skinSelect);
        if (decoSelect != null) RefreshDecoInfo(decoSelect);
        if (dsgSelect != null) RefreshDsgInfo(dsgSelect);
    }

    public void SpendDiamond(int count)
    {
        Infomanager.GetInstance().userData.diamonds -= count;

        //Refresh stuff
        userAssetsPanel.Find("TextDiamond").GetComponent<Text>().text = Infomanager.GetInstance().userData.diamonds.ToString();
        if (skinSelect > -1) RefreshSkinInfo(skinSelect);
        if (decoSelect != null) RefreshDecoInfo(decoSelect);
        if (dsgSelect != null) RefreshDsgInfo(dsgSelect);
    }

    public void GiveSkin(SkinInfo newskin)
    {
        if (Infomanager.GetInstance().userData.mySkins.Contains(newskin))
        {
            //换取金币
            GiveMoney(newskin.price);
            return;
        }

        Infomanager.GetInstance().userData.mySkins.Add(newskin);
    }

    public void SetSkin(SkinInfo skin, bool force = false)
    {
        if (!force && !Infomanager.GetInstance().userData.mySkins.Contains(skin)) return;

        Infomanager.GetInstance().userData.activeSkin = skin;
    } 


    public void GiveDecoration(DecorationInfo decoInfo, bool speedMoney)
    {
        if (Infomanager.GetInstance().userData.myDecorations.Contains(decoInfo))
        {
            //换取金币
            GiveMoney(decoInfo.price);
            return;
        }

        Infomanager.GetInstance().userData.myDecorations.Add(decoInfo);

        if (speedMoney)
        {
            SpendMoney(decoInfo.price);
        }
        else
        {
            SpendMoney(0);
        }
    }
    

    public void SetDecoration(DecorationInfo deco, bool force = false)
    {
        if (!force && !Infomanager.GetInstance().userData.myDecorations.Contains(deco)) return;

        if (deco.group == "head")
        {
            Infomanager.GetInstance().userData.activeDecoration0 = deco;
        }
        if (deco.group == "back")
        {
            Infomanager.GetInstance().userData.activeDecoration1 = deco;
        }
        if (deco.group == "weapon")
        {
            Infomanager.GetInstance().userData.activeDecoration2 = deco;
        }

        //更新预览角色
        charactorPreview.ResetCharactor();
    }

    public void DetachDecoration(DecorationInfo deco)
    {
        if(Infomanager.GetInstance().userData.activeDecoration0 == deco)
        {
            Infomanager.GetInstance().userData.activeDecoration0 = null;
        }
        if (Infomanager.GetInstance().userData.activeDecoration1 == deco)
        {
            Infomanager.GetInstance().userData.activeDecoration1 = null;
        }
        if (Infomanager.GetInstance().userData.activeDecoration2 == deco)
        {
            Infomanager.GetInstance().userData.activeDecoration2 = null;
        }

        //更新预览角色
        charactorPreview.ResetCharactor();
    }

    public void GiveDisguise(DisguiseObjectInfo dsg, bool speedMoney)
    {
        if (Infomanager.GetInstance().userData.myDisguiseObjects.Contains(dsg))
        {
            //换取金币
            GiveMoney(dsg.price);
            return;
        }

        Infomanager.GetInstance().userData.myDisguiseObjects.Add(dsg);

        if (speedMoney)
        {
            SpendMoney(dsg.price);
        }
        else
        {
            SpendMoney(0);
        }
    }

    public void SetDisguise(DisguiseObjectInfo dsg, bool force = false)
    {
        if (!force && !Infomanager.GetInstance().userData.myDisguiseObjects.Contains(dsg)) return;

        Infomanager.GetInstance().userData.activeDisguiseObj = dsg;
    }
    
}
