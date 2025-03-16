// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.RawInstructions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    [CustomPropertyDrawer(typeof(InspectorPTP))]
    public class PropertyDrawerPtp : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var targetTypeField = new PropertyField(property.FindPropertyRelative("_targetType"));
            var sceneJointTargetField = new PropertyField(property.FindPropertyRelative("_sceneJointTarget"));
            var jointTargetField = new PropertyField(property.FindPropertyRelative("_jointTarget"));
            
            var sceneCartesianTargetField = new PropertyField(property.FindPropertyRelative("_sceneCartesianTarget"));
            var cartesianTargetField = new PropertyField(property.FindPropertyRelative("_cartesianTarget"));
            var frameField = new PropertyField(property.FindPropertyRelative("_frame"));
            
            container.Add(targetTypeField);
            container.Add(sceneJointTargetField);
            container.Add(jointTargetField);
            container.Add(sceneCartesianTargetField);
            container.Add(cartesianTargetField);
            container.Add(frameField);
            
            targetTypeField.RegisterValueChangeCallback(TargetTypeFieldCallback);
            
            Debug.Log("draw");

            return container;
        }
        
        
        
        private void TargetTypeFieldCallback(SerializedPropertyChangeEvent evt)
        {
            //var targetType = (PTP.TargetType)evt.changedProperty.enumValueIndex;
            //evt.changedProperty.enumValueIndex
            
            //Debug.Log(targetType);
        }
    }
}
