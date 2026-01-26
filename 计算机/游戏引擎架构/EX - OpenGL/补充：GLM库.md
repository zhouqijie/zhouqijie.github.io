# GLM库中的变量类型    

> 右手坐标系。    
> XYZ旋转次序。    

## 向量：  

`vec2`  
`vec3`  
`vec4`  

## 矩阵：  
`mat3`  
`mat4`  

## 四元数：    

`quat`    



<br />
<br />
<br />
<br />


# GLM库函数  


## 矩阵变换相关      

`inverse()`矩阵/四元数的逆。  

`translate(<mat>, <vector>)` : 构建位移矩阵。(注：可以用mat4(1.0f)构建初始单位矩阵)    

`perspective(<fov>, <aspectRatio>, <nearPlane>, <farPlane>)`:构建透视矩阵。    

`rotate(<mat>, <angle>, <axis>)`：构建旋转矩阵。（Local旋转）    

`transpose()`转置矩阵。      

`quat_cast` 转换四元数。    

`eularAngles`欧拉角。    

# 补充  

- GLM的mat4是按列进行排列存储的，构造时每四个参数写入一列，索引时也是按照[第?列][第?行]方式进行的。  
（因为GLSL的mat4也是按列进行存储的）  