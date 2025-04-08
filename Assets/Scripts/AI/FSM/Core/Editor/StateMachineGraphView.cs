using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

namespace Custom.FSM.Editor
{
    public class StateMachineGraphView : GraphView
    {
        private StateMachine stateMachine;

        private IEventHandler curEventHandler;

        private new readonly List<StateNode> nodes = new();

        private StateNodeTransition curTransitionArrow;



        public StateMachineGraphView()
            : this(null) { }

        public StateMachineGraphView(StateMachine _stateMachine)
        {
            AddManipulators();

            AddStyles();

            AddGridBackground();

            UpdateStateMachineVisual(_stateMachine);

            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }



        #region Initializers
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale + 0.4f);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddStyles()
        {
            styleSheets.Add(Resources.Load<StyleSheet>("GridBackground"));
        }

        private void AddGridBackground()
        {
            GridBackground bg = new()
            {
                visible = true,
            };

            bg.StretchToParentSize();

            Insert(0, bg);
        }

        public void UpdateStateMachineVisual(StateMachine _stateMachine)
        {
            if (_stateMachine)
            {
                stateMachine = _stateMachine;
            }
            else
            {
                stateMachine = new StateMachine();

                CreateNodeAtLocation("Entry", new Vector2(100, 100));
            }

            // Display State Machine data here.
        }
        #endregion

        #region Contextual Menu (Right Click Menu)
        public override void BuildContextualMenu(ContextualMenuPopulateEvent _event)
        {
            curEventHandler = _event.target;

            if (_event.target is GraphView)
            {
                _event.menu.AppendAction("Create Empty State", CreateEmptyState);
                _event.menu.AppendAction("Create Empty Sub-State machine", CreateSubStateMachine);
                _event.menu.AppendSeparator();

                _event.menu.AppendAction("Paste", Paste, PasteStatus);
                _event.menu.AppendAction("Copy Current State Machine", CopyStateMachine);
            }

            if (_event.target is StateNode)
            {
                _event.menu.AppendAction("Make Transition", MakeTransition, MakeTransitionStatus);
                _event.menu.AppendSeparator();
            }
        }



        private void CreateEmptyState(DropdownMenuAction _action)
        {
            CreateNodeAtLocation(
                stateMachine.MakeUniqueStateName("New State"),
                contentViewContainer.WorldToLocal(_action.eventInfo.localMousePosition)
            );
        }


        private void CreateSubStateMachine(DropdownMenuAction _action)
        {
            
        }


        private void Paste(DropdownMenuAction _action)
        {
            PasteCallback();
        }

        private DropdownMenuAction.Status PasteStatus(DropdownMenuAction _action)
        {
            return canPaste ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }


        private void CopyStateMachine(DropdownMenuAction _action)
        {
            Debug.Log("Pending implementation for Copy State Machine.");
        }


        private void MakeTransition(DropdownMenuAction _action)
        {
            CreateTransition(_action.eventInfo.mousePosition);
        }

        private DropdownMenuAction.Status MakeTransitionStatus(DropdownMenuAction _action)
        {
            if (((StateNode)curEventHandler).stateData.status == StateStatus.Exit) 
                return DropdownMenuAction.Status.Disabled;
            else
                return DropdownMenuAction.Status.Normal;
        }
        #endregion

        #region UI Callbacks 
        private void OnMouseUp(MouseUpEvent _event)
        {
            if (curTransitionArrow != null)
                ConfirmTransition(_event);
        }

        private void OnMouseMove(MouseMoveEvent _event)
        {
            if (curTransitionArrow != null)
                UpdateCurrentTransition(_event);
        }
        #endregion

        #region Node Controls
        public override EventPropagation DeleteSelection()
        {
            foreach (var element in selection)
            {
                if (element is not StateNode) continue;

                StateNode node = (StateNode)element;
                stateMachine.RemoveState(node.stateData);
            }

            return base.DeleteSelection();
        }



        private void CreateNodeAtLocation(string _title, Vector3 _location)
        {
            State stateData = stateMachine.AddState(_title);

            DisplayNodeAtLocation(stateData, _location);
        }

        private void CreateNodeAtLocation(State _stateData, Vector3 _location)
        {
            stateMachine.AddState(_stateData);

            DisplayNodeAtLocation(_stateData, _location);
        }

        private void RemoveNode(StateNode _node)
        {
            RemoveElement(_node);

            nodes.Remove(_node);
            stateMachine.RemoveState(_node.stateData);
        }

        private void DisplayNodeAtLocation(State _stateData, Vector3 _location)
        {
            StateNode node = new(_stateData);
            node.SetPosition(new Rect(_location, Vector2.zero));

            AddElement(node);

            nodes.Add(node);
        }
        #endregion

        #region Transition Controls
        private void CreateTransition(Vector2 _worldStartPoint)
        {
            StateNode selectedNode = (StateNode)curEventHandler;

            curTransitionArrow = new()
            {
                StartNode = selectedNode,

                DrawStartArrow = false,
                DrawEndArrow = false,

                EndFollowsCursor = true,
            };

            curTransitionArrow.StartPoint = _worldStartPoint;
            curTransitionArrow.EndPoint = _worldStartPoint;

            AddElement(curTransitionArrow);

            selectedNode.AddTransition(curTransitionArrow, Direction.Output);
        }

        private void ConfirmTransition(MouseUpEvent _event)
        {
            // Left click
            if (_event.button == 0)
            {
                bool validTransition = false;

                curTransitionArrow.EndFollowsCursor = false;

                List<VisualElement> elementsAtCursor = new();
                panel.PickAll(_event.mousePosition, elementsAtCursor);

                foreach (var element in elementsAtCursor)
                {
                    if (element is StateNode node && element != curTransitionArrow.StartNode)
                    {
                        curTransitionArrow.EndNode = node;
                        node.AddTransition(curTransitionArrow, Direction.Input);
                        validTransition = true;
                        break;
                    }
                }

                if (!validTransition)
                {
                    RemoveElement(curTransitionArrow);
                }
            }
            // Right click
            else if (_event.button == 1)
            {
                RemoveElement(curTransitionArrow);
            }

            curTransitionArrow = null;
        }

        private void UpdateCurrentTransition(MouseMoveEvent _event)
        {
            if (!curTransitionArrow.EndFollowsCursor) return;

            curTransitionArrow.EndPoint = _event.mousePosition;

            _event.StopPropagation();
        }
        #endregion
    }
}
