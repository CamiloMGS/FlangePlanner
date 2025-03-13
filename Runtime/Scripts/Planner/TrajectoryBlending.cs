// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Preliy.Flange.Planner;
using UnityEngine;
using Motion = Preliy.Flange.Planner.Instructions.Motion;

namespace Preliy.Flange
{
    public static class TrajectoryBlending
    {
        public static void Blend(this Controller controller, Motion m1, Motion m2)
        {
            var first = m1.Trajectory;
            var second = m2.Trajectory;
            var radius = m2.Blending;

            if (radius <= 0) return;
            if (first == null) throw new NullReferenceException("First trajectory is null");
            if (second == null) throw new NullReferenceException("Second trajectory is null");
            if (!first.IsValid) throw new Exception("First trajectory isn't valid");
            if (!second.IsValid) throw new Exception("Second trajectory isn't valid");
            if (first.Keys.Count < 2) throw new Exception("First trajectory keys count is to small");
            if (second.Keys.Count < 2) throw new Exception("Second trajectory keys count is to small");
            //if (!Math.IsEqual(first.Keys.Last().CartesianPose, second.Keys.First().CartesianPose)) throw new Exception("Blend waypoint is not valid");

            var wayPoint = first.Keys.Last().CartesianPose;

            var firstIntersectionIndex = GetIntersectionIndex(first, wayPoint.GetPosition(), radius, true);
            var secondIntersectionIndex = GetIntersectionIndex(second, wayPoint.GetPosition(), radius);
            
            if (firstIntersectionIndex < 0) throw new Exception("Intersection index isn't found in first trajectory");
            if (secondIntersectionIndex < 0) throw new Exception("Intersection point isn't found in second trajectory");

            var speedFactor = (m1.Speed + m2.Speed) * 0.5f;
            
            var blendTrajectory = CreateBlendTrajectory(controller, first.Keys[firstIntersectionIndex], first.Keys.Last(), second.Keys[secondIntersectionIndex], speedFactor);
            
            first.RemoveRange(firstIntersectionIndex, first.Keys.Count - firstIntersectionIndex, controller.SampleTime);
            second.RemoveRange(0, secondIntersectionIndex + 1, controller.SampleTime);
            
            first.Add(blendTrajectory, controller.SampleTime);
        }

        private static int GetIntersectionIndex(Trajectory trajectory, Vector3 wayPoint, float radius, bool reverse = false)
        {
            if (reverse)
            {
                for (var i = trajectory.Keys.Count - 1; i > -1; i--)
                {
                    if (Vector3.Distance(wayPoint, trajectory.Keys[i].CartesianPose.GetPosition()) > radius)
                    {
                        return i + 1;
                    }
                }
                
            }
            else
            {
                for (var i = 0; i < trajectory.Keys.Count; i++)
                {
                    if (Vector3.Distance(wayPoint, trajectory.Keys[i].CartesianPose.GetPosition()) > radius)
                    {
                        return i - 1;
                    }
                }
            }
            
            return -1;
        }
        
        private static Trajectory CreateBlendTrajectory(Controller controller, TrajectoryKey start, TrajectoryKey waypoint, TrajectoryKey end, float speedFactor)
        {
            var trajectory = new Trajectory();

            var startJointGroup = start.JointPosition;
            var waypointJointGroup = waypoint.JointPosition;
            var endJointGroup = end.JointPosition;
            
            var startTransform = start.CartesianPose;
            var waypointTransform = waypoint.CartesianPose;
            var endTransform = end.CartesianPose;

            var inputPosition = startTransform.GetPosition();
            var targetPosition = waypointTransform.GetPosition();
            var outputPosition = endTransform.GetPosition();
            
            var arc = Math.Get3PointSplineLength(inputPosition, outputPosition, targetPosition);

            var rampsConfigs = new List<RampConfig>();
            var cartesianConfig = RampExtension.GetConfigs(
                controller.MechanicalGroup,
                speedFactor,
                startTransform,
                endTransform,
                arc,
                start.CartesianLimit,
                end.CartesianLimit
            );

            var externalJointsConfig = RampExtension.GetConfigs(
                controller.MechanicalGroup,
                speedFactor,
                startJointGroup.ExtJoint,
                endJointGroup.ExtJoint,
                start.JointSpeed.ExtJoint,
                end.JointSpeed.ExtJoint
            );

            rampsConfigs = cartesianConfig.Concat(externalJointsConfig).ToList();

            var ramp = rampsConfigs.GetRamps(controller.SampleTime).GetMaxDuration();

            foreach (var key in ramp.Keys)
            {
                var inputBlending = Math.Lerp(startJointGroup.ExtJoint, waypointJointGroup.ExtJoint, key.Progress);
                var outputBlending = Math.Lerp(waypointJointGroup.ExtJoint, endJointGroup.ExtJoint, key.Progress);
                var externalJoints = Math.Lerp(inputBlending, outputBlending, key.Progress);
                
                var inputBlendingPosition = Vector3.Lerp(inputPosition, targetPosition, key.Progress);
                var outputBlendingPosition = Vector3.Lerp(targetPosition, outputPosition, key.Progress);
                var resultBlendingPosition = Vector3.Lerp(inputBlendingPosition, outputBlendingPosition, key.Progress);
                
                var inputBlendingRotation = Quaternion.Slerp(startTransform.rotation, waypointTransform.rotation, key.Progress);
                var outputBlendingRotation = Quaternion.Slerp(waypointTransform.rotation, endTransform.rotation, key.Progress);
                var resultBlendingRotation = Quaternion.Slerp(inputBlendingRotation, outputBlendingRotation, key.Progress);
                
                var target = Matrix4x4.TRS(resultBlendingPosition, resultBlendingRotation, Vector3.one);
                var robotTarget = new CartesianTarget(target, controller.Configuration.Value, externalJoints);
                var solution = controller.Solver.ComputeInverse(robotTarget, controller.Tool.Value, (int)CoordinateSystem.World);
                
                var trajectoryKey = new TrajectoryKey
                {
                    CartesianPose = target,
                    JointPosition = solution.JointTarget,
                    IsValid = solution.IsValid
                };
                
                trajectory.Add(trajectoryKey, controller.SampleTime);
            }

            return trajectory;
        }
    }
}
