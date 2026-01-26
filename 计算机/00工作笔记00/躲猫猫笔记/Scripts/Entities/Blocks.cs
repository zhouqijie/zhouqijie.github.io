//#define CUSTOM_DEBUG

using System.Collections.Generic;

using UNBridgeLib.LitJson;

public class BlocksInfo
{
    public List<BPrefab> prefabs;

    public List<BMaterial> materials;

    public List<Block> blocks;

    public static BlocksInfo JsonDeserialize(string json)
    {
        BlocksInfo result = new BlocksInfo();

        result.prefabs = new List<BPrefab>();
        result.materials = new List<BMaterial>();
        result.blocks = new List<Block>();


        JsonData jData = JsonMapper.ToObject(json);

        JsonData prefabs = jData["prefabs"];
        JsonData materials = jData["materials"];
        JsonData blocks = jData["blocks"];


        //Prefabs
        foreach (string prefabKey in prefabs.Keys)
        {
            BPrefab newPrefab = new BPrefab();

            newPrefab.name = prefabKey;

            if (prefabs[prefabKey].ContainsKey("symmetry"))
            {
                int.TryParse(prefabs[prefabKey]["symmetry"].ToString(), out newPrefab.symmetry);
            }


            if (prefabs[prefabKey].ContainsKey("walkthrough"))
            {
                bool.TryParse(prefabs[prefabKey]["walkthrough"].ToString(), out newPrefab.walkthrough);
            }


            if (prefabs[prefabKey].ContainsKey("group"))
            {
                newPrefab.group = prefabs[prefabKey]["group"].ToString();
            }

            result.prefabs.Add(newPrefab);

#if CUSTOM_DEBUG
            Debug.Log(newPrefab.name);
#endif
        }



        //Materials
        foreach (string matKey in materials.Keys)
        {
            BMaterial newMat = new BMaterial();

            newMat.name = matKey;

            if (materials[matKey].ContainsKey("sound"))
            {
                newMat.sound = materials[matKey]["sound"].ToString();
            }
            if (materials[matKey].ContainsKey("texture"))
            {
                newMat.texture = new BTexture();
                if (materials[matKey]["texture"].ContainsKey("u"))
                {
                    int.TryParse(materials[matKey]["texture"]["u"].ToString(), out newMat.texture.u);
                    int.TryParse(materials[matKey]["texture"]["v"].ToString(), out newMat.texture.v);
                }
                else
                {
                    newMat.texture.top = new Top();
                    newMat.texture.side = new Side();
                    int.TryParse(materials[matKey]["texture"]["top"]["u"].ToString(), out newMat.texture.top.u);
                    int.TryParse(materials[matKey]["texture"]["top"]["v"].ToString(), out newMat.texture.top.v);
                    int.TryParse(materials[matKey]["texture"]["side"]["u"].ToString(), out newMat.texture.side.u);
                    int.TryParse(materials[matKey]["texture"]["side"]["v"].ToString(), out newMat.texture.side.v);
                }
            }

            result.materials.Add(newMat);

#if CUSTOM_DEBUG
            Debug.Log("Material: " + newMat.name);
#endif
        }

        //Blocks
        foreach (string blockKey in blocks.Keys)
        {
            Block newBlock = new Block();

            newBlock.name = blockKey;

            if (blocks[blockKey].ContainsKey("symmetry"))
            {
                //newBlock.symmetry = Convert.ToInt32(ele.Element("symmetry").Value);
                int.TryParse(blocks[blockKey]["symmetry"].ToString(), out newBlock.symmetry);
            }

            if (blocks[blockKey].ContainsKey("walkthrough"))
            {
                bool.TryParse(blocks[blockKey]["walkthrough"].ToString(), out newBlock.walkthrough);
            }


            if (blocks[blockKey].ContainsKey("group"))
            {
                newBlock.group = blocks[blockKey]["group"].ToString();
            }


            if (blocks[blockKey].ContainsKey("sound"))
            {
                newBlock.sound = blocks[blockKey]["sound"].ToString();
            }


            if (blocks[blockKey].ContainsKey("texture"))
            {
                newBlock.texture = new BTexture();
                if (blocks[blockKey]["texture"].ContainsKey("u"))
                {
                    int.TryParse(blocks[blockKey]["texture"]["u"].ToString(), out newBlock.texture.u);
                    int.TryParse(blocks[blockKey]["texture"]["v"].ToString(), out newBlock.texture.v);
                }
                else
                {
                    newBlock.texture.top = new Top();
                    newBlock.texture.side = new Side();
                    int.TryParse(blocks[blockKey]["texture"]["top"]["u"].ToString(), out newBlock.texture.top.u);
                    int.TryParse(blocks[blockKey]["texture"]["top"]["v"].ToString(), out newBlock.texture.top.v);
                    int.TryParse(blocks[blockKey]["texture"]["side"]["u"].ToString(), out newBlock.texture.side.u);
                    int.TryParse(blocks[blockKey]["texture"]["side"]["v"].ToString(), out newBlock.texture.side.v);
                }
            }
            else
            {
                newBlock.texture = null;
            }


            if (blocks[blockKey].ContainsKey("material"))
            {
                newBlock.material = result.materials.Find(x => x.name == blocks[blockKey]["material"].ToString());
            }
            else
            {
                newBlock.material = null;
            }

            result.blocks.Add(newBlock);

#if CUSTOM_DEBUG
            Debug.Log("Block:" + newBlock.name);
#endif
        }


        return result;
    }
}

public class BPrefab
{
    public string name;

    public bool walkthrough;

    public int symmetry;

    public string group;
}

public class BMaterial
{
    public string name;

    public BTexture texture;

    public string sound;
}

public class Block
{
    public string name;

    public bool walkthrough;

    public int symmetry;

    public string group;

    public string sound;

    public BMaterial material = null;

    public BTexture texture = null;
}


public class BTexture
{
    public int u;

    public int v;

    public Top top;

    public Side side;
}

public class Top
{
    public int u;

    public int v;
}

public class Side
{
    public int u;

    public int v;
}