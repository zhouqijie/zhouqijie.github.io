# AssetBundle管理    

### 设置与获取AssetBundle名称  

为指定资源路径设置AssetBundle名称和变体。  
`AssetDatabase.SetAssetBundleNameAndVariant(string assetPath, string name, string variant)`    

获取资源所属的AssetBundle名称（若已分配）。  
`string bundleName = AssetDatabase.GetImplicitAssetBundleName(string assetPath)`  

获取资源所属的AssetBundle变体名称（若已分配）。  
`string variantName = AssetDatabase.GetImplicitAssetBundleVariantName(string assetPath)`  

### 管理AssetBundle名称列表  

获取所有已注册的AssetBundle名称（包括变体）。  
`string[] bundleNames = AssetDatabase.GetAssetBundleNames()`  

移除指定的AssetBundle名称，并清除其下所有资源的关联。  
`bool success = AssetDatabase.RemoveAssetBundleName(string name, bool forceRemove)`  

清理未被任何资源引用的AssetBundle名称。  
`AssetDatabase.RemoveUnusedAssetBundleNames()`  

### 依赖关系查询  

获取某个资源的所有依赖资源路径（常用于分析AssetBundle打包内容）。  
`string[] dependencies = AssetDatabase.GetDependencies(string path, bool recursive)`  


(END)  