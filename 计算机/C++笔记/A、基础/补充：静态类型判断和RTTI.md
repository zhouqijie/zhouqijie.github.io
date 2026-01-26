# 静态类型信息判断    

有时要在模板中根据不同的类型做出不同的处理，可以使用静态类型信息判断。    

<br />
<br />

## 示例    

判断类型参数T是否是双精度浮点数：  
```C++  
template<typename T> struct IsDouble{enum{value = false};}//默认情况
template<> struct IsDouble<double>{enum{value = true};}//T为double时候返回true  
```   

<br />
<br /> 

## STL中的接口    

```C++
std::is_integral<int>().value;
std::is_floating_point<int>().value;
```



<br />
<br />
<br />
<br />

# 运行时类型识别(RTTI)    

> CRE：RTTI的用途是得知指针指向的是什么类型的对象。    
> Cheng：一般编译器不建议开启这个选项，因为RTTI有一定的性能损耗。    

## RTTI工作原理    

C++有3个支持RTTI的元素：    
1. `dynamic_cast`运算符将使用一个指向基类的指针来生成一个指向子类的指针，否则返回0，即空指针。    
2. `typeid`运算符返回一个指出对象的类型的值。    
3. `type_info`结构存储了有关特定类型的信息。    

C++ RTTI只适合包含虚函数的类。    

### `dynamic_cast`运算符    

`dynamic_cast`运算符能得出**是否可以安全地将对象地址赋给特定类型的指针**。    

格式：`Person * p = dynamic_cast<Student *>(s);`    
如果转换成功，返回对象地址。否则返回空指针。    


### `typeid`和`type_info`    

`typeid`运算符能使得能够**确定两个对象是否是同种类型**。    
`typeid`用法和`sizeof`相似，接收**类名**或者**对象**作为参数。    

`typeid`返回一个对`type_info`对象的引用，`type_info`是在头文件typeinfo中定义的一个类。    
`type_info`重载了`==`和`!=`运算符以便对类型进行比较。    
`type_info`的实现随厂商而异，但都包含一个`name()`成员，该函数返回一个随实现而异的字符串，通常是类名。    


<br />
<br />
<br />
<br />

# 类型转换运算符    

> C++的类型转换相比C语言的类型转换更加严格更规范。    

- C++添加了4中类型转换运算符：    

1. `dynamic_cast`  
2. `const_cast`  
3. `static_cast`  
4. `reinterpret_cast`  

## `const_cast`运算符    

用于只有一种用途的类型转换，即改变值为`const`或`volatile`特征。    

> 提供该运算符的原因是，有时候可能需要这样一个值，它在大多数时候是常量，而有时又是可以修改的。这种情况下，可以将这个值声明为`const`，并在需要修改它的时候使用`const_cast`。    
> 这也可以通过通用类型转换`(T*)`来实现，但是为了避免同时改变类型和常量特征，所以使用`const_cast`更安全。    

## `static_cast`运算符    

格式：`static_cast<T>(exp)`    

> 只有当T类型可以隐式转换为exp所属类型或者exp可被转换为T所属类型时，上述转换才算合法。（即支持upcast和downcast）      
> 即可从基类转换为子类，或从子类转换为基类。但是不能转换为无关的类。    

由于无需类型转换，枚举值就可以被转换为整型，所以可以用`static_cast`将整型转换为枚举值。    

可以使用`static_cast`将`double`转化为`int`、将`float`转换为`long`以及其他各种数值转换。    


## `reinterpret_cast`运算符    

用于天生危险的类型转换。它不允许删除`const`，但会执行其他令人生厌的操作。    

> CRE：`reinterpret_cast`即重新解释指针。    

```CPP  
struct data{ short a; short b;}
long value = 0xA224B118;
data * p = reinterpret_cast<data *> (&value);
```  



<br />
<br />
<br />
<br />


# RTTI的其他实现方式    

### 类的Static类型字段    

```C++
class Type
{
    //...
}
class A
{
    static Type type;
    virtual Type * GetType const { return type; }
    //...
};
//每个类都要定义。可以封装成宏方便调用。    
```  

### 虚函数    

```C++  
class A
{
public:
    enum TYPE
    {
        T_A,
        T_B,
    }
    virtual TYPE GetType(){ return T_A; }
}
class B : public A
{
public:
    virtual TYPE GetType(){ return T_B; }
}
```  






（END）  