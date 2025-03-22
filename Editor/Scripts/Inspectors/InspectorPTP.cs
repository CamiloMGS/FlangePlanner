// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Editor;
using Preliy.Flange.Planner.Sequence;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Preliy.Flange.Planner.Editor
{
    [CanEditMultipleObjects]
    //[CustomEditor(typeof(PTPCartesianTarget))]
    // ReSharper disable once InconsistentNaming
    public class InspectorPTP : UnityEditor.Editor
    {
        private const string UXML = "UXML/InspectorPTP";

        private PTPCartesianTarget _instruction;
        private VisualElement _stateContainer;
        private ObjectField _cartesianTargetReference;
        private PropertyField _cartesianConfiguration;
        private PropertyField _cartesianExtJoints;
        private Button _buttonJump;
        private HelpBox _helpBox;

        private void OnEnable()
        {
            _instruction = target as PTPCartesianTarget;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(container);

            _stateContainer = container.Q<VisualElement>("stateContainer");
            _cartesianTargetReference = container.Q<ObjectField>("sceneCartesianTarget");
            _cartesianConfiguration = container.Q<PropertyField>("configuration");
            _cartesianExtJoints = container.Q<PropertyField>("extJoints");

            _cartesianTargetReference.AlignedField();

            container.Q<EnumField>("state").AlignedField();
            container.Q<IntegerField>("frame").AlignedField();
            container.Q<IntegerField>("tool").AlignedField();
            container.Q<FloatField>("speed").AlignedField();
            container.Q<FloatField>("blending").AlignedField();

            _cartesianTargetReference.RegisterValueChangedCallback(CartesianTargetRefChanged);

            if (_instruction != null)
            {
                SetCartesianTargetView(_instruction.SceneCartesianTarget);
            }

            _buttonJump = container.Q<Button>("jump");
            _buttonJump.clicked += ButtonJumpOnClicked;

            // _helpBox = new HelpBox
            // {
            //     text = _instruction.Exception.Value?.Message,
            //     messageType = HelpBoxMessageType.Error
            // };
            // _stateContainer.Add(_helpBox);
            //
            // OnExceptionChanged(_instruction.Exception.Value);
            // _instruction.Exception.OnValueChanged += OnExceptionChanged;

            return container;
        }
        
        private void OnDisable()
        {
            _buttonJump.clicked -= ButtonJumpOnClicked;
            //_instruction.Exception.OnValueChanged -= OnExceptionChanged;
        }

        private void SetCartesianTargetView(SceneCartesianTarget sceneCartesianTarget)
        {
            if (sceneCartesianTarget == null)
            {
                _cartesianConfiguration.SetEnabled(true);
                _cartesianConfiguration.SetDisplay(true);
                _cartesianExtJoints.SetEnabled(true);
                _cartesianExtJoints.SetDisplay(true);
            }
            else
            {
                _cartesianConfiguration.SetEnabled(false);
                _cartesianConfiguration.SetDisplay(false);
                _cartesianExtJoints.SetEnabled(false);
                _cartesianExtJoints.SetDisplay(false);
            }
        }
        
        private void CartesianTargetRefChanged(ChangeEvent<Object> evt)
        {
            var cartesianTargetRef = (SceneCartesianTarget)evt.newValue;
            SetCartesianTargetView(cartesianTargetRef);
        }
        
        private void ButtonJumpOnClicked()
        {
            _instruction.JumpToTarget();
        }
        
        private void OnExceptionChanged(Exception exception)
        {
            if (exception == null)
            {
                _helpBox.SetDisplay(false);
            }
            else
            {
                _helpBox.SetDisplay(true);
                _helpBox.text = exception.Message;
            }
        }
    }
}
