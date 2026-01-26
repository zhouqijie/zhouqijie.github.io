using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

using DG.Tweening;

public class ImageEffects
{
    public static void ImageFly(RectTransform from, RectTransform to, string imageName, Vector2 size, Vector2 bias, float duration = 1f)
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        GameObject o = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ImageFly"), canvas.transform);
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + imageName);
        o.GetComponent<RectTransform>().sizeDelta = size;

        o.transform.position = from.transform.position;
        o.GetComponent<RectTransform>().anchoredPosition = o.GetComponent<RectTransform>().anchoredPosition + new Vector2(bias.x * Random.Range(0f, 1f), bias.y * Random.Range(0f, 1f));
        o.transform.DOMove(to.transform.position, duration).SetEase(Ease.Linear).OnComplete(() => { GameObject.Destroy(o); });
    }


    public static void ImageBoomAndFly(RectTransform from, RectTransform to, string imageName, Vector2 size, float radius, float duration = 1f)
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();
        GameObject o = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/ImageFly"), canvas.transform);
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/" + imageName);
        o.GetComponent<RectTransform>().sizeDelta = size;

        o.transform.position = from.transform.position;
        //o.transform.DOMove(to.transform.position, duration).SetEase(Ease.Linear).OnComplete(() => { GameObject.Destroy(o); });

        o.GetComponent<RectTransform>().DOAnchorPos(o.GetComponent<RectTransform>().anchoredPosition + new Vector2(radius * Random.Range(-1f, 1f), radius * Random.Range(-1f, 1f)), duration)
            .OnComplete(() => {
                o.GetComponent<RectTransform>().DOMove(to.transform.position, duration)
                    .OnComplete(() => { GameObject.Destroy(o); });
            });
    }
}
