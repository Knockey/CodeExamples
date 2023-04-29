using System;
using UnityEngine;

namespace Interactions
{
    public interface IInteractable
    {
        public float CurrentCompletionPercent { get; }
        public float ResourceToSpent { get; }
        public bool IsAbleToInteract { get; }
        public bool IsCompleted { get; }
        public Transform Transform { get; }

        public event Action InteractionPercentUpdated;

        public void DisableInteractable();
        public void TryEnableInteractable();
        public void StartInteraction(IInteractor interactor);
        public void Interact(IInteractor interactor);
        public void FinishInteraction(IInteractor interactor);
    }
}
