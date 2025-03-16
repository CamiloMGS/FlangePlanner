// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class PTPCartesianTarget : JointMotion
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

        public override void Plan()
        {
            try
            {
                base.Plan();

                _tool = _controller.GetValidToolIndex(_tool); 
                
                var solution = _controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);
                if (!solution.IsValid) throw solution.Exception;

                _jointTarget = solution.JointTarget;
                
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
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "PTP");
        }
        
        protected const string FORMAT_DESCRIPTION = 
            "R: {0}" +
            "\nE: {1}" +
            "\nS: {2}" +
            "\nB: {3}" +
            "\nT: {4}" +
            "\nF: {5}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, _jointTarget.RobJoint, _jointTarget.ExtJoint, _speed, _blending, _tool, _frame);
        }
    }
}

