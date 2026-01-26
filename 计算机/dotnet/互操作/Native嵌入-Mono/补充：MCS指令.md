# mcs编译器指令

格式： `mcs [参数]`  

示例： `mcs -reference:assembly_core.dll Test1.cs Test2.cs -out:assembly_test.dll -target:library`

参数可以使用-option或者/option的方式。

参数：

--about 显示c#编译器的信息

-checked[+][-]指定溢出数据类型边界的整数算法是否将在运行时导致异常。

-codepage:ID指定编译中的所有源代码文件所使用的代码页

-clscheck[+][-] 禁止编译器依附关系检查

-define:S1[;S2] 定义一个或多个符号，可以简写为/d

-debug[+][-] 产生调试信息

-delaysign[+][-] 仅仅将公钥插入到配件中，并不进行签名

-doc:File 产生XML文档，利用ndoc工具可以产生类似MSDN的文档

-g 产生调试信息

-keycontainer:NAME 指定用来强命名(strongname)配件的密钥对

-keyfile:FILE 指定用来强命名的密钥文件

-lib:PATH1,PATH2 增加配件链接的路径

-main:class 指定入口点(entry point)，也就是缺省可执行的那个类

-noconfig[+][-] 不使用默认的参考编译，和微软的csc的/noconfig对应

-nostdlib[+][-] 不导入标准库

-nowarn:W1[,W2] 显示一个或者多个警告信息

-out:FNAME 指定输出文件名

-pkg:P1[;P2] 引用包P1…P2

--expect-error X 期望抛出X异常

-resource:SPEC搜索指定目录中要编译的源文件

-reference:ASS 编译时引用ASS配件(缩略`-r:`)

-target:KIND 编译输出类型，可以是exe、winexe、liberary和module(缩略`-t:`)

-unsafe[+][-] 编译使用 unsafe 关键字的代码

-warnaserror[+][-] 将警告作为错误对待

-warn:LEVEL 警告等级，最高是4，缺省是2

-help2 获得更多帮助信息

-linkresource:FILE[,ID] 链接一个资源文件

-resource:FILE[,ID] 嵌入一个资源文件

-win32res:FILE 链接一个win32资源文件

-win32icon:FILE 指定图标

@FILE指定响应文件,文件列出了编译器选项或要编译的源代码文件的文件

>链接：https://blog.csdn.net/l_serein/article/details/5883666