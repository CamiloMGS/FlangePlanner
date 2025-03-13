// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Preliy.Flange;
using Preliy.Flange.Planner;
using Preliy.Flange.Planner.Instructions;
using UnityEngine;

public class TutorialLinearUnit : Sequence
{
    [SerializeField]
    private CartesianTarget _home;
    
    [SerializeField]
    private List<SceneCartesianTarget> _targets;
    
    protected override void Program()
    {
        PTP(_home, speed: 1, blending: 0.0f);

        foreach (var target in _targets)
        {
            LIN(target, speed: 0.4f, blending: .2f);
        }
    }
}