using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;

namespace DoubleUpsilon {
    public class Behaviour: UnityEngine.MonoBehaviour {
        private readonly Dictionary<MethodInfo, object> _subscribedActions = new Dictionary<MethodInfo, object>();
        private void OnEnable() {
            var monoType = GetType();
             
            foreach (var method in monoType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = method.GetCustomAttributes(typeof(Subscribe), true);
                if (attributes.Length <= 0) continue;

                var attribute = attributes[0] as Subscribe;
                if (attribute == null) continue;
                
                var type = attribute.Type;
                var info = UnityEventBase.GetValidMethodInfo(this, method.Name, new[] {type});
                var subs = typeof(GameEvents)
                                         .GetMethods()
                                         .First(mi => mi.Name == "Subscribe" && mi.GetParameters().Length == 1);

                if (subs == null) continue;
                var action = (Action<object>) (o => info.Invoke(this, new[] {o}));
                
                subs.MakeGenericMethod(type)
                    .Invoke(null, new object[]{ action });
                
                _subscribedActions.Add(method, action);
            }            
        }

        private void OnDisable() {
            var monoType = GetType();
             
            foreach (var method in monoType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                var attributes = method.GetCustomAttributes(typeof(Subscribe), true);
                if (attributes.Length <= 0) continue;

                var attribute = attributes[0] as Subscribe;
                if (attribute == null) continue;
                
                var type = attribute.Type;
                var subs = typeof(GameEvents)
                                         .GetMethods()
                                         .First(mi => mi.Name == "Unsubscribe");

                if (subs == null) continue;
                var subscribedAction = _subscribedActions[method];
                _subscribedActions.Remove(method);
                
                subs.MakeGenericMethod(type)
                    .Invoke(null, new[]{ subscribedAction });
            }            
        }
    }
    
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Subscribe: Attribute
    {
        public readonly Type Type;
 
        public Subscribe(Type type) {
            Type = type;
        }
    } 
}