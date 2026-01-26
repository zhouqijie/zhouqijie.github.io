using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Utils/Update/Stair")]
    public static void UpdateStair()
    {
        //stairs_straight
        var houseToUpdate = FindObjectOfType<StairUpdate>();
        


        for(int i = houseToUpdate.transform.childCount - 1; i > -1; i--)
        {
            if (houseToUpdate.transform.GetChild(i).name.Length >= 15 && houseToUpdate.transform.GetChild(i).name.Substring(0, 15) == "stairs_straight")
            {
                var objToDestory = houseToUpdate.transform.GetChild(i);
                Vector3 pos = objToDestory.position;
                Quaternion rot = objToDestory.rotation;
                DestroyImmediate(objToDestory.gameObject);

                Instantiate(Resources.Load<GameObject>("Prefabs/Disguises/stairs_straight"), pos, rot, houseToUpdate.transform);


            }
        }

        DestroyImmediate(houseToUpdate);
    }
#endif
}
