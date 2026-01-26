# 一、检测错误、检测状态、打印信息日志

## 编写检测错误的函数:

```CPP
bool checkOpenGLError()
{
	bool foundError = false;
	int glErr = glGetError();
	while (glErr != GL_NO_ERROR)
	{
		cout << "glError:" << glErr << endl;
		foundError = true;
		glErr = glGetError();
	}
	return foundError;
}
```
循环调用`glGetError()`检查是否发生OpenGL错误，编译错误和链接错误都可以检测。  
<br />

## 编写打印信息日志的函数：
```CPP
void printProgramLog(int program)
{
	int len = 0;
	int chWritten = 0;
	char * log;
	glGetProgramiv(program, GL_INFO_LOG_LENGTH, &len);
	if (len > 0)
	{
		log = (char *)malloc(len);
		glGetProgramInfoLog(program, len, &chWritten, log);
		cout << "ProgramInfoLog:" << log << endl;
		free(log);
	}
}
void printShaderLog(GLuint shader)
{
	int len = 0;
	int chWritten = 0;
	char * log;
	glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &len);
	if (len > 0)
	{
		log = (char *)malloc(len);
		glGetShaderInfoLog(shader, len, &chWritten, log);
		cout << "Shader info Log:" << log << endl;
		free(log);
	}
}
```
函数`glGetShaderiv()`和`glGetProgramiv()`用于返回着色器/程序的参数。例如编译状态、删除状态、信息日志长度、源码长度等。  
函数`glGetShaderInfoLog()`和`glGetProgramInfoLog()` 用于获取着色器/程序的信息日志。  

## 检测编译和链接错误：

```CPP
//glCompileShader(vShader);  //以下是检测编译错误的语句。
checkOpenGLError();
glGetShaderiv(vShader, GL_COMPILE_STATUS, &isVertCompiled);
if (isVertCompiled != 1)
{
	cout << "Vertex Compile Failed!" << endl;
	printShaderLog(vShader);
}
```

```CPP
//glLinkProgram(vfProgram);  //以下是检测链接错误的语句。
checkOpenGLError();
glGetProgramiv(vfProgram, GL_LINK_STATUS, &isLinked);
if (isLinked != 1)
{
	cout << "Link Failed" << endl;
	printProgramLog(vfProgram);
}
```
①先检测错误。  
②再检测状态。  
③如果状态是失败，则打印信息日志。  

## 补充：

1、相关函数可以放在Utility.h/cpp文件中而不是main.cpp中。  

2、可以使用`glGet()`类指令获取OpenGL某些方面的数值限制。例如用`glGetFloatv(GL_POINT_SIZE_RANGE, size)`查询点尺寸的范围。  

<br />
<br />

# 二、从文件读取GLSL源代码。

<span style="color:red">读取函数可以放在Utility.h/cpp文件中。</span>

## 读取函数：
```CPP
string readShaderSource(const char * path)
{
	string content;
	ifstream fileStream(path, ios::in);
	string line = "";
	while (!fileStream.eof())
	{
		getline(fileStream, line);
		content.append(line + "\n");
	}
	fileStream.close();
	return content;
}
```
## string转换成const char*类型：
```CPP
string strVertShader = readShaderSource("MyVertShader.glsl");
string strFragShader = readShaderSource("MyFragShader.glsl");

const char * vshaderSource = strVertShader.c_str();
const char * fshaderSource = strFragShader.c_str();
//注意：不能直接用readShaderSource().c_str()
```