using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sample class that will be used to demo MemberExposer.  It contains members of various kinds and types.
/// </summary>
[System.Serializable]
public class TestClass
{
    public float aFloat=2.4f;
    public int anInt;
    public string aString;
    public AnotherClass subClassRef = new AnotherClass();

    public byte GetAByte() { return (byte)anInt; }
    public int GetSetFunc(int a) { anInt = a; return anInt; }
    public void Action() { Debug.Log("I am a test class and I'm doing a thing."); }
    public Sprite testSprite;

}
[System.Serializable]
public class AnotherClass
{
    public string someString;
}
