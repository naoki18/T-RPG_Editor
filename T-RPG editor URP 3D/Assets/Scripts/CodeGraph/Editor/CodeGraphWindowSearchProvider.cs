using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CodeGraphWindowSearchProvider : ScriptableObject, ISearchWindowProvider
{
    public struct SearchContextElement
    {
        public object target { get; set; }
        public string title { get; set; }
        public SearchContextElement(object target, string title)
        {
            this.target = target;
            this.title = title;
        }
    }
    public CodeGraphView view;
    public VisualElement target;
    public Port from;

    public static List<SearchContextElement> elements;
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
        tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));
        elements = new List<SearchContextElement>();

        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.CustomAttributes.ToList() != null)
                {
                    var attribute = type.GetCustomAttribute(typeof(NodeInfoAttribute));
                    if (attribute != null)
                    {
                        NodeInfoAttribute att = (NodeInfoAttribute)attribute;
                        var node = Activator.CreateInstance(type);
                        if (string.IsNullOrEmpty(att.menuItem)) continue;

                        elements.Add(new SearchContextElement(node, att.menuItem));
                    }
                }
            }
        }

        if(from != null)
        {
            MethodInfo[] methods = from.portType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (MethodInfo method in methods.Where(m => !m.IsSpecialName).ToArray())
            {
                var node = new GenericNode(method);
                elements.Add(new SearchContextElement(node, $"Component Methods/{method.Name}"));
            }

            EventInfo[] events = from.portType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            // Event
            foreach (EventInfo @event in events)
            {
                var node = new EventNode(@event);
                elements.Add(new SearchContextElement(node, $"Events/{@event.Name}"));
            }
        }
        

        elements.Sort((entry1, entry2) =>
        {
            string[] splitFirst = entry1.title.Split('/');
            string[] splitSecond = entry2.title.Split('/');

            for (int i = 0; i < splitFirst.Length; i++)
            {
                if (i >= splitSecond.Length)
                {
                    return 1;
                }
                int value = splitFirst[i].CompareTo(splitSecond[i]);
                if (value != 0)
                {
                    if (splitFirst.Length != splitSecond.Length && (i == splitFirst.Length - 1 || i == splitSecond.Length - 1))
                        return splitFirst.Length < splitSecond.Length ? 1 : -1;
                    return value;
                }
            }
            return 0;
        });

        List<string> groups = new List<string>();

        foreach (SearchContextElement element in elements)
        {
            string[] entryTitle = element.title.Split('/');
            string groupName = "";

            for (int i = 0; i < entryTitle.Length - 1; i++)
            {
                groupName += entryTitle[i];
                if (!groups.Contains(groupName))
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                    groups.Add(groupName);
                }
                groupName += "/";
            }
            SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
            entry.level = entryTitle.Length;
            entry.userData = new SearchContextElement(element.target, element.title);
            tree.Add(entry);
        }
        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var windowMousePos = view.ChangeCoordinatesTo(view, context.screenMousePosition - view.window.position.position);
        var graphMousePos = view.contentViewContainer.WorldToLocal(windowMousePos);

        SearchContextElement element = (SearchContextElement)SearchTreeEntry.userData;
        CodeGraphNode node;

        if(element.target.GetType() == typeof(GenericNode))
        {
            string methodName = element.title.Split('/').Last();
            MethodInfo method = from.portType.GetMethod(methodName);
            node = new GenericNode(method);
        }
        else
        {
            node = (CodeGraphNode)element.target;
        }
            node.SetPosition(new Rect(graphMousePos, new Vector2()));
        view.Add(node);
        if (from != null)
        {
            foreach(var input in view.GetNode(node.id).Ports.Where(x => x.direction == Direction.Input))
            {
                if(input.portType == from.portType)
                {
                    view.CreateEdgeFromScratch(from.ConnectTo(input));
                    break;
                }
            }
        }
        from = null;
        return true;
    }
}
