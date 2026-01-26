# 样式化XAML应用程序    

<br />
<br />

## 样式(Style)和资源(Resource)    

- **样式**    

直接给元素设置样式：  
```XML  
<Button Width="150" Content="ClickMe!">
    <Button.Style>
        <Setter Property="Background" Value="Yellow" />  
        <Setter Property="FontSize" Value="14" />  
        <Setter Property="FontWeitght" Value="Bold" />  
        <Setter Property="Margin" Value="5" />  
    </Button.Style>
</Button>
```  

直接设置元素的Style对共享没有什么帮助。    
Style可以放在资源中，在资源中可以把样式赋予指定的元素、某一类型的元素、或为该样式使用一个键。    

```XML  
<Page.Resources>
    <Style TargetType="Button">
        <Setter Property="Background" Value="LemonChiffon" />
        <Setter Property="FontSize" Value="18" />
        <Setter Property="Margin" Value="5" />
    </Style>
    <Style x:Key="btnStyle1" TargetType="Button">
        <Setter Property="Background" Value="Red" />
        <Setter Property="FontSize" Value="18" />
        <Setter Property="Margin" Value="5" />
    <Style>
</Page.Resources>
<!---->
<Button Width="200" Content="XXX" Style="{StaticResource btnStyle1}" />  
```  


样式还提供了一种继承方式，一个样式可以基于另一个样式。  
```XML  
<Style x:Key="AnotherButtonStyle" BasedOn="{StaticResource btnStyle1}" TargetType="Button">
    <!---->
</Style>
```  



- **资源**    

可以在资源中定义任何可冻结(UWP为可共享)的元素。比如画笔等都可以定义为资源。    

- **代码访问资源**    

要从代码中访问资源，基类`FrameworkElement`的`Resource`属性返回ResourceDictionary。该字典使用索引器和资源名称提供对资源的访问。    

- **WPF动态资源**    

通过StaticResource标记扩展，在加载期间搜索资源。如果在运行程序的过程中改变了资源，就应该给WPF使用DynamicResource资源扩展。（UWP不支持）    

```XML  
<Button Name="Button2" Background="{DynamicResource MyBrush" />  
```

使用动态资源定义的按钮会获得动态创建的资源，而用静态资源定义的按钮看起来和以前一样。    


- **资源字典**    

使用资源字典，可以在多个应用程序之间共享文件，也可以把资源字典放在一个程序集，供应用程序共享。  

要共享程序集中的资源字典，应创建一个库。可以把资源字典文件添加到程序集。在WPF中，这个文件的构建动作必须设置为Resource。    

对于目标项目，需要引用这个库，并把资源字典添加到这个字典。通过ResourceDictionary的MergedDictionaries属性，可以使用添加进来的多个资源字典文件。可以把一个资源字典列表添加到合并的字典中。    

```XML  
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="/ResourcesLibWPF;component/Dictionary1.xaml" />
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```  



（END）  