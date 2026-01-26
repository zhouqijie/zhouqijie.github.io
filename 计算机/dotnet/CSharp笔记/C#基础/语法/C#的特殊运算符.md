# C#特殊运算符    

## 类型转换运算符    

`typeof` --获得类型  
`sizeof` --获得类型占用空间大小    
`as` --安全转换  
`is` --类型判断  


## 溢出异常控制运算符    

`checked`、`unchecked` --溢出异常控制符    

示例：  
```C#  
byte b = 255;
unchecked
{
    b++;
}
WriteLine(b);
//不会抛出异常  
```



## 名称空间别名限定符    

`::` --别名限定符      

示例：  
```C#  
using std = System;
//...
void Foo()
{
    std::Console.WriteLine("Hello World!");  
}
```


## 空合并运算符    

`??` --空合并运算符    

示例：  
```C#  
int? a = null;
int b;
b = a ?? 10;
```
空合并运算符不仅对可空类型很重要，对引用类型也很重要。  

示例：  
```C#  
class Test
{
    private Person person = null;

    public Person GetPerson()
    {
        return person ?? (person = new Person());
    }
}
```  


## 空值传播运算符    


生产环境中大量代码都会验证空值条件。访问一个对象的成员变量之前需要检查它是否为null。有时很容易忘记这样的检查。    

C#6.0的新功能之一就是空值传播运算符。使用空值传播运算符会大幅简化代码。    

```C#  
//简化前：  
string city = null;
Person p = new Person();
if(p != null && p.Address != null)
{
    city = p.Address.City;
}
//简化后：  
Person p = new Person();
string city = p?.Address?.City;  
```

## 标识符名称运算符    

`nameof` --标识符名称运算符    

示例：  
```C#  
string str = "AAA";
Console.WriteLine(nameof(str));
//输出：str  
```  


CRE：nameof是较新的C#特性，在旧版本的C#也可以获取变量名。    
```C#  
public static string GetVarName(System.Linq.Expressions.Expression<Func<string, string>> exp)
{
    return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
}
string str = "AAA";
Console.WriteLine(GetVarName(p => str));
//输出：str  
```



(END)  