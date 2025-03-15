// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    public class SubTask : Instruction
    {
        public Task Task
        {
            get => _task;
            set => _task = value;
        }

        [SerializeField]
        private Task _task;

        public override void Plan(Controller controller, int index)
        {
            try
            {
                if (_task == null) throw new Exception("SubTask is null!");
                if (_controller != _task.Controller) throw new Exception("SubTask Controller is not equal to parent Task Controller!");
                _state = InstructionState.Ready;
            }
            catch (Exception)
            {
                _state = InstructionState.Error;
                throw;
            }
        }
        
        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            await _task.Execute(playerLoopTiming, cancellationToken);
            _state = InstructionState.Done;
        }
    }    
}

