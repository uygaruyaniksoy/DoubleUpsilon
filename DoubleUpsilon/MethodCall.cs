using System;
using System.Collections;
using UnityEngine;

namespace DoubleUpsilon {
    public enum TimeEnum {
        MilliSeconds = 1,
        Seconds      = 1000,
        Minutes      = 60000,
    }

    public class MethodCall {
        private Action _action;
        private Action _finally;

        private int _interval;
        private int _times;

        public MethodCall(Action action) {
            _action = action;
        }

        public MethodCall Every(int amount, TimeEnum unit) {
            _interval = amount * (int) unit;
            return this;
        }

        public MethodCall Times(int times) {
            _times = times;
            return this;
        }

        public MethodCall Finally(Action action) {
            _finally = action;
            return this;
        }

        public void Start() {
            if (_times == 0) _times = 1;
            if (_interval == 0) _interval = 1 * (int) TimeEnum.Seconds;

            UnityEnvironment.Instance.RunCoroutine(MethodProcess);
        }

        private IEnumerator MethodProcess() {
            while (_times > 0) {
                _action();
                yield return new WaitForSeconds(_interval / 1000f);
                _times--;
            }

            if (_finally != null) {
                _finally();
            }
        }
    }
}