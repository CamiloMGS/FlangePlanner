// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.RawInstructions
{
    [Serializable]
    public abstract class Target
    {
        public JointTarget ExternalJoints
        {
            get => _externalJoints;
            set => _externalJoints = value;
        }

        public int ToolIndex
        {
            get => _toolIndex;
            set => _toolIndex = value;
        }

        public float Override 
        {
            get => _override;
            set => _override = value;
        }
        
        public float Blending => _blending;
        
        public UnityEngine.Object SceneReference
        {
            get => _sceneReference;
            set => _sceneReference = value;
        }

        [SerializeField]
        protected JointTarget _externalJoints;
        [SerializeField]
        protected int _toolIndex;
        [SerializeField]
        protected float _override = 1f;
        [SerializeField]
        protected float _blending = -1;
        [SerializeField]
        protected UnityEngine.Object _sceneReference;

        protected Target() {}

        protected Target(Target target)
        {
            _toolIndex = target.ToolIndex;
            _externalJoints = target.ExternalJoints;
            _override = target.Override;
            _blending = target.Blending;
            _sceneReference = target.SceneReference;
        }

        public abstract Matrix4x4 GetWorldTransform(Controller controller, bool useJointState = false);
        
        public abstract void Validate(Instruction instruction);

        public Target SetOverride(float @override)
        {
            _override = @override;
            return this;
        }
    }
}
