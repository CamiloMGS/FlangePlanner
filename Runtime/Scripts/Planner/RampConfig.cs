// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Preliy.Flange.Planner
{
    public record RampConfig
    {
        public float P0 { get; set; }
        public float P1 { get; set; }
        public float V0 { get; set; }
        public float V1 { get; set; }
        public float SpeedMax { get; set; }
        public float AccMax { get; set; }

        public void Scale(float factor)
        {
            // V0 *= factor;
            // V1 *= factor;
            SpeedMax *= factor;
            AccMax *= factor;
        }
    }
}
