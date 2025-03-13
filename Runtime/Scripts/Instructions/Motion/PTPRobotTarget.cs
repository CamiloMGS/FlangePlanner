// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class PTPRobotTarget : JointMotion
    {
        public CartesianTarget CartesianTarget
        {
            get => _cartesianTarget;
            set => _cartesianTarget = value;
        }
        
        public int Tool
        {
            get => _tool;
            set => _tool = value;
        }
        
        public int Frame
        {
            get => _frame;
            set => _frame = value;
        }

        [SerializeField]
        private CartesianTarget _cartesianTarget;
        [SerializeField]
        private int _tool;
        [SerializeField]
        private int _frame;

        public override void Plan(Controller controller, int index)
        {
            try
            {
                base.Plan(controller, index);
                //TODO Validate Joint target

                _tool = _controller.GetValidToolIndex(_tool); 
                
                var solution = _controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);
                if (!solution.IsValid) throw solution.Exception;
                
                ////_target = _controller.FrameToWorld(_cartesianTarget.Pose, _frame, _cartesianTarget.ExtJoint);
                //var solution = _controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);
                //if (!solution.IsSuccess) throw solution.Exception;
                
                _state = InstructionState.Ready;
            }
            catch (Exception)
            {
                _state = InstructionState.Error;
                throw;
            }
        }
        
        public override Matrix4x4 GetTargetWorld(Controller controller)
        {
            return controller.Solver.ComputeForward(_jointTarget, _tool);
        }
        
        public override JointTarget GetJointTarget(Controller controller)
        {
            return _jointTarget;
        }
        
        public override void JumpToTarget()
        {
            if (_state != InstructionState.Ready)
            {
                throw new Exception($"Instruction state: {_state} isn't valid! JumpToTarget requires a Ready state");
            }

            //if (_controller == null) throw new NullReferenceException("Controller is null");
            
            var solution = _controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);
            if (!solution.IsValid) throw solution.Exception;

            _controller.Solver.TryApplySolution(solution);
        }
    }
}

