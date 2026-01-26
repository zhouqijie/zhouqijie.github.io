# 可调用类型    

> 可以把函数指针、函数符、lambda表达式通称为函数对象或者*可调用类型(callable type)*。    

<br />
<br />

## **函数符**    

函数符是一个类对象，重载了`operator()()`，所以可以像调用函数一样调用它。    
（详见STL.md）  



<br />
<br />


## **Lambda表达式**    

- **概述**：    

> C++PrimerPlus："Lambda"来自数学中的λ演算。这个系统让你能够使用匿名函数-即无需给函数命名。    
在C++11中，对于接受函数指针或者函数符的函数，可以使用匿名函数定义(lambda)作为其参数。    

示例1：  
`[](int a, int b){return a + b; }`    
示例2：  
`[](int a, int b) ->int{ ...; return a + b;}`    

> 第一个示例没有声明返回类型，是自动推断的。仅当lambda完全由一条返回语句组成时自动类型推断才有用。否则需要使用新增的返回类型后置语法（第二个示例）。    
> CRE：`[]`是捕获列表，用于捕获变量。    
> CRE：没有捕获的Lambda表达式可以转换为函数指针。    


- **为何使用Lambda**:    

1. 让定义位于使用的地方附近方便阅读。翻阅源代码的时候就无需翻阅多页。    
2. 从简洁的角度看，函数符代码比函数和lambda代码更复杂。    
3. lambda拥有一些额外的功能。lambda可访问作用域内的任何动态变量。要捕获要使用的变量，可将其放在中括号中。    


- **变量捕获方式**：    

`[x]`按值捕获。  
`[&x]`按引用捕获。  
`[&x, y]`按引用捕获x，按值捕获y。    
`[=]`按值全部捕获。  


<br />
<br />
<br />
<br />


# 包装器    

> 包装器(wrapper)也叫适配器(adapter)。这些对象用于给其他编程接口提供更一致或更合适的接口。    

## function    

函数指针、函数对象、lambda表达式这些**可调用对象(callable type)**如果作为模板参数，即使类型看起来相同，模板也会实例化多次。    

> 如果函数指针、函数对象、lambda表达式如果参数和返回值相同，则称它们**特征标(call signature)**相同。    

包装器模板`function`是在头文件functional中定义的，他从调用特征标的角度定义了一个对象，可用于包装调用特征标相同的函数指针、函数对象、lambda表达式。使用包装器可以防止出现模板多次实例化。    

> CRE：包装器function类似C#的委托。  

## bind    

> `std::bind`可以用来适配函数的参数列表。    

示例：  
```CPP  
class Calculator
{
public:
	int CalSum(int a, int b)
	{
		return (a + b) * v;
	}
};
int main()
{
	Calculator * ptr = new Calculator();
	std::function<int(int, int)> f = std::bind(&Calculator::CalSum, ptr, std::placeholders::_1, std::placeholders::_2);
}
```  




<br />
<br />
<br />
<br />

# 补充    


> csblog: elephantcc:  可将bind函数看作是一个通用的函数适配器，它接受一个可调用对象，生成一个新的可调用对象来“适应”原对象的参数列表。  
> csdn:每一个类的成员函数都会被隐式的传递一个this指针。  

> csdn:`__cdecl`代表C语言的默认函数调用方式。  
> csdn:`__thiscall`对每个函数都增加了一个类指针参数。  