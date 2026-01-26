


# CRE：渲染总结概览      


```CPP  
=>Application::UpdateScene()

    => if (updateWorld) PlayerLoop() // player loop 
        => RenderManager::UpdateAllRenderers();
        => if !UNITY_EDITOR  
                => if(!batchmode)PlayerRender()//player renderer
                    => RenderManager::RenderOffscreenCameras();//所有离屏相机渲染
	                => RenderManager::RenderCameras();//所有相机渲染
```

# Camera的渲染    

```CPP
            =>Camera.DoRender()

                =>DoRendererLoop(RenderLoop& loop,  RenderingPath renderPath,  CullResults& contents,  bool dontRenderRenderables)   
                  //其中loop是Camera的成员RenderLoop* m_RenderLoop;  
                        =>⭕foreach(node in VisibleNodes)  //遍历所有Visible节点，加入到loop.m_Objects中    
                                foreach(mat)
                                    //fill rodata  

                        =>⭕if(renderPath == kRenderPathPrePass)  
                            DoPrePassRenderLoop()
                        =>⭕else if() 
                            DoForwardShaderRenderLoop (loop.m_Context, loop.m_Objects[kPartOpaque],  ...);//Opaque
                        =>⭕else if()
                             DoForwardShaderRenderLoop (loop.m_Context, loop.m_Objects[kPartAfterOpaque],  ...);//AfterOpaque
                                    =>ForwardVertexRenderLoop queue;//定义局部对象queue(ForwardShaderRenderLoop不继承RenderLoop)  
                                    =>设置queue.m_Objects(RenderObjectDataContainer)  
                                    => queue.PerformRendering()

                                        =>for(shadowmap in m_ShadowMaps)//所有阴影贴图 m_ShadowMaps    
                                            RenderLightShadowMaps(....);//渲染阴影贴图（使用casterBounds、Cascades等因素剔除）

                                        =>for(pass)//遍历所有pass  
                                            =>RenderPassData& rpData = m_PlainRenderPasses[i];//Pass数据  
                                            =>RenderObjectData& roDataH = (*m_Objects)[rpData.roIndex];//pass对应的渲染对象    
                                                    
                                            =>VisibleNode * node = roDataH.visibleNode;  //每个pass只渲染一个node？？

                                            => if(xxxx) m_BatchRenderer.Add(node->renderer, subsetindex, channels, node->worldMatrix, rs.transformType);//使用批处理器添加渲染对象。
                                            => if(xxx) renderer-Render() //渲染那些不适合批处理的对象。
```                                                                  
## 剔除和几何排序流程概述        

所有Renderers -> CullResults -> VisibleNodes -> Passes -> foreach(pass.node){直接Render/加入Batch}


## 几种渲染路径    

```CPP
enum RenderingPath {
	kRenderPathVertex = 0, //对应Vertext Lit
	kRenderPathForward,//对应前向渲染  
	kRenderPathPrePass,//对应延迟渲染
	kRenderPathCount
};
```


## 阴影渲染    

DoPrePass中或者DoForwardShaderRenderLoop都有渲染阴影的调用。      
如果是kRenderPathPrePass（延迟渲染），就会在PrePass中渲染阴影。    



## 相关类定义        

```CPP
class：
CullResults //剔除结果
     VisibleNodes nodes;// VisibleNode的动态数组    
     ...

class:
RenderLoop//渲染循环
    RenderObjectDataContainer m_Objects[kPartCount];// 可能是真正的渲染队列   kPartCount有Opaque和AfterOpaque两种    
    ...
```


```CPP
struct VisibleNode : public TransformInfo
{
	void SetTransformInfo(const TransformInfo& src) { TransformInfo::operator=(src); }
	BaseRenderer* renderer;
	float         lodFade;
};
struct RenderObjectData {
	Unity::Material*	material;	// 4
	SInt16		queueIndex;			// 2
	UInt16		subsetIndex;		// 2
	SInt16		subShaderIndex;		// 2
	UInt16		sourceMaterialIndex;// 2
	UInt16		lightmapIndex;		// 2
	int			staticBatchIndex;	// 4
	float		distance;			// 4

	//@TODO: cold?
	float		 distanceAlongView;	// 4
	VisibleNode* visibleNode;		// 4
	Shader*		shader;				// 4	shader to use
	GlobalLayeringData
				globalLayeringData; // 4
	// 36 bytes
};
```






## 如何选择要渲染的Renderer？    


？？Camera::CustomCull ？？
    ？？PrepareSceneCullingParameters    >>   ？？SceneCullParameters？？实际的场景Renderer选取??  
            sceneCullParameters.renderers[kStaticRenderers].nodes = GetScene().GetStaticSceneNodes();
            sceneCullParameters.renderers[kCameraIntermediate].nodes = cameraIntermediate.GetSceneNodes();
                返回Scene::m_RendererNodes.begin()  
    ？？CullScene？？
            ？？PrepareSceneNodes？？填充VisObjs??  




## Scene.m_RendererNodes来自何处？    

Scene::AddRendererInternal(Renderer*)  

    Scene::AddRenderer  

        Renderer::UpdateRenderer ()


## 几何排序    

```CPP

//构造RenderLoop.m_Objects
void DoRenderLoop()//在DoRenderLoop中  
{
    //...
    foreach(it in cullresults.nodes)
    {
        //...
        foreach(int matidx in matCount)
        {
            //...
            RenderObjectData& odata = loop.m_Objects[part].push_back ();
            odata.xx = xxx;
            odata.xx = xxx;
            odata.xx = xxx;
            //...
        }
        //...
    }
    //...
}

//对passes进行排序  
queue.SortRenderPassData<true> (queue.m_PlainRenderPasses);

//排序函数  
	template <bool opaque>
	void SortRenderPassData( RenderPasses& passes )
	{
		RenderObjectSorter<opaque> sorter;
		sorter.queue = this;
		std::sort( passes.begin(), passes.end(), sorter );
	}

//排序器  
	template <bool opaque>
	struct RenderObjectSorter
	{
		bool operator()( const RenderPassData& ra, const RenderPassData& rb ) const;
		const ForwardShaderRenderLoop* queue;
	};
```


