// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;

namespace Preliy.Flange.Planner.Sequence
{
    [ExecuteInEditMode]
    public class Sequence : MonoBehaviour
    {
        public Task Task => _task;
        
        [SerializeField]
        protected Controller _controller;
        [SerializeField]
        private Task _task;
        
        [SerializeField]
        [SerializeReference]
        private List<MonoInstruction> _instructions = new ();

        private void Reset()
        {
           
        }

        private void OnValidate()
        {
            Refresh();
        }

        public void Refresh()
        {
            _instructions.Clear();
            _instructions = GetComponentsInChildren<MonoInstruction>().ToList();
            
            for (var i = 0; i < _instructions.Count; i++)
            {
                _instructions[i].Controller = _controller;
                _instructions[i].Index = i;
                _instructions[i].OnValidate();
            }
        }

        public void OnTransformChildrenChanged()
        {
            Refresh();
        }
        
        
    }
}
