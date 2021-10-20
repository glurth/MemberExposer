using System.Collections.Generic;
using System.Reflection;
using System;
using EyE.Sys.Reflection;
/// <summary>
/// Stores a MemberInfo, and a scene control that will be used for it's i/o
/// if the scene control does not exist:  the ControlTypeDisplay will define what kind of object is added to the scene when the user click to do so, in the editor (PropertyDrawer)
/// </summary>
[System.Serializable]
public class MemberInfoAndControl
{
    public SerializableMemberInfo fieldInfo;
    public MemberExposerUIAdpater instantiatedPrefFab;

    public MemberInfoAndControl(Type memberOf)
    {
        this.fieldInfo = new SerializableMemberInfo(memberOf);
    }
    public MemberInfoAndControl(MemberInfo memberInfo)
    {
        this.fieldInfo = new SerializableMemberInfo(memberInfo.DeclaringType, memberInfo);
    }
    

}