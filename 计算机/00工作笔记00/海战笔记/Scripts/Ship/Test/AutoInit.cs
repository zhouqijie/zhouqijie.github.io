using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInit : MonoBehaviour
{
    
    private void Awake()
    {

        //-------

        var player = GameObject.FindGameObjectWithTag("Player");
        player.gameObject.AddComponent<PlayerController>();

        ShipConfig playerShipConfig = InfoManager.GetInstance().userData.currentShip;
        player.GetComponent<Hull>().shipConfig = playerShipConfig;

        //AutoInit
        Hull hull = player.GetComponent<Hull>();
        hull.shipPowerMultip = 1f;


        //---设置属性---

        //初始化血量
        //Debug.Log("Cal(1):" + Utils.CalFac(1));
        hull.MaxHP = hull.shipConfig.shipInfo.hp * Utils.CalFac(hull.shipConfig.lvl); //if (ship is AIController) { Debug.Log("战舰裸血量: " + hull.MaxHP + "系数: " + Utils.CalFac(hull.shipConfig.lvl)); }
        if (hull.shipConfig.activeArmorCom != null)
        {
            hull.MaxHP += hull.shipConfig.activeArmorCom.armorInfo.hp * Utils.CalFac(hull.shipConfig.activeArmorCom.lvl); //if (ship is AIController) { Debug.Log("装甲血量加成后：" + hull.MaxHP); }
        }
        hull.HP = hull.MaxHP;

        //初始化装甲
        ShipConfig shipConfig = hull.shipConfig;
        ShipArmorCom armorCom = hull.shipConfig.activeArmorCom;
        float armor = shipConfig.shipInfo.defense * Utils.CalFac(shipConfig.lvl);
        if (armorCom != null)
        {
            armor += armorCom.armorInfo.defense * Utils.CalFac(armorCom.lvl);
        }
        hull.Armor = armor;

        //初始化炮塔
        foreach (var item in hull.GetComponentsInChildren<Turret>())
        {
            item.TurretInit(hull.shipPowerMultip);
        }

        //初始化引擎
        player.GetComponent<Engine>().thrustPower = player.GetComponent<Hull>().shipConfig.shipInfo.speed / 4f;
        player.GetComponent<Engine>().thrustPower *= 1f + (hull.shipConfig.activeEngineCom.engineInfo.speed * 0.001f) * Utils.CalFac(hull.shipConfig.activeEngineCom.lvl);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
