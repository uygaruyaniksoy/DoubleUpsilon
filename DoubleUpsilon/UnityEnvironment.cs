using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoubleUpsilon {
    public class UnityEnvironment : MonoBehaviour {
        private static   UnityEnvironment         _instance;
        private readonly Queue<Action>            _actionsToRun   = new Queue<Action>();
        private readonly Queue<Func<IEnumerator>> _coroutinesToRun = new Queue<Func<IEnumerator>>();

        public static UnityEnvironment Instance => _instance;

        private void Awake() {
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public void Run(Action action) {
            _actionsToRun.Enqueue(action);
        }

        public void RunCoroutine(Func<IEnumerator> func) {
            _coroutinesToRun.Enqueue(func);
        }

        private void Update() {
            while (_actionsToRun.Count != 0)
                _actionsToRun.Dequeue()();
            
            while (_coroutinesToRun.Count != 0)
                StartCoroutine(_coroutinesToRun.Dequeue()());
        }
    }
}