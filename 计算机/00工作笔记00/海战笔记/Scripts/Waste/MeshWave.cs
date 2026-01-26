using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshWave : MonoBehaviour
{
    public Texture2D tex;

    private Mesh mesh;
    
    private float offsetX = 0f;
    private float offsetY = 0f;
    private float speed = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
        mesh = this.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        offsetX = (offsetX + speed * Time.deltaTime) % 1.0f;
        offsetY = (offsetY + speed * Time.deltaTime) % 1.0f;

        Vector3[] verticesNew = new Vector3[mesh.vertexCount];

        for (int v = 0; v < mesh.vertices.Length; v++)
        {
            var d = tex.GetPixelBilinear((mesh.vertices[v].x / 10f) + offsetX, (mesh.vertices[v].z / 10f) + offsetY).r;
            
            verticesNew[v] = new Vector3(mesh.vertices[v].x, d * 1f, mesh.vertices[v].z);
        }

        mesh.vertices = verticesNew;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
