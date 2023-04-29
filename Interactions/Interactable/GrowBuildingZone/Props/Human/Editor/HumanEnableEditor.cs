using UnityEditor;
using UnityEngine;

namespace Interactions.Humans
{
    [CustomEditor(typeof(HumanEnable))]
    public class HumanEnableEditor : Editor
    {
        private HumanEnable _humanEnable;

        private void OnEnable()
        {
            _humanEnable = (HumanEnable)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill humans list"))
                _humanEnable.FillHumansList();
        }
    }
}
