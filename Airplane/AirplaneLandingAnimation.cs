using DG.Tweening;
using Extensions;
using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;

namespace Airplane
{
    public class AirplaneLandingAnimation : MonoBehaviour, ILander
    {
        [SerializeField] private bool _isLanded = true;
        [SerializeField] private Transform _mesh;
        [SerializeField] private Transform _rotationMesh;
        [SerializeField] private float _landedHeight = 1f;
        [SerializeField, Min(0f)] private float _animationTime = 2f;

        private float _defaultHeight = 11f;
        private Vector3 _defaultPositionLanded;
        private Sequence _moveSequence;

        public Transform Transform => transform;
        public float AnimationTime => _animationTime;

        private void Awake()
        {
            _defaultHeight = _mesh.localPosition.y;
            _defaultPositionLanded = _mesh.localPosition.With(y: _landedHeight);
        }

        private void Start()
        {
            if (_isLanded)
            {
                _mesh.localPosition = _mesh.localPosition.With(y: _landedHeight);
                TakeOff();
            }
        }

        public void Land(Transform landingPoint)
        {
            _moveSequence?.Kill();
            _moveSequence = DOTween.Sequence();

            _moveSequence.Insert(0, transform.DOMove(landingPoint.position, AnimationTime));
            _moveSequence.Insert(0, _mesh.DOLocalMove(_defaultPositionLanded, AnimationTime));
            _moveSequence.Insert(0, _mesh.DOLookAt(landingPoint.position.With(y: _mesh.transform.position.y), AnimationTime));
            _moveSequence.Insert(0, _rotationMesh.DORotate(Vector3.zero, AnimationTime));
        }

        public void TakeOff()
        {
            _moveSequence?.Kill();
            _moveSequence = DOTween.Sequence();

            _moveSequence.Insert(0, _mesh.DOLocalMoveY(_defaultHeight, AnimationTime));
        }
    }
}
