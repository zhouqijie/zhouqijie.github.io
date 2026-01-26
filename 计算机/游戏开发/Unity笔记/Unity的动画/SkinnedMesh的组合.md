# SkinnedMesh的组合    

> CRE：为一个骨骼新增蒙皮网格网格，可以使用自定义的组合脚本。  

## 组合器脚本示例    


```C#   
public class MeshCombiner : MonoBehaviour
{
    public SkinnedMeshRenderer defaultSkinnedMesh;

    public void Combine()
    {
        if(this.defaultSkinnedMesh != null)
        {
            foreach (var smr in this.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (smr != null)
                {
                    smr.bones = this.defaultSkinnedMesh.bones;
                    smr.localBounds = this.defaultSkinnedMesh.localBounds;
                }
            }
        }
        else
        {
            Debug.Log("没有设置默认参考皮肤！");
        }
    }
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(MeshCombiner))]
public class CustomMeshCombiner: UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Combine(only active smr)"))
        {
            (this.target as MeshCombiner).Combine();
        }
    }
}
#endif
```  

(END)  