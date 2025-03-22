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

        private const string FORMAT = "{0} [{1}]: {2}";

        public virtual void Refresh()
        {
            name = string.Format(FORMAT, Instruction.Index, GetType().Name, GetDescription());
        }

        protected abstract string GetDescription();
    }
}
