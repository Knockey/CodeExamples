using UnityEditor;
using UnityEngine;

namespace Interactions.EditorExtensions
{
    [CustomEditor(typeof(GrowingBuildingsList))]
    public class GrowingBuildingsListEditor : Editor
    {
        private GrowingBuildingsList _growingBuildingsList;

        private void OnEnable()
        {
            _growingBuildingsList = (GrowingBuildingsList)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill buildings list"))
                _growingBuildingsList.FillBuildingsList();
        }
    }
}
