using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using EyE.EditorUnity.Extensions;

[CustomPropertyDrawer(typeof(MemberInfoAndControl))]
public class MemberInfoAndControlPropertyDrawer : PropertyDrawer
{
    public static Transform currentCreationParent;

    public int selectedAdapterIndex = -1;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //   MethodInfo defaultDraw = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        //   object result=defaultDraw.Invoke(null, new object[3] { position, property, label });

        //base.OnGUI(position, property, label);
        SerializedProperty fieldInfoProp = property.FindPropertyRelative("fieldInfo");
       // SerializedProperty controlDisplayTypeProp = property.FindPropertyRelative("controlDisplayType");
        SerializedProperty instantiatedPrefFabProp = property.FindPropertyRelative("instantiatedPrefFab");

        position.height = EditorGUI.GetPropertyHeight(fieldInfoProp, label);
        EditorGUI.PropertyField(position, fieldInfoProp);
        position.y += position.height;

        position.height = EditorGUI.GetPropertyHeight(instantiatedPrefFabProp, label);
        Object origRef=instantiatedPrefFabProp.objectReferenceValue;
        EditorGUI.PropertyField(position, instantiatedPrefFabProp);
        if (origRef!=null && instantiatedPrefFabProp.objectReferenceValue != origRef)
        {
            //message check- do you want to delete the original reference from the scene?
            string msg = "The object being referenced by this FieldAndControl entry has changed.  The old reference may now be an orphaned MemberDataToUIControlAdpater object. Do you want to remove/delete the old reference object from the scene?";
            bool delete = EditorUtility.DisplayDialog("Potential Orphan", msg, "Delete", "Leave it Alone");
            if (delete)
                Object.DestroyImmediate(((MemberExposerUIAdpater)origRef).gameObject);
        }
        position.y += position.height;
       
        // if no control is assigned yet, but a member IS- then display control creation options
        if (instantiatedPrefFabProp.objectReferenceValue == null && fieldInfoProp.GetValue() != null)
        {
            position.height = EditorGUIUtility.singleLineHeight ;// EditorGUI.GetPropertyHeight(controlDisplayTypeProp, label);
            Rect box = position;
            box.height+= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing*2;
            box.xMin -= 2;
            box.xMax += 2;
            EditorGUI.DrawRect(box, Color.gray);
            position.y += EditorGUIUtility.standardVerticalSpacing;

            //determine which control prefabs in the project are usable with the selected member
            SerializableMemberInfo selectedMember = (SerializableMemberInfo)fieldInfoProp.GetValue();
            string[] allHandlingAdaptersNames;
            List<MemberExposerUIAdpater> allHandlingAdapters;
            if (selectedMember != null && selectedMember.memberInfo!=null)
            {
                allHandlingAdapters = MemberExposerUIAdpatersUtilties.GetAllThatHandleMember(selectedMember);
                allHandlingAdaptersNames = new string[allHandlingAdapters.Count];
                int i = 0;
                foreach (MemberExposerUIAdpater adapter in allHandlingAdapters)
                {
                    allHandlingAdaptersNames[i++] = adapter.name;
                }
            }
            else
            {
                allHandlingAdaptersNames = new string[0];
                allHandlingAdapters = new List<MemberExposerUIAdpater>();
            }
            //show viable controls for selection
            selectedAdapterIndex = EditorGUI.Popup(position, selectedAdapterIndex, allHandlingAdaptersNames);
            position.y += position.height;
            
            position.height = EditorGUIUtility.singleLineHeight;
            if (selectedAdapterIndex == -1)
                GUI.enabled = false;
            bool doCreateNow = GUI.Button(position, "Create Instance");
            GUI.enabled = true;
            if (doCreateNow)// create the selcted prefab in the scene, and record reference to it
            {
                MemberExposerUIAdpater preFabToInstantiate = allHandlingAdapters[selectedAdapterIndex];
                string memberName= fieldInfoProp.FindPropertyRelative("memberName").stringValue;
                string typeName= System.Type.GetType(fieldInfoProp.FindPropertyRelative("typeName").stringValue).Name;
                MemberExposerUIAdpater instantiatedPrefFab =
                    (MemberExposerUIAdpater)PrefabUtility.InstantiatePrefab(preFabToInstantiate);// ControlPreFabsByType.Instance.preFabs[controlDisplayTypeProp.enumValueIndex]);
                instantiatedPrefFab.transform.SetParent(currentCreationParent);
                string preFabNameToUse = instantiatedPrefFab.name.Replace("default","");
                preFabNameToUse = preFabNameToUse.Replace("Default", "");
                preFabNameToUse = preFabNameToUse.Replace("Member", "");
                preFabNameToUse = preFabNameToUse.Replace("member", "");
                string goodName = typeName + "." + memberName + " Control-" + preFabNameToUse;
                instantiatedPrefFab.name = goodName;// "ControlFor: " + memberName + instantiatedPrefFab.name;
                instantiatedPrefFab.Setup(null, selectedMember);
                instantiatedPrefFabProp.objectReferenceValue = instantiatedPrefFab;
            }
        }

    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty fieldInfoProp = property.FindPropertyRelative("fieldInfo");
       // SerializedProperty controlDisplayTypeProp = property.FindPropertyRelative("controlDisplayType");
        SerializedProperty instantiatedPrefFabProp = property.FindPropertyRelative("instantiatedPrefFab");
        float height = EditorGUI.GetPropertyHeight(fieldInfoProp, label)
          // + EditorGUIUtility.singleLineHeight// + EditorGUI.GetPropertyHeight(controlDisplayTypeProp, label)
            + EditorGUI.GetPropertyHeight(instantiatedPrefFabProp, label);
        if (instantiatedPrefFabProp.objectReferenceValue == null)
            height+= EditorGUIUtility.singleLineHeight*2 + EditorGUIUtility.standardVerticalSpacing * 2;
        //return base.GetPropertyHeight(property,label) + EditorGUIUtility.singleLineHeight;
        return height; 
    }
}
