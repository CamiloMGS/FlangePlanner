// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    //[CustomPropertyDrawer(typeof(Preliy.Flange.JointTarget))]
    public class JointTarget : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var robotJointFields = new List<FloatField>();
            var extJointFields = new List<FloatField>();

            var robotJointsProperty = property.FindPropertyRelative("_robJoint");
            var extJointsProperty = property.FindPropertyRelative("_extJoint");

            for (var i = 0; i < 6; i++)
            {
                var jointProperty = robotJointsProperty.FindPropertyRelative($"_r{i + 1}");
                var jointField = new FloatField($"R{i + 1}");
                jointField.BindProperty(jointProperty);
                robotJointFields.Add(jointField);
            }
            for (var i = 0; i < 6; i++)
            {
                var jointProperty = extJointsProperty.FindPropertyRelative($"_e{i + 1}");
                var jointField = new FloatField($"E{i + 1}");
                jointField.BindProperty(jointProperty);
                extJointFields.Add(jointField);
            }

            foreach (var jointField in robotJointFields)
            {
                container.Add(jointField);
            }
            
            foreach (var jointField in extJointFields)
            {
                container.Add(jointField);
            }

            return container;
        }
    }
}
