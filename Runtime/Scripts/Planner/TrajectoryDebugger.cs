// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;

namespace Preliy.Flange.Planner
{
    public class TrajectoryDebugger : MonoBehaviour
    {
        [SerializeField]
        private Task _task;
        
        [Header("Gizmos")]
        [SerializeField]
        private TrajectoryGizmosConfig _config;
        
        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!_config.Enable) return;
            if (_task == null) return;

            // foreach (var motion in _task.Motions)
            // {
            //     motion.Trajectory?.DrawGizmos(_config);
            // }
#endif
        }
    }
}
