# Direct Rendering(直接渲染)

## Basic Drawing:  

`glDrawArrays()`  
`glDrawElememts()`  

## Multi Drawing:  

常用于相同材质(着色器/着色程序)的批量绘制(batch)。  

`glMultiDrawArrays()`  
`glMultiDrawElements()`  


## Base Index 和 Range：

`glDrawElementsBaseVertex()`  
多个模型存在一个VBO中时，使用`glDrawArrays()`直接指定start index和count即可。但是直接使用`glDrawElements()`有索引问题。

`glDrawRangeElements()`  
也是用于处理多个模型存在一个VBO中的情况的。


## Instancing Drawing:  

`glDrawArraysInstanced()` 多次绘制一个模型。（实例化机制）

## 组合：
※BaseIndex方式可以和其他方式组合。

# Indirect Rendering(间接渲染)

间接命令绘制的缓存对象需要被存储于GL_DRAW_INDIRECT_BUFFER中。  

`glDrawArraysIndirect(GLenum mode, const void*indirect)`  

`glDrawElementsIndirect(GLenum mode, GLenum type, const GLvoid * indirect)`  

>DrawArraysIndiret具有DrawArraysInstanced一样的参数及功能，只不过其参数间接从类型为GL_DRAW_INDIRECT_BUFFER的buffer中读出。  
>DrawElementsIndiret具有DrawElementsInstanceBaseVertex一样的参数及功能，只不过其参数间接从buffer中读出。


# Transform Feedback Rendering

>可以使用Transform Feedback来生成顶点信息用于渲染。通常的数据流程是GPU->CPU->GPU  

>我们总是将顶点数据发送到图形处理器，并且只在帧缓存中生成绘制的像素。如果我们想要在经过顶点着色器或几何着色器之后捕获这些顶点呢?在这一章中，我们将探讨一种方法，即transform feedback。  

`glDrawTransformFeedback()`  
`glDrawTransformFeedbackStream()`  
`glDrawTransformFeedbackInstanced()`  
`glDrawTransformFeedbackStreamInstanced()`  



# 参考

>https://www.bilibili.com/read/cv1823723/  
>https://blog.csdn.net/patient16/article/details/50540011  
>https://blog.csdn.net/u011760195/article/details/100841291  


