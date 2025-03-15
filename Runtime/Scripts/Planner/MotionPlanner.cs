// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Preliy.Flange.Planner.RawInstructions;
using Motion = Preliy.Flange.Planner.RawInstructions.Motion;

namespace Preliy.Flange.Planner
{
    public static class MotionPlanner
    {
        public static async UniTask Plan(Task task, CancellationToken cancellationToken = default)
        {
            task.IsValid = false;
            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.SwitchToThreadPool();

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Plan(task.Instructions);
                InitializeMotions(task.Motions);
                CreateTrajectory(task.Controller, task.Motions);
                BlendTrajectory(task.Controller, task.Motions);
            }
            finally
            {
                await UniTask.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();
            task.IsValid = true;
        }
        
        private static void BlendTrajectory(Controller controller, IReadOnlyList<Motion> motions)
        {
            var sequences = new List<MotionSequence>();
            var actualSequence = new MotionSequence();
            actualSequence.Motions.Add(motions[0]);
            sequences.Add(actualSequence);
            
            for (var i = 1; i < motions.Count; i++)
            {
                if (motions[i].Index == motions[i-1].Index + 1)
                {
                    actualSequence.Motions.Add(motions[i]);
                }
                else
                {
                    actualSequence = new MotionSequence();
                    actualSequence.Motions.Add(motions[i]);
                    sequences.Add(actualSequence);
                }
            }

            foreach (var sequence in sequences)
            {
                sequence.BlendTrajectories(controller);
            }
        }

        private static void Plan(IList<Instruction> instructions)
        {
            if (instructions == null)
            {
                throw new NullReferenceException("Instruction list is null");
            }
            
            if (instructions.Count == 0)
            {
                throw new Exception("Instruction list is empty");
            }

            foreach (var instruction in instructions)
            {
                instruction.Plan();
            }
        }
        
        private static void InitializeMotions(IReadOnlyList<Motion> motions)
        {
            if (motions.Count <= 0) return;
            if (motions[0] is not JointMotion)
            {
                throw new Exception("First motion instruction type must be PTP");
            }
            motions[0].IsTempJob = true;
        }
        
        private static void CreateTrajectory(Controller controller, IReadOnlyList<Motion> motions)
        {
            for (var i = 1; i < motions.Count; i++)
            {
                motions[i].CreateTrajectory(motions[i - 1].GetJointTarget(controller));
            }
        }
    }
}
