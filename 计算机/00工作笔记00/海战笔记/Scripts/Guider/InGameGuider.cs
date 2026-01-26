using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class GuideInfo
{
    public int id;
    public System.Func<bool> whenToOpen;
    public System.Func<bool> whenToClose;

    public System.Action opencallback;
    public System.Action closecallback;

    public RectTransform target;

    public bool useDefaultImage;

    public int radius;

    public string guideTxt;



    public string fingerAnim; //""-no

    public GuideInfo()
    {
        opencallback = () => { };
        closecallback = () => { };
    }
}


public class InGameGuider : MonoBehaviour
{
    private static InGameGuider inst;


    //guide
    private List<GuideInfo> guideInfos;
    private static int currentGuideId;
    private static int[] isGuided;


    //dest gizmos
    private static Vector3[] destinationPos;
    private static int currentDestIndex;

    //player
    private GameObject player;


    //obj
    private UICanvas uicanvas;
    private GameObject destinationGizmos;

    public GameObject fingerAnim = null;

    //del
    private System.Func<bool> whenToCloseGuide;
    private System.Func<bool> whenToOpenGuide;


    //tmp
    private float timer = 0f;

    private float timerAutoLock = 0f;
    private bool rotated = false;
    







    private void Awake()
    {
        //硬编码
        inst = this;
        isGuided = new int[10] { 0, 0, 0, 0 ,0 ,0 ,0, 0 ,0 ,0 };
        destinationPos = new Vector3[3] { new Vector3(0f, 0f, 100f), new Vector3(0f, 0f, 200f), new Vector3(100f, 0f, 300f) };

        currentGuideId = 0;
        currentDestIndex = 0;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;

        uicanvas = FindObjectOfType<UICanvas>();

        destinationGizmos = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Destination"), new Vector3(99999f, 100f, 99999f), new Quaternion());
        destinationGizmos.SetActive(false);

        guideInfos = new List<GuideInfo>();

        //Guides
        {
            GuideInfo info0 = new GuideInfo();
            info0.id = 0;
            info0.whenToOpen = (() => { return true; });
            info0.whenToClose = () => { return timer < 0f; };
            info0.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 7, "version", InfoManager.Version, "goInterface");

                timer = 4f;
            });
            info0.target = uicanvas.transform.Find("UI_UserControls").Find("MovePanel").Find("Rotator1").GetComponent<RectTransform>();

            info0.useDefaultImage = true;
            info0.radius = 0;

            info0.guideTxt = "你好，指挥官，欢迎来到战场！";

            info0.fingerAnim = "";

            guideInfos.Add(info0);
        }

        //Guides
        {
            GuideInfo info1 = new GuideInfo();
            info1.id = 1;
            info1.whenToOpen = (() => { return isGuided[0] == 2; });
            info1.whenToClose = () => { return Mathf.Abs( inst.player.GetComponent<IShipController>().Throttle ) > 0.1f; };
            info1.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 8, "version", InfoManager.Version, "goInterface");

                ShowNextDestination(null);
            });
            info1.closecallback = () =>
            {
                timer = 1f;
            };
            info1.target = uicanvas.transform.Find("UI_UserControls").Find("MovePanel").Find("Rotator1").GetComponent<RectTransform>();

            info1.useDefaultImage = true;
            info1.radius = 150;

            info1.guideTxt = "拖动这里移动战舰！";

            info1.fingerAnim = "GuideSlideUp";

            guideInfos.Add(info1);//2,2,1,0,0,0,0,0,0,0
        }


        //Guides
        {
            GuideInfo info2 = new GuideInfo();
            info2.id = 2;
            info2.whenToOpen = (() => { return isGuided[1] == 2 && timer < 0f;  });//Debug.Log(destinationGizmos.transform.position);  
            info2.whenToClose = () => { return false; };
            info2.opencallback = (() => {

                //埋点
                MaiDian.Mai("number", 9, "version", InfoManager.Version, "goInterface");

                HideDestination();
                uicanvas.transform.Find("DragPanel").GetComponent<UIDragPanel>().onBeginDrag.AddListener(() => { HideGuideMask(); uicanvas.transform.Find("DragPanel").GetComponent<UIDragPanel>().onBeginDrag.RemoveAllListeners(); });
            });
            info2.closecallback = (() => {
                timerAutoLock = 5f;
            });
            info2.target = uicanvas.transform.Find("DragPanel").GetComponent<RectTransform>();

            info2.useDefaultImage = true;
            info2.radius = 250;

            info2.guideTxt = "滑动屏幕移动视角！";

            info2.fingerAnim = "GuideSlideRight";

            guideInfos.Add(info2);
        }

        //Guides
        {
            GuideInfo info3 = new GuideInfo();
            info3.id = 3;
            info3.whenToOpen = (() => { return player.GetComponent<PlayerController>().TargetLock != null && uicanvas.CanFireAll; });
            info3.whenToClose = () => { return player.GetComponent<PlayerController>().PrimaryFiringOrder; };//监听开火事件。
            info3.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 10, "version", InfoManager.Version, "goInterface");

                Time.timeScale = 0f;
            });
            info3.closecallback = (() => { timer = 4f; });
            info3.target = uicanvas.transform.Find("UI_UserControls").Find("ButtonFire").GetComponent<RectTransform>();

            info3.useDefaultImage = true;
            info3.radius = 150;

            info3.guideTxt = "按住开火按钮发射炮弹！";

            info3.fingerAnim = "GuidePress";

            guideInfos.Add(info3);
        }


        //Guides
        {
            GuideInfo info4 = new GuideInfo();
            info4.id = 4;
            info4.whenToOpen = (() => { return isGuided[3] == 2 && timer < 0f; });//Debug.Log(destinationGizmos.transform.position);  
            info4.whenToClose = (() => { return timer < 0f; });
            info4.opencallback = () => {
                //埋点
                MaiDian.Mai("number", 11, "version", InfoManager.Version, "goInterface");

                FindObjectOfType<AIController>().aiMode = AIMode.Hunt;
                timer = 2f;
            };
            info4.closecallback = () => {
                timer = 4f;
            };
            info4.target = uicanvas.transform.Find("UI_UserControls").Find("IndSecondary").GetComponent<RectTransform>();

            info4.useDefaultImage = true;
            info4.radius = 65;

            info4.guideTxt = "当锁定的目标进入副炮射程时，副炮会自动开火。";

            info4.fingerAnim = "";

            guideInfos.Add(info4);
        }

        //Guides
        {
            GuideInfo info5 = new GuideInfo();
            info5.id = 5;
            info5.whenToOpen = (() => { return (isGuided[3] == 2 && isGuided[4] == 2 && timer < 0f); });//Debug.Log(destinationGizmos.transform.position);  
            info5.whenToClose = (() => { return player.GetComponent<AbilityRepair>().CanUse == false; });
            info5.opencallback = () => {
                //埋点
                MaiDian.Mai("number", 12, "version", InfoManager.Version, "goInterface");

                Time.timeScale = 0f;
            };
            info5.closecallback = () => {
                Time.timeScale = 1f;
            };
            info5.target = uicanvas.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonAbility1").GetComponent<RectTransform>();

            info5.useDefaultImage = true;
            info5.radius = 65;

            info5.guideTxt = "哦！我们的战舰受伤了，使用技能来维修战舰！";

            info5.fingerAnim = "GuidePress";

            guideInfos.Add(info5);
        }
        ////Guides
        //{
        //    GuideInfo info6 = new GuideInfo();
        //    info6.id = 6;
        //    info6.whenToOpen = (() => { return isGuided[5] == 2 && player.GetComponent<PlayerController>().TargetLock != null; });//Debug.Log(destinationGizmos.transform.position);  
        //    info6.whenToClose = (() => { return player.GetComponent<AbilityMissile>().CD > 0.5f; });
        //    info6.opencallback = () => {
        //        player.GetComponentsInChildren<Turret>().ToList().ForEach(t => t.enabled = false);
        //        player.GetComponent<Hull>().shipPowerMultip = 100f;
        //        Time.timeScale = 0f;
        //    };
        //    info6.closecallback = () => {
        //        Time.timeScale = 1f;
        //        timer = 1.5f;
        //    };
        //    info6.target = uicanvas.transform.Find("UI_UserControls").Find("AbilityPanel").Find("ButtonAbility2").GetComponent<RectTransform>();

        //    info6.useDefaultImage = true;
        //    info6.radius = 65;

        //    info6.guideTxt = "做的好，现在使用巡航导弹给敌人最后一击！";

        //    info6.fingerAnim = "GuidePress";

        //    guideInfos.Add(info6);
        //}

        ////Guides
        //{
        //    GuideInfo info7 = new GuideInfo();
        //    info7.id = 7;
        //    info7.whenToOpen = (() => { return isGuided[6] == 2 && timer < 0f; });//Debug.Log(destinationGizmos.transform.position);  
        //    info7.whenToClose = (() => { return (uicanvas.isChasingShell || timer < -5f); });
        //    info7.opencallback = () => {
        //        player.GetComponentsInChildren<Turret>().ToList().ForEach(t => t.enabled = false);
        //        player.GetComponent<Hull>().shipPowerMultip = 100f;
        //        //Time.timeScale = 0f;
        //    };
        //    info7.closecallback = () => {
        //    };
        //    info7.target = uicanvas.transform.Find("UI_UserControls").Find("ButtonLock").GetComponent<RectTransform>();

        //    info7.useDefaultImage = true;
        //    info7.radius = 40;

        //    info7.guideTxt = "点击这里可以锁定自己发射的炮弹或者导弹！";

        //    info7.fingerAnim = "GuidePress";

        //    guideInfos.Add(info7);
        //}

        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 6;
            info.whenToOpen = (() => { return FindObjectOfType<AIController>().Enabled == false; });//2 sec after enemy die
            info.whenToClose = (() => { return true; });
            info.opencallback = () => {
                timer = 3f;
            };
            info.target = uicanvas.transform.Find("DragPanel").GetComponent<RectTransform>();

            info.useDefaultImage = true;
            info.radius = 5000;

            info.guideTxt = "已经击沉敌舰";

            info.fingerAnim = "";

            guideInfos.Add(info);
        }

        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 7;
            info.whenToOpen = (() => { return timer < 0f ; });//2 sec after enemy die  FindObjectOfType<AIController>().Enabled == false && (uicanvas.isChasingShell == false)
            info.whenToClose = (() => { return false; });
            info.opencallback = () => {
                //埋点
                MaiDian.Mai("number", 14, "version", InfoManager.Version, "goInterface");

                Invoke("BackMainScene", 4.5f);
            };
            info.target = uicanvas.transform.Find("DragPanel").GetComponent<RectTransform>();

            info.useDefaultImage = true;
            info.radius = 0;

            info.guideTxt = "恭喜你完成了新手教程！";

            info.fingerAnim = "";

            guideInfos.Add(info);
        }

        //init
        whenToOpenGuide = guideInfos.Find(g => g.id == currentGuideId).whenToOpen;
    }


    private void Update()
    {
        //---
        timer -= Time.deltaTime;
        //---

        if(isGuided[2] == 2 && !rotated)
        {
            timerAutoLock -= Time.deltaTime;
            if (timerAutoLock < 0f)
            {
                rotated = true;

                Debug.Log("sbmili==false");
                UIDragPanel.enableDrag = false;
                StartCoroutine(CoForceRotate());
            }
        }
        //---

        inst.whenToOpenGuide();

        if (whenToOpenGuide != null && whenToOpenGuide())
        {
            Guide();
        }


        if (whenToCloseGuide != null && whenToCloseGuide())
        {
            HideGuideMask();
            whenToCloseGuide = (() => { return false; });
        }
        
        
    }

    private IEnumerator CoForceRotate()
    {

        //uicanvas.panelDrag.SetActive(false);


        yield return new WaitForSeconds(0.1f);

        FindObjectOfType<CameraSphere>().RotateTo(FindObjectOfType<AIController>().transform.position - Camera.main.transform.position);


        yield return new WaitForSeconds(2.45f);

        //uicanvas.panelDrag.SetActive(true);
        Debug.Log("sbmili==true");
        UIDragPanel.enableDrag = true;
    }
    

    public static void Guide(float delay)
    {
        inst.StartCoroutine(inst.CoGuide(delay));
    }

    private IEnumerator CoGuide(float delay)
    {
        yield return new WaitForSeconds(delay);

        Guide();
    }

    public static void Guide()
    {

        GuideInfo info = inst.guideInfos.Find(g => g.id == currentGuideId);


        inst.SetGuideMask(info.guideTxt, info.target, info.fingerAnim, -1f, info.useDefaultImage, info.radius);

        isGuided[currentGuideId] = 1;

        inst.whenToCloseGuide = info.whenToClose;

        if (info.opencallback != null)
        {
            info.opencallback();
        }
        else
        {
        }


        //++++
        currentGuideId++;//!!!!!!!!!!!!!!

        if (inst.guideInfos.FirstOrDefault(g => g.id == currentGuideId) != null)
        {
            inst.whenToOpenGuide = inst.guideInfos.Find(g => g.id == currentGuideId).whenToOpen;
            
        }
        else
        {
            inst.whenToOpenGuide = () => { return false; };
        }

    }


    /// <summary>
    /// Dest Arrow
    /// </summary>
    /// <param name="callback"></param>
    private static void ShowNextDestination(Action callback)
    {
        if(currentDestIndex < destinationPos.Length)
        {
            inst.destinationGizmos.SetActive(true);
            inst.destinationGizmos.transform.position = destinationPos[currentDestIndex];

            currentDestIndex++;
            //...
        }
    }

    private static void HideDestination()
    {
        inst.destinationGizmos.SetActive(false);
        inst.destinationGizmos.transform.position = new Vector3(99999f, 100f, 99999f);
    }



    /// <summary>
    /// MASK
    /// </summary>
    /// <param name="txt"></param>
    /// <param name="rectt"></param>
    /// <param name="showTime"></param>
    private void SetGuideMask(string txt, RectTransform rectt, string fingerAnim, float showTime = -1f, bool useDefaultImage = false, int radius = 100)
    {
        if (inst.uicanvas.GetComponentInChildren<GuideMask>(true).isShowing)
        {
            HideGuideMask();
        }

        //Set Focus
        inst.uicanvas.GetComponentInChildren<GuideMask>(true).SetTarget(rectt, useDefaultImage, radius);
        inst.uicanvas.GetComponentInChildren<GuideMask>(true).SetText( "<b>" + txt + "</b>");

        //Set Finger Anim
        if(fingerAnim != "")
        {
            inst.fingerAnim = Instantiate(Resources.Load<GameObject>("Prefabs/" + fingerAnim), rectt.transform.position, rectt.transform.rotation, inst.uicanvas.transform);
        }

        if (showTime > 0f)
        {
            Invoke("HideGuideMask", showTime);
        }
    }

    private void HideGuideMask()
    {
        if(currentGuideId - 1 > -1)
        {
            isGuided[currentGuideId - 1] = 2;
            inst.guideInfos.Find(g => g.id == currentGuideId - 1).closecallback();

        }
        inst.uicanvas.GetComponentInChildren<GuideMask>(true).Close();
        

        //anim
        if(fingerAnim != null)
        {
            Destroy(fingerAnim);
            fingerAnim = null;
        }

        //Time
        Time.timeScale = 1f;
    }






    private void BackMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SceneMainMenu");
        InfoManager.showAchieveOnLoad = false;
    }
    private void OnGUI()
    {
        string str = "";
        for (int i = 0; i < isGuided.Length; i++)
        {
            str += isGuided[i].ToString() + ",";
        }
        GUILayout.TextArea(str);
    }
}
