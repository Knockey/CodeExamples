using SaveLoad;
using System;
using UnityEngine;

namespace Upgrades
{
    [Serializable]
    public class UpgradeData : ISaveable
    {
        [SerializeField] private float _initialValue = 1f;
        [SerializeField] private float _modifier = 0.5f;
        [SerializeField, Min(1)] private int _maxUpgradeLevel = 5;
        [Header("Cost")]
        [SerializeField] private int _initialUpgradeCost = 5;
        [SerializeField] private int _upgradeCostModifier = 5;

        private int _currentUpgradeLevel = 0;

        public int CurrentUpgradeLevel => _currentUpgradeLevel;
        public bool IsAbleToUpgrade => _currentUpgradeLevel < _maxUpgradeLevel;

        public string Name => "UpgradeData";

        public event Action NeedToSave;

        public virtual float GetModifiedUpgradeValue() => _initialValue + _modifier * _currentUpgradeLevel;

        public virtual int GetCurrentUpgradeCost() => _initialUpgradeCost + _upgradeCostModifier * _currentUpgradeLevel;

        public virtual void SetUpgradeLevel(int upgradeLevel)
        {
            if (upgradeLevel < 0)
                throw new ArgumentOutOfRangeException($"{nameof(upgradeLevel)} can't be less, than 0 and more, than {_maxUpgradeLevel}!" +
                    $"Now it equals {upgradeLevel}!");

            _currentUpgradeLevel = upgradeLevel;
            NeedToSave?.Invoke();
        }

        public int GetSaveValue()
        {
            return  _currentUpgradeLevel;
        }

        public void LoadSaveData(SaveData saveData)
        {
            if (saveData.ValueToSave < 0)
                throw new ArgumentOutOfRangeException($"{nameof(saveData.ValueToSave)} can't be less, than 0! It equals {saveData.ValueToSave}!");

            SetUpgradeLevel(saveData.ValueToSave);
        }
    }
}
