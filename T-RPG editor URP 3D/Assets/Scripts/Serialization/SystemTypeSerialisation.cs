using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class SystemTypeSerialisation
{
    static public Action AnyTypeChanged;
    public List<string> typeNames = new List<string>();
    // Used to save editor choice
    public string selectedType = string.Empty;
    
    public System.Type Type
    {
        get
        {
            if (selectedType == string.Empty) return null;
            return System.Reflection.Assembly.GetExecutingAssembly().GetType(selectedType);
        }
        private set { }
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
    SerializedProperty serializedType;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        serializedType = property.FindPropertyRelative("selectedType");
        if (GUILayout.Button(serializedType.stringValue, EditorStyles.popup))
        {
            TypeSearchWindow win = ScriptableObject.CreateInstance<TypeSearchWindow>();
            win.onChange += (x) =>
            {
                serializedType.stringValue = x;
                Debug.Log(serializedType.serializedObject.GetType());
                serializedType.serializedObject.ApplyModifiedProperties();
                SystemTypeSerialisation.AnyTypeChanged?.Invoke();
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
