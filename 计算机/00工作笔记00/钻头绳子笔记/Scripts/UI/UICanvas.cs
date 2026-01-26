using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using DG.Tweening;

public class UICanvas : MonoBehaviour
{
    private Game game;
    private PlayerController player;

    private Transform windowStart;
    private Transform windowWin;
    private Transform windowLose;
    private Transform windowShare;
    private Transform windowLuck;
    private Transform windowGot;

    private Transform windowPlaying;

    private Image imageOverHeat;




    //-----over heat-----
    private DrillController drillCtrl;
    private bool isDisplayingOverHeat = false;
    private float overHeatValue = 0.75f;






    //----LUCK Stuff-----
    private int[] bFliped = new int[] { 0, 0, 0 };
    private int[] bGot = new int[] { 0, 0, 0 };
    private int[] bFree = new int[] { 0, 0, 0 };

    private int[] bType = new int[] { 0, 0, 0 };
    private string[] bName = new string[] { "money", "money", "money" };
    private int[] bNum = new int[] { 0, 0, 0 };



    private void Awake()
    {
        windowStart = this.transform.Find("Windows").Find("WindowStart");
        windowWin = this.transform.Find("Windows").Find("WindowWin");
        windowLose = this.transform.Find("Windows").Find("WindowLose");
        windowShare = this.transform.Find("Windows").Find("WindowShare");
        windowLuck = this.transform.Find("Windows").Find("WindowLuck");
        windowGot = this.transform.Find("Windows").Find("WindowGot");

        windowPlaying = this.transform.Find("Playing");

        imageOverHeat = windowPlaying.Find<Image>("ImageOverHeat");


    }
    void Start()
    {
        game = FindObjectOfType<Game>();
        //player
        player = FindObjectOfType<PlayerController>();
        //drill
        drillCtrl = player.GetComponentInChildren<DrillController>();

        //set
        SetWindowPlaying();
    }




    void Update()
    {
        //drill over heat display
        if ((1f - drillCtrl.HeatPercentage) > overHeatValue && !isDisplayingOverHeat)
        {
            EnableHeatImage();
            isDisplayingOverHeat = true;
        }
        if ((1f - drillCtrl.HeatPercentage) < overHeatValue && isDisplayingOverHeat)
        {
            DisableHeatImage();
            isDisplayingOverHeat = false;
        }
    }



    //-----------------------------------------Show Hide Window--------------------------------------------

    public void SetWindowPlaying()
    {
        windowPlaying.Find("PanelLevel").GetComponentInChildren<Text>().text = "<b>关卡：" + Infomanager.Instance.userData.currentLevel.ToString() + "</b>";
    }

