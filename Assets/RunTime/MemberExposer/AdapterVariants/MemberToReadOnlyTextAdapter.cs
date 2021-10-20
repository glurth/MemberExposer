using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EyE.Sys.Reflection;
using System;


/// <summary>
/// Concrete variant of MemberExposerUIAdpater.  Connects a string, from a member of an exposed type, to a read only text UI element.
/// </summary>
public class MemberToReadOnlyTextAdapter : MemberExposerUIAdpater
{
    public Text outputText;

    public override bool handlesMemberType(SerializableMemberInfo memberInfo) 
    {
        if (memberInfo.memberInfo.MemberType != System.Reflection.MemberTypes.Method) 
            return true;  //since ALL objects have a ToString() function
        return memberInfo.memberInfo.GetSystemType() != typeof(void); 
    }
    protected override void UpdateMemberData(object newValue)
    {
        return; //read-only, do nothing with this info
    }
    protected override void UpdateDisplayedData()
    {
        if (outputText != null && memberInfo != null && memberInfo.memberInfo != null && contextObject != null)
            outputText.text = memberInfo.memberInfo.GetValue(contextObject).ToString();
    }
}
