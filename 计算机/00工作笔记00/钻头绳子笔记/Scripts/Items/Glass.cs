using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Glass : MonoBehaviour
{
    public Transform brokenHolder;
    public GameObject whole;

    private int layerExcept1;// = LayerMask.NameToLayer("Environment");
    private int layerExcept2;// = LayerMask.NameToLayer("Default");

    private void Awake()
    {
        layerExcept1 = LayerMask.NameToLayer("Environment");
        layerExcept2 = LayerMask.NameToLayer("Default");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == layerExcept1) return;
        if (other.gameObject.layer == layerExcept2) return;

        Debug.Log(other.name + "Break glass");

        whole.SetActive(false);

        foreach(Transform t in brokenHolder)
        {
            t.GetComponent<MeshRenderer>().enabled = true;
            t.GetComponent<Collider>().enabled = true;
            t.GetComponent<Rigidbody>().isKinematic = false;
            t.GetComponent<Rigidbody>().useGravity = true;
            t.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);
            t.GetComponent<Rigidbody>().AddTorque(Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);

            t.GetComponent<MeshRenderer>().material.DOFade(0f, 3f).OnComplete(() => { Destroy(t.gameObject); });
            
        }
    }
}
