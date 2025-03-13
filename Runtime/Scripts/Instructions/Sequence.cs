// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    public abstract partial class Sequence : MonoBehaviour
    {
        public Controller Controller => _controller;
        public Task Task => _task;
        
        [Header("References")]
        [SerializeField]
        protected Controller _controller;
        [SerializeField]
        private Task _task;

        [Header("Settings")]
        [SerializeField]
        private bool _alwaysCompile;
        [SerializeField]
        private TaskGizmosConfig _taskGizmosConfig;

        protected abstract void Program();

        public async UniTask Compile(CancellationToken cancellationToken)
        {
            if (!gameObject.activeInHierarchy) return;

            try
            {
                _task = new Task(name, this);
                _task.Clear();
                Program();
                await MotionPlanner.Plan(_controller, _task, cancellationToken);
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, exception, this);
            }
        }

        public async UniTask Execute(PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken)
        {
            try
            {
                if (_alwaysCompile)
                {
                    await Compile(cancellationToken);
                    await _task.Execute(_controller, playerLoopTiming, cancellationToken);
                }
                else
                {
                    await _task.Execute(_controller, playerLoopTiming, cancellationToken);
                }
            }
            catch (Exception exception)
            {
                Logger.Log(LogType.Error, exception.Message, this);
                throw;
            }
        }
        
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {

            if (_task == null) return;

            DrawTaskGizmos(_task, _taskGizmosConfig);
        }
        
        public void DrawTaskGizmos(Task task, TaskGizmosConfig config)
        {
            if (!config.Enable) return;

            if (task.Motions.Count == 0) return;
            
            foreach (var motion in task.Motions)
            {
                GizmosUtils.DrawMotionTarget(_controller, motion, config.ColorPoints, config.Scale, config.ShowDescription, config.ShowBlendZone);
            }

            for (var i = 1; i < task.Motions.Count; i++)
            {
                GizmosUtils.DrawSegment(_controller, task.Motions[i-1], task.Motions[i], config.ColorLines);
            }
        }
#endif
    }
}
