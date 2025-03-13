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
    public abstract class Instruction
    {
        public string Name => string.Format(FORMAT, _index, GetType());

        public Controller Controller => _controller;
        public int Index => _index;
        public InstructionState State => _state;

        [SerializeField]
        protected Controller _controller;
        [SerializeField]
        protected int _index;
        [SerializeField]
        protected InstructionState _state = InstructionState.Idle;

        private const string FORMAT = "{0} {1}";

        public virtual void Plan(Controller controller, int index)
        {
            _state = InstructionState.Idle;
            _controller = controller;
            _index = index;
        }

        public abstract UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken);
    }
}


