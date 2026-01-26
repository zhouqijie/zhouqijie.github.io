using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UNBridgeLib.LitJson;

public class EditorInfoManager
{

    private static EditorInfoManager instance = null;
    public static EditorInfoManager GetInstance()
    {
        if (instance == null)
        {
            Init();
        }

        return instance;
    }


    //entities info(map editor)
    public BlocksInfo blocksinfo;




    private static void Init()
    {
        EditorInfoManager infomanager = new EditorInfoManager();


        #region MAP_EDITOR
        //blocks info
        infomanager.blocksinfo = BlocksInfo.JsonDeserialize(System.IO.File.ReadAllText("Assets/Editor/Json/MapEditor/Blocks.json"));
        #endregion
        
        instance = infomanager;
    }


    public RoomInfo GetRoomInfo(string roomName)
    {
        if (!System.IO.File.Exists("Assets/Editor/Json/MapEditor/" + roomName + ".json"))
        {
            return null;
        }

        string str = System.IO.File.ReadAllText("Assets/Editor/Json/MapEditor/" + roomName + ".json"); 

        RoomInfo roomInfo = JsonMapper.ToObject<RoomInfo>(str);

        return roomInfo;
    }

    public List<PaletteEle> GetPalette(string paletteName)
    {
        if(!System.IO.File.Exists("Assets/Editor/Json/MapEditor/" + paletteName + ".json"))
        {
            return null;
        }
        string str = System.IO.File.ReadAllText("Assets/Editor/Json/MapEditor/" + paletteName + ".json");

        var palette = PaletteEle.Deserialize(str, GetInstance().blocksinfo);

        return palette;
    }
}
