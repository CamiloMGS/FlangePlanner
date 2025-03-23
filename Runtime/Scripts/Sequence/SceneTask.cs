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

namespace Preliy.Flange.Planner.Sequence
{
    [ExecuteAlways]
    public class SceneTask : Task
    {
        [SerializeField]
        private List<MonoInstruction> _monoInstructions = new ();

        private void OnEnable()
        {
            InvokeRepeating(nameof(UpdateHierarchy), 1.0F, 0.5f);
        }
        
        private void OnDisable()
        {
            CancelInvoke(nameof(UpdateHierarchy));
        }

        // private void OnValidate()
        // {
        //     Refresh();
        // }
        
        public void Refresh()
        {
            _monoInstructions.Clear();
            _monoInstructions = GetComponentsInChildren<MonoInstruction>().ToList();
            
            for (var i = 0; i < _monoInstructions.Count; i++)
            {
                _monoInstructions[i].Instruction.Initialize(_controller, i);
            }
        }

        public void OnTransformChildrenChanged()
        {
            Refresh();
        }
        
        public override void Validate()
        {
            //throw new System.NotImplementedException();
        }
        
        public override async UniTask Plan(CancellationToken cancellationToken)
        {
            if (!gameObject.activeInHierarchy) return;
            
            try
            {
                _isValid = false;
                //Refresh();
                var instructions = _monoInstructions.Select(monoInstruction => monoInstruction.Instruction).ToList();
                await MotionPlanner.Plan(_controller, instructions, cancellationToken);
                _isValid = true;
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
        
        private async UniTask ExecuteInstructions(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            if (!_isValid) throw new Exception("Task isn't valid");
            
            foreach (var instruction in _monoInstructions.Where(instruction => instruction.Instruction.State == InstructionState.Done))
            {
                instruction.Instruction.State = InstructionState.Ready;
            }
            
            foreach (var instruction in _monoInstructions)
            {
                Logger.LogVerbose(LogType.Log, this, instruction.Instruction, "Start");
                await instruction.Instruction.Execute(playerLoopTiming, cancellationToken);
                Logger.LogVerbose(LogType.Log, this, instruction.Instruction, "End");
            }
        }
       
        private void UpdateHierarchy()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.RepaintHierarchyWindow();
#endif
        }
    }
}
