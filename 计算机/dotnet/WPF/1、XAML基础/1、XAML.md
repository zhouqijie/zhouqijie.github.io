# XAML    

> 如果要编写*UWP*、使用*WPF*的Windows应用程序、使用WF的工作流、Sliverlight等，不仅需要了解C#，还需要*XAML*。    

> System.Windows名称空间中除了System.Windows.Forms（包括较早的WindowsForms技术）之外的所有内容都属于WPF。    

<br />
<br />


## 概述    

*XAML(eXtensible Application Markup Language, 可扩展应用程序标记语言)*是一种声明性的XML语法。    

- **使用WPF把元素映射到类上**    

XAML元素通常映射到.NET类或WindowsRuntime类上。示例：   

```C#  
[STAThread]
static void Main()
{
    //按钮  
    var btn = new Button(){ Content = "Click Me!"};
    btn.Click += (s, e) =>
    {
        b.Content = "Clicked!";
    }
    //窗口  
    var w = new Window(){ Title = "Demo", Content = b };
    //应用程序
    var app = new Application();
    app.Run(w);
}
```  

使用XAML代码可以创建类似的UI:    
```XML  
<Window x:Class = "XAMLINtroWPF.MainWindow" xmlns="..." xmlns:x="" Title="XAML Demo" Height="350", Width="525">
    <!-etc.->
    <Button Content="Click Me!" Click="OnButtonClicked" />
    <!-etc.->
</Window>

<!--Application也可以用XAML定义-->

<Application x:Class="XAMLIntroWPF.App", xmlns="..." xmlns:x="..." StartUpUri="MainWindow.xaml">
    <Application.Resources>
    </Application.Resources>
</Application>
```

- **使用UWP把元素映射到类上**    

略。    

- **使用自定义.NET类**    

```C#  
public class Person{
    public string FirstName{get;set;}
    public string LastName{get;set;}
}
```
```XML  
<Window x:Class="...." xmlns="..." xmlns:x="..." xmlns:datalib="clr-namespace:DataLib;assembly=DataLib" Title="...">
    <!--XXXXX-->
        <datalib:Person FirstName="AAA" LastName="BBB">
    <!--/XXXXX-->
</Window>
```  
> 定义了一个XML命名空间别名datalib，它映射到程序集DataLib的.NET名称空间DataLib上。    
> 有了这个别名，就可以在元素前面加上别名，使用该名称空间的所有类了。    

> 对于UWP应用程序，XAML声明是不同的。    


- **把属性用作特性**    

只要属性的类型可以表示为字符串，或者可以把字符串转换为属性类型。就可以把属性设置为XML特性。    

- **把属性用作元素**    

CRE:属性可以映射为XAML元素的子元素。    

- **数组和集合用作元素**    

WPF定义元素，可以使用`x:Array`扩展，其Type特性用于指定数组元素的类型。    


<br />
<br />



## 依赖属性    

XAML使用依赖属性来完成**数据绑定**、**动画**、**属性变更通知**、**样式化**等。    

- **创建依赖属性**  


下列示例定义了"Value"、"Maximum"、"Minimum"三个属性：  
```C#  
public class MyDependencyObject: DependencyObject
{
    //Value
    public int Value
    {
        get{ return (int)GetValue{ValueProperty};}
        get{ SetValue{ValueProperty, value};}
    }
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(MyDependencyObject));
    
    //Maxinum
    public int Maximum
    {
        get{ return (int)GetValue{MaximumProperty};}
        get{ SetValue{MaximumProperty, value};}
    }
    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(nameof(Maximum), typeof(int), typeof(MyDependencyObject));
    
    //Minimum
    public int Minimum
    {
        get{ return (int)GetValue{MinimumProperty};}
        get{ SetValue{MinimumProperty, value};}
    }
    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(nameof(Minimum), typeof(int), typeof(MyDependencyObject));
}
```  


- **值变更回调和事件**  

为了获得值变更的信息，依赖属性还支持**值变更回调**。在属性值发生变化时调用的`DependencyProperty.Register()`方法中，可以添加一个`DependencyPropertyChanged`事件处理程序。    


