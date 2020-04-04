using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace DoubleUpsilon {
    public static class GameEvents {
        private static readonly Dictionary<Type, object> EventHandlers = new Dictionary<Type, object>();

        public static void Subscribe<T>(Action<T> action) where T : Event {
            Subscribe(action, null);
        }

        public static void Subscribe<T>(Action<T> action, Func<T, bool> predicate) where T : Event {
            var type = typeof(T);
            if (!EventHandlers.ContainsKey(type)) {
                EventHandlers.Add(type, new List<Tuple<Action<T>, Func<T, bool>>>());
            }

            var eventHandlers = EventHandlers[type] as List<Tuple<Action<T>, Func<T, bool>>>;
            eventHandlers?.Add(new Tuple<Action<T>, Func<T, bool>>(action, predicate));
        }

        // TODO: a function as a middleware that is used to modify the event before it is actually fired.
        public static void Intercept<T>(Action<T> action, Func<T, bool> predicate) where T : Event { }

        public static void UnsubscribeFromAll() {
            EventHandlers.Clear();
        }

        public static void Unsubscribe<T>(Action<T> action) where T : Event {
            var type = typeof(T);
            if (!EventHandlers.ContainsKey(type)) {
                Debug.Log("Nothing found");
                return;
            }

            var eventHandlers = EventHandlers[type] as List<Tuple<Action<T>, Func<T, bool>>>;
            eventHandlers?.RemoveAll(tuple => tuple.Item1 == action);
        }

        public static void Raise<T>(T raisedEvent) where T : Event {
            if (!(raisedEvent is Event.INoLog)) Debug.Log(raisedEvent);

            var type = typeof(T);
            if (!EventHandlers.ContainsKey(type)) return;

            var eventHandlers = EventHandlers[type] as List<Tuple<Action<T>, Func<T, bool>>>;

            if (eventHandlers == null) return;
            foreach (var eventHandler in eventHandlers) {
                var action    = eventHandler.Item1;
                var predicate = eventHandler.Item2;
                if (predicate == null || predicate(raisedEvent)) {
                    UnityEnvironment.Instance.Run(() => {
                        action(raisedEvent);
                    });
                }
            }
        }
    }

    public abstract class Event {
        public interface INoLog { }
    }
}