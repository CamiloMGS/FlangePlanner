// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    // ReSharper disable once InconsistentNaming
    public class PTPJointTarget : JointMotion
    {
        public override void Plan()
        {
            try
            {
                base.Plan();
                //TODO Validate Joint target
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
            return controller.Solver.ComputeForward(_jointTarget);
        }
        
        public override JointTarget GetJointTarget(Controller controller)
        {
            return _jointTarget;
        }

        public override void JumpToTarget()
        {
            throw new NotImplementedException();
        }
        
        public override string ToString()
        {
            return string.Format(FORMAT, _index, "PTP");
        }
        
        protected const string FORMAT_DESCRIPTION = 
            "R: {0}\n  " +
            "E: {1}\n " +
            "S: {2}\n " +
            "B: {3}";
        
        public override string ToDescription()
        {
            return string.Format(FORMAT_DESCRIPTION, _jointTarget.RobJoint, _jointTarget.ExtJoint, _speed, _blending);
        }
    }
}

