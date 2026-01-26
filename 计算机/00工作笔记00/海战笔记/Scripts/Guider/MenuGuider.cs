using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

class MenuGuideInfo
{
    public int id;

    public System.Func<bool> whenToOpen;

    public System.Action openCallback;
    
    public bool tapAnyPlaceToClose;
    public RectTransform focus;

    public System.Action closeCallback;

    public string txt;

    public float duration;

    //自定义回调
    public System.Action btnCustomOnClick = null;

    public float bgDarkness = 0.65f;
}

public class MenuGuider : MonoBehaviour
{
    private static MenuGuider inst;

    private int currentId;

    private static int[] isGuided = new int[10];//!!!!!!!!!!!!Link To Infomanager.Userdate

    private List<MenuGuideInfo> guides;



    //temp
    private float timer = 0f;


    //pointers
    private MainMenuCanvas menuCanvas;

    private GameObject mask;
    private GameObject obtn;
    private GameObject finger;
    private Transform t2Follow;

    private void Awake()
    {
        Debug.Log("Awake");

        isGuided = InfoManager.GetInstance().userData.guideProgress;

        inst = this;

        inst.menuCanvas = MonoBehaviour.FindObjectOfType<MainMenuCanvas>();
        inst.mask = this.transform.Find("GUIDEMASK").gameObject;
        inst.obtn = inst.mask.GetComponentInChildren<Button>().gameObject;
        inst.finger = inst.mask.transform.Find("GuidePress").gameObject;

        //guides init
        guides = new List<MenuGuideInfo>();

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 0;
            guide.txt = "点击这里进入教程";
            guide.whenToOpen = () => { return true; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("LeftPanel").Find("ButtonMissions").GetComponent<RectTransform>();
            guide.btnCustomOnClick = () => { inst.menuCanvas.isAtMainWindow = false; inst.menuCanvas.LoadTutorialLevel(); };
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 1;
            guide.txt = "已获得默认战舰[阿瑞斯号]";
            guide.whenToOpen = () => { return isGuided[0] == 2 && inst.menuCanvas.isAtMainWindow == true; };//?????????点击第一个指引后就自动启动了 // click to close to false // 2. Delay
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = true;
            guide.focus = null;
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 2;
            guide.txt = "你现在拥有自己的战舰了";
            guide.whenToOpen = () => { return isGuided[1] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = true;
            guide.focus = null;
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 3;
            guide.txt = "现在去用战胜敌人获得的金币升级你的战舰吧";
            guide.whenToOpen = () => { return isGuided[2] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("RightPanel").Find("ButtonShipWindow").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 4;
            guide.txt = "点击升级按钮升级当前战舰。";
            guide.whenToOpen = () => { return isGuided[3] == 2; };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 15, "version", InfoManager.Version, "goInterface");
            };
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowShips").Find("ShipUpgradePanel").Find("ButtonUpgrade").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 5;
            guide.txt = "现在，返回主菜单。";
            guide.whenToOpen = () => { return isGuided[4] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowShips").Find("UpperPanel").Find("ButtonReturn").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 6;
            guide.txt = "参加你的第一场实战吧！";
            guide.whenToOpen = () => { return isGuided[5] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("LeftPanel").Find("ButtonMatch").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = () => {
                //埋点
                MaiDian.Mai("number", 16, "version", InfoManager.Version, "goInterface");

                menuCanvas.isAtMainWindow = false;
            };

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 7;
            guide.txt = "哇，你打扫战场的时候发现了一个强力装备！现在去装备它吧！";
            guide.whenToOpen = () => { return isGuided[6] == 2 && inst.menuCanvas.isAtMainWindow == true; };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 17, "version", InfoManager.Version, "goInterface");

                MainMenuCanvas.GiveNewCom(InfoManager.GetInstance().weaponInfos.Find(w => w.rank == "B"));
            };
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("RightPanel").Find("ButtonEquipmentWindow").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 8;
            guide.txt = "点击这件装备即可装备上！";
            guide.whenToOpen = () => { return isGuided[7] == 2; };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 18, "version", InfoManager.Version, "goInterface");
            };
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowEquipments").Find("EquipmentsPanel").Find("Weapons").GetComponentInChildren<ScrollRect>().content.GetChild(0).Find("Image").GetComponent<RectTransform>();//index??
            guide.btnCustomOnClick = () => { this.transform.Find("Windows").Find("WindowEquipments").Find("EquipmentsPanel").Find("Weapons").GetComponentInChildren<ScrollRect>().content.GetChild(0).GetComponent<Button>().onClick.Invoke(); };
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 9;
            guide.txt = "干得好！你的战舰能力又增强了！挑战实力强劲的敌人会获得更强的装备，也会更危险。";
            guide.whenToOpen = () => { return isGuided[8] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowEquipments").Find("UpperPanel").Find("ButtonReturn").GetComponent<RectTransform>();//index??
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 10;
            guide.txt = "限时活动进行中，可以领取限量版战舰！";
            guide.whenToOpen = () => { return isGuided[9] == 2; };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 19, "version", InfoManager.Version, "goInterface");
            };
            guide.tapAnyPlaceToClose = true;
            guide.focus = this.transform.Find("Windows").Find("WindowActivity").Find("ButtonShowTryGet").GetComponent<RectTransform>();//index??
            guide.btnCustomOnClick = null;
            guide.closeCallback = () => {
                menuCanvas.ShowWindowActivity();
            };

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 11;
            guide.txt = "进入任务关卡更强的敌人吧！";
            guide.whenToOpen = () => { return isGuided[10] == 2 && !this.transform.Find("Windows").Find("WindowActivity").gameObject.activeInHierarchy && !this.transform.Find("Windows").Find("WindowTryGetShip").gameObject.activeInHierarchy; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("LeftPanel").Find("ButtonMissions").GetComponent<RectTransform>();//index??
            guide.btnCustomOnClick = null;
            guide.closeCallback = () => {
                //埋点
                MaiDian.Mai("number", 20, "version", InfoManager.Version, "goInterface");
                menuCanvas.isAtMainWindow = false;
            };

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 12;
            guide.txt = "一个优秀的船员是战舰变强的必要条件之一。";
            guide.whenToOpen = () => {
                return isGuided[11] == 2 && inst.menuCanvas.isAtMainWindow == true;
            };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 21, "version", InfoManager.Version, "goInterface");

            };
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowMain").Find("AccPanel").Find("RightPanel").Find("ButtonCrew").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 13;
            guide.txt = "现在去招募一个船员试试吧。";
            guide.whenToOpen = () => { return isGuided[12] == 2; };
            guide.openCallback = () => {
                //埋点
                MaiDian.Mai("number", 22, "version", InfoManager.Version, "goInterface");
            };
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowCrew").Find("LowerPanel").Find("ButtonHire").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guide.bgDarkness = 0.25f;

            guides.Add(guide);
        }

        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 14;
            guide.txt = "你招募了一个老练的船员。点击这里继续。";
            guide.whenToOpen = () => { return isGuided[13] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowGetCrew").Find("ButtonClose").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guide.bgDarkness = 0.0f;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 15;
            guide.txt = "我们让他上任吧。";
            guide.whenToOpen = () => { return isGuided[14] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowCrew").Find("CrewsPanel").GetComponent<ScrollRect>().content.GetChild(0).Find("Image").GetComponent<RectTransform>();
            guide.btnCustomOnClick = () => { this.transform.Find("Windows").Find("WindowCrew").Find("CrewsPanel").GetComponent<ScrollRect>().content.GetChild(0).GetComponent<Button>().onClick.Invoke(); };
            guide.closeCallback = null;

            guide.bgDarkness = 0.25f;

            guides.Add(guide);
        }
        //-------------------------------------
        {
            MenuGuideInfo guide = new MenuGuideInfo();
            guide.id = 16;
            guide.txt = "现在，你已经是一个合格的指挥官了，去挑战更多更强大的敌人吧。";
            guide.whenToOpen = () => { return isGuided[15] == 2; };
            guide.openCallback = null;
            guide.tapAnyPlaceToClose = false;
            guide.focus = this.transform.Find("Windows").Find("WindowCrew").Find("UpperPanel").Find("ButtonReturn").GetComponent<RectTransform>();
            guide.btnCustomOnClick = null;
            guide.closeCallback = null;

            guides.Add(guide);
        }

        //
    }

    void Start()
    {
        
    }
    

    void Update()
    {
        //---
        timer -= Time.deltaTime;
        //---



        Guide();//!!!!!!!!!!!!Invoke In Events To Reduce Cost

        //Btn follow
        if (t2Follow != null)
        {
            obtn.transform.position = t2Follow.position;
            obtn.transform.rotation = t2Follow.rotation;

            if (finger.activeInHierarchy)
            {
                finger.transform.position = t2Follow.position;
                finger.transform.rotation = t2Follow.rotation;
            }
        }
    }




    public static void Guide()
    {
        foreach(var guide in inst.guides)
        {
            int idTmp = guide.id;

            if (guide.whenToOpen() && isGuided[guide.id] == 0)
            {


                //Acivate Guide
                inst.mask.SetActive(true);
                //BG darkness
                inst.mask.GetComponent<Image>().color = new Color(0f, 0f, 0f, guide.bgDarkness);

                //text
                inst.mask.GetComponentInChildren<Text>().text = "<b>" + guide.txt + "</b>";

                //openCallback
                guide.openCallback?.Invoke();

                //how to close
                if (guide.tapAnyPlaceToClose)
                {
                    inst.obtn.GetComponent<Animation>().enabled = false;

                    inst.obtn.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
                    inst.obtn.GetComponent<Image>().sprite = null;

                    inst.obtn.GetComponent<RectTransform>().anchoredPosition = new Vector2();
                    inst.obtn.GetComponent<RectTransform>().anchorMin = new Vector2();
                    inst.obtn.GetComponent<RectTransform>().anchorMax = new Vector2();
                    inst.obtn.GetComponent<RectTransform>().sizeDelta = new Vector2(5000, 5000);
                    inst.obtn.transform.localScale = Vector3.one;
                    inst.t2Follow = null;

                    //finger
                    inst.finger.gameObject.SetActive(false);
                }
                else
                {
                    inst.obtn.GetComponent<Animation>().enabled = true;

                    inst.obtn.GetComponent<Image>().color = guide.focus.GetComponent<Image>().color;
                    inst.obtn.GetComponent<Image>().sprite = guide.focus.GetComponent<Image>().sprite;

                    inst.obtn.transform.SetParent(null);
                    inst.obtn.GetComponent<RectTransform>().sizeDelta = guide.focus.sizeDelta;
                    inst.obtn.GetComponent<RectTransform>().pivot = guide.focus.pivot;

                    inst.obtn.transform.localScale = guide.focus.lossyScale;
                    inst.obtn.transform.SetParent(inst.mask.transform);
                    inst.obtn.transform.SetAsFirstSibling();
                    inst.t2Follow = guide.focus;


                    //finger
                    inst.finger.SetActive(true);
                    inst.finger.GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToViewportPoint(guide.focus.transform.position) * inst.menuCanvas.GetComponent<RectTransform>().sizeDelta;
                }

                inst.mask.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                inst.mask.GetComponentInChildren<Button>().onClick.AddListener(() => {
                    //Invoke Focus Button
                    if(guide.btnCustomOnClick != null)
                    {
                        guide.btnCustomOnClick();
                    }
                    else if(!guide.tapAnyPlaceToClose)
                    {
                        guide.focus.GetComponent<Button>().onClick?.Invoke();
                    }

                    //Hide Mask
                    inst.mask.SetActive(false);
                    isGuided[idTmp] = 2;

                    //callback
                    guide.closeCallback?.Invoke();
                });


                isGuided[guide.id] = 1;
            }
        }
    }
    


    //private void OnGUI()
    //{
    //    string str = "";
    //    for(int i = 0; i < isGuided.Length; i++)
    //    {
    //        str += isGuided[i].ToString() + "\n";
    //    }

    //    GUILayout.TextArea(str);
    //}
}

