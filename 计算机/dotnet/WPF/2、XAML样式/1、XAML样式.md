# 样式化XAML应用程序    

> 当WindowsForms是创建桌面应用程序的技术时，用户界面没有提供许多设置应用程序样式的选项。但是WPF改变了这一切，WPF基于DirectX，从而提供了向量图形，可以定制不同的外观。    


<br />
<br />

## 形状(Shape)    

形状时XAML的核心元素。利用形状，可以绘制各种二维图形，这些图形用派生自抽象类`Shape`的类表示。    

示例：  
```XML  
<Canvas>
    <!--绘制椭圆-->
    <Ellipse Canvas.Left="10" Canvas.Top="10" Width="10" Height="10" Stroke="Blue" StrokeThickness="4" Fill="Yellow" />
    <!--绘制路径-->
    <Path Name="line1" Stroke="Blue" StrokeThickness="4" Data="M 40,74 Q 57,95 80,74" />
</Canvas>  
```


椭圆线条这些XAML元素都可以通过编程方式访问。只需要设置`Name`或者`x:Name`属性。  
例如线条的Name属性设置为"line1"，就可以在代码中使用变量名"line1"来访问该元素。    

```C#  
//路径标记语言PML，M定义了起点，Q定义了二次贝塞尔曲线的控制点和端点(UWP不可用)    
line1.Data = Geometry.Parse("M 40,82 Q 57,65 80,82");  
```  



<br />
<br />

## 几何图形(Geometry)    

`Geometry`元素非常类似`Shape`。Geometry也有`LineGeomery`、`EllipseGeometry`、`RectangleGeometry`来绘制各种形状。    

但是`Geometry`和`Shape`有显著的区别。`Shape`是派生自`FrameworkElement`，而`FrameworkElement`派生自`UIElement`。所以形状会参与系统的布局，而`Geometry`类不参与布局，特性和系统开销也较少。    

> `Shape`中的`Path`可以使用Geometry来绘制。    


- **使用Segment的几何图形**    

可以使用`BezierSegment`、`LineSegment`、`ArcSegment`等元素来绘制`PathGeometry`。    

- **使用PML的几何图形**    

PML中，特殊字符定义点的连接方式。    

M --标记起点  
L --到指定点的线条命令  
Z --闭合图形的闭合命令  
H --水平线  
V --垂直线  
C --三次贝塞尔曲线  
Q --二次贝塞尔曲线  
S --光滑的三次贝塞尔曲线  
T --光滑的二次贝塞尔曲线  
A --椭圆弧    
......  


- **合并的几何图形**    

在WPF中，使用`CombinedGeometry`可以合并多个几何图形。    

```XML  
<Path.Data>
    <CombinedGeometry GeometryCombineMode="Union">
        <CombinedGeometry.Geometry1>
            <EllipseGeometry Center="80,60" RadiusX="80" RadiusY="40" />
        </CombinedGeometry.Geometry1>   
        <CombinedGeometry.Geometry2>
            <RectangleGeometry Rect="30,60 105, 50" />
        </CombinedGeometry.Geometry2>   
    </CombinedGeometry>
</Path.Data>
```  


<br />
<br />  


## 变换    

因为XAML基于矢量，所以可以缩放，旋转和倾斜。    

- **单个变换**    

```XML  
<!--缩放Scale-->
<XXX>
    <XXX.RenderTransform>
        <ScaleTransform ScaleX="1.6" ScaleY="0.8"/>
    </XXX.RenderTransform>
</XXX>

<!--平移Translate-->
<XXX>
    <XXX.RenderTransform>
        <TranslateTransform X="10" Y="-10"/>
    </XXX.RenderTransform>
</XXX>

<!--旋转Rotate-->
<XXX>
    <XXX.RenderTransform>
        <RotateTransform Angle="45" CenterX="10" CenterY="-20"/>
    </XXX.RenderTransform>
</XXX>

<!--倾斜Skew（也叫切变(Shear)）-->
<XXX>
    <XXX.RenderTransform>
        <RotateTransform Angle="45" CenterX="10" CenterY="-20"/>
    </XXX.RenderTransform>
</XXX>
```


- **组合变换和复合变换**    

```XML  
<XXX>
    <XXX.RenderTransform>
        <TransformGroup>
            <SkewTransform AngleX="15" />
            <RotateTransform Angle="45" />
            <ScaleTranform ScaleX="0.5" ScaleY="1.5" />
        </TransformGroup>
    </XXX.RenderTransform>
</XXX>
```


- **使用矩阵的变换**    

同时执行多种变换的另一个旋转是使用矩阵。  
`Matrix`属性有6个值。    

```XML  
<!---->
<XXX.RenderTransform>
    <MatrixTranform Matrix="0.5, 1.4, 0.4, 0.5, -200, 0" />
</XXX.RenderTransform>
<!---->
<XXX.RenderTransform>
    <MatrixTranform>
        <MatrixTransfrom.Matrix>
            <Matrix M11="0.5" M12="1.4" M21="0.4" M22="0.5" OffsetX="-200" OffsetY="0" />
        </MatrixTransfrom.Matrix>
    </MatrixTranform>
</XXX.RenderTransform>
```



- **LayoutTranform和RenderTranform**    

WPF不仅支持`RenderTranform`，还支持`LayoutTranform`。区别是LayoutTransform发生在布局阶段之前，RenderTranform在布局阶段之后发生。    



（END）    
