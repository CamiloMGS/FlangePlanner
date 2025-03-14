// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEngine;

public class TutorialLIN : Program
{
    [SerializeField]
    private SceneCartesianTarget _target;
    
    protected override void Sequence()
    {
        PTP(_target);
        PTPRel(_target, new Vector3(0.3f, 0.2f, 0.3f));
        LINRel(_target, new Vector3(0.3f, 0.2f, -0.3f), speed: 0.3f, blending: 0.1f);
        LINRel(_target, new Vector3(-0.3f, 0.2f, -0.3f), speed: 0.3f, blending: 0.1f);
        LINRel(_target, new Vector3(-0.3f, 0.2f, 0.3f), speed: 0.3f, blending: 0.1f);
        LINRel(_target, new Vector3(0.3f, 0.2f, 0.3f), speed: 0.3f, blending: 0.1f);
        PTP(_target);
    }
}