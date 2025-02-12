using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Custom.FSM.Editor
{
    public class StateMachineEditorWindow : EditorWindow
    {
        private StateMachineGraphView graphView;

        private string selectedFilePath;
        private StateMachine stateMachine = new();

        private bool fileUpdated = false;



        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;

            SetupGraphView();
            SetupToolbar();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;

            RemoveGraphView();
        }

        private void OnDestroy()
        {
            if (!fileUpdated) return;

            if (string.IsNullOrEmpty(selectedFilePath))
                SaveNew();
            else
                Save();
        }



        [MenuItem("Tools/Custom/AI/Finite State Machine")]
        public static void OpenEditorWindow()
        {
            OpenEditorWindow("");
        }

        [MenuItem("Tools/Custom/AI/Close Finite State Machine")]
        public static void CloseEditorWindow()
        {
            var window = GetWindow<StateMachineEditorWindow>("State Machine");
            window.Close();
        }

        public static void OpenEditorWindow(string _filePath)
        {
            var window = GetWindow<StateMachineEditorWindow>("State Machine");
            window.selectedFilePath = _filePath;

            if (!string.IsNullOrEmpty(_filePath))
            {
                // Load State Machine to Editor Window
            }

            window.Show();
        }



        #region Callbacks - Selection
        private void OnSelectionChanged()
        {
            var selectedObject = Selection.activeObject;
            if (selectedObject == null) return;

            // Update data.
            string assetPath = AssetDatabase.GetAssetPath(selectedObject);
            if (assetPath.EndsWith(".fsm"))
            {
                selectedFilePath = assetPath;
                stateMachine = StateMachineFileHandler.LoadFromFile(selectedFilePath);
            }
            else
            {
                selectedFilePath = null;
                stateMachine = null;
            }

            // Update visual.


            Repaint();
        }
        #endregion

        #region File Saving
        private void Save()
        {
            StateMachineFileHandler.SaveToFile(stateMachine, selectedFilePath);
        }

        private void SaveNew()
        {
            string newFilePath = EditorUtility.SaveFilePanel(
                "Save FSM File As",     // Dialog title
                "Assets",               // Default directory
                "NewStateMachine",      // Default file name
                "fsm"                   // File extension
            );

            if (string.IsNullOrEmpty(newFilePath)) return;

            StateMachineFileHandler.SaveToFile(stateMachine, newFilePath);
        }
        #endregion

        #region Graph View
        private void SetupGraphView()
        {
            graphView = new(stateMachine)
            {
                name = "Graph View"
            };

            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void RemoveGraphView()
        {
            rootVisualElement.Remove(graphView);
        }
        #endregion

        #region Toolbar
        private void SetupToolbar()
        {
            rootVisualElement.Add(GetToolbarButtonsDisplay());

            if (!string.IsNullOrEmpty(selectedFilePath))
            {
                rootVisualElement.Add(new HelpBox(selectedFilePath, HelpBoxMessageType.None));
            }
        }



        private Toolbar GetToolbarButtonsDisplay()
        {
            Toolbar toolbar = new();
            toolbar.styleSheets.Add(Resources.Load<StyleSheet>("Toolbar"));

            ToolbarButton blackboardToggleButton = new(() => Debug.Log("Toggle Blackboard"))
            {
                text = "Blackboard"
            };
            blackboardToggleButton.AddToClassList("left");
            toolbar.Add(blackboardToggleButton);

            ToolbarButton clearAllButton = new(() => graphView.Clear())
            {
                text = "Clear All",
                
            };
            clearAllButton.AddToClassList("left");
            toolbar.Add(clearAllButton);

            return toolbar;
        }
        #endregion
    }
}