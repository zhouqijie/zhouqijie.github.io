using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UNBridgeLib.LitJson;


public class UserData
{
    public string name;

    public int level;

    public int vipExp;

    public int vipLvl;

    public int avatarId;

    public int borderId;

    
    public int money;

    public int diamonds;

    public int cardA;

    public int cardB;

    //others(???)
    public int[] unlocks;
    public string[] checkins1;
    public string[] checkins2;
    public int[] guideProgress;


    //statistics data
    public int totalClassicGames;
    public int totalBattleGames;
    //everyday data
    public string today;
    public List<int> todayMissionFinished;
    public int todayClassicGames;
    public int todayBattleGames;
    public int todayClassicWins;
    public int todayBattleWins;
    public int todayKills;

    //inventory(???)
    public List<CharactorInstance> charactors;


    public List<SkinInfo> mySkins;
    public List<DecorationInfo> myDecorations;
    public List<DisguiseObjectInfo> myDisguiseObjects;

    //current
    public SkinInfo activeSkin;
    public DecorationInfo activeDecoration0;
    public DecorationInfo activeDecoration1;
    public DecorationInfo activeDecoration2;
    public DisguiseObjectInfo activeDisguiseObj;


    public static UserData Deserialize(string json, Infomanager infomanager)
    {
        UserData userdata = new UserData();

        JsonData jData = JsonMapper.ToObject(json)["UserData"];
        
        //assets
        userdata.name = (string)jData["name"];
        userdata.level = (int)jData["level"];
        userdata.vipExp = (int)jData["vipExp"];
        userdata.vipLvl = (int)jData["vipLvl"];
        userdata.avatarId = (int)jData["avatarId"];
        userdata.borderId = (int)jData["borderId"];
        userdata.money = (int)jData["money"];
        userdata.diamonds = (int)jData["diamonds"];
        userdata.cardA = (int)jData["cardA"];
        userdata.cardB = (int)jData["cardB"];

        //unlocks
        userdata.unlocks = new int[10];
        for (int i = 0; i < 10; i++)
        {
            userdata.unlocks[i] = (int)jData["unlocks"][i];
        }

        //checkins
        userdata.checkins1 = new string[7];
        userdata.checkins2 = new string[7];
        for (int i = 0; i < 7; i++)
        {
            userdata.checkins1[i] = (string)jData["checkins1"][i];
            userdata.checkins2[i] = (string)jData["checkins2"][i];
        }

        //guide progress
        userdata.guideProgress = new int[jData["guideProgress"].Count];
        for (int i = 0; i < jData["guideProgress"].Count; i++)
        {
            userdata.guideProgress[i] = (int)jData["guideProgress"][i];
        }

        //statistics
        userdata.totalClassicGames = (int)jData["totalClassicGames"];
        userdata.totalBattleGames = (int)jData["totalBattleGames"];

        //today
        userdata.today = (string)jData["today"];
        userdata.todayMissionFinished = new List<int>();
        for(int i = 0; i < jData["todayMissionFinished"].Count; i++)
        {
            userdata.todayMissionFinished.Add((int)jData["todayMissionFinished"][i]);
        }
        userdata.todayClassicGames = (int)jData["todayClassicGames"];
        userdata.todayBattleGames = (int)jData["todayBattleGames"];
        userdata.todayClassicWins = (int)jData["todayClassicWins"];
        userdata.todayBattleWins = (int)jData["todayBattleWins"];
        userdata.todayKills = (int)jData["todayKills"];


        //charactors status
        int countC = jData["charactors"].Count;
        userdata.charactors = new List<CharactorInstance>();
        for(int i = 0; i < countC; i++)
        {
            CharactorInstance chara = new CharactorInstance();
            chara.info = infomanager.characterInfos.FirstOrDefault(c => c.id == (int)jData["charactors"][i]["id"]);
            
            chara.unlockedNodes = new int[jData["charactors"][i]["unlocked"].Count];
            for(int n = 0; n < chara.unlockedNodes.Length; n++)
            {
                chara.unlockedNodes[n] = (int)jData["charactors"][i]["unlocked"][n];
            }

            userdata.charactors.Add(chara);
        }

        //my skins
        int countSkin = jData["mySkins"].Count;
        userdata.mySkins = new List<SkinInfo>();
        for(int i = 0; i < countSkin; i++)
        {
            userdata.mySkins.Add(infomanager.skinInfos.FirstOrDefault(s => s.id == (int)jData["mySkins"][i]));
        }


        //my decorations
        int countD = jData["myDecorations"].Count;
        userdata.myDecorations = new List<DecorationInfo>();
        for(int i = 0; i < countD; i++)
        {
            userdata.myDecorations.Add(infomanager.decorationInfos.FirstOrDefault(d => d.id == (int)jData["myDecorations"][i]));
        }

        //my disguises
        int countP = jData["myDisguiseObjects"].Count;
        userdata.myDisguiseObjects = new List<DisguiseObjectInfo>();
        for(int i = 0; i < countP; i++)
        {
            userdata.myDisguiseObjects.Add(infomanager.disguiseObjInfos.FirstOrDefault(p => p.id == (int)jData["myDisguiseObjects"][i]));
        }

        //active
        userdata.activeSkin = userdata.mySkins.FirstOrDefault(c => c.id == (int)jData["activeSkin"]);
        userdata.activeDecoration0 = userdata.myDecorations.FirstOrDefault(c => c.id == (int)jData["activeDecoration"][0]);//not found == null
        userdata.activeDecoration1 = userdata.myDecorations.FirstOrDefault(c => c.id == (int)jData["activeDecoration"][1]);//not found == null
        userdata.activeDecoration2 = userdata.myDecorations.FirstOrDefault(c => c.id == (int)jData["activeDecoration"][2]);//not found == null
        userdata.activeDisguiseObj = userdata.myDisguiseObjects.FirstOrDefault(p => p.id == (int)jData["activeDisguiseObj"]);

        return userdata;
    }

