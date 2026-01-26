# Object类    

<br />
<br />


## RTTI & Produce        

- **`REGISTER_DERIVED_CLASS(,)`宏**：    

定义了获取ClassID的方法。  
定义了Produce方法。    
定义了Abstract/Seal等信息。  


- **`RTTI`结构体**：    

```CPP  
	struct RTTI
	{
		RTTI*                    base;// super rtti class
		Object::FactoryFunction* factory;// the factory function of the class
		int                      classID;// the class ID of the class
		std::string              className;// the name of the class
		int                      size;// sizeof (Class)
		bool                     isAbstract;// is the class Abstract?
	};
```


- **`static Object* Produce (int classID, int instanceID = 0, ...)`工厂方法**：      

- mode和instanceId相关assert。    
- RTTIMap中迭代查找工厂方法。    
- 生产对应的Object。      
- 如果传入的instanceId不为0就生成一个InstanceId。    
- 最后对象要插入一个instanceId->Object*的Map。   
- （注意上锁和解锁）    

<br />
<br />

## InstanceID    

> 运行时生成的Object以及编辑器场景添加且未保存的Object是负的InstanceID。    
> 读取载入的Object是正的InstanceID。    

- **Object成员**：  

`SInt32 m_InstanceID;`    
`GetInstanceID();`    
`SetInstanceID(int inID);`    

- **生成方法**：  

`AllocateAndAssignInstanceIDNoLock`（在Produce中调用）    





<br />
<br />

## SetDirty方法    

> Whenever variables that are being serialized in Transfer change, SetDirty () should be called。  
> This will allow tracking of objects that have changed since the last saving to disk or over the network。  
> 每当在Transfer中序列化的变量发生变化时，应调用SetDirty。  
> 这将允许跟踪自上次保存到磁盘或通过网络以来已更改的对象。  


