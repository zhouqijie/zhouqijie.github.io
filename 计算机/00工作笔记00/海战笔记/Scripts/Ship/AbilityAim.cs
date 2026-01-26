using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityAim : MonoBehaviour, Ability
{
    public int Index { get { return 2; } }

    public string Name { get { return "精确射击"; } }

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
        float tmp = this.GetComponent<Hull>().accuracyMultipiler;
        this.GetComponent<Hull>().accuracyMultipiler = tmp * 4f;

        yield return new WaitUntil(() => { return this.GetComponentsInChildren<Turret>().Where(t => t.isLoaded && t.isRightAngle).ToArray().Length > 0 && this.GetComponent<IShipController>().PrimaryFiringOrder == true; });

        yield return new WaitForSeconds(1f);

        this.GetComponent<Hull>().accuracyMultipiler = tmp; // * 0.1f??
    }
}
