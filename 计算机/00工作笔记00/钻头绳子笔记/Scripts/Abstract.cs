using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGrabable
{
    GameObject gameObj { get; }

    Transform defaultPos { get; }


    bool CanGrab { get; }

    void OnGrab(Transform grab);

    void OnDrop(Vector3 dir);
}

public interface IDamageable
{
    GameObject gameObj { get; }

    Transform defaultPos { get; }

    bool IsBroken { get; }

    void OnBreak();

    void StartDamage(Collider collider, float dps);

    void EndDamage();
}


public enum WaypointType
{
    Walk,
    Jump,
    Activate,
    Aim,
    End
}

public interface IActicvatable
{
    float WaitTime { get; }
    void Activate();
}

