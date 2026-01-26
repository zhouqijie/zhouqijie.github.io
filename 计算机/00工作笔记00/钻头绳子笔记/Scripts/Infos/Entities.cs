using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using LitJson;







public class DrillInfo
{
    public int id;

    public int dps;

    public int maxHeat;

    public int price;

    public static DrillInfo[] Deserialize(string json)
    {
        List<DrillInfo> list = new List<DrillInfo>();

        JsonData jData = JsonMapper.ToObject(json)["drills"];

        foreach (JsonData jDrill in jData)
        {
            DrillInfo info = new DrillInfo();

            info.id = (int)jDrill["id"];
            info.dps = (int)jDrill["dps"];
            info.maxHeat = (int)jDrill["maxHeat"];
            info.price = (int)jDrill["price"];

            list.Add(info);
        }

        return list.OrderBy(d => d.id).ToArray();
    }
}


public class HookInfo
{
    public int id;

    public int speed;

    public int price;

    public static HookInfo[] Deserialize(string json)
    {
        List<HookInfo> list = new List<HookInfo>();

        JsonData jdata = JsonMapper.ToObject(json)["hooks"];

        foreach(JsonData jHook in jdata)
        {
            HookInfo info = new HookInfo();

            info.id = (int)jHook["id"];
            info.speed = (int)jHook["speed"];
            info.price = (int)jHook["price"];

            list.Add(info);
        }

        return list.OrderBy(x => x.id).ToArray();
    }
}


public class BonusInfo
{
    public int id;

    public string name;

    public int amount;

    public int type;

    public static void Deserialize(string json, out BonusInfo[] checkins1, out BonusInfo[] checkins2, out BonusInfo[] rankBonus1, out BonusInfo[] rankBonus2)
    {
        checkins1 = new BonusInfo[7];
        checkins2 = new BonusInfo[7];
        rankBonus1 = new BonusInfo[7];
        rankBonus2 = new BonusInfo[7];


        JsonData jdata = JsonMapper.ToObject(json);
        JsonData jdataB1 = jdata["checkinBonus1"]; jdataB1.SetJsonType(JsonType.Array);
        JsonData jdataB2 = jdata["checkinBonus2"]; jdataB2.SetJsonType(JsonType.Array);
        JsonData jdataRB1 = jdata["rankBonus1"]; jdataRB1.SetJsonType(JsonType.Array);
        JsonData jdataRB2 = jdata["rankBonus2"]; jdataRB2.SetJsonType(JsonType.Array);

        for (int i = 0; i < jdataB1.Count; i++)
        {
            checkins1[i] = new BonusInfo();
            checkins1[i].id = (int)jdataB1[i]["id"];
            checkins1[i].name = (string)jdataB1[i]["name"];
            checkins1[i].amount = (int)jdataB1[i]["amount"];
            checkins1[i].type = (int)jdataB1[i]["type"];
        }
        for (int i = 0; i < jdataB2.Count; i++)
        {
            checkins2[i] = new BonusInfo();
            checkins2[i].id = (int)jdataB2[i]["id"];
            checkins2[i].name = (string)jdataB2[i]["name"];
            checkins2[i].amount = (int)jdataB2[i]["amount"];
            checkins2[i].type = (int)jdataB2[i]["type"];
        }
        for (int i = 0; i < jdataRB1.Count; i++)
        {
            rankBonus1[i] = new BonusInfo();
            rankBonus1[i].id = (int)jdataRB1[i]["id"];
            rankBonus1[i].name = (string)jdataRB1[i]["name"];
            rankBonus1[i].amount = (int)jdataRB1[i]["amount"];
            rankBonus1[i].type = (int)jdataRB1[i]["type"];
        }
        for (int i = 0; i < jdataRB2.Count; i++)
        {
            rankBonus2[i] = new BonusInfo();
            rankBonus2[i].id = (int)jdataRB2[i]["id"];
            rankBonus2[i].name = (string)jdataRB2[i]["name"];
            rankBonus2[i].amount = (int)jdataRB2[i]["amount"];
            rankBonus2[i].type = (int)jdataRB2[i]["type"];
        }
    }
}


public class MissionInfo
{
    public int id;

    public string description;

    public int bonusMoneyAmount;

