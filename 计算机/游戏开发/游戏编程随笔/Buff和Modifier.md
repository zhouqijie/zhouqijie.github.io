# Buff和Modifier    

Buff最好分为有自主复杂行为的Buff(Buff)，和纯修改数值的Buff(Modifier)。 

Buff最好作为单独的Entity，包含DurationCom等，每种特殊效果有一个Com，比如VisualEffectCom、DamageCom等。符合纯ECS设计理念。 

Modifier可以放到ModifierContainerComponent种作为List<Modifier>字段。也是符合ECS设计理念的。 

> Buff 不再只是“改数值”，而是很快会变成一个“小行为体”时，独立 Entity 的价值才明显。 
> 不推荐把Buff作为一个Component，不符合ECS设计。

