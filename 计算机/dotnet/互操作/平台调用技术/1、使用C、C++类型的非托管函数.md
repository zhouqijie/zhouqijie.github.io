# 使用C/C++类型的非托管函数    


<br />
<br />

## 平台调用简介    

*平台调用(P/Invoke)*可以消除.NET托管代码和非托管代码之间的鸿沟，它允许托管代码调用在DLL中实现的非托管函数。    

托管代码必须在**CLR环境**下运行，而非托管代码一般是**本机代码(native code)**。    


- **使用场合**    

1. 前期用C/C++开发了很多工作良好的DLL。但是后续版本需要用托管代码开发。    
2. 需要通过平台调用Win32的API来实现CLR没有提供的一些功能。  
3. 为了提高整体性能，使用C/C++编写核心算法，然后托管代码调用。    


- **非托管函数的平台调用过程**    

1. 查找包含该函数的DLL。  
2. 将该DLL加载到内存。  
3. 查找函数在内存中的地址并将其参数压入堆栈，以封送所需的数据。（DLL会一直驻留内存，除非关闭应用程序域）   
4. 控制权转移给非托管函数。    


<br />
<br />

## HelloWorld实例    

首先，使用VisualC++创建一个非托管DLL，命名为Native.dll：  

```C++  
void PrintMsg(const char* msg)
{
    printf("%s/n", msg);
    return;
}
```  

只要在托管代码中正确声明函数后，就能像调用普通函数一样调用该函数。其他的加载DLL，查找函数地址、数据封送等操作由.NET在运行时自动完成。    

```C#  
[DllImport("NativeLib.dll")]
static extern void PrintMsg(string msg);
```
```C#
public static void Main(string[] args)
{
    PrintMsg("HelloWorld!");
}
```

<br />
<br />


## 平台调用基本知识    

示例:  
```C++  
//函数定义：
int _stdcall Multiply(int a, int y)
{
    return x * y;
}

//头文件中：    
extern "C" _declspec(dllexport)
	int _stdcall Multiply(int x, int y);
```  

> `_stdcall`是调用规约，要求函数的参数从右往左依次入栈，调用者负责清理堆栈。  


- **声明函数的3点注意**    

1. 声明函数时，必须使用`extern`修饰符。告诉编译器函数是外部实现的，没有方法体。  
2. 声明函数时，必须使用`static`修饰符，即非实例方法。    
3. 声明函数时，必须加上`DllImport`Attribute，传入DLL的名称作为参数（相对路径或绝对路径）。    

- **搜索顺序**    

1. 当前目录。  
2. Windows系统目录。  
3. PATH环境变量的所有目录。    

- **DllImport的属性字段**    

|字段|说明|
|-|-|
|`BestFitMapping`|切换最适合的映射|
|`CallConvertion`|指定用于传递方法的调用约定。默认值`WinAPI`，该值对应基于32位Intel的`_stdcall`|
|`CharSet`|控制名称重整以及封送字符串的方式。默认为`CharSet.Ansi`|
|`EntryPoint`|入口点|
|`ExactSpelling`|控制是否应修改入口点以对应字符集|
|`PreserveSig`|控制托管方法签名是否应转换成返回HRESULT且返回值有一个附加的[out, retval]参数的非托管签名。默认为true。|
|`SetLastError`|使调用方能够使用Marshal.GetLastWin32Error API函数确定在执行该方法时是否出错。C#/C++中默认false|
|`ThrowOnUnmappableChar`|控制转换为ANSI'?'字符的、不可映射的Unicode字符引发异常|


（END）    