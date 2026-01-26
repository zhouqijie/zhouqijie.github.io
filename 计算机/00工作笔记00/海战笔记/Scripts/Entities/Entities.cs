using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


using UNBridgeLib.LitJson;


//------------------------------------------------------Static Info Entities--------------------------------------------------

//ShipInfo

public class ShipInfo
{
    public int id;

    public string shipName;//example: bismarck

    public string displayName;

    public float attack;

    public float defense;

    public float hp;

    public float speed;

    public string rank;//0-D   1-C   2-B   3-A   4-S (0 - 4)

    public int ranklvl;

    public int get;

    public static List<ShipInfo> Deserialize(string json)
    {
        List<ShipInfo> shipInfos = new List<ShipInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach(JsonData item in jData["Ships"])
        {
            ShipInfo shipInfo = new ShipInfo();
            shipInfo.id = (int)item["id"];// if (shipInfo.id > 90) Debug.Log("load dd:" + shipInfo.id);
            shipInfo.rank = (string)item["rarity"];
            switch (shipInfo.rank)
            {
                case "C":
                    shipInfo.ranklvl = 1;
                    break;
                case "B":
                    shipInfo.ranklvl = 2;
                    break;
                case "A":
                    shipInfo.ranklvl = 3;
                    break;
                case "S":
                    shipInfo.ranklvl = 4;
                    break;
                default:
                    break;
            }

            shipInfo.shipName = (string)item["name"];
            shipInfo.displayName = (string)item["displayName"];
            shipInfo.attack = (int)item["attack"];
            shipInfo.defense = (int)item["defense"];
            shipInfo.hp = (float)item["highesthp"];
            shipInfo.speed = (float)item["speed"];
            shipInfo.get = (int)item["get"];

            shipInfos.Add(shipInfo);
        }

        return shipInfos;
    }
}


public class CrewInfo
{
    public string name;

    public string rank;

    public float attack;

    public float hp;

    public float defense;

    public float speed;

    public static List<CrewInfo> Deserialize(string json)
    {
        List<CrewInfo> crewInfoList = new List<CrewInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach(JsonData item in jData["Crews"])
        {
            CrewInfo crew = new CrewInfo();

            crew.name = (string)item["Name"];
            crew.rank = (string)item["Rank"];
            crew.attack = (int)item["Attack"];
            crew.hp = (int)item["Hp"];
            crew.defense = (int)item["Defense"];
            crew.speed = (int)item["Speed"];

            crewInfoList.Add(crew);
        }

        return crewInfoList;
    }
}


public class EquipmentInfo
{
    public int id;

    public string rank;

    public int ranklvl;

    public string displayName;//example: 380HE
    //------

    public Texture2D icon;
    //-----

    public void DeserialzeJData(JsonData jdata)
    {
        this.id = (int)jdata["id"];
        this.rank = (string)jdata["rarity"];
        switch (rank)
        {
            case "C":
                this.ranklvl = 1;
                break;
            case "B":
                this.ranklvl = 2;
                break;
            case "A":
                this.ranklvl = 3;
                break;
            case "S":
                this.ranklvl = 4;
                break;
            default:
                break;
        }
        this.displayName = (string)jdata["displayName"];
    }
}

//Weapon(Shell)Info

public class WeaponInfo : EquipmentInfo
{

    public float reloadTime;

    public float piercingPercentage;

    public float attack;

    //---MyCustom
    public int maxAmmo;
    public float baseV0;
    public float minAngle;

    public static List<WeaponInfo> Deserialize(string json)
    {
        List<WeaponInfo> weaponinfoList = new List<WeaponInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach(JsonData item in jData["Weapons"])
        {
            WeaponInfo weaponInfo = new WeaponInfo();
            weaponInfo.DeserialzeJData(item);

            weaponInfo.attack = (float)item["attack"];
            weaponInfo.reloadTime = 15f * (1f - (0.1f * (float)item["barrel"]));
            weaponInfo.piercingPercentage = Mathf.Clamp01((float)item["piercing"] / 100f);

            weaponInfo.baseV0 = 500f;
            weaponInfo.minAngle = 20f;
            weaponInfo.maxAmmo = 200;

            weaponinfoList.Add(weaponInfo);
        }

        return weaponinfoList;
    }
}


