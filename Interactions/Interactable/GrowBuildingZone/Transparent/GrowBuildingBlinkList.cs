using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions
{
    public class GrowBuildingBlinkList : MonoBehaviour
    {
        [SerializeField] private bool _isNeedToBlink = true;
        [SerializeField] private List<GrowBuildingBlink> _blinkBuildingsList;

        private GrowBuildingBlink _currentBlinking;

        public void DisableAll()
        {
            _blinkBuildingsList.ForEach(blinking => blinking.DisableObject());
        }

        public void TryDisableCurrent()
        {
            _currentBlinking?.DisableObject();
        }

        public void EnableAll()
        {
            _blinkBuildingsList.ForEach(blinking => blinking.EnableObject());
        }

        public void TryEnableByIndex(int index)
        {
            if (index >= _blinkBuildingsList.Count)
                return;

            _blinkBuildingsList[index].EnableObject();
            _currentBlinking = _blinkBuildingsList[index];
        }

        public void EnableBlink()
        {
            _currentBlinking.TryEnableBlink(_isNeedToBlink);
        }

        public void DisableBlink()
        {
            _currentBlinking.TryDisableBlink();
        }

#if UNITY_EDITOR
        public void FillBuildingsList()
        {
            _blinkBuildingsList = GetComponentsInChildren<GrowBuildingBlink>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