    public static string Serialize(UserData userdata)
    {
        JsonData jdata = new JsonData();

        JsonData jUserdata = jdata["UserData"] = new JsonData();
        jUserdata["name"] = userdata.name;
        jUserdata["level"] = userdata.level;
        jUserdata["vipExp"] = userdata.vipExp;
        jUserdata["vipLvl"] = userdata.vipLvl;
        jUserdata["avatarId"] = userdata.avatarId;
        jUserdata["borderId"] = userdata.borderId;
        jUserdata["money"] = userdata.money;
        jUserdata["diamonds"] = userdata.diamonds;
        jUserdata["cardA"] = userdata.cardA;
        jUserdata["cardB"] = userdata.cardB;

        //unlocks
        jUserdata["unlocks"] = new JsonData();
        jUserdata["unlocks"].SetJsonType(JsonType.Array);
        for (int i = 0; i < 10; i++)
        {
            jUserdata["unlocks"].Add(i);

            jUserdata["unlocks"][i] = userdata.unlocks[i];
        }

        //checkins
        jUserdata["checkins1"] = new JsonData();
        jUserdata["checkins1"].SetJsonType(JsonType.Array);
        jUserdata["checkins2"] = new JsonData();
        jUserdata["checkins2"].SetJsonType(JsonType.Array);
        for (int i = 0; i < 7; i++)
        {
            jUserdata["checkins1"].Add(i);
            jUserdata["checkins2"].Add(i);

            jUserdata["checkins1"][i] = userdata.checkins1[i];
            jUserdata["checkins2"][i] = userdata.checkins2[i];
        }

        //guideProgress
        jUserdata["guideProgress"] = new JsonData();
        jUserdata["guideProgress"].SetJsonType(JsonType.Array);
        for(int i = 0; i < userdata.guideProgress.Length; i++)
        {
            jUserdata["guideProgress"].Add(i);

            jUserdata["guideProgress"][i] = userdata.guideProgress[i];
        }

        //statistics
        jUserdata["totalClassicGames"] = userdata.totalClassicGames;
        jUserdata["totalBattleGames"] = userdata.totalBattleGames;
        //today
        jUserdata["today"] = userdata.today;
        jUserdata["todayMissionFinished"] = new JsonData();
        jUserdata["todayMissionFinished"].SetJsonType(JsonType.Array);
        for (int i = 0; i < userdata.todayMissionFinished.Count; i++)
        {
            jUserdata["todayMissionFinished"].Add(i);
            jUserdata["todayMissionFinished"][i] = userdata.todayMissionFinished[i];
        }
        jUserdata["todayClassicGames"] = userdata.todayClassicGames;
        jUserdata["todayBattleGames"] = userdata.todayBattleGames;
        jUserdata["todayClassicWins"] = userdata.todayClassicWins;
        jUserdata["todayBattleWins"] = userdata.todayBattleWins;
        jUserdata["todayKills"] = userdata.todayKills;


        //charactors status
        jUserdata["charactors"] = new JsonData();
        jUserdata["charactors"].SetJsonType(JsonType.Array);
        for (int i = 0; i < userdata.charactors.Count; i++)
        {
            jUserdata["charactors"].Add(i);

            jUserdata["charactors"][i] = new JsonData();
            jUserdata["charactors"][i]["id"] = userdata.charactors[i].info.id;
            jUserdata["charactors"][i]["unlocked"] = new JsonData(); jUserdata["charactors"][i]["unlocked"].SetJsonType(JsonType.Array);

            for(int n = 0; n < userdata.charactors[i].unlockedNodes.Length; n++)
            {
                jUserdata["charactors"][i]["unlocked"].Add(n);
                jUserdata["charactors"][i]["unlocked"][n] = (int)userdata.charactors[i].unlockedNodes[n];
            }

        }

        //my skins
        jUserdata["mySkins"] = new JsonData();
        jUserdata["mySkins"].SetJsonType(JsonType.Array);
        for(int i = 0; i < userdata.mySkins.Count; i++)
        {
            jUserdata["mySkins"].Add(i);

            jUserdata["mySkins"][i] = userdata.mySkins[i].id;
        }

        //my decorations
        jUserdata["myDecorations"] = new JsonData();
        jUserdata["myDecorations"].SetJsonType(JsonType.Array);
        for (int i = 0; i < userdata.myDecorations.Count; i++)
        {
            jUserdata["myDecorations"].Add(i);

            jUserdata["myDecorations"][i] = userdata.myDecorations[i].id;
        }
        //my disguises
        jUserdata["myDisguiseObjects"] = new JsonData();
        jUserdata["myDisguiseObjects"].SetJsonType(JsonType.Array);
        for (int i = 0; i < userdata.myDisguiseObjects.Count; i++)
        {
            jUserdata["myDisguiseObjects"].Add(i);

            jUserdata["myDisguiseObjects"][i] = userdata.myDisguiseObjects[i].id;
        }




        //active skin
        jUserdata["activeSkin"] = new JsonData();
        jUserdata["activeSkin"] = userdata.activeSkin.id;
        //active deco
        jUserdata["activeDecoration"] = new JsonData();
        jUserdata["activeDecoration"].SetJsonType(JsonType.Array);
        jUserdata["activeDecoration"].Add(0);
        jUserdata["activeDecoration"][0] = userdata.activeDecoration0 != null ? userdata.activeDecoration0.id : -1;
        jUserdata["activeDecoration"].Add(1);
        jUserdata["activeDecoration"][1] = userdata.activeDecoration1 != null ? userdata.activeDecoration1.id : -1;
        jUserdata["activeDecoration"].Add(2);
        jUserdata["activeDecoration"][2] = userdata.activeDecoration2 != null ? userdata.activeDecoration2.id : -1;
        //active disguise
        jUserdata["activeDisguiseObj"] = new JsonData();
        jUserdata["activeDisguiseObj"] = userdata.activeDisguiseObj.id;


        return System.Text.RegularExpressions.Regex.Unescape(JsonMapper.ToJson(jdata));
    }
}

