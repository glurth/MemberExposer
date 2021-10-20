
using UnityEngine;
using UnityEditor;
using EyE.EditorUnity.Extensions;
using System;
[CustomPropertyDrawer(typeof(MemberExposer))]
public class MemberExposerPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        SerializedProperty containingTypeProperty =  property.FindPropertyRelative("containingType");
        SerializedProperty containingTypePropertyTypeName = containingTypeProperty.FindPropertyRelative("typeName");
        SerializedProperty newInstanceParentProperty = property.FindPropertyRelative("newInstanceParent");
        SerializedProperty fieldsAndControlsProperty = property.FindPropertyRelative("membersAndControls");// a list


        position.height = EditorGUI.GetPropertyHeight(containingTypeProperty);
        SerializableSystemType seriType = (SerializableSystemType)containingTypeProperty.GetValue();

        Type originalType = seriType.type;// containingTypePropertyTypeName.stringValue;
        EditorGUI.PropertyField(position, containingTypeProperty);
        Type newType = seriType.type; //containingTypePropertyTypeName.stringValue;
        if (originalType!=newType)
        {
            if (EditorUtility.DisplayDialog("Confirm", "Changing the type will invalidate any entries in the fieldsAndControls list and the list will be cleared.  This may lead to orphaned controls.  Confirm action.", "Confirm", "Abort"))
            {
                property.serializedObject.Update();
                fieldsAndControlsProperty.arraySize = 0;
                fieldsAndControlsProperty.serializedObject.ApplyModifiedProperties();
            }
            else
            {
                seriType.type = originalType;
                GUI.FocusControl("list");
            }
            property.serializedObject.Update();
        }

        position.y += position.height;
        position.height = EditorGUI.GetPropertyHeight(fieldsAndControlsProperty,true);
        //this list is being drawn as JUST a foldout- no element count visible, no scroll bars visible, nothing.
        int initialCount = fieldsAndControlsProperty.arraySize;
        GUI.SetNextControlName("list");
        MemberInfoAndControlPropertyDrawer.currentCreationParent = newInstanceParentProperty.objectReferenceValue as Transform;
        EditorGUI.PropertyField(position, fieldsAndControlsProperty,true);
        while (initialCount < fieldsAndControlsProperty.arraySize) 
            //update any new entries in list with the "type" of TypeFieldDisplay object
            // and remove any initially assigned control refs
        {
            SerializedProperty fieldAndControlElement = fieldsAndControlsProperty.GetArrayElementAtIndex(initialCount++);
            SerializedProperty fieldInfo = fieldAndControlElement.FindPropertyRelative("fieldInfo");
            SerializedProperty controlRef = fieldAndControlElement.FindPropertyRelative("instantiatedPrefFab");
            SerializedProperty fieldType = fieldInfo.FindPropertyRelative("typeName");
            SerializedProperty typeFieldsDisplayType = containingTypeProperty.FindPropertyRelative("typeName");
            fieldType.stringValue = typeFieldsDisplayType.stringValue;
            fieldType.serializedObject.ApplyModifiedProperties();
            controlRef.objectReferenceValue = null;
            fieldInfo.serializedObject.ApplyModifiedProperties();
            fieldAndControlElement.serializedObject.ApplyModifiedProperties();
        }
        position.y += EditorGUI.GetPropertyHeight(fieldsAndControlsProperty, true);
        position.height = EditorGUI.GetPropertyHeight(newInstanceParentProperty);
        EditorGUI.PropertyField(position, newInstanceParentProperty);
        if (newInstanceParentProperty.objectReferenceValue == null)
        {
            position.y += position.height;
           // GUI.Button(position,"Create Instance Now");
            Debug.Log("newInstanceParentProperty.objectReferenceValue == null");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty containingTypeProperty = property.FindPropertyRelative("containingType");
        SerializedProperty fieldsAndControlsProperty = property.FindPropertyRelative("membersAndControls");
        SerializedProperty newInstanceParentProperty = property.FindPropertyRelative("newInstanceParent");
        float height = EditorGUI.GetPropertyHeight(containingTypeProperty)
            + EditorGUI.GetPropertyHeight(fieldsAndControlsProperty,true)
            + EditorGUI.GetPropertyHeight(newInstanceParentProperty)
            + EditorGUIUtility.standardVerticalSpacing;
        return height;
    }
}

