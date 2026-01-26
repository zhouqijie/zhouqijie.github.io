# 扩展方法    

```C#  
public static Vector3 ToVector3(this Vector4 vec4)
{
    return new Vector3(vec4.x, vec4.y, vec4.z);
}
//
static void Main()
{
    Vector4 vec4;
    Vector3 vec3 = vec4.ToVector3(); 
}
```  



# 运算符重载    

> CRE：有些C++能重载的运算符在C#是不能重载或者受限的。    

> 能重载的一元运算符:`+`、`-`、`!`、`~`、`++`、`--`、`True`、`False`      
> 能重载的二元运算符: `+`、`-`、`*`、`/`、`%`、`&`、`|`、`!`、`^`、`<<`(限制为整数)、`>>`(限制为整数)、`==`、`!=`、`>`、`<`、`>=`、`<=`    
> 不能重载的运算符：`=`、`&&`、`||`、`[]`、`()`......    



```C#    
    class Person
    {
        public string name;

        public static Person operator +(Person a, Person b)
        {
            var stu = new Person();
            stu.name = "Son";
            return stu;
        }
    }
```  



(END)    