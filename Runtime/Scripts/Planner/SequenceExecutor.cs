// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [RequireComponent(typeof(Controller))]
    public class SequenceExecutor : MonoBehaviour
    {
        public IProperty<ExecutionState> State => _state;
        
        [SerializeField]
        private Property<ExecutionState> _state = new (ExecutionState.Idle);
        [SerializeField]
        private Property<Program> _sequence = new (null);
        [SerializeField]
        private Property<bool> _autoStart = new (false);
        [SerializeField]
        private Property<bool> _loop = new (false);
        
        
        private CancellationTokenSource _cancellationTokenSource;
        
        private void OnEnable()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            if (Application.isPlaying && _autoStart.Value) Execute();
        }

        private void OnDisable()
        {
            _cancellationTokenSource?.Cancel();
        }
        
        private void Update()
        {
            if (!Application.isPlaying) return;
            if (_loop.Value && _state.Value == ExecutionState.Done) Execute();
        }
        
        public void Execute()
        {
            if (!Application.isPlaying) return;

            if (_sequence.Value == null)
            {
                Logger.Log(LogType.Error, "Sequence reference is null", this);
                return;
            }
            
            if (_state.Value == ExecutionState.Error) return;

            if (_state.Value == ExecutionState.Busy)
            {
                Logger.Log(LogType.Warning, "Controller is already busy! At the same time, only one task may be executed", this);
                return;
            }

            if (_sequence == null)
            {
                Logger.Log(LogType.Error, "Sequence reference is null", this);
                CancelExecution();
                return;
            }
            
            Cycle().Forget();
        }

        public void Stop()
        {
            if (_state.Value != ExecutionState.Busy) return;
            CancelExecution();
            Logger.Log(LogType.Warning, "Controller execution is canceled! Stop command execution!", this);
        }

        public void ResetError()
        {
            if (_state.Value != ExecutionState.Error) return;
            _state.Value = ExecutionState.Idle;
            Logger.Log(LogType.Log, "Controller reset state to Idle", this);
        }
        
        private async UniTask Cycle()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                _state.Value = ExecutionState.Busy;
                await _sequence.Value.Execute(PlayerLoopTiming.FixedUpdate, _cancellationTokenSource.Token);
                _state.Value = ExecutionState.Done;
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                CancelExecution();
            }
        }

        private void CancelExecution()
        {
            _state.Value = ExecutionState.Error;
            _cancellationTokenSource.Cancel();
        }
        
        public enum ExecutionState
        {
            Idle,
            Busy,
            Done,
            Error
        }
    }
}
