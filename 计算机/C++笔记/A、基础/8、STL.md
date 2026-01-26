# 智能指针    

> CRE：智能指针在析构时会自动delete。防止了很多时候忘记delete或者因为抛出异常而错过了delete。（函数终止或者异常抛出时都会自动清除堆栈并析构局部对象变量，属于C++的特性）    

## 三种智能指针的使用    

> 避免两个指针指向同一个休想，因为可能导致程序删除一个对象两次的问题。    

**`auto_ptr`**：一个特定对象只能一个智能指针拥有。赋值操作会转让所有权。    
**`unique_ptr`**：比auto更加严格。    
**`shared_ptr`**：智能更高的指针。能跟踪特定对象的引用计数。最后一个指针过期时自动调用`delete`。    



<br />  
<br />  
<br />  
<br />  

# 容器概述      


<br />
<br />

## 迭代器（Iterator）      

- 为何使用迭代器：  

模板使得算法独立于存储的数据结构，而迭代器使算法独立于使用的容器类型。    

- 迭代器分类：  

1. 输入迭代器。（程序<=容器）（不一定能让程序修改值）（不保证第二次遍历时候顺序不变）（不保证先前值能解引用）
2. 输出迭代器。（程序=>容器）（能修改容器值，但是不能读取）    
3. 正向迭代器。（只能++单向迭代）（总是按照相同的顺序遍历）（递增后前面的迭代器值依然可以解引用）（可以使得能读能写，也可以使得只能读取）  
4. 双向迭代器。（具有正向迭代器所有特性，还支持递减）    
5. 随机访问迭代器。（可以随机访问例如使用索引`[]`）    

- 迭代器示例：    

`S::iterator  (例如vector<Component *>::iterator it)`    

- 迭代器运算：    

`it++;`  //指向下一个元素，返回类型不确定。    
`++it;` //指向下一个元素，返回it自身的引用。   

- 迭代器和指针：  

迭代器是广义指针，而指针满足所有的迭代器要求。    

- Cre-补充：    

在遍历map时候：`(*it).first`会得到key。`(*it).second`会得到value。    

<br />
<br />


## 插入器（Inserter）    

> 插入迭代器可以与STL算法（如`std::copy`、`std::transform`等）一起使用，使得代码更加通用。你不需要知道目标容器的具体插入方法，只需要一个插入迭代器。      

使用`std::back_inserter`创建一个`std::back_insert_iterator`，它会将元素插入到容器的末尾。（适用于支持`push_back`操作的容器）    
使用`std::front_inserter`创建一个`std::front_insert_iterator`，它会将元素插入到容器的末尾。（适用于支持`push_front`操作的容器）    
使用`std::inserter`创建一个`std::insert_iterator`，它会将元素插入到容器的末尾。（适用于支持`insert`操作的容器）    


<br />
<br />

## 容器基本功能    

```C++  
begin()  //返回指向第一个容器的迭代器。
end()  //返回最后一个元素的下一个位置的迭代器。(即超过容器尾)

//begin       end
// |           |
//|1|2|3|4|5|6|

clear()  //清空。
empty()  //返回是否为空。
size()  //返回元素个数。(类型size_t)  


for_each(begin, end, func)//遍历  
random_shuffle(begin, end)//打乱  
sort()//排序


s1.swap(s2)  //交换s1和s2容器的内容。
```

<br />
<br />
<br />
<br />

# Ⅰ、顺序容器（即序列Sequence）      

常见的顺序容器有：动态数组`vector<T>`、双向链表`list<>`、双端队列`deque<>`、队列`queue<>`、优先队列`priority_queue<>`、栈`stack<>`等。          
`vector`是顺序结构，`list`是链式结构。      
`vector`/`list`存储的是值。所以用来存放对象时通常是存放指针。      

> CRE：C#中的List是顺序结构的，相当于C++的vector。    

## 顺序容器方法    

通用：  
```CPP  
insert();  
erase();
clear();
front();  //含义: a.begin()
back();   //含义: --a.end()
push_back();  
push_front();  
pop_front();  
pop_back();  
[n];
at(t);  //含义同[n]
```  

其他：  
```CPP  
merge(x)//(list)  将调用链表与调用链表合并，两个链表必须已经排序。合并后的经过排序的链表保存在调用链表中。  
remove(val)//(list)  从链表删除val的所有示例。  
sort()
splice(it, x)//(list)  将链表x的内容插入到it前。   
push(); //(stack/queue) 栈顶或者队尾插入。  
pop(); //(stack/queue) 栈顶或者队首删除。  
```  

## 插入元素    

`insert();` //插入元素。  
※重载：s.insert(p1, t) //在p1所指位置插入新元素t，插入后的元素夹在原p1和p1-1所指元素之间。返回一个迭代器指向新插入元素。  
※重载：s.insert(p1, n, t) //在p1所指位置插入n个新元素t，插入后的元素夹在原p1和p1-1所指元素之间。无返回值。  
※重载：s.insert(p1, q1, q2) //将[q1,q2)区间内的元素顺序插入s容器的p1位置处，插入后的元素夹在原p1和p1-1所指元素之间。  

## 删除元素    

`erase();`  
※重载：erase(p)和erase(b,e);第一个删除迭代器p所指向的元素，第二个删除迭代器b,e所标记的范围内的元素。  
※补充：erase函数的返回值是一个指向被删除元素的下一个元素的迭代器。  
使用示例：  
```C++
for (vector<Component *>::iterator it = coms.begin(); it != coms.end();)
{
	if (*it == com1)
	{
		it = coms.erase(it);
	}
	else
	{
		it++;
	}
}
delete com1;
```
`clear()`全部清除，等价于`erase(begin(), end())`。    



# Ⅱ、关联容器    


> CRE：关联容器可以理解为存储键值对的容器。    

常见的关联容器：`set<>`、`multimap<>`、`unordered_map`、`unordered_set`等。    

**无序关联容器**与**关联容器**的区别是，关联容器是基于**红黑树**的，而无需关联容器基于**哈希表**，提高了添加和删除元素的速度以及提高查找算法的效率。    

<br />
<br />

## Map    

map被定义为一对数值，即*键值对(key-value pair)*。字典就是map的一个不错实例。      

> 按key查找value时，key不存在map中，这个key会自动加入map中并给与value默认值。（最好使用`find()`函数查询是否存在key）            

任何一个key值在map中最多存在一份。如果需要存储多份相同的key值，就必须使用multimap。    

<br />
<br />

## Set    

set由一群key组合而成。如果我们想知道某值是否存在于某个集合内，就可以使用set。    

对于任何key值，set只能存储一份。如果要存储多份相同的key值，必须使用multiset。    




（END）    