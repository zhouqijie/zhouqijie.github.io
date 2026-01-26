# 使用命令行编译(.NET4.6)    

对源文件允许C#命令行编译器(csc.exe)，就可以编译这个程序。要设置了某些环境变量后才能使用csc命令。        

`> csc HelloWorld.cs`    

# 使用.NET Core CLI编译    

### 设置环境    

安装.NET Core CLI工具，要把dotnet工具作为入口点来启动这些所有工具：    
`> dotnet`    

会看到，dotnet工具的所有不同选项都可用。repl命令适合于学习和测试简单的C#特性，而无需创建程序。使用dotnet工具启动repl：  
`> dotnet repl`  

这会启动一个交互式repl对话。    



### 构建应用程序    

dotnet工具提供了一种简单的方法来创建应用程序。创建一个新的目录HelloWorldApp，用命令行进入目录，然后输入命令：  
`> dotnet new`  

这个命令创建了一个Program.cs文件，一个Nuget.config文件和新的项目配置文件project.json。    

有了项目结构后，就可以使用命令下载应用程序所有的依赖项：  
`> dotnet restore`  

要编译应用程序，需要命令：  
`> dotnet build`    

还可以使用命令行把程序编译为本地代码：  
`> dotnet build --native`  

要运行应用程序，使用命令：  
`> dotnet run`  

要使用特定的框架启动应用程序，可以使用-framework选项：（这个框架必须用project.json文件配置）  
`> dotnet run --framework net46`    


### 打包和发布    

创建NuGet包：  
`> dotnet pack`    

将应用程序及其依赖项发布到文件夹以部署到托管系统：  
`> dotnet publish`    


(END)  