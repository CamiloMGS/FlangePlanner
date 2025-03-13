// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    public class MotionSequence
    {
        public readonly List<Motion> Motions = new ();

        public void BlendTrajectories(Controller controller)
        {
            for (var i = 0; i < Motions.Count - 1; i++)
            {
                AdjustBlending(controller, Motions[i], Motions[i + 1]);
            }
            
            for (var i = 1; i < Motions.Count - 1; i++)
            {
                controller.Blend(Motions[i], Motions[i + 1]);
            }
        }
        
        private static void AdjustBlending(Controller controller, Motion motion, Motion next)
        {
            var distance = Vector3.Distance(motion.GetTargetWorld(controller).GetPosition(), next.GetTargetWorld(controller).GetPosition());
            var limitMax = distance * 0.5f;

            if (motion.Blending > 0)
            {
                if (next.Index - motion.Index > 1)
                {
                    motion.Blending = 0;
                    Logger.Log(LogType.Warning, $"{motion.Name}: Motion queue is interrupted by another command. Blending set to 0!");
                }
                else
                {
                    if (motion.Blending > limitMax)
                    {
                        motion.Blending = limitMax;
                        Logger.Log(LogType.Warning, $"{motion.Name}: Blending value {motion.Blending} is limited to {limitMax}");
                    }
                }
            }

            if (next.Blending > limitMax)
            {
                next.Blending = limitMax;
                Logger.Log(LogType.Warning, $"{next.Name}: Blending value {next.Blending} is limited to {limitMax}");
            }
        }
    }
}
