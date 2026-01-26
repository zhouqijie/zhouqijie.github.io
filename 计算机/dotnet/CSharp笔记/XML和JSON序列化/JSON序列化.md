# 一、Nuget获得Newtonsoft.Json  

PM> `Install-Package Newtonsoft.Json`  

<br />

# 二、字符串和JObject互相转换  

`JObject jObj = JObject.Parse（string json）;`

`string str = jObj.ToString();`  

<br />

# 三、匿名对象转换JObject  

`JObject jObj = JObject.FromObject(new { name = "李四", age = 23 });`  

<br />

# 四、LINQ方式解析JSON  

## 访问子节点：  

```C#
string json = @"
{
    name: 'zqj',
    age: 25,
    items:
    [
        {name: 'Phone', weight:200},
        {name: 'Keys', weight:50}
    ]
}";
JObject jobj = JObject.Parse(json);
JArray jarray = (JArray)jobj["items"];
string item1Name = (string)jobj["items"][0]["name"];

```

## SelectToken方式解析：  

```C#

string json = @"{
    school:{
    name:'实验高中',
    students:[
        {name:'张三',age:18},
        {name:'李四',age:19}
    ],
    sites:['济南','聊城']
    }
}";
JObject o = JObject.Parse(json);
//SelectToken 方法使用
string schname = (string)o.SelectToken("school.name");
Console.WriteLine(schname); //实验高中
string stuname = (string)o.SelectToken("school.students[1].name");
Console.WriteLine(stuname); //李四
//SelectToken with JSONPath
JToken stu1 = o.SelectToken("$.school.students[?(@.name=='张三')]");
Console.WriteLine(stu1); //{"name": "张三","age": 18}
Console.WriteLine(stu1["age"]); //18
IEnumerable<JToken> stus = o.SelectTokens("$..students[?(@.age>15)]");
foreach (var item in stus)
{
    Console.WriteLine(item); //{"name": "张三",   "age": 18 }  
                                //{ "name": "李四",   "age": 19}
}
//SelectToken with LINQ
// $...name  意思是从当前接口文档的1,2,3级中查找name，并返回结果
IList<string> names = o.SelectTokens("$...name").Select(q => (string)q).ToList();
Console.WriteLine(string.Join(",",names)); //实验高中,张三,李四

```
> 参考文章：https://blog.csdn.net/u011127019/article/details/52487130


<br />

# 补充  

>Unity可以使用JsonUtility进行序列化。