# 语言集成查询(LINQ)    

## 概述    

- Linq表达式    

```C#  
var result = from 查询值 in 要查找的数据来源  
             where 条件  
             select 查询值  
```  

- IQueryable接口    

提供对数据源的查询进行计算的功能。  
成员：  
`FirstOrDefault()`  
`First()`  
`LastOrDefault()`  
......  





## LINQ To SQL    

1. 添加LinqToSQL类文件并映射表。会自动生成.dbml(数据库标记语言)文件。(包含表的实体类和`DataContext`类)    
2. 连接到数据库。用`DataContext`的构造函数建立连接。   
3. 进行增删改查，调用`DataContext.SubmitChanges()`提交变更。  


## LINQ To DataSet    

> LINQ TO ADO.NET的一种独立技术。    

## LINQ To Entity    

> EntityFramework(EF)的相关LINQ操作。    

## LINQ To XML    

> XML(可扩展标记语言)是树状结构的。    
> CRE：System.Xml.Linq命名空间包含一些对XML进行的LINQ操作。    
> CRE：旧的Xml操作（例如逐行读取等）在System.Xml命名空间内。  

## LINQ To Object    

> 使用LINQ可以查询集合，例如数组和泛型列表。    


（END）    