using System;
using UnityEngine;
using Upgrades;

namespace Interactions.Resources
{
    public class UpgradeResource : MonoBehaviour, IResource
    {
        private const string UpgradeResourcePref = "UPGRADE_RESOURCE";
        private const int MaxAmount = 99999;

        [SerializeField, Min(0f)] private float _resourceToSpent = 5000;
        [SerializeField] private UpgradeData _upgradeData;

        private int _currentResourceAmount;
        private float _interactionResourceSpent = 0;
        private int _currentUpgradeResourcePerTick => (int)_upgradeData.GetModifiedUpgradeValue();

        public UpgradeData UpgradeData => _upgradeData;

        public event Action<int> ResourceAmountInited;
        public event Action<int, int> ResourceAmountUpdated;

        private void Awake()
        {
            _currentResourceAmount = PlayerPrefs.GetInt(UpgradeResourcePref, 0);
        }

        private void Start()
        {
            ResourceAmountInited?.Invoke(_currentResourceAmount);
        }

        public void IncreaseSpentResourceAmount(float resourceAmountSpent)
        {
            if (resourceAmountSpent < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(resourceAmountSpent)} can't be less, than 0! It equals {resourceAmountSpent} now!");
            }

            _interactionResourceSpent += resourceAmountSpent;

            if (_interactionResourceSpent >= _resourceToSpent)
            {
                var diff = _interactionResourceSpent - _resourceToSpent;
                _interactionResourceSpent = diff > 0f ? diff : 0f;

                IncreaceUpgradeResourceAmount(_currentUpgradeResourcePerTick);
            }
        }

        public void IncreaceUpgradeResourceAmount(int resourceAmount)
        {
            if (resourceAmount < 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(resourceAmount)} can't be less, than 0! It equals {resourceAmount} now!");
            }

            var newAmount = _currentResourceAmount + resourceAmount;
            _currentResourceAmount = newAmount > MaxAmount ? MaxAmount : newAmount;

            PlayerPrefs.SetInt(UpgradeResourcePref, _currentResourceAmount);
            ResourceAmountUpdated?.Invoke(_currentResourceAmount, resourceAmount);
        }

        public bool IsAbleToDecreaseResourceAmount(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException($"{nameof(amount)} can't be less, than 0! It equals {amount} now!");

            return _currentResourceAmount - amount >= 0;
        }

        public void DecreaseResourceAmount(int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException($"{nameof(amount)} can't be less, than 0! It equals {amount} now!");

            if (IsAbleToDecreaseResourceAmount(amount) == false)
                throw new ArgumentOutOfRangeException($"Can't decrease {nameof(_currentResourceAmount)}, because it's full! Check {nameof(_currentResourceAmount)} by call {nameof(IsAbleToDecreaseResourceAmount)}!");

            _currentResourceAmount -= amount;
            PlayerPrefs.SetInt(UpgradeResourcePref, _currentResourceAmount);
            ResourceAmountUpdated?.Invoke(_currentResourceAmount, 0);
        }
    }
}
