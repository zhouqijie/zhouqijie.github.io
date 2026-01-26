using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class BillboardText : MonoBehaviour
{
    [HideInInspector] public float distance;
    private float fovFac = 0.5f;
    // Start is called before the first frame update
    void Awake()
    {
        if(Camera.current != null)
        {
            distance = (Camera.current.transform.position - this.transform.position).magnitude;
            fovFac = Camera.current.fieldOfView / 60f;
            this.transform.rotation = Camera.current.transform.rotation;
            this.transform.localScale = Vector3.one * distance * fovFac * 0.01f;
        }
    }

    // Update is called once per frame
    void Update()
    {

        //if (Camera.current != null)
        //{
        //    distance = (Camera.current.transform.position - this.transform.position).magnitude;

        //    this.transform.rotation = Camera.current.transform.rotation;
        //    this.transform.localScale =  Vector3.one * distance * 0.01f;
        //}
    }



    public static void ShowNumber(int number, Vector3 pos, Color color)
    {
        GameObject o = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/DamageText"), pos, new Quaternion(), null);
        var btext = o.GetComponent<BillboardText>();
        o.transform.DOMoveY(btext.distance * btext.fovFac * 0.15f, 3f);

        string str = number.ToString();

        for(int i = 0; i < str.Length; i++)
        {
            GameObject sprite;
            if (i == 0)
            {
                sprite = o.transform.GetChild(0).gameObject;
            }
            else
            {
                sprite = Instantiate(o.transform.GetChild(0).gameObject, o.transform);
            }

            sprite.transform.localPosition = new Vector3(i * 5, 0, 0);
            sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Numbers/" + str[i].ToString());
            sprite.GetComponent<SpriteRenderer>().color = color;
            sprite.GetComponent<SpriteRenderer>().DOFade(0f, 3f);
        }

        Destroy(o, 3.5f);
    }

    public static float ShowNumber(int number, Hull hull, int posOffset, Color color, float facIn = -1f)
    {

        GameObject o = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/DamageText"), hull.transform.position, new Quaternion(), null);
        
        float fac;

        if (facIn > 0f)
        {
            fac = facIn;
        }
        else
        {
            var btext = o.GetComponent<BillboardText>();
            fac = btext.distance * btext.fovFac;
        }
         
        o.transform.position += new Vector3(0f, (posOffset + 0.5f) * fac * 0.045f, 0f);
        o.transform.DOMoveY(o.transform.position.y + fac * 0.15f, 3f);

        string str = number.ToString();

        for (int i = 0; i < str.Length; i++)
        {
            GameObject sprite;
            if (i == 0)
            {
                sprite = o.transform.GetChild(0).gameObject;
            }
            else
            {
                sprite = Instantiate(o.transform.GetChild(0).gameObject, o.transform);
            }

            sprite.transform.localPosition = new Vector3(i * 5, 0, 0);
            sprite.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Numbers/" + str[i].ToString());
            sprite.GetComponent<SpriteRenderer>().color = color;
            sprite.GetComponent<SpriteRenderer>().DOFade(0f, 3f);
        }

        Destroy(o, 3.5f);

        return fac;
    }
}
