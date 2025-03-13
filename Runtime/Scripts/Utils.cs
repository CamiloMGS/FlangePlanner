// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;

namespace Preliy.Flange.Planner
{
    public static class Utils
    {
        public static T CreateComponentWithGameObject<T>(this Transform transform) where T : Component
        {
            var gameObject = new GameObject
            {
                transform =
                {
                    parent = transform.transform
                }
            };
            
            var component = gameObject.AddComponent<T>();
            return component;
        }
    }
}
