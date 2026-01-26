using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEditor;

public class Test
{
#if UNITY_EDITOR
    [MenuItem("Utils/Clean UserData")]
    public static void CleanUserdata()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
}
