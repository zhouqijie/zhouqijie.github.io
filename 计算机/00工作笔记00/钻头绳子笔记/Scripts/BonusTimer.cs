using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusTimer : MonoBehaviour
{
    private const float timeconst = 240f;// (s)



    public static BonusTimer inst = null;
    public static BonusTimer CreateInstance()
    {
        if(inst != null)
        {
            return inst;
        }
        GameObject newo = new GameObject("BonusTimer");
        newo.AddComponent<BonusTimer>();
        DontDestroyOnLoad(newo);

        inst = newo.GetComponent<BonusTimer>();
        return inst;
    }

    private float remainTime;
    
    public bool IsAvaliable { get { return remainTime < 0f; } }
    public float RemainTime { get { return remainTime; } }



    public BonusInfo bonusCurrent = null;

    void Start()
    {
        remainTime = timeconst;
    }

    void Update()
    {
        remainTime -= Time.deltaTime;
    }

    public void ResetTimer()
    {
        remainTime = timeconst;
    }
}
