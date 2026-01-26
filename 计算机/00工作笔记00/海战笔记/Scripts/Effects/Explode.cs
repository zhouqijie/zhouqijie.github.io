using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteProgram(float emTime)
    {
        Invoke("StopEm", emTime);
    }

    public void StopEm()
    {
        ParticleSystem[] syss = this.GetComponentsInChildren<ParticleSystem>();

        foreach(var sys in syss)
        {
            var em = sys.emission;
                em.enabled = false;
        }

        this.GetComponentInChildren<Light>().enabled = false;
    }
}
