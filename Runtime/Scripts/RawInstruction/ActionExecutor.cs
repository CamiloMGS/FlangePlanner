// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    public class ActionExecutor : Instruction
    {
        public Action Action
        {
            get => _action;
            set => _action = value;
        }

        private Action _action;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            _action?.Invoke();
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
            _state = InstructionState.Done;
        }
    }    
}

