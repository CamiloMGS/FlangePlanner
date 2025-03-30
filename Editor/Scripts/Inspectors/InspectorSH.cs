// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, you can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Preliy.Flange;
using Preliy.Flange.Editor;
using Preliy.Flange.Planner;
using Preliy.Flange.Planner.Sequence;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CanEditMultipleObjects]
[CustomEditor(typeof(SequenceHandler))]
public class InspectorPTP : Editor
{
    private const string UXML = "UXML/InspectorSH";
    private SequenceHandler _sequenceHandler;
    private int _previousInstructionCount;
    private Button _buttonCompile;
    private Dictionary<GameObject, MonoInstruction> _instructionDictionary = new();
    private void OnEnable()
    {
        _sequenceHandler = target as SequenceHandler;
        _previousInstructionCount = _sequenceHandler.RawInstructions.Count;
        CleanAndGetAvailableInstructions();
        //This event is trigger from the PropertyDrawerSH script
        SequenceHandler.OnInstructionChanged += OnInstructionChanged;
    }
    public override VisualElement CreateInspectorGUI()
    {
        var container = new VisualElement();
        Resources.Load<VisualTreeAsset>(UXML).CloneTree(container);

        PropertyField rawInstructions = container.Q<PropertyField>("Instructions");
        _buttonCompile = container.Q<Button>("DoSomething");
        _buttonCompile.clicked += ButtonCompileOnClicked;
        //InspectorElement.FillDefaultInspector(container, serializedObject, this);

        rawInstructions.RegisterValueChangeCallback(OnRawInstructionsChanged);
        return container;
    }
    private void OnDisable()
    {
        SequenceHandler.OnInstructionChanged -= OnInstructionChanged;
    }
    private void OnInstructionChanged(object sender, SequenceHandler.OnInstructionChangedEventArgs e)
    {
        if (e.NewValue != SequenceHandler.InstructionType.None)
        {
            AddNewInstruction(e.InstructionIndex, e.NewValue);
        }
        else if (e.PreviousValue != SequenceHandler.InstructionType.None)
        {
            RemoveInstructionAtIndex(e.InstructionIndex);
        }

    }

    private void ButtonCompileOnClicked()
    {
        Debug.Log("Hola...");
    }

    /// <summary>
    /// Iterates backwards through the raw instructions to remove any invalid item.
    /// This is used when the GameObject containing a MonoInstruction has been deleted.
    /// It also generates a dictionary of valid instructions to detect when an instruction
    /// is removed from the list in the inspector.
    /// </summary>
    private void CleanAndGetAvailableInstructions()
    {
        for (int i = _sequenceHandler.RawInstructions.Count - 1; i >= 0; i--)
        {
            var instruction = _sequenceHandler.RawInstructions[i];
            if (instruction.instructionType != SequenceHandler.InstructionType.None && instruction.gameObject == null)
            {
                _sequenceHandler.RawInstructions.RemoveAt(i);
                _sequenceHandler.TryToRefresh();
            }
            else
            {
                _instructionDictionary.Add(instruction.gameObject, instruction.monoInstruction);
            }
        }
    }

    private void OnRawInstructionsChanged(SerializedPropertyChangeEvent evt)
    {
        int currentCount = _sequenceHandler.RawInstructions.Count;

        // New Item have been added to the inspector's list
        if (_sequenceHandler.RawInstructions.Count > _previousInstructionCount)
        {
            var newInstruction = _sequenceHandler.RawInstructions.Last();

            newInstruction.instructionType = SequenceHandler.InstructionType.None;
            newInstruction.monoInstruction = null;
            newInstruction.gameObject = null;

            _previousInstructionCount = currentCount;
        }
        // Item have been removed from the inspector's list.
        // The problem is that Unity does not give any information about the index of the removed item.
        else if (currentCount < _previousInstructionCount)
        {
            //So, this method will handle this
            RemoveInstruction();
            _sequenceHandler.TryToRefresh();
            _previousInstructionCount = currentCount;
        }
    }
    public void AddNewInstruction(int index, SequenceHandler.InstructionType instructionType)
    {
        var instruction = _sequenceHandler.RawInstructions[index];

        GameObject monoInstructionObject = new GameObject();
        monoInstructionObject.transform.SetParent(_sequenceHandler.transform);
        Pose tcpPose = GetTcpPose(_sequenceHandler.RobotTask.Controller);
        monoInstructionObject.transform.SetPositionAndRotation(tcpPose.position, tcpPose.rotation);
        instruction.gameObject = monoInstructionObject;
        MonoInstruction monoInstruction = CreateMonoInstruction(monoInstructionObject, instructionType);

        instruction.monoInstruction = monoInstruction;

        _instructionDictionary.Add(monoInstructionObject, monoInstruction);
        _sequenceHandler.TryToRefresh();
    }
    public void RemoveInstructionAtIndex(int index)
    {
        var instruction = _sequenceHandler.RawInstructions[index];
        DestroyImmediate(instruction.gameObject);

        _sequenceHandler.RawInstructions[index].gameObject = null;
        _sequenceHandler.RawInstructions[index].monoInstruction = null;

        _sequenceHandler.TryToRefresh();
    }
    private MonoInstruction CreateMonoInstruction(GameObject monoInstructionObject, SequenceHandler.InstructionType instructionType)
    {
        switch (instructionType)
        {
            case SequenceHandler.InstructionType.None:
                return null;
            case SequenceHandler.InstructionType.PTP:
                return monoInstructionObject.AddComponent<PTPCartesianTarget>();
            default:
                return null;
        }
    }
    private Pose GetTcpPose(Controller controller)
    {
        Matrix4x4 tcpMatrix = controller.PoseObserver.ToolCenterPointFrame.Value;
        Vector3 tcpPosition = tcpMatrix.MultiplyPoint3x4(Vector3.zero);
        Quaternion tcpRotation = Quaternion.LookRotation(tcpMatrix.GetColumn(2), tcpMatrix.GetColumn(1));
        return new Pose(tcpPosition, tcpRotation);
    }
    /// <summary>
    /// Iterates through the children of the _sequenceHandler and checks each child's associated MonoInstruction 
    /// in the _instructionDictionary. If a child does not have a corresponding instruction in the _sequenceHandler.RawInstructions list,
    /// it is considered orphaned. In that case, the GameObject is removed from the dictionary and destroyed immediately.
    /// </summary>
    public void RemoveInstruction()
    {
        GameObject objectToRemove = null;

        foreach (Transform child in _sequenceHandler.transform)
        {
            if (_instructionDictionary.TryGetValue(child.gameObject, out MonoInstruction monoInst))
            {
                var instruction = _sequenceHandler.RawInstructions.Find(ins => ins.monoInstruction == monoInst);
                if (instruction == null)
                {
                    objectToRemove = child.gameObject;
                    break;
                }
            }
        }

        if (objectToRemove != null)
        {
            _instructionDictionary.Remove(objectToRemove);
            DestroyImmediate(objectToRemove);
        }
    }
}
