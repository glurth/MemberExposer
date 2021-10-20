using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class PopupTest : PopupWindowContent
{

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public override void OnGUI(Rect pos)
    {
        EditorGUI.LabelField(pos, "test");
    }



    public override void OnOpen()
    {
        Debug.Log("Popup opened: " + this);
    }

    public override void OnClose()
    {
        Debug.Log("Popup closed: " + this);
    }
}

//[CustomPropertyDrawer(typeof(SerializableSystemType))]
public class TestPropertyDrawer : PropertyDrawer
{
    PopupTest editorWindowContents;
    string text="test";
    bool isOpen = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (editorWindowContents == null)
            editorWindowContents = new PopupTest();

        //if(GUI.Button(position, "test"))
        // {
        GUI.SetNextControlName("FilterText");
        text = EditorGUI.TextField(position, new GUIContent("Filter"), text);
        if (GUI.GetNameOfFocusedControl() == "FilterText")
        {
            if (!isOpen)
            {
                
                Debug.Log("not being shown now and field has focus");
                isOpen = true;
                PopupWindow.Show(position, editorWindowContents);
                Debug.Log("POST show log");
            }
            GUI.FocusControl("FilterText");
        }
        else
        {
            if (isOpen)
            {
                PopupWindow.Show(position, editorWindowContents);
                Debug.Log("IS being shown now and field does not have focus");
                isOpen = false;
            }
        }

    }
}
