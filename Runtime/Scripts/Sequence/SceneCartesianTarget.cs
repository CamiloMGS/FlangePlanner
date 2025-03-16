// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Preliy.Flange.Planner.Sequence
{
    /// <summary>
    /// Target in scene as MonoBehaviour
    /// Contains the <see cref="CartesianTarget">RobotTarget</see> and <see cref="JointTarget">ExtJointTarget</see> definition as well as the <see cref="Configuration">Configuration</see> for the current transformation position.
    /// </summary>
    public class SceneCartesianTarget : MonoBehaviour
    {
        public CartesianTarget Target => new (transform.GetMatrix(), Configuration, ExtJointTarget, this);
        
        public ExtJoint ExtJointTarget
        {
            
            get => _extJointTarget;
            set => _extJointTarget = value;
        }
        
        public Configuration Configuration
        {
            
            get => _configuration;
            set => _configuration = value;
        }
        
        public bool ShowGizmos
        {
            get => _showGizmos;
            set => _showGizmos = value;
        }
        
        public bool ShowLabel
        {
            get => _showLabel;
            set => _showLabel = value;
        }

        [SerializeField]
        private Configuration _configuration;
        [SerializeField]
        private ExtJoint _extJointTarget;

        [Header("Gizmos")]
        [SerializeField]
        private bool _showGizmos;
        [SerializeField]
        private bool _showLabel;
        [Range(0.01f, 10f)]
        [SerializeField]
        private float _gizmosScale = 1f;

        public void Initialize(Controller controller)
        {
            var parent = controller.GetFrameTransform(controller.Frame.Value);
            if (parent == null) parent = controller.transform.root;
            
            transform.parent = parent;
            transform.SetMatrix(controller.GetTcpWorld());
            _extJointTarget = controller.MechanicalGroup.JointState.ExtJoint;
            _configuration = controller.Configuration.Value;
        }
        
        public static void Create(Controller controller)
        {
            var parent = controller.GetFrameTransform(controller.Frame.Value);
            if (parent == null) parent = controller.transform.root;
            
            var index = parent.GetComponentsInChildren<CartesianTarget>().Length;
            var gameObject = new GameObject($"p_{index * 10}");
            var sceneTarget = gameObject.AddComponent<SceneCartesianTarget>();
            sceneTarget.transform.parent = parent.transform;
            sceneTarget.transform.SetMatrix(controller.GetTcpWorld());
            sceneTarget.ExtJointTarget = controller.MechanicalGroup.JointState.ExtJoint;
            sceneTarget.Configuration = controller.Configuration.Value;
            sceneTarget.ShowGizmos = true;
            sceneTarget.ShowLabel = true;

#if UNITY_EDITOR     
            Undo.RegisterCreatedObjectUndo(gameObject, $"Cartesian Pose {gameObject.name} created");
#endif
            Logger.Log(LogType.Log,$"{controller.name}: Cartesian Pose {gameObject.name} created!", controller);
        }
        
        /// <summary>
        /// Jump to specific <see cref="CartesianTarget"/>
        /// </summary>
        /// <param name="controller"> <see cref="Controller"/></param>
        /// <param name="ignoreMask"> <see cref="SolutionIgnoreMask"/></param>
        /// <param name="showErrorMassage">Show error message, if error occured</param>
        public void JumpTo(Controller controller, SolutionIgnoreMask ignoreMask, bool showErrorMassage = true)
        {
            var solution = controller.Solver.ComputeInverse(transform.GetMatrix(), controller.Tool.Value, Configuration, ExtJointTarget, ignoreMask);
            controller.Solver.TryApplySolution(solution, showErrorMassage);
        }
        
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR            
            if (!_showGizmos) return;
            Gizmos.DrawSphere(transform.position, 0.01f * _gizmosScale);
            if (_showLabel) Handles.Label(transform.position, transform.name);
            Flange.GizmosUtils.DrawHandle(transform.GetMatrix(), _gizmosScale);
#endif
        }
    }
}
