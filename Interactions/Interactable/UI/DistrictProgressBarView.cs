using BarUI;
using LoadingScreenView;
using UnityEngine;

namespace Interactions
{
    public class DistrictProgressBarView : BarView
    {
        [Header("District view")]
        [SerializeField] private ViewPanel _incompleteDistrictView;
        [SerializeField] private ViewPanel _completeDistrictView;

        private void Awake()
        {
            _completeDistrictView.DisableView();
            _incompleteDistrictView.EnableView();
        }

        protected override void OnValueInited(float value, float maxValue)
        {
            base.OnValueInited(value, maxValue);
            TrySetIcon(value, maxValue);
        }

        protected override void OnValueChanged(float value, float maxValue)
        {
            base.OnValueChanged(value, maxValue);
            TrySetIcon(value, maxValue);
        }

        private void TrySetIcon(float value, float maxValue)
        {
            if (value / maxValue >= 1f)
            {
                _completeDistrictView.EnableView();
                _incompleteDistrictView.DisableView();
                return;
            }

            _completeDistrictView.DisableView();
            _incompleteDistrictView.EnableView();
        }
    }
}
