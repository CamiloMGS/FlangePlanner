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
    public abstract class Instruction
    {
        public string Name => name;
        public int Index => _index;
        public InstructionState State => _state;

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        // Need for unity Array Element Name
        private string name;
        [SerializeField]
        protected Controller _controller;
        [SerializeField]
        protected int _index;
        [SerializeField]
        protected InstructionState _state = InstructionState.Idle;

        protected const string FORMAT = "{0} {1}";

        public void Initialize(Controller controller, int index)
        {
            _state = InstructionState.Idle;
            _controller = controller;
            _index = index;
            name = ToString();
        }

        public virtual void Plan()
        {
            if (_state != InstructionState.Idle) throw new Exception("State is not Idle");
        }

        public abstract UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken);

        public abstract override string ToString();
        public abstract string ToDescription();
    }
}


