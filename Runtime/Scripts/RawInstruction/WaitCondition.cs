// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    public class WaitCondition : Instruction
    {
        public Func<bool> Condition
        {
            get => _condition;
            set => _condition = value;
        }

        private Func<bool> _condition;
        
        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            await UniTask.WaitUntil(_condition, cancellationToken: cancellationToken);
            _state = InstructionState.Done;
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "Wait Until");
        }
        
        protected const string FORMAT_DESCRIPTION = "{0}: {1}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, "Condition", _condition);
        }
    }    
}

