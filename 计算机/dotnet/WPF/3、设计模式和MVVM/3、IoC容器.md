# IoC容器    

使用依赖注入，还可以使用一个控制反转(Ioc)容器，让IoC容器注入依赖项。    

微软官方的IoC容器，可通过NuGet包Microsoft.Framework.DependencyInjection获得。这是一个轻量级框架，支持构造函数注入和依赖注入容器。    


- 示例：  

```C#  
//App.xaml.cs
public class App : Application
{
    private IServiceProvider RegisterServices()
    {
        var serviceConnection = new ServiceCollection();
        serviceConnection.AddTransient<BooksViewModel>();
        serviceConnection.AddSingleton<IBooksService, BooksService>();
        serviceConnection.AddSingleton<IBooksRepository, BooksSampleRepository>();

        return serviceConnection.BuildServiceProvider();
    }

    public ISeriviceProvider Container{get; private set;}

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Container = RegiseterServices();

        var window = new MainWindow();
        window.Show();
    }
}
```  

> 在App类中，可以把服务添加到`ServiceConnection`中。
> `AddTransient()`方法注册一个类型，它用类型的每个解析进行新的实例化。    
> `AddSingleton()`只实例化类型一次，每次解析类型都会返回相同的实例。    


```C#  
//BooksView.xaml.cs
public BooksViewModel ViewModel {get;} = (App.Current as App).Container.GetService<BooksViewModel>(); 
```  

> CRE：由于`BooksViewModel`依赖`IBookService`，而`BooksService`又依赖`IBooksRepository`。所以如果`SeriviceCollection`中没有添加`BooksService`和`IBooksRepository`就会在`GetService<BooksViewModel>()`时报错。    




（END）    