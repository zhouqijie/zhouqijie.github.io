# 视图模型(View Model)    

<br />
<br />

## 视图模型的定义    

示例：  
```C#  
public class BooksViewModel : ViewModelBase
{
    private IBooksService _booksService;
    public BooksViewModel(IBooksService booksService){}

    public List<Book> Books => _booksService.Books;
    public ICommand GetBooksCommand{get;}
    public async void OnGetBooks(){}
}

```

<br />
<br />


## 视图模型的命令实现      

视图模型提供了实现`ICommand`接口的命令。命令允许通过数据绑定分离视图和命令处理程序方法。    

```C#  
public class BooksViewModel
{
    //...
    public BookViewModel(IBooksService booksService)
    {
        //...
        GetBooksCommand = new DelegateCommand(OnGetBooks, CanGetBooks);
        AddBookCommand = new DelegateCommand(OnAddBook);
    }
    //...  
    public async void OnGetBooks()
    {
        await _booksService.LoadBooksAsync();
        //...
    }
} 

```  

在XAML代码中，`GetBooksCommand`可以绑定到Button的Command属性上：  
```XML  
<Button Content="Load" Command="{Binding ViewModel.GetBooksCommand, Mode=OneTime}">  
```  

<br />
<br />


## 视图模型的服务和依赖注入    

示例：  
```C#  
public interface IBooksService
{
    Task LoadBooksAsync();
    List<Book> Books{get;}
}

public class BooksService
{
    private IBooksRepository _booksRepository;
    public BooksService(IBookRepository repository)
    {
        _booksRepository = repository;
    }
    //...
    private ObservableCollection<Book> _books = new  ObservableCollection<Book>();
    //...

    //接口实现
    IEumerable<Book> IBooksService.Books => _books;
    //接口实现
    public async Task LoadBooksAsync{}
    {
        IEnumerable<Book> books = await _bookRepository.GetItemsAsync();
        _books.Clear();
        foreach(var b in books)
        {
            _books.Add(b);
        }
    }
}

public class BooksViewModel
{
    private IBooksService _booksService;
    public BooksViewModel(IBooksService bookService)
    {
        _booksService = booksService;
    }
    //...
} 

```  
> Repository是数据访问层和模型之间的中介，是在WPF应用程序中注入的。它抽象出了数据访问层。        

> 视图模型不需要知道IBooksService的具体实现--只需要该接口。这称为*控制反转(Inversion of Control, IoC)*。该模式命名为*依赖注入(DI)*。所需的依赖项从别的地方注入（例如WPF应用程序中）。    


<br />
<br />
<br />
<br />


# 视图(View)    

<br />
<br />

## 在视图中注入视图模型  

对于视图，重要的是视图模型是如何映射的。    

为了把视图模型映射到视图上，在后台代码中定义ViewModel属性，在其中实例化所需的视图模型。    

```C#  
public sealed partial class BookView : UserControl
{
    public BookModelView ViewModel {get;} = new BooksViewModel( (App.Current as App).BooksService );
}
```  

<br />
<br />

## WPF中的数据绑定    

对于用于WPF的数据绑定，需要在XAML代码中设置DataContext。在每一个用于元素的数据绑定中，要在父元素的树中检查DataContext，找出绑定的来源。    

```XML  
<UserControl x:Class="BooksDesktopApp.Views.BooksView"
        xmlns="..."
        xmlns:x="..."
        xmlns:local="clr-namespace:BooksDesktopApp.Views"
        x:Name="booksView"
        DataContext="{Binding ElementName=booksView}">
```  

```XML  
<!--按钮绑定命令-->
<Button Content="Load" Command="{Binding ViewModel.GetBooksCommand, Mode=OneTime}" />
<!--绑定图书列表-->
<ListBox Grid.Row="1" ItemSource="{Binding ViewModel.Books, Mode=OneTime}">
    <!--......-->
</ListBox>
```  

> 数据绑定模式OneTime没有注册更改通知。模式设置为OneWay，就注册了数据源的更改通知。    


<br />  
<br />  
<br />  
<br />  


（END）  