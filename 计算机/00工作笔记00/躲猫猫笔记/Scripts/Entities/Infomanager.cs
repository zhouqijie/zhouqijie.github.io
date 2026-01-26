using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UNBridgeLib.LitJson;

public class Infomanager
{
    private static Infomanager instance = null;
    public static Infomanager GetInstance()
    {
        if(instance == null)
        {
            Init();
        }

        return instance;
    }

    //Version
    public static int Version { get { return 100; } }

    //game base info

    public const int maxLevel = 27;


    //avatars and borders Info
    public static int countAvatars = 9;
    public static int countBorders = 15;



    //nickNames
    public List<string> nickNames;

    //Chats
    public List<string> chatContents;

    //entities info(game base)
    public List<DailyMission> missions;
    public List<Bonus> checkins1;
    public List<Bonus> checkins2;


    public CharactorInfo[] characterInfos;
    public SkinInfo[] skinInfos;

    public DecorationInfo[] decorationInfos;
    public DisguiseObjectInfo[] disguiseObjInfos;

    public UserData userData;


    //tmp guides
    public static int guidesNum = 0;

    //tmp menu
    public static bool newDay = false;

    //tmp game
    public static List<MatchUser> currentRoomUsers = null;

    public static int playerTeam = 0;
    public static int gameType;









    private static void Init()
    {
        instance = new Infomanager();

        //nickNames
        instance.nickNames = NickNames.Deserialzie(Resources.Load<TextAsset>("Json/NickNames").text);

        //chats
        instance.chatContents = ChatContent.Deserialize(Resources.Load<TextAsset>("Json/Chats").text);

        //bonus and Missions
        instance.missions = DailyMission.Deseralize(Resources.Load<TextAsset>("Json/DailyMissions").text);
        Bonus.Deserialize(Resources.Load<TextAsset>("Json/Bonus").text, out instance.checkins1, out instance.checkins2);

        //Characters
        string jsonCharactors = Resources.Load<TextAsset>("Json/Charactors").text;
        instance.characterInfos = CharactorInfo.Deserialize(jsonCharactors);
        instance.skinInfos = SkinInfo.Deserialize(instance, jsonCharactors);

        //Decorations and Disguises
        instance.decorationInfos = DecorationInfo.Deserialize(Resources.Load<TextAsset>("Json/Decorations").text);
        instance.disguiseObjInfos = DisguiseObjectInfo.Deserialize(Resources.Load<TextAsset>("Json/Prefabs").text);


        //Userdata
        string jsonUserData = PlayerPrefs.GetString("UserData", "null");
        if (jsonUserData != "null")
        {
            instance.userData = UserData.Deserialize(jsonUserData, instance);
        }
        else
        {
            instance.userData = UserData.Deserialize(Resources.Load<TextAsset>("Json/UserDataDefault").text, instance);
        }

        //(new day) refresh today data
        if(instance.userData.today != System.DateTime.Today.ToShortDateString())
        {
            newDay = true;


            instance.userData.today = System.DateTime.Today.ToShortDateString();
            instance.userData.todayMissionFinished = new List<int>();
            instance.userData.todayClassicGames = 0;
            instance.userData.todayBattleGames = 0;
            instance.userData.todayClassicWins = 0;
            instance.userData.todayBattleWins = 0;
        }
        //(new week) refresh checkin
        if (GetTodayCheckinId() > 6)
        {
            Debug.Log("CleanCheckIns");
            CleanCheckins();
        }


        //debug
        //...

    }


    //--------------------------------------签到--------------------------------------------------

    public static int GetTodayCheckinId()
    {
        string[] checkins1 = Infomanager.GetInstance().userData.checkins1;
        string[] checkins2 = Infomanager.GetInstance().userData.checkins1;

        if (checkins1[0] == "" && checkins2[0] == "")
        {
            return 0;
        }
        else
        {
            System.DateTime firstDay;
            if (checkins1[0] != "")
            {
                System.DateTime.TryParse(checkins1[0], out firstDay);
            }
            else
            {
                System.DateTime.TryParse(checkins2[0], out firstDay);
            }

            int num = (System.DateTime.Today - firstDay).Days;

            return num;
        }
    }
    public static int DayIdChecked(int dayIndex)
    {
        int result = 0;
        if (Infomanager.GetInstance().userData.checkins1[dayIndex] != "") result++;
        if (Infomanager.GetInstance().userData.checkins2[dayIndex] != "") result++;
        return result;
    }

    public static void CheckIn(bool getall)
    {
        int todayId = GetTodayCheckinId();
        CheckIn(todayId, getall);
    }
    public static void CheckIn(int dayId, bool getall)
    {
        if (dayId > 6) return;

        Infomanager.GetInstance().userData.checkins1[dayId] = System.DateTime.Today.ToShortDateString();
        if (getall)
        {
            Infomanager.GetInstance().userData.checkins2[dayId] = System.DateTime.Today.ToShortDateString();
        }
    }
    public static void CleanCheckins()
    {
        Infomanager.GetInstance().userData.checkins1 = new string[7];
        Infomanager.GetInstance().userData.checkins2 = new string[7];
        for (int i = 0; i < 7; i++)
        {
            Infomanager.GetInstance().userData.checkins1[i] = "";
            Infomanager.GetInstance().userData.checkins2[i] = "";
        }
    }

}
