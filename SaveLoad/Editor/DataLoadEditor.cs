using UnityEditor;
using UnityEngine;

namespace SaveLoad
{
    [CustomEditor(typeof(DataLoad))]
    public class DataLoadEditor : Editor
    {
        private DataLoad _sceneLoad;

        private void OnEnable()
        {
            _sceneLoad = (DataLoad)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill savables list"))
                _sceneLoad.FillBuildingsList();
        }
    }
}
