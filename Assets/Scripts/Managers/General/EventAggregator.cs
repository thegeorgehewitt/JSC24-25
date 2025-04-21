using System;
using System.Collections.Generic;

using UnityEngine;

namespace Custom.Manager.EventHandling
{
    /// <summary>
    /// Event manager for subscribing and publishing events.
    /// </summary>
    public class EventAggregator
    {
        private static EventAggregator instance;
        public static EventAggregator Instance => instance ??= new();

        private readonly Dictionary<Type, List<Delegate>> eventHandlers = new();



        #region Static 
        /// <summary>
        /// Add a new handler to an event.
        /// </summary>
        /// <typeparam name="T">    Any data structure. </typeparam>
        /// <param name="_handler"> Delegate handler assigned to event. </param>
        public static void Subscribe<T>(Action<T> _handler)
        {
            Instance.Core_Subscribe(_handler);
        }

        /// <inheritdoc cref="Subscribe{T}(Action{T})"/>.
        public static void Subscribe<T>(Action _handler)
        {
            Instance.Core_Subscribe<T>(_handler);
        }

        /// <summary>
        /// Remove a new handler from an event.
        /// </summary>
        /// <typeparam name="T">    Any data structure. </typeparam>
        /// <param name="_handler"> Delegate handler assigned to event. </param>
        public static void Unsubscribe<T>(Action<T> _handler)
        {
            Instance.Core_Unsubscribe(_handler);
        }

        /// <summary>
        /// Call an all handlers attached to an event.
        /// </summary>
        /// <typeparam name="T">      Any data structure. </typeparam>
        /// <param name="_eventData"> A data structure of type <typeparamref name="T"/>. </param>
        /// <returns>
        /// Was there any handlers called with this publish?
        /// </returns>
        public static bool Publish<T>(T _eventData)
        {
            return Instance.Core_Publish(_eventData);
        }
        #endregion



        #region Core
        private void Core_Subscribe<T>(Action<T> _handler)
        {
            Type eventType = typeof(T);
            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Delegate>();
            }
            eventHandlers[eventType].Add(_handler);
        }

        private void Core_Subscribe<T>(Action _handler)
        {
            Type eventType = typeof(T);
            if (!eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType] = new List<Delegate>();
            }
            eventHandlers[eventType].Add(_handler);
        }

        private void Core_Unsubscribe<T>(Action<T> _handler)
        {
            Type eventType = typeof(T);
            if (eventHandlers.ContainsKey(eventType))
            {
                eventHandlers[eventType].Remove(_handler);
                if (eventHandlers[eventType].Count == 0)
                {
                    eventHandlers.Remove(eventType);
                }
            }
        }

        private bool Core_Publish<T>(T _eventData)
        {
            Type eventType = typeof(T);
            if (!eventHandlers.ContainsKey(eventType)) return false;

            var eventHandlersCopy = new List<Delegate>(eventHandlers[eventType]);

            foreach (var handler in eventHandlersCopy)
            {
                if (handler is Action<T> typedDelegate)
                    typedDelegate?.Invoke(_eventData);
                else if (handler is Action emptyDelegate)
                    emptyDelegate?.Invoke();
                else
                    throw new InvalidOperationException($"Occurred during publishing ({eventType})");
            }

            return true;
        }
        #endregion
    }
}
