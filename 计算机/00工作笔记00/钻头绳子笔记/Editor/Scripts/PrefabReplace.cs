using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabReplace
{
    [MenuItem("Utils/ReplaceGlassSelect")]
    public static void ReplaceGlassSelected()
    {
        GameObject[] gameObjs = Selection.gameObjects;

        foreach(var obj in gameObjs)
        {
            //var newObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Items/glass"), obj.transform.position, obj.transform.rotation, obj.transform.parent);
            var newObj = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>("Prefabs/Items/glass"), obj.transform.parent) as GameObject;
            newObj.transform.position = obj.transform.position;
            newObj.transform.rotation = obj.transform.rotation;
            newObj.transform.localScale = obj.transform.localScale;

        }

        for(int i = gameObjs.Length - 1; i > -1; i--)
        {
            GameObject.DestroyImmediate(gameObjs[i]);
        }
    }

    
}
