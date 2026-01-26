using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using System.Linq;



/// <summary>
/// 无镂空地图
/// </summary>

public class MeshData
{
    public List<Vector3> vertices = new List<Vector3>();

    public List<Vector3> normals = new List<Vector3>();

    public List<Vector2> texcoords = new List<Vector2>();

    public List<int> triangles = new List<int>();


    public static Vector2[] GetFloatUV(float intU, float intV)
    {
        float uvScale = 0.03125f;
        Vector2[] uvs = new Vector2[4];

        uvs[0] = (new Vector2(intU * uvScale + (uvScale * 0.02f), (intV + 1) * uvScale - (uvScale * 0.02f)));
        uvs[1] = (new Vector2((intU + 1) * uvScale - (uvScale * 0.02f), (intV + 1) * uvScale - (uvScale * 0.02f)));
        uvs[2] = (new Vector2((intU + 1) * uvScale - (uvScale * 0.02f), intV * uvScale + (uvScale * 0.02f)));
        uvs[3] = (new Vector2(intU * uvScale + (uvScale * 0.02f), intV * uvScale + (uvScale * 0.02f)));


        //uvs[0] = (new Vector2(intU * uvScale, (intV + 1) * uvScale));
        //uvs[1] = (new Vector2((intU + 1) * uvScale, (intV + 1) * uvScale));
        //uvs[2] = (new Vector2((intU + 1) * uvScale, intV * uvScale));
        //uvs[3] = (new Vector2(intU * uvScale, intV * uvScale));

        return uvs;
    }
}


public class MapGenarator : MonoBehaviour
{
    public Texture2D texture;//const 512x512

    private int[,] heightArr = null; //const 512x512

    private MeshData meshData;//tmp 64x64

    private GameObject[] chunks;// 4x4





    //terrain block
    private bool uvsInit = false;
    private BTexture texGrass;
    private BTexture texSand;
    private Vector2 uvGrassT;
    private Vector2 uvSandT;
    private Vector2 uvGrassS;
    private Vector2 uvSandS;
    private const float uvScale = 0.03125f;







    void Start()
    {

        
    }


    private void UVInit()
    {
        texGrass = EditorInfoManager.GetInstance().blocksinfo.blocks.FirstOrDefault(b => b.name == "grass_block").texture;
        texSand = EditorInfoManager.GetInstance().blocksinfo.blocks.FirstOrDefault(b => b.name == "sand").texture;

        uvGrassT = new Vector2(texGrass.top.u, texGrass.top.v);
        uvSandT = new Vector2(texSand.top.u, texSand.top.v);
        uvGrassS = new Vector2(texGrass.side.u, texGrass.side.v);
        uvSandS = new Vector2(texSand.side.u, texSand.side.v);

        uvsInit = true;
    }


    public void ArrayInit()
    {
        heightArr = new int[texture.width, texture.height];

        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                heightArr[i, j] = Mathf.FloorToInt(texture.GetPixel(i, j).r * 10f) - 5;
            }
        }
    }

    private bool GetVoxel(int x, int y, int z)
    {
        if (!(heightArr != null)) ArrayInit();

        if (x > texture.width - 1 || x < 0 || z > texture.height - 1 || z < 0 || y > 4) return false;

        if(heightArr[x, z] >= y)
        {
            return true;
        }
        
        return false;
    }


