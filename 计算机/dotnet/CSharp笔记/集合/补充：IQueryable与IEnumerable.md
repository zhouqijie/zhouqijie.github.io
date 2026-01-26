# IQueryable和IEnumerable    

## 关系图    
<img src="Images/IQueryable与IEumerable关系图.png" />  


## 两个静态类    

`Enumerable`类：  
此类中的大多数方法都定义为扩展方法IEnumerable<T>。 这意味着, 可以像调用实现IEnumerable<T>的任何对象上的实例方法一样调用它们。    

`Queryable`类：  
此类中的大多数方法被定义为扩展IQueryable<T>类型的扩展方法。 这意味着, 可以像调用实现IQueryable<T>的任何对象上的实例方法一样调用它们。    


（END）  