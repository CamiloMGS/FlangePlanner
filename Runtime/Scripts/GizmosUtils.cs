// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Preliy.Flange.Planner.RawInstructions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Preliy.Flange.Planner
{
    public static class GizmosUtils
    {
        private const float DEFAULT_HANDLE_SCALE = 0.1f;

        public static void DrawHandle(Matrix4x4 matrix, float scale = 1f)
        {
            if (!matrix.ValidTRS()) return;
            var position = matrix.GetPosition();
            var rotation = matrix.rotation;
            
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.right),
                DEFAULT_HANDLE_SCALE * scale,
                EventType.Repaint
            );
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.up),
                DEFAULT_HANDLE_SCALE * scale,
                EventType.Repaint
            );
            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(
                0,
                position,
                rotation * Quaternion.LookRotation(Vector3.forward),
                DEFAULT_HANDLE_SCALE * scale,
                EventType.Repaint
            );
        }

        public static void DrawSegment(Controller controller, RawInstructions.Motion start, RawInstructions.Motion target, Color colorLine, float scale = 1f)
        {
            var p0 = start.GetTargetWorld(controller).GetPosition();
            var p1 = target.GetTargetWorld(controller).GetPosition();

            switch (target)
            {
                case JointMotion:
                    DrawDottedLine(p0, p1, colorLine, scale);
                    DrawHandleCap(p0, p1, colorLine, scale);
                    break;
                case LIN:
                    Handles.color = colorLine;
                    Handles.DrawLine(p0, p1, scale);
                    DrawHandleCap(p0, p1, colorLine, scale);
                    break;
                case CIRC circMove:
                    var wayPoint = circMove.TargetWayPoint.GetPosition();
                    DrawArc(p0, p1, wayPoint, colorLine);
                    break;
            }
        }

        public static void DrawMotionTarget(Controller controller, RawInstructions.Motion instruction, Color color, float scale = 1f, bool description = false, bool blendZone = false)
        {
            var target = instruction.GetTargetWorld(controller); 
            var position = target.GetPosition();
            
            Gizmos.color = color;
            Gizmos.DrawSphere(position, 0.01f * scale);

            var labelPosition = position + Vector3.down * 0.04f * scale;
            Handles.Label(labelPosition, description ? $"{instruction.Name} \n{instruction.Name}" : instruction.Name);

            if (blendZone && instruction.Blending > Math.TOLERANCE_FLOAT)
            {
                var colorAlpha = Color.yellow;
                colorAlpha.a = 0.2f;
                Handles.color = colorAlpha;
                Handles.RadiusHandle(Quaternion.identity, position, instruction.Blending);
            }

            DrawHandle(target, scale);
        }
        
        public static void DrawFrameOffset(Frame frame, Color pointColor, Color lineColor, float scale = 1f)
        {
            var style = new GUIStyle
            {
                normal =
                {
                    textColor = lineColor
                }
            };

            var handleSize = HandleUtility.GetHandleSize(frame.transform.position);
            var pivotPoint = Vector3.back * frame.Config.A;
            var parentPoint = Quaternion.Inverse(frame.transform.localRotation) * -frame.transform.localPosition;
            
            Handles.color = lineColor;
            
            if (Mathf.Abs(frame.Config.A) > 1e-5)
            {
                Handles.DrawLine(Vector3.zero, pivotPoint, 1);
                Handles.Label(pivotPoint * 0.5f, frame.Config.A.ToString("F3"), style);
            }
            
            if (Mathf.Abs(frame.Config.D) > 1e-5)
            {
                Handles.DrawLine(pivotPoint, parentPoint, 1);
                Handles.Label((pivotPoint + parentPoint) * 0.5f, frame.Config.D.ToString("F3"), style);
            }
            
            Gizmos.color = pointColor;
            Gizmos.DrawSphere(Vector3.zero,  handleSize * 0.05f * scale);
        }
        
        private static void DrawHandleCap(Vector3 p0, Vector3 p1, Color color, float scale)
        {
            var difference = p1 - p0;
            if (difference.magnitude < 0.0001f) return;

            var position = p0 + difference / 2;
            var rotation = Quaternion.LookRotation(difference.normalized);
            var size = 0.02f * scale;
            Handles.color = color;
            Handles.ConeHandleCap(
                0,
                position,
                rotation,
                size,
                EventType.Repaint
            );
        }
        
        private static void DrawDottedLine(Vector3 p0, Vector3 p1, Color color, float scale)
        {
            var distance = Vector3.Distance(p0, p1);
            scale = Mathf.Clamp(scale, 0.001f, 100f) / 20f;
            var segmentsNumber = Mathf.CeilToInt(distance / scale);
            var points = new Vector3[segmentsNumber];
            var step = 1f / segmentsNumber;

            Handles.color = color;

            for (var i = 0; i < segmentsNumber; i++)
            {
                points[i] = Vector3.Lerp(p0, p1, step * i);
                if (i > 0 && i % 2 == 0) Handles.DrawLine(points[i], points[i - 1], 20f * scale);
            }
        }
        
        private static void DrawArc(Vector3 start, Vector3 end, Vector3 wayPoint, Color color)
        {
            var arc = new Arc(start, end, wayPoint);
            var segmentCount = (int)arc.Angle / 2;
            if (segmentCount < 2) return;
            var points = new Vector3[segmentCount];
            points[0] = start;
            points[^1] = end;
            var step = 1f / segmentCount;

            for (var i = 1; i < segmentCount - 1; i++)
            {
                points[i] = arc.GetPoint(step * i);
            } 
            
            Handles.color = color;
            Handles.DrawPolyLine(points);
        }
        
        public static void DrawPoint(Matrix4x4 matrix, float scale = 1f)
        {
            DrawPoint(matrix.GetPosition(), scale);
        }
        
        public static void DrawPoint(Vector3 position, float scale = 1f)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(position, 0.01f * scale);
        }
    }
}
#endif
