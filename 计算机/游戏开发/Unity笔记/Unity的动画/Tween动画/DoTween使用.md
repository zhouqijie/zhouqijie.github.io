# 零、简介

DOTween是脚本化动画API插件。可以实现各种想要的缓动/动画效果。

# 一、DOTween类关系  

## `ABSSequentiable`  
↓  
## `Tween`  
↓  
## `Tweener`  
↓  
## `TweenerCore`  


# 二、DOTween简单使用  

示例：  
`Tween tween = t.DOMove(t.position + new Vector3(0, 0, 1f), 0.5f);`  

函数列表：
```CSharp
//十秒内X,Y,Z 局部坐标（localPosition）移动到  10，10，10 位置
        //transform.DOBlendableLocalMoveBy(new Vector3(10, 10, 10), 10);
        //十秒内 X,Y,Z 方向的局部旋转（localPosition），转动到 30，30，30
        //transform.DOBlendableLocalRotateBy(new Vector3(30, 30, 30), 10);
        //十秒内X,Y,Z坐标移动到 自身坐标 加 new Vector3（ 10，10，10） 位置 原始 坐标 3，3，3，移动后 13，13，13
        //transform.DOBlendableMoveBy(new Vector3(10, 10, 10), 10);
        //十秒内X,Y,Z自身旋转到 30，30，30（有父物体的相对于父物体）
        //transform.DOBlendableRotateBy(new Vector3(30, 30, 30), 10);
        //十秒内 自身X,Y,Z方向的比例 加 3，3，3如原始比例 2，1，1 变化后5，4，4
        //transform.DOBlendableScaleBy(new Vector3(10, 10, 10), 10);
        //执行该方法，变化立即结束，且完成移动
        //transform.DOComplete();
        //在变化过程中执行该方法，则物体慢慢的变回原样，如果变化已经完成，该方法无效
        //transform.DOFlip();
        // 变化过程中执行该方法，则物体变化到 第二秒 时该物体的位置、比例等
        //transform.DOGoto(2);
        //十秒内 弹跳 3次
        //transform.DOJump(new Vector3(10, 10, 10), 3, 10);
        //停止掉当前的变化
        //transform.DOKill();
        // 十秒内 弹跳 3次， 局部坐标最终变化为  10， 0， 10
        //transform.DOLocalJump(new Vector3(10, 10, 10), 3, 10);
        // 5 秒内， 局部坐标变化到  10，10，10
        //transform.DOLocalMove(new Vector3(10, 10, 10), 5);
        // 10 秒内 X 局部坐标变换到 5
        //transform.DOLocalMoveX(5, 10);
        // 10 秒内 Y 局部坐标变化到 5
        //transform.DOLocalMoveY(5, 10);
        //10 秒内 Z 局部坐标变化到 5
        //transform.DOLocalMoveZ(5, 10);
        //transform.DOLocalPath();
        //5 秒内 局部旋转变化到  10，10， 10
        //transform.DOLocalRotate(new Vector3(10, 10, 10), 5);
        // 自身朝向 坐标（10，10，10）
        //transform.DOLookAt(new Vector3(10, 10, 10), 5);
        // 5 秒内 移动到 坐标 （10，10，10）
        //transform.DOMove(new Vector3(10, 10, 10), 5);
        //10 秒内 X 局部坐标变化到 5
        //transform.DOMoveX(5, 10);
        //10 秒内 Y 局部坐标变化到 5
        //transform.DOMoveY(5, 10);
        //10 秒内 Z 局部坐标变化到 5
        //transform.DOMoveZ(5, 10);
        //
        //transform.DOPath();
        //执行该方法停止 变化
        //transform.DOPause();
        //transform.DOPlay();
        //变化结束前调用该方法，物体回到原始位置
        //transform.DOPlayBackwards();
        //执行 transform.DOPlayBackwards(); 物体回到原始位置
        //执行 下面方法则再次变化
        //transform.DOPlayForward();
        //冲压机，在 5 秒内在原始坐标和下面坐标之间，来回冲压
        //transform.DOPunchPosition(new Vector3(10, 10, 10), 5);
        //冲压机，在 5 秒内在原始旋转和下面角度之间，来回冲压变化
        //transform.DOPunchRotation(new Vector3(50, 50, 50), 5);
        //冲压机，在 5 秒内在原始比例和下面比例之间，来回冲压变化
        //transform.DOPunchScale(new Vector3(5, 5, 5), 5);
        //在变化结束之前，执行该方法，则重新开始变化
        //transform.DORestart();
        //变化过程中执行该方法，回到原始
        //transform.DORewind();
        // 10 秒内 旋转角度 到  （50，50，50）
        //transform.DORotate(new Vector3(50, 50, 50), 5);
        // 10 秒内 比例变化到  （5，5，5）
        //transform.DOScale(new Vector3(5, 5, 5), 5);
        // 10 秒内 X 比例变化到 5
        //transform.DOScaleX(5, 10);
        // 10 秒内 Y 比例变化到 5
        //transform.DOScaleY(5, 10);
        // 10 秒内 Z 比例变化到 5
        //transform.DOScaleZ(5, 10);
        // 10 秒内 物体 X,Y,Z 坐标在   自身-5 到 自身加 5 之间震动
        //transform.DOShakePosition(10, new Vector3(10, 10, 10));
        // 10 秒内， 物体 X,Y,Z 旋转角度在 自身-5 到 自身加 5 之间震动
        //transform.DOShakeRotation(10, new Vector3(10, 10, 10));
        // 10 秒内， 物体 X,Y,Z 比例在 自身-5 到 自身加 5 之间震动
        //transform.DOShakeScale(10, new Vector3(10, 10, 10));
        //在变化过程中执行该方法，停止、开始、停止、开始
        //transform.DOTogglePause();
        // 执行该方法，坐标立即变化为 0，5，0， 从 0，5，0 两秒移动到初始位置
        //transform.DOMove(new Vector3(0, 5, 0), 2).From();
        // 执行该方法，移动到相对于原始位置 6，0，2 的位置
        // 如原始位置 3，2，1。 移动后位置为 3 6，2 0，2 2 即 9，2，4
        //transform.DOMove(new Vector3(6, 0, 2), 2).SetRelative();
```

