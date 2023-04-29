using BarUI;
using System;
using UnityEngine;
using Upgrades;

namespace Interactions.Resources
{
    public class InteractionResource : MonoBehaviour, IValueChanger, IUpgradable
    {
        [SerializeField] private ResourcePerTick _resourcePerTick;
        [Header("Upgrades")]
        [SerializeField] private UpgradeData _interactionResourceUpgrade;

        private float _currentResourceAmount = 0f;

        public float MaxResourceAmount => _interactionResourceUpgrade.GetModifiedUpgradeValue();
        public float ResourcePerTick => _resourcePerTick.UpgradeData.GetModifiedUpgradeValue();
        public bool IsResourceMaxed => _currentResourceAmount >= MaxResourceAmount;
        public UpgradeData UpgradeData => _interactionResourceUpgrade;
        public ResourcePerTick ResourcePerTickUpgradable => _resourcePerTick;

        public event Action<float, float> ValueInited;
        public event Action<float, float> ValueChanged;

        private void Start()
        {
            _currentResourceAmount = MaxResourceAmount;
            ValueInited?.Invoke(_currentResourceAmount, MaxResourceAmount);
        }

        public bool TryDecreaseResourceAmount()
        {
            var isAbleToDecreace = _currentResourceAmount > 0f;

            _currentResourceAmount -= ResourcePerTick;
            _currentResourceAmount = _currentResourceAmount > 0f ? _currentResourceAmount : 0f;

            ValueChanged?.Invoke(_currentResourceAmount, MaxResourceAmount);

            return isAbleToDecreace;
        }

        public bool IsAbleToIncreaseAmount()
        {
            return _currentResourceAmount < MaxResourceAmount;
        }

        public void IncreaseResourceAmount(float refillPerTick)
        {
            if (refillPerTick < 0)
                throw new ArgumentOutOfRangeException($"{nameof(refillPerTick)} can't be less, than 0! It euqlas {refillPerTick}!");

            if (IsAbleToIncreaseAmount() == false)
                throw new ArgumentOutOfRangeException($"Can't increase {nameof(_currentResourceAmount)}, because it's full! Check {nameof(_currentResourceAmount)} by call {nameof(IsAbleToIncreaseAmount)}!");

            var currentRefill = refillPerTick;

            if (_currentResourceAmount + refillPerTick > MaxResourceAmount)
                currentRefill = MaxResourceAmount - _currentResourceAmount;

            _currentResourceAmount += currentRefill;
            ValueChanged?.Invoke(_currentResourceAmount, MaxResourceAmount);
        }
    }
}
