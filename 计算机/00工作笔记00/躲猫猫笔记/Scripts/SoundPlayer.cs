using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    /// <summary>
    /// 全局音量
    /// </summary>
    private static float volume = 1f;
    public static float Volume { get { return volume; } }




    [HideInInspector] public static string[] collectionConcrate = new string[] { "concrete0", "concrete1", "concrete2", "concrete3" };
    [HideInInspector] public static string[] collectionManHit = new string[] { "soldier_hit01", "soldier_hit02" };
    [HideInInspector] public static string[] collectionCoin = new string[] { "coin1", "coin2", "coin3" };

    //soldier_hit01


    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="str"></param>
    public static void PlaySound2D(string str)//
    {
        try
        {
            GameObject.FindGameObjectWithTag("BGM").GetComponent<AudioSource>().PlayOneShot(Resources.Load<AudioClip>("Sounds/" + str));
        }
        catch
        {

        }
    }
    
    public static void PlaySound3D(string name, Vector3 pos, float volumMultipiler = 1f, float maxDistance = 50f)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/AudioSource3D"));
        AudioSource asc = obj.GetComponent<AudioSource>();

        obj.transform.position = pos;

        asc.clip = Resources.Load<AudioClip>("Sounds/" + name);
        asc.maxDistance = maxDistance;

        if (!(asc.clip != null)) return;

        asc.volume = volume * volumMultipiler;
        asc.Play();
        GameObject.Destroy(obj, asc.clip.length);
    }

    public static void ApplyVolume()
    {
        foreach (var auS in FindObjectsOfType<AudioSource>())
        {
            auS.volume = volume;
        }
    }

    /// <summary>
    /// 改变声音
    /// </summary>
    /// <param name="newVolume"></param>
    public static void ChangeVolume(float newVolume)
    {
        volume = newVolume;
        foreach (var auS in FindObjectsOfType<AudioSource>())
        {
            auS.volume = newVolume;
        }
    }


    public static float GetVolume()
    {
        return volume;
    }
}
