using BarUI;
using LoadingScreenView;
using UnityEngine;

namespace Interactions
{
    public class LowResourceView : ViewPanel
    {
        [SerializeField, Min(0f)] private float _lowAmountBorder = 0.35f;
        [SerializeField, Min(0f)] private float _noResourceBorder = 0.05f;
        [SerializeField] private MonoBehaviour _interactionResourceBehaviour;
        [SerializeField] private ViewPanel _directionView;
        [SerializeField] private ViewPanel _noResourceView;
        [SerializeField] private NeedToBuildDirectionView _needToBuildResourceView;

        protected IValueChanger InteractionResource => (IValueChanger)_interactionResourceBehaviour;

        private void OnValidate()
        {
            if (_interactionResourceBehaviour == null)
                return;

            if (_interactionResourceBehaviour is IValueChanger)
                return;

            Debug.LogWarning(_interactionResourceBehaviour.name + " needs to implement " + nameof(IValueChanger));
            _interactionResourceBehaviour = null;
        }

        private void Awake()
        {
            _directionView.DisableView();
            _noResourceView.DisableView();
        }

        private void OnEnable()
        {
            InteractionResource.ValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            InteractionResource.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(float value, float maxValue)
        {
            var percent = value / maxValue;
            SetViewState(percent, _lowAmountBorder, _directionView);
            SetViewState(percent, _noResourceBorder, _noResourceView);

            if (percent < _lowAmountBorder && _needToBuildResourceView.IsEnabled)
            {
                _needToBuildResourceView.Disable();
            }
        }

        private void SetViewState(float percent, float border, ViewPanel viewPanel)
        {
            if (percent < border && viewPanel.IsEnabled == false)
            {
                viewPanel.EnableView();
            }

            if (percent >= border && viewPanel.IsEnabled)
            {
                viewPanel.DisableView();
            }
        }
    }
}
