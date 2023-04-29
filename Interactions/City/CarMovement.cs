using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace City
{
    [RequireComponent(typeof(Car))]
    public class CarMovement : MonoBehaviour
    {
        private const float MinDistance = 0.01f;

        [SerializeField, Min(0f)] private float _movementSpeed = 5f;

        private Car _car;
        private Coroutine _movement;
        private List<CarPoint> _visitedPoints;

        public CarPoint CurrentCarPoint { get; private set; }

        public event Action DestinationReached;
        public event Action AllPointsVisited;

        private void Awake()
        {
            _visitedPoints = new List<CarPoint>();
            _car = GetComponent<Car>();
        }

        public void Init(CarPoint carPoint)
        {
            _visitedPoints.Clear();
            RestartMovement(carPoint);
        }

        public void ResetMovement()
        {
            _visitedPoints.Clear();

            RestartMovement(CurrentCarPoint);
        }

        public void FindNewPoint()
        {
            RestartMovement(CurrentCarPoint);
        }

        public void StartMovement()
        {
            _movement = StartCoroutine(MoveTowardsPoint());
        }

        public void StopMovement()
        {
            if (_movement != null)
                StopCoroutine(_movement);
        }

        private void RestartMovement(CarPoint carPoint)
        {
            StopMovement();

            var newCarPoint = carPoint.GetLinkedPoint(_visitedPoints);

            if (newCarPoint == null)
            {
                AllPointsVisited?.Invoke();
                return;
            }

            _car.SwitchCarPoint(newCarPoint);

            CurrentCarPoint = newCarPoint;
            _visitedPoints.Add(carPoint);
            _movement = StartCoroutine(MoveTowardsPoint());
        }

        private IEnumerator MoveTowardsPoint()
        {
            var newPosition = CurrentCarPoint.GetPosition(transform.position);

            while (transform.position.DistanceTo(newPosition) > MinDistance)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, newPosition, _movementSpeed * Time.deltaTime);
                transform.LookAt(newPosition);
            }

            DestinationReached?.Invoke();
        }
    }
}
