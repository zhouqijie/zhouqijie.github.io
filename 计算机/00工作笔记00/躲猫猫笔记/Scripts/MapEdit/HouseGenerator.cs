using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class HouseGenerator : MonoBehaviour
{
    public string houseName;
    public string paletteName;
    public bool hasCeiling;
    public bool hasFurnitures;
    

    public Vector4[] culls;


    private int[,,] blockArr;
    private int[,,] rotationArr;

    private List<PaletteEle> palettes = null;


    private bool IsInBound(int x, int y, int z)
    {
        if(x < 0 || y < 0 || z < 0 || x > blockArr.GetLength(0) - 1 || y > blockArr.GetLength(1) - 1 || z > blockArr.GetLength(2) - 1)
        {
            return false;
        }
        return true;
    }

    private int GetVoxel(int x, int y, int z)
    {
        if (!IsInBound(x, y, z)) return -1;
        
            return blockArr[x, y, z];
    }

    private bool HasCube(int x, int y, int z)
    {
        if (!IsInBound(x, y, z)) return false;

        if (blockArr[x, y, z] != -1 && palettes[blockArr[x, y, z]].isCube == true)
        {
             return true;
        }
        else
        {
            return false;
        }
    }








#if UNITY_EDITOR
    [ExecuteInEditMode]
    [MenuItem("Utils/House/GenerateAndSaveMesh")]
    public static void GenerateHouse()
    {
        var house = FindObjectOfType<HouseGenerator>();

        house.palettes = EditorInfoManager.GetInstance().GetPalette(house.paletteName);

        var roomInfo = EditorInfoManager.GetInstance().GetRoomInfo(house.houseName);

        if(roomInfo == null)
        {
            Debug.Log("Error");
            return;
        }

        //Arr Init
        house.blockArr = new int[roomInfo.size.x, roomInfo.size.y, roomInfo.size.z];
        for (int x = 0; x < house.blockArr.GetLength(0); x++)
        {
            for (int y = 0; y < house.blockArr.GetLength(1); y++)
            {
                for (int z = 0; z < house.blockArr.GetLength(2); z++)
                {
                    house.blockArr[x, y, z] = -1;
                }
            }
        }



        foreach (var floor in roomInfo.layout.floor)
        {
            house.blockArr[floor.x, floor.y, floor.z] = floor.id;
        }
        foreach (var stair in roomInfo.layout.stairs)
        {
            if (!house.IsInBound(stair.x, stair.y, stair.z)) continue;
            house.blockArr[stair.x, stair.y, stair.z] = stair.id;
        }
        foreach (var door in roomInfo.layout.doors)
        {
            house.blockArr[door.x, door.y, door.z] = door.id;
        }
        foreach (var ourWall in roomInfo.layout.outerWalls)
        {
            house.blockArr[ourWall.x, ourWall.y, ourWall.z] = ourWall.id;
        }
        foreach (var innerWall in roomInfo.layout.innerWalls)
        {
            house.blockArr[innerWall.x, innerWall.y, innerWall.z] = innerWall.id;
        }
        if (house.hasCeiling)//天花板
        {
            foreach (var ceiling in roomInfo.layout.ceiling)
            {
                house.blockArr[ceiling.x, ceiling.y, ceiling.z] = ceiling.id;
            }
        }
        if (house.hasFurnitures)//家具
        {
            foreach (var fur in roomInfo.layout.furniture)
            {
                house.blockArr[fur.x, fur.y, fur.z] = fur.id;
            }
        }


        //=============Cull npc/cow/paint===========================
        for(int i = 0; i < house.blockArr.GetLength(0); i ++)
        {
            for (int j= 0; j < house.blockArr.GetLength(1); j++)
            {
                for (int k = 0; k < house.blockArr.GetLength(2); k++)
                {
                    if (house.blockArr[i, j, k] == house.palettes.FindIndex(p => p.eleName == "npc") || house.blockArr[i, j, k] == house.palettes.FindIndex(p => p.eleName == "cow") || house.blockArr[i, j, k] == house.palettes.FindIndex(p => p.eleName == "painting"))
                    {
                        house.blockArr[i, j, k] = -1;
                    }
                }
            }
        }


        //============CULL=======================
        foreach (Vector4 vec4 in house.culls)
        {
            int x = Mathf.RoundToInt(vec4.x);
            int z = Mathf.RoundToInt(vec4.y);
            int y1 = Mathf.RoundToInt(vec4.z);
            int y2 = Mathf.RoundToInt(vec4.w);

            for (int i = y1; i < y2; i++)
            {
                house.blockArr[x, i, z] = -1;
            }
        }

        //=======================================




        //Generate Mesh
        MeshData meshData = new MeshData();
        int tmp = 0;
        for(int x = 0; x < house.blockArr.GetLength(0); x++)
        {
            for (int y = 0; y < house.blockArr.GetLength(1); y++)
            {
                for (int z = 0; z < house.blockArr.GetLength(2); z++)
                {

                    //skip none cube
                    if (house.GetVoxel(x, y, z) < 0) continue;

                    //Get Id
                    int id = house.blockArr[x, y, z];

                    //Generate Mesh
                    if (!house.HasCube(x, y, z)) continue;
                    

                    //Top
                    if(!house.HasCube(x, y + 1, z))
                    {

                        meshData.vertices.Add(new Vector3(x, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z));
                        meshData.vertices.Add(new Vector3(x, y + 1, z));

                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));
                        meshData.normals.Add(new Vector3(0, 1, 0));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvTop.x, house.palettes[id].uvTop.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }
                    //Down
                    if (!house.HasCube(x, y - 1, z))
                    {

                        meshData.vertices.Add(new Vector3(x, y, z));
                        meshData.vertices.Add(new Vector3(x + 1, y, z));
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));
                        meshData.vertices.Add(new Vector3(x, y, z + 1));

                        meshData.normals.Add(new Vector3(0, -1, 0));
                        meshData.normals.Add(new Vector3(0, -1, 0));
                        meshData.normals.Add(new Vector3(0, -1, 0));
                        meshData.normals.Add(new Vector3(0, -1, 0));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvTop.x, house.palettes[id].uvTop.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }

                    //Back
                    if (!house.HasCube(x, y, z - 1))
                    {

                        meshData.vertices.Add(new Vector3(x, y + 1, z));
                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z));
                        meshData.vertices.Add(new Vector3(x + 1, y, z));
                        meshData.vertices.Add(new Vector3(x, y, z));

                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));
                        meshData.normals.Add(new Vector3(0, 0, -1));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvSide.x, house.palettes[id].uvSide.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }
                    //Forward
                    if (!house.HasCube(x, y, z + 1))
                    {

                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x, y, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));

                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));
                        meshData.normals.Add(new Vector3(0, 0, 1));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvSide.x, house.palettes[id].uvSide.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }
                    //Left
                    if (!house.HasCube(x - 1, y, z))
                    {

                        meshData.vertices.Add(new Vector3(x, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x, y + 1, z));
                        meshData.vertices.Add(new Vector3(x, y, z));
                        meshData.vertices.Add(new Vector3(x, y, z + 1));

                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));
                        meshData.normals.Add(new Vector3(-1, 0, 0));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvSide.x, house.palettes[id].uvSide.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 1);
                        meshData.triangles.Add(tmp + 2);

                        meshData.triangles.Add(tmp);
                        meshData.triangles.Add(tmp + 2);
                        meshData.triangles.Add(tmp + 3);

                        tmp += 4;
                    }
                    //Right
                    if (!house.HasCube(x + 1, y, z))
                    {

                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z));
                        meshData.vertices.Add(new Vector3(x + 1, y + 1, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y, z + 1));
                        meshData.vertices.Add(new Vector3(x + 1, y, z));

                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));
                        meshData.normals.Add(new Vector3(1, 0, 0));

                        Vector2[] uvs = MeshData.GetFloatUV(house.palettes[id].uvSide.x, house.palettes[id].uvSide.y);
                        meshData.texcoords.Add(uvs[0]);
                        meshData.texcoords.Add(uvs[1]);
                        meshData.texcoords.Add(uvs[2]);
                        meshData.texcoords.Add(uvs[3]);

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

        Mesh mesh = new Mesh(); mesh.name = roomInfo.name;
        mesh.vertices = meshData.vertices.ToArray();
        mesh.normals = meshData.normals.ToArray();
        mesh.uv = meshData.texcoords.ToArray();
        mesh.triangles = meshData.triangles.ToArray();

        AssetDatabase.CreateAsset(mesh, "Assets/Models/HouseMesh/" + house.houseName + ".asset");
        AssetDatabase.SaveAssets();
    }


