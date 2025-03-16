// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEngine;

public class TutorialPTP : Task
{
    [SerializeField]
    private SceneCartesianTarget _target;
    [SerializeField]
    private Vector3 _corner;
    
    protected override void Create()
    {
        PTP(_target);
        PTPRel(_target, new Vector3(_corner.x, _corner.y, _corner.z), speed: 0.1f, blending: 0.15f);
        PTPRel(_target, new Vector3(_corner.x, _corner.y, -_corner.z), speed: 0.1f, blending: 0.15f);
        PTPRel(_target, new Vector3(-_corner.x, _corner.y, -_corner.z), speed: 0.1f, blending: 0.15f);
        PTPRel(_target, new Vector3(-_corner.x, _corner.y, _corner.z), speed: 0.1f, blending: 0.15f);
        PTPRel(_target, new Vector3(_corner.x, _corner.y, _corner.z), speed: 0.1f, blending: 0.15f);
        PTP(_target);
    }
}