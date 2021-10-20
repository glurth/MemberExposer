using UnityEngine;
using UnityEngine.UI;
using EyE.Sys.Reflection;
using System;

/// <summary>
/// Abstract base class that will be used for all control <-> member exposer data exchanges.
/// A concrete descendant must define the two functions
///     UpdateMemberData(object newData) and UpdateDisplayedData()
/// These two functions will define how the control changes when the contaianingObj's data changes, 
/// and how user input changes the data (even if it doesn't).
///     
/// Usage: 
/// `Setup()` is called when Adapters are added to the scene via the MemberExposer inspector (in the editor). This can also be done manually.
/// `Update()` will invoke the abstract UpdateDisplayedData function.  
/// `UpdateMemberData` should be invoked whenever the user changes the input.  Usually it is added as a 'listener' to an event for a unity UI control- this is something descendant classes should do in an overridden version of `Setup()`.
/// </summary>
abstract public class MemberExposerUIAdpater : MonoBehaviour
{
    public SerializableMemberInfo memberInfo;
    public object contextObject; // the field value in this object will be used

    /// <summary>
    /// this base class version just stores the parameters in local member variables.
    /// A Warning is displayed if the adapter can handle the member provided via the memberInfo param.
    /// </summary>
    /// <param name="contaianingObj">the object that contains member data to be exposed.</param>
    /// <param name="memberInfo"></param>
    public virtual void Setup(object contaianingObj, SerializableMemberInfo memberInfo)
    {
        contextObject = contaianingObj;
        this.memberInfo = memberInfo;
        if (!handlesMemberType(memberInfo) )
            Debug.LogWarning("MemberDataToUIControlAdpater :" + name + " handles a different kind of member behavior, than the member exhibits.");
        UpdateDisplayedData();
    }

    /// <summary>
    /// specifies what kind of data it is capable of exchanging.  It is up to the descendant class to decide this.
    /// This function is used by the MemberExposer inspector, when displaying potential adapter prefabs for addition to the scene.
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public abstract bool handlesMemberType(SerializableMemberInfo memberInfo);

    protected abstract void UpdateMemberData(object newData);
    protected abstract void UpdateDisplayedData();
    protected virtual void Update()
    {
        UpdateDisplayedData();
    }
}