#endif

#if UNITY_EDITOR
    [ExecuteInEditMode]
    [MenuItem("Utils/House/GeneratePrefabs")]
    public static void GeneratePrefabs()
    {
        var house = FindObjectOfType<HouseGenerator>();
        house.palettes = EditorInfoManager.GetInstance().GetPalette(house.paletteName);

        var roomInfo = EditorInfoManager.GetInstance().GetRoomInfo(house.houseName);

        if (roomInfo == null)
        {
            Debug.Log("Error");
            return;
        }

        //Arr Init
        house.blockArr = new int[roomInfo.size.x, roomInfo.size.y, roomInfo.size.z];
        house.rotationArr = new int[roomInfo.size.x, roomInfo.size.y, roomInfo.size.z];

        for (int x = 0; x < house.blockArr.GetLength(0); x++)
        {
            for (int y = 0; y < house.blockArr.GetLength(1); y++)
            {
                for (int z = 0; z < house.blockArr.GetLength(2); z++)
                {
                    house.blockArr[x, y, z] = -1;
                    house.rotationArr[x, y, z] = 0;
                }
            }
        }
        foreach (var ourWall in roomInfo.layout.outerWalls)
        {
            house.blockArr[ourWall.x, ourWall.y, ourWall.z] = ourWall.id;
            house.rotationArr[ourWall.x, ourWall.y, ourWall.z] = ourWall.d;
        }
        foreach (var innerWall in roomInfo.layout.innerWalls)
        {
            house.blockArr[innerWall.x, innerWall.y, innerWall.z] = innerWall.id;
            house.rotationArr[innerWall.x, innerWall.y, innerWall.z] = innerWall.d;
        }
        foreach (var stair in roomInfo.layout.stairs)
        {
            if (!house.IsInBound(stair.x, stair.y, stair.z)) continue;
            house.blockArr[stair.x, stair.y, stair.z] = stair.id;
            house.rotationArr[stair.x, stair.y, stair.z] = stair.d;
        }
        foreach (var door in roomInfo.layout.doors)
        {
            house.blockArr[door.x, door.y, door.z] = door.id;
            house.rotationArr[door.x, door.y, door.z] = door.d;
        }
        foreach (var fur in roomInfo.layout.furniture)
        {
            house.blockArr[fur.x, fur.y, fur.z] = fur.id;
            house.rotationArr[fur.x, fur.y, fur.z] = fur.d;
        }
        if (house.hasCeiling)
        {
            foreach (var ceil in roomInfo.layout.ceiling)
            {
                if (!house.IsInBound(ceil.x, ceil.y, ceil.z)) continue;
                house.blockArr[ceil.x, ceil.y, ceil.z] = ceil.id;
                house.rotationArr[ceil.x, ceil.y, ceil.z] = ceil.d;
            }
        }


        //Generate Mesh
        MeshData meshData = new MeshData();
        int tmp = 0;
        for (int x = 0; x < house.blockArr.GetLength(0); x++)
        {
            for (int y = 0; y < house.blockArr.GetLength(1); y++)
            {
                for (int z = 0; z < house.blockArr.GetLength(2); z++)
                {

                    //skip none cube
                    if (house.GetVoxel(x, y, z) < 0) continue;

                    //Get Id
                    int id = house.blockArr[x, y, z];

                    //Instantiate Prefabs
                    if (!house.palettes[id].isCube && house.palettes[id].prefabName != null)
                    {
                        string prefabName = house.palettes[id].prefabName;
                        GameObject prefab = Resources.Load<GameObject>("Prefabs/Disguises/" + prefabName);
                        if(prefab != null) Instantiate(prefab, house.transform.position + new Vector3(x + 0.5f, y, z + 0.5f), Quaternion.Euler(0, house.rotationArr[x, y, z], 0), house.transform);
                    }
                }
            }
        }
    }
#endif
}





#if UNITY_EDITOR

[CustomEditor(typeof(HouseGenerator))]
public class HouseGeneratorEditor: Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("测试"))
        {
            Debug.Log("测试");
        }
    }
}

#endif
