using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Large,
    Default,
    Consume //Stackable
}

public interface IItem
{
    GameObject GameObj { get; }
    ItemType Type { get; }
    string Name { get; }
    int Count { get; set; }

    GameObject ObjDisplay { get; }

    void SetHeld();
    void SetFree();

	void Action0();
	void Action1();
}
