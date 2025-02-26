using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Custom.Extensions;
using System.Linq;

namespace Custom.FSM.Editor
{
    public class StateNode : GraphElement
    {
        public State stateData;

        private readonly VisualElement mainContainer;

        private readonly VisualElement nodeStatusDisplay;
        private bool active = true;
        private bool lastActive = true;

        private readonly VisualElement titleContainer;
        private readonly Label titleLabel;

        private readonly VisualElement collapseButton;
        private bool expanded = true;

        private readonly VisualElement behaviourContainer;
        private readonly List<StateNodeBehaviour> behaviours = new();
        private readonly HelpBox emptyStateHelpBox;

        private readonly HashSet<StateNodeTransition> inputTransitions = new();
        private readonly HashSet<StateNodeTransition> outputTransitions = new();


        /// <summary>
        /// Node's title element.
        /// </summary>
        public override string title
        {
            get
            {
                return (titleLabel != null) ? titleLabel.text : string.Empty;
            }
            set
            {
                if (titleLabel != null)
                {
                    titleLabel.text = value;
                }
            }
        }

        /// <summary>
        /// Expanded nodes will attempt to display attached behaviours.
        /// </summary>
        private bool Expanded
        {
            get
            {
                return expanded;
            }
            set
            {
                if (expanded == value) return;

                expanded = value;
                RefreshExpandedState();
            }
        }

        /// <summary>
        /// Inactive node will be rendered partially transparent.
        /// </summary>
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                if (active == value) return;

                active = value;

