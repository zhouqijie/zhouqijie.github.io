# 调用Win32API      


<br />
<br />


## 确定要调用的函数    

由于许多Win32API函数都有ANSI和Unicode两个版本，因此需要指定调用的是哪个版本。（后缀A代表ANSI，后缀W代表宽字符Unicode）      

<br />
<br />

## 处理Win32函数返回的错误码    

要想获得调用Win32函数过程中出现的错误信息，首先必须将DllImport的SetLastError字段设置为true。只有这样P/Invoke才会将最后一次调用Win32API产生的错误码保存起来。然后通过`Marshal.GetLastWin32Error`方法获得这个保存的错误码。    

<br />
<br />

## 处理回调函数    

在许多Win32函数以及其他非托管函数中，可能需要传递回调函数作为参数。    

要想在C#对这种函数进行平台调用，首先要为该回调函数声明和实现一个**委托**。    


<br />
<br />

## 使用Windows定义的常量    

许多Win32函数都接收常量作为参数或者返回一个常量。在通常情况下这些常量会定义在定义该Windows函数的头文件中。    

可以查找MSDN文档，或者最原始的方法：写一个C++程序把这些常量输出出来。    

在C#中可以把这些常量定义为枚举值。    

<br />
<br />

## 封送Win32类型    

（略）Win32数据类型的封送处理以及blittable类型可以查表。    

<br />
<br />

## 处理句柄    

很多Win32函数会接收一个或返回一个代表非托管资源的*句柄(handle)*。比如一个文件句柄或一个等待句柄。     

如果平台调用过程中使用了托管对象，并且该对象在调用开始后没有被引用，那么垃圾回收器很有可能会回收该托管对象。这将使资源被释放并导致属于该对象的句柄无效。一个可行的解决办法是将句柄包装在`HandleRef`结构体中，这样就能保证该托管对象不被GC回收。    

```C#
// WIN32 API  
// DWORD GetFileSize(HANDLE hFile, LPDWORD lpFileSizeLength);
[DllImport("Kernel32.dll")]
static extern int GetFileSize(HandleRef fileHandle, IntPtr fileSizeHigh);

//...
static void Foo()
{
    FileStream fs = new FileStream("Test.txt", FileMode.Open);
    HandleRef handleref = new HandleRef(fs, fs.Handle);
    int fileSize = GetFileSize(handleRef, IntPtr.Zero);
    Console.WriteLine("FileSize:" + fileSize);
}
```  

> `HandleRef`的第一个参数是包装句柄的托管对象，第二个参数是非托管句柄。    
> 将实例化的`HandleRef`传入`GetFileSize`之后，即使`FileStream`的对象`fs`已经不再引用，它也不会被回收。    
> `HandleRef`也是一个可以被封送拆收器识别的特殊类型。    
> 使用类型为`Normal`的`GCHandle`也可以防止不合时宜的垃圾回收。    

<br />
<br />

## 传递托管对象    

一些Win32函数允许将任何类型的对象以参数形式传入。如果把托管对象传入到该函数，就会面临一个问题，就是垃圾回收器可能会回收该对象，而非托管代码还要使用该对象。这时就可以用`GCHandle`手动监视和控制对象的生命周期。    

一般情况下，要想控制对象的生命周期，需要调用`GCHandle.Alloc`方法，并将需要监视或者控制的对象传递给方法。主要步骤如下。    

1. 使用`GCHandle.Alloc`为托管对象分配一个`GCHandle`。    
2. 将生成的`GCHandle`实例传递给非托管代码并使用它。    
3. 非托管函数返回后，必须调用`GCHandle.Free`方法释放该`GCHandle`。    


<br />
<br />

（END）    