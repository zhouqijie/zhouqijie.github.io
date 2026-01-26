using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;



public class GuideInfo
{
    public int id;
    public System.Func<bool> whenToOpen;
    public System.Func<bool> whenToClose;

    public System.Action opencallback;
    public System.Action closecallback;

    public bool useDefaultImage;

    public int radius;

    public string guideTxt;

    public RectTransform fingerTarget;

    public Vector2 fingerMovement;

    

    public GuideInfo()
    {
        opencallback = () => { };
        closecallback = () => { };

        fingerTarget = null;
        fingerMovement = new Vector2();
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


    //time tmp
    private float timer = 0f;
    private int frameCount = 0;

    //isShowing
    private bool isShowing = false;

    //------finger tmp------
    private Sequence seq = null;

    //-------tmp-------
    private Vector3 dirTmp = new Vector3();

    private CharactorController playerController = null;
    private PlayerInput playerInput;
    private CharactorController playerChaseTmp = null;
    private IGame currentGame = null;




    private void Awake()
    {
        //硬编码
        inst = this;
        isGuided = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        destinationPos = new Vector3[3] { new Vector3(0f, 0f, 100f), new Vector3(0f, 0f, 200f), new Vector3(100f, 0f, 300f) };

        currentGuideId = 0;
        currentDestIndex = 0;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerInput>().gameObject;

        //tmp
        playerController = player.GetComponent<CharactorController>();
        playerInput = player.GetComponent<PlayerInput>();
        currentGame = FindObjectOfType<GameStarter>().Game;

        uicanvas = FindObjectOfType<UICanvas>();

        //destinationGizmos = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Destination"), new Vector3(99999f, 100f, 99999f), new Quaternion());
        //destinationGizmos.SetActive(false);


        #region GUIDES_BATTLE

        List<GuideInfo> guidesShooter = new List<GuideInfo>();

        //Guides
        {
            GuideInfo info0 = new GuideInfo();
            info0.id = 0;
            info0.whenToOpen = (() => { return currentGame.GameTime > 3f; });
            info0.whenToClose = () => { return timer < 0f; };
            info0.opencallback = (() => {
                Infomanager.guidesNum++;

                //埋点(新手教程第二局)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 19, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 21, "version", Infomanager.Version, "goInterface");
                }

                playerController.aiInvisiable = true;

                timer = 4f;
            });

            info0.guideTxt = "你好，欢迎来到战场！";


            guidesShooter.Add(info0);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 1;
            info.whenToOpen = (() => { return isGuided[0] == 2; });
            info.whenToClose = () => { return Vector3.Angle(player.transform.forward, dirTmp) > 20f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 2, "version", Infomanager.Version, "goInterface");
                
                dirTmp = player.transform.forward;
            });
            info.closecallback = () =>
            {
                timer = 2f;
            };

            info.guideTxt = "滑动屏幕右侧旋转视角";

            info.fingerTarget = uicanvas.transform.Find("DragPanel").GetComponent<RectTransform>();
            info.fingerMovement = new Vector2(60, 0);

