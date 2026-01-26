using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Displayer : MonoBehaviour
{
    private Displayer inst;

    private static UnityEngine.UI.Text displayer;
    private static bool needToInvokeClean;
    void Awake()
    {
        inst = this;
        displayer = inst.GetComponent<UnityEngine.UI.Text>();
    }
    void Update()
    {
        
    }

    public static void Display(string txt, Color color)
    {
        displayer.DOComplete();
        displayer.text = "<b>" + txt + "</b>";
        displayer.color = color;
        displayer.DOFade(0f, 4f);
    }
    
    
}