using System;
using UnityEngine;

namespace City
{
    [RequireComponent(typeof(CarMovement))]
    public class Car : MonoBehaviour
    {
        [SerializeField, Min(1)] private int _pointToVisitCount = 5;
        [Header("VFX")]
        [SerializeField] private bool _isNeedSpawnEffect;
        [SerializeField] private ParticleSystem _spawnEffect;
        [SerializeField] private bool _isNeedDestroyEffect;
        [SerializeField] private ParticleSystem _destroyEffect;

        private Camera _mainCamera;
        private int _pointsVisited = 0;

        protected CarMovement CarMovement;

        public event Action<Car> Destroyed;

        protected virtual void Awake()
        {
            CarMovement = GetComponent<CarMovement>();
        }

        protected virtual void OnDisable()
        {
            CarMovement.DestinationReached -= OnDestinationReached;
            CarMovement.AllPointsVisited -= OnAllPointsVisited;
        }

        public void SwitchCarPoint(CarPoint newCarPoint)
        {
            if (CarMovement.CurrentCarPoint != null && newCarPoint.CarSpawner != CarMovement.CurrentCarPoint.CarSpawner)
            {
                CarMovement.CurrentCarPoint.CarSpawner.RemoveCar(this);
                newCarPoint.CarSpawner.AddCar(this);
            }
        }

        public void Init(CarPoint carPoint, Camera mainCamera)
        {
            _mainCamera = mainCamera;

            CarMovement.DestinationReached += OnDestinationReached;
            CarMovement.AllPointsVisited += OnAllPointsVisited;
            CarMovement.Init(carPoint);

            TrySpawnEffect(_isNeedSpawnEffect, _spawnEffect);
        }

        private void OnDestinationReached()
        {
            _pointsVisited += 1;

            if (_pointsVisited >= _pointToVisitCount)
            {
                TryDestroyCar();
                return;
            }

            CarMovement.FindNewPoint();
        }

        private void OnAllPointsVisited()
        {
            TryDestroyCar();
        }

        private void TryDestroyCar()
        {
            if (IsOnScreen())
            {
                _pointsVisited = 1;
                CarMovement.ResetMovement();

                return;
            }

            DestroyCar();
        }

        private void DestroyCar()
        {
            Destroyed?.Invoke(this);
            TrySpawnEffect(_isNeedDestroyEffect, _destroyEffect);
            Destroy(gameObject);
        }

        private void TrySpawnEffect(bool isNeedEffect, ParticleSystem effect)
        {
            if (isNeedEffect)
            {
                var spawnedEffect = Instantiate(effect, transform.position, Quaternion.identity);
                var main = spawnedEffect.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
            }
        }

        private bool IsOnScreen()
        {
            var position = _mainCamera.WorldToScreenPoint(transform.position);
            return (position.x > 0f && position.x < Screen.width && position.y > 0f && position.y < Screen.height);
        }
    }
}
