using BarUI;
using Extensions;
using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class ZoneViewDisable : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _valueChangerBehaviour;
        [SerializeField] private List<ViewPanel> _objectsToDisable;

        private IValueChanger _valueChanger => (IValueChanger)_valueChangerBehaviour;

        private void OnValidate()
        {
            GenericInterfaceInjection.TrySetObject<IValueChanger>(ref _valueChangerBehaviour);
        }

        private void OnEnable()
        {
            _valueChanger.ValueInited += OnValueChanged;
            _valueChanger.ValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _valueChanger.ValueInited -= OnValueChanged;
            _valueChanger.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float value, float maxValue)
        {
            if (value >= maxValue)
                DisableView();
        }

        private void DisableView()
        {
            _objectsToDisable.ForEach(obj => obj.DisableView());
        }
    }
}