public class MatchUser
{
    public string name;

    public int avatarId;

    public int borderId;

    public int rank;

    public MatchUser(string initName, int initAvatar, int initBorder, int initrank)
    {
        this.name = initName;
        this.avatarId = initAvatar;
        this.borderId = initBorder;
        this.rank = initrank;
    }
}







//-------------------------------Instance classes--------------------------------------

//---职业实例----
public class CharactorInstance
{
    public CharactorInfo info;//info

    public int[] unlockedNodes;//已解锁信息
}





//-------------------------------Static Infos--------------------------------------




public class CharactorInfo
{
    public int id;

    public string name;

    public string description;

    public int price;//useless

    public List<AbilityInfo> abilities;

    public List<TalentNodeInfo> talentNodes;


    public static CharactorInfo[] Deserialize(string json)
    {

        List<CharactorInfo> charactorInfos = new List<CharactorInfo>();
        List<AbilityInfo> allAbilities = new List<AbilityInfo>();
        List<TalentNodeInfo> allTalentNodes = new List<TalentNodeInfo>();

        //...
        JsonData jdata = JsonMapper.ToObject(json);
        
        //---ability---
        foreach(JsonData item in jdata["Abilities"])
        {
            AbilityInfo ab = new AbilityInfo();

            ab.id = (int)item["id"];
            ab.name = (string)item["name"];
            ab.description = (string)item["description"];
            ab.team = (int)item["team"];

            allAbilities.Add(ab);
        }
        //---talentnodes---
        foreach (JsonData item in jdata["TalentNodes"])
        {
            TalentNodeInfo node = new TalentNodeInfo();

            node.id = (int)item["id"];

            allTalentNodes.Add(node);
        }
        foreach (JsonData item in jdata["TalentNodes"])
        {
            TalentNodeInfo node = allTalentNodes.FirstOrDefault(t => t.id == (int)item["id"]);
            //pos
            node.pos = new Vector2((int)item["pos"][0], (int)item["pos"][1]);
            //name
            node.name = (string)item["name"];
            //description
            node.description = (string)item["description"];
            //value
            node.value = (int)item["value"];
            //team
            node.team = (int)item["team"];
            //group
            node.group = (string)item["group"];
            //price
            node.price = (int)item["price"];


            //preNodes
            node.preNodes = new List<TalentNodeInfo>();
            for (int i = 0; i < item["preNodes"].Count; i++)
            {
                node.preNodes.Add(allTalentNodes.FirstOrDefault(t => t.id == (int)item["preNodes"][i]));
            }
        }


        //---charactors---
        foreach (JsonData jCharactor in jdata["Charactors"])
        {
            CharactorInfo charactorInfo = new CharactorInfo();

            //id / name /description
            charactorInfo.id = (int)jCharactor["id"];
            charactorInfo.name = (string)jCharactor["name"];
            charactorInfo.description = (string)jCharactor["description"];
            charactorInfo.price = (int)jCharactor["price"];

            //ab
            charactorInfo.abilities = new List<AbilityInfo>();
            for(int i = 0; i < jCharactor["abilities"].Count; i++)
            {
                charactorInfo.abilities.Add(allAbilities.FirstOrDefault(a => a.id == (int)jCharactor["abilities"][i]));
            }

            //talent
            charactorInfo.talentNodes = new List<TalentNodeInfo>();
            for (int i = 0; i < jCharactor["talentNodes"].Count; i++)
            {
                charactorInfo.talentNodes.Add(allTalentNodes.FirstOrDefault(a => a.id == (int)jCharactor["talentNodes"][i]));
            }

            charactorInfos.Add(charactorInfo);
        }

        

        return charactorInfos.ToArray();
    }
}


