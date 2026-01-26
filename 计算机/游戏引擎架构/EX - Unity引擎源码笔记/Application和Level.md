
# Unity的Application类以及Level卸载/载入    

> Mono Application.LoadLevel就是C++ Application.LoadScene?    

## 游戏主循环    

```CPP  
=> WINAPI WinMain  
    => MainMessageLoop()  
        => while(!isQuitSignal)
            => Application::TickTimer()    
                => Application::UpdateSceneIfNeed()
                    => Application::UpdateScene()  
                        =>| TimeManager::SetTime()  
                        =>| GfxDevice::...
                        =>| if(updateworld ) 
                                        =>| UpdateScreenManagerAndInput()   
                                        =>| PlayerLoop()   
                                                => RenderManager::UpdateAllRenderers();
                                                => if !UNITY_EDITOR  
                                                        => if(!batchmode)PlayerRender()//player renderer
                                                            => RenderManager::RenderOffscreenCameras();
	                                                        => RenderManager::RenderCameras();
                        => |if(!updateworld) RenderManager::UpdateAllRenderers() (!updateworld)

```  

## App::NewScene  

```C++  
=> Application::NewScene    
    => DestroyWorld(bool destroySceneAssets)    
        => ...  
    => CreateWorldEditor()    
    => 创建MainCamera    
    => SceneTracker相关（编辑器的类？）      
    => UpdateScene()    
```


## App::LoadScene      

```C++  
=> Application::OpenScene  
    => Application::LoadSceneInternal    
        => SceneTracker相关（编辑器的类？）      
        => DestroyWorld(bool destroySceneAssets)    
            => if (true) CollectAllSceneObjects()    
                            => Object::FindAllDerivedObjects (ClassID (EditorExtension), &objects);    
                                => 查找 Object::IDToPointerMap
            => if (false) CollectSceneGameObjects()    
                            => Object::FindObjectsOfType (&gameObjects);    
                                => 查找 Object::IDToPointerMap
        
        => LoadSceneEditor(...)  
            =>ProloadLevelOperation::LoadLevel() //异步加载场景  
                =>PreloadManager::AddToQueue (PreloadManagerOperation* op);    
                            => 一连串操作 调用了 SerializedFile::ReadObject

```