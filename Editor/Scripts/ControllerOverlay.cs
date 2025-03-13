// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Search;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    [Overlay(typeof(SceneView), "Controller", true)]
    public partial class ControllerOverlay : Overlay, ITransientOverlay
    {
        public bool visible => Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Controller>() != null;

        private Controller _controller;
        private Toggle _showHandlesToggle;

        public override VisualElement CreatePanelContent()
        {
            if (Selection.activeGameObject == null)
            {
                displayed = false;
                return new VisualElement();
            }

            _controller = Selection.activeGameObject.GetComponent<Controller>();
            if (_controller == null)
            {
                displayed = false;
                return new VisualElement();
            }

            SessionState.SetString("Selected_Controller", SearchUtils.GetHierarchyPath(_controller.gameObject, false));
            
            var container = new VisualElement();

            _showHandlesToggle = new Toggle("Show Handles")
            {
                value = EditorPrefs.GetBool("ControllerShowHandles")
            };
            _showHandlesToggle.RegisterValueChangedCallback(OnShowHandleChangeEvent);
            
            var createTargetButton = new Button(CreateTarget)
            {
                text = "Create Pose",
                tooltip = "Create Robot Pose"
            };

            container.Add(_showHandlesToggle);
            container.Add(createTargetButton);
            return container;
        }

        public override void OnWillBeDestroyed()
        {
            base.OnWillBeDestroyed();
            _showHandlesToggle.UnregisterValueChangedCallback(OnShowHandleChangeEvent);
        }

        private void CreateTarget()
        {
            //TODO
            //if (_controller != null) _controller.CreateSceneTarget();
        }
        
        private static void OnShowHandleChangeEvent(ChangeEvent<bool> evt)
        {
            EditorPrefs.SetBool("ControllerShowHandles", evt.newValue);
        }
    }
}
