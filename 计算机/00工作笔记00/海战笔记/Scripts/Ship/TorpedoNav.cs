using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoNav : MonoBehaviour
{
    private Rigidbody rigid;

    private float timer;

    private float speed;

    private bool floating;
    // Start is called before the first frame update
    void Start()
    {
        rigid = this.GetComponent<Rigidbody>();
        rigid.angularDrag = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if(!floating && this.transform.position.y < -3f)
        {
            floating = true;
            rigid.isKinematic = true;
            speed = 20f;
            var em = this.GetComponentInChildren<ParticleSystem>().emission; em.enabled = true;
            this.GetComponent<Collider>().enabled = true;
        }

        if (floating)
        {
            this.transform.position += new Vector3(0f, (-0.75f - this.transform.position.y) * 0.01f, 0f);

            speed = Mathf.Clamp(speed + Time.fixedDeltaTime * 20f, 0f, 40f);

            this.transform.position += new Vector3(this.transform.forward.x, 0f, this.transform.forward.z) * Time.fixedDeltaTime * speed;
        }

        if(timer > 120f)
        {
            Destroy(this.gameObject);
        }
    }
    
}
