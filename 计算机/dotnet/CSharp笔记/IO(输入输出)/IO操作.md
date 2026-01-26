# IO操作    

> CRE：File类、Directory类都是静态操作。而FileInfo和DirectoryInfo都是实例操作。    

## 文件    

创建文件：  
`File.Create()`    
`fileInfo.Create();`  

删除文件：  
`File.Delete()`  
`fileInfo.Delete()`  

复制文件：  
`File.Copy()`  
`fileInfo.CopyTo()`  

移动文件：  
`File.Move()`  
`fileInfo.MoveTo()`    

文件是否存在：  
`File.Exist()`  


文件信息：  
`FullName`完全限定名（包括目录路径和后缀名）  
`DirectoryName`目录名  
`Length`长度  
`IsReadOnly`只读    


## 目录    

创建目录：  
`Directory.CreateDirectory()`  
`directoryInfo.Create()`  

删除目录：  
`Directory.Delete()`  
`directoryInfo.Delete()`  

移动：  
`Directory.Move()`  
`directoryInfo.MoveTo()`  

遍历：  
`directoryInfo.GetDirectories()`所有子目录。  
`directoryInfo.GetFiles()`所有文件。  
`directoryInfo.GetFileSystemInfos()`获取文件与目录。  

<br />
<br />
<br />
<br />

(END)  