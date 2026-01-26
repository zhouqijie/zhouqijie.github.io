# Renderer类    

## Renderer::Render()  

纯虚函数。在不同的子类中实现。  

## Renderers **更新** 调用层次结构      

```CPP  
=> Application::TickTimer()
    => Application::UpdateSceneIfNeed()
        => Application::UpdateScene()  
            => RenderManager::UpdateAllRenderers()
                => Renderer::UpdateAllRenderersInternal()  
                    => for(;;) renderer->Renderer::UpdateRenderer()  
                                    => if(shouldBeInScene)
                                        => if(!isInScene) GetScene().AddRenderer(this)
                                        => UpdateSceneHandle()
                                    => else 
                                        => 从场景移除    
```

## Renderer::UpdateRenderer()    

源码：  
```CPP  
if (ShouldBeInScene ())
{
	if (!IsInScene())
	{
		m_SceneHandle = GetScene().AddRenderer (this);
	}
	Assert (m_SceneHandle != kInvalidSceneHandle);
	UpdateSceneHandle ();
}
else
{
	// This should not be necessary but fixes a weird bug where a mesh is
	// being leaked when disabled before loading a scene. Happens in the fps tutorial.
	RemoveFromScene ();
}
```  


## 以MeshRenderer为例        

```CPP
MeshRenderer::Render(int subsetindex, const ChannelAssigns& channels)//subsetIndex是子网格的index    
    DrawUtil::DrawMeshRaw (channels, *mesh, subsetIndex);
        VBO* vbo = mesh.GetSharedVBO (channels.GetSourceMap());//共享VBO？？
        DrawUtil::DrawVBOMeshRaw (*vbo, mesh, channels, subsetIndex);
                SubMesh& submesh = mesh.GetSubMeshFast(subsetIndex);//子网格  
                vbo.DrawVBO (channels, submesh.firstByte, submesh.indexCount, submesh.topology, firstVertex, vertexCount);//纯虚函数
                    =>派生D3D9VBO.DrawVBO
                    =>派生D3D11VBO.DrawVBO
                    =>派生GLESVBO.DrawVBO
                        GLESVBO.DrawInternal
                            //...
                            GLES_CHK(glDrawElements(gltopo, indexCount, GL_UNSIGNED_SHORT, indices));
                            //...
```


（END）  