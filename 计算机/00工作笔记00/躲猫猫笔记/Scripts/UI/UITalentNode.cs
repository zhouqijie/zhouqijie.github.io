using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using DG.Tweening;


public class UITalentNode : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{

    [HideInInspector] public int id;

    [HideInInspector] public bool available = false;

    [HideInInspector] public bool selected = false;

    [HideInInspector] public bool unlocked = false;
    private bool isUnlocking = false; public bool IsUnLocking { get { return this.isUnlocking; } }


    //AUTO
    private bool isThisFree = false;
    private bool isAutoFilling = false;


    private float progress;
    private int fillMoney;
    private Image imgBottom;
    private Image imgFill;


    private TalentNodeInfo talent; public TalentNodeInfo TalentInfo { get { return this.talent; } }
    private MainMenuCanvas mCanvas;


    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAutoFilling) return;

        if (isUnlocking) return;


        if (!selected)
        {
            FindObjectOfType<MainMenuCanvas>().RefreshTalentSelect(this);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isAutoFilling) return;


        if (!unlocked && available && selected)   //不必剔除isUnlocking情况来防止重复灌注不同节点。因为此时不能选中其他节点。
            isUnlocking = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isAutoFilling) return;


        if (!unlocked && available && selected)
        {
            isUnlocking = false;
            progress = 0f;

            //effect
            for (int i = 0; i < 10; i++)
            {
                ImageEffects.ImageFly(this.GetComponent<RectTransform>(), mCanvas.userAssetsPanel.Find("ImageCoin").GetComponent<RectTransform>(), "Items/Coin", new Vector2(50, 50), new Vector2(50, 50), 0.4f);
            }
            //sound coinsback
            SoundPlayer.PlaySound2D("coins");

            ResetMoneyDisplay();
        }
    }




    private void Awake()
    {
        imgBottom = this.GetComponent<Image>();
        imgFill = this.transform.Find("Fill").GetComponent<Image>();
    }

    private void Start()
    {
        //images
        this.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Talents/" + this.id + "b");
        this.imgFill.sprite = Resources.Load<Sprite>("Sprites/Talents/" + this.id + "a");

        //..
        mCanvas = FindObjectOfType<MainMenuCanvas>();
        talent = Infomanager.GetInstance().userData.activeSkin.charaInfo.talentNodes.FirstOrDefault(t => t.id == this.id);

        //set unlock or not effect...
        if (unlocked)
        {
            ShowAsUnlocked(false);
        }
        else
        {
            if (available)
            {
                ShowAsAvailable(false);
            }
            else
            {
                ShowAsUnAvailable(false);
            }
        }
    }

    private void Update()
    {
        if (isUnlocking)
        {
            if (!isThisFree) //----------------不免费--------------
            {
                //填充金币数量
                fillMoney = Mathf.RoundToInt(progress * talent.price);
                //显示金币
                mCanvas.userAssetsPanel.Find("TextMoney").GetComponent<Text>().text = Mathf.Clamp(Infomanager.GetInstance().userData.money - fillMoney, 0, int.MaxValue).ToString();
                //可以继续填充
                if (Infomanager.GetInstance().userData.money - fillMoney >= 0)
                {
                    progress = Mathf.Clamp01(progress + 0.005f);

                    //特效
                    if (progress % 0.1f < 0.005f)
                    {
                        ImageEffects.ImageFly(mCanvas.userAssetsPanel.Find("ImageCoin").GetComponent<RectTransform>(), this.GetComponent<RectTransform>(), "Items/Coin", new Vector2(50, 50), new Vector2(0, 0), 0.2f);

                        SoundPlayer.PlaySound2D(SoundPlayer.collectionCoin[Random.Range(0, SoundPlayer.collectionCoin.Length)]);
                    }
                }

                if (fillMoney >= talent.price && Infomanager.GetInstance().userData.money >= talent.price)
                {
                    UnLock();
                }
            }
            else //--------------------免费-------------------
            {
                fillMoney = 0;

                progress = Mathf.Clamp01(progress + 0.005f);

                if (progress >= 1f)
                {
                    UnLock();
                }
            }
        }



        imgFill.fillAmount = progress;
    }


    public void AutoFill(bool free = false)
    {
        if (isUnlocking) return; //不能两个同时灌注

        if (free)
        {
            isThisFree = true;
        }
        else
        {
            if (!mCanvas.MoneyEnough(talent.price)) return;
        }
        

        StartCoroutine(CoAutoFill());
    }
    private IEnumerator CoAutoFill()
    {
        isAutoFilling = true;

        isUnlocking = true;

        yield return new WaitUntil(() => progress >= 1f);

        isAutoFilling = false;
    }





    public void ResetMoneyDisplay()
    {
        mCanvas.userAssetsPanel.Find("TextMoney").GetComponent<Text>().text = Infomanager.GetInstance().userData.money.ToString();
    }


    /// <summary>
    ///开关选择框 
    /// </summary>
    public void ShowSelectedOtNot()
    {
        this.transform.Find("Selected").gameObject.SetActive(this.selected);
    }

    /// <summary>
    /// 显示为已解锁
    /// </summary>
    public void ShowAsUnlocked(bool anim = false)
    {
        this.imgBottom.sprite = Resources.Load<Sprite>("Sprites/Talents/" + this.id + "a");
        if (anim)
        {
            this.GetComponent<RectTransform>().DOScale(1.2f, 0.2f).OnComplete(() => { this.GetComponent<RectTransform>().DOScale(1f, 0.2f); });
        }
    }
    /// <summary>
    /// 显示为可用
    /// </summary>
    /// <param name="anim"></param>
    public void ShowAsAvailable(bool anim = false)
    {
        this.imgBottom.sprite = Resources.Load<Sprite>("Sprites/Talents/" + this.id + "a");
        this.imgBottom.color = Color.gray;
        if (anim)
        {
            this.GetComponent<RectTransform>().DOScale(1.2f, 0.2f).OnComplete(() => { this.GetComponent<RectTransform>().DOScale(1f, 0.2f); });
        }
    }
    /// <summary>
    /// 显示为不可用
    /// </summary>
    /// <param name="anim"></param>
    public void ShowAsUnAvailable(bool anim = false)
    {
        this.imgBottom.sprite = Resources.Load<Sprite>("Sprites/Talents/" + this.id + "b");
        this.imgBottom.color = Color.white;
        if (anim)
        {
            this.GetComponent<RectTransform>().DOScale(1.2f, 0.2f).OnComplete(() => { this.GetComponent<RectTransform>().DOScale(1f, 0.2f); });
        }
    }

    /// <summary>
    /// 设置为可用
    /// </summary>
    public void SetAvailable()
    {
        if (!available)
        {
            ShowAsAvailable(true);
        }

        this.available = true;
    }

    /// <summary>
    /// 解锁
    /// </summary>
    private void UnLock()
    {
        isUnlocking = false;
        unlocked = true;
        progress = 1f;
        //isAutoFilling = false;  set false in CoAutoFill coroutine

        //效果
        ShowAsUnlocked(true);

        //音效
        SoundPlayer.PlaySound2D("unlock");


        //扣除金币/技能点
        if (!isThisFree)
        {
            //不免费则扣除金币并刷新
            Infomanager.GetInstance().userData.money -= talent.price;
            ResetMoneyDisplay();
        }

        isThisFree = false;
        //...


        //获取要解锁的职业信息
        CharactorInstance chara = Infomanager.GetInstance().userData.charactors.FirstOrDefault(c => c.info == Infomanager.GetInstance().userData.activeSkin.charaInfo);
        //解锁
        if(chara != null)
        {
            if (!chara.unlockedNodes.Contains(this.id))
            {
                chara.unlockedNodes = chara.unlockedNodes.Append(this.id).ToArray();
            }
        }


        //刷新所有节点的available
        mCanvas.RefreshTalentNodesAvailable();

        //刷新选择
        mCanvas.RefreshTalentSelect(this);
    }

}
