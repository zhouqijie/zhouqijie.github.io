
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{

    //private MeshRenderer meshRenderer;
    //private Material mat;
    //private Mesh mesh;

    public float waveScale;

    private float offsetX = 0f;
    private float offsetY = 0f;
    private float speed = 20f;//const



    //private float chunkWidth = 10f;
    //private int instanceCount = 400;
    //private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
    //private ComputeBuffer positionBuffer;
    //private ComputeBuffer argsBuffer;



    void Start()
    { 
//#if INSTANCE_INDIRECT
//        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
//#endif
    }
    


    void Update()
    {
        offsetX = (offsetX + speed * Time.deltaTime) % 200f;//100米波浪
        offsetY = (offsetY + speed * Time.deltaTime) % 200f;
        
    }
    
    public float GetWaveHeight(Vector3 worldPos)
    {
        float h1 = waveScale * Mathf.Sin(((worldPos.x + offsetX) / 200f) * Mathf.PI * 2);
        float h2 = waveScale * Mathf.Sin(((worldPos.z + offsetY) / 200f) * Mathf.PI * 2);
        return h1 + h2;
    }


}
