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
    [CustomEditor(typeof(MainTaskExecutor))]
    public class SequenceExecutorInspector : UnityEditor.Editor
    {
        private const string USS = "USS/Inspector";
        private Label _stateLabel;
        
        private MainTaskExecutor _mainTaskExecutor;

        private void OnEnable()
        {
            _mainTaskExecutor = target as MainTaskExecutor;
        }
        
        private void OnDisable()
        {
            _mainTaskExecutor.State.Unsubscribe(SetStateText);
        }
        
        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            
            container.styleSheets.Add(Resources.Load<StyleSheet>(USS));
            _stateLabel = new Label
            {
                text = $"State: {_mainTaskExecutor.State.Value.ToString()}",
                style = { unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold)}
            };
            _stateLabel.AddToClassList("unity-base-field");
            _stateLabel.AddToClassList("unity-text-field");

            var taskField = new PropertyField(serializedObject.FindProperty("_sequence").FindPropertyRelative("value"), "Sequence");
            var autoStartField = new PropertyField(serializedObject.FindProperty("_autoStart").FindPropertyRelative("value"), "Auto Start");
            var loopField = new PropertyField(serializedObject.FindProperty("_loop").FindPropertyRelative("value"), "Loop");

            var executeButton = new Button(() => _mainTaskExecutor.Execute())
            {
                text = "Execute"
            };
            
            var resetButton = new Button(() => _mainTaskExecutor.ResetError())
            {
                text = "Reset"
            };
            
            var stopButton = new Button(() => _mainTaskExecutor.Stop())
            {
                text = "Stop",
                style = { backgroundColor = new StyleColor(new Color(0.5f, 0.15f, 0.15f))}
            };
            
            container.Add(_stateLabel);
            container.Add(taskField);
            container.Add(autoStartField);
            container.Add(loopField);
            container.Add(executeButton);
            container.Add(resetButton);
            container.Add(stopButton);
            
            _mainTaskExecutor.State.Subscribe(SetStateText);
            
            return container;
        }
        
        private void SetStateText(MainTaskExecutor.ExecutionState state)
        {
            _stateLabel.text = $"State: {state.ToString()}";
        }
    }
}