public class ArmorInfo : EquipmentInfo
{

    public float hp;

    public float defense;

    public static List<ArmorInfo> Deserialize(string json)
    {
        List<ArmorInfo> armorInfoList = new List<ArmorInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach (JsonData item in jData["Armors"])
        {
            ArmorInfo armorInfo = new ArmorInfo();

            armorInfo.DeserialzeJData(item);

            armorInfo.hp = (float)item["highesthp"];
            armorInfo.defense = (float)item["defense"];

            armorInfoList.Add(armorInfo);
        }

        return armorInfoList;
    }
}


public class EngineInfo : EquipmentInfo
{

    public float hp;

    public float speed;

    public static List<EngineInfo> Deserialize(string json)
    {
        List<EngineInfo> engineInfoList = new List<EngineInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach (JsonData item in jData["Engines"])
        {
            EngineInfo engineInfo = new EngineInfo();

            engineInfo.DeserialzeJData(item);

            engineInfo.hp = (float)item["highesthp"];
            engineInfo.speed = (float)item["speed"];

            engineInfoList.Add(engineInfo);
        }

        return engineInfoList;
    }
}

public class AbilityInfo
{
    public string abilityName;//example: Repair DamageControl Missile

    //???
    public float cd;
}


public class AchieveBonus
{
    public int id;

    public int type;

    public string name;

    public int amount;

    public static void Deserialize(string json, out List<AchieveBonus> bonusListCommon, out List<AchieveBonus> bonusListSpecial, out List<AchieveBonus> bonusListCheckin)
    {
        bonusListCommon = new List<AchieveBonus>();
        bonusListSpecial = new List<AchieveBonus>();
        bonusListCheckin = new List<AchieveBonus>();

        JsonData jData = JsonMapper.ToObject(json);
        
        foreach(JsonData item in jData["Common"])
        {
            AchieveBonus b = new AchieveBonus();
            b.id = (int)item["id"];
            b.type = (int)item["type"];
            b.name = (string)item["name"];
            b.amount = (int)item["amount"];

            bonusListCommon.Add(b);
        }

        foreach (JsonData item in jData["Special"])
        {
            AchieveBonus b = new AchieveBonus();
            b.id = (int)item["id"];
            b.type = (int)item["type"];
            b.name = (string)item["name"];
            b.amount = (int)item["amount"];

            bonusListSpecial.Add(b);
        }

        foreach (JsonData item in jData["CheckIn"])
        {
            AchieveBonus b = new AchieveBonus();
            b.id = (int)item["id"];
            b.type = (int)item["type"];
            b.name = (string)item["name"];
            b.amount = (int)item["amount"];

            bonusListCheckin.Add(b);
        }
    }
}


//------------------------------------------------------Dynamic Entities--------------------------------------------------

public class Crew//不重复
{
    public CrewInfo crewInfo;

    public int lvl;
}

public class ShipConfig
{
    public int shipConfigId;

    public int lvl;

    public ShipInfo shipInfo;

    //ShipConfig
    //public int activePrimaryCom1;//(use index)
    public ShipWeaponCom activeWeaponCom;
    public ShipArmorCom activeArmorCom;
    public ShipEngineCom activeEngineCom;

    public Crew crew;
}


public class ShipCom//可重复
{
    public int comId;
    public int lvl;
    public EquipmentInfo EquipmentInfo
    {
        get
        {
            if(this is ShipWeaponCom)
            {
                return (this as ShipWeaponCom).weaponInfo;
            }
            else if (this is ShipArmorCom)
            {
                return (this as ShipArmorCom).armorInfo;
            }
            else
            {
                return (this as ShipEngineCom).engineInfo;
            }
        }

        set
        {
            if (this is ShipWeaponCom)
            {
                (this as ShipWeaponCom).weaponInfo = value as WeaponInfo;
            }
            else if (this is ShipArmorCom)
            {
                (this as ShipArmorCom).armorInfo = value as ArmorInfo;
            }
            else
            {
                (this as ShipEngineCom).engineInfo = value as EngineInfo;
            }
        }
    }
}


public class ShipWeaponCom : ShipCom
{
    public WeaponInfo weaponInfo;
}

public class ShipArmorCom : ShipCom
{
    public ArmorInfo armorInfo;
}

