// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;
using UnityEditor;
using UnityEngine;

namespace Preliy.Flange.Planner.Editor
{
    [InitializeOnLoad]
    public static class HierarchyIconDrawer
    {
        static HierarchyIconDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            EditorApplication.RepaintHierarchyWindow();
        }

        private static void HierarchyWindowItemOnGUI(int instanceId, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceId) is not GameObject gameObject) return;

            if (gameObject.TryGetComponent(out MonoInstruction instruction))
            {
                DrawInstructionStateIcon(instruction, selectionRect);
            }
        }

        private static void DrawInstructionStateIcon(MonoInstruction monoInstruction, Rect selectionRect)
        {
            Color color;
            
            switch (monoInstruction.Instruction.State)
            {
                case InstructionState.Idle:
                    color = new Color(0.2f, 0.2f, 0.2f);
                    color = Color.gray;
                    break;
                case InstructionState.Ready:
                    color = Color.gray;
                    break;
                case InstructionState.Busy:
                    color = Color.yellow;
                    break;
                case InstructionState.Done:
                    color = Color.green;
                    break;
                case InstructionState.Error:
                    color = Color.red;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var iconRect = new Rect(selectionRect.xMax, selectionRect.y, 4, 14);
            EditorGUI.DrawRect(iconRect, color);
        }
    }
}
