// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;

namespace Preliy.Flange.Planner
{
    public record TrajectoryKey
    {
        public JointTarget JointPosition { get; set; } = JointTarget.Default;
        public JointTarget JointSpeed { get; set; } = JointTarget.Default;
        
        /// <summary>
        /// Cartesian pose in world space
        /// </summary>
        public Matrix4x4 CartesianPose { get; set; }
        public CartesianLimit CartesianLimit { get; set; } = CartesianLimit.Default;
        public float Time { get; set; }
        public bool IsValid { get; set; }

        public TrajectoryKey(bool isValid = false)
        {
            IsValid = isValid;
        }

        /// <summary>
        /// Trajectory key
        /// </summary>
        /// <param name="cartesianPose">Pose in World Space</param>
        /// <param name="cartesianLimit">Cartesian limits</param>
        /// <param name="isValid">Is valid flag</param>
        public TrajectoryKey(Matrix4x4 cartesianPose, CartesianLimit cartesianLimit = null, bool isValid = false)
        {
            CartesianPose = cartesianPose;
            CartesianLimit = cartesianLimit;
            IsValid = isValid;
        }
        
        /// <summary>
        /// Trajectory key
        /// </summary>
        /// <param name="jointPosition">Joint position values</param>
        /// <param name="jointSpeed">Joint speed values</param>
        public TrajectoryKey(JointTarget jointPosition, JointTarget jointSpeed = null)
        {
            JointPosition = jointPosition;
            JointSpeed = jointSpeed;
        }
    }
}
