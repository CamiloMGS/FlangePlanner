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
        
        protected override async UniTask LocalExecute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _action?.Invoke();
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "ACTION");
        }
        
        protected const string FORMAT_DESCRIPTION = "{0}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, _action);
        }
    }    
}

