# 一、概述 
## Osi网络七层模型

<escape>
<table>
<tr>
	<th>OSI网络七层模型</th>
    <th>功能</th>
	<th>TCP/IP参考模型</th>
    <th>对应网络协议</th>
</tr>
<tr>
	<td>应用层</td>
    <td>文件传输、文件服务、电子邮件、虚拟终端。</td>
	<td rowspan="3">应用层</td>
    <td rowspan="3">HTTP、FTP、DNS</td>
</tr>
<tr>
	<td>表示层</td>
    <td>数据格式化、代码转换、数据加密。</td>
</tr>
<tr>
	<td>会话层</td>
    <td>解除或者建立与其他结点的连接。</td>
</tr>
<tr>
	<td>传输层</td>
    <td>提供端对端接口。</td>
	<td>传输层</td>
    <td>TCP、UDP</td>
</tr>
<tr>
	<td>网络层</td>
    <td>为数据包选择路由</td>
	<td>网络层</td>
    <td>IP、ICMP...</td>
</tr>
<tr>
	<td>数据链路</td>
    <td>传输有地址的帧，错误检测功能。</td>
	<td rowspan="2">网络接口</td>
    <td rowspan="2">...</td>
</tr>
<tr>
	<td>物理层</td>
    <td>以二进制形式在物理媒体上传输数据。</td>
</tr>
</table>
</escape>

## TCP 和 UDP
- ### **TCP:** 
1.面向无连接。

2.面向报文。

3.不可靠传输。

4.头部开销小，高效。
- ### **TCP:** 
1.面向连接。三次握手建立连接。

2.面向字节流传输。

3.可靠传输。

4.效率较低。
## Socket抽象层
~~[](1.jpg)~~

> 两个进程需要通信必须唯一标识一个进程。本地进程通讯可以用PID。网络进程通信使用IP+协议+端口可以唯一标识网络中的一个进程。

> Socket是在应用层和传输层之间的一个抽象层，把TCP/IP层的复杂操作抽象为几个简单接口供应用层调用，以实现网络进程通信。

- ### **流式Socket(STREAM)：**
对应TCP协议。
~~[](2.jpg)~~
#### 服务端：

>1、服务器申请一个welcoming socket，绑定到一个IP和一个端口上。
>2.开始监听端口。（监听客户端连接信息）(不负责通信)
>3.接到连接请求后，产生一个新的socket(端口大于1024)与客户端建立连接并通信。
>4.原welcoming socket继续监听。
#### 服务端：
>1.客户端申请一个client socket。
>2.连接服务器指定端口。
- ### **报式Socket(DATAGRAM)：**  
对应UDP协议。

# 二、System.Net.Sockets.<span style = "color:orange">Socket</span>类

#### 构造函数：
```C#
public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType pProtocolType)
//示例：
Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
```
#### 方法：
```C#
Bind(); //绑定一个本地的IP和端口
Listen();//让Socket侦听传入的连接尝试，并指定侦听队列容量。
Connect();//初始化与另一个Socket的连接。
Accept();//接收连接并返回一个新的Socket。(阻塞线程)

Send();//输出数据到Socket。
Receive();//从Socket读取数据。（阻塞线程）


Close();//关闭socket。
Shutdown(...);//关闭socket。

```

#### 关于socket关闭：
Close()和Shutdown()都可以关闭socket，但是有区别:
>shutdown是一种优雅地单方向或者双方向关闭socket的方法。shutdown先把socket缓冲区中的数据发送出去，然后发送FIN数据包。并且这个时候，并没有释放本地资源。

>close则立即双方向暴力关闭socket并释放相关资源。closesocket会把socket缓冲区中的数据先马上丢弃，然后发送FIN数据包。并且关闭本地文件对象，释放资源。

补充：

>close把描述符的引用计数减1，仅在该计数变为0时关闭套接字（即发送FIN）。shutdown可以不管引用计数就激发TCP的正常连接终止序列。如果有多个进程共享一个socket，shutdown影响所有进程，而close只影响本进程。

