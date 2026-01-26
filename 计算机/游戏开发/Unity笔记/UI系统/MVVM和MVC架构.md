# MVVM    

> Baike:MVVM模式，应用的UI以及基础表示和业务逻辑被分成三个独立的类：视图，用于封装UI和UI逻辑；视图模型，用于封装表示逻辑和状态；以及模型，用于封装应用的业务逻辑和数据。    


### ViewModel和BindProperty示例    

```C#
//BindableProperty
 public class BindableProperty<T>
{
    public delegate void ValueChangedHandler(T oldValue, T newValue);
 
    public ValueChangedHandler OnValueChanged;
 
    private T _value;
    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            if (!object.Equals(_value, value))
            {
                T old = _value;
                _value = value;
                ValueChanged(old, _value);
            }
        }
    }
 
    private void ValueChanged(T oldValue, T newValue)
    {
        if (OnValueChanged != null)
        {
            OnValueChanged(oldValue, newValue);
        }
    }
 
    public override string ToString()
    {
        return (Value != null ? Value.ToString() : "null");
    }
}

//ViewModel
public class SetupViewModel : ViewModel
{
    public BindableProperty<string> Name = new BindableProperty<string>();
    public BindableProperty<string> Job = new BindableProperty<string>();
    public BindableProperty<int> ATK = new BindableProperty<int>();
    public BindableProperty<float> SuccessRate = new BindableProperty<float>();
    public BindableProperty<State> State = new BindableProperty<State>();
}
```


### 双向绑定    

- ViewModel绑定View:  

通过ViewModel的BindableProperty<>字段的onChange事件来调用View中定义的响应函数实现绑定。   

- View绑定ViewModel：  

通过在View 中定义一个OnUIElementValueChanged响应函数，当UI控件的数据改变时，在响应函数中就数据同步到ViewModel中。  



### 把数据的请求和处理放在ViewModel中    

CRE：数据请求和响应回调处理函数放在ViewModel中，在View逻辑里面就可以直接调用这些函数。    


# 参考文章    

> https://blog.csdn.net/ys5773477/article/details/52795933  


