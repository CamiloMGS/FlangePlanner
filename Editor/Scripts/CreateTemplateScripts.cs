// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using UnityEditor;

namespace Preliy.Flange.Editor
{
    public static class CreateTemplateScripts
    {
        private const string PATH_TEMPLATES = "Assets/com.preliy.robotics/Editor/ScriptsTemplates/";
        private const string TASK_TEMPLATE = "TaskTemplate.cs.txt";

        [MenuItem(itemName: "Assets/Robotics/Create New Sequence", isValidateFunction: false, priority: 50)]
        public static void CreateTaskFromTemplate()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(PATH_TEMPLATES + TASK_TEMPLATE, "NewTask.cs");
        }
    }
}
