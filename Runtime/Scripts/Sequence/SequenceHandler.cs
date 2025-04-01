using System;
using System.Collections;
using System.Collections.Generic;
using Preliy.Flange.Planner.Sequence;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    /// <summary>
    /// Handles a list of raw instructions for the robot.
    /// </summary>
    [RequireComponent(typeof(SceneSequence))]
    public class SequenceHandler : MonoBehaviour
    {
        public List<RawInstruction> RawInstructions = new List<RawInstruction>();

        [Serializable]
        public class RawInstruction
        {
            public InstructionType instructionType;
            public MonoInstruction monoInstruction;

            public GameObject gameObject;
        }

        /// <summary>
        /// Enum representing the available instruction types.
        /// </summary>
        public enum InstructionType
        {
            None,
            PTP,
        }

        // Static event raised when an instruction type is changed from the inspector.
        public static EventHandler<OnInstructionChangedEventArgs> OnInstructionChanged;

        public class OnInstructionChangedEventArgs : EventArgs
        {
            public OnInstructionChangedEventArgs(int instructionIndex, int previousValue, int newValue)
            {
                InstructionIndex = instructionIndex;
                PreviousValue = (InstructionType)previousValue;
                NewValue = (InstructionType)newValue;
            }
            public int InstructionIndex;
            public InstructionType PreviousValue;
            public InstructionType NewValue;
        }


        [SerializeField] private Task _task;

        public Task RobotTask { get => _task; }

        void OnValidate()
        {
            if (_task == null)
            {
                if (TryGetComponent<Task>(out _task))
                {
                    Debug.Log("Task component found and assigned.");
                }
                else
                {
                    Debug.LogError("Task component not found.");
                }
            }
        }

        /// <summary>
        /// Tries to refresh the scene sequence by invoking Refresh on the Task,
        /// if it is a SceneSequence.
        /// </summary>
        public void TryToRefresh()
        {
            SceneSequence sceneSequence = _task as SceneSequence;
            if (sceneSequence != null)
            {
                sceneSequence.Refresh();
            }
        }
    }
}
