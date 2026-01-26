# Resources重定向  

CRE：如果有些资源可以随主包发布，也可以放在AssetBundle包中热更新，需要使用同一套逻辑读取，可以使用Resources重定向策略。    


## 示例    

```C#  
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using LitJson;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ResourceBundle
{
    //开关
    public static bool enable = false;


    //AssetBundle包
    public static List<AssetBundle> resAbs = null;

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        if (!enable) { return Resources.Load<T>(path); }
        if (resAbs == null) { return Resources.Load<T>(path); }

        string abName = "resources_" + path.Substring(0, path.LastIndexOf('/')).Replace('/', '$').ToLower() + ".ab";
        string objName = path.Substring(path.LastIndexOf('/') + 1);

        var targetAB = resAbs.FirstOrDefault(ab => ab.name == abName);

        if (targetAB != null && objName != "")
        {
            T result = targetAB.LoadAsset<T>(objName);

            if(result != null)
            {
                if (result is GameObject)
                {
                    ProcessGameObject(result as GameObject);
                }
                return result;
            }
            else
            {
                return Resources.Load<T>(path);
            }
        }
        else
        {
            return Resources.Load<T>(path);
        }
    }





    /// <summary>
    /// 处理游戏对象或预制体
    /// </summary>
    /// <param name="gobj"></param>
    public static void ProcessGameObject(GameObject gobj)
    {
        gobj.GetComponentsInChildren<Renderer>(true).ToList().ForEach(rdr => ResourceBundle.ResetRenderShader(rdr));
        gobj.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true).ToList().ForEach(rdr => ResourceBundle.ResetRenderShader(rdr));
        gobj.GetComponentsInChildren<ParticleSystemRenderer>(true).ToList().ForEach(rdr => ResourceBundle.ResetRenderShader(rdr));


        gobj.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true).ToList().ForEach(txtCom => ResourceBundle.ResetProText(txtCom));
    }



    private static TMPro.TMP_FontAsset fontAsset = null;
    /// <summary>
    /// 处理字体
    /// </summary>
    /// <param name="textCom"></param>
    public static void ResetProText(TMPro.TextMeshProUGUI textCom)
    {
        if (fontAsset == null)
        {
            var hanFont = Resources.Load<TMPro.TMP_FontAsset>("SourceHanSansK-Bold SDF");

            fontAsset = hanFont;
        }
        
        //Debug.Log("----start----");

        //fonts.ToList().ForEach(f => Debug.Log(AssetDatabase.GetAssetPath(f) + " :" +  f.name));

        //Debug.Log("---- end ----");

        textCom.font = fontAsset;
    }


    /// <summary>
    /// 处理Shader
    /// </summary>
    /// <param name="rdrCom"></param>
    public static void ResetRenderShader(Component rdrCom)
    {
        string typeName = (rdrCom is TMPro.TextMeshProUGUI) ? "TMP" : (rdrCom is ParticleSystemRenderer ? "PSRDR" : "RDR");
        switch (typeName)
        {
            case "TMP":
                {
                    if ((rdrCom as TMPro.TextMeshProUGUI).fontSharedMaterial != null && (rdrCom as TMPro.TextMeshProUGUI).fontSharedMaterial.shader != null)
                        (rdrCom as TMPro.TextMeshProUGUI).fontSharedMaterial.shader = Shader.Find((rdrCom as TMPro.TextMeshProUGUI).fontSharedMaterial.shader.name);
                }
                break;
            //case "PSRDR":
            //    {
            //        var prdr = (rdrCom as ParticleSystemRenderer);
            //        if (prdr.sharedMaterial != null && prdr.sharedMaterial.shader != null)
            //            (rdrCom as ParticleSystemRenderer).sharedMaterial.shader = Shader.Find((rdrCom as ParticleSystemRenderer).sharedMaterial.shader.name);

            //        if (prdr.trailMaterial != null && prdr.trailMaterial.shader != null)
            //            (rdrCom as ParticleSystemRenderer).trailMaterial.shader = Shader.Find((rdrCom as ParticleSystemRenderer).trailMaterial.shader.name);
            //    }
            //    break;
            default: //"RDR"
                {
                    Material[] sharematerials = null;
                    string[] shaderNames;

                    sharematerials = (rdrCom as Renderer).sharedMaterials;
                    shaderNames = new string[sharematerials.Length];

                    for (int i = 0; i < sharematerials.Length; i++)
                    {
                        if (sharematerials[i] != null && sharematerials[i].shader != null)
                            shaderNames[i] = sharematerials[i].shader.name;
                        else
                            shaderNames[i] = "";
                    }

                    for (int i = 0; i < sharematerials.Length; i++)
                    {
                        if (sharematerials[i] != null)
                            sharematerials[i].shader = Shader.Find(shaderNames[i]);
                    }
                }
                break;
        }
    }






































#if UNITY_EDITOR

    [MenuItem("资源/选中对象移除所有字体引用")]
    public static void RemoveAllRefOfTextMeshProFont()
    {
        GameObject[] gameobjs = Selection.gameObjects;

        foreach (var gobj in gameobjs)
        {
            foreach (var t in gobj.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
            {
                Debug.Log(t.gameObject.name);
                t.font = null;
            }


            AssetImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(gobj));
            if (importer != null)
            {
                Debug.Log("importer: " + importer.name);
                EditorUtility.SetDirty(gobj);
            }
        }
    }

    [MenuItem("资源/AB打包(先设置标签)")]
    public static void BuildAssetBundles()
    {
#if WECHAT
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.WebGL);
#else
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.Android);
#endif
    }


    [MenuItem("资源/自动设置分包标签")]
    public static void AutoSetAssetBundle()
    {
        Debug.Log("EXIST RES DIR: " + Directory.Exists("Assets/ResourcesBundle"));

        SetDir("Assets/ResourcesBundle");

        AssetDatabase.RemoveUnusedAssetBundleNames();
    }
    public static void SetDir(string dirPath)
    {
        DirectoryInfo dir = new DirectoryInfo(dirPath);
        var files = dir.GetFiles();
        var childDirs = dir.GetDirectories();


        foreach (var f in files)
        {
            //Debug.Log("will get file:" + dir + f.Name);

            AssetImporter importer = AssetImporter.GetAtPath(dirPath + "/" + f.Name);
            if (importer != null)
            {
                string finalName = "resources_" + dirPath.Replace("Assets/ResourcesBundle/", "").Replace("/", "$").Replace(f.Extension, "");
                //Debug.Log("will set: " + finalName);

                importer.assetBundleName = finalName;
                importer.assetBundleVariant = "ab";
            }
        }

        foreach (var childDir in childDirs)
        {
            SetDir(dirPath + "/" + childDir.Name);
        }
    }



    [MenuItem("资源/生成AB包清单Json(先打包)")]
    public static void GenList()
    {
        JsonData jdata = new JsonData();
        JsonData jRequires = jdata["basic"] = new JsonData();

        DirectoryInfo dir = new DirectoryInfo(Application.streamingAssetsPath);

        var filesNeed = dir.GetFiles().Where(f => f.Extension.Contains("ab")).ToArray();

        for (int i = 0; i < filesNeed.Length; ++i)
        {
            jRequires.Add(i);
            jRequires[i] = new JsonData();
            jRequires[i]["file"] = filesNeed[i].Name;
            jRequires[i]["size"] = filesNeed[i].Length;
        }


        string json = JsonMapper.ToJson(jdata);
        System.IO.File.WriteAllText("Assets/Resources/AssetBundles.json", json);

    }



#endif
}

```  