using LoadingScreenView;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class DirectionView : MonoBehaviour
    {
        [SerializeField] private ViewPanel _directionView;
        [SerializeField] private Vector2 _screnOffset;

        private Transform _directionPoint;
        private Coroutine _updateTarget;
        private Camera _mainCamera;

        public bool IsEnabled => _directionView.IsEnabled;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void EnableView()
        {
            _directionView.EnableView();
        }

        public void DisableView()
        {
            _directionView.DisableView();
        }

        public void Enable(Transform directionPoint)
        {
            _directionPoint = directionPoint;
            _directionView.EnableView();
            _updateTarget = StartCoroutine(UpdateTarget());
        }

        public void SetDirectionPoint(Transform directionPoint)
        {
            _directionPoint = directionPoint;
        }

        public void Disable()
        {
            if (_updateTarget != null)
                StopCoroutine(_updateTarget);

            _directionPoint = null;
            _directionView.DisableView();
        }

        private IEnumerator UpdateTarget()
        {
            while (enabled)
            {
                yield return null;

                if (_directionPoint != null)
                {
                    var airportScreenPosition = _mainCamera.WorldToScreenPoint(_directionPoint.position);
                    UpdateView(airportScreenPosition);
                }
                else
                {
                    _directionView.DisableView();
                }
            }
        }

        private void UpdateView(Vector3 airportScreenPosition)
        {
            var newPosition = airportScreenPosition;
            newPosition.x = Mathf.Clamp(newPosition.x, _screnOffset.x, Screen.width - _screnOffset.x);
            newPosition.y = Mathf.Clamp(newPosition.y, _screnOffset.y, Screen.height - _screnOffset.y);
            _directionView.transform.position = newPosition;

            if (airportScreenPosition.x > _screnOffset.x && airportScreenPosition.x < Screen.width - _screnOffset.x &&
                airportScreenPosition.y > _screnOffset.y && airportScreenPosition.y < Screen.height - _screnOffset.y)
            {
                if (_directionView.IsEnabled)
                {
                    _directionView.DisableView();
                }

                return;
            }

            if (_directionView.IsEnabled == false)
            {
                _directionView.EnableView();
            }
        }
    }
}
