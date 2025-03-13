// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    public class SetToolIndex : Instruction
    {
        public int ToolIndex
        {
            get => _toolIndex;
            set => _toolIndex = value;
        }

        [SerializeField]
        private int _toolIndex;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            _controller.Tool.Value = _toolIndex;
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
        }
    }    
}

