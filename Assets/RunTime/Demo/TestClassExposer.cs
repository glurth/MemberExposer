using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Note: this implementation is not perfect- the typeExposer member will be shown, in the editor inspector, with changeable TYPE. 
/// But, changing the type will prevent it from working with this example.
/// </summary>
public class TestClassExposer : MonoBehaviour
{
    public MemberExposer typeExposer = new MemberExposer(typeof(TestClass));
    public TestClass containingObject= new TestClass();
    private void Reset()
    {
        typeExposer = new MemberExposer(typeof(TestClass));
        typeExposer.membersAndControls.Add(new MemberInfoAndControl(typeof(TestClass)));
        typeExposer.newInstanceParent = transform;
        containingObject = new TestClass();
    }
    private void OnEnable()
    {
        typeExposer.SetSceneFieldControlsActiveAndLinkDataToObject(containingObject);
    }

}
