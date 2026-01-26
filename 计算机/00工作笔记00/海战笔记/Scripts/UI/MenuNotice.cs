using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

public class MenuNotice : MonoBehaviour
{
    private bool isIdle;

    private static MenuNotice inst;

    private GameObject noticePrefab;

    private Queue<GameObject> noticeToShow;

    private Queue<GameObject> existNotices;

    private void Awake()
    {
        inst = this;
        noticePrefab = this.transform.GetChild(0).gameObject;

        noticeToShow = new Queue<GameObject>();
        existNotices = new Queue<GameObject>();

        isIdle = true;
    }



    void Start()
    {
        
    }




    void Update()
    {
        if(noticeToShow.Count > 0 && isIdle)
        {
            isIdle = false;

            GameObject o = noticeToShow.Dequeue();
            o.SetActive(true);

            o.GetComponent<RectTransform>().anchoredPosition = new Vector2(350, 0);
            o.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.5f).OnComplete(() => {
                isIdle = true;
                existNotices.Enqueue(o);

                //fade out
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(2f).AppendCallback(() => {
                    o.GetComponent<Image>().DOFade(0f, 1f);
                    o.transform.Find("Image").GetComponent<Image>().DOFade(0f, 0.45f);
                    o.transform.Find("Text").GetComponent<Text>().DOFade(0f, 0.45f);
                });
            });

            foreach(var item in existNotices)
            {
                item.GetComponent<RectTransform>().DOAnchorPos(item.GetComponent<RectTransform>().anchoredPosition + new Vector2(0, 80), 0.45f);
            }
        }

        if(existNotices.Count > 5)
        {
            Destroy(existNotices.Dequeue());
        }
    }

    public static void Notice(string imageName, string txt, Color bgColor)
    {
        Color bgClrTrue = bgColor * new Color(0.5f, 0.5f, 0.5f, 1f);

        GameObject o = Instantiate(inst.noticePrefab, inst.transform);
        o.GetComponent<Image>().color = bgClrTrue;
        o.transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Item/" + imageName);
        o.transform.GetComponentInChildren<Text>().text = "<b>" + txt + "</b>";
        inst.noticeToShow.Enqueue(o);
    }
}
