using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Given a specific system type, this control will store a list of members of that type
/// Each member is linked with scene objects to handle their i/o via the MemberInfoAndControl class.
/// The inspector for this object, in the Editor, allow the user to add members to be exposed.
/// Once a member is exposed, the user can select a control to display that member's data.
/// Note: while any MemberToUIAdapter can be dragged onto this reference, NOT all adapters will be compatible.
/// Most controls can only work with a particular Type of data- only compatible ones will be displayed in the drop-down.
/// The inspector also allow for exposed members to be removed.  This will NOT remove their controls from the scene- they will be "orphaned".
/// </summary>
[System.Serializable]
public class MemberExposer
{
    [SerializeField]
    SerializableSystemType containingType;
    public Type ContainingType => containingType.type;
    public List<MemberInfoAndControl> membersAndControls;
    public Transform newInstanceParent;
    public MemberExposer(Type containingType, List<MemberInfoAndControl> fieldInfos = null)
    {
        if (fieldInfos != null)
            this.membersAndControls = fieldInfos;
        else
            membersAndControls = new List<MemberInfoAndControl>();

        this.containingType = new SerializableSystemType(containingType);
    }

    /// <summary>
    /// This is meant to be called by the state machine, when the object's controls should be displayed.
    /// it will activate them all, and ensure data-exchange with the specified object.  
    /// it is NOT responsible for the placement of the controls
    /// </summary>
    /// <param name="objectContainingFields">the object whose data will be exchanged in the controls.</param>
    public void SetSceneFieldControlsActiveAndLinkDataToObject(object objectContainingFields)
    {
        foreach (MemberInfoAndControl member in membersAndControls)
        {
        //    if (member.instantiatedPrefFab == null)
        //        member.instantiatedPrefFab = MemberDataToUIControlAdpater.Instantiate<MemberDataToUIControlAdpater>(ControlPreFabsByType.Instance.preFabs[(int)member.controlDisplayType], newInstanceParent);
            member.instantiatedPrefFab.gameObject.SetActive(true);
            member.instantiatedPrefFab.Setup(objectContainingFields,member.fieldInfo);
        }
    }
}
