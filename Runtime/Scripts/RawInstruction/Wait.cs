// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
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

        protected override async UniTask LocalExecute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            await UniTask.Delay(_time, DelayType.Realtime, cancellationToken: cancellationToken);
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "WAIT");
        }
        
        protected const string FORMAT_DESCRIPTION = "{0}: {1}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, "Time[ms]", _time);
        }
    }    
}

