using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Debug.Log("CanvasCount: " + FindObjectsOfType<Canvas>().Length);
            foreach(var canvas in FindObjectsOfType<Canvas>())
            {
                Debug.Log("Canvasname:" + canvas.gameObject.name);
            }

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100000f))
            {
                Debug.Log("HIT:" + hit.collider.gameObject.name);
            }
        }
    }
}
