using Analytics;
using Interactions.UpgradesIsland;
using LoadingScreenView;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class UpgradeZone : InteractableZone
    {
        private const int InteractionsCount = int.MaxValue;

        [SerializeField] private Transform _landingPoint;
        [SerializeField] private UpgradeZoneView _upgradeView;
        [SerializeField] private List<ViewPanel> _viewsToDisable;
        [Header("Refill")]
        [SerializeField] private AirplaneRefill _airplaneRefill;
        [SerializeField] private DestroyTrigger _destroyTrigger;
        [SerializeField] private ConveyorSpeedModifier _tapConveyorsSpeedIncrease;

        private IInteractor _currentInteractor;

        public int MaxResourceLevelValue => _airplaneRefill.MaxResourceLevelValue;

        public event Action<IInteractable> TakeOffButtonClicked;
        public event Action Landed;
        public event Action<UpgradeZone> TriedToUpgradeCapacity;
        public event Action<UpgradeZone> TriedToUpgradeIncome;
        public event Action<UpgradeZone> TriedToUpgradeResource;

        protected override void Awake()
        {
            base.Awake();
            SetMaxObjectsCount(InteractionsCount);
        }

        private void OnEnable()
        {
            _upgradeView.TakeOffButtonClicked += OnTakeOffButtonClicked;
            _upgradeView.UpgradeCapacityButtonClicked += OnUpgradeCapacityButtonClicked;
            _upgradeView.UpgradeIncomeButtonClicked += OnUpgradeIncomeButtonClicked;
            _upgradeView.UpgradeResourceButtonClicked += OnUpgradeResourceButtonClicked;

            _airplaneRefill.TryBuySpawner += OnTryBuySpawner;
            _airplaneRefill.TryUpgradeSpawner += OnTryUpgradeSpawner;
        }

        private void OnDisable()
        {
            _upgradeView.TakeOffButtonClicked -= OnTakeOffButtonClicked;
            _upgradeView.UpgradeCapacityButtonClicked -= OnUpgradeCapacityButtonClicked;
            _upgradeView.UpgradeIncomeButtonClicked -= OnUpgradeIncomeButtonClicked;
            _upgradeView.UpgradeResourceButtonClicked -= OnUpgradeResourceButtonClicked;

            _airplaneRefill.TryBuySpawner -= OnTryBuySpawner;
            _airplaneRefill.TryUpgradeSpawner -= OnTryUpgradeSpawner;
        }

        public override void StartInteraction(IInteractor interactor)
        {
            _currentInteractor = interactor;
            interactor.Movement.StopMovement();
            interactor.Lander.Land(_landingPoint);

            _airplaneRefill.DoInstantRefill(interactor.InteractionResource);
            _airplaneRefill.StartRefill(interactor.InteractionResource);
            _tapConveyorsSpeedIncrease.StartTapObserver();

            _upgradeView.EnableView(interactor.InteractionResource.IsResourceMaxed);
            _airplaneRefill.EnableSpawnersView();
            _destroyTrigger.EnableView();
            _viewsToDisable.ForEach(view => view.DisableView());

            Landed?.Invoke();

            if (interactor.IsResourceMaxed == false)
            {
                base.StartInteraction(interactor);
            }
        }

        public override void FinishInteraction(IInteractor interactor)
        {
            base.FinishInteraction(interactor);

            interactor.Lander.TakeOff();
            interactor.Movement.StartMovement();
            _airplaneRefill.StopRefill(interactor.InteractionResource);
            _currentInteractor = null;
        }

        public void SetUpgradeButtonState(IInteractor interactor)
        {
            _upgradeView.SetUIState(interactor);
            _airplaneRefill.UpdateSpanwersButtonState(interactor);
        }

        public void RestartRefill(IInteractor interactor) => _airplaneRefill.StartRefill(interactor.InteractionResource);

        private void OnTakeOffButtonClicked()
        {
            FinishInteraction(_currentInteractor);

            _tapConveyorsSpeedIncrease.StopTapObserver();

            _upgradeView.DisableView();
            _destroyTrigger.DisableView();
            _viewsToDisable.ForEach(view => view.EnableView());
            _airplaneRefill.DisableSpawnersView();

            TakeOffButtonClicked?.Invoke(this);
        }

        private void OnUpgradeCapacityButtonClicked()
        {
            TriedToUpgradeCapacity?.Invoke(this);
        }

        private void OnUpgradeIncomeButtonClicked()
        {
            TriedToUpgradeIncome?.Invoke(this);
        }

        private void OnUpgradeResourceButtonClicked()
        {
            TriedToUpgradeResource?.Invoke(this);
        }

        private void OnTryBuySpawner(ConveyorObjectSpawner conveyorObjectSpawner)
        {
            var currentCost = conveyorObjectSpawner.SpawnerCost;
            var isAbleToUpgrade = conveyorObjectSpawner.UpgradeData.IsAbleToUpgrade && _currentInteractor.UpgradeResource.IsAbleToDecreaseResourceAmount(currentCost);

            if (isAbleToUpgrade)
            {
                _currentInteractor.UpgradeResource.DecreaseResourceAmount(currentCost);

                conveyorObjectSpawner.EnableObject();
                _airplaneRefill.UpdateSpanwersButtonState(_currentInteractor);
                _upgradeView.SetUIState(_currentInteractor);

                FreeplayAnalytics.Instance.SendConveyorUpgradeEvent(conveyorObjectSpawner.Name, ConveyorUpgradeType.ConveyorBuy, 0);
            }
        }

        private void OnTryUpgradeSpawner(ConveyorObjectSpawner conveyorObjectSpawner)
        {
            var currentCost = conveyorObjectSpawner.UpgradeData.GetCurrentUpgradeCost();
            var isAbleToUpgrade = conveyorObjectSpawner.UpgradeData.IsAbleToUpgrade && _currentInteractor.UpgradeResource.IsAbleToDecreaseResourceAmount(currentCost);

            if (isAbleToUpgrade)
            {
                _currentInteractor.UpgradeResource.DecreaseResourceAmount(currentCost);

                conveyorObjectSpawner.UpgradeSpawner();
                _airplaneRefill.UpdateSpanwersButtonState(_currentInteractor);
                _upgradeView.SetUIState(_currentInteractor);

                FreeplayAnalytics.Instance.SendConveyorUpgradeEvent(conveyorObjectSpawner.Name, ConveyorUpgradeType.ConveyorUpgrade, conveyorObjectSpawner.CurrentLevel);
            }
        }
    }
}
