# 外观    

### GUI样式(GUIStyle)  

GUIStyle用于设置控件的外观。    

可以在GUISkin资源中定义每个控件默认的GUIStyle。    

`EditorStyles`有一些预定义的样式。      



### GUI图标    

获取Unity的built-in图标：    
```C#  
public class WindowIconViewer : EditorWindow
{
    private Vector2 scrollPosition;
    private List<Texture2D> icons = new List<Texture2D>();

    [MenuItem("Window/Internal/Built-in Icons Viewer")]
    public static void ShowWindow()
    {
        GetWindow<WindowIconViewer>("Built-in Icons");
    }

    private void OnEnable()
    {
        var method = typeof(EditorGUIUtility).GetMethod("GetEditorAssetBundle", BindingFlags.Static | BindingFlags.NonPublic);
        AssetBundle ab = null;
        if (method != null) 
            ab = method.Invoke(null, null) as AssetBundle;
        
        if (ab != null)
        {
            // 加载所有Texture2D资源
            var allAssets = ab.LoadAllAssets();
            foreach (var asset in allAssets)
            {
                if (asset is Texture2D texture)
                {
                    icons.Add(texture);
                }
            }
        }
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        // 分列显示图标（每行5个）
        int columns = 5;
        int index = 0;
        while (index < icons.Count)
        {
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < columns && index < icons.Count; i++)
            {
                var icon = icons[index];
                // 显示图标和名称
                GUILayout.Box(icon, GUILayout.Width(50), GUILayout.Height(50));
                EditorGUILayout.LabelField(icon.name, GUILayout.Width(100));
                index++;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
```


### GUI动画    

示例：  
```C#
private AnimBool animation;

private void OnEnable()
{
    animation = new AnimBool();
    animation.valueChanged.AddListener(Repaint);
}
public override void OnInspectorGUI()
{
    animation.target = EditorGUILayout.Foldout(animation.target, "展开", true);
    if(EditorGUILayout.BeginFadeGroup(anim.faded))
    {
        GUILayout.Label("--被折叠内容--");
        base.OnInspectorGUI();
    }
    EditorGUILayout.EndFadeGroup();
}
```

（END）    