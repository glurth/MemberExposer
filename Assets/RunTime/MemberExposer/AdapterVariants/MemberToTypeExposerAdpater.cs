using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Sys.Reflection;
public class MemberToTypeExposerAdpater : MemberExposerUIAdpater
{
    public MemberExposer subClassExposer;

    public override void Setup(object contaianingObj, SerializableMemberInfo memberInfo)
    {
        subClassExposer = new MemberExposer(memberInfo.memberInfo.GetSystemType());
        subClassExposer.newInstanceParent = this.transform;
        base.Setup(contaianingObj, memberInfo);
    }
    public override bool handlesMemberType(SerializableMemberInfo memberInfo)
    {
        return !(memberInfo.memberInfo.GetSystemType().IsValueType);
    }
    protected override void UpdateMemberData(object newValue)
    {
        memberInfo.memberInfo.SetValue(contextObject, newValue);
    }

    protected override void UpdateDisplayedData()
    {
        subClassExposer.SetSceneFieldControlsActiveAndLinkDataToObject(contextObject);
    }
}
