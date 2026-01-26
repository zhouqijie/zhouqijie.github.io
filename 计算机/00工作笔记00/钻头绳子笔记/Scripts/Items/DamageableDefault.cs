using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableDefault : MonoBehaviour, IDamageable
{
    public GameObject gameObj => this.gameObject;

    public Transform defaultPos => null;

    public bool IsBroken => false;

    

    public void OnBreak()
    {

    }

    public void StartDamage(Collider collider, float dps)
    {
    }
    public void EndDamage()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
