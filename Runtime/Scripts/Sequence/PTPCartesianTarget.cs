// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Preliy.Flange.Planner.Sequence
{
    // ReSharper disable once InconsistentNaming
    public class PTPCartesianTarget : MonoInstruction
    {
        public override Instruction Instruction => _instruction;
        
        public SceneCartesianTarget SceneCartesianTarget
        {
            get => _sceneCartesianTarget;
            set => _sceneCartesianTarget = value;
        }

        [Header("Instruction Parameters")]
        [SerializeField]
        private SceneCartesianTarget _sceneCartesianTarget;
        [SerializeField]
        private RawInstructions.PTPCartesianTarget _instruction = new ();

        [Header("Gizmos")]
        [SerializeField]
        private bool _showGizmos;
        [SerializeField]
        private bool _showLabel;
        [Range(0.01f, 10f)]
        [SerializeField]
        private float _gizmosScale = 1f;

        private void Reset()
        {
            _instruction = new RawInstructions.PTPCartesianTarget();
            _instruction.Initialize(transform.parent.GetComponent<Task>().Controller, transform.GetSiblingIndex());
        }

        private void OnValidate()
        {
            Refresh();
        }

        public override void Refresh()
        {
            //_instruction ??= new RawInstructions.PTPCartesianTarget();
            if (_sceneCartesianTarget != null)
            {
                _instruction.CartesianTarget = _sceneCartesianTarget.Target;
            }
            else
            {
                var target = _instruction.CartesianTarget; 
                target.Pose = transform.GetMatrix();
                _instruction.Controller.ConvertFrame(target, (int)CoordinateSystem.World, _instruction.Frame);
                _instruction.CartesianTarget = target;
            }
            
            base.Refresh();
        }
        
        protected override string GetDescription()
        {
            if (_sceneCartesianTarget != null)
            {
                return $"REF:{_sceneCartesianTarget.name} {_instruction.ToDescription()}";
            }
            return $"{_instruction.ToDescription()}";
        }

        public void JumpToTarget()
        {
            try
            {
                Refresh();
                //_instruction.Initialize(_controller, _index);
                _instruction.Plan();
                _instruction.JumpToTarget();
            }
            catch (Exception exception)
            {
                //_exception.Value = exception;
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
