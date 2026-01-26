using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using StarkSDKSpace;

public class Vibrate
{

    public static void DoVibrate(int mSec = 1000)
    {
        StarkSDK.API.Vibrate(new long[2] {  0, mSec });
    }

    public static void DoVibrateArr(long[] arr)
    {
        StarkSDK.API.Vibrate(arr);
    }
}
