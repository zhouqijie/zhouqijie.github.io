# Unity全局光照概述    

## 相关的设置    

1. Lighting窗口的GI勾选。（RealtimeGI和BakedGI）    
2. 静态物体的LightmapStatic勾选。    
3. 光源的baking设置Mixed/Baked/Realtime。    


## 注意事项    

> Realtime GI 和 Baked GI 都只适用于静态物体被照射。    
> Realtime GI 可以让光照效果随着光源的改变而改变，而 Baked GI 则不能。    

## 光照探头    

> 动态物体的全局光照。    

1. 场景中布置光照探头，要足够密集。  
2. 动态物体的Renderer设置中LightProbes设置为BlendProbes。    

> 暂不支持动态物体对动态物体的全局光照。  
> 光照探头只可以接收光照信息但没有阴影效果。    

## 反射探头    

> 光滑物体表面反射模拟。    

有两种类型：Realtime和Baked。Baked类型不会反射动态物体。    
Realtime类型有3种刷新模式：OnAwake和EveryFrame以及Via Scripting。    

> 一般布置在场景中并设置好边界，边界内的光滑物体应用此探头。也可以附在光滑物体上，可使用Realtime-EveryFrame模式。      



（END）    