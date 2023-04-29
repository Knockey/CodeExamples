using UnityEditor;
using UnityEngine;

namespace Interactions.Humans
{
    [CustomEditor(typeof(HumanEnableList))]
    public class HumanEnableListEdtior : Editor
    {
        private HumanEnableList _humanEnableList;

        private void OnEnable()
        {
            _humanEnableList = (HumanEnableList)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill human enable list"))
                _humanEnableList.FillHumanEnableList();
        }
    }
}
