using Extensions;
using GameState;
using LoadingScreenView;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(DirectionView))]
    public class NeedToBuildDirectionView : ViewPanel
    {
        [SerializeField] private Transform _playerMovement;
        [SerializeField, Min(0f)] private float _updateTime = 0.25f;

        private Coroutine _search;
        private DirectionView _directionView;
        private LevelProgressionHandler _levelProgressionHandler;

        private void Awake()
        {
            _directionView = GetComponent<DirectionView>();
        }

        private void Start()
        {
            if (_directionView.IsEnabled)
            {
                Disable();
                Enable();
            }
        }

        public void Init(LevelProgressionHandler levelProgressionHandler)
        {
            _levelProgressionHandler = levelProgressionHandler;
        }

        public override void EnableView()
        {
            base.EnableView();
            Enable();
        }

        public override void DisableView()
        {
            Disable();
            base.DisableView();
        }

        public void Enable()
        {
            Transform closestTransform = GetClosestTransform();
            _directionView.Enable(closestTransform);

            _search = StartCoroutine(SearchClosestTarget());
        }

        public void Disable()
        {
            if (_search != null)
                StopCoroutine(_search);

            _directionView.Disable();
        }

        private IEnumerator SearchClosestTarget()
        {
            var tick = new WaitForSeconds(_updateTime);

            while (enabled)
            {
                yield return tick;

                Transform closestTransform = GetClosestTransform();

                if (closestTransform != null)
                {
                    _directionView.SetDirectionPoint(closestTransform);
                }
            }
        }

        private Transform GetClosestTransform()
        {
            Transform closestTransform = null;
            var closestDistance = float.MaxValue;

            _levelProgressionHandler.Interactables.ForEach(interactable =>
            {
                var interactablePosition = interactable.Transform.position.With(y: _playerMovement.position.y);
                var newDistance = interactablePosition.DistanceTo(_playerMovement.position);

                if (newDistance < closestDistance && interactable.IsCompleted == false && interactable.IsAbleToInteract)
                {
                    closestDistance = newDistance;
                    closestTransform = interactable.Transform;
                }
            });

            return closestTransform;
        }
    }
}
