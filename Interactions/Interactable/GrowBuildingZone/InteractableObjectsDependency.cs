using Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class InteractableObjectsDependency : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> _interactableMonoBehaviours;

        private List<IInteractable> _interactables = new();

        public bool IsReady
        {
            get
            {
                var isReadyToUnlock = true;
                _interactables?.ForEach(interactable => isReadyToUnlock = isReadyToUnlock && interactable.IsCompleted);
                return isReadyToUnlock;
            }
        }

        public event Action IsReadyToUnlock;

        private void OnValidate()
        {
            if (_interactables.Count > 0)
            {
                _interactableMonoBehaviours.ForEach(behaviour =>
                {
                    GenericInterfaceInjection.TrySetObject<IInteractable>(ref behaviour);
                });
            }
        }

        private void Awake()
        {
            FillInteractablesList();
        }

        private void OnEnable()
        {
            _interactables?.ForEach(interactable => interactable.InteractionPercentUpdated += OnInteractionPercentUpdated);
        }

        private void OnDisable()
        {
            _interactables?.ForEach(interactable => interactable.InteractionPercentUpdated -= OnInteractionPercentUpdated);
        }

        private void Start()
        {
            if (_interactables == null || (_interactables != null && _interactables.Count == 0))
                IsReadyToUnlock?.Invoke();
        }

        private void OnInteractionPercentUpdated()
        {
            if (IsReady)
                IsReadyToUnlock?.Invoke();
        }

        private void FillInteractablesList()
        {
            _interactableMonoBehaviours.ForEach(behaviour =>
            {
                _interactables.Add(behaviour as IInteractable);
            });
        }
    }
}
