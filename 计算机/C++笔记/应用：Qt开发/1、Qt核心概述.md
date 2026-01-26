# Qt核心概述    


<br />  
<br />  


## 元对象系统    

元对象(Meta-Object)系统提供了对象之间通信的信号与槽机制、运行时类型信息和动态属性系统。    

> CRE：反射系统。  

### 三个部分：  

1. QObject类：所有使用元对象系统的基类。  
2. 在类中声明`Q_Object`宏，使得类可以使用元对象的特性，如动态属性、信号与槽。    
3. 元对象编译器(MOC)为每个QObject子类提供必要的代码来实现元对象系统的特性。  

### 一些功能：  

1. 获取元对象。(`obj->metaObject()`)    
2. 类型继承判断。(`obj->inherit("XXX")`)(可使用`qobject_cast<>`转换对象类型)      
3. 属性反射(`obj->setProperty()/obj->property()`)    
4. 翻译字符串。  


### 重点补充：  

> CSDN：在C++编译器编译源码之前，元对象编译器MOC会先分析Qt特有的的宏，例如`Q_OBJECT`和`Q_PROPERTY`等，然后生成包含这些宏实现的源文件。最后再给C++编译一起编译。        
> CRE：虚幻引擎4也是使用类似的分析工具做属性反射。（详见《游戏引擎原理与实践--程东哲）    


<br />
<br />

## 属性系统    

- 属性定义：  

Qt提供一个`Q_PROPERTY()`宏可以定义属性，他也是基于元对象系统实现的。    


- 属性使用：  

使用`setProperty()/property()`设置/获取属性值。  

- 动态属性：    

`obj->setProperty()`可以在运行时为类定义一个新的属性，称之为动态属性，是针对**类的实例**定义的。    

- 类的附加信息：    

属性系统还有一个`Q_CLASSINFO()`宏，可以为类的元对象定义键值信息。    


<br />
<br />

## 信号与槽    

> CRE：Qt的信号/槽机制类似其他一些开发框架的事件回调系统/消息系统。    


### connect函数的重载形式：  

一般句法：`conncect(sender, SIGNAL(signal()), receiver, SLOT(func()))`      

另一种：`connect(sender, &XXX::aaa, receiver, &YYY::bbb, conncectionType)`  

- `Qt::ConnectionType`的值：  

1. Qt::DirectConnection:立即执行，槽函数和信号在同一线程。  
2. Qt::QueuedConnection：在事件循环回到接收者线程后执行槽函数，槽函数和信号在不同线程。  
3. Qt::BlockingQueueConnection：与Queued类似，只是信号线程会阻塞直到槽函数执行完毕。（信号和槽函数在同一线程会造成死锁）    
4. Qt::AutoConnection(默认)：如果信号的接收者和发射者在同一线程，就使用Qt::DirectConnection方式，否则使用Qt::QueuedConnection方式，在信号发射时自动确定关联方式。    


### 使用sender()获取发射者：  

在槽函数里使用`sender()`可以获取信号发送者的指针。  

### 自定义信号及其使用：  

在自己的类里可以自定义信号，信号就是在类定义声明一个函数，这个函数无需实现，只需发射(Emit)。    

