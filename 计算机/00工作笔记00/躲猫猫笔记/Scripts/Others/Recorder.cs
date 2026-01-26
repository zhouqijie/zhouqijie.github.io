using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class Recorder : MonoBehaviour
{
    private static Recorder inst;




    private void Awake()
    {
        inst = this;

        var recorder = StarkSDKSpace.StarkSDK.API.GetStarkGameRecorder();
        recorder.SetEnabled(true);

    }


    void Update()
    {

    }



    public static void StartRecord(UnityAction successCallback, UnityAction<int, string> failCallback, int maxTimeSec)
    {
        inst.StartCoroutine(inst.CoStartRecord(successCallback, failCallback, maxTimeSec));
    }

    public static void StopRecord(UnityAction<string> successCallback, UnityAction<int, string> failCallback)
    {
        var recorder = StarkSDKSpace.StarkSDK.API.GetStarkGameRecorder();
        recorder.SetEnabled(false);
        recorder.StopRecord((path) => { successCallback(path); }, (code, msg) => { failCallback(code, msg); });
    }

    public static void ShareVideo(UnityAction successCallback, UnityAction<string> failCallback, UnityAction cancelCallBack)
    {
        inst.StartCoroutine(inst.CoShareVideo(successCallback, failCallback, cancelCallBack));
    }



    private IEnumerator CoStartRecord(UnityAction successCallback, UnityAction<int, string> failCallback, int maxTimeSec)
    {
        yield return new WaitForSeconds(1.5f);

        var recorder = StarkSDKSpace.StarkSDK.API.GetStarkGameRecorder();
        recorder.SetEnabled(true);
        recorder.StartRecord(true, maxTimeSec, () => { successCallback(); }, (code, msg) => { failCallback(code, msg); });
    }

    private IEnumerator CoShareVideo(UnityAction successCallback, UnityAction<string> failCallback, UnityAction cancelCallBack)
    {
        yield return new WaitForSeconds(1.5f);

        string title = "像素战场冲突战争";
        List<string> list = new List<string>();
        list.Add("像素战场冲突战争 #超刺激的吃鸡躲猫猫游戏，快来上钻石");

        var recorder = StarkSDKSpace.StarkSDK.API.GetStarkGameRecorder();
        recorder.ShareVideoWithTitleTopics((arg) => { successCallback(); }, (msg) => { failCallback(msg); }, () => { cancelCallBack(); }, title, list);
    }
}