# 三、流式Socket通信实现
## 示例：字符串发送（聊天室）
~~[](3.jpg)~~
```C#
//------------示例聊天室服务端代码-------------------------
Socket welcomingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
IPAddress ipAddress = IPAddress.Parse("192.168.0.101");
IPEndPoint endPoint = new IPEndPoint(ipAddress, 23333);

welcomingSocket.Bind(endPoint);
welcomingSocket.Listen(100);

while (true)
{
    Socket newSocket = welcomingSocket.Accept();

    //实例化socket连接对象。（构造函数创建新线程开始ReceiveMsg方法循环接收数据）
    //每次接收到数据都要调用列表中所有socket连接对象的SendMsg方法进行广播。
    SocketConnection conn = new SocketConnection(newSocket);
    //添加进列表。
    conns.Add(conn);
}
```

```C#
//------------示例聊天室客户端代码-------------------------
//对象声明：
public Socket clientSocket;
public IPEndPoint endPoint;
//建立Socket连接：
clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
endPoint = new IPEndPoint(IPAddress.Parse("192.168.0.101"), 23333);
clientSocket.Connect(endPoint);
//开启新线程，循环接收消息：
//略
```

## 示例：其他多种信息发送（文件/震动）

⭐设计协议：
>例如把传递的二进制信息前面都加一个字节作为标识。字符串信息用0开头+字符串字节数组。文件信息用1开头+文件二进制信息。

## 补充：判断远程主机是否断开

⭐不能使用Socket.Connected属性来判断远程主机是否断开。Socket类的Connected属性只表示最后一次I/O操作的状态，如果这之后远程主机断开了，它还一直返回true， 除非你再通过socket来发送数据。

⭐可根据Socket.Receive()方法返回长度是否为0来判断。但是只适用于ShutDown()关闭的socket，Close()关闭的和意外关闭的都不行。

⭐可使用`Poll(Int32, SelectMode)`方法检查socket状态，从而进行判断。但是此方法无法检测某些类型的连接问题，例如网络电缆损坏或远程主机已关闭意外。 您必须尝试发送或接收数据，以检测这些类型的错误。

>msdn: 为 SelectMode.SelectRead 参数指定 selectMode 以确定 Socket 是否可读。 指定 SelectMode.SelectWrite 以确定 Socket 是否可写。 使用 SelectMode.SelectError 检测错误条件。 Poll 将阻止执行到指定的时间段（以为单位） microseconds 。 如果要 microSeconds 无限期等待响应，请将参数设置为负整数。 如果要检查多个套接字的状态，可能更倾向于使用 Select 方法。


## 补充：如果远程主机意外断开

客户端意外断开时，服务器如果处于Receive中，则会错误关闭。应该在try语句块中Receive信息，并且不要在catch中throw抛出错误信息，否则也会发生错误关闭，但是可以exception.ToString()打印出来。


## 补充：心跳机制

>参考：https://blog.csdn.net/qq_23167527/article/details/54290726<br /><br />
要判定掉线，只需要send或者recv一下，如果结果为零，则为掉线。但是，在长连接下，有可能很长一段时间都没有数据往来。理论上说，这个连接是一直保持连接的，但是实际情况中，如果中间节点出现什么故障是难以知道的。更要命的是，有的节点（防火墙）会自动把一定时间之内没有数据交互的连接给断掉。在这个时候，就需要我们的心跳包了，用于维持长连接，保活。<br /><br />
它像心跳一样每隔固定时间发一次，以此来告诉服务器，这个客户端还活着。事实上这是为了保持长连接，至于这个包的内容，是没有什么特别规定的，不过一般都是很小的包，或者只包含包头的一个空包。<br /><br />
在获知了断线之后，服务器逻辑可能需要做一些事情，比如断线后的数据清理呀，重新连接呀……当然，这个自然是要由逻辑层根据需求去做了。<br /><br />
总的来说，心跳包主要也就是用于长连接的保活和断线处理。一般的应用下，判定时间在30-40秒比较不错。如果实在要求高，那就在6-9秒。<br />


