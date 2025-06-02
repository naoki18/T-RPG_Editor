#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
}
#endif