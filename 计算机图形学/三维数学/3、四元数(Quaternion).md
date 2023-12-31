# 一、四元数(Quaternion)  

四元数是复数的延申，最初用于解决力学问题。四元数遵守一组规则，这些规则称为实数域上的四维**赋范可除代数**。

在应用上，单位长度的四元数($x^2 + y^2 + z^2 = 1$)能代表三维旋转。  

## 旋转矩阵表示旋转的缺点：  

- 矩阵需要存储9个浮点值表示旋转，而旋转只有3个自由度，冗余过大。  
- 矢量矩阵乘法来旋转矢量，要做3次点积，计算速度不如四元数运算。  
- 表示中间旋转，即线性插值时，用矩阵来计算这些中间值很困难。  

## 单位四元数用来代表三维旋转  

单位四元数可以看作三维矢量加上第四维的标量坐标，即$q = [q_V \space q_S]$，或者$[asin{θ \over 2} \space \space cos{θ \over 2}]$。  

> (矢量部分时旋转轴的单位矢量a乘以旋转半角的正弦。标量部分是旋转半角的余弦。)  

> 也表示为$q = [a_{x}sin{θ \over 2} \space a_{y}sin{θ \over 2} \space a_{z}sin{θ \over 2} \space cos{θ \over 2}]$  

单位四元数表示和轴角表示很相似。而且四元数在数学上比轴角更方便。  

# 二、四元数运算  

## 四元数乘法：  

四元数乘法有很多种，与三维旋转的乘法为*格拉斯曼积(Grassmann Product)*：$pq = [(p_{S}q_{V} + q_{S}p_{V} + p_{V} × q_{V}) \space \space \space (p_{S}q_{S} - p_{V}·q_{V})]$  

(pq代表两旋转的合成旋转，即先旋转Q再旋转P)  

## 共轭四元数和逆四元数：  

共轭四元数的定义$q^{*} = [-q_{V} \space q_{S}]$，即四元数的矢量部分求反。  

逆四元数的定义$q^{-1} = {q_{*} \over {|q|}^2}$。

> 单位长度的四元数其逆四元数和共轭四元数相等。  

> 计算逆四元数比计算逆矩阵快得多，可利用此特性优化引擎。  

## 四元数积的共轭和逆  

四元数积德共轭，等于求各个四元数的共轭后，以相反次序相乘。(即$(pq)^* = q^*p^*$)  

四元数积的逆，等于求各个四元数的逆，然后相反次序相乘。(即$(pq)^{-1} = q^{-1}p^{-1}$)  

# 三、用四元数旋转矢量  


用四元数旋转矢量v,要先把矢量重写为四元数形式，标量项设为0，即$[v_x \space v_y \space v_z \space 0]$。  

