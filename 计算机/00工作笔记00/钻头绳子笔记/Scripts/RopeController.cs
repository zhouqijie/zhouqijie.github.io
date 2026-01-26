using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Obi;

public class RopeController : MonoBehaviour
{
    public ObiCollider baseCube;
    public Rigidbody hook;

    private ObiRopeCursor cursor;
    private ObiRope rope;

    //Status
    private bool firing = false;
    private float targetlength;
    private float minLength;
    private float maxLength;


    void Start()
    {
        rope = this.GetComponentInChildren<ObiRope>();
        cursor = this.GetComponentInChildren<ObiRopeCursor>();

        minLength = (hook.transform.position - baseCube.transform.position).magnitude;
        maxLength = 25f;
        targetlength = minLength;
    }
    
    void FixedUpdate()
    {
        if (firing)
        {
            targetlength = (hook.transform.position - baseCube.transform.position).magnitude * 1.25f;
            
            ApprochLength(targetlength);
        }
        else
        {
            targetlength = minLength;

            ApprochLength(targetlength);
        }

        //Delay Execution

    }


    private void ApprochLength(float length)
    {
        if(Mathf.Abs(rope.RestLength - length) < 0.1f)
        {
            cursor.ChangeLength(length);
        }
        else
        {
            cursor.ChangeLength(rope.RestLength + (length - rope.RestLength) * Time.fixedDeltaTime * 10f);
        }
    }


    public void ThrowOut()
    {
        firing = true;
        targetlength = maxLength;
    }

    public void PullBack()
    {
        firing = false;
        targetlength = minLength;
    }
    
}
