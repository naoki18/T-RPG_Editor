#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TypeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    List<string> typeNames = new List<string>();
    public Action<string> onChange;
    public TypeSearchWindow()
    {
        var targetAssemblies = AppDomain.CurrentDomain.GetAssemblies()
           .Where(assembly => assembly.FullName.Contains("Unity") || assembly == Assembly.GetExecutingAssembly());

        foreach (var assembly in targetAssemblies)
        {
            foreach (System.Type t in assembly.GetTypes())
            {
                bool isMonoBehaviour = t.IsSubclassOf(typeof(MonoBehaviour));
                if (!isMonoBehaviour && !t.IsSubclassOf(typeof(Component))) continue;

                string test = t.FullName;
                if (isMonoBehaviour)
                {
                    test = "MonoBehaviour." + test;
                }
                typeNames.Add(test);
            }
        }


    }
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
        searchList.Add(new SearchTreeGroupEntry(new GUIContent("Types"), 0));
        List<string> groups = new();
        foreach (string type in typeNames)
        {
            string[] entryTitle = type.Split('.');
            string groupName = "";
            for (int i = 0; i < entryTitle.Length - 1; i++)
            {
                groupName = entryTitle[i];
                if (!groups.Contains(groupName))
                {
                    searchList.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groups.Add(groupName);
                }
                groupName += ".";
            }
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
            entry.level = entryTitle.Length;
            entry.userData = entryTitle.Last();
            searchList.Add(entry);
        }
        return searchList;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        onChange?.Invoke(SearchTreeEntry.userData.ToString());
        return true;
    }

}
#endif