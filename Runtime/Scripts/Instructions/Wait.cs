// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [System.Serializable]
    public class Wait : Instruction
    {
        public int Time
        {
            get => _time;
            set => _time = value;
        }

        [SerializeField]
        private int _time;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            await UniTask.Delay(_time, DelayType.Realtime, cancellationToken: cancellationToken);
            _state = InstructionState.Done;
        }
    }    
}

