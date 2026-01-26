# GDI+绘图    

### Graphic创建    

`Graphics.FromImage()`（图像）  
`Graphics.CreateGraphic()`(控件)  


### 图形绘制与填充  

```C#  
Rectangle rect = new Rectangle();
graphics.FillRectangle(brush, rect);
```  


### 双缓冲技术    

> 继承一个新的控件。  
> 在构造函数中调用SetStyle()方法修改样式并开启双缓冲。（或者`this.DoubleBuffered = true`）    
> 不要在控件上直接绘制，绘制在Bitmap上然后替换控件的BackgroundImage。    

### 窗体与控件的绘制规律    

> 替换BackgroundImage会触发控件的Paint。  
> 所有控件的Paint都会触发窗体的Paint。  
> 窗体改变尺寸也会触发Paint。  


(END)    