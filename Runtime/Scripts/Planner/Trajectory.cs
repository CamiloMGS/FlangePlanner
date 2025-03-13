// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Preliy.Flange.Planner
{
    public class Trajectory
    {
        public IReadOnlyList<TrajectoryKey> Keys => _keys;
        public float Duration => _duration;
        public bool IsValid => _isValid;

        private readonly List<TrajectoryKey> _keys = new ();
        private float _duration;
        private bool _isValid;

        private Vector3[] _line;
        private bool _lineIsCreated;

        public void Add(TrajectoryKey key, float sampleTime)
        {
            _keys.Add(key);
            Refresh(sampleTime);
        }
        
        public void InsertRange(Trajectory trajectory, int index, float sampleTime)
        {
            _keys.InsertRange(index, trajectory.Keys);
            Refresh(sampleTime);
        }
        
        public void RemoveRange(int index, int count, float sampleTime)
        {
            _keys.RemoveRange(index, count);
            Refresh(sampleTime);
        }

        public void Refresh(float sampleTime)
        {
            if (_keys.Count > 0)
            {
                
                for (var i = 0; i < _keys.Count; i++)
                {
                    _keys[i].Time = i * sampleTime;
                }
                
                _duration = _keys.Count * sampleTime;
                _isValid = _keys.All(key => key.IsValid);
            }
            else
            {
                _duration = 0;
                _isValid = true;
                return;
            }

            ApplyJointSpeed(sampleTime);
            ApplyCartesianSpeed(sampleTime);
        }
        
        public Trajectory Add(Trajectory trajectory, float sampleTime)
        {
            _keys.AddRange(trajectory.Keys);
            Refresh(sampleTime);
            return this; 
        }

        public Trajectory AddTimeOffset(float deltaTime, float offset)
        {
            if (_keys.Count == 0) return this;
            
            for (var i = 0; i < _keys.Count; i++)
            {
                _keys[i].Time = offset + i * deltaTime;
            }
            
            _duration = _keys.Count * deltaTime;
            return this; 
        }
        
        public TrajectoryKey GetKey(float progress)
        {
            progress = Mathf.Clamp01(progress);
            var index = (int)Mathf.Lerp(0, _keys.Count - 1, progress);
            return _keys[index];
        }

        private void ApplyJointSpeed(float sampleTime)
        {
            if (_keys == null) return;
            if (_keys.Count == 0) return;

            for (var i = 1; i < _keys.Count; i++)
            {
                for (var j = 0; j < JointTarget.LENGTH; j++)
                {
                    var speed = (_keys[i].JointPosition[j] - _keys[i - 1].JointPosition[j]) / sampleTime;
                    _keys[i].JointSpeed[j] = speed;
                }
            }

            _keys.First().JointSpeed = JointTarget.Null;
            _keys.Last().JointSpeed = JointTarget.Null;
        }

        private void ApplyCartesianSpeed(float sampleTime)
        {
            if (_keys == null) return;
            if (_keys.Count == 0) return;

            for (var i = 1; i < _keys.Count; i++)
            {
                _keys[i].CartesianLimit.LinearSpeed = Vector3.Distance(_keys[i].CartesianPose.GetPosition(), _keys[i - 1].CartesianPose.GetPosition()) / sampleTime;
                _keys[i].CartesianLimit.RotationSpeed = Quaternion.Angle(_keys[i].CartesianPose.rotation, _keys[i - 1].CartesianPose.rotation) / sampleTime;
            }
            
            _keys.First().CartesianLimit = CartesianLimit.Null;
            _keys.Last().CartesianLimit = CartesianLimit.Null;
        }
        
#if UNITY_EDITOR
        private void CreatePolyLine()
        {
            if (_keys.Count > 0)
            {
                _line = new Vector3[_keys.Count];
                for (var i = 0; i < _keys.Count; i++)
                {
                    _line[i] = _keys[i].CartesianPose.GetPosition();
                }
            }
            else
            {
                _line = Array.Empty<Vector3>();
            }
        }
        
        public void DrawGizmos(TrajectoryGizmosConfig config)
        {
            if (!config.Enable) return;
            if (!_lineIsCreated) CreatePolyLine();
            if (_line == null) return;
            if (_line.Length == 0) return;
            Handles.DrawPolyLine(_line);

            if (config.ShowPoints)
            {
                for (var i = 0; i < _keys.Count; i++)
                {
                    var position = _keys[i].CartesianPose.GetPosition();
                    
                    if (_keys[i].IsValid)
                    {
                        GizmosUtils.DrawHandle(_keys[i].CartesianPose, 0.25f * config.Scale);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(position, 0.01f * config.Scale);
                    }

                    if (config.ShowPointLinearSpeed)
                    {
                        var labelPosition = position + Vector3.down * 0.04f * config.Scale;
                        Handles.Label(labelPosition, GetPointDescription(i, config));
                    }
                }
            }
        }

        private string GetPointDescription(int index, TrajectoryGizmosConfig config)
        {
            var result = index.ToString();
            if (config.ShowPointLinearSpeed)
            {
                result += $"\nLin\n{_keys[index].CartesianLimit.LinearSpeed:F2}";
            }
            return result;
        }
        
#endif
    }
}
