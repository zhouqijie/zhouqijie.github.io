# Animator  

### Layer  

CRE：下面的层级会覆盖/混合上面的层级。可以设置遮罩防止局部被覆盖。  

CRE：UnityIK可以单独设置一个允许IK的层级。状态默认Idle。  


### 关于SetTrigger    

1. Trigger触发后会消失，如果没有触发会一直存在，需要Reset。    
2. Trigger可能同时触发两个层级的状态转换。多层级共用Trigger时可能会出问题。    
3. 可以用协程设置Bool参数代替Trigger。    