# 样式化XAML应用程序    

<br />
<br />

## 画笔(Brush)    

> 使用画笔时，WPF提供的内容比UWP应用多得多。    

- **纯色画笔**    

纯色画笔`SolidColorBrush`，全部区域使用同一种颜色绘制。    

```XML  
<!---->
<Button Height="30" Background="#FFC9659C">BTN</Button>
<!---->
<Button Content="BTN" Margin="10">
    <Button.Background>
        <SolidColorBrush Color="#FFC9659C" />
    </Button.Background>
</Button>
```  

- **渐变画笔**    

渐变画笔`LinearGradientBrush`定义了`StartPoint`和`EndPoint`属性。使用这些属性可以为线性渐变定义2D坐标。默认的渐变是从(0,0)到(1,1)的对角线。    

```XML  
<Button.Background>
    <LinearGradientBrush StartPoint="0,0" EndPoint="1,1">
        <GradientStop Offset="0", Color="LightGreen" />
        <GradientStop Offset="0.4", Color="Green" />
        <GradientStop Offset="1", Color="DarkGreen" />
    </LinearGradientBrush>
</Button.Background>
```  

- **图像画笔**  

要把图像加载到画笔，可以使用`ImageBrush`元素。通过这个元素，显示ImageSource属性定义的图像。    

图像可以在文件系统中访问，或者从程序集的资源中访问。    

```XML  
<Button Content="Image Brush" Width="100" Height="80" Margin="5">
    <Button.Background>
        <ImageBrush ImageSource="Build2015.png" Opacity="0.5" />
    </Button.Background>
</Button>
```


<br />
<br />

## 仅能用于UWP的画笔  

`WebViewBrush`这个画笔使用WebView作为画笔。仅能用于UWP。    


<br />
<br />


## 仅能用于WPF的画笔    

`RadialGradientBrush`可以以放射方式产生平滑的颜色渐变。  
`DrawingBrush`定义用画笔绘制的图形。    
`VisualBrush`可以在画笔中使用其他XAML元素。可以给VisualBrush添加任意的UIElement。        


示例-使用MediaElement播放视频：  
```XML  
<Button Content="VisualBrushWithMedia" Width="200" Height="150">
    <Button.Background>
        <VisualBrush>
            <VisualBrush.Visual>
                <MediaElement Source="./IceSkating.mp4" LoadedBehavior="Play" />
            </VisualBrush.Visual>
        </VisualBrush>
    </Button.Background>
</Button>
```  




（END）  