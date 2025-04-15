// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Preliy.Flange.Planner.Sequence
{
    public class SceneSequence : Task
    {
        protected override void Create()
        {
            foreach (var monoInstruction in _monoInstructions)
            {
                monoInstruction.Initialize();
                Add(monoInstruction.Instruction);
            }
        }

        [SerializeField]
        [SerializeReference]
        private List<MonoInstruction> _monoInstructions = new();
        private void Reset()
        {
            Refresh();
        }

        private void OnValidate()
        {
            Refresh();
        }

        public void Refresh()
        {
            _monoInstructions.Clear();
            _monoInstructions = GetComponentsInChildren<MonoInstruction>().ToList();

            for (var i = 0; i < _monoInstructions.Count; i++)
            {
                _monoInstructions[i].Controller = _controller;
                _monoInstructions[i].Index = i;
                _monoInstructions[i].OnValidate();
            }
        }

        public void OnTransformChildrenChanged()
        {
            Debug.Log("Hello");
            Refresh();
        }

        public void AddMonoInstruction(MonoInstruction monoInstruction)
        {
            _monoInstructions.Add(monoInstruction);
        }
    }
}
