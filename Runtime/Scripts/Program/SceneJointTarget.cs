// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;

namespace Preliy.Flange.Planner
{
    public class SceneJointTarget : MonoBehaviour
    {
        public JointTarget Target => _jointTarget;


        [SerializeField]
        private JointTarget _jointTarget;
    }
}