# 三、Tween的链式调用  

Tween链式调用示例：  
```CSharp
Tween tween1 = DOTween.To
              (
                  () => t.position,
                  (x) => t.position = x,
                  t.position + new Vector3(-2f, 0f, 0f),
                  0.25f
              ).OnComplete(
                () => t.transform.DOMoveX(this.transform.position.x + 2f, 0.25f)

              ).SetEase(Ease.OutSine);
//getter 用来获取当前需要设置的值，setter用来设置当前数值，参数x是有DoTween计算过后的数值。endValue就是最终的数值，duration是使用的时间。和标准的Tween动画是一样的。
```

On开头的回调方法：  
```CSharp
Tween tween = DOTween.To(() => {}});
tween.OnStart()
.OnKill()
.OnPause()
.OnPlay()
.OnRewind();
tween.OnStepComplete().
OnUpdate().
OnComplete();
```

Set开头的设置方法：  
```CSharp
// SetAs(tween)        复制一个Tween对象的设置
// SetAutoKill  设置自动销毁
// SetDelay     设置延迟
// SetEase      设置缓冲类型
// SetId        设置ID 可以只用 int、string、object等类型的值
// SetLoops     设置循环类型
// SetRecyclable 设置为可回收，可循环使用的
// SetRelative   设置相对变化
// SetSpeedBased
// SetTarget
// 设置 Update 的值 告诉 Tween对象 是否忽视 Unity的 的 timeScale ，即是否受Unity 时间的影响
// SetUpdate(UpdateType.Normal, true) 设置为 true 为忽视 Unity的时间影响
//                                    设置为 false 为不忽视Unity的时间影响
```

# 四、Sequence动画队列

Sequence结构：  

<div style="border:solid 1px gray; margin: 10px 10px">
<p> Sequence</p>
<div style="float:left; border:solid 0.5px gray; margin: 10px 10px"> Tween </div>
<div style="float:left; margin: 10px 10px"> → </div>
<div style="float:left; border:solid 0.5px gray; margin: 10px 10px"> Tween </div>
<div style="float:left; margin: 10px 10px"> → </div>
<div style="float:left; border:solid 0.5px gray; margin: 10px 10px"> Tween </div>
<div style="float:left; margin: 10px 10px"> → </div>
<div style="float:left; border:solid 0.5px gray; margin: 10px 10px"> Tween </div>
<div style="float:left; margin: 10px 10px"> → </div>
<div style="float:left; border:solid 0.5px gray; margin: 10px 10px"> Tween </div>
<div style="float:left; margin: 10px 10px"> ... </div>
<div style="clear:both"></div>
</div>  

动画队列使用示例：  
```CSharp
DOTween.Sequence().Append(
                  t.DOMoveY(t.position.y - 1f, 2f)
              )
              .AppendInterval(
                  0.1f
              )
              .Append(
                  tween1
              )
              .AppendCallback(
                  () => { }  
              );
```


# 注意事项    

Dotween.To()和Sequence都不能通过Dokill来终止，必须通过Kill方法终止。    



# Reference  

> https://gameinstitute.qq.com/community/detail/116463
