// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Preliy.Flange.Planner.RawInstructions;
using Motion = Preliy.Flange.Planner.RawInstructions.Motion;

namespace Preliy.Flange.Planner
{
    public static class MotionPlanner
    {
        public static async UniTask Plan(Controller controller, List<Instruction> instructions, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.SwitchToThreadPool();
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                var motions = instructions.OfType<Motion>().ToList();
                
                if (instructions == null)
                {
                    throw new NullReferenceException("Instruction list is null");
                }
            
                if (instructions.Count == 0)
                {
                    throw new Exception("Instruction list is empty");
                }


                for (var i = 0; i < instructions.Count; i++)
                {
                    instructions[i].Initialize(controller, i);
                    instructions[i].Plan();
                }
                
                InitializeMotions(motions);
                CreateTrajectory(controller, motions);
                BlendTrajectory(controller, motions);
            }
            finally
            {
                await UniTask.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();
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
