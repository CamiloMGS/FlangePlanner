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

        protected override void LocalPlan()
        {
            if (_task == null) throw new Exception("SubTask is null!");
            if (_controller != _task.Controller) throw new Exception("SubTask Controller is not equal to parent Task Controller!");
        }
        
        protected override async UniTask LocalExecute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            await _task.Execute(playerLoopTiming, cancellationToken);
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "TASK");
        }
        
        protected const string FORMAT_DESCRIPTION = "{0}: {1}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, "SubTask:", _task.name);
        }
    }    
}

