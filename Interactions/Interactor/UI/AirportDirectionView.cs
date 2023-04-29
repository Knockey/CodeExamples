using LoadingScreenView;
using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(DirectionView))]
    public class AirportDirectionView : ViewPanel
    {
        [SerializeField] private Transform _airportPoint;

        private DirectionView _directionView;

        private void Awake()
        {
            _directionView = GetComponent<DirectionView>();
        }

        private void OnEnable()
        {
            _directionView.Enable(_airportPoint);
        }

        private void OnDisable()
        {
            _directionView.Disable();
        }
    }
}