public class ShipEngineCom : ShipCom
{
    public EngineInfo engineInfo;
}


//------------------------------------------------------User Data--------------------------------------------------

public class UserData
{
    public string name;
    public int money;
    public int equipmentMat;
    public int[] debris;


    public int achieveProgress;
    public bool[] bonusGet1;
    public bool[] bonusGet2;
    public int currentMission;
    public int currentLvl;
    public string[] checkins;
    public int totalGames;


    //guider
    public int[] guideProgress;


    public List<ShipConfig> userShips;

    public ShipConfig currentShip;

    public List<Crew> crews;

    public List<ShipWeaponCom> myWeaponComs;
    public List<ShipArmorCom> myArmorComs;
    public List<ShipEngineCom> myEngineComs;



    public static UserData Deserialize(string json, InfoManager infoManager)
    {
        JsonData jDataBase = JsonMapper.ToObject(json);
        JsonData jData = jDataBase["UserData"];


        UserData userData = new UserData();

        //Assets
        userData.name = (string)jData["Name"];
        userData.money = (int)jData["Assets"]["Money"];
        userData.equipmentMat = (int)jData["Assets"]["Materials"];
        userData.debris = new int[6];
        for (int i = 0; i < userData.debris.Length; i++)
        {
            if (jData["Assets"]["Debris"][i] != null)
            {
                userData.debris[i] = (int)jData["Assets"]["Debris"][i];
            }
            else
            {
                userData.debris[i] = 0;
            }
        }

        //Achieves
        userData.achieveProgress = (int)jData["AchieveProgress"];
        userData.bonusGet1 = new bool[20];
        for (int i = 0; i < userData.bonusGet1.Length; i++)
        {
            if (jData["BonusGet1"][i] != null)
            {
                userData.bonusGet1[i] = (int)jData["BonusGet1"][i] == 0 ? false : true;
            }
            else
            {
                userData.bonusGet1[i] = false;
            }
        }

        userData.bonusGet2 = new bool[20];
        for (int i = 0; i < userData.bonusGet2.Length; i++)
        {
            if (jData["BonusGet2"][i] != null)
            {
                userData.bonusGet2[i] = (int)jData["BonusGet2"][i] == 0 ? false : true;
            }
            else
            {
                userData.bonusGet2[i] = false;
            }
        }


        //Levels
        userData.currentMission = (int)jData["CurrentMission"];
        userData.currentLvl = (int)jData["CurrentLvl"];

        //checkins
        userData.checkins = new string[7];
        for (int i = 0; i < 7; i++)
        {
            if (jData["CheckIn"][i] != null)
            {
                userData.checkins[i] = (string)jData["CheckIn"][i];
            }
            else
            {
                userData.checkins[i] = "";
            }
        }


        //Tota Games
        userData.totalGames = (int)jData["TotalGames"];



        //Guide Progress
        userData.guideProgress = new int[20];
        jData["GuideProgress"].SetJsonType(JsonType.Array);
        for (int i = 0; i < 20; i++)
        {
            if(jData["GuideProgress"][i] != null)
            {
                userData.guideProgress[i] = (int)jData["GuideProgress"][i];
            }
        }

        //Crews
        userData.crews = new List<Crew>();
        foreach (JsonData item in jData["Crews"])
        {
            Crew crew = new Crew();

            crew.lvl = (int)item["Lvl"];
            crew.crewInfo = infoManager.crewInfos.Find(c => c.name == (string)item["Name"]);

            userData.crews.Add(crew);
        }

        //Coms
        userData.myWeaponComs = new List<ShipWeaponCom>();
        foreach (JsonData item in jData["WeaponComs"])
        {
            ShipWeaponCom weaponCom = new ShipWeaponCom();

            weaponCom.comId = (int)item["ComId"];
            weaponCom.weaponInfo = infoManager.weaponInfos.Find(w => w.id == (int)item["WeaponInfoId"]);
            weaponCom.lvl = (int)item["lvl"];

            userData.myWeaponComs.Add(weaponCom);
        }

        userData.myArmorComs = new List<ShipArmorCom>();
        foreach (JsonData item in jData["ArmorComs"])
        {
            ShipArmorCom armorCom = new ShipArmorCom();

            armorCom.comId = (int)item["ComId"];
            armorCom.armorInfo = infoManager.armorInfos.Find(w => w.id == (int)item["ArmorInfoId"]);
            armorCom.lvl = (int)item["lvl"];

            userData.myArmorComs.Add(armorCom);
        }

        userData.myEngineComs = new List<ShipEngineCom>();
        foreach (JsonData item in jData["EngineComs"])
        {
            ShipEngineCom engineCom = new ShipEngineCom();

            engineCom.comId = (int)item["ComId"];
            engineCom.engineInfo = infoManager.engineInfos.Find(w => w.id == (int)item["EngineInfoId"]);
            engineCom.lvl = (int)item["lvl"];

            userData.myEngineComs.Add(engineCom);
        }

        userData.userShips = new List<ShipConfig>();
        foreach(JsonData item in jData["UserShips"])
        {
            ShipConfig userShip = new ShipConfig();
            userShip.shipConfigId = (int)item["ShipConfigId"];
            userShip.lvl = (int)item["lvl"];
            userShip.shipInfo = infoManager.shipInfos.Find(s => s.id == (int)item["ShipInfoId"]);

            userShip.crew = userData.crews.FirstOrDefault(c => c.crewInfo.name == (string)item["CrewName"]);


            userShip.activeWeaponCom = userData.myWeaponComs.FirstOrDefault(w => w.comId == (int)item["activeWeaponComId"]);
            userShip.activeArmorCom = userData.myArmorComs.FirstOrDefault(w => w.comId == (int)item["activeArmorComId"]);
            userShip.activeEngineCom = userData.myEngineComs.FirstOrDefault(w => w.comId == (int)item["activeEngineComId"]);

            userData.userShips.Add(userShip);
        }

        userData.currentShip = userData.userShips.Find(s => s.shipConfigId == (int)jData["CurrentShipId"]);


        return userData;
    }

