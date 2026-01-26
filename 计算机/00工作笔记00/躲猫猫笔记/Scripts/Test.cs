using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Test
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Utils/ConvertTexture")]
    public static void ExeConvert()
    {
        int mipCount = 1;

        Texture2D rawTex = Resources.Load<Texture2D>("Textures/atlas");
        Texture2D newTex = new Texture2D(rawTex.width, rawTex.height, TextureFormat.ETC2_RGBA8, mipCount, false);

        for (var i = 0; i < mipCount; i++)
        {
                Graphics.CopyTexture(rawTex, 0, i, newTex, 0, i);
        }

        UnityEditor.AssetDatabase.CreateAsset(newTex, "Assets/Resources/Textures/AtlasNew.asset");
        UnityEditor.AssetDatabase.SaveAssets();

        AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Textures/AtlasNew.asset");
    }
#endif
}
