# dotnet发展历程      

## C#1.0 / .Net1.0（CLR）    

- **CLR**    

> 在.NET和CLR推出之前，每种编程语言都有自己的运行库。在C++中，C++运行库与每个C++程序链接起来、Java的运行库是Java虚拟机。而CLR是每种.NET语言比如C#、VB、ManagedC++都使用的运行库。    


- **中间语言(IL)**    

.NET编程语言的编译器生成中间语言(*Intermediate Landuage, IL*)代码。IL代码看起来像面向对象的机器码。    

CLR包含一个即时(*Just-In-Time,JIT)编译器。当程序开始运行时，JIT编译器会从IL代码生成Native代码。  


- **垃圾回收器(GC)**    

CLR的其他部分是垃圾回收器、调试器扩展和线程实用工具。垃圾回收器负责清理不再引用的托管内存，这个安全机制使用代码访问安全性来验证允许代码做什么。    



## C#2 /.NET2（泛型）    

- **泛型(Generic)**    

C#2和.NET2是一个巨大的更新。一个大的变化是泛型。泛型允许创建类型，而不需要知道使用什么内部类型。所使用的内部类型在实例化时定义。    



## .NET3.0（WPF、WCF、WF）    

- **WPF**  

发布.NET3时，不需要新版本的C#。3.0版本只提供了新的库，发布了大量新的类型和名称空间。WPF就是新框架最大的一部分，用于创建桌面应用程序。    

- **WCF**  

.NET3之前，Asp.Net WebService和.NET Remoting用于应用程序间通信。各种技术使用不同的API编程。而WCF把其他API的所有选项结合到一个API中。    

- **WF**    

.NET 第三部分是Windows WF(Workflow Foundation)。    


## C#3和.NET3.5（LINQ）    

主要改进是使用C#定义的查询语法。它允许使用相同的语句来查找、排序对象列表、XML文件和数据库。    

> CRE：即LINQ To Objects、LINQ To XML、LINQ To SQL等。    


## C#4和.NET4.0（dynamic、TPL）    

- **dynamic**    

C#主题是动态集成脚本语言，使其更容易使用COM集成。C#语法扩展为使用**dynamic关键字**、**命名参数**、**可选参数**、**泛型增强的逆变和协变**。    

- **任务并行库(TPL)**    

任务并行库(*Task Parallel Library*)使用**Task**类和**Parallel**类抽象出线程，更容易创建并行代码。    

- **ASP.NET MVC**    

与WebForms不同，需要编写HTML和JS，并使用C#和.NET的服务端功能。    


## C#5（异步编程、UWP、WebAPI）    

- **异步编程**    

C#5只有两个新关键字：`await`和`async`，然而它们大大简化了异步方法的编程。    

- **WebAPI**    

WCF提供有状态和无状态服务，以及许多不同的网络协议。而Asp.Net WebAPI则简单得多，它是基于REST架构风格的。    


## C#6和.NET Core    

- **C#6一些小的新特性**    

1. 静态using声明。(`using static System.Console;`)  
2. 表达式体方法。(`public bool IsSquare => rect.Height == rect.Width;`)  
3. 自动实现的属性初始化器。(`public int Age{get;set;} = 42;`)  
4. 只读的自动属性。(`public BookId{get;}`)  
5. nameof运算符。(`if(obj == null) throw new ArgumentNullException(nameof(obj));`)  
6. 空值传播运算符。(`int? age = p?.Age;`)  
7. 字符串插值。(`public override ToString() => $"{Title}{Publisher}";`)  
8. 字典初始化。(`var dic = new Dictionary<int,string>(){ [3]="three", [7]="seven" };`)    
9. 异常过滤器。(`try{...}catch(MyException ex) when (ex.ErrorCode == 405){...}`)  


- **.NET Core**    

.NET Framework并不是唯一的.NET框架。还有运行在Web浏览器的Sliverlight以及WindowsPhone的Sliverlight等小型版本的框架。    

随着时间的推移，有了很多的不同.NETFramework版本，可移植库的管理已成为噩梦。为了解决这些问题，需要新的.NET版本，新的版本命名为*.NET Core*。.NET Core较小，带有模块化的NuGet包以及分布给每个应用程序的运行库是开源的，可以用于不同的操作系统。    

.NET Framework要求必须在系统上安装应用程序需要的特定版本，而在.NET Core1.0中，框架(包括运行库)是与应用程序一起交付的。    


（Continue...）  