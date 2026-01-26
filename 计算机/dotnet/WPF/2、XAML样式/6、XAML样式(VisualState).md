# 样式化XAML应用程序    


## VisualState管理    

对于XAML元素，可以定义可视化状态、状态组和状态，指定状态的特定动画。    

例如WPF按钮定义了状态组`CommonState`、`FocusSatets`、`ValidationStates`。    

多个状态可以同时激活，但一个状态组内只有一个状态是激活的。    


- **用控件模板预定义状态**    

```XML
<!--示例-->
<ControlTemplate TargetType="Button">
    <Grid>
        <VisualStateManager.VisaulStateGroups>
            <VisualStateGroup x:Name="CommonStates">
                <VisualState x:Name="Normal" />
                <VisualState x:Name="Pressed" >
                    <Storyboard>
                        <!--.....-->
                    </Storyboard>
                </VisaulState>
            </VisualStateGroup>
        </VisualStateManager.VisaulStateGroups>
        <!--其他UI元素....-->
    </Grid>
</ControlTemplate>
```  



- **定义和设置自定义状态**    

```XML  

        <VisualStateManager.VisaulStateGroups>
            <VisualStateGroup x:Name="CustomStates">
                <VisualState x:Name="Enabled" />
                <VisualState x:Name="Disabled" >
                    <Storyboard>
                        <!--.....-->
                    </Storyboard>
                </VisaulState>
            </VisualStateGroup>
        </VisualStateManager.VisaulStateGroups>
```  


```C#  
VisaulStateManager.GoToState(this, "Enabled", useTransition:true);
```  


(END)    