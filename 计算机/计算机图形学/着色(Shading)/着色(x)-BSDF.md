# 一、双向散射分布函数(BSDF)    

> BRDF和BTDF统称BSDF。  
> BSDF是实现PBR的重要方法。  


性质：  
> 非负性(Non-Negtivity)。  
> 可逆性(Reciprocity)。  
> 线性(Linearity)。  
> 能量守恒(Energy Conservation)。  


## 1. BSDF漫反射    

> LambertBRDF是使用最广最简单的计算公式。  
> 出射光和入射光Irradiance相等。且Radiance是Uniform。  

公式：  
$L_o(w_o) = \int_{H^2}^{} f_r L_i(w_i)cos{θ_i}dw_i$  

$L_o(w_o) =f_r L_i \int_{H^2}^{} (w_i)cos{θ_i}dw_i$

$L_o(w_o) = π f_r L_i$  

> $f_r = ρ / π$  (ρ反射率称为Albedo)    

## 2. BSDF镜面反射    

反射方向计算公式：  
$w_o = -w_i + 2(w_i · n)n$  

## 3. BSDF折射    

公式：  
$η_isinθ_i = η_tsinθ_t$(不同材质的折射率η)  
$cosθ_t = \sqrt{1 - ({η_i} \over {η_t})^2  (1 - cos^2θ_i) }$  

> 当$1 - ({η_i} \over {η_t})^2  (1 - cos^2θ_i) < 0$时，发生全反射。  

## 4. 菲涅尔项(Fresnel Term)    

> 当入射光几乎与平面平行入射时，则会被几乎完全反射。  
> 当入射光几乎与平面垂直入射时，更多能量会直接穿过去。  

- Schlick's Approxiamation:  

$R(θ) = R_0 + (1 - R_0) (1 - cosθ)^5$  
$R(0) = ({n_1 - n_2} \over {n_1 + n_2})^2$  

## 5.微表面理论  

> 微表面理论是PBR的核心理论。  

> 微表面理论(Microsurface)理论把粗糙的平面看作凹凸的镜面。  



$f(i, o) = {F(i, h) G(i, o, h) D(h)} \over {4(n, i)(n, o)}$  

> D(h) -- 法线分布。  
> G(i, o, h) -- 几何项(自遮挡)。  
> F(i, h) -- 菲涅尔项。  

> 微表面理论常用来表达高光反射。  


## 6.各向异性材质BSDF    

> 受绝对方位角有关的材质。例如金属横纹、光碟等。  

对各向同性的材质有：$f_r(θ_i, Φ_i; θ_r, Φ_r) = f_r(θ_i, θ_r, |Φ_r - Φ_i|)$  
对各向异性的材质有：$f_r(θ_i, Φ_i; θ_r, Φ_r) ≠ f_r(θ_i, θ_r, Φ_r - Φ_i)$  



## 7.测量BSDF    

> 理论模型不够准确，与实际存在差异，需要使用仪器测量。    



（END）    