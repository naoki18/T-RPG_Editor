using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SystemTypeSerialisation
{
    List<string> list = new List<string>();
    System.Type _type = typeof(object);
    public int _test;
    public System.Type Type
    {
        get => _type; set => _type = value;
    }

    public SystemTypeSerialisation()
    {
        foreach(System.Type t in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (!t.IsSubclassOf(typeof(MonoBehaviour))) continue;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SystemTypeSerialisation))]
public class SystemTypeEditor : Editor
{
    SerializedProperty test;
    private void OnEnable()
    {
        test = serializedObject.FindProperty("_test");
    }
    public override void OnInspectorGUI()
    {
        //EditorGUILayout.PropertyField(test);
        EditorGUI.BeginChangeCheck();
    }
}
#endif
