# Unity的GameObject类    

<br />
<br />


## GameObject创建    

> CRE：创建即加入场景？？   
> 
```C++
    => CreateGameObject()      
        => GameObject &go = *NEW_OBJECT (GameObject);
        => ActivateGameObject (go, name);
                => GameObject::Reset ()  
                => GameObject::SetName ()  
                => GameObject::AwakeFromLoad ()  
                => GameObject::Activate ()  
                    => UpdateActiveGONode()  
                        => Cre：移除或者加入GameObjectManager的GONode列表。    
```



<br />
<br />


## GameObject列表管理    

貌似没有管理GO的Scene或World类。    

貌似是使用GameObjectManager单例管理。    
貌似只管理TaggedGameObjects和ActiveGameObjects？不管理Inactive的GameObject？    


<br />
<br />

## AddComponent方法    

> Unity很多地方使用ImmediatePtr<>作为指针。    
> 在GameObject.h和GameObjectUtility.h中有不同定义。  
> Mono脚本很多地方需要特殊处理。    

**`AddComponent(GameObject&, ClassID...)`**      

- 定义`std::set<ScriptingClassPtr> process`作为参数。    
- 调用以下函数。    

**`AddComponentInterval(Gameobject&, ClassID, MonoScriptPtr, std::set<ScriptingClassPtr> &...)`**    

- 是否继承自Component ?  
- 是否存在冲突Component？  
- 是否存在相同组件并且不允许相同组件？  
- ？？？导入模型不允许添加Component？？？？    
- 先添加该Component需求的Component。  
- 不允许添加抽象组件。    
- 调用以下函数。    

**`AddComponentUnchecked (GameObject& go, int classID, MonoScriptPtr script...)`**    

- 工厂模式生产对象：`Object::Produce (classID)`    
- 调用`GameObject.AddComponentInternal (Component*)`方法添加组件到GameObject。  
- 对Mono脚本做特殊处理。    
- 其他处理例如SetDirty等。    
- 事件回调。    

**`GameObject.AddComponentInternal (Component*)`**    

- 向GameObject的m_Component容器添加(ClassId, ImmediatePtr<Component>)对。    
- 确认这个组件没有被添加到其他的GameObject。      
- 将Component的gameobjct指针指向本GameObject。    
- 其他操作例如SetDirty()、消息等。。。    







