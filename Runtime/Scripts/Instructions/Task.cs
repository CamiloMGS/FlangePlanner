// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    public class Task
    {
        public string Name => _name;
        public Object Context => _context;
        public List<Instruction> Instructions => _instructions;
        public List<Motion> Motions => _motions;
        public float Duration => _duration;

        public bool IsValid
        {
            get => _isValid;
            set => _isValid = value;
        }
        
        public bool Verbose
        {
            get => _verbose;
            set => _verbose = value;
        }

        public Action OnInstructionsListChanged;

        [SerializeField]
        private string _name;

        [HideInInspector]
        [SerializeField]
        private Object _context;
        
        [SerializeField]
        [SerializeReference]
        private List<Instruction> _instructions = new ();

        [SerializeField]
        private float _duration;
        
        [HideInInspector]
        [SerializeField]
        [SerializeReference]
        private List<Motion> _motions = new ();
        
        [SerializeField]
        private bool _isValid;
        [SerializeField]
        private bool _verbose;
        
        public Task(string name = "", Object context = null)
        {
            _name = name;
            _context = context;
            _isValid = false;
        }

        public void Add(Instruction instruction)
        {
            _isValid = false;
            _instructions.Add(instruction);
            Refresh();
            OnInstructionsListChanged?.Invoke();
        }
        
        public void Add(IEnumerable<Instruction> instructions)
        {
            _isValid = false;
            _instructions.AddRange(instructions);
            Refresh();
            OnInstructionsListChanged?.Invoke();
        }

        public void Clear()
        {
            _isValid = false;
            _instructions.Clear();
            _motions.Clear();
            OnInstructionsListChanged?.Invoke();
        }

        public async UniTask Execute(Controller controller, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            if (!_isValid) throw new Exception("Task isn't valid");

            foreach (var instruction in _instructions)
            {
                Logger.Log(LogType.Log, this, instruction, ActionState.Start);
                await instruction.Execute(playerLoopTiming, cancellationToken);
                Logger.Log(LogType.Log, this, instruction, ActionState.End);
            }
        }

        public void RefreshDuration()
        {
            _duration = _motions.Sum(instruction => instruction.Trajectory.Duration);
        }

        private void Refresh()
        {
            for (var i = 0; i < _instructions.Count; i++)
            {
                //_instructions[i].Index = i;
            }

            _motions = _instructions.OfType<Motion>().ToList();
        }
    }
}
