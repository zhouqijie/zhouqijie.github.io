# opengl、glew、glfw关系总结：

### glfw库:   
用于处理 各个平台 的 窗口创建，事件循环.通常可以替换成为其他，比如glut,freeglut 等界面库。  

### opengl库:   
提供基本的gl函数的支持opengl库。是写opengl程序时所必须的。  


### glew库:   
提供高版本gl函数的支持。如果不嫌麻烦的话，也可以手写函数指针，来判断各个opengl高版本函数是否支持，但是 glew库做了大大的简化，使得opengl各个版本的函数像原生函数一样，可以随意调用。glew库暂时不知道有没有其他库可以替换。  

————————————————  
原文链接：CSDN博主「aiyaya730」https://blog.csdn.net/korekara88730/article/details/78935233  
————————————————  