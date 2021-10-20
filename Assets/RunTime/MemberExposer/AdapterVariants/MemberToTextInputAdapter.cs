using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EyE.Sys.Reflection;
using System;

public interface IAmParsable
{
    object Parse(string str);
}
/// <summary>
/// Concrete variant of MemberExposerUIAdpater.  Connects a member of an exposed type, to a input field UI element.
/// In order for this adapter to work, the member must be writable AND either implement IAmParsable, or be a numerical base type, or a string.
/// </summary>
public class MemberToTextInputAdapter : MemberExposerUIAdpater
{
    public InputField ioText;
    
    void Start()
    {
        if (ioText != null && memberInfo != null && memberInfo.memberInfo != null && contextObject != null)
        {
            ioText.text = memberInfo.memberInfo.GetValue(contextObject).ToString();
            ioText.onValueChanged.AddListener(UpdateMemberData);
        }
    }
    protected override void UpdateMemberData(object newValue)
    {
        UpdateMemberData((string)newValue);
    }

    public override bool handlesMemberType(SerializableMemberInfo memberInfo)
    {
        bool handlesIt = memberInfo.memberInfo.BehavesAs() == MemberBehavior.fieldLike;
        handlesIt &= memberInfo.memberInfo.GetSystemType().IsParsable();
        return handlesIt;
    }

    protected override void UpdateDisplayedData()
    {
        if (ioText != null && memberInfo != null && memberInfo.memberInfo != null && contextObject != null)
            ioText.text = memberInfo.memberInfo.GetValue(contextObject).ToString();
    }

    void UpdateMemberData(string newValue)
    {
        try
        {
            Type memberType = memberInfo.memberInfo.GetSystemType();
            if (memberType == typeof(int))
                memberInfo.memberInfo.SetValue(contextObject, int.Parse(newValue));
            else if (memberType == typeof(float))
                memberInfo.memberInfo.SetValue(contextObject, float.Parse(newValue));
            else if (memberType == typeof(long))
                memberInfo.memberInfo.SetValue(contextObject, long.Parse(newValue));
            else if (memberType == typeof(double))
                memberInfo.memberInfo.SetValue(contextObject, double.Parse(newValue));
            else if (memberType == typeof(string))
                memberInfo.memberInfo.SetValue(contextObject, newValue);
            else if (typeof(IAmParsable).IsAssignableFrom(memberType))
            {
                object parseResult = ((IAmParsable)memberInfo.memberInfo.GetValue(contextObject)).Parse(newValue);
                if(parseResult!=null)
                    memberInfo.memberInfo.SetValue(contextObject, parseResult);
            }
        }
        catch 
        {
            UpdateDisplayedData();
        }
    }
}

