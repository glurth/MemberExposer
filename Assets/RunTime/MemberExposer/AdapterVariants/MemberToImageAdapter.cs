using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EyE.Sys.Reflection;
public class MemberToImageAdapter : MemberExposerUIAdpater
{
    public UnityEngine.UI.Image displayImage;

    public override bool handlesMemberType(SerializableMemberInfo memberInfo)
    {
        return memberInfo.memberInfo.GetSystemType() == typeof(Sprite);
    }

    protected override void UpdateMemberData(object newData)
    {
        return;
    }
    protected override void UpdateDisplayedData()
    {
        if (displayImage != null && memberInfo != null && memberInfo.memberInfo != null && contextObject != null)
            displayImage.sprite = (Sprite)memberInfo.memberInfo.GetValue(contextObject);
    }
}
