using BarUI;

namespace Interactions
{
    public class ZoneFillView : BarView
    {
        protected override void OnValueInited(float value, float maxValue)
        {
            base.OnValueInited(value, maxValue);
            TrySetAmount();
        }

        protected override void OnValueChanged(float value, float maxValue)
        {
            TrySetAmount();
        }

        private void TrySetAmount()
        {
            if (BarChanger is InteractableZone interactableZone)
                Image.fillAmount = interactableZone.ResourceLeftToSpentPercent;
        }
    }
}
