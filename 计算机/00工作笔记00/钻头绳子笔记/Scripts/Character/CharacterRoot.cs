using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRoot : MonoBehaviour
{
    //FACE
    private SkinnedMeshRenderer smrFace;
    private bool faceDamaging = false;      public bool FaceDamaging { set { faceDamaging = value; ResetFace(); } }
    private bool faceGrabing = false;       public bool FaceGrabing { set { faceGrabing = value; ResetFace(); } }



    void Start()
    {
        smrFace = this.GetComponentInChildren<Tag_Head>().GetComponent<SkinnedMeshRenderer>();
    }
    
    void Update()
    {
        
    }


    //------------------------------------FACE STUFF ------------------------------------

    private void ResetFace()
    {
        Vector3 faceOffset = new Vector3();
        if (faceGrabing)
        {
            faceOffset = new Vector3(0.5f, 0);
        }
        if (faceDamaging)
        {
            faceOffset = new Vector3(0.5f, 0.5f);
        }

        smrFace.material.SetTextureOffset("_MainTex", faceOffset);
    }
}
