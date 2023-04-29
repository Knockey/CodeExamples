using BarUI;
using UnityEngine;

namespace Interactions
{
    public class ResourceAmountView : BarView
    {
        [SerializeField] private ResourceAmountText _resourceAmountText;

        protected override void OnValueInited(float value, float maxValue)
        {
            base.OnValueInited(value, maxValue);
            _resourceAmountText.SetAmountText(value);
        }

        protected override void OnValueChanged(float value, float maxValue)
        {
            base.OnValueChanged(value, maxValue);
            _resourceAmountText.SetAmountText(value);
        }
    }
}
