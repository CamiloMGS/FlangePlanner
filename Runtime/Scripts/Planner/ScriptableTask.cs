// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Preliy.Flange.Planner.RawInstructions;
using UnityEngine;
using Motion = Preliy.Flange.Planner.RawInstructions.Motion;

namespace Preliy.Flange.Planner
{
    public abstract partial class ScriptableTask : Task
    {
        [Header("Sequence")]
        [SerializeField]
        [SerializeReference]
        private List<Instruction> _instructions = new ();
        
        [HideInInspector]
        [SerializeField]
        [SerializeReference]
        private List<Motion> _motions = new ();
        
        protected abstract void Create();

        public override void Validate()
        {
            
        }
        
        public override async UniTask Plan(CancellationToken cancellationToken)
        {
            if (!gameObject.activeInHierarchy) return;

            try
            {
                Clear();
                Create();
                await MotionPlanner.Plan(_controller, _instructions, cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, exception, this);
            }
        }
        
        public override async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            if (!gameObject.activeInHierarchy) return;

            try
            {
                if (_alwaysCompile)
                {
                    await Plan(cancellationToken);
                    await ExecuteInstructions(playerLoopTiming, cancellationToken);
                }
                else
                {
                    await ExecuteInstructions(playerLoopTiming, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, exception.Message, this);
                throw;
            }
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
        
        private async UniTask ExecuteInstructions(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            if (!_isValid) throw new Exception("Task isn't valid");

            foreach (var instruction in _instructions)
            {
                Logger.LogVerbose(LogType.Log, this, instruction, ActionState.Start);
                await instruction.Execute(playerLoopTiming, cancellationToken);
                Logger.LogVerbose(LogType.Log, this, instruction, ActionState.End);
            }
        }
        
        private void Refresh()
        {
            for (var i = 0; i < _instructions.Count; i++)
            {
                _instructions[i].Initialize(_controller, i);
            }

            _motions = _instructions.OfType<Motion>().ToList();
        }
        
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            DrawTaskGizmos(_taskGizmosConfig);
        }
        
        public void DrawTaskGizmos(TaskGizmosConfig config)
        {
            if (!config.Enable) return;

            if (_motions.Count == 0) return;
            
            foreach (var motion in _motions)
            {
                GizmosUtils.DrawMotionTarget(_controller, motion, config.ColorPoints, config.Scale, config.ShowDescription, config.ShowBlendZone);
            }

            for (var i = 1; i < _motions.Count; i++)
            {
                GizmosUtils.DrawSegment(_controller, _motions[i-1], _motions[i], config.ColorLines);
            }
        }
#endif
    }
}
