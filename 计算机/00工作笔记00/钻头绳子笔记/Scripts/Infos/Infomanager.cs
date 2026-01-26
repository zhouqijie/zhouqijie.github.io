using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infomanager
{
    //Static
    public static int Version => 100;
    public static int levelSceneCount = 5;

    //Single instance
    private static Infomanager inst = null;
    public static Infomanager Instance
    {
        get
        {
            if (inst != null)
            {
                return inst;
            }
            else
            {
                Init();
                return inst;
            }
        }
    }




    //field

    public UserData userData = null;

    public DrillInfo[] drillInfos = null;
    public HookInfo[] hookInfos = null;

    public BonusInfo[] checkinBonus1 = null;
    public BonusInfo[] checkinBonus2 = null;
    public BonusInfo[] rankBonus1 = null;
    public BonusInfo[] rankBonus2 = null;

    public MissionInfo[] missionInfos = null;


    //Com
    public BonusTimer bonusTimer;

    //TMP
    public static bool newDay = false;








    private static void Init()
    {
        inst = new Infomanager();

        //hook and drill infos
        inst.drillInfos = DrillInfo.Deserialize(Resources.Load<TextAsset>("Json/Drills").text);
        inst.hookInfos = HookInfo.Deserialize(Resources.Load<TextAsset>("Json/Hooks").text);

        //bonus infos
        BonusInfo.Deserialize(Resources.Load<TextAsset>("Json/Bonus").text, out inst.checkinBonus1, out inst.checkinBonus2, out inst.rankBonus1, out inst.rankBonus2);

        //mIssion infos
        inst.missionInfos = MissionInfo.Deserialize(Resources.Load<TextAsset>("Json/Missions").text);

        //userdata infos
        if (PlayerPrefs.GetString("UserData", "") != "")
        {
            inst.userData = UserData.Deserialzie(PlayerPrefs.GetString("UserData"));
        }
        else
        {
            inst.userData = UserData.Deserialzie(Resources.Load<TextAsset>("Json/UserDataDefault").text);
        }


        //new day
        if(System.DateTime.Today.ToShortDateString() != inst.userData.today)
        {
            //new day
            newDay = true;
            inst.userData.today = System.DateTime.Today.ToShortDateString();
            inst.userData.todayLoginBonus = 0;
            inst.userData.todayKills = 0;
            inst.userData.todayLevels = 0;
            inst.userData.todayShares = 0;
            inst.userData.todayBuys = 0;
            inst.userData.todayGrabRockets = 0;
            inst.userData.todayGrabCameras = 0;
        }

        //new week
        if (UtilsCheckin.GetTodayIndex(inst.userData.checkins1, inst.userData.checkins2) > 6)
        {
            //clean checkins
            UtilsCheckin.CleanCheckins(ref inst.userData.checkins1, ref inst.userData.checkins2);
        }


        //Instances
        Recorder.CreateInstance();
        AdManager.CreateInstance();
        inst.bonusTimer = BonusTimer.CreateInstance();


        //test
        //...
    }



    public static void Save()
    {
        PlayerPrefs.SetString("UserData", Instance.userData.Serialize());
    }



#if UNITY_EDITOR
    [UnityEditor.MenuItem("InfoManager/UserData/Clean")]
    public static void CleanUserData()
    {
        PlayerPrefs.DeleteKey("UserData");
    }

    [UnityEditor.MenuItem("InfoManager/UserData/SaveToDeskTop")]
    public static void SaveToDesktop()
    {
        string str = Instance.userData.Serialize();
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\userdata_save_desktop.json";
        System.IO.File.WriteAllText(path, str);
    }
#endif
}




