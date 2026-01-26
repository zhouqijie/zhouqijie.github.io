# Hello World    


## 示例：  

C#代码：  

```C#  
//Hello.cs
public class HelloWorld 
{
  public static void Main (string[] args) 
  {
    System.Console.WriteLine ("Hello, World!");
  }
}
```  

编译：  

`mcs Hello.cs`  

执行：  

`mono Hello.exe`  



## WinForms Hello World    

```CPP
using System;
using System.Windows.Forms;

public class HelloWorld : Form
{
    static public void Main ()
    {
        Application.Run (new HelloWorld ());
    }

    public HelloWorld ()
    {
        Text = "Hello Mono World";
    }
}
```  

`csc hello.cs -r:System.Windows.Forms.dll`  
` mono hello.exe`  



## GTK# Hello World    

```CPP  
using Gtk;
using System;

class Hello
{
    static void Main ()
    {
        Application.Init ();

        Window window = new Window ("Hello Mono World");
        window.Show ();

        Application.Run ();
    }
}
```  

`mcs hello.cs -pkg:gtk-sharp-2.0`  
`mono hello.exe`  


（END）  