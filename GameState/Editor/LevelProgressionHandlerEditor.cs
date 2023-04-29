using UnityEditor;
using UnityEngine;

namespace GameState.EditorExtensions
{
    [CustomEditor(typeof(LevelProgressionHandler))]
    public class LevelProgressionHandlerEditor : Editor
    {
        private LevelProgressionHandler _levelProgressionHangler;

        private void OnEnable()
        {
            _levelProgressionHangler = (LevelProgressionHandler)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Fill interactables list"))
                _levelProgressionHangler.FillInteractablesList();

            if (GUILayout.Button("Fill generators list"))
                _levelProgressionHangler.FillGeneratorsLists();
        }
    }
}
