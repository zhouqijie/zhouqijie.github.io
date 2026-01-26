# 样式化XAML应用程序    


## 动画    

> 在动画中，可以使用移动的元素、颜色变化、变换等制作平滑的变换效果。  

- **时间轴(Timeline)**  

Timeline定义了值随时间的变化方式。   

有不同类型的时间轴，可用于改变不同类型的值。所有时间轴基类都是`Timeline`。连续改变double值可以用`DoubleAnimation`类。改变int值可以用`Int32Animation`，连续改变点可以用`<PointAnimation>`，连续改变颜色可用`ColorAnimation`。    

`Storyboard`类可以用于合并时间轴，它派生自`TimelineGroup`基类，而`TimelineGroup`派生自`Timeline`。    

```XML  
<EventTrigger>
    <BeginStoryboard>
        <Storyboard x:Name="xxx">
            <DoubleAnimation Duration="0:0:10" To="5"
                Storyboard.TargetName="ellipse1" 
                Storyboard.TargetProperty="(UIElement.RenderTransform).(CompositeTransform.ScaleX)" />
        </Storyboard>       
    </BeginStoryboard>    
</EventTrigger>     
```  

|Timeline属性|说明|
|-|-|
|AutoReverse|指定连续改变的值在动画结束后是否返回初始值|
|SpeedRatio|改变动画的移动速度|
|BeginTime|指定从触发器事件开始到动画开始之间的时间长度|
|Duration|动画重复一次的时长|
|RepeatBehavior|定义动画的重复次数或时间|
|FillBehavior|如果父元素的时间轴有不同的持续时间，该属性就很重要|  





- **缓动函数(EasingFuntion)**    

动画类有`EasingFuntion`属性。这个属性接受一个实现了`IEasingFuntion`的对象。通过这个类型，缓动函数对象可以定义值如何随时间变化。    

```XML  
<DoubleAnimation xx="xx">
    <DoubleAnimation.EasingFuntion>
        <BounceEase EasingMode="EaseInOut" />
    </DoubleAnimation.EasingFuntion>
</DoubleAnimation>
```  


- **关键帧动画**    

如果需要为动画指定几个值，可以使用关键帧动画。  

`DoubleAnimationUsingKeyFrames`是double类型的关键帧动画。其他的有`Int32AnimationUsingKeyFrames`、`PointAnimationUsingKeyFrames`以及`ObjectAniamtionUsingKeyFrames`等。    



（END）  