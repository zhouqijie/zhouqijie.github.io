using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEmpty : MonoBehaviour, Ability
{
    public int Index { get { return 2; } }

    public string Name { get { return "空"; } }

    private IShipController controller;
    
    [HideInInspector] public float CD { get { return 1f; } }



    public MonoBehaviour _This { get { return this as MonoBehaviour; } }

    private bool canUse = false;
    public bool CanUse { get { return this.canUse; } }



    // Start is called before the first frame update
    void Start()
    {
        this.controller = this.GetComponent<IShipController>();
    }

    void Update()
    {
        canUse = false;
    }
    
}