    /// <summary>
    /// 开始
    /// </summary>
    /// <param name="show"></param>
    public void ShowWindowStart(bool show)
    {
        if (show)
        {

            windowStart.gameObject.SetActive(true);
            windowStart.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            windowStart.transform.DOScale(Vector3.one, 0.5f);

            //Random Try
            int hookIdx = Infomanager.Instance.userData.activeHook;
            int drillIdx = Infomanager.Instance.userData.activeDrill;
            
            bool drillOrHook = Random.value < 0.5f;
            if (drillOrHook)
            {
                DrillInfo[] drillsNotHave = Infomanager.Instance.drillInfos.Where(d => !Infomanager.Instance.userData.myDrills.Contains(d.id)).ToArray();
                if (drillsNotHave.Length > 0)
                {
                    drillIdx = drillsNotHave[Random.Range(0, drillsNotHave.Length)].id;
                }
            }
            else
            {
                HookInfo[] hooksNotHave = Infomanager.Instance.hookInfos.Where(d => !Infomanager.Instance.userData.myHooks.Contains(d.id)).ToArray();
                if (hooksNotHave.Length > 0)
                {
                    hookIdx = hooksNotHave[Random.Range(0, hooksNotHave.Length)].id;
                }
            }

            //Image
            if (drillOrHook)
            {
                windowStart.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Drills/" + drillIdx);

            }
            else
            {
                windowStart.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>("Sprites/Hooks/" + hookIdx);
            }


            //Btn
            windowStart.transform.Find("ButtonStart1").GetComponent<Button>().onClick.RemoveAllListeners();
            windowStart.transform.Find("ButtonStart1").GetComponent<Button>().onClick.AddListener(() =>
            {
                //SOUND
                SoundPlayer.PlaySound2D("click");

                Game.Instance.StartGame2();
                ShowWindowStart(false);


                //guides
                StartCoroutine(CoShowHideGuides());
            });
            windowStart.transform.Find("ButtonStart2").GetComponent<Button>().onClick.RemoveAllListeners();
            windowStart.transform.Find("ButtonStart2").GetComponent<Button>().onClick.AddListener(() =>
            {
                AdManager.VideoAd(() => {

                    //SOUND
                    SoundPlayer.PlaySound2D("click");

                    //Switch Weapon
                    player.ResetHooksAndDrills(hookIdx, drillIdx);

                    Game.Instance.StartGame2();
                    ShowWindowStart(false);


                    //guides
                    StartCoroutine(CoShowHideGuides());

                }, () => { });
            });
        }
        else
        {
            windowStart.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 指引
    /// </summary>
    public IEnumerator CoShowHideGuides()
    {
        if (Infomanager.Instance.userData.todayLevels > 0) yield break;
        


        this.transform.Find("Guides").gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        foreach (var img in this.transform.Find("Guides").GetComponentsInChildren<Image>())
        {
            img.DOFade(0f, 2f);
        }

        yield return new WaitForSeconds(2.5f);

        this.transform.Find("Guides").gameObject.SetActive(false);
    }

    /// <summary>
    /// 胜利
    /// </summary>
    /// <param name="show"></param>
    public void ShowWindowWin(bool show)
    {
        if (show)
        {
            //AD
            AdManager.InterstitialOrBannerAd(null);

            //SOUND
            SoundPlayer.PlaySound2D("win");

            windowWin.gameObject.SetActive(true);
            windowWin.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            windowWin.transform.DOScale(Vector3.one, 0.5f);

            //..do
            StartCoroutine(CoSetWindowWin());
        }
        else
        {
            windowWin.gameObject.SetActive(false);
        }
    }
    public IEnumerator CoSetWindowWin()
    {
        //coin count 
        windowWin.Find<Text>("TextMoney").text = "<b>" + PlayerController.moneySum.ToString() + "</b>";

        yield return null;

        ShowHideWindowShare(true);

        yield return new WaitUntil(() => !windowShare.gameObject.activeInHierarchy);

        ShowHideWindowLuck(true);

        yield return new WaitUntil(() => !windowLuck.gameObject.activeInHierarchy);


        windowWin.Find<Image>("ImageProgress").fillAmount = (float)(PlayerController.lvlSum - 1) / 3f;
        windowWin.Find<Image>("ImageProgress").DOFillAmount(((float)(PlayerController.lvlSum)  / 3f), 0.6f);

        if (PlayerController.lvlSum >= 3)
        {
            PlayerController.lvlSum = 0;

            bool canget;
            bool isDrill;
            int idx;
            Game.RandomEquipNotHave(out canget, out isDrill, out idx);

            if (canget)
            {
                if (isDrill && !Infomanager.Instance.userData.myDrills.Contains(idx))
                {
                    Infomanager.Instance.userData.myDrills.Add(idx);
                    //show....
                    ShowHideWindowGot(true, "Sprites/Drills/" + idx);
                }
                if (!isDrill && !Infomanager.Instance.userData.myHooks.Contains(idx))
                {
                    Infomanager.Instance.userData.myHooks.Add(idx);
                    //show....
                    ShowHideWindowGot(true, "Sprites/Hooks/" + idx);
                }
            }
        }


        yield return new WaitUntil(() => !windowGot.gameObject.activeInHierarchy);

        //Btn
        windowWin.transform.Find("ButtonNext1").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWin.transform.Find("ButtonNext1").GetComponent<Button>().onClick.AddListener(() =>
        {
            //disable
            windowWin.transform.Find("ButtonNext1").GetComponent<Button>().onClick.RemoveAllListeners();
            windowWin.transform.Find("ButtonNext2").GetComponent<Button>().onClick.RemoveAllListeners();


            //SOUND
            SoundPlayer.PlaySound2D("click");

            //get coint
            Infomanager.Instance.userData.money += PlayerController.moneySum;

            //effects
            Effects.ImageBoom("Sprites/Items/coin");

            //next
            StartCoroutine(CoNextLevel());
        });

        windowWin.transform.Find("ButtonNext2").GetComponent<Button>().onClick.RemoveAllListeners();
        windowWin.transform.Find("ButtonNext2").GetComponent<Button>().onClick.AddListener(() =>
        {
            AdManager.VideoAd(() => {

                //disable
                windowWin.transform.Find("ButtonNext1").GetComponent<Button>().onClick.RemoveAllListeners();
                windowWin.transform.Find("ButtonNext2").GetComponent<Button>().onClick.RemoveAllListeners();

                //SOUND
                SoundPlayer.PlaySound2D("click");


                //get coint
                int multip = GetMultp(windowWin.GetComponentInChildren<UICursorRandom>().CurrentAngle); Debug.Log("加成倍数：" + multip.ToString("f2"));
                Infomanager.Instance.userData.money += (PlayerController.moneySum * multip);

                //effects
                Effects.ImageBoom("Sprites/Items/coin");

                //next
                StartCoroutine(CoNextLevel());

            }, () => { });

        });

    }
    public IEnumerator CoNextLevel()
    {
        yield return new WaitForSeconds(1.0f);

        NextLevel();
    }
    public static void NextLevel()
    {
        Game.LoadNextLevel();
    }
    public static int GetMultp(float angle)
    {
        //26 // 14/ 3.5
        if (angle < -26f)
        {
            return 2;
        }
        else if (angle < -14f)
        {
            return 3;
        }
        else if (angle < -3.5f)
        {
            return 4;
        }
        else if (angle < 3.5f)
        {
            return 5;
        }
        else if (angle < 14f)
        {
            return 4;
        }
        else if (angle < 26f)
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    /// <param name="show"></param>
    public void ShowWindowLose(bool show)
    {
        if (show)
        {
            //AD
            AdManager.InterstitialOrBannerAd(null);

            //SOUND
            SoundPlayer.PlaySound2D("lose");

            windowLose.gameObject.SetActive(true);
            windowLose.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            windowLose.transform.DOScale(Vector3.one, 0.5f);

            //CoinCount


            //Btn
            windowLose.transform.Find("ButtonContinue").GetComponent<Button>().onClick.RemoveAllListeners();
            windowLose.transform.Find("ButtonContinue").GetComponent<Button>().onClick.AddListener(() =>
            {
                AdManager.VideoAd(() => {
                    game.Continue();
                    ShowWindowLose(false);
                }, () => {
                    Displayer.Display("暂无广告", 48, 3f);
                });
            });
        }
        else
        {
            windowLose.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 分享
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowShare(bool show)
    {
        if (show)
        {
            windowShare.gameObject.PopActive(true);

            windowShare.Find<Button>("ButtonShare").onClick.RemoveAllListeners();
            windowShare.Find<Button>("ButtonShare").onClick.AddListener(() => {

                //SOUND
                SoundPlayer.PlaySound2D("click");

                Recorder.ShareVideo(() => {

                    //Bonus
                    Infomanager.Instance.userData.money += 200;

                    //Effect
                    Effects.ImageBoom("Sprites/Items/coin");

                    //events
                    FindObjectOfType<PlayerController>().onShare.Invoke();

                    //showhide
                    ShowHideWindowShare(false);

                }, (str) => { }, () => { });

            });
        }
        else
        {
            windowShare.gameObject.PopActive(false);
        }


    }

    /// <summary>
    /// 获取
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowGot(bool show)
    {
        if (!show)
        {
            ShowHideWindowGot(false, "");
        }
    }
    public  void ShowHideWindowGot(bool show, string path)
    {
        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {
            windowGot.gameObject.PopActive(true);

            //Image
            windowGot.Find<Image>("ImageItem").sprite = Resources.Load<Sprite>(path);
        }
        else
        {
            windowGot.gameObject.PopActive(false);
        }
    }

    /// <summary>
    /// 幸运抽卡
    /// </summary>
    /// <param name="show"></param>
    public void ShowHideWindowLuck(bool show)
    {
        //AD
        AdManager.InterstitialOrBannerAd(null);

        //SOUND
        SoundPlayer.PlaySound2D("click");

        if (show)
        {
            windowLuck.gameObject.PopActive(true);

            //refresh
            RefreshWindowLuck();
        }
        else
        {
            windowLuck.gameObject.PopActive(false);
        }
    }
    public void RefreshWindowLuck()
    {
        //bonuses
        for (int i = 0; i < 3; i++)
        {
            int iTmp = i;

            if (bFliped[i] == 0)
            {
                windowLuck.Find("CardContainer").GetChild(i).Find("ImagePleaseSelect").gameObject.SetActive(true);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageGolden").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet1").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageGot").gameObject.SetActive(false);


                windowLuck.Find("CardContainer").GetChild(i).Find("ImagePleaseSelect").GetComponent<Button>().onClick.RemoveAllListeners();
                windowLuck.Find("CardContainer").GetChild(i).Find("ImagePleaseSelect").GetComponent<Button>().onClick.AddListener(() => {

                    SetRandomBonus(iTmp);

                    bFliped[iTmp] = 1;

                    for (int k = 0; k < 3; k++)
                    {
                        if(k != iTmp)
                        {
                            SetRandomBonus(k);

                            bFliped[k] = 1;
                        }
                    }

                    RefreshWindowLuck();
                });
            }
            else
            {
                windowLuck.Find("CardContainer").GetChild(i).Find("ImagePleaseSelect").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageGolden").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet1").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet2").gameObject.SetActive(false);
                windowLuck.Find("CardContainer").GetChild(i).Find("ImageGot").gameObject.SetActive(false);
                

                //Images
                if(bType[i] == 0)
                {
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").gameObject.SetActive(true);
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Items/coins2");
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").GetComponent<Image>().SetNativeSize();

                    windowLuck.Find("CardContainer").GetChild(i).Find("TextItem").GetComponent<Text>().text = "<b>" + bNum[iTmp].ToString() + "</b>";
                }
                else if(bType[i] == 1)
                {
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageGolden").gameObject.SetActive(true);
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").gameObject.SetActive(true);
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Drills/" + bNum[iTmp]);

                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageHalo").gameObject.SetActive(true);
                }
                else if (bType[i] == 2)
                {
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageGolden").gameObject.SetActive(true);
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").gameObject.SetActive(true);
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageItem").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Hooks/" + bNum[iTmp]);

                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageHalo").gameObject.SetActive(true);
                }

                //BTNs
                if (bGot[i] == 0) // 未领取
                {
                    if (bFree[i] == 1)
                    {
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet1").gameObject.SetActive(true);
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.RemoveAllListeners();
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet1").GetComponent<Button>().onClick.AddListener(() => {
                                GetLuckBonus(iTmp);
                                RefreshWindowLuck();
                        });
                    }
                    else
                    {
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet2").gameObject.SetActive(true);
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.RemoveAllListeners();
                        windowLuck.Find("CardContainer").GetChild(i).Find("ButtonGet2").GetComponent<Button>().onClick.AddListener(() => {
                            AdManager.VideoAd(() => {
                                GetLuckBonus(iTmp);
                                RefreshWindowLuck();
                            }, () => { });
                        });
                    }
                }
                else // 已经领取
                {
                    windowLuck.Find("CardContainer").GetChild(i).Find("ImageGot").gameObject.SetActive(true);
                }
            }
        }
    }
    public void SetRandomBonus(int id)
    {
        if (id > 2) return;
        if (id < 0) return;

        if(bFliped.Count(f => f > 0) == 0)
        {
            bFree[id] = 1;
            bType[id] = 0;
            bNum[id] = Random.Range(1, 3) * 100;
        }
        else if(bFliped.Count(f => f > 0) == 1)
        {
            bType[id] = 0;
            bNum[id] = Random.Range(2, 4) * 100;
        }
        else
        {
            bType[id] = Random.Range(1, 3);
            bNum[id] = Random.Range(1, 9);
        }




    }
    public void GetLuckBonus(int id)
    {
        if (id > 2) return;
        if (id < 0) return;

        ///...
        switch (bType[id])
        {
            case 0:
                {
                    Infomanager.Instance.userData.money += bNum[id];
                }
                break;
            case 1:
                {
                    if (!Infomanager.Instance.userData.myDrills.Contains(bNum[id])) Infomanager.Instance.userData.myDrills.Add(bNum[id]);
                }
                break;
            case 2:
                {
                    if (!Infomanager.Instance.userData.myHooks.Contains(bNum[id])) Infomanager.Instance.userData.myHooks.Add(bNum[id]);
                }
                break;
            default:
                break;
        }

        bGot[id] = 1;
    }

    //------------------------------------------ BUTTON  ----------------------------------
    public void ButtonReturn()
    {
        Game.ReturnMainMenu();
    }


    //------------------------------Utils---------------------------

    public void EnableHeatImage()
    {
        imageOverHeat.gameObject.SetActive(true);
    }
    public void DisableHeatImage()
    {
        imageOverHeat.gameObject.SetActive(false);
    }
}
