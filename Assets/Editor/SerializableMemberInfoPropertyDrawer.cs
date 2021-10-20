using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using EyE.Sys.Reflection;
using EyE.EditorUnity.Extensions;
[CustomPropertyDrawer(typeof(SerializableMemberInfo))]
public class SerializableMemberInfoPropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializableMemberInfo propertyData = (SerializableMemberInfo)property.GetValue();// GetValue(property);
        if (propertyData == null)
        {
            EditorGUI.LabelField(position, "Reference is null");
            return;
        }
        string[] popupListedNames;
        
        if (propertyData.MemberOf == null)
        {
            EditorGUI.LabelField(position, "Error: Type that the member is in, is not set.");
            return;
        }
        List<MemberInfo> members = propertyData.MemberOf.GetAllPublicFieldlikeReadonlyAndActionMembers();
        popupListedNames = new string[members.Count];
        int i = 0;
        int selection=-1;
        foreach (MemberInfo mi in members)
        {
            if (propertyData.memberInfo == mi)
                selection = i;
            popupListedNames[i++] = mi.Name;
        }
        
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        
        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;


        Rect namesRect = position;// new Rect(position.x, position.y, 30, position.height);
       
        
        selection = EditorGUI.Popup(namesRect, selection, popupListedNames);
        // SerializedProperty nameprop =  property.FindPropertyRelative("memberName");
        // nameprop.stringValue = popupListedNames[selection];
        if (selection >= 0)
        {
            Undo.RecordObject(property.serializedObject.targetObject, "change to member selection");
            propertyData.memberInfo = members[selection];
        }
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

      // property.serializedObject.ApplyModifiedProperties();
       
       // property.serializedObject.Update();

        EditorGUI.EndProperty();

        
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return EditorGUIUtility.singleLineHeight; }
}
