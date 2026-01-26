using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyAmmo : MonoBehaviour
{
    [HideInInspector] public int count = 30;

    void Update()
    {
        this.transform.localEulerAngles = this.transform.localEulerAngles + new Vector3(0, Time.deltaTime * 20f, 0);
    }
    
    

    private void OnTriggerEnter(Collider other)
    {
        var chara = other.GetComponentInParent<CharactorController>();
        if (chara != null)
        {
            chara.GetAmmo(count);
            Destroy(this.gameObject);
        }
    }
}
