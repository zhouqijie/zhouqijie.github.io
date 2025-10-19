# AssetDatabase    

Unity的`AssetDatabase`类是Unity编辑器API中的一个核心工具类，主要用于在编辑器环境下管理和操作项目资源（Assets）。它提供了一系列静态方法，方便开发者通过脚本对资源进行加载、查找、创建、删除、导入、刷新等操作。（仅在编辑器中有效）    


### 常用方法    

通过路径直接加载资源:  
`Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Red.mat")`    

创建新资源:  
`AssetDatabase.CreateAsset(obj, "Assets/Data/NewData.asset")`  

保存所有未保存的资源修改:  
`AssetDatabase.SaveAssets()`  

刷新资源数据库(确保新文件或外部修改被Unity识别):    
`AssetDatabase.Refresh()`    

强制重新导入资源（如修改贴图后重新生成元数据）:    
`AssetDatabase.ImportAsset("Assets/Models/Character.fbx", ImportAssetOptions.ForceUpdate)`    

删除指定路径    
`AssetDatabase.DeleteAsset("Assets/Temp/UnusedFile.prefab")`    

复制资源到新路径:  
`AssetDatabase.CopyAsset("Assets/Scenes/Level1.unity", "Assets/Scenes/Level2.unity")`  

返回符合条件的所有资源的GUID:    
`AssetDatabase.FindAssets("t:Texture2D player_", new[]{"Assets/Art"})`      

获取资源对象的文件路径:  
`string path = AssetDatabase.GetAssetPath(selectedObject)`  

将GUID转换为资源路径:  
`string path = AssetDatabase.GUIDToAssetPath(guid)`  

### 子资源相关       

 - **向主资源对象添加子对象**：    

```C#
// 创建主资源
MyScriptableObject mainAsset = ScriptableObject.CreateInstance<MyScriptableObject>();
AssetDatabase.CreateAsset(mainAsset, "Assets/Data/MainAsset.asset");

// 创建子资源并添加到主资源
Material subMaterial = new Material(Shader.Find("Standard"));
subMaterial.name = "EmbeddedMaterial";
AssetDatabase.AddObjectToAsset(subMaterial, mainAsset); // 添加到主资源中
```


 - **从资源文件中移除指定的 子对象（但不会删除对象本身）**：    

```C#
// 加载包含子资源的 FBX 文件
Object[] assets = AssetDatabase.LoadAllAssetsAtPath("Assets/Models/Character.fbx");
foreach (var asset in assets)
{
    if (asset is Material mat && mat.name == "OldMaterial")
    {
        AssetDatabase.RemoveObjectFromAsset(mat); // 从 FBX 中移除材质
        Object.DestroyImmediate(mat, true);      // 可选：彻底销毁材质
        AssetDatabase.SaveAssets();
        break;
    }
}
```

### 其他方法    

资源类型:  
`IsForeignAsset()`外部资产。    
`IsNativeAsset()`原生资产。  
`IsSubAsset()`子资产。  
`IsMainAsset()`主资产。    

资源内容提取与元数据操作:  
`AssetDatabase.ExtractAsset(embeddedMat, "Assets/Materials/ExtractedMat.mat")`从导入的复合资源（如FBX）中提取子资源（如材质）。    
`Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Models/Character.fbx")`加载路径下所有资源（包括主资源和子资源）。    
`AssetDatabase.SetMainObject(material, "Assets/Models/Character.fbx")`指定资源文件中的主要对象（如下次导入后优先显示的对象）。    

依赖管理:  
`string[] dependencies = AssetDatabase.GetDependencies("Assets/Prefabs/Character.prefab");`获取指定资源的所有依赖项路径（直接和间接）  
`string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/Data/Config.asset")`生成唯一的资源路径，避免覆盖已有文件。    
`string[] subFolders = AssetDatabase.GetSubFolders("Assets/Art")`获取指定文件夹下的所有子目录路径。    

批量操作与导入控制:  
`AssetDatabase.StartAssetEditing()/AssetDatabase.StopAssetEditing()`批量操作数据时，将多次导入合并为一次，提升性能。    
`AssetDatabase.ForceReserializeAssets(new[] { "Assets/Scenes/Level1.unity" })`强制重新序列化资源，确保数据更新到磁盘。  
`AssetDatabase.DisallowAutoRefresh()/AssetDatabase.AllowAutoRefresh()`临时禁用自动刷新，优化大规模操作性能。    

文件系统交互与验证:  
`string error = AssetDatabase.ValidateMoveAsset("Assets/Scenes/OldScene.unity", "Assets/Scenes/New")`检查资源是否可移动（不实际执行操作）。  
`bool isValid = AssetDatabase.IsValidFolder("Assets/Art/Textures")`验证指定路径是否为有效文件夹。  

标签与元数据管理:  
`AssetDatabase.SetLabels(texture, new[] { "Environment", "HighPriority" })`设置标签。  
`string[] labels = AssetDatabase.GetLabels(texture)`获取标签。    


（END）  