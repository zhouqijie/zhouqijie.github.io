using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class AbilityMissile : MonoBehaviour, Ability
{
    public int Index { get { return 2; } }

    public string Name { get { return "反舰导弹"; } }

    private IShipController controller;

    private float maxCD = 30f;
    private float remainTime;
    [HideInInspector]public float CD { get { return Mathf.Clamp01(remainTime / maxCD); } }

    public GameObject[] launchers;



    public MonoBehaviour _This { get { return this as MonoBehaviour; } }


    private bool canUse = false;
    public bool CanUse { get { return this.canUse; } }






    private void Awake()
    {

    }

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
            if(controller.TargetLock != null)
            {
                canUse = true;
            }
            else
            {
                canUse = false;
            }
        }

        if (canUse && (Index == 1 ? controller.Ability1Order : controller.Ability2Order))
        {
            Do();
        }
    }

    void Do()
    {
        remainTime = maxCD;

        GameObject launcher = launchers.FirstOrDefault(l => Vector3.Dot(l.transform.forward, (controller.TargetLock.transform.position - l.transform.position)) > 0);
        if (launcher == null) launcher = launchers[0];

        GameObject missile = Instantiate(Resources.Load<GameObject>("Prefabs/Shells/Missile"), launcher.transform.position, launcher.transform.rotation, null);

        var hull = this.GetComponent<Hull>();
        var shipConfig = hull.shipConfig;
        missile.GetComponent<MissileNav>().damage = hull.shipPowerMultip * shipConfig.shipInfo.attack * Utils.CalFac(shipConfig.lvl) * 4f;

        missile.GetComponent<MissileNav>().piercingPercentage = 0f;
        missile.GetComponent<MissileNav>().shooter = this.GetComponent<IShipController>();
        missile.GetComponent<MissileNav>().targetObj = controller.TargetLock.GetComponent<IShipController>();

        SoundPlayer.PlaySound3D("导弹发射", this.transform.position);
    }
}
