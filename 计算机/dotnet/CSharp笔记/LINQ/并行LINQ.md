# 并行LINQ    

> 此外。`AsParallel()`方法，它扩展了`IEnumerable<>`接口，返回`ParallelQuery<>`类，所以正常的集合类可以并行方式查询。    

> `ParallelIEnumerable`定义了`ParallelQuery<>`类的的扩展方法。        

## 并行查询    

示例：    
```C#  
var res = (from x in data.AsParallel()
        where Math.Log(x) < 4
        select x).Average();
var res = data.AsParallel().Where(x => x.Math.log(x) < 4).Select(x => x).Average();
```  


## 分区器    

`AsParallel()`方法不仅扩展了`IEnumerable<>`接口，还扩展了`Partitioner`类。通过它，可以影响要创建的分区。    

```C#  
var result = (from x in Partitioner.Create(data, true).AsParallel()
        where Math.Log(x) < 4
        select x).Average();
```  


## 取消    

.NET提供了一种标准方式，来取消长时间运行的任务，这也适合于并行LINQ。    

要取消长时间运行的查询，可以给查询添加`WithCancellation()`方法，并传递一个`CancellationToken`令牌作为参数。令牌从`CancellationTokenSource`类中创建。    

```C#  
var cts = new CancellationTokenSource();

Task.Run(() => {
    try
    {
        var res = (from x in data.AsParallel().WithCancellation(cts.Token)
        where Math.Log(x) < 4
        select x).Average();

        WriteLine($"query finish, sum:{res}");
    }
    catch
    {
        WriteLine(ex.Message);
    }
});

WriteLine("query started");
Write("cancel?  (yes/no)");
string input = ReadLine();
if(input == "yes")
{
    cts.Cancel();
}
```    


（END）    