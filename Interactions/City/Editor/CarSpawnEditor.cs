using UnityEditor;
using UnityEngine;

namespace City
{
    [CustomEditor(typeof(CarSpawner))]
    public class CarSpawnEditor : Editor
{
        private CarSpawner _carSpawner;

        private void OnEnable()
        {
            _carSpawner = (CarSpawner)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill car points list"))
                _carSpawner.FillCarPointsList();
        }
    }
}
