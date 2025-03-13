// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [System.Serializable]
    public class TaskExecutor : Instruction
    {
        public Task Task
        {
            get => _task;
            set => _task = value;
        }

        [SerializeField]
        private Task _task;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            await _task.Execute(_controller, playerLoopTiming, cancellationToken);
            _state = InstructionState.Done;
        }
    }    
}

