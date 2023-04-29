using SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class GrowBuildingsZone : InteractableZone, ISaveable
    {
        [Header("Grow building")]
        [SerializeField] private GrowingBuildingsList _buildingsList;
        [SerializeField] private GrowBuildingBlinkList _growBuildingBlinkList;
        [Header("Components to set active")]
        [SerializeField] private List<GameObject> _objectsToSetActive;
        [SerializeField] private InteractableObjectsDependency _interactableObjectsDependency;
        [SerializeField] private PropsEnable _propsEnable;
        [SerializeField] private Humans.HumanEnable _humanEnable;

        public string Name => gameObject.name;

        public event Action NeedToSave;

        protected override void Awake()
        {
            base.Awake();
            SetMaxObjectsCount(_buildingsList.BuildingsCount);

            _growBuildingBlinkList.DisableAll();
            DisableInteractable();
            _propsEnable.DisableProps();
            _humanEnable.DisableHumans();
        }

        private void OnEnable()
        {
            _interactableObjectsDependency.IsReadyToUnlock += OnReadyToUnlock;
        }

        private void OnDisable()
        {
            _interactableObjectsDependency.IsReadyToUnlock -= OnReadyToUnlock;
        }

        public int GetSaveValue()
        {
            return CurrentInteractionsCount;
        }

        public void LoadSaveData(SaveData saveData)
        {
            SetObjectsCount(saveData.ValueToSave);

            if (CurrentInteractionsCount > 0)
            {
                for (int buildingIndex = 0; buildingIndex < CurrentInteractionsCount; buildingIndex += 1)
                {
                    _buildingsList.GrowBuilding(buildingIndex);
                }

                TryEnableOnComplete();
            }
        }

        public override void TryEnableInteractable()
        {
            if (CurrentCompletionPercent < MaxCompletionPercent && _interactableObjectsDependency.IsReady)
            {
                base.TryEnableInteractable();
                _objectsToSetActive.ForEach(obj => obj.SetActive(true));
                _growBuildingBlinkList.TryEnableByIndex(CurrentInteractionsCount);
            }
        }

        public override void DisableInteractable()
        {
            base.DisableInteractable();
            _objectsToSetActive.ForEach(obj => obj.SetActive(false));
            _growBuildingBlinkList.DisableAll();
        }

        public override void StartInteraction(IInteractor interactor)
        {
            base.StartInteraction(interactor);
            _growBuildingBlinkList.EnableBlink();
        }

        public override void FinishInteraction(IInteractor interactor)
        {
            base.FinishInteraction(interactor);
            _growBuildingBlinkList.DisableBlink();
        }

        protected override void DoInteraction()
        {
            if (_buildingsList.BuildingsCount > CurrentInteractionsCount)
            {
                _growBuildingBlinkList.TryDisableCurrent();
                _buildingsList.GrowBuilding(CurrentInteractionsCount);
                base.DoInteraction();
                NeedToSave?.Invoke();

                _growBuildingBlinkList.TryEnableByIndex(CurrentInteractionsCount);
                _growBuildingBlinkList.EnableBlink();

                TryEnableOnComplete();
            }
        }

        private void TryEnableOnComplete()
        {
            if (IsCompleted)
            {
                _propsEnable.EnableProps();
                _humanEnable.EnableHumans();
            }
        }

        private void OnReadyToUnlock()
        {
            TryEnableInteractable();
        }
    }
}
