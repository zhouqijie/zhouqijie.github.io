# 简介  

单指令多数据(SIMD)是指CPU能用一个指令并行地对多个数据进行数学运算。例如把4个浮点数并行地进行相乘。SIMD 广泛应用在游戏引擎的数学库中，它能迅速执行常见的矢量计算例如点积和矩阵乘法。  

> Intel在1994年首次将多媒体拓展(MMX)指令集加入CPU产品线。MMX指令集能对MMX寄存器的数据进行SIMD运算。后来Intel在奔腾3中加入了第一个SSE(单指令多数据流扩展)指令集，它是继MMX的扩充指令集，SSE指令采用128位寄存器，可存储整数和IEEE浮点数。  

游戏引擎中最常见的SSE模式位32位浮点数打包模式，4个float值被打包进单个1238位寄存器，单个指令可对4个浮点数进行并行运算。对于例如4维向量乘以4x4矩阵这类运算正合适。  

# 一、SSE寄存器  

每个SSE寄存器包含4个32位浮点数。通常表示为$[x \space y \space z \space w]$。  

### SIMD指令示例：  

`addps xmm0. xmm1`  

addps指令把128位XMM0寄存器中的4个float分别与XMM1寄存器的4个float相加，运算结果写回XMM0。  

### 另一个方式分析：  

`xmm0.x = xmm0.x + xmm1.x`  
`xmm0.y = xmm0.y + xmm1.y`  
`xmm0.z = xmm0.z + xmm1.z`  
`xmm0.w = xmm0.w + xmm1.w`  

存储在SSE寄存器的4个浮点数，可以个别抽出存进内存或从内存载入，但是操作速度较慢。所以应当避免普通float运算和SIMD运算混合使用。   
> (因为x87 FPU寄存器和SSE寄存器之间传输数据糟糕，因为CPU须等待x87单元或SSE单元完成所有正在进行的工作。)   

尽量把数据尽可能久的存储在SSE寄存器中，减少与FPU寄存器/内存的数据传输。标量值例如矢量点积也尽可能存储在SSE寄存器中而不是传输至float变量。  
> 可以把单个浮点值复制至SSE寄存器的4个位置以表示标量，即$x=y=z=w=s$来存储s标量。  

# 二、__m128数据类型  

对于C/C++语言，VisualStudio编译器提供了内建的__m128数据类型。大多数情况下，这类型的变量存储在内存中，但是在计算时会直接在CPU的SSE寄存器中运用。以__m128声明的自动变量或者函数参数，编译器通常会把它们直接放在SSE寄存器而非内存堆栈。    

### __m128对齐：  

一个__m128变量存储在内存中时应确保它是128位对齐的。  

编译器会自动为类和结构加入填充padding，所以如果整个类或结构是128位对齐的，类或结构中的__m128变量也是正确对齐的。  

声明一个或者多个自动或全局类/结构，编译器会自动把对象对齐。但是对于用malloc和new动态分配的数据结构，必须编程时手动对齐。    

# 三、SSE内部函数编程  

SSE运算可以用原始的汇编语言实现，也可通过C/C++中的内联汇编，也可使用编译器提供的内部函数。  

但是汇编实现缺乏可移植性。考虑简便（直观、清楚、代码少）、优化（优化寄存器分配、调乱指令次序等）、跨平台(有些不支持内联汇编)等一系列问题，应当尽量使用内部函数。    


内联汇编示例：  
```CPP
__m128 addWithAsembly(__m128 a, __m128 b)
{
    __m128 r;
    __asm
    {
        movaps xmm0, xmmword ptr [a]
        movaps xmm1, xmmword ptr [b]
        addps xmm0, xmm1
        movaps xmmword ptr [r], xmm0
    }
    return r;
}
```
内部函数示例：    
```CPP
__m128 addWithIntrinsics(__m128 a, __m128 b)
{
    __m128 r = _mm_add_ps(a, b);
    return r;
}
```  


