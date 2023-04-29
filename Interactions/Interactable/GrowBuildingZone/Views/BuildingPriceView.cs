using BarUI;
using Extensions;
using UnityEngine;

namespace Interactions
{
    public class BuildingPriceView : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _valueChangerBehaviour;
        [Header("UI")]
        [SerializeField] private TMPro.TMP_Text _valueText;
        [SerializeField] private string _1kSuffix = "K";
        [SerializeField] private string _textPrefix = "";
        [SerializeField] private string _textSuffix = "$";

        private IValueChanger _valueChanger => (IValueChanger)_valueChangerBehaviour;

        private void OnValidate()
        {
            GenericInterfaceInjection.TrySetObject<IValueChanger>(ref _valueChangerBehaviour);
        }

        private void OnEnable()
        {
            _valueChanger.ValueInited += OnValueChanged;
            _valueChanger.ValueChanged += OnValueChanged;
            OnValueChanged(0f, 0f);

        }

        private void OnDisable()
        {
            _valueChanger.ValueInited -= OnValueChanged;
            _valueChanger.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float value, float maxValue)
        {
            if (_valueChanger is InteractableZone interactableZone)
            {
                var currentValue = (int)interactableZone.ResourceLeftToSpent;
                var currentValueSuffix = "";
                var divider = 1000;

                while (currentValue >= divider)
                {
                    currentValue /= divider;
                    currentValueSuffix += _1kSuffix;
                }

                _valueText.text = _textPrefix + currentValue + currentValueSuffix + _textSuffix;
            }
        }
    }
}
