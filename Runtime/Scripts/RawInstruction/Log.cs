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
    public class Log : Instruction
    {
        public string Message
        {
            get => _message;
            set => _message = value;
        }
        
        public LogType LogType
        {
            get => _logType;
            set => _logType = value;
        }

        [SerializeField]
        private string _message;
        [SerializeField]
        private LogType _logType;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _state = InstructionState.Busy;
            await UniTask.Yield(playerLoopTiming);
            _state = InstructionState.Done;
        }
    }    
}

