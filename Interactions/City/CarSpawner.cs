using Interactions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace City
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _carSpawnTime = 3f;
        [SerializeField] private GrowBuildingDistrict _growBuildingsDistrict;
        [SerializeField] private List<Car> _cars;
        [SerializeField] private List<CarPoint> _carPoints;
        [Header("Cars amount formula")]
        [SerializeField, Min(0f)] private float _minPercent = 0.33f;
        [SerializeField] private int _maxCarsAmount = 4;

        private List<Car> _spawnedCars;
        private bool _isActive = false;
        private Camera _mainCamera;

        private void Awake()
        {
            _spawnedCars = new List<Car>();
            _mainCamera = Camera.main;
        }

        private void OnDisable()
        {
            _spawnedCars.ForEach(car => car.Destroyed -= OnCarDestroyed);
        }

        private void Start()
        {
            StartCoroutine(SpawnCars());
        }

        public void AddCar(Car car)
        {
            _spawnedCars.Add(car);
            car.Destroyed += OnCarDestroyed;
        }

        public void RemoveCar(Car car)
        {
            car.Destroyed -= OnCarDestroyed;
            _spawnedCars.Remove(car);
        }

        private IEnumerator SpawnCars()
        {
            var tick = new WaitForSeconds(_carSpawnTime);

            while (enabled)
            {
                yield return tick;

                var carsCount = GetCurrentCarsCount();
                TryActivatePoints();

                if (_spawnedCars.Count < carsCount)
                {
                    SpawnCar();
                }
            }
        }

        private void TryActivatePoints()
        {
            if (_isActive == false && _growBuildingsDistrict.CurrentCompletionPercent >= _minPercent)
            {
                _isActive = true;
                _carPoints.ForEach(carPoint => carPoint.ActivatePoint(this));
            }
        }

        private int GetCurrentCarsCount()
        {
            if (_growBuildingsDistrict.CurrentCompletionPercent <= _minPercent)
                return 0;

            return (int)(_growBuildingsDistrict.CurrentCompletionPercent * _maxCarsAmount);
        }

        private void SpawnCar()
        {
            var carPoints = new List<CarPoint>();
            _carPoints.ForEach(carPoint =>
            {
                if (carPoint.IsOnScreen() == false && carPoint.IsActive)
                    carPoints.Add(carPoint);
            });

            if (carPoints == null || carPoints.Count <= 0)
                return;

            var pointID = Random.Range(0, carPoints.Count);
            var carPoint = carPoints[pointID];

            var carID = Random.Range(0, _cars.Count);
            var car = Instantiate(_cars[carID], carPoint.transform.position, Quaternion.identity);
            car.Init(carPoint, _mainCamera);
            AddCar(car);
        }

        private void OnCarDestroyed(Car car)
        {
            car.Destroyed -= OnCarDestroyed;
            _spawnedCars.Remove(car);
        }

#if UNITY_EDITOR
        public void FillCarPointsList()
        {
            _carPoints = GetComponentsInChildren<CarPoint>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
