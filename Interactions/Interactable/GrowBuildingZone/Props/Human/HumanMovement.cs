using DG.Tweening;
using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Humans
{
    public class HumanMovement : MonoBehaviour
    {
        private const float MinDistance = 0.1f;

        [SerializeField, Min(0f)] private float _movementSpeed = 5f;
        [SerializeField, Min(0f)] private float _rotationTime = 0.5f;
        [SerializeField] private List<Transform> _movementPoints;

        private int _currentPointIndex = 0;

        private void OnEnable()
        {
            StartMovement();
        }

        private void StartMovement()
        {
            StartCoroutine(MoveTowardsPoint());
        }

        private IEnumerator MoveTowardsPoint()
        {
            var currentPointPosition = _movementPoints[_currentPointIndex].position.With(y: transform.position.y);
            transform.DOLookAt(currentPointPosition, _rotationTime);

            while (enabled)
            {
                transform.position = Vector3.MoveTowards(transform.position, currentPointPosition, _movementSpeed * Time.deltaTime);

                if (transform.position.DistanceTo(currentPointPosition) <= MinDistance)
                {
                    _currentPointIndex = _currentPointIndex < _movementPoints.Count - 1 ? _currentPointIndex + 1 : 0;
                    currentPointPosition = _movementPoints[_currentPointIndex].position.With(y: transform.position.y);

                    transform.DOLookAt(currentPointPosition, _rotationTime);
                }

                yield return null;
            }
        }
    }
}
