using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCollider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.root.CompareTag("Enemy")) return;

        var doll = other.GetComponentInParent<DamageableDoll>();

        if (doll != null)
        {
            doll.DieImmidiate();
        }
    }
}
