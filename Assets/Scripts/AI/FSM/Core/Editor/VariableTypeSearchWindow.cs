using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Custom.FSM.Editor
{
    public class VariableTypeSearchWindow : EditorWindow, ISearchWindowProvider
    {
        private readonly List<Type> serializableClasses = new();
        private string searchQuery = "";
        private Vector2 scrollPosition;



        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void OnGUI()
        {
            searchQuery = EditorGUILayout.TextField(searchQuery);
        }



        #region ISearchWindowProvider
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Variable Type"), 0),
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            return true;
        }
        #endregion

        #region Utility
        private void FindSerializableClasses()
        {
            serializableClasses.Clear();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes()
                    .Where(
                        t => t.IsClass &&
                        !t.IsAbstract &&
                        t.IsSubclassOf(typeof(UnityEngine.Object)) &&
                        !t.Name.StartsWith("<") &&
                        !t.IsNestedPrivate
                    );

                serializableClasses.AddRange(types);
            }
        }
        #endregion
    }
}