            guidesShooter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 2;
            info.whenToOpen = (() => { return isGuided[1] == 2 && timer < 0f; });
            info.whenToClose = () => { return playerInput.MoveVec.magnitude > 0.5f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 3, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = () =>
            {
                timer = 2f;
                
            };

            info.guideTxt = "拖动摇杆移动角色";


            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("MovePanel").GetComponent<RectTransform>();
            info.fingerMovement = new Vector2(0, 50);

            guidesShooter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 3;
            info.whenToOpen = (() => { return isGuided[2] == 2 && timer < 0f; });
            info.whenToClose = () => { return playerController.ConstantFiring == true; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 4, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = () =>
            {
                timer = 5f; //unneccessary

                //命令一个附近AI过来
                var aiinput = FindObjectsOfType<AIInput1>().Where(ai => ai.enabled).OrderBy(ai => (ai.transform.position - player.transform.position).sqrMagnitude).First();
                playerChaseTmp = aiinput.GetComponent<CharactorController>();
                playerChaseTmp.aiInvisiable = true;
                playerChaseTmp.Heal(100f);

                aiinput.FoolChase(playerController);
            };

            info.guideTxt = "点击开火按钮开火";

            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("HunterStuff").Find("ButtonFire").GetComponent<RectTransform>();

            guidesShooter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 4;
            info.whenToOpen = (() => { return isGuided[3] == 2 && timer < 0f && ((playerChaseTmp.transform.position - player.transform.position).sqrMagnitude < 100f || playerChaseTmp.enabled == false); }); //死了或者靠近都到这一步
            info.whenToClose = () => { return playerController.Kills > 0; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 5, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = () =>
            {
                timer = 2f;
            };

            info.guideTxt = "是敌人！快击败他";

            guidesShooter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 5;
            info.whenToOpen = (() => { return isGuided[4] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 6, "version", Infomanager.Version, "goInterface");

                timer = 3f;
            });
            info.closecallback = () =>
            {
                timer = 3f;
            };

            info.guideTxt = "干得好！击败所有敌人后胜利";

            guidesShooter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 6;
            info.whenToOpen = (() => { return isGuided[5] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 7, "version", Infomanager.Version, "goInterface");

                timer = 3f;
            });
            info.closecallback = () =>
            {
                //埋点(新手教程第二局结束)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 20, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局结束)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 22, "version", Infomanager.Version, "goInterface");
                }

                playerController.aiInvisiable = false;

                timer = 10f;
            };

            info.guideTxt = "安全区会随时间缩小，当你在安全区外，会受到伤害";

