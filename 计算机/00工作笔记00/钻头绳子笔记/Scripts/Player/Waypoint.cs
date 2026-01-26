using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Waypoint : MonoBehaviour
{
    public WaypointType locType;

    public Waypoint nextLoc;

    public GameObject[] targetObjs;
    public IDamageable[] targets;

    public void Awake()
    {
        targets = targetObjs.Select(t => t.GetComponentInChildren<IDamageable>()).ToArray();
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, 0.1f);

        if (nextLoc != null)
        {
            Debug.DrawLine(this.transform.position, nextLoc.transform.position, Color.white);
        }


        switch (locType)
        {
            case WaypointType.Walk:
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawWireSphere(this.transform.position, 1f);
                }
                break;
            case WaypointType.Jump:
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(this.transform.position, 1f);
                }
                break;
            case WaypointType.Activate:
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(this.transform.position, 1f);
                }
                break;
            case WaypointType.Aim:
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(this.transform.position, 1f);

                    for(int i = 0; i < this.targetObjs.Length; i++)
                    {
                        if(targetObjs[i] != null) Debug.DrawLine(this.transform.position, this.targetObjs[i].transform.position, new Color(1f, 0.5f, 0f, 0.5f));

                    }
                }
                break;
            case WaypointType.End:
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(this.transform.position, 1f);
                }
                break;
            default:
                break;
        }
    }
}
