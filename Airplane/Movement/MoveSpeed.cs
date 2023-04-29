using Interactions.Resources;
using System.Collections;
using UnityEngine;

namespace Airplane
{
    [RequireComponent(typeof(Collider))]
    public class MoveSpeed : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _citySpeed = 7f;
        [SerializeField, Min(0f)] private float _waterSpeed = 11f;
        [SerializeField, Min(0f)] private float _switchTime = 1f;
        [Header("Empty airplane settings")]
        [SerializeField] private InteractionResource _interactionResource;
        [SerializeField, Min(0f)] private float _resourcePercent = 0.15f;
        [SerializeField, Min(0f)] private float _emptySpeed = 40f;

        private Coroutine _switchCoroutine;
        private bool _isOnWater = true;
        private bool _isOnEmptySpeed = false;

        public float CurrentSpeed { get; private set; }

        private void Awake()
        {
            CurrentSpeed = _waterSpeed;
            _isOnWater = true;

            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnEnable()
        {
            _interactionResource.ValueChanged += OnValueChanged;
        }

        private void OnDisable()
        {
            _interactionResource.ValueChanged -= OnValueChanged;
        }

        public void ChangeCitySpeed(float speed)
        {
            ChangeSpeed(speed, ref _citySpeed);
        }

        public void ChangeWaterSpeed(float speed)
        {
            ChangeSpeed(speed, ref _waterSpeed);
        }

        public void SwitchSpeed()
        {
            if (_isOnEmptySpeed)
                return;

            if (_switchCoroutine != null)
                StopCoroutine(_switchCoroutine);

            var newSpeed = CurrentSpeed <= _citySpeed ? _waterSpeed : _citySpeed;
            _isOnWater = !_isOnWater;
            _switchCoroutine = StartCoroutine(SwitchSpeedLinear(newSpeed));
        }

        private void ChangeSpeed(float speed, ref float serializedSpeed)
        {
            serializedSpeed = speed;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (_isOnWater)
            {
                CurrentSpeed = _waterSpeed;
            }
            else
            {
                CurrentSpeed = _citySpeed;
            }
#endif
        }

        private void OnValueChanged(float value, float maxValue)
        {
            var percent = value / maxValue;

            if (percent > _resourcePercent && _isOnEmptySpeed == true)
            {
                _isOnEmptySpeed= false;
                CurrentSpeed = _waterSpeed;
            }

            if (percent <= _resourcePercent && _isOnEmptySpeed == false)
            {
                _isOnEmptySpeed = true;
                CurrentSpeed = _emptySpeed;
            }
        }

        private IEnumerator SwitchSpeedLinear(float newSpeed)
        {
            var currentSpeed = CurrentSpeed;
            var time = 0f;

            while (time <= _switchTime)
            {
                yield return null;
                CurrentSpeed = Mathf.Lerp(currentSpeed, newSpeed, time / _switchTime);
                time += Time.deltaTime;
            }

            CurrentSpeed = newSpeed;
        }
    }
}
