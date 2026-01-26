using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScaner : MonoBehaviour
{
    private PlayerInput player;
    private CharactorController target = null;

    private RectTransform ind;



    private Vector2 scalerSize;







    void Start()
    {
        player = this.GetComponentInParent<UICanvas>().player;

        ind = this.transform.Find("Indicator").GetComponent<RectTransform>();

        scalerSize = this.GetComponentInParent<CanvasScaler>().referenceResolution;



        InvokeRepeating("RepeatingGetTarget", Random.Range(0.5f, 1f), 0.5f);
    }
    


    void Update()
    {
        if(target != null)
        {
            ind.anchoredPosition = Camera.main.WorldToViewportPoint(target.transform.position) * scalerSize;
        }
    }

    void RepeatingGetTarget()
    {
        float distance = player.Team == 0 ? 10f : 4f;

        var charas = FindObjectsOfType<CharactorController>().Where(c => c.gameInput.Team != player.Team && c.enabled).ToArray();


        target = null;

        foreach (var chara in charas)
        {
            if((chara.transform.position - player.transform.position).sqrMagnitude < Mathf.Pow(distance, 2f))
            {
                target = chara;
            }
        }
        

        //ind set active
        if (target != null)
        {
            ind.gameObject.SetActive(true);
        }
        else
        {
            ind.gameObject.SetActive(false);
        }
    }
}
