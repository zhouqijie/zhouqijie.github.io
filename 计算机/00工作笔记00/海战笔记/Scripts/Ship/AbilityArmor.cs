using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using DG.Tweening;

public class AbilityArmor : MonoBehaviour, Ability
{
    public int Index { get { return 2; } }

    public string Name { get { return "终极防御"; } }

    private IShipController controller;

    private float maxCD = 30f;
    private float remainTime;
    [HideInInspector] public float CD { get { return Mathf.Clamp01(remainTime / maxCD); } }



    public MonoBehaviour _This { get { return this as MonoBehaviour; } }

    private bool canUse = false;
    public bool CanUse { get { return this.canUse; } }



    // Start is called before the first frame update
    void Start()
    {
        this.controller = this.GetComponent<IShipController>();
    }

    void Update()
    {
        if (remainTime > 0f)
        {
            canUse = false;
            remainTime -= Time.deltaTime;
        }
        else
        {
            canUse = true;
        }

        if (canUse && (Index == 1 ? controller.Ability1Order : controller.Ability2Order))
        {
            StartCoroutine(CoDo());
        }
    }

    private IEnumerator CoDo()
    {
        remainTime = maxCD;

        //？？
        float tmp = this.GetComponent<Hull>().armorMultipiler;
        this.GetComponent<Hull>().armorMultipiler = tmp * 10f;

        this.transform.Find("SHELD").GetComponent<MeshRenderer>().material.DOFade(0.15f, 1f);

        yield return new WaitForSeconds(10f);

        this.GetComponent<Hull>().armorMultipiler = tmp;
        

        this.transform.Find("SHELD").GetComponent<MeshRenderer>().material.DOFade(0f, 1f);
    }
}
