# Job的链表兼容    

> CRE：NativeContainer目前只支持数组、字符串、哈希表等。  
> CRE：对于链表的支持，可以使用自定义的静态链表，并且静态链表的内存布局对CPU的缓存一致性也比较友好。    


## 一个静态链表示例  

```C#  


public class NativeSLL<T> : IEnumerable<T>, IEnumerable
{
    //节点
    public struct Node
    {
        public int next;
        public int prev;
        public T value;
    }
    public NativeArray<Node> nodes;

    //指针
    private int firstPtr;
    private int lastPtr;
    private int emptyFirstPtr;
    private int emptyLastPtr;


    //标记
    private int count;

    //数据
    private int datalength;
    private T defaultValue;

    // ---------------------------- Public Interface -------------------------------

    public NativeSLL(int initLength, T defaultValue = default)
    {
        this.datalength = initLength;
        this.defaultValue = defaultValue;

        //this.count = -1;
        //this.firstPtr = -1;
        //this.lastPtr = -1;
        //this.emptyFirstPtr = -1;
        //this.emptyLastPtr = -1;
        //this.nodes = default;
        Clear();
    }


    public int Count => this.count;


    public int AddLast(T newItem)
    {
        return AddLastInternal(newItem);
    }

    public void Remove(T item)
    {
        if (object.Equals(item, this.defaultValue)) return;


        for (int i = 0; i < nodes.Length; ++i)
        {
            if (object.Equals(nodes[i].value, item))
            {
                RemoveAtInternal(i);
                return;
            }
        }
    }


    public void RemoveAt(int indexInNodes)
    {
        RemoveAtInternal(indexInNodes);
    }

    public void Clear()
    {
        if (nodes.IsCreated) nodes.Dispose();
        nodes = new NativeArray<Node>(this.datalength, Allocator.Persistent);

        this.firstPtr = 0;
        this.lastPtr = 0;
        this.emptyFirstPtr = 0;
        this.emptyLastPtr = this.datalength - 1;

        for (int i = 0; i < nodes.Length; ++i)
        {
            int nodePrev = (i - 1) > -1 ? (i - 1) : -1;
            int nodeNext = (i + 1) < (nodes.Length) ? (i + 1) : -1;
            nodes[i] = new Node() { value = this.defaultValue, next = nodeNext, prev = nodePrev };
        }

        this.count = 0;
    }

    public void SetValueAt(int indexInNodes, T newvalue)
    {
        var node = this.nodes[indexInNodes];

        node.value = newvalue;

        this.nodes[indexInNodes] = node;
    }

    public IEnumerator<T> GetEnumerator()
    {
        int current = firstPtr;
        while (current != emptyFirstPtr && current != -1)
        {
            if (object.Equals(nodes[current].value, defaultValue))
            {
                Debug.LogAssertion("遍历到空元素：打印链表：");
                Log();
            }

            yield return nodes[current].value;

            if (current == lastPtr) break;

            current = nodes[current].next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        int current = firstPtr;
        while (current != emptyFirstPtr && current != -1)
        {
            if (object.Equals(nodes[current].value, defaultValue))
            {
                Debug.LogAssertion("遍历到空元素：打印链表：");
                Log();
            }

            yield return nodes[current].value;

            if (current == lastPtr) break;

            current = nodes[current].next;
        }
    }


    public void Dispose()
    {
        if(nodes.IsCreated)
            nodes.Dispose();
    }



    // ---------------------------- Private -------------------------------
    private int AddLastInternal(T newItem)
    {
        if (object.Equals(newItem, defaultValue))
        {
            Debug.LogAssertion("正在往表中添加默认值！");
        }

        //不足则扩充
        if (emptyFirstPtr == emptyLastPtr)
        {
            Extend();
        }
        //获取空位
        int target = emptyFirstPtr;

        //首空位指针更新
        int newEmptyFirst = this.nodes[emptyFirstPtr].next;
        if (newEmptyFirst < 0 || newEmptyFirst > this.nodes.Length - 1)
        {
            Debug.LogAssertion("空节点列表指针越界。打印：");
            Log();
        }
        emptyFirstPtr = newEmptyFirst;

        //旧的末数据节点更新
        nodes[lastPtr] = new Node() { value = nodes[lastPtr].value, prev = nodes[lastPtr].prev, next = target };

        //新的末数据节点设置
        nodes[target] = new Node() { value = newItem, prev = lastPtr, next = -1 };

        //末数据位指针更新
        lastPtr = target;

        count += 1;



        return target;
    }
    private void RemoveAtInternal(int idx)
    {
        if (idx < 0 || idx > (this.nodes.Length - 1)) return;//越界
        if (object.Equals(this.nodes[idx].value, this.defaultValue)) return;//空闲节点不能删除
        if (firstPtr == lastPtr && lastPtr == emptyFirstPtr) return; //空表  



        if (idx == firstPtr && idx == lastPtr)//唯一节点  
        {
            FreeNodeInternal(idx);
        }
        else if (idx == firstPtr)//首节点
        {
            int thisNext = nodes[idx].next;
            nodes[thisNext] = new Node() { value = nodes[thisNext].value, prev = -1, next = nodes[thisNext].next };
            firstPtr = thisNext;

            FreeNodeInternal(idx);
        }
        else if (idx == lastPtr)//末节点
        {
            int thisPrev = nodes[idx].prev;
            nodes[thisPrev] = new Node() { value = nodes[thisPrev].value, prev = nodes[thisPrev].prev, next = -1 };
            lastPtr = thisPrev;

            FreeNodeInternal(idx);
        }
        else//中间节点
        {
            int thisPrev = nodes[idx].prev;
            int thisNext = nodes[idx].next;
            if (thisPrev > -1)
            {
                nodes[thisPrev] = new Node() { value = nodes[thisPrev].value, prev = nodes[thisPrev].prev, next = nodes[idx].next };
            }
            if (thisNext > -1)
            {
                nodes[thisNext] = new Node() { value = nodes[thisNext].value, prev = nodes[idx].prev, next = nodes[thisNext].next };
            }

            FreeNodeInternal(idx);
        }

        count -= 1;
    }
    private void FreeNodeInternal(int idx)
    {
        if (idx < 0 || idx > this.nodes.Length - 1)
        {
            Debug.LogAssertion("Free Idx越界");
            Log();
        }

        int nextEmpty = nodes[emptyFirstPtr].next;
        if (emptyFirstPtr == emptyLastPtr)
            nextEmpty = emptyLastPtr;

        nodes[idx] = new Node() { value = defaultValue, prev = -1, next = emptyFirstPtr };
        nodes[emptyFirstPtr] = new Node() { value = defaultValue, prev = idx, next = nextEmpty };
        emptyFirstPtr = idx;
    }

    private void Extend()
    {
        int oldLength = this.datalength;

        //创建新的数组并拷贝数据 
        this.datalength *= 2;
        var newDataNodes = new NativeArray<Node>(this.datalength, Allocator.Temp);
        for (int i = 0; i < newDataNodes.Length; ++i)
        {
            if (i < this.nodes.Length)
            {
                newDataNodes[i] = this.nodes[i];
            }
            else
            {
                int nodePrev = (i - 1) > -1 ? (i - 1) : -1;
                int nodeNext = (i + 1) < (newDataNodes.Length) ? (i + 1) : -1;
                newDataNodes[i] = new Node() { value = this.defaultValue, next = nodeNext, prev = nodePrev };
            }
        }
        //指向新数组
        if (this.nodes.IsCreated) this.nodes.Dispose();
        this.nodes = new NativeArray<Node>(this.datalength, Allocator.Persistent);
        this.nodes.CopyFrom(newDataNodes);
        newDataNodes.Dispose();


        //首尾空节点指针设置  
        this.nodes[emptyLastPtr] = new Node() { value = this.defaultValue, prev = this.nodes[emptyLastPtr].prev, next = (oldLength - 1 + 1) };
        this.nodes[(oldLength - 1 + 1)] = new Node() { value = this.defaultValue, prev = emptyLastPtr, next = this.nodes[(oldLength - 1 + 1)].next };
        emptyLastPtr = this.nodes.Length - 1;

    }


    public void Log()
    {
        System.Text.StringBuilder strb = new System.Text.StringBuilder();

        strb.Append("************ StaticLinkedList(size:" + this.nodes.Length + ") *************");

        for (int i = 0; i < this.nodes.Length; ++i)
        {
            strb.Append("\n");

            if (i == firstPtr && i == lastPtr)
            {
                strb.Append("<color=#FF0000>first/last  -></color>\t");
            }
            else if (i == firstPtr)
            {
                strb.Append("<color=#FF0000>first       -></color>\t");
            }
            else if (i == lastPtr)
            {
                strb.Append("<color=#FF0000>last        -></color>\t");
            }
            else
            {
                strb.Append("<color=#FF0000>--------------</color>\t");
            }


            strb.Append("<color=#AAAA00>【");
            strb.Append(i);
            strb.Append("】</color>");

            strb.Append("(prev:" + this.nodes[i].prev + ")");

            strb.Append("<b>:" + this.nodes[i].value.ToString() + "</b>");

            strb.Append("(next:" + this.nodes[i].next + ")");


            if (i == emptyFirstPtr && i == emptyLastPtr)
            {
                strb.Append("<color=#00FF00>  ->  empty first/last</color>");
            }
            if (i == emptyFirstPtr)
            {
                strb.Append("<color=#00FF00>  ->  empty first</color>");
            }
            else if (i == emptyLastPtr)
            {
                strb.Append("<color=#00FF00>  ->   empty last</color>");
            }
        }

        Debug.Log(strb.ToString());
    }
}


```  