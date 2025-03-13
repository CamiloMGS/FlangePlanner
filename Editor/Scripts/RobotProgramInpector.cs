// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Threading;
using Cysharp.Threading.Tasks;
using Preliy.Flange.Planner.Instructions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Preliy.Flange.Planner.Editor
{
    [CustomEditor(typeof(Sequence), true)]
    public class RobotProgramInspector : UnityEditor.Editor
    {
        private Sequence _sequence;
        private VisualElement _instructionListContainer;

        private void OnEnable()
        {
            _sequence = target as Sequence;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var container = new VisualElement();
            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            var buttonCompile = new Button(CompileButtonClick)
            {
                text = "Compile"
            };
            
            container.Add(buttonCompile);
            
            return container;
        }
        
        private void CompileButtonClick()
        {
            _sequence.Compile(new CancellationToken()).Forget();
        }
    }
}
