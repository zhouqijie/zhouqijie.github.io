using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Door : MonoBehaviour
{
    private Transform doorAxis;
    private Transform door;

    private bool isOpened = false;

    void Start()
    {
        doorAxis = this.transform.GetChild(0);
        door = doorAxis.GetChild(0);


        InvokeRepeating("RepeatCheck", Random.Range(0.2f, 0.6f), 0.4f);
    }
    
    void Update()
    {
        
    }

    private void RepeatCheck()
    {
        CharactorController[] charactors = FindObjectsOfType<CharactorController>();

        foreach(var charactor in charactors)
        {
            if ((charactor.transform.position - this.transform.position).sqrMagnitude < Mathf.Pow(1.5f, 2))
            {
                if(isOpened == false)
                {
                    OpenDoor();
                }
                return;
            }
        }

        if(isOpened == true)
        {
            CloseDoor();
        }
    }

    void OpenDoor()
    {
        isOpened = true;

        SoundPlayer.PlaySound3D("door1",this.transform.position);

        door.DOKill();
        door.transform.DOLocalRotate(new Vector3(0, -90, 0), 0.2f);
    }

    void CloseDoor()
    {
        isOpened = false;

        SoundPlayer.PlaySound3D("door2", this.transform.position);

        door.DOKill();
        door.transform.DOLocalRotate(new Vector3(), 0.2f);
    }
}
