// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    public static class RampExtension
    {
        public static List<RampTrapezoidal> GetRamps(this IEnumerable<RampConfig> configs, float deltaTime)
        {
            return configs.Select(config => new RampTrapezoidal(deltaTime, config)).ToList();
        }
        
        public static RampTrapezoidal GetMaxDuration(this List<RampTrapezoidal> ramps)
        {
            return ramps.Count == 0 ? new RampTrapezoidal() : ramps.MaxBy(ramp => ramp.Duration);
        }
        
        public static IEnumerable<RampConfig> GetConfigs(MechanicalGroup mechanicalGroup, float speedFactor, JointTarget from, JointTarget to)
        {
            var configs = new List<RampConfig>();
            configs.AddRange(GetConfigs(mechanicalGroup, speedFactor, from.RobJoint, to.RobJoint));
            configs.AddRange(GetConfigs(mechanicalGroup, speedFactor, from.ExtJoint, to.ExtJoint));
            return configs;
        }
        
        public static IEnumerable<RampConfig> GetConfigs(MechanicalGroup mechanicalGroup, float speedFactor, Matrix4x4 m0, Matrix4x4 m1, ExtJoint ext0, ExtJoint ext1, float distance = 0f)
        {
            IEnumerable<RampConfig> cartesianRamps = GetConfigs(mechanicalGroup, speedFactor, m0, m1, distance);
            IEnumerable<RampConfig> externalJointsRamps = GetConfigs(mechanicalGroup, speedFactor, ext0, ext1);
            return cartesianRamps.Concat(externalJointsRamps).ToList();
        }

        public static IEnumerable<RampConfig> GetConfigs(MechanicalGroup mechanicalGroup, float speedFactor, Matrix4x4 m0, Matrix4x4 m1, float distance = 0f, CartesianLimit l0 = null, CartesianLimit l1 = null)
        {
            var cartesianLimit = mechanicalGroup.Robot.CartesianLimit;
            
            var configs = new List<RampConfig>();

            var cartesianPositionConfig = new RampConfig
            {
                P0 = 0,
                P1 = distance > 0 ? distance : Vector3.Distance(m0.GetPosition(), m1.GetPosition()),
                V0 = l0?.LinearSpeed ?? 0,
                V1 = l1?.LinearSpeed ?? 0,
                SpeedMax = cartesianLimit.LinearSpeed,
                AccMax = cartesianLimit.LinearAcc
            };

            var cartesianRotationConfig = new RampConfig()
            {
                P0 = 0,
                P1 = Quaternion.Angle(m0.rotation, m1.rotation),
                V0 = l0?.RotationSpeed ?? 0,
                V1 = l1?.RotationSpeed ?? 0,
                SpeedMax = cartesianLimit.RotationSpeed,
                AccMax = cartesianLimit.RotationAcc
            };
            
            cartesianPositionConfig.Scale(speedFactor);
            cartesianRotationConfig.Scale(speedFactor);
            
            configs.Add(cartesianPositionConfig);
            configs.Add(cartesianRotationConfig);

            return configs;
        }

        public static IEnumerable<RampConfig> GetConfigs(MechanicalGroup mechanicalGroup, float speedFactor, RobJoint p0, RobJoint p1, RobJoint v0 = null, RobJoint v1 = null)
        {
            var configs = new List<RampConfig>();

            for (var i = 0; i < mechanicalGroup.RobotJoints.Count; i++)
            {
                var config = new RampConfig
                {
                    P0 = p0[i],
                    P1 = p1[i],
                    V0 = v0 == null ? 0 : v0[i],
                    V1 = v1 == null ? 0 : v1[i],
                    SpeedMax = mechanicalGroup.RobotJoints[i].Config.SpeedMax,
                    AccMax = mechanicalGroup.RobotJoints[i].Config.AccMax
                };
                
                config.Scale(speedFactor);
                configs.Add(config);
            }

            return configs;
        }
        
        public static IEnumerable<RampConfig> GetConfigs(MechanicalGroup mechanicalGroup, float speedFactor, ExtJoint p0, ExtJoint p1, ExtJoint v0 = null, ExtJoint v1 = null)
        {
            var configs = new List<RampConfig>();

            for (var i = 0; i < mechanicalGroup.ExternalJoints.Count; i++)
            {
                var config = new RampConfig
                {
                    P0 = p0[i],
                    P1 = p1[i],
                    V0 = v0 == null ? 0 : v0[i],
                    V1 = v1 == null ? 0 : v1[i],
                    SpeedMax = mechanicalGroup.ExternalJoints[i].Config.SpeedMax,
                    AccMax = mechanicalGroup.ExternalJoints[i].Config.AccMax
                };
                
                config.Scale(speedFactor);
                configs.Add(config);
            }

            return configs;
        }
    }
}