//---角色皮肤---
public class SkinInfo
{
    public int id;

    public string name;

    public int charaId;
    public CharactorInfo charaInfo;

    public int price;

    public int get;

    public string rank;

    

    public static SkinInfo[] Deserialize(Infomanager infomanager, string json)
    {
        List<SkinInfo> list = new List<SkinInfo>();

        JsonData jdata = JsonMapper.ToObject(json);

        //----skins----
        foreach (JsonData jSkin in jdata["Skins"])
        {
            SkinInfo info = new SkinInfo();

            info.id = (int)jSkin["id"];
            info.name = (string)jSkin["name"];
            info.charaId = (int)jSkin["charactor"];
            info.charaInfo = infomanager.characterInfos.FirstOrDefault(c => c.id == info.charaId);
            info.price = (int)jSkin["price"];
            info.get = (int)jSkin["get"];
            info.rank = (string)jSkin["rank"];

            list.Add(info);
        }

        return list.ToArray();
    }
}

//---角色技能----
public class AbilityInfo
{
    public int id;

    public string name;

    public int team;

    public string description;
}

//----天赋树----

public class TalentNodeInfo
{
    public int id;

    public Vector2 pos;

    public string name;

    public string description;

    public int value;//example: attack_5

    public int team;

    public string group;

    public int price;

    public List<TalentNodeInfo> preNodes;
}







public class DecorationInfo
{
    public int id;

    public string name;

    public string chinese;

    public string rank;

    public int price;

    public string group;//back weapon head





    public static DecorationInfo[] Deserialize(string json)
    {
        List<DecorationInfo> list = new List<DecorationInfo>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach(JsonData deco in jData["Decorations"])
        {
            DecorationInfo decoInfo = new DecorationInfo();

            decoInfo.id = (int)deco["id"];
            decoInfo.name = (string)deco["name"];
            decoInfo.chinese = (string)deco["chinese"];
            decoInfo.rank = (string)deco["rank"];
            decoInfo.price = (int)deco["price"];
            decoInfo.group = (string)deco["group"];

            list.Add(decoInfo);
        }
        
        return list.ToArray();
    }
}




public class DisguiseObjectInfo
{
    public int id;

    public string name;

    public string chinese;

    public string rank;

    public int price;

    public string group;



