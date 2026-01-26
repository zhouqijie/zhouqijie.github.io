# 流(Stream)    

> CRE：参考C++PrimerPlus对流的描述。    


## 文件流    

> CRE：对文件流的操作可看作对文件的操作。    

- 常用模式：  

`OpenOrCreate`  
`Create`  
`CreateNew`  
`Open`  
`Append`  
`Truncate`  


- 常用方法：  

`Write()`  
`Read()`  
`Flush()`  
`Close()`    


## 其他流    


- 文本文件读写：  

`StreamWriter`类。  
`StreamReader`类。  

```C#  
StreamWriter sw = new StreamWriter(path);  
sw.WriteLine();
sw.Close();
StreamReader sr = new StreamReader(path);
string str = sr.ReadToEnd();  
```  


- 二进制文件读写：  

`BinaryWriter`类。  
`BinaryReader`类。  

```C#  
//BinaryWriter和BinaryReader类实例化要用FileStream对象做参数。    
FileStream fs = new FileStream(path);
BinaryWriter br = new BinaryWriter(fs);
br.Write(bytes);
br.Close();  
fs.Close();  
```  


（END）    