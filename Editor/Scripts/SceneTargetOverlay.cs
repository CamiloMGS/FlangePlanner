// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;

namespace Preliy.Flange.Planner.Editor
{
    [Overlay(typeof(SceneView), "Scene Robot Pose", true)]
    public class SceneTargetOverlay : Overlay, ITransientOverlay
    {
        public bool visible => Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SceneCartesianTarget>() != null;

        private Controller _controller;

        public override VisualElement CreatePanelContent()
        {
            var controllerPath = SessionState.GetString("Selected_Controller", "");
            if (!string.IsNullOrEmpty(controllerPath))
            {
                var gameObject = GameObject.Find(controllerPath);
                if (gameObject != null && gameObject.TryGetComponent<Controller>(out var controller)) _controller = controller;
            }
            
            var container = new VisualElement();

            var controllerField = new ObjectField
            {
                objectType = typeof(Controller),
                value = _controller
            };
            
            var jumpToButton = new Button(JumpToTarget)
            {
                text = "Jump To",
                tooltip = "Jump robot to target"
            };
            
            var overrideButton = new Button(OverrideTarget)
            {
                text = "Override",
                tooltip = "Override transform with actual robot pose"
            };

            container.Add(controllerField);
            container.Add(jumpToButton);
            container.Add(overrideButton);
            return container;
        }

        private void OverrideTarget()
        {
            var sceneTarget = Selection.activeGameObject.GetComponent<SceneCartesianTarget>();
            
            if (sceneTarget == null)
            {
                Logger.Log(LogType.Warning, "Pose isn't selected!");
                return;
            }
            
            if (_controller is null)
            {
                Logger.Log(LogType.Warning, "Controller isn't selected!");
                return;
            }

            Undo.RecordObject(_controller, $"Override target {sceneTarget}");
            sceneTarget.Initialize(_controller);
        }
        
        private void JumpToTarget()
        {
            var sceneTarget = Selection.activeGameObject.GetComponent<SceneCartesianTarget>();
            
            if (sceneTarget == null)
            {
                Logger.Log(LogType.Warning, "Pose isn't selected!");
                return;
            }
            
            if (_controller is null)
            {
                Logger.Log(LogType.Warning, "Controller isn't selected!");
                return;
            }

            
            sceneTarget.JumpTo(_controller, SolutionIgnoreMask.None);
        }
    }
}