    public static DisguiseObjectInfo[] Deserialize(string json)
    {
        List<DisguiseObjectInfo> list = new List<DisguiseObjectInfo>();

        JsonData jdata = JsonMapper.ToObject(json);
        foreach(JsonData disguiseObj in jdata["Prefabs"])
        {
            DisguiseObjectInfo info = new DisguiseObjectInfo();

            info.id = (int)disguiseObj["id"];
            info.name = (string)disguiseObj["name"];
            info.chinese = (string)disguiseObj["chinese"];
            info.rank = (string)disguiseObj["rank"];
            info.price = (int)disguiseObj["price"];
            info.group = (string)disguiseObj["group"];

            list.Add(info);
        }
        
        return list.ToArray();
    }
}



public class NickNames
{
    public static List<string> Deserialzie(string json)
    {
        List<string> nickNameList = new List<string>();

        JsonData jData = JsonMapper.ToObject(json)["nickNames"];
        
        for(int i = 0; i < jData.Count; i++)
        {
            string nickName = (string)jData[i];

            nickNameList.Add(nickName);
        }

        return nickNameList;
    }
}

public class ChatContent
{
    public static List<string> Deserialize(string json)
    {
        List<string> chats = new List<string>();

        JsonData jData = JsonMapper.ToObject(json)["chats"];

        for (int i = 0; i < jData.Count; i++)
        {
            string chatContent = (string)jData[i];

            chats.Add(chatContent);
        }

        return chats;
    }
}


public class DailyMission
{
    public int id;

    public string description;

    public List<Bonus> bonuses;

    public bool Achieved()
    {
        switch (id)
        {
            case 0:
                {
                    if(Infomanager.GetInstance().userData.todayClassicGames > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case 1:
                {
                    if(Infomanager.GetInstance().userData.todayBattleGames > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case 2:
                {
                    if (Infomanager.GetInstance().userData.todayKills >= 5)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case 3:
                {
                    if (Infomanager.GetInstance().userData.todayClassicWins > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case 4:
                {
                    if (Infomanager.GetInstance().userData.todayBattleWins > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            default:
                {
                    return false;
                }
                break;
        }
    }
    
    public static List<DailyMission> Deseralize(string json)
    {
        List<DailyMission> missions = new List<DailyMission>();

        JsonData jData = JsonMapper.ToObject(json);
        
        foreach(JsonData jMission in jData["dailyMissions"])
        {
            DailyMission m = new DailyMission();
            m.id = (int)jMission["id"];
            m.description = (string)jMission["description"];

            m.bonuses = new List<Bonus>();
            if((int)jMission["money"] > 0)
            {
                Bonus b = new Bonus();
                b.id = 999;
                b.type = 0;
                b.name = "money";
                b.amount = (int)jMission["money"];

                m.bonuses.Add(b);
            }
            if ((int)jMission["diamonds"] > 0)
            {
                Bonus b = new Bonus();
                b.id = 999;
                b.type = 4;
                b.name = "diamonds";
                b.amount = (int)jMission["diamonds"];

                m.bonuses.Add(b);
            }
            if ((int)jMission["vipExp"] > 0)
            {
                Bonus b = new Bonus();
                b.id = 999;
                b.type = 5;
                b.name = "vipExp";
                b.amount = (int)jMission["vipExp"];

                m.bonuses.Add(b);
            }


            missions.Add(m);
        }

        return missions;
    }
}



public class Bonus
{
    public int id;

    public int type;//0-money  1-charactor 2- decoration 3-disguise 4-diamonds

    public string name;

    public int amount;

    public static void Deserialize(string json, out List<Bonus> checkins1, out List<Bonus> checkins2)
    {
        checkins1 = new List<Bonus>();
        checkins2 = new List<Bonus>();

        JsonData jData = JsonMapper.ToObject(json);

        foreach(JsonData jcheckin in jData["checkins1"])
        {
            Bonus bonus = new Bonus();

            bonus.id = (int)jcheckin["id"];
            bonus.name = (string)jcheckin["name"];
            bonus.type = (int)jcheckin["type"];
            bonus.amount = (int)jcheckin["amount"];

            checkins1.Add(bonus);
        }
        foreach (JsonData jcheckin2 in jData["checkins2"])
        {
            Bonus bonus2 = new Bonus();

            bonus2.id = (int)jcheckin2["id"];
            bonus2.name = (string)jcheckin2["name"];
            bonus2.type = (int)jcheckin2["type"];
            bonus2.amount = (int)jcheckin2["amount"];

            checkins2.Add(bonus2);
        }
        //...
    }
}








