// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;

namespace Preliy.Flange.Planner.Sequence
{
    public abstract class MonoInstruction : MonoBehaviour, IInstructionProvider
    {
        public abstract Instruction Instruction { get; }

        public Controller Controller
        {
            get => _controller;
            set => _controller = value;
        }
        
        public int Index
        {
            get => _index;
            set => _index = value;
        }

        public Property<Exception> Exception => _exception;

        [SerializeField]
        protected Controller _controller;
        [SerializeField]
        protected int _index;
        [SerializeField]
        protected Property<Exception> _exception = new ();

        private const string FORMAT = "{0} [{1}]: {2}";

        public abstract void Initialize();
        
        public void OnValidate()
        {
            name = string.Format(FORMAT, _index, GetType().Name, GetDescription());
        }

        protected abstract string GetDescription();
    }
}
