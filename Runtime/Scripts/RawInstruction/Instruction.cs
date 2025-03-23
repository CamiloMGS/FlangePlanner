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
        public InstructionState State
        {
            get => _state;
            set => _state = value;
        }
        public Controller Controller => _controller;

        [SerializeField]
        // ReSharper disable once InconsistentNaming
        // Need for unity Array Element Name
        protected string name;
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
            try
            {
                if (_state != InstructionState.Idle) throw new Exception($"{name}: State is not Idle");
                if (_controller == null) throw new Exception("Controller is null");
                LocalPlan();
                _state = InstructionState.Ready;
            }
            catch (Exception)
            {
                _state = InstructionState.Error;
                throw;
            }
        }

        public async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            try
            {
                if (_state != InstructionState.Ready) throw new Exception($"{name}: State is not Ready");
                if (_controller == null) throw new Exception("Controller is null");
                
                _state = InstructionState.Busy;
                await LocalExecute(playerLoopTiming, cancellationToken);
                _state = InstructionState.Done;
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                _state = InstructionState.Error;
                throw;
            }
        }

        public abstract override string ToString();
        public abstract string ToDescription();

        protected virtual void LocalPlan()
        {
            
        }
        
        protected abstract UniTask LocalExecute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken);
    }
}


