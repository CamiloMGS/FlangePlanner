// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    [Serializable]
    public class TrajectoryGizmosConfig
    {
        public bool Enable => _enable;
        public float Scale => _scale;
        public Color Color => _color;
        public bool ShowPoints => _showPoints;
        public bool ShowPointLinearSpeed => _showPointLinearSpeed;
        
        [SerializeField]
        private bool _enable;
        [Range(0.01f, 10f)]
        [SerializeField]
        private float _scale = 1f;
        [SerializeField]
        private Color _color = Color.cyan;
        [SerializeField]
        private bool _showPoints;
        [SerializeField]
        private bool _showPointLinearSpeed;
    }
}
