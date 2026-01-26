# TransfromJob的使用示例     


```C#  

    public class Manager
    {
        public struct TransformJob : IJobParallelForTransform
        {     
            [ReadOnly]
            public NativeArray<Vector3> velocity;

            public float deltaTime;

            public void Execute(int index, TransformAccess transform)
            {
                var pos = transform.position;
                pos += velocity[index] * deltaTime;
                transform.position = pos;
            }
        }
$$
        public static TransformAccessArray accessArray;
        
        private static List<SimpleUnit> allSimpleUnits = new List<SimpleUnit>();

        private static bool jobScheduled = false;
        private static JobHandle handleTrans = default;
        private static TransformJob jobTrans = default;



        public static void RegisterUnit(SimpleUnit unit)
        {
            if (accessArray.isCreated == false)
            {
                accessArray = new TransformAccessArray(500);
            }
            allSimpleUnits.Add(unit);
            accessArray.Add(unit.transform);
        }

        public static void Clear()
        {
            allSimpleUnits.Clear();
            if(accessArray.isCreated) accessArray.Dispose();
        }

        public static void DisposeAllNativeContainer()
        {
            if(jobScheduled == true)
            {
                handleTrans.Complete();
                jobScheduled = false;
            }
            if(jobTrans.velocity.IsCreated)
            {
                jobTrans.velocity.Dispose();
            }
            if(accessArray.isCreated)
            {
                accessArray.Dispose();
            }
        }


        public static void FixedUpdate()
        {
            fixedTicksGlobal++;

            if (allSimpleUnits.Count == 0) return;
            if (accessArray.isCreated == false) return;

            //移动Job  
            if(fixedTicksGlobal % movementSkip == 0)
            {
                //有旧的任务  
                if (jobScheduled == true)
                {
                    //完成旧的任务  
                    handleTrans.Complete();
                    jobTrans.velocity.Dispose();
                    jobScheduled = false;
                }

                //新的任务  
                jobTrans = new TransformJob();
                jobTrans.deltaTime = Time.fixedDeltaTime * movementSkip;
                jobTrans.velocity = new NativeArray<Vector3>(allSimpleUnits.Count, Allocator.TempJob);
            }


            //批处理SimpleUnits  
            for (int i = 0; i < allSimpleUnits.Count; ++i)
            {
                var u = allSimpleUnits[i];

                //...

                //设置速度  
                jobTrans.velocity[i] = u.velocity;
            }


            //安排位移任务  
            if (fixedTicksGlobal % movementSkip == 0)
            {
                handleTrans = jobTrans.Schedule(accessArray);
                jobScheduled = true;
            }
        }
    }
```  