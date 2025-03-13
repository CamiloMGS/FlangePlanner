// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace Preliy.Flange.Planner
{
    public abstract class Ramp
    {
        public abstract List<RampKey> Keys { get; }
        public abstract float Duration { get; }

        public Ramp(float deltaTime, float initialPosition, float targetPosition, float speedLimit, float accLimit)
        {
            
        }
    }
}
