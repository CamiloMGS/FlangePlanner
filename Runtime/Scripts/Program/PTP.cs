// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.Instructions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Preliy.Flange.Planner
{
    // ReSharper disable once InconsistentNaming
    public class PTP : MonoInstruction
    {
        public override Instruction Instruction => _jointMotion;
        
        public SceneCartesianTarget SceneCartesianTarget
        {
            get => _sceneCartesianTarget;
            set => _sceneCartesianTarget = value;
        }
        
        public CartesianTarget CartesianTarget
        {
            get => _cartesianTarget;
            set => _cartesianTarget = value;
        }
        
        public int Frame
        {
            get => _frame;
            set => _frame = value;
        }

        [SerializeField]
        private SceneCartesianTarget _sceneCartesianTarget;
        [SerializeField]
        private CartesianTarget _cartesianTarget = CartesianTarget.Default;
        [SerializeField]
        private int _tool;
        [SerializeField]
        private int _frame;
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

        [SerializeField]
        private JointMotion _jointMotion = new PTPRobotTarget();

        private void Refresh()
        {
            _exception.Value = null;
            _cartesianTarget.Pose = transform.GetMatrix();
            _jointMotion = new PTPRobotTarget
            {
                CartesianTarget = _sceneCartesianTarget != null ? _sceneCartesianTarget.Target : _cartesianTarget,
                Tool = _tool,
                Frame = _frame,
                Speed = _speed,
                Blending = _blending
            };
        }
        
        protected override string GetDescription()
        {
            if (_sceneCartesianTarget != null)
            {
                return $"REF:{_sceneCartesianTarget.name} T:{_tool} F:{_frame} S:{_speed} B:{_blending}";
            }
            return $"T:{_tool} F:{_frame} S:{_speed} B:{_blending}";
        }

        public void JumpToTarget()
        {
            try
            {
                Refresh();
                _jointMotion.Plan(_controller, _index);
                _jointMotion.JumpToTarget();
            }
            catch (Exception exception)
            {
                _exception.Value = exception;
                Flange.Logger.Log(LogType.Error, $"{name}: {exception}", this);
#if UNITY_EDITOR
                if (!Application.isPlaying) EditorUtility.SetDirty(this);
#endif
            }
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
