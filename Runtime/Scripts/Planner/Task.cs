// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    [Serializable]
    public abstract class Task : MonoBehaviour
    {
        public Controller Controller => _controller;
        
        public bool IsValid => _isValid;
        
        public bool Verbose
        {
            get => _verbose;
            set => _verbose = value;
        }

        public Action OnInstructionsListChanged;

        [Header("State")]
        [SerializeField]
        protected bool _isValid;
        
        [Header("References")]
        [SerializeField]
        protected Controller _controller;

        [Header("Settings")]
        [Tooltip("Compile by execution")]
        [SerializeField]
        protected bool _alwaysCompile;
        [SerializeField]
        protected bool _verbose;
        [SerializeField]
        protected TaskGizmosConfig _taskGizmosConfig;


        public abstract void Validate();
        public abstract UniTask Plan(CancellationToken cancellationToken);

        public abstract UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken);
    }
}
