using Extensions;
using UnityEngine;

namespace Airplane
{
    public class AirplaneFlyAnimation : MonoBehaviour
    {
        [SerializeField] private MeshRotation _meshRotation;
        [SerializeField] private float _rotationAngle = 100f;
        [Header("Hoover")]
        [SerializeField, Min(0f)] private float _minDistanceToRotate = 0.5f;
        [SerializeField, Min(0f)] private float _hooverSpeedModifier = 0.7f;
        [SerializeField] private RotatingPoint _rotatingPoint;
        [Header("Speed change")]
        [SerializeField, Min(0f)] private float _minSpeedDistance = 10f;
        [SerializeField, Min(0f)] private float _minSpeedPercentModifier = 0.95f;
        [SerializeField, Min(0f)] private float _maxSpeedDistance = 100f;
        [SerializeField, Min(0f)] private float _maxSpeedPercentModifier = 1.1f;

        private bool _isRotating = false;

        public void UpdateMeshTransform(float movementSpeed, Joystick stick)
        {
            var distance = transform.position.With(y: 0f).DistanceTo(_meshRotation.transform.position.With(y: 0f));

            if (stick.Direction != Vector2.zero || (_isRotating == false && distance > _minDistanceToRotate))
            {
                _isRotating = false;

                LookAtTarget(transform);
                _meshRotation.RotateByCurrentAngle();

                var currentSpeed = ClampSpeed(movementSpeed);
                MoveMesh(currentSpeed);

                _rotatingPoint.KeepDistanceFrom(_meshRotation.transform);

                return;
            }

            _isRotating = true;
            _rotatingPoint.RotateAround();
            _meshRotation.RotateByCurrentAngle(diff: 10);

            LookAtTarget(_rotatingPoint.transform);
            MoveMesh(movementSpeed * _hooverSpeedModifier);
        }

        private void LookAtTarget(Transform target)
        {
            var lookPoint = target.position.With(y: _meshRotation.transform.position.y);
            var lookDirection = lookPoint - _meshRotation.transform.position;
            var lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            _meshRotation.transform.rotation = Quaternion.RotateTowards(_meshRotation.transform.rotation, lookRotation, _rotationAngle * Time.deltaTime);
        }

        private float ClampSpeed(float movementSpeed)
        {
            var distance = transform.position.With(y: 0f).DistanceTo(_meshRotation.transform.position.With(y: 0f));
            var distanceClamped = Mathf.Clamp(distance, _minSpeedDistance, _maxSpeedDistance);

            if (distanceClamped <= _minSpeedDistance)
                return _minSpeedPercentModifier * movementSpeed;

            var modifier = Mathf.Lerp(_minSpeedPercentModifier, _maxSpeedPercentModifier, distanceClamped / _maxSpeedDistance);

            return modifier * movementSpeed;
        }

        private void MoveMesh(float movementSpeed)
        {
            var direction = movementSpeed * Time.deltaTime * Vector3.forward;
            _meshRotation.transform.Translate(direction);
        }
    }
}
