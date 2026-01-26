# AssetImporter类    

/// An AssetImporter is used by the AssetDatabase to convert an asset file (textures/ sound/ meshes etc.) into 
/// an Object that can be serialized, like converting a jpg texture file into a Texture2D class, which can then 
/// be made persistent and serialized to disk. The Texture2D class should not know about how to load from assets.

//CRE:AssetImporter被AssetDataBase用于把一个资产文件（texture/sound/mesh etc）转换为一个可序列化的Object。  
//比如转换一个jpg纹理到Texture2D类型。Texture2D可以序列化到磁盘，Texture2D文件不应该直到如何从资产中加载。     



/// void GenerateAssetData has to be overridden in order to convert the asset file to the serializable object.
/// You can get the AssetsPathName that should be loaded by using: GetCompleteAssetPathName ()
/// Inside GenerateAssetData you have to create the object, load it 
/// somehow from the pathname and add the instanceID of the the created objects to objectsLoadedFromAsset
/// If you generate datatemplates you have to do the same for the datatemplates

//CRE:
//为了将资产文件转换为可序列化对象，必须重写void GenerateAssetData。
//可以使用GetCompleteAssetPathName（）获取应该加载的AssetPathName
//在GenerateAssetData中，您必须创建对象,从路径名加载它
//并将创建的对象的instanceID添加到objectsLoadedFromAsst
//如果生成数据模板，则必须对数据模板执行相同的操作。  



（END）  