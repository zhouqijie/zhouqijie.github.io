using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Float : MonoBehaviour
{
    public float up;
    public float down;


    private Wave wave;
    //Debug
    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    //Params
    public float floatFacoter;

    //Property
    private Rigidbody rigid;
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;


    //------------------FastCal-------------
    public Vector2 widthLength;



    //------------Get balence height--------
    [HideInInspector] public float BalanceHeight { get { return (-down + up) / 2f; } }

    //float
    private float frontFac = 1f;
    private float backFac = 1f;
    private float leftFac = 1f;
    private float rightFac = 1f;


    //-----------------TEMP-----------------

    int id0;
    int id1;
    int id2;

    Vector3 p0;// = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3]].x, mesh.vertices[mesh.triangles[i * 3]].y, mesh.vertices[mesh.triangles[i * 3]].z, 1f);
    Vector3 p1;// = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3 + 1]].x, mesh.vertices[mesh.triangles[i * 3 + 1]].y, mesh.vertices[mesh.triangles[i * 3 + 1]].z, 1f);
    Vector3 p2;// = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3 + 2]].x, mesh.vertices[mesh.triangles[i * 3 + 2]].y, mesh.vertices[mesh.triangles[i * 3 + 2]].z, 1f);

    Vector3 normal;// = (this.transform.localToWorldMatrix * (mesh.normals[mesh.triangles[i * 3]] + mesh.normals[mesh.triangles[i * 3 + 1]] + mesh.normals[mesh.triangles[i * 3 + 2]])).normalized;
    
    float a;// = Vector3.ProjectOnPlane(p1 - p0, Vector3.up).magnitude;
    float b;// = Vector3.ProjectOnPlane(p2 - p1, Vector3.up).magnitude;
    float c;// = Vector3.ProjectOnPlane(p0 - p2, Vector3.up).magnitude;

    float p;// = (a + b + c) / 2f;
    float area;// = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));
    
    Vector3 center;// = (p0 + p1 + p2) / 3.0f;
    
    float triangleDepth;// = 1f - center.y;

    //---------------------


    




    void Start()
    {
        wave = FindObjectOfType<Wave>();

        rigid = this.GetComponentInParent<Rigidbody>();
        if (false)
        {
            mesh = this.GetComponent<MeshFilter>().mesh;
            triangles = mesh.triangles;
            vertices = mesh.vertices;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log(this.name + ":" + Calculate());
        }
    }

    private void FixedUpdate()
    {
        stopwatch.Reset();
        stopwatch.Start();


        if(false)//GetComponent<PlayerController>() != null
        {
            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                id0 = triangles[i * 3];
                id1 = triangles[i * 3 + 1];
                id2 = triangles[i * 3 + 2];

                p0 = this.transform.localToWorldMatrix * new Vector4(vertices[id0].x, vertices[id0].y, vertices[id0].z, 1f);
                p1 = this.transform.localToWorldMatrix * new Vector4(vertices[id1].x, vertices[id1].y, vertices[id1].z, 1f);
                p2 = this.transform.localToWorldMatrix * new Vector4(vertices[id2].x, vertices[id2].y, vertices[id2].z, 1f);

                //float x1 = p0.x;

                normal = Vector3.Cross(p1 - p0, p2 - p0);

                //投影三角形海伦公式
                //a = Vector3.ProjectOnPlane(p1 - p0, Vector3.up).magnitude;
                //b = Vector3.ProjectOnPlane(p2 - p1, Vector3.up).magnitude;
                //c = Vector3.ProjectOnPlane(p0 - p2, Vector3.up).magnitude;
                //p = (a + b + c) / 2f;
                //area = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));

                //投影三角形面积计算
                float x1 = p0.x, y1 = p0.z;
                float x2 = p1.x, y2 = p1.z;
                float x3 = p2.x, y3 = p2.z;
                area = 0.5f * Mathf.Abs(x1 * y2 + x2 * y3 + x3 * y1 - x1 * y3 - x2 * y1 - x3 * y2);

                //重心坐标
                center = (p0 + p1 + p2) / 3.0f;
                //水深
                triangleDepth = wave.GetWaveHeight(center) - center.y;//水深变化函数

                float dot = Vector3.Dot(Vector3.down, normal);
                if (triangleDepth > 0f && Mathf.Abs(dot) > 0.001f)
                {
                    rigid.AddForceAtPosition(new Vector3(0, area * triangleDepth * floatFacoter * Mathf.Sign(dot) * 9.8f, 0), center, ForceMode.Force);//!!!!!!!!!!! * 1000dm^3
                }

            }
        }
        else
        {
            Vector3 posCenter = GetComponentInParent<Rigidbody>().worldCenterOfMass;

            Vector3 posL = posCenter - this.transform.right * widthLength.x;
            posL = posL - new Vector3(0f, wave.GetWaveHeight(posL), 0f);
            float floatL = Mathf.Clamp01((down - posL.y) / (up + down)) * 0.5f * GetComponentInParent<Rigidbody>().mass * 9.8f * floatFacoter * leftFac;

            Vector3 posR = posCenter + this.transform.right * widthLength.x;
            posR = posR - new Vector3(0f, wave.GetWaveHeight(posR), 0f);
            float floatR = Mathf.Clamp01((down - posR.y) / (up + down)) * 0.5f * GetComponentInParent<Rigidbody>().mass * 9.8f * floatFacoter * rightFac;

            Vector3 posB = posCenter - this.transform.forward * widthLength.y;
            posB = posB - new Vector3(0f, wave.GetWaveHeight(posB), 0f);
            float floatB = Mathf.Clamp01((down - posB.y) / (up + down)) * 0.5f * GetComponentInParent<Rigidbody>().mass * 9.8f * floatFacoter * backFac;

            Vector3 posF = posCenter + this.transform.forward * widthLength.y;
            posF = posF - new Vector3(0f, wave.GetWaveHeight(posF), 0f);
            float floatF = Mathf.Clamp01((down - posF.y) / (up + down)) * 0.5f * GetComponentInParent<Rigidbody>().mass * 9.8f * floatFacoter * frontFac;


            rigid.AddForceAtPosition(new Vector3(0, floatL, 0), posL, ForceMode.Force);//!!!!!!!!!!! * 1000dm^3
            rigid.AddForceAtPosition(new Vector3(0, floatR, 0), posR, ForceMode.Force);//!!!!!!!!!!! * 1000dm^3
            rigid.AddForceAtPosition(new Vector3(0, floatB, 0), posB, ForceMode.Force);//!!!!!!!!!!! * 1000dm^3
            rigid.AddForceAtPosition(new Vector3(0, floatF, 0), posF, ForceMode.Force);//!!!!!!!!!!! * 1000dm^3
            
        }

        

        stopwatch.Stop();
        //print(stopwatch.ElapsedMilliseconds);
    }
    
    //体积计算
    public float Calculate()
    {
        if(mesh != null)
        {
            float sum = 0f;

            for (int i = 0; i < mesh.triangles.Length / 3; i++)
            {
                Vector3 p0 = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3]].x, mesh.vertices[mesh.triangles[i * 3]].y, mesh.vertices[mesh.triangles[i * 3]].z, 1f);
                Vector3 p1 = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3 + 1]].x, mesh.vertices[mesh.triangles[i * 3 + 1]].y, mesh.vertices[mesh.triangles[i * 3 + 1]].z, 1f);
                Vector3 p2 = this.transform.localToWorldMatrix * new Vector4(mesh.vertices[mesh.triangles[i * 3 + 2]].x, mesh.vertices[mesh.triangles[i * 3 + 2]].y, mesh.vertices[mesh.triangles[i * 3 + 2]].z, 1f);

                Vector3 normal = this.transform.localToWorldMatrix * (mesh.normals[mesh.triangles[i * 3]] + mesh.normals[mesh.triangles[i * 3 + 1]] + mesh.normals[mesh.triangles[i * 3 + 2]]);

                //投影三角形海伦公式
                float a = (p1 - p0).magnitude;
                float b = (p2 - p1).magnitude;
                float c = (p0 - p2).magnitude;

                float p = (a + b + c) / 2f;
                float area = Mathf.Sqrt(p * (p - a) * (p - b) * (p - c));

                //重心坐标
                Vector3 center = (p0 + p1 + p2) / 3.0f;
                
                sum += Vector3.Project(this.transform.position - center, normal).magnitude * area * 0.333f;

            }

            return sum;
        }

        return 0f;
    }


    public void Sunk()
    {
        StartCoroutine(CoSunk());
    }

    private IEnumerator CoSunk()
    {
        yield return null;

        backFac = 0f;
        leftFac = 0f;
        rightFac = 0f;

        yield return new WaitForSeconds(2f);

        this.enabled = false;
    }
}
