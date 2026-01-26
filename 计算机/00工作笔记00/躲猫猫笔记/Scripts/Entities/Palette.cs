using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UNBridgeLib.LitJson;

public class PaletteEle
{
    public string eleName;

    public bool isCube;

    public string prefabName;

    public Vector2 uvTop;

    public Vector2 uvSide;

    public static List<PaletteEle> Deserialize(string json, BlocksInfo blocksInfo)
    {
        List<PaletteEle> paletteList = new List<PaletteEle>();

        JsonData jData = JsonMapper.ToObject(json);
        
        jData["palette"].SetJsonType(JsonType.Array);
        for (int i = 0; i < jData["palette"].Count; i++)
        {
            PaletteEle palette = new PaletteEle();

            palette.eleName = (string)jData["palette"][i];

            //--------------------
            palette.isCube = false;
            palette.prefabName = null;
            palette.uvTop = new Vector2();
            palette.uvSide = new Vector2();

            //blocks?
            var block = blocksInfo.blocks.FirstOrDefault(b => b.name == palette.eleName);
            if (block != null)
            {
                if (block.material != null)
                {
                    palette.isCube = true;
                    palette.uvTop = new Vector2(block.material.texture.u, block.material.texture.v);
                    palette.uvSide= new Vector2( block.material.texture.u, block.material.texture.v);
                }
                else if (block.texture != null)
                {
                    palette.isCube = true;
                    palette.uvTop = new Vector2(block.texture.top.u, block.texture.top.v);
                    palette.uvSide = new Vector2(block.texture.side.u, block.texture.side.v);
                }
                else
                {
                    palette.isCube = false;
                    palette.prefabName = palette.eleName;
                }
            }


            //prefabs?
            var prefab = blocksInfo.prefabs.FirstOrDefault(b => b.name == palette.eleName);
            if(prefab != null)
            {
                palette.isCube = false;
                palette.prefabName = palette.eleName;
            }


            //matPrefabs?
            string subName = palette.eleName.Substring(palette.eleName.IndexOf('_') + 1);
            var matPrefab = blocksInfo.prefabs.FirstOrDefault(b => b.name == subName);
            if (matPrefab != null)
            {
                palette.isCube = false;
                palette.prefabName = subName;
            }


            //----------------------
            paletteList.Add(palette);
        }
        

        return paletteList;
    }
}
