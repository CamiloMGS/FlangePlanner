// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Preliy.Flange.Planner;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEngine;

public class TutorialExternalUnit : ScriptableTask
{
    [SerializeField]
    private SceneCartesianTarget _home;
    
    [SerializeField]
    private List<SceneCartesianTarget> _targets;
    
    protected override void Create()
    {
        PTP(_home, speed: 1, blending: 0.4f, tool: 1);

        foreach (var target in _targets)
        {
            LIN(target, speed: 0.6f, tool: 1, frame: 1);
        }
    }
}