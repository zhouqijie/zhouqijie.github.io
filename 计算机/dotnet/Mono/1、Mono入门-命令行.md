# 命令行    

`mcs`  

MonoC#的编译器。可以用它编译、嵌入资源、链接Mono程序。  

不直接编译为可执行代码，而是编译为CIL。  

`mono`  

用虚拟机执行一个编译后的Mono程序。  
mono使用JIT编译器翻译CIL为机器码。  

`mint`  

Mono运行的平台并不是都有JIT编译器实现，mint是一个较慢的替代。在程序执行时实时解释CIL字节码。    

`monodoc`  

...  

`monop <class>`  

打印类的签名(signature)。  


`gacutil`  

操纵GAC的内容。    

例如`gacutil -l`查看所有程序集。  

> 使用Windows和.NET，系统范围的程序集存储在*全局程序集缓存(GAC)*中。  



<br />
<br />
<br />
<br />


（END）  