using UnityEditor;
using UnityEngine;

namespace Interactions.EditorExtensions
{
    [CustomEditor(typeof(GrowBuildingDistrict))]
    public class GrowBuildingDistrictEditor : Editor
    {
        private GrowBuildingDistrict _growingBuildingsDistrict;

        private void OnEnable()
        {
            _growingBuildingsDistrict = (GrowBuildingDistrict)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill interactables list"))
                _growingBuildingsDistrict.FillInteractablesList();

            if (GUILayout.Button("Fill transparent list"))
                _growingBuildingsDistrict.FillTransparentObjectsLists(); 
            
            if (GUILayout.Button("Fill generators list"))
                _growingBuildingsDistrict.FillGeneratorsLists();
        }
    }
}
