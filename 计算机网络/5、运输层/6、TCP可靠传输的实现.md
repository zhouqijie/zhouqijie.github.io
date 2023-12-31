# TCP可靠传输的实现    

## 1、以字节为单位的滑动窗口    

TCP的滑动窗口是以字节为单位的。    

### 发送窗口和接收窗口    

发送窗口表示：在没有收到确认的情况下，可以连续把窗口内的数据都发送出去。凡是已经发送的数据，在未收到确认之前都必须展示保留以便超时重传。    

发送窗口里的序号表示允许发送的序号。上一节我们知道接收方会把自己接收窗口数值放在窗口字段发送给对方，因此A的发送窗口大小一定不能超过B的接收窗口大小。此外发送窗口大小还要受到网络拥塞程度的影响。        

发送窗口后沿的后面部分表示已发送且已收到了确认。这些数据显然不需要再保留了。而发送窗口前沿的前面部分表示不允许发送的，因为接收方都没有为这部分数据保留临时存放的缓存空间。    

发送窗口的位置由窗口前沿和后沿的位置共同决定。发送窗口后沿的变化情况有两种可能，即不动（没有收到新确认）和前移（收到新确认）。发送窗口后沿不可能向后移动，因为不能撤销已收到的确认。发送窗口前沿通常是不断向前移动，但也有可能不动。（不动的两种情况：1、没有收到新确认且对方通知的窗口大小也不变。2、收到了新的确认但对方通知的窗口缩小了使得发送窗口前沿正好不动）    

发送窗口的前沿也可能向后收缩。这发生在对方通知的窗口缩小了，但TCP标准强烈不赞成这样做。因为很可能发送方在收到这个通知以前已经发送了窗口中的许多数据，现在又要收缩窗口，不让发送这些数据，这样就会产生一些错误。    

- 发送窗口三个指针：    

P3-P1：A的发送窗口    
P2-P1：已发送但尚未收到确认的字节数    
P3-P2：允许发送但当前尚未发送的字节数（有效窗口）    


接收方B只能对按序收到的数据中最高序号给出确认（CRE：如果中间有丢失则不能对后面的给出确认）。    

发送方会使用超时计时器来重传数据（CRE：确认滞留在网络中时就需要超时重传）。    

### 窗口和缓存    

缓存空间和序号空间都是有限的，并且都是循环使用的。    

- **发送缓存用来存放**：    

1. 发送应用程序传送给发送方TCP准备发送的数据。    
2. TCP已发送出但尚未收到确认的数据。    

- **接收缓存用来存放**：    

1. 按序到达的、尚未被接收应用程序读取的数据。    
2. 未按序到达的数据。    

- **注意事项**：  

1. 虽然A的发送窗口是根据B的接收窗口设置的，但在同一时刻，A的发送窗口并不总是和B的接收窗口一样大。这是因为通过网络传送窗口值需要一定的时间滞后。（并且发送方A还可能根据网络拥塞情况适当减少自己的发送窗口值）    
2. 对于不按序到达的数据应如何处理，TCP无明确规定。如果接收方把不按序到达的数据一律丢弃，那么接收窗口的管理将会比较简单但对网络资源利用不利(重复发送较多)。因此TCP通常对不按序到达的数据总是先临时存放在接收窗口，等到字节流中缺少的字节收到后，再按序交付应用程序。    
3. TCP要求接收方必须有累积确认的功能，这样可以减少传输开销。接收方可以在合适的时候发送确认，也可以自己有数据要发送时把确认信息顺带捎上。但是注意两点：一是接收方不应该过分推迟发送确认，否则会导致发送方不必要重传，反而浪费了网络资源，TCP规定确认推迟不应该超过0.5s，如果收到一连串最大长度的报文段，则必须每隔一个报文段就发送一个确认。二是捎带确认实际上不经常发生，大多数应用程序很少同时在两个方向上发送数据。    
4. TCP通信是全双工通信。通信中每一方都有自己的发送窗口和接收窗口。    


<br />
<br />

## 2、超时重传时间的选择    

> 超时重传时间的选择是TCP最复杂的问题之一。TCP的下层是互联网环境，发送的报文段可能只经过一个高速率的局域网，也可能经过多个低速率的网络，并且每个IP数据报所选择的路由还可能不同。如果超时重传时间过短，会引起很多不必要的重传，增大网络负荷。如果超时重传时间过长，则会降低传输效率。    

TCP采用了一种自适应算法，它记录一个报文段发出的时间，以及收到相应的确认的时间，这两个时间之差就是报文段的**往返时间RTT**。TCP保留了$RTT$的一个加权平均往返时间（或称为平滑往返时间）$RTT_{smooth}$。每测量一个RTT样本，就按照以下公式重新计算。    

$RTT_{smooth} = (1 - α)RTT_{smooth} + αRTT_{new}$        

> RFC6298推荐的α值为0.125。    

显然，**超时重传时间RTO**应该大于$RTT_{smooth}$。RFC6298建议使用下式计算RTO。    

$RTO = RTT_{smooth} + 4RTT_D$    

其中$RTT_D$是$RTT$的偏差的加权平均值，RFC6298建议这样计算$RTT_D$。    

$RTT_D = {1 \over 2} RTT_{new}$    *(第一次测量)*        
$RTT_D = (1 - β)RTT_D + β|RTT_{smooth} - RTT_{new}|$    


### 超时重传导致的问题    

有时超时重传会导致收到两次相同的确认，从而影响RTT的测量。    

Karn提出了一个算法：计算$RTT_{smooth}$时，只要报文段重传了，就不采用其RTT样本。这样计算的$RTT_{smooth}$和$RTO$就比较准确。    

但是新的问题是如果时延突然增大，导致报文一直重传，但是RTO又因为重传而无法更新。修正方法是报文段每重传一次，就把RTO增大一倍，当不再发生报文段重传时，才根据公式计算RTO。    


<br />
<br />

## 3、选择确认SACK    

如果未按序收到报文段，能否设法只传送缺少的数据而不重传已经正确收到的数据？    
答案是可以，*选择确认*就是一种可行的处理方法。    

选择确认SACK的工作原理：TCP报文段首部的“确认号字段”用法仍然不变，只是在TCP报文段首部中增加SACK选项，以便报告收到的不连续的字节块的边界。    


（END）    


