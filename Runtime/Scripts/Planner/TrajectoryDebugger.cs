// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.Instructions;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    public class TrajectoryDebugger : MonoBehaviour
    {
        [SerializeField]
        private Sequence _sequence;
        
        [Header("Gizmos")]
        [SerializeField]
        private TrajectoryGizmosConfig _config;
        
        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (!_config.Enable) return;
            if (_sequence == null) return;

            foreach (var motion in _sequence.Task.Motions)
            {
                motion.Trajectory?.DrawGizmos(_config);
            }
#endif
        }
    }
}
