// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEngine;

public class TutorialCIRC : ScriptableTask
{
    [SerializeField]
    private SceneCartesianTarget _start;
    [SerializeField]
    private SceneCartesianTarget _target;
    [SerializeField]
    private SceneCartesianTarget _waypoint;
    
    protected override void Create()
    {
        PTP(_start);
        CIRC(_target, _waypoint, 0.1f);
        PTP(_start);
    }
}