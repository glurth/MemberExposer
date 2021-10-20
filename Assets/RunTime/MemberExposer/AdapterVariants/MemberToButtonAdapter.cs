using System.Collections;
using System.Reflection;
using UnityEngine.UI;
using EyE.Sys.Reflection;

public class MemberToButtonAdapter : MemberExposerUIAdpater
{
    public Button buttonRef;

    void Start()
    {
        buttonRef.onClick.AddListener(clickHandler);
    }

    public override bool handlesMemberType(SerializableMemberInfo memberInfo)
    {
        return memberInfo.memberInfo.BehavesAs() == MemberBehavior.action;
    }
    protected override void UpdateMemberData(object newValue)
    {
        return;
    }
    protected override void UpdateDisplayedData()
    {

    }
    void clickHandler()
    {
        ((MethodInfo)(memberInfo.memberInfo)).Invoke(contextObject, null);
    }
}
