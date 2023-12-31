# 远程终端协议TELNET    

用户用TELNET就可以在其所在地通过TCP连接注册(即登录)到远地的另一台主机上(使用主机名或IP地址)。    

TELNET能将用户的按键传输到远地主机，同时也能将远地主机的输出通过TCP连接返回到用户屏幕。这种服务是透明的，因为用户感到好像键盘和显示器是直接连接到远地主机上的。因此TELNET又称为**终端仿真协议**。    

TELNET也使用客户/服务器方式。在本地系统运行TELNET客户进程，而在远地则运行TELNET服务器进程。和FTP情况相似，服务器中的主进程等待新的请求，并产生从属进程来处理每一个连接。    

TELNET能够适应许多计算机和操作系统的差异。例如不同操作系统的回车和换行。例如终端一个程序时用`ESC`还是`Ctrl-C`。为了适应这种差异，TELNET定义了数据和命令应该怎样通过互联网。这些定义就是所谓的**网络虚拟终端VNT**。    

### NVT    

客户软件把用户的按键和命令转换为NVT格式，并送交服务器。服务器软件把收到的数据和命令从NVT格式转换为远地系统所需格式。向用户返回数据时，服务器把远地系统格式转换为NVT格式，本地客户再从NVT格式转换到本地系统所需的格式。　　　　

NVT的格式定义很简单。所有的通信都使用8位一个字节。在运转时，NVT使用7位ASCII码传送数据，而当高位置１时用作控制命令。ASCII码共有９５个可打印字符和33个控制字符。所有可打印字符在NVT中的意义和在ASCII码中一样。但NVT只使用了ASCII码控制字符中的几个。此外NVT还定义了两字符的`CR-LF`为标准的行结束控制符。当用户键入回车按键时，TELNET的客户就把它转换为CR-LF再进行传输。而TELNET服务器要把`CR-LF`转换为远地机器的行结束符。    

### 选项协商    

TEL的*选项协商(option Negotiation)*使TELNET客户和服务器可商定使用更多的终端功能，协商的双方是平等的。    



(END)    