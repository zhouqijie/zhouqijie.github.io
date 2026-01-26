using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UNBridgeLib.LitJson;

public class InfoManager
{
    private static InfoManager instance;

    public static int Version { get { return 100; } }
    
    public static int[] debrisIndexOfShipInfo;

    public List<ShipInfo> shipInfos;
    public List<CrewInfo> crewInfos;
    public List<WeaponInfo> weaponInfos;
    public List<ArmorInfo> armorInfos;
    public List<EngineInfo> engineInfos;
    public List<EquipmentInfo> allEquipmentInfos;

    public List<AchieveBonus> commonBonusList;
    public List<AchieveBonus> specialBonusList;
    public List<AchieveBonus> checkinBonusList;


    public UserData userData;


    //-----TEMP------

    public static int currentGameType; // -1 tu  0 - 2 mission 99 Match

    public static bool showAchieveOnLoad;

    public static int devicePerformance = 0;
    //-------





    public static InfoManager GetInstance()
    {
        if(!(instance != null))
        {
            InitInfos();
        }
        return instance;
    }

    public static void InitInfos()
    {
        if (instance != null)
        {
            return;
        }

        InfoManager infomanager = new InfoManager();

        //deserialize
        infomanager.shipInfos = ShipInfo.Deserialize(Resources.Load<TextAsset>("Json/ShipInfos").text);
        infomanager.crewInfos = CrewInfo.Deserialize(Resources.Load<TextAsset>("Json/CrewInfos").text);
        

        infomanager.weaponInfos = WeaponInfo.Deserialize(Resources.Load<TextAsset>("Json/WeaponInfos").text);
        infomanager.armorInfos = ArmorInfo.Deserialize(Resources.Load<TextAsset>("Json/ArmorInfos").text);
        infomanager.engineInfos = EngineInfo.Deserialize(Resources.Load<TextAsset>("Json/EngineInfos").text);
        infomanager.allEquipmentInfos = new List<EquipmentInfo>();
        infomanager.allEquipmentInfos.AddRange(infomanager.weaponInfos);
        infomanager.allEquipmentInfos.AddRange(infomanager.armorInfos);
        infomanager.allEquipmentInfos.AddRange(infomanager.engineInfos);

        AchieveBonus.Deserialize(Resources.Load<TextAsset>("Json/AchieveBonus").text, out infomanager.commonBonusList, out infomanager.specialBonusList, out infomanager.checkinBonusList);
        
        //读取存档
        if (PlayerPrefs.GetString("UserData", "null") != "null")//改为Uniy SetStringSystem.IO.File.Exists(path)
        {
            string str = PlayerPrefs.GetString("UserData");
            //Get Document Version??
            infomanager.userData = UserData.Deserialize(str, infomanager);
            Debug.Log("读取已有用户数据");
        }
        else
        {
            infomanager.userData = UserData.Deserialize(Resources.Load<TextAsset>("Json/UserDataDefault").text, infomanager);
            Debug.Log("未找到用户数据，读取默认。");
        }

        ////读取用户信息
        //try
        //{
        //    StarkSDKSpace.StarkSDK.API.GetAccountManager().Login(new StarkSDKSpace.StarkAccount.OnLoginSuccessCallback((code, anoCode, isLogin) => {
        //        StarkSDKSpace.StarkSDK.API.GetAccountManager().GetScUserInfo(new StarkSDKSpace.StarkAccount.OnGetScUserInfoSuccessCallback((ref StarkSDKSpace.ScUserInfo info) => {
        //            infomanager.userData.name = info.nickName;
        //        }), new StarkSDKSpace.StarkAccount.OnGetScUserInfoFailedCallback((string msg) => {
        //            infomanager.userData.name = "无名";
        //        }));

        //    }), new StarkSDKSpace.StarkAccount.OnLoginFailedCallback((msg) => {
        //        infomanager.userData.name = "无名";
        //    }));
        //}
        //catch
        //{
        //    infomanager.userData.name = "无名";
        //}
        

        //debris
        debrisIndexOfShipInfo = new int[6] { 0, 1, 2, 3, 4, 5 };


        instance = infomanager;
    }
    



    /// -----------------------------------------------------------------------------------------------------------
    /// -----------------------------------------------------------------------------------------------------------
    /// -----------------------------------------------------------------------------------------------------------

    
    public static string[] playerNames;
    public static int[] playerIds;
    public static void RefreshPlayers()
    {
        int max = 202;
        List<int> idArr = new List<int>();

        for (int i = 0; i < 10; i++)
        {
            int id = Random.Range(0, max);
            do
            {
                id = Random.Range(0, max);
            }
            while (idArr.Contains(id));

            idArr.Add(id);
        }
        playerIds = idArr.ToArray();



        playerNames = NickName.GetNickNames(Resources.Load<TextAsset>("Json/NickNames").text, playerIds);
        playerNames[0] = GetInstance().userData.name;
    }
}
