using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaiDian : MonoBehaviour
{
    public static void Mai(string key1, int value1, string key2, int value2, string name)
    {
        //埋点
        Dictionary<string, int> dic = new Dictionary<string, int>();
        dic[key1] = value1;
        dic[key2] = value2;
        //Debug.Log(key1 + " : " + value1.ToString() + ", " + key2 + " : " + value2.ToString() + ", " + "name: " + name);
        StarkSDKSpace.StarkSDK.API.ReportAnalytics(name, dic);
        //。。。
    }
}
