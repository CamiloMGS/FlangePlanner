// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    public abstract class JointMotion : Motion
    {
        public JointTarget JointTarget
        {
            get => _jointTarget;
            set => _jointTarget = value;
        }
        
        [SerializeField]
        protected JointTarget _jointTarget = JointTarget.Default;
        
        public override void CreateTrajectory(JointTarget initJointState)
        {
            _trajectory = new Trajectory();
            
            var ramp = RampExtension.GetConfigs(_controller.MechanicalGroup, _speed, initJointState, _jointTarget)
                .GetRamps(_controller.SampleTime)
                .GetMaxDuration();

            foreach (var key in ramp.Keys)
            {
                var jointPosition = new JointTarget();

                for (var i = 0; i < JointTarget.LENGTH; i++)
                {
                    jointPosition[i] = Mathf.Lerp(initJointState[i], _jointTarget[i], key.Progress);
                }

                var trajectoryKey = new TrajectoryKey
                {
                    JointPosition = jointPosition,
                    CartesianPose = _controller.Solver.ComputeForward(jointPosition, -1),
                    IsValid = true
                };

                _trajectory.Add(trajectoryKey, _controller.SampleTime);
            }
        }
    }
}
