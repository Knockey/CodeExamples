using UnityEditor;
using UnityEngine;

namespace Interactions.EditorExtensions
{
    [CustomEditor(typeof(PropsEnable))]
    public class PropsEnableEditor : Editor
    {
        private PropsEnable _propsEnable;

        private void OnEnable()
        {
            _propsEnable = (PropsEnable)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill props"))
                _propsEnable.FillBuildingsList();
        }
    }
}
