using UnityEngine;

namespace Interactions
{
    public class ResourceAmountText : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _valueText;
        [SerializeField] private string _1kSuffix = "K";
        [SerializeField] private string _textPrefix = "";
        [SerializeField] private string _textSuffix = "$";

        public void SetAmountText(float value)
        {
            var currentValue = (int)value;
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