    public static string Serialize(UserData userdata)
    {

        JsonData jData = new JsonData();

        jData["UserData"] = new JsonData();
        jData["UserData"]["Name"] = userdata.name;
        jData["UserData"]["AchieveProgress"] = userdata.achieveProgress;
        jData["UserData"]["BonusGet1"] = new JsonData();
        jData["UserData"]["BonusGet2"] = new JsonData();
        jData["UserData"]["CheckIn"] = new JsonData();//CheckIn
        jData["UserData"]["CurrentMission"] = new JsonData();
        jData["UserData"]["CurrentLvl"] = new JsonData();

        jData["UserData"]["TotalGames"] = new JsonData();

        jData["UserData"]["GuideProgress"] = new JsonData();

        JsonData jAssets = new JsonData();
        JsonData jCrews = new JsonData(); jCrews.SetJsonType(JsonType.Array);
        JsonData jWeaponComs = new JsonData(); jWeaponComs.SetJsonType(JsonType.Array);
        JsonData jArmorComs = new JsonData(); jArmorComs.SetJsonType(JsonType.Array);
        JsonData jEngineComs = new JsonData(); jEngineComs.SetJsonType(JsonType.Array);
        JsonData jUserShips = new JsonData(); jUserShips.SetJsonType(JsonType.Array);

        //Assets
        jAssets["Money"] = userdata.money;
        jAssets["Materials"] = userdata.equipmentMat;
        jAssets["Debris"] = new JsonData();

        jAssets["Debris"].SetJsonType(JsonType.Array);
        for(int i = 0; i < 6; i++)
        {
            jAssets["Debris"].Add(i);
            jAssets["Debris"][i] = userdata.debris[i];
        }

        //Achieves
        //checkin
        jData["UserData"]["BonusGet1"].SetJsonType(JsonType.Array);
        jData["UserData"]["BonusGet1"].SetJsonType(JsonType.Array);
        jData["UserData"]["CheckIn"].SetJsonType(JsonType.Array);
        for (int i = 0; i < 20; i++)
        {
            jData["UserData"]["BonusGet1"].Add(i);
            jData["UserData"]["BonusGet1"][i] = userdata.bonusGet1[i] == false ? 0 : 1;

            jData["UserData"]["BonusGet2"].Add(i);
            jData["UserData"]["BonusGet2"][i] = userdata.bonusGet2[i] == false ? 0 : 1;
        }
        for (int i = 0; i < 7; i++)
        {
            jData["UserData"]["CheckIn"].Add(i);
            jData["UserData"]["CheckIn"][i] = userdata.checkins[i];
        }

        //Levels
        jData["UserData"]["CurrentMission"] = userdata.currentMission;
        jData["UserData"]["CurrentLvl"] = userdata.currentLvl;



        //Total Games
        jData["UserData"]["TotalGames"] = userdata.totalGames;


        //Guides
        jData["UserData"]["GuideProgress"].SetJsonType(JsonType.Array);
        for(int i = 0; i < 20; i++)
        {
            jData["UserData"]["GuideProgress"].Add(i);
            jData["UserData"]["GuideProgress"][i] = userdata.guideProgress[i];
        }


        //Crews
        foreach (var crew in userdata.crews)
        {
            JsonData jCrew = new JsonData();
            jCrew["Name"] = crew.crewInfo.name;
            jCrew["Lvl"] = crew.lvl;

            jCrews.Add(jCrew);
        }

        //Coms
        foreach (var weaponCom in userdata.myWeaponComs)
        {
            JsonData jWeaponCom = new JsonData();
            jWeaponCom["ComId"] = weaponCom.comId;
            jWeaponCom["WeaponInfoId"] = weaponCom.weaponInfo.id;
            jWeaponCom["lvl"] = weaponCom.lvl;

            jWeaponComs.Add(jWeaponCom);
        }

        foreach (var armorCom in userdata.myArmorComs)
        {
            JsonData jArmorCom = new JsonData();
            jArmorCom["ComId"] = armorCom.comId;
            jArmorCom["ArmorInfoId"] = armorCom.armorInfo.id;
            jArmorCom["lvl"] = armorCom.lvl;

            jArmorComs.Add(jArmorCom);
        }

        foreach (var engineCom in userdata.myEngineComs)
        {
            JsonData jEngineCom = new JsonData();
            jEngineCom["ComId"] = engineCom.comId;
            jEngineCom["EngineInfoId"] = engineCom.engineInfo.id;
            jEngineCom["lvl"] = engineCom.lvl;

            jEngineComs.Add(jEngineCom);
        }


        foreach(var ship in userdata.userShips)
        {
            JsonData jUserShip = new JsonData();
            jUserShip["ShipConfigId"] = ship.shipConfigId;
            jUserShip["lvl"] = ship.lvl;
            jUserShip["ShipInfoId"] = ship.shipInfo.id;

            if (ship.crew != null)
            {
                jUserShip["CrewName"] = ship.crew.crewInfo.name;
            }
            else
            {
                jUserShip["CrewName"] = "";
            }

            jUserShip["activeWeaponComId"] = -1;
            jUserShip["activeArmorComId"] = -1;
            jUserShip["activeEngineComId"] = -1;

            if (ship.activeWeaponCom != null)
            {
                jUserShip["activeWeaponComId"] = ship.activeWeaponCom.comId;
            }
            if(ship.activeArmorCom != null)
            {
                jUserShip["activeArmorComId"] = ship.activeArmorCom.comId;
            }
            if(ship.activeEngineCom != null)
            {
                jUserShip["activeEngineComId"] = ship.activeEngineCom.comId;
            }

            jUserShips.Add(jUserShip);
        }

        jData["UserData"]["Assets"] = jAssets;
        jData["UserData"]["Crews"] = jCrews;
        jData["UserData"]["WeaponComs"] = jWeaponComs;
        jData["UserData"]["ArmorComs"] = jArmorComs;
        jData["UserData"]["EngineComs"] = jEngineComs;
        jData["UserData"]["UserShips"] = jUserShips;
        jData["UserData"]["CurrentShipId"] = userdata.currentShip.shipConfigId;
        
        return System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(jData));
    }
}
//------------------------------------------------------------------
public class NickName
{
    public static string[] GetNickNames(string json, int[] ids)
    {
        string[] names = new string[10];

        JsonData jData = JsonMapper.ToObject(json);

        jData["NickNames"].SetJsonType(JsonType.Array);



        for(int i = 0; i < 10; i++)
        {
            names[i] = (string)jData["NickNames"][ids[i]];
        }

        return names;
    }
}



//-------------------------------------------------------------------------------------------------------------------

public class LevelReward//Level Bonus
{
    public string name;

    public int count;

    public int index;
}