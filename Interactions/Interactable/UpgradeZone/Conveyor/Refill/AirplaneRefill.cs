using Interactions.Resources;
using Interactions.UpgradesIsland;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public abstract class AirplaneRefill : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _refillPercentWithNoAirplaneLanded = 0.33f;
        [SerializeField] private List<ConveyorObjectSpawner> _resourceSpawners;

        protected float InstantRefillResourceAmount = 0f;
        protected List<ConveyorObjectSpawner> ResourceSpawners => _resourceSpawners;
        protected float InstantRefillPercent => _refillPercentWithNoAirplaneLanded;

        public int MaxResourceLevelValue
        {
            get
            {
                var enabledCount = 0;

                _resourceSpawners.ForEach(spawner =>
                {
                    if (spawner.IsEnabled)
                    {
                        enabledCount += 1;
                    }
                });

                return enabledCount;
            }
        }

        public event Action<ConveyorObjectSpawner> TryBuySpawner;
        public event Action<ConveyorObjectSpawner> TryUpgradeSpawner;

        protected virtual void OnEnable()
        {
            _resourceSpawners.ForEach(spawner => spawner.TryBuySpawner += OnTryBuySpawner);
            _resourceSpawners.ForEach(spawner => spawner.TryUpgradeSpawner += OnTryUpgradeSpawner);
        }

        protected virtual void OnDisable()
        {
            _resourceSpawners.ForEach(spawner => spawner.TryBuySpawner -= OnTryBuySpawner);
            _resourceSpawners.ForEach(spawner => spawner.TryUpgradeSpawner -= OnTryUpgradeSpawner);
        }

        public void EnableSpawnersView()
        {
            _resourceSpawners.ForEach(spawner => spawner.EnableUpgradeView());
        }

        public void DisableSpawnersView()
        {
            _resourceSpawners.ForEach(spawner => spawner.DisableUpgradeView());
        }

        public void UpdateSpanwersButtonState(IInteractor interactor) => _resourceSpawners.ForEach(spawner => spawner.UpdateButtonsState(interactor));

        public void DoInstantRefill(InteractionResource interactionResource)
        {
            if (interactionResource.IsAbleToIncreaseAmount())
            {
                var instantRefillAmount = InstantRefillResourceAmount <= interactionResource.MaxResourceAmount * _refillPercentWithNoAirplaneLanded ?
                    InstantRefillResourceAmount :
                    interactionResource.MaxResourceAmount * _refillPercentWithNoAirplaneLanded;

                interactionResource.IncreaseResourceAmount(instantRefillAmount);
                InstantRefillResourceAmount = 0f;
            }
        }

        public abstract void StartRefill(InteractionResource interactionResource);

        public abstract void StopRefill(InteractionResource interactionResource);

        private void OnTryBuySpawner(ConveyorObjectSpawner spawner) => TryBuySpawner?.Invoke(spawner);

        private void OnTryUpgradeSpawner(ConveyorObjectSpawner spawner) => TryUpgradeSpawner?.Invoke(spawner);
    }
}
