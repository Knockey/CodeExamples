using UnityEditor;
using UnityEngine;

namespace Interactions.EditorExtensions
{
    [CustomEditor(typeof(GrowBuildingBlinkList))]
    public class GrowBuildingBlinkListEditor : Editor
    {
        private GrowBuildingBlinkList _growingBuildingsBlinkList;

        private void OnEnable()
        {
            _growingBuildingsBlinkList = (GrowBuildingBlinkList)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill blink buildings list"))
                _growingBuildingsBlinkList.FillBuildingsList();
        }
    }
}
