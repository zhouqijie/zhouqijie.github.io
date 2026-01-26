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

    public static void PlaySound3D(string name, Vector3 pos, float volumMultipiler = 1f, float maxDistance = 10000f)
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
