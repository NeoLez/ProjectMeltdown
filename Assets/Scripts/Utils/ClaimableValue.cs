using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Timers {
    public class ClaimableValue<T> where T : struct {
        private readonly List<(T Value, Component Requester)> _valueList = new();
        private Action<T> _setValueAction;
        private Func<T> _setDefaultAction;

        public ClaimableValue(Action<T> setValueAction, Func<T> setDefaultAction)
        {
            if(setValueAction == null || setDefaultAction == null)
                throw new ArgumentNullException($"Tried to initialize {nameof(ClaimableValue<T>)} with null actions.)");
            _setValueAction = setValueAction;
            _setDefaultAction = setDefaultAction;
        }

        public T GetCurrentValue() {
            if (_valueList.Count > 0) return _valueList[^1].Value;
            return _setDefaultAction.Invoke();
        }
        
        public void RequestControl(T value, Component requester) {
            if (requester == null) return;
            
            _valueList.Add(ValueTuple.Create(value, requester));
            _setValueAction.Invoke(value);
        }
        
        public void RelinquishControl(Component requester) {
            if (_valueList.Count == 0) return;
            bool topChanged = ClearNullRequestsAtTopOfStack();
            
            if (_valueList.Count == 0)
            {
                if (topChanged) 
                    _setValueAction.Invoke(_setDefaultAction.Invoke());
                return;
            }
            
            for (int i = _valueList.Count - 1; i >= 0; i--)
            {
                if (_valueList[i].Requester == null)
                {
                    _valueList.RemoveAt(i);
                    if (i == _valueList.Count) topChanged = true;
                    continue;
                }

                if (_valueList[i].Requester == requester)
                {
                    _valueList.RemoveAt(i);
                    if(i == _valueList.Count) topChanged = true;
                    break;
                }
            }

            if (topChanged || ClearNullRequestsAtTopOfStack()) {
                if (_valueList.Count == 0)
                {
                    _setValueAction.Invoke(_setDefaultAction.Invoke());
                }
                else
                {
                    _setValueAction.Invoke(_valueList[^1].Value);
                }
            }
        }

        private bool ClearNullRequestsAtTopOfStack() {
            bool clearedAny = false;
            while (_valueList.Count > 0 && _valueList[^1].Requester == null)
            {
                _valueList.RemoveAt(_valueList.Count - 1);
                clearedAny = true;
            }

            return clearedAny;
        }
        
        public void ClearListAndSetToDefault()
        {
            _valueList.Clear();
            _setValueAction.Invoke(_setDefaultAction.Invoke());
        }

        public override string ToString() {
            StringBuilder sb = new();
            foreach (var valueTuple in _valueList)
            {
                sb.Append($"{valueTuple.Value.ToString()} - {valueTuple.Requester}");
            }
            return sb.ToString();
        }
    }
}