# Object类  

> CRE：`UnityEngine.Object`是所有Unity对象的基类。和C#的基类不同。    

> StackOverflow：派生自 `UnityEngine.Object`的所有类型都只是C++后端中实际底层本机对象的C#层API中的表示形式。因此，直接从`UnityEngine.Object`派生自定义类型是没有意义的，因为后端不会识别该类型。    


> CRE：Object都有一个InstanceID，运行时生成的Object以及编辑器场景添加且未保存的Object是负的InstanceID。（详见Unity源码笔记章节）   