```CPP
//排序器实现(operator())中条件优先顺序排列：

layer和order  
//全局layer和order    

首通道标志 (kPackFirstPassFlag)：
//优先处理首通道，这对于确保依赖于特定通道顺序的渲染操作（如阴影或基础光照计算）至关重要。

光照贴图索引 (lightmapIndex)：
//由于相同光照贴图的对象渲染时可以共享资源，按此索引排序有助于减少光照贴图切换，提高渲染效率。

静态批处理索引 (staticBatchIndex)：
//在启用批处理的情况下，按批处理索引排序，使得批处理的对象优先渲染，这样可以大幅度减少绘制调用，优化渲染性能。

材质索引 (sourceMaterialIndex)：
//对于不属于任何批处理的对象，按照材质索引排序，以确保同一材质的对象连续渲染，减少材质切换次数。

着色器实例ID (shader->GetInstanceID())：
//通过按着色器实例排序，减少着色器程序的切换，可以优化GPU资源的使用。

材质实例ID (material->GetInstanceID())：
//类似于着色器，按材质实例排序也是为了减少材质切换，维持渲染管线的连贯性和效率。

渲染通道 (pass)：
//最后，在同一材质和着色器内部，按渲染通道排序，确保同一材质的不同通道按照预定顺序执行，有助于正确实现复杂的材质效果。

```



```CPP
//设置Pass然后渲染：   
				channels = rs.material->SetPassWithShader(rs.passIndex, rs.shader, rs.subshaderIndex);
				if (channels)
				{
					SetupObjectMatrix (node->worldMatrix, rs.transformType);
					node->renderer->Render( subsetIndex, *channels );
				}

//GPT: ChannelAssigns类负责管理顶点数据通道的绑定

//一般包括以下几类信息:
//顶点数据通道：指定如何从顶点缓冲区读取数据，例如位置、法线、纹理坐标、颜色等。
//顶点布局描述：定义了顶点数据的组织和格式，告诉渲染管线如何解释顶点缓存中的数据。
//纹理引用：包含哪些纹理需要被绑定到着色器中使用。
//纹理采样方式：如何对纹理进行采样，包括纹理过滤、包裹模式等。
//Uniforms/常量
//渲染状态信息
//Shader程序引用

class ChannelAssigns {
public:
    // 构造函数
    ChannelAssigns();

    // 从解析的着色器通道配置中设置通道绑定
    void FromParsedChannels (const ShaderLab::ParserBindChannels& parsed);

    // 绑定一个源通道到指定的顶点组件
    void Bind(ShaderChannel source, VertexComponent target);

    // 解除一个顶点组件的绑定
    void Unbind(VertexComponent target);

    // 与另一个ChannelAssigns对象合并通道配置
    void MergeWith(const ChannelAssigns& additional);

    // 获取已配置的目标顶点组件的映射（哪些顶点组件已经被设置了数据源）
    UInt32 GetTargetMap() const;

    // 获取已使用的源通道的映射（哪些数据源通道已经被使用）
    UInt32 GetSourceMap() const;

    // 检查通道分配是否为空（没有任何通道绑定）
    bool IsEmpty() const;

    // 检查所有源通道是否直接映射到对应的目标组件，没有交叉连接（如src.TexCoord0 -> dst.TexCoord1）
    bool IsDirectlyWired() const;

    // 根据目标顶点组件获取其数据源的通道
    ShaderChannel GetSourceForTarget(VertexComponent target) const;

    // 比较两个ChannelAssigns对象是否相等
    bool operator== (const ChannelAssigns& other) const;

private:
    // 重新计算直接连接的状态
    void RecalculateIsDirectlyWired();

private:
    UInt32  m_TargetMap;  // 表示哪些顶点组件已经被设置了数据源的位字段
    UInt32  m_SourceMap;  // 表示哪些数据源通道已经被使用的位字段
    SInt8   m_Channels[kVertexCompCount];  // 每个顶点组件的数据源通道
    bool    m_DirectlyWired;  // 所有源通道是否直接映射到对应的目标组件

    // 序列化时使用的友元结构
    friend struct GfxRet_ChannelAssigns;
};

//VertexComponent枚举  
enum VertexComponent
{
	kVertexCompNone = 0,
	kVertexCompVertex,
	kVertexCompColor,
	kVertexCompNormal,
	kVertexCompTexCoord,
	kVertexCompTexCoord0, kVertexCompTexCoord1, kVertexCompTexCoord2, kVertexCompTexCoord3,
	kVertexCompTexCoord4, kVertexCompTexCoord5, kVertexCompTexCoord6, kVertexCompTexCoord7,
	kVertexCompAttrib0, kVertexCompAttrib1, kVertexCompAttrib2, kVertexCompAttrib3,
	kVertexCompAttrib4, kVertexCompAttrib5, kVertexCompAttrib6, kVertexCompAttrib7,
	kVertexCompAttrib8, kVertexCompAttrib9, kVertexCompAttrib10, kVertexCompAttrib11,
	kVertexCompAttrib12, kVertexCompAttrib13, kVertexCompAttrib14, kVertexCompAttrib15,
	kVertexCompCount // keep this last!
};
enum VertexChannelFormat
{
    kChannelFormatFloat = 0,
    kChannelFormatFloat16,
    kChannelFormatColor,
    kChannelFormatByte,
    kChannelFormatCount
};
```

（END）  