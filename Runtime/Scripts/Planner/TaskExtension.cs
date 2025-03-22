// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;
using UnityEngine.Events;
using Preliy.Flange.Planner.RawInstructions;
using Preliy.Flange.Planner.Sequence;

namespace Preliy.Flange.Planner
{
    public abstract partial class ScriptableTask
    {
        /// <summary>
        /// Log message is console
        /// </summary>
        protected void Log(LogType type, string message)
        {
            Add(new Log()
            {
                LogType = type,
                Message = message
            });
        }
        
        /// <summary>
        /// Point to point motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">RobotTarget</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTP(CartesianTarget target, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            Add(new RawInstructions.PTPCartesianTarget
            {
                CartesianTarget = target,
                Tool = tool,
                Frame = frame,
                Speed = speed,
                Blending = blending
            });
        }
        
        /// <summary>
        /// Point to point motion
        /// </summary>
        /// <param name="target"><see cref="JointTarget">JointTarget</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTP(JointTarget target, float speed = 1f, float blending = -1f)
        {
            Add(new RawInstructions.PTPJointTarget
            {
                JointTarget = target,
                Speed = speed,
                Blending = blending
            });
        }
        
        /// <summary>
        /// Point to point motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTP(SceneCartesianTarget target, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var robotTarget = _controller.ConvertFrame(target.Target, (int)CoordinateSystem.World, frame);
            PTP(robotTarget, speed, tool, frame, blending);
        }

        /// <summary>
        /// Linear continuous motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">RobotTarget</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LIN(CartesianTarget target, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            if (tool < -1) tool = _controller.Tool.Value;
            Add(new LIN
            {
                CartesianTarget = target, 
                Tool = tool, 
                Frame = frame,
                Speed = speed,
                Blending = blending
            });
        }
        
        /// <summary>
        /// Linear continuous motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LIN(SceneCartesianTarget target, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var robotTarget = _controller.ConvertFrame(target.Target, (int)CoordinateSystem.World, frame);
            LIN(robotTarget, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Circular continuous motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Robot Target</see></param>
        /// <param name="wayPoint"><see cref="Flange.CartesianTarget">Robot Target for way point</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void CIRC(CartesianTarget target, CartesianTarget wayPoint, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            if (tool < -1) tool = _controller.Tool.Value;
            Add(new CIRC
            {
                CartesianTarget = target, 
                WayPoint = wayPoint,
                Tool = tool, 
                Frame = frame,
                Speed = speed,
                Blending = blending
            });
        }

        /// <summary>
        /// Circular continuous motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="wayPoint"><see cref="CartesianTarget">Scene Way Point</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void CIRC(SceneCartesianTarget target, SceneCartesianTarget wayPoint, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var robotTarget = _controller.ConvertFrame(target.Target, (int)CoordinateSystem.World, frame);
            var wayPointTarget = _controller.ConvertFrame(wayPoint.Target, (int)CoordinateSystem.World, frame);
            CIRC(robotTarget, wayPointTarget, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Matrix4x4">Offset matrix</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(CartesianTarget target, Matrix4x4 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var pose = target.Pose * offset;
            target.Pose = pose;
            PTP(target, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Vector3">Offset position</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(CartesianTarget target, Vector3 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            PTPRel(target, Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Quaternion">Offset rotation</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(CartesianTarget target, Quaternion offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            PTPRel(target, Matrix4x4.TRS(Vector3.zero, offset, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Matrix4x4">Offset matrix</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(SceneCartesianTarget target, Matrix4x4 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var robotTarget = _controller.ConvertFrame(target.Target, (int)CoordinateSystem.World, frame);
            PTPRel(robotTarget, offset, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Vector3">Offset position</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(SceneCartesianTarget target, Vector3 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            PTPRel(target, Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative point to point motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Quaternion">Offset rotation</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void PTPRel(SceneCartesianTarget target, Quaternion offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            PTPRel(target, Matrix4x4.TRS(Vector3.zero, offset, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Matrix4x4">Offset matrix</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(CartesianTarget target, Matrix4x4 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var pose = target.Pose * offset;
            target.Pose = pose;
            LIN(target, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Vector3">Offset position</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(CartesianTarget target, Vector3 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            LINRel(target, Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="Flange.CartesianTarget">Target</see></param>
        /// <param name="offset"><see cref="Quaternion">Offset rotation</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(CartesianTarget target, Quaternion offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            LINRel(target, Matrix4x4.TRS(Vector3.zero, offset, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Matrix4x4">Offset matrix</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(SceneCartesianTarget target, Matrix4x4 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            var robotTarget = _controller.ConvertFrame(target.Target, (int)CoordinateSystem.World, frame);
            LINRel(robotTarget, offset, speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Vector3">Offset position</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(SceneCartesianTarget target, Vector3 offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            LINRel(target, Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one), speed, tool, frame, blending);
        }
        
        /// <summary>
        /// Relative linear motion
        /// </summary>
        /// <param name="target"><see cref="CartesianTarget">Scene Target</see></param>
        /// <param name="offset"><see cref="Quaternion">Offset rotation</see></param>
        /// <param name="speed">Speed factor [0-1]</param>
        /// <param name="tool">Tool index</param>
        /// <param name="frame">Frame index</param>
        /// <param name="blending">Blending distance [mm]</param>
        // ReSharper disable once InconsistentNaming
        public void LINRel(SceneCartesianTarget target, Quaternion offset, float speed = 1f, int tool = -1, int frame = (int)CoordinateSystem.Base, float blending = -1f)
        {
            LINRel(target, Matrix4x4.TRS(Vector3.zero, offset, Vector3.one), speed, tool, frame, blending);
        }

        /// <summary>
        /// Set tool
        /// </summary>
        public void SetTool(int index)
        {
            Add(new SetToolIndex
            {
                ToolIndex = index
            });
        }
        
        /// <summary>
        /// Wait for time
        /// </summary>
        /// <param name="milliseconds">time [ms]</param>
        public void Wait(int milliseconds)
        {
            Add(new Wait
            {
                Time = milliseconds
            });
        }

        /// <summary>
        /// Wait until condition is true
        /// </summary>
        /// <param name="condition">condition</param>
        public void WaitUntil(Func<bool> condition)
        {
            Add(new WaitCondition
            {
                Condition = condition
            });
        }
        
        /// <summary>
        /// Execute action
        /// </summary>
        /// <param name="action">Action</param>
        public void Action(Action action)
        {
            Add(new ActionExecutor
            {
                Action = action
            });
        }

        /// <summary>
        /// Execute Unity event
        /// </summary>
        /// <param name="unityEvent">Unity event</param>
        /// <param name="value">value</param>
        public void Action<T>(UnityEvent<T> unityEvent, T value)
        {
            Add(new UnityEventInstruction<T>
            {
                UnityEvent = unityEvent,
                Value = value
            });
        }
    }
}
