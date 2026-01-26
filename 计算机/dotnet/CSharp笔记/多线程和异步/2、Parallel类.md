# Parallel类    

Parallel类是对线程的一个很好的抽象。该类位于System.Threading.Tasks名称空间中，提供了数据和任务并行性。    

## 使用Parallel.For()方法循环    

`Parallel.For()`类似`for`循环语句妈也是多次执行一个任务。使用Parallel.For可以并行运行迭代，迭代的顺序没有定义。    

```C#  
public static void Log(string prefix)
{
    WriteLine($"{prefix}, task:{Task.Current}, thread: {Thread.CurrentThread.ManagedThreadId}");
}
public static void Test()
{
    ParallelLoopResult result = Parallel.For((0, 10, i => {
        Log($"S {i}");
        Task.Delay(10).Wait();
        Log($"E {i}");
    }));
    WriteLine($"Is Complete :{result.IsCompleleted}");
}
//输出结果：  
//顺序不能保证，每次运行可以看到不同的结果  
//所有i的"S"和"E"在同一个线程打印。  
//在所有的"S"和"E"的最后才输出IsCompleted:true  


public static void Test2()
{
    ParallelLoopResult result = Parallel.For((0, 10, async i => {
        Log($"S {i}");
        await Task.Delay(10).Wait();
        Log($"E {i}");
    }));
    WriteLine($"Is Complete :{result.IsCompleleted}");
}
//输出结果：  
//i的"S"和"E"在不同的线程打印。  
//IsCompleted:True在所有的"S"之后打印，在所有的"E"之前打印。  
//（原因是Parallel只等待它创建的任务，不等待其他后台活动）  
```    


## 提前停止Parallel.For    

可以提前中断Parallel.For方法，而不是等待完成所有迭代。    

```C#  
public static void Test()
{
    ParallelLoopResult result = Parallel.For((0, 10, (int i, ParallelLoopState pls) => {
        if(i > 12)
        {
            pls.Break();
            Log($"break at {i}");
        }
    }));
    WriteLine($"Is Complete :{result.IsCompleleted}");
    WriteLine($"LowestBreakInstruction :{result.LowestBreakInstruction}");
}
```  


## Parallel.For的初始化    

Parallel.For需要使用几个线程来执行循环。如果需要对线程进行初始化，就可以使用`Parallel.For<TLocal>()`方法。    

```C#  
//函数签名  
public static ParallelLoopResult For<TLocal>(int fromInclusive, int toExclusive, Func<TLocal> localInit, Func<int, ParallelLoopState, TLocal, TLocal> body, Action<TLocal> localFinally);
```


## 使用Parallel.ForEach()方法循环    

`Parallel.ForEach()`方法遍历实现了IEnumerable的集合，其方式类似foreach语句，但是以异步方式遍历。    



## 通过Parallel.Invoke()方法调用多个方法    

如果多个任务将并行运行，就可以使用`Parallel.Invoke()`方法，它提供了任务并行模式。    

```C#  
public static void Test()
{
    Parallel.Invoke(Foo1, Foo2);
}
public static void Foo1()
{
    Console.WriteLine("Foo1");
}
public static void Foo2()
{
    Console.WriteLine("Foo2");    
}
```  



（END）    