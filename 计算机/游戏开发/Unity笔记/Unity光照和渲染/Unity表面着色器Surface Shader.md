
# 一、表面着色器简介：

表面着色器时在顶点/片段着色器上添加的一层抽象。  
把渲染流程划分为表面着色器、光照模型、光照着色器。光照模型是预定义的，光照着色器由系统实现，我们只需关注表面着色器(纹理颜色法线等)，大大了减轻工作量。

# 二、编译指令：

编译指令最重要的作用是指明着色器使用的表面函数和光照函数，并设置一些可选参数。编译指令往往写在CG块的第一句。  
格式：`#pragma surface <surface func> <light model> [optional params]`  

## 表面函数<surface func>：  

表面函数用于定义反射率、光滑度、透明度等表面属性。  
函数格式：`void surf(Input IN, inout <stuct> o)`  
SurfaceOutput、SurfaceOutputStandard、SurfaceOutputStandardSpecular都是Unity内置的结构体，后两个是Unity5加入物理渲染后新增的。  

## 光照函数<light model>：  

光照函数会使用表面函数中设置的各种属性来应用光照模型。  
Unity内置了基于物理的光照模型函数Standard和StandardSpecular,以及Lambert和BlinnPhong。  

自定义光照函数：(Unity手册)  
`half4 Lighting<Name>(SurfaceOutput s, half3 lightDir, half atten)`  
`half4 Lighting<Name>(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)`  


# 三、结构体属性声明  

### Input结构体中的变量：
`float3 viewDir` 视角方向。  
`float4 COLOR` 插值后的逐顶点颜色。  
`float4 screenPos`屏幕空间坐标。  
`float3 worldPos`世界空间位置。  
`float3 worldRefl`世界空间反射。  
`float3 worldNormal`世界空间法线。  

不需要计算这些变量，只需在Input定义中按名称声明它们。  
如果修改了表面法线o.Normal，需要告诉Unity要基于修改后的法线计算世界空间反射方向和法线方向。  

### Output结构体属性：  

SurfaceOutput、SurfaceOutputStandard、SurfaceOutputStandardSpecular的声明可以在Lighting.cginc和UnityPBSLighting.cginc中找到：  
```HLSL
//
struct SurfaceOutput
{
    fixed3 Albedo;      //光源反射率。（纹理采样和颜色属性乘积）
    fixed3 Normal;      //法线方向。
    fixed3 Emission;    //自发光。
    half Specular;      //高光反射指数部分系数。
    fixed Gloss;        //高光反射强度系数。
    fixed Alpha;        //透明通道。
}
//
stuct SurfaceOutputStandard
{
    fixed3 Albedo;
    fixed3 Normal;
    fixed3 Emission;
    half Metallic;
    half Smoothness;
    half Occlusion;
    fixed Alpha;
}//
stuct SurfaceOutputStandardSpecular
{
    fixed3 Albedo;
    fixed3 Specular;
    fixed3 Normal;
    fixed3 Emission;
    half Smoothness;
    half Occlusion;
    fixed Alpha;
}
```


# 四、表面着色器底层原理  

Unity会根据表面着色器生成一个包含很多Pass的顶点/片段着色器。 

![](Images\chapter_17_1.png)  




# 补充：可选参数：

### 透明度和 Alpha 测试：
`alpha` 或 `alpha:auto` - 对于简单的光照函数，将选择淡化透明度（与 `alpha:fade` 相同）；对于基于物理的光照函数，将选择预乘透明度（与 `alpha:premul` 相同）。  
`alpha:blend` - 启用 Alpha 混合。  
`alpha:fade` - 启用传统淡化透明度。  
`alpha:premul` - 启用预乘 Alpha 透明度。  
`alphatest:VariableName` - 启用 Alpha 镂空透明度。剪切值位于具有 VariableName 的浮点变量中。您可能还想使用 addshadow 指令生成正确的阴影投射物通道。  
`keepalpha` - 默认情况下，无论输出结构的 Alpha 输出是什么，或者光照函数返回什么，不透明表面着色器都将 1.0（白色）写入 Alpha 通道。使用此选项可以保持光照函数的 Alpha 值，即使对于不透明的表面着色器也是如此。  
`decal:add` - 附加贴花着色器（例如 terrain AddPass）。这适用于位于其他表面之上并使用附加混合的对象。  
`decal:blend` - 半透明贴花着色器。这适用于位于其他表面之上并使用 Alpha 混合的对象。  

