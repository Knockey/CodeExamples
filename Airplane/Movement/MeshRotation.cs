using DG.Tweening;
using Extensions;
using UnityEngine;

namespace Airplane
{
    public class MeshRotation : MonoBehaviour
    {
        [SerializeField] private Transform _meshToRotate;
        [SerializeField, Min(0f)] private float _rotationAngle = 30f;
        [SerializeField, Min(0f)] private float _angleSwitchTime = 1f;
        [SerializeField, Min(0.1f)] private float _anglePrecisionModifier = 0.5f;

        private float _previousRotation;
        private float _currentAngle;
        private Tween _angleSwitch;

        private void Awake()
        {
            _previousRotation = transform.localRotation.eulerAngles.y;
            _currentAngle = 0f;
        }

        private void OnDisable()
        {
            _angleSwitch?.Kill();
        }

        public void RotateMesh(float interpolationValue)
        {
            var rotationAngle = _meshToRotate.localRotation.eulerAngles.With(z: _rotationAngle * interpolationValue * -1f);
            _meshToRotate.localRotation = Quaternion.Euler(rotationAngle);
        }

        public void RotateByCurrentAngle(int? diff = null)
        {
            var currentDiff = diff ?? (transform.localRotation.eulerAngles.y - _previousRotation) * _anglePrecisionModifier;
            var newAngleZ = GetCurrentAngle((int)currentDiff);
            _previousRotation = transform.localRotation.eulerAngles.y;

            if (newAngleZ == _currentAngle)
                return;

            _currentAngle = newAngleZ;
            _angleSwitch?.Kill();
            var newRotation = new Vector3(0f, 0f, newAngleZ);
            _angleSwitch = _meshToRotate.DOLocalRotate(newRotation, _angleSwitchTime);
        }

        private float GetCurrentAngle(int currentDiff)
        {
            if (currentDiff > 0f)
                return -_rotationAngle; 
            
            if (currentDiff < 0f)
                return _rotationAngle;

            return 0f;
        }
    }
}
