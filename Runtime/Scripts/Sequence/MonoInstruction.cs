// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;

namespace Preliy.Flange.Planner.Sequence
{
    public abstract class MonoInstruction : MonoBehaviour
    {
        public abstract Instruction Instruction { get; }

        private const string FORMAT = "{0} {1}";

        public virtual void Refresh()
        {
            name = string.Format(FORMAT, Instruction.Index, GetType().Name);
        }

        protected abstract string GetDescription();
    }
}
