using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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

    bool isInit = false;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        nbMember = -1;
        serializedType = property.FindPropertyRelative("selectedType");
        serializedTypeList = property.FindPropertyRelative("typeNames");

        if (GUILayout.Button(serializedType.stringValue, EditorStyles.popup))
        {
            Debug.Log(serializedType.stringValue);
            TypeSearchWindow win = ScriptableObject.CreateInstance<TypeSearchWindow>();
            win.test += (x) =>
            {
                serializedType.stringValue = x;
                serializedType.serializedObject.ApplyModifiedProperties();
            };

            SearchWindow.Open(new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)), win);
        }
        
    }

    //private void DrawMember(Rect position, SerializedProperty propertyToDraw)
    //{
    //    nbMember++;
    //    EditorGUI.indentLevel++;
    //    float posX = position.min.x;
    //    float posY = position.min.y + EditorGUIUtility.singleLineHeight * nbMember;
    //    float width = position.size.x;
    //    float height = EditorGUIUtility.singleLineHeight;

    //    Rect drawArea = new Rect(posX, posY, width, height);
    //    EditorGUI.PropertyField(drawArea, propertyToDraw);
    //    EditorGUI.indentLevel--;
    //}
}


#endif
