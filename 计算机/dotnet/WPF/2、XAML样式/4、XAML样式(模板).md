# 样式化XAML应用程序    


## 模板  

使用基于模板的XAML控件，控件的外观和功能在WPF中是完全分离的。虽然按钮有默认的外观，但可以用模板完全定制其外观。  

WPF和UWP应用程序提供了几个模板类型，它们派生自`FramworkTemplate`。    

|模板类型|说明|
|-|-|
|`ControlTemplate`|可以指定控件的可视化结构，重新设计其外观。|
|`ItemsPanelTemplate`|对于ItemsControl，赋予该模板类型，以指定其项的布局。|
|`DataTemplate`|非常适用于对象的图形表示。|
|`HierachicalDataTemplate`|用于排列对象的树形结构。|  



- **控件模板**  


```XML  
<!--1.在ControlTemplate.xaml中定义模板-->
<ResourceDictionary xmlns="...." xmlns:x="....">
    <Style x:Key="btnStyle1" TargetType="Button">
        <Setter Property="..." />
        <Setter Property="..." />
        <Setter Property="..." />
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <!--ButtonStructureDefination.....-->
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
<!--...-->

<!--2.在app.xaml中引用资源字典-->
<Application.Resources>
    <ResourceDictionary Source="Styles/ControlTemplates.xaml" />  
</Application.Resources>

<!--3.关联样式到样式上-->
<Button Style="StaticResource btnStyle1" Content="ClickMe!"><!--注：Content需要用ContentPresenter元素占位-->  
```



- **数据模板**    

ContentControl基类元素的`Content`可以是任意内容，还可以是C#对象。    

```XML
<DataTemplate x:Key="CountryDataTemplate">
    <!---->
    <Image Width="200" Source="{Binding ImagePath}" />
    <TextBlock xxx="xxx" xx="xx" Text="{Binding Name}" />
</DataTemplate>

<Button x:Name="countryButton" ContentTemplate="{StaticResource CountryDataTemplate}" />
```


```C#  
public class Country
{
    public string Name{get;set;}
    public string ImagePath{get;set;}
    public override string ToString() => Name;
}

countryButton.Content= new Country()
{
    Name = "Austria",
    ImagePath = "images/Austria.bmp"
};
```  



（END）  
