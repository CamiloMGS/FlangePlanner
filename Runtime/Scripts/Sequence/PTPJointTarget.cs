// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Preliy.Flange.Planner.Sequence
{
    // ReSharper disable once InconsistentNaming
    public class PTPJointTarget : MonoInstruction
    {
        public override Instruction Instruction => _jointMotion;

        public SceneJointTarget SceneJointTarget
        {
            get => _sceneJointTarget;
            set => _sceneJointTarget = value;
        }
        
        public JointTarget JointTarget
        {
            get => _jointTarget;
            set => _jointTarget = value;
        }

        [SerializeField]
        private SceneJointTarget _sceneJointTarget;
        [SerializeField]
        private JointTarget _jointTarget = JointTarget.Default;

        [SerializeField]
        private float _speed;
        [SerializeField]
        private float _blending;

        [Header("Gizmos")]
        [SerializeField]
        private bool _showGizmos;
        [SerializeField]
        private bool _showLabel;
        [Range(0.01f, 10f)]
        [SerializeField]
        private float _gizmosScale = 1f;

        private JointMotion _jointMotion;

        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }
        protected override string GetDescription()
        {
            if (_sceneJointTarget != null)
            {
                return $"REF:{_sceneJointTarget.name} S:{_speed} B:{_blending}";
            }
            return  $"T:{_jointTarget.RobJoint} S:{_speed} B:{_blending}";
        }

        public void Jump()
        {
            //TODO
        }
        
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR            
            if (!_showGizmos) return;
            Gizmos.DrawSphere(transform.position, 0.01f * _gizmosScale);
            if (_showLabel) Handles.Label(transform.position, transform.name);
            Flange.GizmosUtils.DrawHandle(transform.GetMatrix(), _gizmosScale);
#endif
        }
        
    }
}