要用四元数q旋转矢量v，须用q前乘矢量v，再后乘逆四元数$q^{-1}$。旋转后的矢量即为$v^{'} = qvq^{-1}$的矢量部分。  

## 连续变换（四元数串接）  

和矩阵的变换一样，四元数也可以相乘串接旋转，把多次变换合成为一次。  

例如三个四元数q1,q2,q3分别表示不同旋转，合成旋转四元数为$q_{net} = q_3q_2q_1$，注意四元数的相乘次序和旋转次序相反。  

最终旋转后的结果为$v^{'} = q_{net}v{q_{net}}^{-1} = q_3q_2q_1v{q_1}^{-1}{q_2}^{-1}{q_3}^{-1}$  

# 四、四元数和矩阵的互相转换  

任何三维旋转都能在四元数和三维旋转之间转换。  

### 四元数转换矩阵：  

$R = 
{\begin{bmatrix}
 {1 - 2y^2 - 2z^2} & 2xy + 2zw & 2xy - 2yw \\ 
 2xy - 2zw & {1 - 2x^2 - 2z^2} & 2yz + 2xw \\
 2xz + 2yw & 2yz -2xw & 1- 2x^2 - 2y^2
 \end{bmatrix}}$  

 ### 矩阵转换四元数：  

⚪基本原理：对角线元素可以分别求xyzw的值    


{% raw %}

$w = \sqrt {m11 + m22 + m33 + 1} / 2$  
$x = \sqrt {m11 - m22 - m33 + 1} / 2$  
$y = \sqrt {-m11 + m22 - m33 + 1} / 2$  
$z = \sqrt {-m11 - m22 + m33 + 1} / 2$  
(警告：这种计算方式中的开方未考虑根的正负)  

- 并且已知：  
$m12 + m21 = (2xy + 2wz) + (2xy - 2wz) = 4xy$  
$m12 - m21 = (2xy + 2wz) - (2xy - 2wz) = 2wz$  
$m31 + m13 = (2xz + 2wy) + (2xz - 2wy) = 4xz$  
$m31 - m13 = (2xz + 2wy) - (2xz - 2wy) = 4wy$  
$m23 + m32 = (2yz + 2wx) + (2yz - 2wx) = 4yz$  
$m23 - m32 = (2yz + 2wx) - (2yz - 2wx) = 4wx$  

{% endraw %}

⚪所以求得w之后可以由求得剩余三个。同样也可先求x或y或z然后再求其他三个：  

{% raw %}

$w = \sqrt{m11 + m22 + m33 + 1} / 2$  
$x = {{m23 - m32} \over 4w}$$y = {{m31 - m13} \over 4w}$$z = {{m12 - m21} \over 4w}$  

$x = \sqrt{m11 - m22 - m33 + 1} / 2$  
$w = {{m23 - m32} \over 4x}$$y = {{m12 + m21} \over 4x}$$z = {{m31 + m13} \over 4x}$  

$y = \sqrt{-m11 + m22 - m33 + 1} / 2$  
$w = {{m31 - m13} \over 4y}$$x = {{m12 + m21} \over 4y}$$z = {{m23 + m32} \over 4y}$  

$z = \sqrt{-m11 - m22 + m33 + 1} / 2$  
$w = {{m12 - m21} \over 4z}$$x = {{m31 + m13} \over 4z}$$y = {{m23 + m32} \over 4z}$  

{% endraw %}

(一般先计算一个分量，比如w，再计算xyz。)  

(但是这种方式在w接近0或非常小时数值不稳定。所以有学者建议首先判断wxyz中哪个最大，就用对角线元素先计算该元素，再计算其他三个)  

⚪参考代码：  

```CSharp
    //1. 已验证
    //2. 行列方式： R[row, col]     
    public static float[] MatrixToQuaternionTest(float[,] R)
    {
        float[] q = new float[4];

        float trace = R[0, 0] + R[1, 1] + R[2, 2];

        if (trace > 0f)
        {
            float s = Mathf.Sqrt(trace + 1.0f);
            q[3] = s * 0.5f;

            float t = 0.5f / s;
            q[0] = (R[2, 1] - R[1, 2]) * t;
            q[1] = (R[0, 2] - R[2, 0]) * t;
            q[2] = (R[1, 0] - R[0, 1]) * t;
        }
        else
        {
            int i = 0;
            if (R[1, 1] > R[0, 0]) i = 1;
            if (R[2, 2] > R[i, i]) i = 2;
            
            int[] next = new int[3] { 1, 2, 0 };
            int j = next[i];
            int k = next[j];

            float s = Mathf.Sqrt(R[i, i] - R[j, j] - R[k, k] + 1.0f);

            q[i] = s * 0.5f;

            float t;
            if (s != 0.0f)
            {
                t = 0.5f / s;
            }
            else
            {
                t = s;
            }

            q[3] = (R[k, j] - R[j, k]) * t;
            q[j] = (R[j, i] + R[i, j]) * t;
            q[3] = (R[k, i] + R[i, k]) * t;
        }
        
        return q;
    }
```
 
> 参考文章:https://blog.csdn.net/loongkingwhat/article/details/88428310   
> 参考文章:《游戏引擎架构》-Jason Gregory  

# 五、四元数插值  

### 简单线性插值（沿弦线插值）：  

即Lerp运算。简单的加权平均插值，逐项计算加权平均。  

Lerp运算不保持矢量长度值，注意归一化。  

### 球面线性插值（沿弧线插值）：  

四元数是四维超球(hyper sphere)上的点。Lerp运算实际是沿着弦线插值而不是球面上插值，所以导致匀速变化插值时旋转角速度不是线性的而是两边慢中间快。解决办法时球面线性插值(Slerp)。

{% raw %}

SLerp和Lerp的公式相近，但是加权系数不一样。  
$Slerp(q_1, q_2, t) = t_1q_1 + t_2q_2$  

其中$t_1 = {{sin(1 - t)θ} \over {sinθ}}$。  
其中$t_2 = {{sintθ} \over {sinθ}}$。  
其中θ为两单位四元数夹角，可用四维点积得cosθ。

{% endraw %}

### SLerp和Lerp比较：  

Slerp性能稍昂贵，Lerp在游戏中已经够用了。  

