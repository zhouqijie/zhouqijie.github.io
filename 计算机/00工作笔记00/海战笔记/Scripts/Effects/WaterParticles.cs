using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterParticles : MonoBehaviour
{

    private GameObject ship;

    private GameObject waveEffectBow;
    private GameObject waveEffectFront;
    private GameObject waveEffectMid;

    public Vector2 backFrontPos;
    public float shipWidth;
    public float shipSizeFac;

    private float height;
    private bool isEnable;
    private bool isForward;
    private GameObject waveBow;
    private GameObject waveFront1;
    private GameObject waveFront2;
    private GameObject waveMid;

    ParticleSystem sysB1;
    ParticleSystem sysB2;
    ParticleSystem sysBspeed;
    ParticleSystem sysFront1;
    ParticleSystem sysFront2;
    ParticleSystem sysMid;

    private Color clr;
    private Color clrTransparent;



    //tmp
    private float rdm;// = Random.Range(-1f, 1f);
    private float posBF;// = rdm > 0 ? rdm * backFrontPos.y * 0.8f : -rdm * backFrontPos.x * 0.8f;
    private float posLR;// = Mathf.Sign(Random.value - 0.5f) * (1f - rdm * rdm) * shipWidth * 0.4f;

    private Vector3 defaultPosFront;// = ship.transform.position + (ship.transform.forward * backFrontPos.y);
    private Vector3 defaultPosMid;// = ship.transform.position + (ship.transform.forward * posBF) + (ship.transform.right * posLR);
    private Vector3 instantiatePosFront;// = new Vector3(defaultPosFront.x, height, defaultPosFront.z);
    private Vector3 instantiatePosMid;//= new Vector3(defaultPosMid.x, height, defaultPosMid.z);



    private void Awake()
    {
        waveEffectBow = Resources.Load<GameObject>("Prefabs/Effects/WaterWaveBow");
        waveEffectFront = Resources.Load<GameObject>("Prefabs/Effects/WaterWaveFront");
        waveEffectMid = Resources.Load<GameObject>("Prefabs/Effects/WaterWaveMid");

        clrTransparent = new Color(1f, 1f, 1f, 0f);
    }


    void Start()
    {
        height = 0.5f;
        //isEnable = false;
        ship = this.GetComponentInParent<Rigidbody>().gameObject;
        
        Invoke("EnableParticleSys", 1f);
    }

    void EnableParticleSys()
    {
        Light light = FindObjectsOfType<Light>().FirstOrDefault(l => l.type == LightType.Directional);
        clr = light.intensity * light.color; clr = Color.white;

        waveBow = Instantiate(waveEffectBow, new Vector3(), Quaternion.Euler(90f, 0, 0));
        //...

        waveFront1 = Instantiate(waveEffectFront, new Vector3(), Quaternion.Euler(90f, 0, 0));
        var mainFront1 = waveFront1.GetComponent<ParticleSystem>().main;
        mainFront1.startColor = new ParticleSystem.MinMaxGradient(clr);

        waveFront2 = Instantiate(waveEffectFront, new Vector3(), Quaternion.Euler(-90f, 0, 0));
        var mainFront2 = waveFront2.GetComponent<ParticleSystem>().main;
        mainFront2.startColor = new ParticleSystem.MinMaxGradient(clr);

        waveMid = Instantiate(waveEffectMid, new Vector3(), Quaternion.Euler(0, 0, 0));
        var mainMid = waveMid.GetComponent<ParticleSystem>().main;
        mainMid.startSize = new ParticleSystem.MinMaxCurve(mainMid.startSize.constant * shipSizeFac);
        mainMid.startColor = new ParticleSystem.MinMaxGradient(clr);
        
        sysB1 = waveBow.transform.GetChild(0).GetComponent<ParticleSystem>();
        sysB2 = waveBow.transform.GetChild(1).GetComponent<ParticleSystem>();
        sysBspeed = waveBow.transform.GetChild(2).GetComponent<ParticleSystem>();
        sysFront1 = waveFront1.GetComponent<ParticleSystem>();
        sysFront2 = waveFront2.GetComponent<ParticleSystem>();
        sysMid = waveMid.GetComponent<ParticleSystem>();

        //AI Em
        if (ship.GetComponent<IShipController>() != null && ship.GetComponent<IShipController>() is AIController)
        {
            var emB1 = sysB1.emission;
            emB1.rateOverTime = new ParticleSystem.MinMaxCurve(emB1.rateOverTime.constant * 0.5f);
            var emB2 = sysB2.emission;
            emB2.rateOverTime = new ParticleSystem.MinMaxCurve(emB2.rateOverTime.constant * 0.5f);
            var emBspeed = sysBspeed.emission;
            emBspeed.rateOverTime = new ParticleSystem.MinMaxCurve(emBspeed.rateOverTime.constant * 0.0f);

            var emf1 = sysFront1.emission;
            emf1.rateOverTime = new ParticleSystem.MinMaxCurve(emf1.rateOverTime.constant * 0.4f);
            var emf2 = sysFront2.emission;
            emf2.rateOverTime = new ParticleSystem.MinMaxCurve(emf2.rateOverTime.constant * 0.4f);

            var emMid = sysMid.emission;
            emMid.rateOverTime = new ParticleSystem.MinMaxCurve(emMid.rateOverTime.constant * 0.4f);
        }

        isEnable = true;
    }

    void FixedUpdate()
    {
        if (!this.enabled) return;

        //水面贴合
        rdm = Random.Range(-1f, 1f);
        posBF = rdm > 0 ? rdm * backFrontPos.y * 0.8f : -rdm * backFrontPos.x * 0.8f;
        posLR = Mathf.Sign(Random.value - 0.5f) * (1f - rdm * rdm) * shipWidth * 0.4f;

        defaultPosFront = ship.transform.position + (ship.transform.forward * backFrontPos.y);
        defaultPosMid = ship.transform.position + (ship.transform.forward * posBF) + (ship.transform.right * posLR);
        instantiatePosFront = new Vector3(defaultPosFront.x, height, defaultPosFront.z);
        instantiatePosMid = new Vector3(defaultPosMid.x, height, defaultPosMid.z);

        if (isEnable)
        {
            waveBow.transform.position = instantiatePosFront;
            waveBow.transform.rotation = Quaternion.LookRotation(this.transform.forward, Vector3.up);
            waveFront1.transform.position = instantiatePosFront;
            waveFront1.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), this.transform.right);
            waveFront2.transform.position = instantiatePosFront;
            waveFront2.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), -this.transform.right);
            waveMid.transform.position = instantiatePosMid;
            waveMid.transform.rotation = Quaternion.LookRotation(new Vector3(0, 1, 0), -this.transform.forward);

            // 前进后退
            isForward = false;
            if (Vector3.Dot(ship.GetComponent<Rigidbody>().velocity, ship.transform.forward) >= 0)
            {
                isForward = true;
            }

            //速度绑定
            float speedSqr = ship.GetComponent<Rigidbody>().velocity.sqrMagnitude;
            //
            float colorScale = (speedSqr - 9f) / 900;
            //
            if (speedSqr <= 9f)
            {
                colorScale = 0;
            }
            if (speedSqr > 909f)
            {
                colorScale = 1;
            }

            var mainB1 = sysB1.main;
            var mainB2 = sysB2.main;
            var trailBspeed = sysBspeed.trails;
            mainB1.startColor = isForward ? new Color(clr.r, clr.g, clr.b, colorScale) : clrTransparent;//startColor基于粒子系统的色彩模块
            mainB2.startColor = isForward ? new Color(clr.r, clr.g, clr.b, colorScale) : clrTransparent;//startColor基于粒子系统的色彩模块
            trailBspeed.colorOverTrail = isForward ? new ParticleSystem.MinMaxGradient(new Color(clr.r, clr.g, clr.b, colorScale)) : clrTransparent;

            //float maxLifeTime0 = sysFront1.main.startLifetime.constant;
            //ParticleSystem.Particle[] particles0 = new ParticleSystem.Particle[sysFront1.particleCount];
            //int length0 = sysFront1.GetParticles(particles0);
            //for (int i = 0; i < length0; i++)
            //{
            //    if (particles0[i].remainingLifetime >= (maxLifeTime0 - 0.1f))
            //    {
            //        particles0[i].startColor = new Color(clr.r, clr.g, clr.b, colorScale);//startColor基于粒子系统的色彩模块
            //        particles0[i].velocity = particles0[i].velocity.normalized * sysFront1.main.startSpeed.constant * speedScale * (isForward ? 2.5f : 0.5f);
            //    }
            //}
            //sysFront1.SetParticles(particles0, length0);

            var main1 = sysFront1.main;
            main1.startColor = isForward ? new Color(clr.r, clr.g, clr.b, colorScale) : clrTransparent;//startColor基于粒子系统的色彩模块

            //float maxLifeTime1 = sysFront2.main.startLifetime.constant;
            //ParticleSystem.Particle[] particles1 = new ParticleSystem.Particle[sysFront2.particleCount];
            //int length1 = sysFront2.GetParticles(particles1);
            //for (int i = 0; i < length1; i++)
            //{
            //    if (particles1[i].remainingLifetime >= (maxLifeTime1 - 0.1f))
            //    {
            //        particles1[i].startColor = new Color(clr.r, clr.g, clr.b, colorScale);//startColor基于粒子系统的色彩模块
            //        particles1[i].velocity = particles1[i].velocity.normalized * sysFront2.main.startSpeed.constant * speedScale * (isForward ? 2.5f : 0.5f);
            //    }
            //}
            //sysFront2.SetParticles(particles1, length1);


            var main2 = sysFront2.main;
            main2.startColor = isForward ? new Color(clr.r, clr.g, clr.b, colorScale) : clrTransparent;//startColor基于粒子系统的色彩模块

            //float maxLifeTime2 = sysMid.main.startLifetime.constant;
            //ParticleSystem.Particle[] particles2 = new ParticleSystem.Particle[sysMid.particleCount];
            //int length2 = sysMid.GetParticles(particles2);
            //for (int i = 0; i < length2; i++)
            //{
            //    if (particles2[i].remainingLifetime >= (maxLifeTime2 - 0.1f))
            //    {
            //        particles2[i].startColor = new Color(clr.r, clr.g, clr.b, (0.6f + colorScale * 0.4f));//startColor基于粒子系统的色彩模块 //静止状态也有粒子颜色
            //    }
            //}
            //sysMid.SetParticles(particles2, length2);


            var mainMid = sysMid.main;
            mainMid.startColor = new Color(clr.r, clr.g, clr.b, (0.6f + colorScale * 0.4f));//startColor基于粒子系统的色彩模块 //静止状态也有粒子颜色

        }

    }


    //清除
    public void Clean()
    {
        var emB1 = sysB1.emission; emB1.enabled = false;
        var emB2 = sysB2.emission; emB2.enabled = false;
        var emBs = sysBspeed.emission; emBs.enabled = false;
        var emF1 = sysFront1.emission; emF1.enabled = false;
        var emF2 = sysFront2.emission; emF2.enabled = false;
        var emM = sysMid.emission; emM.enabled = false;
        
        this.enabled = false;

        StartCoroutine(DestroyParticleObjs());
    }

    private IEnumerator DestroyParticleObjs()
    {
        yield return new WaitForSeconds(5f);

        Destroy(waveBow);
        Destroy(waveFront1);
        Destroy(waveFront2);
        Destroy(waveMid);
    }

    public void CleanImmidiate()
    {
        this.enabled = false;

        Destroy(waveBow);
        Destroy(waveFront1);
        Destroy(waveFront2);
        Destroy(waveMid);
    }
}
