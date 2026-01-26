using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrill : MonoBehaviour
{
    private UICanvas uiCanvas;

    private DrillController drillCtrl;
    private Image fill;





    void Start()
    {
        drillCtrl = this.GetComponentInParent<DrillController>();
        fill = this.transform.Find("ImageFill").GetComponent<Image>();
    }

    private void GetUICanvas()
    {
    }
    

    void Update()
    {
        fill.fillAmount = 0.375f + 0.25f * Mathf.Clamp01(1f - drillCtrl.HeatPercentage);

    }
}
