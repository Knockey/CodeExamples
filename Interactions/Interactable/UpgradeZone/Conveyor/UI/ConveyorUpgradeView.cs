using Extensions;
using LoadingScreenView;
using System;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Interactions.UpgradesIsland
{
    public class ConveyorUpgradeView : ViewPanel
    {
        [SerializeField] private Button _spawnerUpgradeButton;
        [SerializeField] private string _maxedText = "MAX";
        [SerializeField] private ViewPanel _fingerImage;
        [SerializeField] private TextView _levelText;
        [SerializeField] private TextView _incomePerSecondText;
        [SerializeField] private TextView _upgradeCostText;
        [Header("Income view")]
        [SerializeField] private string _1kSuffix = "K";

        public event Action UpgradeButtonClicked;

        private void OnEnable()
        {
            _spawnerUpgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        }

        private void OnDisable()
        {
            _spawnerUpgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
        }

        public void UpdateIncomeView(float incomePerSecond)
        {
            SetIncomeView(incomePerSecond);
        }

        public void UpdateView(float incomePerSecond, bool isInteractable, bool isAbleToUpgrade, int currentLevel, int currentCost)
        {
            SetIncomeView(incomePerSecond);
            _spawnerUpgradeButton.interactable = isInteractable;
            currentLevel += 1;

            if (isAbleToUpgrade)
            {
                _levelText.SetTextFormated(currentLevel.ToString());
                _upgradeCostText.SetTextFormated(currentCost.ToString());

                return;
            }

            _levelText.SetTextUnformated(_maxedText);
            _upgradeCostText.SetTextUnformated(_maxedText);
            _upgradeCostText.PlaceTextOnCenter();
            _fingerImage.DisableView();
        }

        private void SetIncomeView(float incomePerSecond)
        {
            var currentValue = (int)incomePerSecond;
            var currentValueSuffix = "";
            var divider = 1000;
            var hundredDivider = 100;
            var floatPart = 0f;

            while (currentValue >= divider)
            {
                floatPart = currentValue % divider;
                floatPart /= hundredDivider;

                currentValue /= divider;
                currentValueSuffix += _1kSuffix;
            }

            var incomeText = currentValue + "." + (int)floatPart + currentValueSuffix;

            _incomePerSecondText.SetTextFormated(incomeText);
        }

        private void OnUpgradeButtonClicked() => UpgradeButtonClicked?.Invoke();
    }
}
