// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Linq;
using Preliy.Flange.Planner;
using UnityEngine;

namespace Preliy.Flange
{
    public static class TrajectoryUtils
    {
        public static Trajectory GetLerpSegment(Trajectory t1, Trajectory t2, int offset, float sampleTime)
        {
            var trajectory = new Trajectory();
            
            var inputTransform = t1.Keys[^(offset + 1)].CartesianPose;
            var targetTransform = t1.Keys.Last().CartesianPose;
            var outputTransform = t2.Keys[offset].CartesianPose;
            var inputPosition = inputTransform.GetPosition();
            var targetPosition = targetTransform.GetPosition();
            var outputPosition = outputTransform.GetPosition();
            
            var inputGroupJoint = t1.Keys[^(offset + 1)].JointPosition;
            var targetGroupJoint = t1.Keys.Last().JointPosition;
            var outputGroupJoint = t2.Keys[offset].JointPosition;

            for (var i = 0; i <= offset; i++)
            {
                var progress = Mathf.InverseLerp(0, offset, i);
                var inputBlendingPosition = Vector3.Lerp(inputPosition, targetPosition, progress);
                var outputBlendingPosition = Vector3.Lerp(targetPosition, outputPosition, progress);
                var resultBlendingPosition = Vector3.Lerp(inputBlendingPosition, outputBlendingPosition, progress);
                
                var inputBlending = new JointTarget();
                var outputBlending = new JointTarget();
                var result = new JointTarget();
                
                for (var j = 0; j < JointTarget.LENGTH; j++)
                {
                    inputBlending[j] = Mathf.Lerp(inputGroupJoint[j], targetGroupJoint[j], progress);
                    outputBlending[j] = Mathf.Lerp(targetGroupJoint[j], outputGroupJoint[j], progress);
                    result[j] = Mathf.Lerp(inputBlending[j], outputBlending[j], progress);
                }

                var key = new TrajectoryKey(Matrix4x4.TRS(resultBlendingPosition, Quaternion.identity, Vector3.one))
                {
                    JointPosition = result,
                    IsValid = true,
                    Time = -1
                };
                
                trajectory.Add(key, sampleTime);
            }
            
            trajectory.Refresh(sampleTime);
            return trajectory;
        }
    }
}
