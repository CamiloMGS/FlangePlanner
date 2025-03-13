// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using Preliy.Flange.Planner.Instructions;
using UnityEditor;
using UnityEngine;

namespace Preliy.Flange.Planner.Editor
{
    [InitializeOnLoad]
    public static class HierarchyIconDrawer
    {
        private const string INSTRUCTION_STATE_ICON_IDLE = "sv_icon_dot0_pix16_gizmo";
        private const string INSTRUCTION_STATE_ICON_READY = "sv_icon_dot1_pix16_gizmo";
        private const string INSTRUCTION_STATE_ICON_BUSY = "sv_icon_dot4_pix16_gizmo";
        private const string INSTRUCTION_STATE_ICON_DONE = "sv_icon_dot3_pix16_gizmo";
        private const string INSTRUCTION_STATE_ICON_ERROR = "redLight";

        static HierarchyIconDrawer()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            
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
            GUIContent icon;
            
            switch (monoInstruction.Instruction.State)
            {
                case InstructionState.Idle:
                    icon = EditorGUIUtility.IconContent(INSTRUCTION_STATE_ICON_IDLE);
                    break;
                case InstructionState.Ready:
                    icon = EditorGUIUtility.IconContent(INSTRUCTION_STATE_ICON_READY);
                    break;
                case InstructionState.Busy:
                    icon = EditorGUIUtility.IconContent(INSTRUCTION_STATE_ICON_BUSY);
                    break;
                case InstructionState.Done:
                    icon = EditorGUIUtility.IconContent(INSTRUCTION_STATE_ICON_DONE);
                    break;
                case InstructionState.Error:
                    icon = EditorGUIUtility.IconContent(INSTRUCTION_STATE_ICON_ERROR);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var iconRect = new Rect(selectionRect.xMax - 20, selectionRect.y, 16, 16);
            GUI.Label(iconRect, new GUIContent(icon));
        }
    }
}
