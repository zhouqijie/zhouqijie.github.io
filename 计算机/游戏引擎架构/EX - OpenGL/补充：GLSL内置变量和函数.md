# GLSL变量类型  

## 向量：

注意：向量可以用x、y、z、w/r、g、b、a取分量。

`vec2`二维向量。(`uvec2`、`ivec2`、`fvec2`、`dvec2`)  
`vec3`三维向量。(`uvec3`、`ivec3`、`fvec3`、`dvec3`)  
`vec4`四维向量。(`uvec4`、`ivec4`、`fvec4`、`dvec4`)  

## 矩阵：  

注意：GLSL的矩阵是按列排列存储的。

`mat3`：3x3矩阵。  
`mat?x?`：?x?矩阵。  
`mat4`：4x4矩阵。  

## 纹理/采样器：
`sampler2DShadow`：阴影纹理。  
`samplerCube`：CubeMap纹理。  
`sampler2D`：普通纹理。  

# GLSL内置变量

## 顶点着色器

`gl_Position`: 用于顶点着色器, 写顶点位置。（裁剪空间）

`gl_VertexID`：顶点ID。常用于在顶点着色器中用硬编码的方式定义顶点数据。

## 片段着色器

`gl_FragCoord`:  
gl_FragCoord是片段着色器的输入变量，只读。  
gl_FragCoord是个vec4，四个分量分别对应x, y, z和1/w。其中，x和y是当前片元的窗口相对坐标，不过它们不是整数，小数部分恒为0.5。x - 0.5和y - 0.5分别位于[0, windowWidth - 1]和[0, windowHeight - 1]内。  

# GLSL内置函数  

## 基本函数：

`sin()、cos()、tan()、asin()、acos()、atan()、degrees()、radians()`  

`pow()、exp()、log()、exp2()、log2()、sqrt()`  

`abs()、sign()、floor()、ceil()、fract()、mod()、min()、max()、clamp()、mix()`  

`length()、distance()、dot()、cross()、normalize()、reflect()、faceforward()`  

`any()、all()、not()、greaterThan()、lessThan()、equal()、notEqual()`

## 采样函数：  

`texture()`采样普通纹理，用`vec2`类型采样，返回`vec4`类型。  

`texture()`采样CubeMap纹理，用`vec3`类型采样，返回`vec4`类型。 

`texturnProj()`采样阴影纹理，用`vec4`类型采样，返回`float`类型。（除以最后一个分量w后采样）  