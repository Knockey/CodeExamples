using UnityEditor;
using UnityEngine;

namespace Interactions.EditorExtensions
{
    [CustomEditor(typeof(GrowBuildingBlink))]
    public class GrowBuildingBlinkEditor : Editor
    {
        private GrowBuildingBlink _growingBuildingsBlink;

        private void OnEnable()
        {
            _growingBuildingsBlink = (GrowBuildingBlink)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill renderers list"))
                _growingBuildingsBlink.FillRenderedsList();
        }
    }
}