### 自定义修改器函数：
`vertex:VertexFunction` - 自定义顶点修改函数。在生成的顶点着色器的开始处调用此函数，并且此函数可以修改或计算每顶点数据。请参阅表面着色器示例。  
 `finalcolor:ColorFunction` - 自定义最终颜色修改函数。请参阅表面着色器示例。  
`finalgbuffer:ColorFunction` - 用于更改 G 缓冲区内容的自定义延迟路径。  
`finalprepass:ColorFunction` - 自定义预通道基本路径。  

### 阴影和曲面细分：
`addshadow` - 生成阴影投射物通道。常用于自定义的顶点修改，以便阴影投射也可以获得程序化顶点动画。通常情况下，着色器不需要任何特殊的阴影处理，因为它们可以通过回退机制来使用阴影投射物通道。  
`fullforwardshadows` - 支持前向渲染路径中的所有光源阴影类型。默认情况下，着色器仅支持前向渲染中来自一个方向光的阴影（以节省内部着色器变体数量）。如果在前向渲染中需要点光源阴影或聚光灯阴影，请使用此指令。  
`tessellate:TessFunction` - 使用 DX11 GPU 曲面细分；该函数计算曲面细分因子。有关详细信息，请参阅表面着色器曲面细分。  

### 代码生成选项：
`exclude_path:deferred、exclude_path:forward 和 exclude_path:prepass` - 不为给定的渲染路径（分别对应延迟着色路径、前向路径和旧版延迟路径）生成通道。  
`noshadow` - 禁用此着色器中的所有阴影接受支持。  
`noambient` - 不应用任何环境光照或光照探针。  
`novertexlights` - 在前向渲染中不应用任何光照探针或每顶点光源。  
`nolightmap` - 禁用此着色器中的所有光照贴图支持。  
`nodynlightmap` - 禁用此着色器中的运行时动态全局光照支持。  
`nodirlightmap` - 禁用此着色器中的方向光照贴图支持。  
`nofog` - 禁用所有内置雾效支持。  
`nometa` - 不生成“Meta”通道（由光照贴图和动态全局光照用于提取表面信息）。  
`noforwardadd` - 禁用前向渲染附加通道。这会使着色器支持一个完整方向光，所有其他光源均进行每顶点/SH 计算。也能减小着色器。  
`nolppv` - 禁用此着色器中的光照探针代理体支持。  
`noshadowmask` - 为此着色器禁用阴影遮罩支持（包括 Shadowmask 和 Distance Shadowmask）。  

### 其他：
`softvegetation` - 仅在开启 Soft Vegetation 时才渲染表面着色器。  
`interpolateview` - 在顶点着色器中计算视图方向并进行插值；而不是在像素着色器中计算。这可以使像素着色器更快，但会额外消耗一个纹理插值器。  
`halfasview` - 将半方向矢量传入光照函数而不是视图方向。计算半方向并按每个顶点对其进行标准化。这更快，但并不完全正确。  
`approxview` - 在 Unity 5.0 中已删除。请改用 interpolateview。  
`dualforward` - 在前向渲染路径中使用双光照贴图。  
`dithercrossfade` - 使表面着色器支持抖动效果。然后，可将此着色器应用于使用细节级别组 (LOD Group) 组件（配置为交叉淡入淡出过渡模式）的游戏对象。  



# 注意事项：

## 一. 多PASS

在surface shader中，不能用pass关键字。但是最终的shader生成后，会自己生成pass。如果想在Surface shader中写多个pass只需把pass的内容包含到CGPROGRAM/ENDCG之中。
实例：
```
//...
Cull Front
CGPROGRAM
//...
ENDCG
Cull Back
CGPROGRAM
//...
ENDCG
//...
```

## 二. 关键字和可选参数冲突

Surface shader的pragma可选参数可能会覆盖ZWrite ZTest。使用keepalpha可选参数可使ZTest/Zwirte为开启。可以用于由不透明物体的变透明渐出效果。  



