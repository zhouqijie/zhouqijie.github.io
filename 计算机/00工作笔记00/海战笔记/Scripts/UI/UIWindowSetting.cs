using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowSetting : MonoBehaviour
{
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }


    private void OnEnable()
    {
        this.transform.GetComponentInChildren<UnityEngine.UI.Slider>(true).value = SoundPlayer.Volume;
    }
}
