// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using UnityEngine;

namespace Preliy.Flange.Planner.Instructions
{
    [Serializable]
    public abstract class CartesianMotion : Motion
    {
        public CartesianTarget CartesianTarget 
        {
            get => _cartesianTarget;
            set => _cartesianTarget = value;
        }
        
        public int Tool
        {
            get => _tool;
            set => _tool = value;
        }
        
        public int Frame
        {
            get => _frame;
            set => _frame = value;
        }
        
        public Matrix4x4 Target => _target;


        [SerializeField]
        protected Matrix4x4 _target;
        
        
        [SerializeField]
        protected CartesianTarget _cartesianTarget;
        [SerializeField]
        protected int _tool;
        [SerializeField]
        protected int _frame;
        
        
    }
}