#if UNITY_EDITOR
    private void Generate64x64(int iIn, int jIn)
    {
        if (!uvsInit) UVInit();
        if (!(heightArr != null)) ArrayInit();

        meshData = new MeshData();
        int offsetX = iIn * 64;
        int offsetZ = jIn * 64;
        int tmp = 0;

        for (int x = 0; x < 64; x++)
        {
            for (int z = 0; z < 64; z++)
            {
                for(int y = -5; y < 5; y++)
                {
                    Vector2[] uvsTop = y < 1 ? MeshData.GetFloatUV(uvSandT.x, uvSandT.y) : MeshData.GetFloatUV(uvGrassT.x, uvGrassT.y);
                    Vector2[] uvsSide = y < 1 ? MeshData.GetFloatUV(uvSandS.x, uvSandS.y) : MeshData.GetFloatUV(uvGrassS.x, uvGrassS.y);

                    //UP
                    if (!GetVoxel(x + offsetX, y, z + offsetZ)) continue;

                    if(GetVoxel(x + offsetX, y + 1, z + offsetZ) == false)
                    {
                        meshData.vertices.Add(new Vector3(x, y, z));
                        meshData.vertices.Add(new Vector3(x, y, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y, z));


                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));
                        
                        meshData.texcoords.Add(uvsTop[0]);
                        meshData.texcoords.Add(uvsTop[1]);
                        meshData.texcoords.Add(uvsTop[2]);
                        meshData.texcoords.Add(uvsTop[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }

                    //Back
                    if (GetVoxel(x + offsetX, y, z + offsetZ - 1) == false)
                    {
                        meshData.vertices.Add(new Vector3(x, y, z));
                        meshData.vertices.Add(new Vector3(x + 1, y, z));
                        meshData.vertices.Add(new Vector3(x + 1, y - 1, z));
                        meshData.vertices.Add(new Vector3(x, y - 1, z));


                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));
                        
                        meshData.texcoords.Add(uvsSide[0]);
                        meshData.texcoords.Add(uvsSide[1]);
                        meshData.texcoords.Add(uvsSide[2]);
                        meshData.texcoords.Add(uvsSide[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }

                    //Forward
                    if (GetVoxel(x + offsetX, y, z + offsetZ + 1) == false)
                    {
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));
                        meshData.vertices.Add(new Vector3(x, y, z + 1));
                        meshData.vertices.Add(new Vector3(x, y - 1, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y - 1, z + 1));


                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));
                        
                        meshData.texcoords.Add(uvsSide[0]);
                        meshData.texcoords.Add(uvsSide[1]);
                        meshData.texcoords.Add(uvsSide[2]);
                        meshData.texcoords.Add(uvsSide[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }

                    //Left
                    if (GetVoxel(x + offsetX - 1, y, z + offsetZ) == false)
                    {
                        meshData.vertices.Add(new Vector3(x, y, z + 1));
                        meshData.vertices.Add(new Vector3(x, y, z));
                        meshData.vertices.Add(new Vector3(x, y - 1, z));
                        meshData.vertices.Add(new Vector3(x, y - 1, z + 1));


                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        
                        meshData.texcoords.Add(uvsSide[0]);
                        meshData.texcoords.Add(uvsSide[1]);
                        meshData.texcoords.Add(uvsSide[2]);
                        meshData.texcoords.Add(uvsSide[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }

                    //Right
                    if (GetVoxel(x + offsetX + 1, y, z + offsetZ) == false)
                    {
                        meshData.vertices.Add(new Vector3(x + 1, y, z));
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y - 1, z));


                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));
                        
                        meshData.texcoords.Add(uvsSide[0]);
                        meshData.texcoords.Add(uvsSide[1]);
                        meshData.texcoords.Add(uvsSide[2]);
                        meshData.texcoords.Add(uvsSide[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }
                }
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.name = "MeshTerrain";
        mesh.vertices = meshData.vertices.ToArray();//Debug.Log(mesh.vertexCount);
        mesh.normals = meshData.normals.ToArray();
        mesh.uv = meshData.texcoords.ToArray();
        mesh.triangles = meshData.triangles.ToArray();

        //Save
        UnityEditor.AssetDatabase.CreateAsset(mesh, "Assets/Models/ChunkMesh/chunk_" + iIn + "_" + jIn + ".asset");
        UnityEditor.AssetDatabase.SaveAssets();

    }
#endif


#if UNITY_EDITOR
    [MenuItem("Utils/Chunks/Instantiate8x8")]
    [ExecuteInEditMode]
    private static void InstatiateChunks8x8()
    {
        var map = FindObjectOfType<MapGenarator>();

        for (int i = 1; i < 64; i++)
        {
            Instantiate(map.transform.GetChild(0).gameObject, new Vector3(), new Quaternion(), FindObjectOfType<MapGenarator>().transform);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                map.transform.GetChild(i * 8 + j).name = "chunk_" + i + "_" + j;
                map.transform.GetChild(i * 8 + j).position = new Vector3(i * 64, 0, j * 64);
            }
        }
    }

    [MenuItem("Utils/Chunks/Instantiate2x2")]
    [ExecuteInEditMode]
    private static void InstatiateChunks2z2()
    {
        var map = FindObjectOfType<MapGenarator>();

        for (int i = 1; i < 4; i++)
        {
            Instantiate(map.transform.GetChild(0).gameObject, new Vector3(), new Quaternion(), FindObjectOfType<MapGenarator>().transform);
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                map.transform.GetChild(i * 2 + j).name = "chunk_" + i + "_" + j;
                map.transform.GetChild(i * 2 + j).position = new Vector3(i * 64, 0, j * 64);
            }
        }
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Utils/Chunks/CreateMeshes8x8")]
    [ExecuteInEditMode]
    private static void CreateMeshes8x8()
    {
        var map = FindObjectOfType<MapGenarator>();

        map.ArrayInit();
        

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                map.Generate64x64(i, j);
            }
        }
    }

    [MenuItem("Utils/Chunks/CreateMeshes2x2")]
    [ExecuteInEditMode]
    private static void CreateMeshes2x2()
    {
        var map = FindObjectOfType<MapGenarator>();

        map.ArrayInit();


        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                map.Generate64x64(i, j);
            }
        }
    }
#endif



#if UNITY_EDITOR
    [MenuItem("Utils/Chunks/LoadMeshes")]
    [ExecuteInEditMode]
    private static void LoadMeshes()
    {
        var map = FindObjectOfType<MapGenarator>();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                map.transform.GetChild(i * 8 + j).GetComponent<MeshFilter>().mesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/Models/ChunkMesh/" + map.transform.GetChild(i * 8 + j).name + ".asset");
                map.transform.GetChild(i * 8 + j).GetComponent<MeshCollider>().sharedMesh = map.transform.GetChild(i * 8 + j).GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
#endif
}
