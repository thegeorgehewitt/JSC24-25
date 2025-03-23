using System;
using System.Collections.Generic;

namespace Custom.AI.BehaviourTree
{
    public class Blackboard
    {
        public event Action<string> OnPropertyChanged;

        private readonly Dictionary<string, object> data = new();



        public void PrintAll()
        {
            foreach (var pair in data)
            {
                string suffix;
                if (pair.Value == null)
                    suffix = "Not Set";
                else
                    suffix = "Set";
                UnityEngine.Debug.Log($"{pair.Key}: {suffix}");
            }
        }



        /// <summary>
        /// Sets a value in the blackboard if the key exists, or adds a new key-value pair if it does not. 
        /// </summary>
        /// <param name="_key">   The key associated with the value. </param>
        /// <param name="_value"> The value to set or add. </param>
        public void SetOrAdd<T>(string _key, T _value)
        {
            if (!HasKey(_key))
            {
                data.Add(_key, _value);
            }
            else
            {
                data[_key] = _value;
            }

            OnPropertyChanged?.Invoke(_key);
        }

        /// <summary>
        /// Sets a value in the blackboard if the key exists. 
        /// </summary>
        /// <param name="_key">   The key associated with the value. </param>
        /// <param name="_value"> The value to set. </param>
        /// <returns> 
        /// <see langword="true"/> if the key exists and the value was set; <br/>
        /// otherwise, <see langword="false"/>. 
        /// </returns>
        public bool Set<T>(string _key, T _value)
        {
            if (!HasKey(_key)) return false;

            data[_key] = _value;

            OnPropertyChanged?.Invoke(_key);

            return true;
        }

        /// <summary>
        /// Retrieves a value from the blackboard. 
        /// </summary>
        /// <param name="_key">           The key associated with the value. </param>
        /// <param name="_defaultValue">  The default value if the key is not found. </param>
        /// <returns> 
        /// The retrieved value if found; <br/>
        /// otherwise, the provided default value. 
        /// </returns>
        public T Get<T>(string _key, T _defaultValue = default)
        {
            if (data.TryGetValue(_key, out object value))
            {
                if (value is T typedValue)
                    return typedValue;
            }
            return _defaultValue;
        }

        /// <summary>
        /// Attempts to retrieve a value associated with the given key.
        /// </summary>
        /// <param name="_key">   The key to look up in the data storage. </param>
        /// <param name="_value"> The retrieved value if the key exists and is of the correct type. </param>
        /// <returns>
        /// <see langword="true"/> if the key exists and the value is successfully retrieved; <br/>
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public bool TryGetValue<T>(string _key, out T _value)
        {
            _value = default;

            if (!data.TryGetValue(_key, out object value)) return false;
            if (value is not T typedValue) return false;

            _value = typedValue;
            return true;
        }

        /// <summary>
        /// Checks whether the blackboard contains the specified key. 
        /// </summary>
        /// <param name="_key"> The key to check for existence. </param>
        /// <returns> 
        /// <see langword="true"/> if the key exists; <br/>
        /// otherwise, <see langword="false"/>. 
        /// </returns>
        public bool HasKey(string _key) => data.ContainsKey(_key);

        /// <summary>
        /// Check whether the given key exist and valid within the blackboard. <br/>
        /// <b>NOTE:</b> A key is considered valid if the data attached to it is not <see langword="null"/>.
        /// </summary>
        /// <param name="_key"> The key to check for existence. </param>
        /// <returns>
        /// <see langword="true"/> if the key exists and is valid; <br/>
        /// otherwise, <see langword="false"/>. 
        /// </returns>
        public bool HasValidKey(string _key) => HasKey(_key) && data[_key] != null;

        /// <summary>
        /// Removes a key and its associated value from the blackboard. 
        /// </summary>
        /// <param name="_key"> The key to remove. </param>
        public void Remove(string _key)
        {
            if (!data.ContainsKey(_key)) return;

            data.Remove(_key);
        }

        /// <summary>
        /// Clear the value of a given key. <br/>
        /// This does not remove the key from the blackboard, so using <see cref="HasKey(string)"/> would still returns <see langword="true"/>. <br/>
        /// To remove the key, use <see cref="Remove(string)"/> instead.
        /// </summary>
        /// <param name="_key"> The key to invalidate. </param>
        public void Invalidate(string _key)
        {
            if (!data.ContainsKey(_key)) return;

            data[_key] = null;
        }

        /// <summary>
        /// Add a an empty value key to the blackboard.
        /// </summary>
        /// <param name="_key"> The key to add. </param>
        public void Add(string _key)
        {
            if (data.ContainsKey(_key)) return;

            data.Add(_key, null);
        }

        /// <summary>
        /// Clear all data in the blackboard.
        /// </summary>
        public void Clear() => data.Clear();
    }
}
