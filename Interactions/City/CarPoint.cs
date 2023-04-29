using Extensions;
using Interactions;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace City
{
    public class CarPoint : MonoBehaviour
    {
        [SerializeField] private List<CarPoint> _linkedPoints;
        [SerializeField] private Color _linkedPointsColor = Color.yellow;
        [SerializeField] private GrowBuildingsZone _linkedRoad;

        private Camera _mainCamera;
        private bool _isActive = false;

        public CarSpawner CarSpawner { get; private set; }
        [Serialize]
        public bool IsActive => _isActive && _linkedRoad.IsCompleted;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void ActivatePoint(CarSpawner spanwer)
        {
            _isActive = true;
            CarSpawner = spanwer;
        }

        public Vector3 GetPosition(Vector3 user)
        {
            return transform.position.With(y: user.y);
        }

        public bool IsOnScreen()
        {
            var position = _mainCamera.WorldToScreenPoint(transform.position);
            return (position.x > 0f && position.x < Screen.width && position.y > 0f && position.y < Screen.height);
        }

        public CarPoint GetLinkedPoint(List<CarPoint> visitedPoints)
        {
            List<CarPoint> unvisitedPoints = new List<CarPoint>();
            _linkedPoints.ForEach(carPoint =>
            {
                if (visitedPoints.Contains(carPoint) == false && carPoint.IsActive)
                {
                    unvisitedPoints.Add(carPoint);
                }
            });

            if (unvisitedPoints.Count == 0)
            {
                return null;
            }

            var randomID = Random.Range(0, unvisitedPoints.Count);
            return unvisitedPoints[randomID];
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_linkedPoints.Count > 0)
            {
                foreach (var point in _linkedPoints)
                {
                    Debug.DrawLine(transform.position, point.GetPosition(transform.position), _linkedPointsColor);
                }
            }

            var color = _linkedRoad == null || (_linkedRoad != null && _linkedRoad.transform.position.DistanceTo(transform.position) > 2f) ? Color.red : Color.green;

            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
#endif
    }
}