测试程序：  
```CPP
#include <xmmintrin.h>

_declspec(align(16)) float A[] = { 2.0f, -1.0f, 3.0f, 4.0f };
_declspec(align(16)) float B[] = { -1.0f, 3.0f, 4.0f, 2.0f };
_declspec(align(16)) float C[] = { 0.0f, 0.0f, 0.0f, 0.0f };
_declspec(align(16)) float D[] = { 0.0f, 0.0f, 0.0f, 0.0f };

int main()
{
	//把浮点数组载入a和b
	__m128 a = _mm_load_ps(&A[0]);
	__m128 b = _mm_load_ps(&B[0]);

	//测试两种函数
    __m128 c = addWithAsembly(a, b);
    __m128 d = addWithIntrinsics(a, b);

    //把ab的值存储回原来的数组，确保他们没被改动
    _mm_store_ps(&A[0], a);
    _mm_store_ps(&B[0], b);
    //把结果存储至数组，以便打印
    _mm_store_ps(&C[0], c);
    _mm_store_ps(&D[0], d);

    //检查结果
    printf("%g %g %g %g \n", A[0], A[1], A[2], A[3]);
    printf("%g %g %g %g \n", B[0], B[1], B[2], B[3]);
    printf("%g %g %g %g \n", C[0], C[1], C[2], C[3]);
    printf("%g %g %g %g \n", D[0], D[1], D[2], D[3]);

    return 0;
}
```  

> 内部函数_mm_load_ps的作用是把内存中的float数组载入__m128变量(即SSE寄存器)。  

> 四个全局数组都使用了__declspec(align(16))强制声明为16字节对齐，省去该指令会崩溃。  


# 四、SSE实现矢量乘矩阵  


> （假设是矩阵左乘列向量）  
> 
> （矩阵对矩阵的乘法也可用类似方法实现）  


### 标准流程：  

向量分别与矩阵的每行相乘，得到中间结果即4个向量，然后把每个中间结果的4个分量相加（问题是把SSE寄存器的四个分量相加是困难和低效的）。最后还需要把分散在四个寄存器的相加结果介个到单个向量。    

### 更好的计算流程：  

使用矩阵的列分别乘以向量的分量，这样就可以并行地相加，最终结果也会置于单个SSE寄存器中。    

要这么做需要把向量里的单个向量复制(replicate)到其余分量，需要使用SSE指令shufps(对于内部函数_mm_shuffle_ps)。    

shufps是用来任意调整分量的通用指令，可以定义宏来让它复制xyzw分量：  
```CPP  
#define SHUFFLE_PARAM(x, y, z, w) \
    ((x) | ((y) << 2) | ((z) << 4) | ((w) << 6))
#define _mm_replicate_x_ps(v) \
    _mm_shuffle_ps((v), (v), SHUFFLE_PARAM(0, 0, 0, 0))
#define _mm_replicate_y_ps(v) \
    _mm_shuffle_ps((v), (v), SHUFFLE_PARAM(1, 1, 1, 1))
#define _mm_replicate_z_ps(v) \
    _mm_shuffle_ps((v), (v), SHUFFLE_PARAM(2, 2, 2, 2))
#define _mm_replicate_w_ps(v) \
    _mm_shuffle_ps((v), (v), SHUFFLE_PARAM(3, 3, 3, 3))
```  
示例：    
```CPP
__m128 mulVectorMatrix(__m128 v, __m128 Mcol1, __m128 Mcol2, __m128 Mcol3, __m128 Mcol4 )
{
    __m128 xMcol1 = _mm_mul_ps(_mm_replicate_x_ps(v), Mcol1);
    __m128 yMcol2 = _mm_mul_ps(_mm_replicate_y_ps(v), Mcol2);
    __m128 zMcol3 = _mm_mul_ps(_mm_replicate_z_ps(v), Mcol3);
    __m128 wMcol4 = _mm_mul_ps(_mm_replicate_w_ps(v), Mcol4);

    __m128 result = _mm_add_ps(xMcol1, yMcol2);
    result = _mm_add_ps(result, zMcol3);
    result = _mm_add_ps(result, wMcol4);

    return result;
}
```  

### 进一步优化：  

如果能使用某些CPU的乘加指令，那么还能进一步优化。可惜SSE不支持乘加指令，但是可以定义宏来替代也不错。  
```CPP
#define _mm_madd_ps(a, b, c) \
    _mm_add_ps(_mm_mul_ps((a), (b)), (c))

__m128 mulVectorMatrixFinal(__m128v, __m128 Mcol1, __m128 Mcol2, __m128 Mcol3, __m128 Mcol4 )
{
    __m128 result;
    result = _mm_mul_ps(_mm_replicate_x_ps(v), Mcol1);
    result = _mm_madd_ps(_mm_replicate_y_ps(v), Mcol2, result);
    result = _mm_madd_ps(_mm_replicate_z_ps(v), Mcol3, result);
    result = _mm_madd_ps(_mm_replicate_w_ps(v), Mcol4, result);
    return result;
}
```  

