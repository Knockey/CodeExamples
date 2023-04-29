using Extensions;
using UnityEngine;

namespace Airplane
{
    public class RotatingPoint : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _rotationSpeed = 100f;
        [SerializeField] private Transform _center;

        private float _radius;

        private void Awake()
        {
            _radius = transform.localPosition.z;
        }

        public void RotateAround()
        {
            transform.RotateAround(_center.position.With(y: transform.position.y), Vector3.up, _rotationSpeed * Time.deltaTime);
        }

        public void KeepDistanceFrom(Transform target)
        {
            var startPosition = target.localPosition.With(y: transform.position.y);
            var centerPosition = _center.localPosition.With(y: transform.position.y);
            var direction = (centerPosition- startPosition).normalized;

            Ray ray = new Ray(startPosition, direction);
            var newPosition = ray.GetPoint(_radius);
            transform.position = newPosition;
        }
    }
}
