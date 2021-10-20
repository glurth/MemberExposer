using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using EyE.Sys.Reflection;
[System.Serializable]
public class AnIntClass
{
    public int anotherInt=12;
}






[System.Serializable]
public class SerializableMemberInfo : ISerializationCallbackReceiver
{
    private Type memberOf;
    public Type MemberOf { get => memberOf; }

    [NonSerialized]
    public MemberInfo memberInfo;
   
    [SerializeField]
    string memberName;
   
    [SerializeField]
    string typeName;

    //[SerializeField]
    public MemberBehavior behavesAs
    {
        get
        {
            return memberInfo.BehavesAs();
            /*
                -functions that return their one and only param type, will be considered read/write accessors and equivalent to a "field"
                -functions that return a not-void type and take no params will be considered read-only accessors
                -functions that take no param, and return void will be considered "actions"
                -functions that take a param, and return void will be unusable here.
                -functions that take a param, and return a different type that the param, will be unusable here.
             */
        }
    }

    public SerializableMemberInfo(Type memberOf, string memberName)
    {
        this.memberOf = memberOf;
        this.memberInfo = MemberInfoExtension.FindMemberByName(memberOf, memberName);
    }

    /// <summary>
    /// If no MemberInfo param is provided, the memberInfo will be null, but the type any potential members belog to WILL be set.
    /// This is useful for use with initializing a list in unity inspectors.
    /// </summary>
    /// <param name="memberOf"></param>
    /// <param name="memberInfo"></param>
    public SerializableMemberInfo(Type memberOf, MemberInfo memberInfo=null)
    {
        this.memberOf = memberOf;
        this.memberInfo = memberInfo;
    }

    public void OnBeforeSerialize()
    {
        if (memberInfo != null)
        {
            typeName = memberInfo.DeclaringType.AssemblyQualifiedName;
            memberName = memberInfo.Name;
        }
        else
        {
            typeName = string.Empty;
            memberName = string.Empty;
        }
        //Debug.Log("Serialized member info with name " + memberName);
    }
    public void OnAfterDeserialize()
    {
       // Debug.Log("DeSerializing member info with name " + memberName);
        Type containingType = Type.GetType(typeName);
        if (containingType == null)
        {
            //Debug.Log("SerializableTypeInfo Error: Unable to Deserialize type '" + typeName + "' that should hold field `" + memberName+"'");
            return;
        }
        memberOf = containingType;
        //memberInfo = 
        MemberInfo[] membersWithName = containingType.GetMember(memberName);
        if (membersWithName == null || membersWithName.Length==0)
        {
            //Debug.Log("SerializableTypeInfo Error: Unable to Deserialize member " + memberName + " in type " + typeName);
            return;
        }
        memberInfo = membersWithName[0];
    }
}
