using BarUI;
using System;
using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(Collider))]
    public abstract class InteractableZone : MonoBehaviour, IInteractable, IValueChanger
    {
        private const int DefaultObjectsCount = -1;
        protected const int MaxCompletionPercent = 1;

        [SerializeField, Min(0f)] private float _fullObjectCost = 100f;
        [Header("Interaction settings")]
        [SerializeField] private bool _isAlwaysInteractable = false;

        private bool _isAbleToInteract = true;
        private int _objectsCount = 0;
        private int _maxObjectsCount = DefaultObjectsCount;
        private float _currentResouceGathered = 0f;
        private Collider _collider;

        protected float _nextObjectCost = 100f;
        protected int CurrentInteractionsCount => _objectsCount;

        public bool IsCompleted => CurrentCompletionPercent >= MaxCompletionPercent;
        public float CurrentCompletionPercent => _objectsCount / (float)_maxObjectsCount;
        public bool IsAbleToInteract => _isAlwaysInteractable || (CurrentCompletionPercent < MaxCompletionPercent && _isAbleToInteract);
        public float ResourceToSpent => _nextObjectCost;
        public float ResourceLeftToSpent => _nextObjectCost * (_maxObjectsCount - _objectsCount) - _currentResouceGathered;
        public float ResourceLeftToSpentPercent => 1 - (ResourceLeftToSpent / (_nextObjectCost * _maxObjectsCount));
        public Transform Transform => transform;

        public event Action<float, float> ValueInited;
        public event Action<float, float> ValueChanged;
        public event Action InteractionPercentUpdated;

        protected virtual void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        protected virtual void Start()
        {
            if (_maxObjectsCount == DefaultObjectsCount)
                throw new ArgumentOutOfRangeException($"{nameof(_maxObjectsCount)} not set! Invoke {nameof(SetMaxObjectsCount)} before {nameof(Start)}!");

            _nextObjectCost = _fullObjectCost / _maxObjectsCount;
            ValueInited?.Invoke(_objectsCount, _maxObjectsCount);
            InteractionPercentUpdated?.Invoke();
        }

        public virtual void DisableInteractable()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            _isAbleToInteract = false;
            _collider.enabled = false;
        }

        public virtual void TryEnableInteractable()
        {
            if (CurrentCompletionPercent < MaxCompletionPercent)
            {
                _isAbleToInteract = true;
                _collider.enabled = true;
            }
        }

        public virtual void StartInteraction(IInteractor interactor) { }

        public virtual void FinishInteraction(IInteractor interactor) { }

        public void Interact(IInteractor interactor)
        {
            if (interactor.ResourcePerTick <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(interactor.ResourcePerTick)} can't be less, than 1! It equals {interactor.ResourcePerTick} now!");

            _currentResouceGathered += interactor.ResourcePerTick;
            ValueChanged?.Invoke(_objectsCount, _maxObjectsCount);

            if (_currentResouceGathered >= _nextObjectCost)
            {
                _currentResouceGathered = 0f;
                DoInteraction();
            }
        }

        protected void SetMaxObjectsCount(int maxObjectsCount)
        {
            if (maxObjectsCount <= 0)
                throw new ArgumentOutOfRangeException($"{nameof(maxObjectsCount)} can't be less, than 1! It equals {maxObjectsCount} now!");

            _maxObjectsCount = maxObjectsCount;
        }

        protected virtual void DoInteraction()
        {
            _objectsCount += 1;
            ValueChanged?.Invoke(_objectsCount, _maxObjectsCount);
            InteractionPercentUpdated?.Invoke();
        }

        protected void SetObjectsCount(int objectsCount)
        {
            if (objectsCount < 0)
                throw new ArgumentOutOfRangeException($"{nameof(objectsCount)} can't be less, than 0! It equals {objectsCount}!");

            _objectsCount = objectsCount;
        }
    }
}
