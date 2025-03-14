// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Editor;
using Preliy.Flange.Planner.RawInstructions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    public class ProgramEditor : EditorWindow
    {
        private const string UXML = "UXML/Program";

        private VisualElement _container;
        private VisualElement _empty;
        private VisualElement _content;
        private ObjectField _programObject;
        private TaskEditor _taskEditor;
        private Button _buttonAddPtp;

        private readonly Property<Program> _program = new ();
        
        
        [MenuItem("Flange/Program Editor")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<ProgramEditor>();
            window.titleContent = new GUIContent("Program Editor");
        }

        public void CreateGUI()
        {
            _container = new VisualElement();
            Resources.Load<VisualTreeAsset>(UXML).CloneTree(_container);
            _container.style.flexGrow = 1;
            _container.style.flexShrink = 1;

            _empty = _container.Q<VisualElement>("empty");
            _content = _container.Q<VisualElement>("content");
            _programObject = _container.Q<ObjectField>("programObject");
            _programObject.RegisterValueChangedCallback(OnProgramObjectChanged);

            _buttonAddPtp = _container.Q<Button>("addPTP");
            _buttonAddPtp.clicked += AddPtpButtonOnClicked;
            

            _taskEditor = new TaskEditor();
            _content.Add(_taskEditor);
            

            rootVisualElement.Add(_container);

            EnableContent(false);
            
            Select(Selection.activeGameObject);
        }

        private void OnEnable()
        {
            _program.OnValueChanged += OnProgramChanged;
        }
        
        private void OnDisable()
        {
            _program.OnValueChanged -= OnProgramChanged;
            _programObject.UnregisterValueChangedCallback(OnProgramObjectChanged);
            _buttonAddPtp.clicked -= AddPtpButtonOnClicked;
        }

        private void OnSelectionChange()
        {
            Select(Selection.activeGameObject);
        }
        
        private void OnProgramChanged(Program sequence)
        {
            EnableContent(sequence != null);
            _programObject.SetValueWithoutNotify(sequence);

            var serializedObject = new SerializedObject(sequence);
            
            _taskEditor.Bind(sequence.Task);
            
            _taskEditor.Bind(serializedObject);
        }

        private void Select(GameObject gameObject)
        {
            if (gameObject == null) return;
            if (gameObject.TryGetComponent<Program>(out var program))
            {
                _program.Value = program;
            }
        }

        private void EnableContent(bool enable)
        {
            if (enable)
            {
                _empty.SetDisplay(false);
                _content.SetDisplay(true);
                _content.SetEnabled(true);
            }
            else
            {
                _empty.SetDisplay(true);
                _content.SetDisplay(false);
                _content.SetEnabled(false);
            }
        }

        private void OnProgramObjectChanged(ChangeEvent<Object> evt)
        {
            _program.Value = (Program)evt.newValue;
        }
        
        private void AddPtpButtonOnClicked()
        {
            if (_program.Value == null) return;
            //_program.Value.Task.Add(new PTP());
        }
    }
}
