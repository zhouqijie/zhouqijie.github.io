using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VegetationGenerator : MonoBehaviour
{
    private static VegetationGenerator inst = null;
    public static VegetationGenerator GetInst()
    {
        if (inst == null) inst = FindObjectOfType<VegetationGenerator>();

        return inst;
    }


    private static float density = 0.025f;
    private static string[] plantsArr = new string[] { "grass", "allium", "dandelion", "nether_wart", "oxeye_daisy", "pink_tulip", "poppy" };


    private static RaycastHit hit;



#if UNITY_EDITOR
    [MenuItem("Utils/Vegetation/Genarate")]
    public static void GenerateVegetation()
    {
        for(int i = 0; i < 512; i++)
        {
            for(int j = 0; j < 512; j++)
            {
                if(Random.value < density)
                {
                    if (Physics.Raycast(new Ray(new Vector3(i + 0.5f, 50f, j + 0.5f), -Vector3.up), out hit, 100f))
                    {
                        if (Mathf.RoundToInt(hit.point.y) == 4)
                        {
                            string str;
                            if(Random.value < 0.1f)
                            {
                                str = plantsArr[Random.Range(0, plantsArr.Length)];
                            }
                            else
                            {
                                str = "grass";
                            }
                            GameObject vege = Instantiate(Resources.Load<GameObject>("Prefabs/Disguises/" + str), new Vector3(i + 0.5f, 4, j + 0.5f), new Quaternion(), GetInst().transform);
                            
                        }
                    }
                }
                
            }
        }
    }
#endif

#if UNITY_EDITOR
    [MenuItem("Utils/Vegetation/Clean")]
#endif
    public static void DelVegetation()
    {
        for(int i = GetInst().transform.childCount - 1; i > -1; i--)
        {
            DestroyImmediate(GetInst().transform.GetChild(i).gameObject);
        }
    }
}
