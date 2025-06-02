using System;
using System.Collections.Generic;
using System.Reflection;
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