    public static MissionInfo[] Deserialize(string json)
    {
        JsonData jData = JsonMapper.ToObject(json)["missions"];

        MissionInfo[] missionInfos = new MissionInfo[jData.Count];

        for(int i = 0; i < jData.Count; i++)
        {
            missionInfos[i] = new MissionInfo();
            missionInfos[i].id = (int)jData[i]["id"];
            missionInfos[i].description = (string)jData[i]["description"];
            missionInfos[i].bonusMoneyAmount = (int)jData[i]["bonusMoneyAmount"];
        }

        return missionInfos;
    }
}




public class UserData
{
    public string name;

    public int money;

    public int rank;

    public int activeHook;

    public int activeDrill;

    public List<int> myHooks;

    public List<int> myDrills;




    public int currentLevel;
    public int currentRank;
    public string today;
    public int todayLoginBonus;
    public int todayKills;
    public int todayLevels;
    public int todayShares;
    public int todayBuys;
    public int todayGrabRockets;
    public int todayGrabCameras;

    public string[] checkins1;
    public string[] checkins2;
    public int[] missionFinished;





    public static UserData Deserialzie(string json)
    {
        UserData userdata = new UserData();

        JsonData jData = JsonMapper.ToObject(json)["userData"];

        userdata.name = (string)jData["name"];
        userdata.money = (int)jData["money"];
        userdata.rank = (int)jData["rank"];
        userdata.activeHook = (int)jData["activeHook"];
        userdata.activeDrill = (int)jData["activeDrill"];
        

        userdata.myHooks = UtilsLitJson.GetList<int>(jData["myHooks"]);
        userdata.myDrills = UtilsLitJson.GetList<int>(jData["myDrills"]);


        userdata.currentLevel = (int)jData["currentLevel"];
        userdata.currentRank = (int)jData["currentRank"];
        userdata.today = (string)jData["today"];
        userdata.todayLoginBonus = (int)jData["todayLoginBonus"];
        userdata.todayKills = (int)jData["todayKills"];
        userdata.todayLevels = (int)jData["todayLevels"];
        userdata.todayShares = (int)jData["todayShares"];
        userdata.todayBuys = (int)jData["todayBuys"];
        userdata.todayGrabRockets = (int)jData["todayGrabRockets"];
        userdata.todayGrabCameras = (int)jData["todayGrabCameras"];

        userdata.checkins1 = UtilsLitJson.GetArray<string>(jData["checkins1"]);
        userdata.checkins2 = UtilsLitJson.GetArray<string>(jData["checkins2"]);
        userdata.missionFinished = UtilsLitJson.GetArray<int>(jData["missionFinished"]);

        return userdata;
    }


    public string Serialize()
    {
        JsonData jdata = new JsonData();
        JsonData juserdata = jdata["userData"] = new JsonData();

        juserdata["name"] = this.name;
        juserdata["money"] = this.money;
        juserdata["rank"] = this.rank;
        juserdata["activeHook"] = this.activeHook;
        juserdata["activeDrill"] = this.activeDrill;
        
        juserdata["myHooks"] = UtilsLitJson.ToJsonData<int>(this.myHooks.ToArray());
        juserdata["myDrills"] = UtilsLitJson.ToJsonData<int>(this.myDrills.ToArray());


        juserdata["currentLevel"] = this.currentLevel;
        juserdata["currentRank"] = this.currentRank;
        juserdata["today"] = this.today;
        juserdata["todayLoginBonus"] = this.todayLoginBonus;
        juserdata["todayKills"] = this.todayKills;
        juserdata["todayLevels"] = this.todayLevels;
        juserdata["todayShares"] = this.todayShares;
        juserdata["todayBuys"] = this.todayBuys;
        juserdata["todayGrabRockets"] = this.todayGrabRockets;
        juserdata["todayGrabCameras"] = this.todayGrabCameras;

        juserdata["checkins1"] = UtilsLitJson.ToJsonData<string>(this.checkins1);
        juserdata["checkins2"] = UtilsLitJson.ToJsonData<string>(this.checkins2);
        juserdata["missionFinished"] = UtilsLitJson.ToJsonData<int>(this.missionFinished);


        return System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(jdata));
    }
}
