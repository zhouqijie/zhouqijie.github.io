# ADO.NET    

> ADO.NET是一个用于数据库访问的类库。    
> 提供了一系列方法，用于支持对SQL和XML的访问。  


```C#  
//             [DataSet]  DataSet对象(含DataTable)
//            /   ↑
//           /    |
// [Application]  |  --[DataAdaptor]  
//           \    |
//            \   |
//             [Command]
//                |
//                | -- [Connection]
//                |
//             [数据源(SQL Server、XML等)]
//
//
```  

- 两种更新方式    

1. 使用Command直接修改。    
2. 使用DataSet存放并修改，再用Update方法更新回数据库。    