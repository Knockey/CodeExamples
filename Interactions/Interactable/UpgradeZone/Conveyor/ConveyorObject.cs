using Extensions;
using Interactions.Resources;
using System.Collections;
using UnityEngine;

namespace Interactions.UpgradesIsland
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ConveyorObject : MonoBehaviour, IConveyorCollidable, IRefillResource
    {
        [SerializeField, Min(0f)] private float _movementSpeed = 5f;
        [SerializeField, Min(0f)] private float _directionsDiffusionTime = 1f;
        [Header("Initial push")]
        [SerializeField] private Vector2 _initialRotation = new Vector2(-90f, 90f);
        [SerializeField] private Vector3 _pushRotation = new Vector3(45f, 45f, 45f);
        [SerializeField] private float _heightModifier = 0.25f;
        [SerializeField] private AnimationCurve _initialHeighCurve;
        [SerializeField] private AnimationCurve _initialSpeedCurve;
        [SerializeField] private float _animationTime = 1f;
        [Header("Destroy")]
        [SerializeField] private ParticleSystem _destroyParticle;

        private ConveyorSpeedModifier _conveyorSpeedModifier;
        private float _resourceAmount;
        private Rigidbody _rigidbody;
        private float _currentSpeed = 0f;
        private Vector3 _currentEndPosition = new Vector3(-100f, -100f, -100f);

        public float ResourceAmount => _resourceAmount;

        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = false;

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.isKinematic = false;
            _rigidbody.useGravity = true;
        }

        private void Update()
        {
            Move();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Conveyor conveyor))
            {
                SetMoveDirection(conveyor.EndPoint);
                _rigidbody.freezeRotation = true;
            }

            if (other.TryGetComponent(out DestroyTrigger _))
            {
                var particle = Instantiate(_destroyParticle, transform.position, Quaternion.identity);
                var main = particle.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                main.loop = false;

                Destroy(gameObject);
            }
        }

        public void Init(Transform endPoint, float resourceAmount, ConveyorSpeedModifier conveyorSpeedModifier)
        {
            if (resourceAmount < 0f)
                throw new System.ArgumentOutOfRangeException($"{nameof(resourceAmount)} can't be less, than {0}! Now it equals {resourceAmount}!");

            _resourceAmount = resourceAmount;
            _currentEndPosition = endPoint.position.With(y: transform.position.y);
            _conveyorSpeedModifier = conveyorSpeedModifier;

            SetInitialRotation();
            AddInitialForce();
        }

        private void SetInitialRotation()
        {
            var xRotation = Random.Range(_initialRotation.x, _initialRotation.y);
            var yRotation = Random.Range(_initialRotation.x, _initialRotation.y);
            var zRotation = Random.Range(_initialRotation.x, _initialRotation.y);

            transform.rotation = Quaternion.Euler(new Vector3(xRotation, yRotation, zRotation));
        }

        private void AddInitialForce()
        {
            StartCoroutine(PushObject());
        }

        private IEnumerator PushObject()
        {
            var time = 0f;

            while (time < _animationTime)
            {
                yield return null;
                time += Time.deltaTime;

                var height = transform.position.y + _initialHeighCurve.Evaluate(time / _animationTime) * _heightModifier;
                transform.position = transform.position.With(y: height);

                _currentSpeed = _movementSpeed * _initialSpeedCurve.Evaluate(time / _animationTime);

                transform.Rotate(_pushRotation);
            }

            StartCoroutine(MoveTowardsPoint());
        }

        private IEnumerator MoveTowardsPoint()
        {
            while (enabled)
            {
                yield return null;
                Move();
            }
        }

        private void Move()
        {
            var distanceDelta = _currentSpeed * _conveyorSpeedModifier.CurrentMoveSpeedModifier * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _currentEndPosition, distanceDelta);
        }

        private void SetMoveDirection(Transform endPoint)
        {
            var nextEndPosition = endPoint.position.With(y: transform.position.y);
            StartCoroutine(DiffuseDirections(_currentEndPosition, nextEndPosition));
        }

        private IEnumerator DiffuseDirections(Vector3 previousEndPoint, Vector3 nextEndPoint)
        {
            var time = 0f;

            while (time <= _directionsDiffusionTime)
            {
                yield return null;

                time += Time.deltaTime;
                _currentEndPosition = Vector3.Lerp(previousEndPoint, nextEndPoint, time / _directionsDiffusionTime);
            }
        }
    }
}