                if (active)
                    RemoveFromClassList("disabled");
                else
                    AddToClassList("disabled");
            }
        }



        public StateNode(string _title)
            : this(new State(_title)) { }

        public StateNode(State _stateData)
        {
            stateData = _stateData;

            capabilities |=
                Capabilities.Selectable |
                Capabilities.Movable |
                Capabilities.Deletable |
                Capabilities.Ascendable |
                Capabilities.Copiable |
                Capabilities.Snappable |
                Capabilities.Groupable;

            usageHints = UsageHints.DynamicTransform;

            // Setup Display
            VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("StateNode");

            if (!visualTreeAsset) return;

            visualTreeAsset.CloneTree(this);

            AddToClassList("node");
            mainContainer = this.Q("main-container");
            mainContainer.style.minWidth = 150;

            nodeStatusDisplay = this.Q("node-status");

            titleContainer = this.Q("title");
            titleLabel = this.Q<Label>("title-label");
            titleLabel.text = stateData.name;

            behaviourContainer = this.Q("behaviours-container");
            emptyStateHelpBox = this.Q<HelpBox>("empty-state-content");

            collapseButton = this.Q("collapse-button");
            collapseButton.AddManipulator(new Clickable(ToggleCollapse));

            UpdateNodeStatusDisplay();
            RefreshExpandedState();

            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));

            this.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            this.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }



        #region Overrides
        public override void OnSelected()
        {
            base.OnSelected();

            AddToClassList("selected");
            lastActive = Active;
            Active = true;
        }

        public override void OnUnselected()
        {
            base.OnUnselected();

            RemoveFromClassList("selected");
            Active = lastActive;
        }



        public override void SetPosition(Rect newPos)
        {
            style.position = Position.Absolute;
            style.left = newPos.x;
            style.top = newPos.y;
        }

        public override Rect GetPosition()
        {
            if (resolvedStyle.position == Position.Absolute)
            {
                return new Rect(resolvedStyle.left, resolvedStyle.top, layout.width, layout.height);
            }

            return layout;
        }
        #endregion

        #region Contextual Menu (Right Click Menu)
        private void BuildContextualMenu(ContextualMenuPopulateEvent _event)
        {
            if (_event.target is not StateNode) return;
            
            _event.menu.AppendAction("Add Behaviour", AddBehaviour, AddBehaviourStatus);
            _event.menu.AppendSeparator();
        }



        private void AddBehaviour(DropdownMenuAction _action)
        {
            // TODO DATA LOGIC

            StateBehaviourIdle behaviour = ScriptableObject.CreateInstance<StateBehaviourIdle>();

            stateData.AddStateBehaviour(behaviour);

            AddBehaviourDisplay(behaviour);
        }

        private DropdownMenuAction.Status AddBehaviourStatus(DropdownMenuAction _action)
        {
            if (stateData.status == StateStatus.Exit)
                return DropdownMenuAction.Status.Disabled;
            else
                return DropdownMenuAction.Status.Normal;
        }
        #endregion

        #region UI Callbacks
        private void OnGeometryChanged(GeometryChangedEvent _event)
        {
            UpdateTransitionsPosition();
        }

        private void OnDetachFromPanel(DetachFromPanelEvent _event)
        {
            foreach (var transition in inputTransitions.ToArray())
            {
                transition.RemoveFromHierarchy();
            }

            foreach (var transition in outputTransitions.ToArray())
            {
                transition.RemoveFromHierarchy();
            }
        }
        #endregion

        #region Node Status
        private void UpdateNodeStatusDisplay()
        {
            switch (stateData.status)
            {
                case StateStatus.Inactive:
                case StateStatus.Active:
                    nodeStatusDisplay.style.backgroundColor = new Color(0.21f, 0.21f, 0.21f, 1.0f);
                    break;

                case StateStatus.Entry:
                    nodeStatusDisplay.style.backgroundColor = new Color(0.22f, 0.86f, 0.35f, 1.0f);
                    break;

                case StateStatus.Exit:
                    nodeStatusDisplay.style.backgroundColor = new Color(0.8f, 0.2f, 0.2f, 1.0f);
                    break;

                default: break;
            }

            Active = stateData.status != StateStatus.Inactive;
        }
        #endregion

        #region Node Collapse/Expand
        private void ToggleCollapse()
        {
            Expanded = !Expanded;
        }

        private void RefreshExpandedState()
        {
            RemoveFromClassList(expanded ? "collapsed" : "expanded");
            AddToClassList(expanded ? "expanded" : "collapsed");
        }
        #endregion

        #region State Transitions
        public void AddTransition(StateNodeTransition _transition, Direction _direction)
        {
            if (_direction == Direction.Input)
            {
                inputTransitions.Add(_transition);
            }
            else
            {
                outputTransitions.Add(_transition);
            }

            UpdateTransitionsPosition();
        }

        public void RemoveTransition(StateNodeTransition _transition, Direction _direction)
        {
            if (_direction == Direction.Input)
            {
                inputTransitions.Remove(_transition);
            }
            else
            {
                outputTransitions.Remove(_transition);
            }

            UpdateTransitionsPosition();
        }

        public void UpdateTransitionsPosition()
        {
            foreach (var transition in inputTransitions)
            {
                SnapTransitionToNode(transition, Direction.Input);
            }

            foreach (var transition in outputTransitions)
            {
                SnapTransitionToNode(transition, Direction.Output);
            }
        }



        private void SnapTransitionToNode(StateNodeTransition _transition, Direction _direction)
        {
            var side = worldBound.ClosestSide((_direction == Direction.Input) ? _transition.StartPoint : _transition.EndPoint);

            if (_direction == Direction.Input) 
                _transition.EndPoint = GetTransitionPositionAtSide(_direction, side);
            else 
                _transition.StartPoint = GetTransitionPositionAtSide(_direction, side);
        }

        private Vector2 GetTransitionPositionAtSide(Direction _direction, RectSide _side)
        {
            switch (_side)
            {
                case RectSide.Top:
                    return new Vector2(worldBound.center.x, worldBound.yMax);

                case RectSide.Bottom:
                    return new Vector2(worldBound.center.x, worldBound.yMin);

                case RectSide.Left:
                    return new Vector2(worldBound.xMin, worldBound.center.y);

                case RectSide.Right:
                    return new Vector2(worldBound.xMax, worldBound.center.y);

                default: 
                    return Vector2.zero;
            }
        }
        #endregion

        #region State Behaviours
        private void AddBehaviourDisplay(StateBehaviour _behaviourData)
        {
            StateNodeBehaviour nodeBehaviour = new(_behaviourData);

            behaviourContainer.Add(nodeBehaviour);
            behaviours.Add(nodeBehaviour);

            RefreshEmptyBehaviourStatus();
        }

        private void RefreshEmptyBehaviourStatus()
        {
            if (behaviours.Count == 0)
                emptyStateHelpBox.style.display = DisplayStyle.Flex;
            else
                emptyStateHelpBox.style.display = DisplayStyle.None;
        }
        #endregion
    }
}
