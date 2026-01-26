# IntPtr 简介

## 简介

`IntPtr` 是 .NET 中表示平台特定指针或句柄的值类型（struct）。它的大小依赖运行时平台：在 32 位进程中为 4 字节，在 64 位进程中为 8 字节。常用于与非托管代码互操作（P/Invoke）、表示原生内存地址或系统句柄等场景。

常见用途包括：
- 传递或存储本机指针/句柄
- 与 `Marshal`、`GCHandle`、或 `SafeHandle` 一起进行托管/非托管交互
- 表示“空指针”使用 `IntPtr.Zero`

## 常用 API

- `IntPtr.Zero`：表示空指针。
- 构造函数：`new IntPtr(int)` / `new IntPtr(long)`。
- `IntPtr.Size`：指针大小（字节），等于 4 或 8。
- 转换：`ToInt32()`、`ToInt64()`（在 64 位进程中调用 `ToInt32()` 可能抛出溢出异常）。
- 指针算术：使用 `IntPtr.Add(IntPtr, int)` / `IntPtr.Subtract(IntPtr, int)`，或通过转换后进行算术运算。

## 用法示例

1) 基本创建与比较

```csharp
IntPtr p1 = IntPtr.Zero;
IntPtr p2 = new IntPtr(0x1234);
if (p1 == IntPtr.Zero) { /* 空指针处理 */ }

// 获取平台相关的大小
int ptrSize = IntPtr.Size; // 4 或 8
```

2) 分配/释放非托管内存（使用 Marshal）

```csharp
using System;
using System.Runtime.InteropServices;

IntPtr mem = Marshal.AllocHGlobal(16);
try
{
	// 写入数据
	Marshal.WriteInt32(mem, 0, 0x12345678);
	int v = Marshal.ReadInt32(mem, 0);
}
finally
{
	Marshal.FreeHGlobal(mem);
}
```

3) 与 P/Invoke 一起传递不透明句柄

```csharp
[DllImport("kernel32", SetLastError = true)]
static extern IntPtr GetCurrentProcess();

IntPtr h = GetCurrentProcess();
if (h != IntPtr.Zero) { /* 使用句柄 */ }
```

4) 使用 `GCHandle` 获取托管对象地址（注意 pinned）

```csharp
using System.Runtime.InteropServices;

GCHandle gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
try
{
	IntPtr addr = gch.AddrOfPinnedObject();
	// 传递 addr 到本机代码
}
finally
{
	gch.Free();
}
```

5) 从托管委托获取函数指针

```csharp
var del = new Action(() => Console.WriteLine("hi"));
IntPtr fp = Marshal.GetFunctionPointerForDelegate(del);
// 将 fp 传给本机作为回调
```

6) 指针算术示例

```csharp
IntPtr basePtr = new IntPtr(0x1000);
IntPtr offset = IntPtr.Add(basePtr, 0x20);
// 或者： new IntPtr(basePtr.ToInt64() + offsetValue)
```

## 注意事项与最佳实践

- 避免直接把 `GCHandle.ToIntPtr` 的值当作可解引用的原生地址。`GCHandle.ToIntPtr` 返回的是一个托管运行时内的 token/槽标识符，不能在本机端当作真实指针来读取内存；唯一安全的使用场景是当它作为不可见的标识符（opaque token）在本机端保存并在托管端恢复。（CRE:它在不同运行时实现中的含义不一样，通常是指向GCHandle的一个槽。）    
- 注意托管对象被固定（pinned）会影响垃圾回收性能，尽量缩短固定时间范围。
- 优先使用 `SafeHandle` 或自定义 `SafeHandle` 子类来管理本机句柄的生命周期，取代裸 `IntPtr`，以减少资源泄漏和异常期间的句柄泄漏风险。
