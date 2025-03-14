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
        
        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            _unityEvent?.Invoke(_value);
            await UniTask.Yield(playerLoopTiming);
            _state = InstructionState.Done;
        }
    }    
}

