# Qt全局定义    

<br />
<br />


## Qt数据类型    

`qint8`  
`qint16`  
`qint32`  
`quint8`  
`quint16` 
`uchar`  
`uint`  
`qreal`(double)  
`qfloat16`  
......


<br />
<br />

## Qt全局函数    

`T qAbs(const T& value)`(求绝对值)  
`const T &qBound(const T &min, const T &value, const T &max)`(限定value的范围到min-max)    
`qrand() / qsrand(seed)`（随机数）  
......


<br />
<br />

## 宏定义    

`QT_VERSION`Qt编译器版本  
......  
`QT_DECL_IMPORT`/`Q_DECL_EXPORT`在使用或设计共享库时，用于导入或导出库的内容。    
......  
`QT_DECL_OVERRIDE`在类定义中，用于重载一个虚函数。    
......  



<br />  
<br />  
<br />  
<br />  


# Qt容器    

> 《QT5.9开发指南》：Qt的容器比STL中的容器更轻巧安全和易用。这些容器是隐式共享和可重入的，而且它们进行了速度优化和存储优化，可以减小可执行文件的大小。此外它们还是线程安全的。    


## 顺序容器    

- **QList**  

最常用的容器类，是以数组列表(arrayList)的形式实现的。  

- **QLinkedList**  

链表。无法随机访问,其他接口和QList基本相同。  

- **QVector**  

动态数组。和QList几乎完全一样但是更高性能，因为数据项是连续存储的。    


- **QStack**  

堆栈。  

- **QQueue**  

队列。  


<br />
<br />

## 关联容器类    

- **QSet**    

集合。基于散列表，查找值的速度很快。      

- **QMap**    

QMap存储数据是按照键的顺序，如果不在乎存储顺序，使用QHash更快。    


- **QMultiMap**  

多值映射。    

- **QHash**  

散列表实现的字典。  

- **QMultiHash**  

多汁映射。  


<br />  
<br />  
<br />  
<br />  

# 容器迭代器    


## STL和Java类型迭代器    

Qt有两种迭代器类：Java类型迭代器和STL类型的迭代器。    

## foreach    

Qt还提供一个关键字`foreach`（实际上是一个宏），用于方便地访问容器里的所有数据项。  

```C++  
//示例：  
QLinkedList<QString> list;
QString str;
foreach(str, list)
{
    qDebug() << str;
}

//或者：
QLinkedList<QString> list;
foreach(const QString &str, list)
{
    qDebug() << str;  
}
```  




<br />
<br />
<br />
<br />

# Qt类库的模块    

## Qt基本模块    

`Qt Core`  
`Qt GUI`  
`Qt Multimedia`  
`Qt Multimedia Widget`  
`Qt Network`  
`Qt QML`  
`Qt Quick`    
`Qt SQL`  
......  


需要在项目中使用模块，就在项目配置中加入语句：    
`QT += xxx`    
不使用某一功能，则需要加入：    
`QT -= yyy`    
示例：    
`QT += sql`  
`QT -= gui`    


## Qt附加模块    

`Active Qt`用于开发使用ActiveX和COM的Windows应用程序。  
`Qt 3D`支持2D和3D渲染，提供用于开发近实时仿真系统的功能。  
......    


## 其他模块    

增值模块：商业版许可的Qt才有的。    
技术预览模块：开发和测试阶段的模块。  
Qt工具：在所有支持平台上都可以使用，用于帮助应用程序开发和设计。    




（END）  