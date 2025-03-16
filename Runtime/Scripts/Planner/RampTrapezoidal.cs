// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Preliy.Flange.Planner
{
    [Serializable]
    public class RampTrapezoidal
    {
        public List<RampKey> Keys => _keys;
        public float Duration => _duration;
        
        [SerializeField]
        private float _duration;
        [SerializeField]
        private List<RampKey> _keys = new ();

        private float _deltaTime;
        private float _p1, _v1, _a1, _t1;
        private float _p2, _v2, _a2, _t2;
        private float _p3, _v3, _a3, _t3;
        
        private readonly float _posInit;
        private readonly float _posTarget;
        
        private readonly float _speedInit;
        private readonly float _speedTarget;
        
        private readonly float _speedMax;
        private readonly float _accMax;

        public RampTrapezoidal()
        {
            
        }

        public RampTrapezoidal(float deltaTime, RampConfig config) : this(deltaTime, config.P0, config.P1, config.SpeedMax, config.AccMax, config.V0, config.V1){}

        public RampTrapezoidal(float deltaTime, float posInit, float posTarget, float speedMax, float accMax, float speedInit = 0f, float speedTarget = 0f)
        {
            _deltaTime = deltaTime;
            
            _posInit = posInit;
            _posTarget = posTarget;

            _speedInit = speedInit;
            _speedTarget = speedTarget;
            
            _speedMax = Mathf.Abs(speedMax);
            _accMax = Mathf.Abs(accMax);

            if (Mathf.Abs(_posInit - _posTarget) < Math.TOLERANCE_FLOAT)
            {
                return;
            }
            
            Initialize();
            Calculate();
        }
        
        private void Initialize()
        {
            var signDirection = Mathf.Sign(_posTarget - _posInit);
            
            var speedMax = signDirection * _speedMax;
            var speedInit = _speedInit;
            var speedTarget = _speedTarget;

            var distance = Mathf.Abs(_posTarget - _posInit);
            var speedDeltaPhaseAcc = speedMax - speedInit;
            var speedDeltaPhaseDec = speedMax - speedTarget;
            
            var signAcc = Mathf.Sign(speedDeltaPhaseAcc);
            var signDec = Mathf.Sign(speedDeltaPhaseDec);
                
            var t1 = Mathf.Abs(speedDeltaPhaseAcc) / _accMax;
            var t3 = Mathf.Abs(speedDeltaPhaseDec) / _accMax;
            var distanceS1 = 0.5f * Mathf.Abs(speedMax + speedInit) * t1;
            var distanceS3 = 0.5f * Mathf.Abs(speedMax + speedTarget) * t3;
            var t2 = (distance - distanceS1 - distanceS3) / Mathf.Abs(speedMax);

            if (distance <= distanceS1 + distanceS3)
            {
                var distancePseudo = distance + 0.5f * speedInit * (speedInit / _accMax) + 0.5f * speedTarget * (speedTarget / _accMax);
                speedMax = signDirection * Mathf.Sqrt(2.0f * distancePseudo * _accMax * _accMax / (_accMax + _accMax));
                
                speedDeltaPhaseAcc = speedMax - speedInit;
                speedDeltaPhaseDec = speedMax - speedTarget;
            
                signAcc = Mathf.Sign(speedDeltaPhaseAcc);
                signDec = Mathf.Sign(speedDeltaPhaseDec);

                t1 = Mathf.Abs(speedMax - speedInit) / _accMax;
                t2 = 0;
                t3 = Mathf.Abs(speedMax - speedTarget) / _accMax;
            }
            
            _p1 = _posInit;
            _v1 = speedInit;
            _a1 = signAcc * _accMax;
            _t1 = t1;

            _p2 = _p1 + (_v1 + _a1 * 0.5f * _t1) * _t1;
            _v2 = speedMax;
            _a2 = 0;
            _t2 = t2;
                
            _p3 = _p2 + _v2 * _t2;
            _v3 = speedMax;
            _a3 = -signDec * _accMax;
            _t3 = t3;

            _duration = _t1 + _t2 + _t3;
        }

        private void Calculate()
        {
            _keys = new List<RampKey>();
            var keyCount = Mathf.CeilToInt(_duration / _deltaTime);

            for (var i = 0; i < keyCount; i++)
            {
                var time = _deltaTime * i;
                var position = GetPosition(time);
                var speed = GetSpeed(time);
                var acc = GetAcc(time);
                var progress = Mathf.InverseLerp(_posInit, _posTarget, position);
                _keys.Add(new RampKey(position, speed, acc, time, progress));
            }

            _keys.Add(new RampKey(_posTarget, _speedTarget, 0, _duration, 1));
        }
        
        private float GetPosition(float time)
        {
            if (time <= 0)
            {
                return _p1;
            }
            if (time < _t1)
            {
                return _p1 + (_v1 + _a1 * 0.5f * time) * time;
            }
            if (time < _t1 + _t2)
            {
                var t2 = time - _t1;
                return _p2 + (_v2 + _a2 * 0.5f * t2) * t2;
            }
            if (time < _t1 + _t2 + _t3)
            {
                var t3 = time - _t1 - _t2;
                return _p3 + (_v3 + _a3 * 0.5f * t3) * t3;
            }
            return _posTarget;
        }
        
        private float GetSpeed(float time)
        {
            if (time < 0)
            {
                return _v1;
            }
            if (time < _t1)
            {
                return _v1 + _a1 * time;
            }
            if (time < _t1 + _t2)
            {
                return _v2 + _a2 * (time - _t1);
            }
            if (time < _t1 + _t2 + _t3)
            {
                return _v3 + _a3 * (time - _t1 - _t2);
            }
            return 0;
        }

        private float GetAcc(float time)
        {
            if (time <= 0)
            {
                return 0;
            }
            if (time <= _t1)
            {
                return _a1;
            }
            if (time <= _t1 + _t2)
            {
                return _a2;
            }
            if (time <= _t1 + _t2 + _t3)
            {
                return _a3;
            }
            return 0;
        }
    }
}
