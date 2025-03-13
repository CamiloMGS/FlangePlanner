// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class CIRC : CartesianMotion
    {
        public CartesianTarget WayPoint
        {
            get => _wayPoint;
            set => _wayPoint = value;
        }

        public Matrix4x4 TargetWayPoint => _targetWayPoint;
        
        [SerializeField]
        private Matrix4x4 _targetWayPoint;
        [SerializeField]
        private CartesianTarget _wayPoint;
        
        public override void Plan(Controller controller, int index)
        {
            try
            {
                base.Plan(controller, index);
                
                _tool = _controller.GetValidToolIndex(_tool); 
                _target = _controller.FrameToWorld(_cartesianTarget.Pose, _frame, _cartesianTarget.ExtJoint);
                _targetWayPoint = _controller.FrameToWorld(_wayPoint.Pose, _frame, _wayPoint.ExtJoint);
                var solution = _controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);
                if (!solution.IsValid) throw solution.Exception;
                
                _state = InstructionState.Ready;
            }
            catch (Exception)
            {
                _state = InstructionState.Error;
                throw;
            }
        }

        public override void CreateTrajectory(JointTarget initJointState)
        {
            _trajectory = new Trajectory();

            var startTransform = _controller.Solver.ComputeForward(initJointState, _tool);
            startTransform = _controller.WorldToFrame(startTransform, _frame, initJointState.ExtJoint);
            
            var arc = new Arc(startTransform.GetPosition(), _cartesianTarget.Pose.GetPosition(), _wayPoint.Pose.GetPosition());
            var ramp = RampExtension.GetConfigs(_controller.MechanicalGroup, _speed, startTransform, _cartesianTarget.Pose, initJointState.ExtJoint, _cartesianTarget.ExtJoint, arc.Length)
                .GetRamps(_controller.SampleTime)
                .GetMaxDuration();

            foreach (var key in ramp.Keys)
            {
                var extJointTarget = new ExtJoint();
                
                for (var i = 0; i < ExtJoint.LENGTH; i++)
                {
                    extJointTarget[i] = Mathf.Lerp(initJointState.ExtJoint[i], _cartesianTarget.ExtJoint[i], key.Progress);
                }
                
                var position = arc.GetPoint(key.Progress);
                var rotation = Quaternion.Slerp(startTransform.rotation, _cartesianTarget.Pose.rotation, key.Progress);
                var target = Matrix4x4.TRS(position, rotation, Vector3.one);
                
                var robotTarget = new CartesianTarget(target, _cartesianTarget.Configuration, extJointTarget);
                
                var solution = _controller.Solver.ComputeInverse(robotTarget, _tool, _frame);

                var trajectoryKey = new TrajectoryKey
                {
                    CartesianPose = _controller.FrameToWorld(robotTarget.Pose, _frame, robotTarget.ExtJoint),
                    JointPosition = solution.JointTarget,
                    IsValid = solution.IsValid
                };

                _trajectory.Add(trajectoryKey, _controller.SampleTime);
            }
        }
        
        public override Matrix4x4 GetTargetWorld(Controller controller)
        {
            return controller.FrameToWorld(_cartesianTarget.Pose, _frame, _cartesianTarget.ExtJoint);
        }
        
        public override JointTarget GetJointTarget(Controller controller)
        {
            var solution = controller.Solver.ComputeInverse(_cartesianTarget, _tool, _frame);

            if (solution.IsValid)
            {
                return solution.JointTarget;
            }
            
            throw new Exception($"IK Solution is not valid: {solution}");
        }
        
        public override void JumpToTarget()
        {
            throw new NotImplementedException();
        }
    }
}

