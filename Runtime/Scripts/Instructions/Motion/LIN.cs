// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class LIN : CartesianMotion
    {
        public override void Plan(Controller controller, int index)
        {
            try
            {
                base.Plan(controller, index);
                
                //TODO compute inverse with external axes
                _tool = _controller.GetValidToolIndex(_tool); 
                _target = _controller.FrameToWorld(_cartesianTarget.Pose, _frame, _cartesianTarget.ExtJoint);
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
            var startWorld = _controller.Solver.ComputeForward(initJointState, _tool);
            var targetWorld = _controller.FrameToWorld(_cartesianTarget.Pose, _frame, _cartesianTarget.ExtJoint);
            var startLocal = _controller.WorldToFrame(startWorld, _frame, initJointState.ExtJoint);

            var ramp = RampExtension.GetConfigs(_controller.MechanicalGroup, _speed, startWorld, targetWorld, initJointState.ExtJoint, _cartesianTarget.ExtJoint)
                .GetRamps(_controller.SampleTime)
                .GetMaxDuration();

            foreach (var key in ramp.Keys)
            {
                var externalJoints = new ExtJoint();
                
                for (var i = 0; i < ExtJoint.LENGTH; i++)
                {
                    externalJoints[i] = Mathf.Lerp(initJointState.ExtJoint[i], _cartesianTarget.ExtJoint[i], key.Progress);
                }
                
                var position = Vector3.Lerp(startLocal.GetPosition(), _cartesianTarget.Pose.GetPosition(), key.Progress);
                var rotation = Quaternion.Slerp(startLocal.rotation, _cartesianTarget.Pose.rotation, key.Progress);
                var target = Matrix4x4.TRS(position, rotation, Vector3.one);
                
                var robotTarget = new CartesianTarget(target, _cartesianTarget.Configuration, externalJoints);
                
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

