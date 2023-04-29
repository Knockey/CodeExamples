using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions
{
    public class PropsEnable : MonoBehaviour
    {
        [SerializeField] private List<Prop> _propsEnableOnComplete;

        public void EnableProps()
        {
            if (_propsEnableOnComplete != null && _propsEnableOnComplete.Count > 0)
            {
                _propsEnableOnComplete.ForEach(prop => prop.Enable());
            }
        }

        public void DisableProps()
        {
            if (_propsEnableOnComplete != null && _propsEnableOnComplete.Count > 0)
            {
                _propsEnableOnComplete.ForEach(prop => prop.Disable());
            }
        }

#if UNITY_EDITOR
        public void FillBuildingsList()
        {
            _propsEnableOnComplete = GetComponentsInChildren<Prop>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
