// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;
using UnityEngine.Events;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Preliy.Flange.Planner.RawInstructions
{
    [System.Serializable]
    public class UnityEventInstruction<T> : Instruction
    {
        public UnityEvent<T> UnityEvent
        {
            get => _unityEvent;
            set => _unityEvent = value;
        }
        
        public T Value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private UnityEvent<T> _unityEvent;
        [SerializeField]
        private T _value;
        
        protected override async UniTask LocalExecute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _unityEvent?.Invoke(_value);
            await UniTask.Yield(playerLoopTiming);
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "EVENT");
        }
        
        protected const string FORMAT_DESCRIPTION = "{0}: {1}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, "UnityEvent:", _unityEvent);
        }
    }    
}

