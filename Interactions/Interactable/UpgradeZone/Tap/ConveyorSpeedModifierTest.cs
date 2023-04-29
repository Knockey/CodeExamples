using Extensions;
using LoadingScreenView;
using UnityEngine;
using UnityEngine.UI;

namespace Interactions
{
    public class ConveyorSpeedModifierTest : ViewPanel
    {
        [SerializeField] private ConveyorSpeedModifier _conveyorSpeedModifier;
        [Header("Borders")]
        [SerializeField, Min(0f)] private float _spawnSpeedMinBorder = 1f;
        [SerializeField, Min(0f)] private float _spawnSpeedMaxBorder = 10f;
        [SerializeField, Min(0f)] private float _moveSpeedMinBorder = 1f;
        [SerializeField, Min(0f)] private float _moveSpeedMaxBorder = 10f;
        [Header("Toggles")]
        [SerializeField] private Toggle _spawnSpeedToggle;
        [SerializeField] private Toggle _moveSpeedToggle;
        [Header("Sliders")]
        [SerializeField] private Slider _spawnSpeedSlider;
        [SerializeField] private Slider _moveSpeedSlider;
        [Header("Text fields")]
        [SerializeField] private TextView _spawnSpeedText;
        [SerializeField] private TextView _moveSpeedText;

        private void OnEnable()
        {
            _spawnSpeedToggle.onValueChanged.AddListener(_ => OnUIChanged());
            _moveSpeedToggle.onValueChanged.AddListener(_ => OnUIChanged());
            _spawnSpeedSlider.onValueChanged.AddListener(_ => OnUIChanged());
            _moveSpeedSlider.onValueChanged.AddListener(_ => OnUIChanged());

            _spawnSpeedSlider.value = 0.115f;
            _moveSpeedSlider.value = 0.115f;
        }

        private void OnDisable()
        {
            _spawnSpeedToggle.onValueChanged.RemoveListener(_ => OnUIChanged());
            _moveSpeedToggle.onValueChanged.RemoveListener(_ => OnUIChanged());
            _spawnSpeedSlider.onValueChanged.RemoveListener(_ => OnUIChanged());
            _moveSpeedSlider.onValueChanged.RemoveListener(_ => OnUIChanged());
        }

        private void OnUIChanged()
        {
            var spawnSpeed = Mathf.Lerp(_spawnSpeedMinBorder, _spawnSpeedMaxBorder, _spawnSpeedSlider.value);
            var moveSpeed = Mathf.Lerp(_moveSpeedMinBorder, _moveSpeedMaxBorder, _moveSpeedSlider.value);

            //_conveyorSpeedModifier.SetParams(spawnSpeed, moveSpeed, _spawnSpeedToggle.isOn, _moveSpeedToggle.isOn);
            _spawnSpeedText.SetTextFormated(spawnSpeed.ToString());
            _moveSpeedText.SetTextFormated(moveSpeed.ToString());
        }
    }
}
