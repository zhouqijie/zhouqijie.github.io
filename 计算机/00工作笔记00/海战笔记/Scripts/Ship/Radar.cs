using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    private IShipController controller;
    private Hull hull;


    //Temp
    [HideInInspector] public float flyingTime;
    private Vector3 target;

    private float v0;
    private float length;
    private float height;
    private Vector3 targetDir;
    private Vector3 targetDir_hor;
    [HideInInspector] public Vector3 targetFireDir;





    void Start()
    {
        controller = GetComponent<IShipController>();
        hull = GetComponentInParent<Hull>();
        InvokeRepeating("CalParacurve", Random.Range(0.1f, 0.6f), 0.5f);

        v0 = hull.secondaryV0 * 3f;
    }
    




    private void CalParacurve()
    {
        if (controller == null) return;

        //controller目标获取
        if (controller.TargetLock != null)
        {
            if (!float.IsNaN(flyingTime))
            {
                target = ((controller.TargetLock.transform.position + controller.TargetLock.GetComponent<Rigidbody>().velocity * (flyingTime + Random.Range(0f, 1f)))).OnOceanPlane();
            }
            else
            {
                target = ((controller.TargetLock.transform.position + controller.TargetLock.GetComponent<Rigidbody>().velocity * 30f)).OnOceanPlane();
            }
        }
        else
        {
            target = (controller.Target).OnOceanPlane();
        }

        //计算发射角
        Vector3 firePos = this.transform.position.OnOceanPlane() + new Vector3(0, 2f, 0);
        targetDir = target - firePos;
        targetDir_hor = new Vector3(targetDir.x, 0, targetDir.z);
        length = (new Vector3(target.x - firePos.x, 0, target.z - firePos.z)).magnitude;
        height = target.y - firePos.y;
        targetFireDir = targetDir_hor.magnitude * Mathf.Tan(Utils.CalculateAngle(length, height, v0, out flyingTime, true, 98f)) * (new Vector3(0, 1f, 0)) + targetDir_hor;

        if (float.IsNaN(targetFireDir.x) || float.IsNaN(targetFireDir.y) || float.IsNaN(targetFireDir.z))
        {
            targetFireDir = targetDir_hor.normalized + new Vector3(0f, 10f, 0f);
        }
        
    }
}
