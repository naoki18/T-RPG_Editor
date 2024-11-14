using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SystemTypeSerialisation
{
    public List<string> typeNames = new List<string>();
    // Used to save editor choice
    public string selectedType = string.Empty;
    private System.Type _type = typeof(object);
    public System.Type Type
    {
        get => _type; set => _type = value;
    }

    public SystemTypeSerialisation()
    {
        foreach (System.Type t in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!t.IsSubclassOf(typeof(MonoBehaviour))) continue;
            typeNames.Add(t.Name);
        }
        
    }

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SystemTypeSerialisation))]
public class SystemTypeEditor : PropertyDrawer
{
    SerializedProperty serializedTypeList;
    SerializedProperty serializedType;
    int nbMember;
    int selected = 0;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        nbMember = -1;
        serializedType = property.FindPropertyRelative("selectedType");
        serializedTypeList = property.FindPropertyRelative("typeNames");

        string[] _typeList = new string[serializedTypeList.arraySize];
        for (int i = 0; i < serializedTypeList.arraySize; i++)
        {
            _typeList[i] = serializedTypeList.GetArrayElementAtIndex(i).stringValue;
            if (_typeList[i] == serializedType.stringValue) selected = i;
        }
        selected = EditorGUILayout.Popup("Label", selected, _typeList, GUILayout.Width(250));
        serializedType.stringValue = _typeList[selected];


    }

    private void DrawMember(Rect position, SerializedProperty propertyToDraw)
    {
        nbMember++;
        EditorGUI.indentLevel++;
        float posX = position.min.x;
        float posY = position.min.y + EditorGUIUtility.singleLineHeight * nbMember;
        float width = position.size.x;
        float height = EditorGUIUtility.singleLineHeight;

        Rect drawArea = new Rect(posX, posY, width, height);
        EditorGUI.PropertyField(drawArea, propertyToDraw);
        EditorGUI.indentLevel--;
    }
}


#endif
