// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace Preliy.Flange.Planner
{
    [Serializable]
    public struct RampKey
    {
        public readonly float Time;
        public readonly float Position;
        public readonly float Speed;
        public readonly float Acc;
        public readonly float Progress;

        public RampKey(float position, float speed, float acc, float time, float progress)
        {
            Time = time;
            Position = position;
            Speed = speed;
            Acc = acc;
            Progress = progress;
        }
    }
}
