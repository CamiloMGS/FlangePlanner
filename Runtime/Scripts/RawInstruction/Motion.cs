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
    public abstract class Motion : Instruction
    {
        public Trajectory Trajectory => _trajectory;
        
        public float Speed 
        {
            get => _speed;
            set => _speed = value;
        }

        public float Blending
        {
            get => _blending;
            set => _blending = value;
        }
        
        public bool IsTempJob
        {
            get => _isTempJob;
            set => _isTempJob = value;
        }

        [SerializeField]
        protected float _speed;
        [SerializeField]
        protected float _blending;
        [SerializeField]
        protected bool _isTempJob;

        protected Trajectory _trajectory;

        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            try
            {
                if (_controller == null) throw new Exception("Controller is null");
                
                _state = InstructionState.Busy;
                await ExecuteTrajectory(playerLoopTiming, cancellationToken);
                _state = InstructionState.Done;
            }
            catch (Exception e) when (e is not OperationCanceledException)
            {
                throw;
            }
        }
        
        public abstract void CreateTrajectory(JointTarget initJointState);
        
        protected async UniTask ExecuteTrajectory(PlayerLoopTiming playerLoopTiming, CancellationToken token)
        {
            if (_trajectory == null)
            {
                CreateTrajectory(_controller.MechanicalGroup.JointState);
                await UniTask.Yield(playerLoopTiming);
            }

            if (_trajectory == null)
            {
                throw new Exception("Trajectory is null!");
            }

            foreach (var key in _trajectory.Keys)
            {
                _controller.MechanicalGroup.SetJoints(key.JointPosition, true);
                await UniTask.Yield(playerLoopTiming);
                token.ThrowIfCancellationRequested();
            }

            if (_isTempJob) _trajectory = null;
        }

        public abstract Matrix4x4 GetTargetWorld(Controller controller);
        public abstract JointTarget GetJointTarget(Controller controller);

        public abstract void JumpToTarget();
    }
}

