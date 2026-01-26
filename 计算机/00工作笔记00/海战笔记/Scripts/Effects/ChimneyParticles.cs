using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChimneyParticles : MonoBehaviour {
    
	private IShipController controller;


    private void Awake()
    {
        //this.gameObject.SetActive(false);
    }

    void Start(){
        controller = this.GetComponentInParent<IShipController>();

        if(controller is AIController)
        {
            var em = this.GetComponent<ParticleSystem>().emission;
            em.rateOverTime = new ParticleSystem.MinMaxCurve(em.rateOverTime.constant * 0.4f);
        }
	}	
	void Update () {
		float throttle = Mathf.Abs(controller.Throttle);
		var sys = this.GetComponent<ParticleSystem>();
        var main = sys.main;
        main.startColor = new Color(127, 127, 127, (1f * throttle));//startColor基于粒子系统的色彩模块
    }
}
