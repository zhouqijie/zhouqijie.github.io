using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisableAllGI
{
    [MenuItem("Utils/Diable All GI Of MR")]
    public static void DisableAllGIFunc()
    {
        MeshRenderer[] mrs = GameObject.FindObjectsOfType<MeshRenderer>();

        foreach(var mr in mrs)
        {
            //mr.receiveGI = ReceiveGI.LightProbes;
        }
    }
}
