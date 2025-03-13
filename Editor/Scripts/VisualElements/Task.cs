// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    public class TaskEditor : VisualElement
    {
        private readonly TwoPaneSplitView _splitView;
        private MultiColumnListView _columnListView;
        private readonly VisualElement _content;

        private Instructions.Task _task;

        private SerializedObject _serializedObject;

        public TaskEditor()
        {
            style.flexGrow = 1f;
            style.flexShrink = 1f;
            
            _splitView = new TwoPaneSplitView();
            _columnListView = CreateListView();
            _content = new VisualElement();

            _splitView.Add(_columnListView);
            _splitView.Add(_content);

            Add(_splitView);
        }

        public void Bind(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
            _columnListView.Bind(_serializedObject);
        }

        public void Bind(Instructions.Task task)
        {
            _task = task;
            _task.OnInstructionsListChanged += OnInstructionsListChanged;
            OnInstructionsListChanged();
        }

        public void Dispose()
        {
            if (_task != null) _task.OnInstructionsListChanged += OnInstructionsListChanged;
        }

        private MultiColumnListView CreateListView()
        {
            var columnListView = new MultiColumnListView();
            columnListView.reorderable = true;
            columnListView.showBorder = true;

            columnListView.columns.Add(new Column() { name = "index", title = "Index", width = 20f });
            columnListView.columns.Add(new Column() { name = "type", title = "Type", minWidth = 100, stretchable = true});
            columnListView.columns.Add(new Column() { name = "data", title = "Data", minWidth = 100, stretchable = true});

            columnListView.columns["index"].makeCell = () => new Label();
            columnListView.columns["type"].makeCell = () => new Label();
            columnListView.columns["data"].makeCell = () => new Label();
            
            columnListView.columns["index"].bindCell = (element, index) => ((Label)element).text = index.ToString();
            columnListView.columns["type"].bindCell = (element, index) => 
            {
                ((Label)element).text = _task.Instructions[index].Name;
            };
            
            columnListView.columns["type"].bindCell = (element, index) => 
            {
                ((Label)element).text = _task.Instructions[index].ToString();
            };
            
            columnListView.selectionChanged += ColumnListViewOnSelectionChanged;

            return columnListView;
        }
        
        private void OnInstructionsListChanged()
        {
            _columnListView.itemsSource = _task.Instructions;
            _columnListView.Rebuild();
        }
        
        private void ColumnListViewOnSelectionChanged(IEnumerable<object> obj)
        {
            Debug.Log(_columnListView.selectedIndex);
            
            _content.Clear();
            
            var taskProperty = _serializedObject.FindProperty("_task");
            var instructionsProperty = taskProperty.FindPropertyRelative("_instructions");
            var instruction = instructionsProperty.GetArrayElementAtIndex(_columnListView.selectedIndex);
            instruction.isExpanded = true;

            var propertyField = new PropertyField(instruction, "Instruction");
            propertyField.Bind(_serializedObject);

            _content.Add(propertyField);
        }

        private void ShowInstructionContent()
        {
            
        }
    }
}
