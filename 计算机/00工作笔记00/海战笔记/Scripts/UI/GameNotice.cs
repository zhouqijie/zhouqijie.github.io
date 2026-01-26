using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;


public class GameNotice : MonoBehaviour
{
    private bool isIdle;

    private static GameNotice inst;

    private GameObject noticePrefab;

    private GameObject noticeCenter;

    private Queue<GameObject> noticesToShow;

    private Queue<GameObject> existNotices;

    private int[] groupCounter;




    private void Awake()
    {
        inst = this;
        noticePrefab = this.transform.GetChild(0).gameObject;

        noticeCenter = this.GetComponentInParent<UICanvas>().transform.Find("NOTICE_CENTER").gameObject;

        noticesToShow = new Queue<GameObject>();
        existNotices = new Queue<GameObject>();

        isIdle = true;

        groupCounter = new int[10];
    }



    void Start()
    {

    }




    void Update()
    {
        if (noticesToShow.Count > 0 && isIdle)
        {
            isIdle = false;

            GameObject o = noticesToShow.Peek();
            o.SetActive(true);

            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, 0);
            o.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f).OnComplete(() => {
                isIdle = true;
                noticesToShow.Dequeue();
                existNotices.Enqueue(o);

                //fade out
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(2f).AppendCallback(() => {
                    groupCounter[o.GetComponent<NoticeGroup>().group] = 0;
                    o.GetComponent<NoticeGroup>().group = 0;
                    o.transform.Find("ImageBottom").GetComponent<Image>().DOFade(0f, 1f);
                    o.transform.Find("ImageIcon").GetComponent<Image>().DOFade(0f, 0.45f);
                    o.transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.45f);
                });
            });

            foreach (var item in existNotices)
            {
                item.GetComponent<RectTransform>().DOAnchorPos(item.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), 0.45f);
            }
        }

        if (existNotices.Count > 5)
        {
            Destroy(existNotices.Dequeue());
        }
    }

    public static void Notice(Color color, string imageName, string txt, int group = 0)
    {
        inst.groupCounter[group]++;

        if (group != 0)
        {
            foreach (var notice in inst.noticesToShow)
            {
                if (notice.GetComponent<NoticeGroup>().group == group)
                {
                    notice.GetComponentInChildren<Text>().text = "<b>" + txt + inst.groupCounter[group].ToString() + "</b>";//group COunt
                    return;
                }
            }
            foreach (var notice in inst.existNotices)
            {
                if (notice.GetComponent<NoticeGroup>().group == group)
                {
                    notice.GetComponentInChildren<Text>().text = "<b>" + txt + inst.groupCounter[group].ToString() + "</b>";//group COunt
                    return;
                }
            }
        }

        GameObject o = Instantiate(inst.noticePrefab, inst.transform);

        o.GetComponent<NoticeGroup>().group = group;

        o.transform.Find("ImageBottom").GetComponent<Image>().color = color;
        o.transform.Find("ImageIcon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + imageName);
        o.transform.GetComponentInChildren<Text>().text = "<b>" + txt + inst.groupCounter[group].ToString() + "</b>";//group COunt
        inst.noticesToShow.Enqueue(o);
    }



    public static void NoticeCenter(string imageName)
    {
        inst.noticeCenter.GetComponent<Image>().DOComplete();
        inst.noticeCenter.transform.GetChild(0).GetComponent<Image>().DOComplete();

        inst.noticeCenter.SetActive(true);

        inst.noticeCenter.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + imageName);
        inst.noticeCenter.GetComponent<Image>().color = Color.white;
        inst.noticeCenter.GetComponent<Image>().DOFade(0f, 4f).OnComplete(() => {
            inst.noticeCenter.SetActive(false);
        });



        inst.noticeCenter.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + imageName + "2");
        inst.noticeCenter.transform.GetChild(0).GetComponent<Image>().color = Color.white;
        inst.noticeCenter.transform.GetChild(0).GetComponent<Image>().DOFade(0f, 4f);
    }
}