```C#  
public class MyDependencyObject: DependencyObject
{
    public int Value
    {
        get{ return (int)GetValue{ValueProperty};}
        get{ SetValue{ValueProperty, value};}
    }
    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(MyDependencyObject), new PropertyMetadata(0, OnValueChanged, CoerceVlaue));

    private static void OnValueChange(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        //...
    }
    private static object CoerceValue(DependencyObject d, object baseBalue)
    {
        int newValue = (int)baseValue;
        //...
        return newValue;
    }
}
```  

> 利用WPF，依赖属性支持强制检查，可以检查属性的值是否有效，例如是否在某个取值范围内。    
> 把方法CoerceValue()传递给PropertyMetadata对象，再传递为DependencyProperty.Register()的一个参数。现在，SetValue方法的实现代码中，对属性值的更改每次都调用CoerceValue方法。  



<br />  
<br />  


## 路由事件    

在基于XAML的应用程序中，路由事件扩展了事件模型。元素包含元素，组成了一个层次模型。在路由事件中，事件通过元素的层次结构路由。    

如果路由事件在控件中触发，例如按钮，那么可以用按钮处理事件，但它**向上路由到所有父控件那里也可以触发**。这称为**冒泡**。    

> 把事件的`Handled`属性设置为true，可以阻止事件冒泡到父控件。    

- **UWP的路由事件**    

略。    

- **WPF的冒泡和隧道**    

在WPF中，支持路由的事件比UWP应用程序支持的更多。除了沿着控件层次向上冒泡，WPF还支持**隧道**，隧道事件和冒泡方向相反-从外部进入内部控件。事件要么是冒泡事件，要么是隧道事件，要么是直接事件。    

事件经常成对定义，PreviewMouseMove是一个隧道事件，从外到内。MouseMove事件跟在PreviewMouseMove之后，它是一个冒泡事件，从内到外。    


- **WPF实现自定义路由事件**    

要让MyDependencyObject支持在值改变时触发事件，必须改为继承自`UIElement`，因为这个类定义了AddHandler和RemoveHandler方法。    


<br />
<br />


## 附加属性    

依赖属性是可用于特定类型的属性。而通过*附加属性AttachedProperty*，可以为其他类型定义属性。    

一些容器控件为其子控件定义了附加属性：  
```XML  
<DockPanel>
    <Button Content="Top", DockPanel.Dock="Top" />  
    <Button Content="Left", DockPanel.Dock="Left" />  
</DockPanel>
```  

附加属性的定义与依赖属性非常相似。定义附加属性的类必须派生自DependencyObject基类，并定义一个普通的属性。接着不调用DependencyObject的Register()方法，而是调用`RegisterAttached()`方法。该方法注册一个附加属性，现在它可以用于每个元素。    





<br />  
<br />  


## 扩展标记    

通过扩展标记，可以扩展XAML的元素或特性语法。如果XML特性包含花括号`{}`，就表示这是标记扩展的一个符号。    

```XML  
<!---->
<Button Content="Test" Background="{StaticResource gradientBrush1}">
<!---->
<Button Content="Test">
    <Button.Background>
        <StaticResourceExtension ResourceKey="gradientBrush1">
    </Button.Background>
</Button>
```  


- **创建自定义标记扩展**  

> WPF可以创建自定义的标记扩展，而UWP只能使用预定义的标记扩展。    

要创建标记扩展，可以定义继承自`MarkupExtension`基类的一个类。大多数标记扩展都有Extension后缀。    

有了自定义标记扩展后，就只需要重写`ProvideValue(IServiceProvider)`方法，它返回扩展的值。返回的类型用`MarkupExtensionReturnType`特性注解类。        

通过该方法的参数（`IServiceProvider`接口）可以查询不同的服务。例如`IProvideValueTarget`或`IXamlTypeResolver`。IXamlTypeResolver可用于把XAML元素名解析为CLR对象。    

```C#  
[MarkupExtensionReturnType(typeof(string))]
public class CalculatorExtension : MarkupExtension
{
    //...
    public override object ProvideValue(IserviceProvider serviceProvider)
    {
        IProvideValueTarget provideValue = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;  
        //...
    }
}
```

- **XAML定义的标记扩展**    

XAML定义的标记扩展也很常用。例如：  
1. `x.Array`定义为`ArrayExtension`。    
2. `x.Type`定义为`TypeExtension`。它根据字符串输入返回类型。    
3. `x:Null`定义为`NullExtension`。可以用在XAML中把值设置为空。  
4. `x:Static`定义为`StaticExtension`。可调用类的静态成员。  



（END）    