            guidesShooter.Add(info);
        }
        #endregion


        #region GUIDES_HUNTER

        List<GuideInfo> guidesHunter = new List<GuideInfo>();

        //Guides
        {
            GuideInfo info0 = new GuideInfo();
            info0.id = 0;
            info0.whenToOpen = (() => { return currentGame.GameTime > 3f; });
            info0.whenToClose = () => { return timer < 0f; };
            info0.opencallback = (() => {
                Infomanager.guidesNum++;

                //埋点(新手教程第二局)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 19, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 21, "version", Infomanager.Version, "goInterface");
                }

                playerController.aiInvisiable = true;

                timer = 4f;
            });

            info0.guideTxt = "你好，欢迎来到追踪者模式！";


            guidesHunter.Add(info0);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 1;
            info.whenToOpen = (() => { return isGuided[0] == 2; });
            info.whenToClose = () => { return Vector3.Angle(player.transform.forward, dirTmp) > 20f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 8, "version", Infomanager.Version, "goInterface");

                dirTmp = player.transform.forward;
            });
            info.closecallback = () =>
            {
                timer = 2f;
            };

            info.guideTxt = "滑动屏幕右侧旋转视角";

            info.fingerTarget = uicanvas.transform.Find("DragPanel").GetComponent<RectTransform>();
            info.fingerMovement = new Vector2(60, 0);

            guidesHunter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 2;
            info.whenToOpen = (() => { return isGuided[1] == 2 && timer < 0f; });
            info.whenToClose = () => { return playerInput.MoveVec.magnitude > 0.5f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 9, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = () =>
            {
                timer = 4f;
            };

            info.guideTxt = "拖动摇杆移动角色";
            
            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("MovePanel").GetComponent<RectTransform>();
            info.fingerMovement = new Vector2(0, 50);

            guidesHunter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 3;
            info.whenToOpen = (() => { return isGuided[2] == 2 && timer < 0f; });
            info.whenToClose = () => { return playerController.ConstantFiring == true; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 10, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = () =>
            {
                timer = 5f;
            };

            info.guideTxt = "点击开火按钮开火";

            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("HunterStuff").Find("ButtonFire").GetComponent<RectTransform>();

            guidesHunter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 4;
            info.whenToOpen = (() => { return isGuided[3] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 11, "version", Infomanager.Version, "goInterface");

                timer = 2f;
            });
            info.closecallback = () =>
            {
                timer = 5f;
            };

            info.guideTxt = "你的身份是追踪者，快到处转转吧！";

            guidesHunter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 5;
            info.whenToOpen = (() => { return isGuided[4] == 2 && timer < 0f; });
            info.whenToClose = () => { return !playerChaseTmp.enabled; }; //Hidder killed
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 12, "version", Infomanager.Version, "goInterface");

                //Instantiate Hidder
                playerChaseTmp = FindObjectOfType<ClassicGame>().SpawnAndIntFool(player.transform.position + player.transform.forward * 3f, new Quaternion());
            });
            info.closecallback = () =>
            {
                timer = 2f; 
            };

            info.guideTxt = "是躲藏者！躲藏者会变成道具蒙混过关，快击败他";

            guidesHunter.Add(info);
        }

        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 6;
            info.whenToOpen = (() => { return isGuided[5] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; }; //Hidder killed
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 13, "version", Infomanager.Version, "goInterface");

                timer = 2f;
            });
            info.closecallback = () =>
            {
                timer = 3f;
            };

            info.guideTxt = "干得好！击败所有躲藏者后胜利";

            guidesHunter.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 7;
            info.whenToOpen = (() => { return isGuided[6] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; }; //Hidder killed
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 14, "version", Infomanager.Version, "goInterface");

                timer = 2f;
            });
            info.closecallback = () =>
            {
                //埋点(新手教程第二局结束)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 20, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局结束)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 22, "version", Infomanager.Version, "goInterface");
                }

                timer = 99f;
            };

            info.guideTxt = "安全区会随时间缩小，当你在安全区外，会受到伤害";

            guidesHunter.Add(info);
        }
        #endregion


        #region GUIDES_HIDDER

        List<GuideInfo> guidesHidder = new List<GuideInfo>();

        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 0;
            info.whenToOpen = (() => { return currentGame.GameTime > 3f; });
            info.whenToClose = () => { return timer < 0f; };
            info.opencallback = (() => {
                Infomanager.guidesNum++;

                //埋点(新手教程第二局)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 19, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 21, "version", Infomanager.Version, "goInterface");
                }

                playerController.aiInvisiable = true;

                timer = 4f;
            });

            info.guideTxt = "你好，欢迎来到躲藏者模式！";


            guidesHidder.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 1;
            info.whenToOpen = (() => { return isGuided[0] == 2 && timer < 0f; });
            info.whenToClose = () => { return uicanvas.transform.Find("Windows").Find("WindowDisguise").gameObject.activeInHierarchy; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 15, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = (() => {
                timer = 2f;
            });

            info.guideTxt = "点击变身按钮选择变身";

            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("HidderStuff").Find("ButtonDisguise").GetComponent<RectTransform>();


            guidesHidder.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 2;
            info.whenToOpen = (() => { return isGuided[1] == 2 && timer < 0f && !uicanvas.transform.Find("Windows").Find("WindowDisguise").gameObject.activeInHierarchy; });
            info.whenToClose = () => { return !playerController.IsDisguised; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 16, "version", Infomanager.Version, "goInterface");
            });
            info.closecallback = (() => {
                timer = 4f;
            });

            info.guideTxt = "再次点击变身按钮可以取消变身";

            info.fingerTarget = uicanvas.transform.Find("UserStuff").Find("HidderStuff").Find("ButtonDisguise").GetComponent<RectTransform>();


            guidesHidder.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 3;
            info.whenToOpen = (() => { return isGuided[2] == 2 && timer < 0f; });
            info.whenToClose = () => { return timer < 0f; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 17, "version", Infomanager.Version, "goInterface");

                timer = 3f;
            });
            info.closecallback = (() => {
                timer = 4f;
            });

            info.guideTxt = "你的身份是躲藏者，快找个房子进去不要被发现！";


            guidesHidder.Add(info);
        }
        //Guides
        {
            GuideInfo info = new GuideInfo();
            info.id = 4;
            info.whenToOpen = (() => { return isGuided[3] == 2 && timer < 0f && player.transform.position.y > 4.9f && playerController.IsGround; });
            info.whenToClose = () => { return timer < 0f && playerController.IsDisguised; };
            info.opencallback = (() => {
                //埋点
                MaiDian.Mai("number", 18, "version", Infomanager.Version, "goInterface");

                timer = 2f;
            });
            info.closecallback = (() => {

                //埋点(新手教程第二局结束)
                if (Infomanager.guidesNum == 2)
                {
                    MaiDian.Mai("number", 20, "version", Infomanager.Version, "goInterface");
                }
                //埋点(新手教程第三局结束)
                if (Infomanager.guidesNum == 3)
                {
                    MaiDian.Mai("number", 22, "version", Infomanager.Version, "goInterface");
                }

                playerController.aiInvisiable = false;

                timer = 10f;
            });

            info.guideTxt = "变身躲避追踪者的发现，当时间结束后你会胜利";


            guidesHidder.Add(info);
        }

        #endregion

        //Choose GuideList
        if (this.GetComponent<GameStarter>().Game is BattleGame)
        {
            guideInfos = guidesShooter;
        }
        else
        {
            if(player.GetComponent<PlayerInput>().Team == 0)
            {
                guideInfos = guidesHidder;
            }
            else
            {
                guideInfos = guidesHunter;
            }
        }


        //init
        whenToOpenGuide = guideInfos.Find(g => g.id == currentGuideId).whenToOpen;
    }


    private void Update()
    {
        //---
        timer -= Time.deltaTime;
        //---
        frameCount++;
        //---

        //inst.whenToOpenGuide();

        if(frameCount % 5 == 0)
        {
            if (whenToOpenGuide != null && whenToOpenGuide())
            {
                Guide();
            }


            if (whenToCloseGuide != null && whenToCloseGuide())
            {
                HideGuideStuff();
                whenToCloseGuide = (() => { return false; });
            }
        }
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


        inst.ShowGuideStuff(info.guideTxt, info.fingerTarget, info.fingerMovement, -1f);

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


    private void ShowGuideStuff(string txt, RectTransform fingerTarget, Vector2 fingerMovement, float showTime = -1f)
    {
        if (isShowing) HideGuideStuff();

        isShowing = true;

        //show
        uicanvas.transform.Find("Guider").gameObject.SetActive(true);
        uicanvas.transform.Find("Guider").GetComponentInChildren<Text>().text = txt;


        //finger anim
        if(fingerTarget != null)
        {
            if (seq != null) seq.Kill();

            RectTransform finger = uicanvas.transform.Find("Finger").GetComponent<RectTransform>();

            finger.gameObject.SetActive(true);
            
            finger.position = fingerTarget.position;
            
            Vector2 anchorPosTmp = finger.anchoredPosition;
            
            //finger.DOAnchorPos(finger.anchoredPosition + fingerMovement, 1f).OnStart(() =>
            //{
            //    finger.GetChild(0).gameObject.SetActive(false);
            //    finger.GetChild(1).gameObject.SetActive(true);
            //}).OnComplete(() =>
            //{
            //    finger.GetChild(0).gameObject.SetActive(true);
            //    finger.GetChild(1).gameObject.SetActive(false);
            //}).SetLoops(-1);



            seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                finger.anchoredPosition = anchorPosTmp;
                finger.GetChild(0).gameObject.SetActive(true);
                finger.GetChild(1).gameObject.SetActive(false);
            }).AppendInterval(
                1f
            ).AppendCallback(() =>
            {
                finger.DOAnchorPos(anchorPosTmp + fingerMovement, 1f);
                finger.GetChild(0).gameObject.SetActive(false);
                finger.GetChild(1).gameObject.SetActive(true);
            }).AppendInterval(
                1.2f
            ).SetLoops(-1);

        }

        //auto hide
        if (showTime > 0f)
        {
            Invoke("HideGuideStuff", showTime);
        }
    }

    private void HideGuideStuff()
    {
        isShowing = false;

        if(currentGuideId - 1 > -1)
        {
            isGuided[currentGuideId - 1] = 2;
            inst.guideInfos.Find(guideInfos => guideInfos.id == currentGuideId - 1).closecallback(); 
        }

        //hide
        uicanvas.transform.Find("Guider").gameObject.SetActive(false);

        //fingetAnim
        uicanvas.transform.Find("Finger").GetComponent<RectTransform>().DOKill();
        uicanvas.transform.Find("Finger").gameObject.SetActive(false);


        //TimeScale
        //...
    }








    
    //private void OnGUI()
    //{
    //    string str = "GUIDE PROGRESS: ";
    //    for (int i = 0; i < isGuided.Length; i++)
    //    {
    //        str += isGuided[i].ToString() + ",";
    //    }
    //    GUILayout.TextArea(str);
    //}
}
