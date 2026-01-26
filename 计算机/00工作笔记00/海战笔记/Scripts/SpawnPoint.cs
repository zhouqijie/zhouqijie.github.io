using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public int team;
    // Start is called before the first frame update
    void Awake()
    {
    }
    
    private void OnDrawGizmos()
    {
        Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0, 200f, 0), team == 0 ? Color.green : Color.red);

        Gizmos.DrawWireSphere(this.transform.position, FindObjectOfType<Game>().viewDistance);
    }
}
