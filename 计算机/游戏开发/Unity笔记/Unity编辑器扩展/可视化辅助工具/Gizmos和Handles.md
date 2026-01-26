# Gizmos    

### 绘制Gizmos的方法    

绘制Gizmos的MonoBehaviour函数`OnDrawGizmox()`、`OnDrawGizmosSelected()`。仅在编辑器模式生效。    

为指定类型的Component绘制Gizmos：`DrawGizmoAttribute`。    


### Gizmos类方法    

`Gizmos.DrawRay()`  
`Gizmos.DrawLine()`  
`Gizmos.DrawCube()`  
`Gizmos.DrawFrustum()`  
`Gizmos.DrawMesh()`  
`Gizmos.DrawWireMesh()`  
`Gizmos.DrawIcon()`  
`Gizmos.DrawGUITexture()`  




<br />
<br />
<br />
<br />

# Scene窗口和Handles      

Handles类通常用于在Scene窗口绘制可交互的图形。    
OnSceneGUI方法用于绘制Scene窗口上的元素。  

### Handles类方法    

`Handles.Label()`  
`Handles.DrawLine()`    
`Handles.DrawWireCube()`    
`Handles.DrawWireDisc()`、`Handles.DrawSolidDisc()`  
`Handles.DrawWireArc()`、`Handles.DrawSolidArc()`  


`Handles.PositionHandle()`  
`Handles.RotationHandle()`  
`Handles.ScaleHandle()`  
`Handles.RadiusHandle()`  

`Handles.XXXHandleCap()`  

### 其他常用方法    

除了Handles，也可以用Graphics绘制：    
`Graphics.DrawMesh(Resources.GetBuiltinResource<Mesh>("Cube.fbx"), matrix, mat, 0);`  

重绘Scene窗口:  
`SceneView.currentDrawingSceneView.Repaint();`  

Gameobject坐标转Scene窗口屏幕坐标:
`HandleUtility.GUIPointToWorldRay(MousePos);`    

射线检测:  
`Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);`    

场景相机  
`SceneView.lastActiveSceneView.camera`  

设置脏标记：  
`EditorUtility.SetDirty();`    

禁止scene窗口拖动时框选  
`HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));`  

### GameObject可视和可选设置  

```C#  
SceneVisibilityManager.instance.EnablePicking();
SceneVisibilityManager.instance.Show();
```


### SceneView的常驻UI绘制      

```C#  
public class PersistantSceneUI 
{
    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethod()
    {
        SceneView.onSceneGUIDelegate = delegate (SceneView sceneView)
        {
            Handles.BeginGUI();

            GUI.Label(new Rect(0f, 0f, 50f, 15f), "标题");
            GUI.Button(new Rect(0f, 20f, 50f, 50f),
                AssetDatabase.LoadAssetAtPath<Texture>("Assets/unity.png"));


            Handles.EndGUI();
        };   
    }
}
```  

### 补充：获取当前事件    

```C#  
Event.current.type == EventType.MouseDown  //鼠标按下事件  
Event.current.type == EventType.MouseDrag  //鼠标拖动事件  
Event.current.type == EventType.MouseUp  //鼠标松开事件  
Event.current.type == EventType.MouseMove  //鼠标移动事件
Event.current.type == EventType.Repaint  //重绘事件  

```  



# 参考文章    

> https://zhuanlan.zhihu.com/p/548571466?utm_id=0  

(END)  