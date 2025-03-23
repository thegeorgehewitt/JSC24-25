namespace Custom.AI.BehaviourTree
{
    /// <summary>
    /// Represents a property that can either have a direct value or be bound to a blackboard value.
    /// </summary>
    public class BindableProperty<T>
    {
        private string key;
        private Blackboard blackboard;

        public T Value { get; private set; }



        public BindableProperty(T _defaultValue)
        {
            Value = _defaultValue;
        }

        public BindableProperty(string _blackboardKey, Blackboard _blackboard)
        {
            Bind(_blackboardKey, _blackboard);
        }

        public BindableProperty(string _blackboardKey, Blackboard _blackboard, T _defaultValue)
        {
            Bind(_blackboardKey, _blackboard, _defaultValue);
        }

        public static implicit operator BindableProperty<T>(T _value)
        {
            return new BindableProperty<T>(_value);
        }

        public static implicit operator T(BindableProperty<T> _property)
        {
            return _property.Value;
        }



        /// <summary>
        /// Binds the property to a blackboard key, allowing it to retrieve its value from the blackboard.
        /// </summary>
        /// <param name="_key"> The key used to look up the value in the blackboard. </param>
        /// <param name="_blackboard">      The blackboard to bind this property to. </param>
        /// <param name="_defaultValue">    The default value to assign to this property. </param>
        public void Bind(string _key, Blackboard _blackboard, T _defaultValue)
        {
            key = _key;
            blackboard = _blackboard;

            if (blackboard.HasKey(_key))
            {
                Value = blackboard.Get<T>(_key);
            }
            else
            {
                blackboard.SetOrAdd(_key, Value);
            }

            blackboard.OnPropertyChanged += OnPropertyChanged;
        }

        /// <inheritdoc cref="Bind(string, Blackboard, T)"/>
        public void Bind(string _key, Blackboard _blackboard)
        {
            key = _key;
            blackboard = _blackboard;

            if (blackboard.HasKey(_key))
            {
                Value = blackboard.Get<T>(_key);
            }
            else
            {
                blackboard.Add(_key);
            }

            blackboard.OnPropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Sets a direct value for the property and unbinds it from the blackboard.
        /// </summary>
        /// <param name="_value"> The direct value to assign. </param>
        public void Unbind(T _value)
        {
            Value = _value;

            Unbind();
        }

        /// <summary>
        /// Unbinds this property from the blackboard and keep its current value.
        /// </summary>
        public void Unbind()
        {
            if (blackboard != null)
                blackboard.OnPropertyChanged -= OnPropertyChanged;

            key = null;
            blackboard = null;
        }



        private void OnPropertyChanged(string _key)
        {
            if (_key != key) return;

            Value = blackboard.Get<T>(key);
        }
    }
}
