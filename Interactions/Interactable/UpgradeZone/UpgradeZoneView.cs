using Extensions;
using LoadingScreenView;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;

namespace Interactions
{
    public class UpgradeZoneView : MonoBehaviour
    {
        [SerializeField] private ViewPanel _upgradeView;
        [SerializeField] private Button _takeOffButton;
        [SerializeField] private List<Canvas> _relativeCanvases;
        [Header("Upgrades")]
        [SerializeField] private ViewPanel _upgradeViewPanel;
        [SerializeField] private Button _upgradeCapacityButton;
        [SerializeField] private Button _upgradeIncomeButton;
        [SerializeField] private Button _upgradeResourceButton;
        [Header("Upgrade text")]
        [SerializeField] private TMPro.TMP_Text _capacityCostText;
        [SerializeField] private ViewPanel _capacityFingerImage;
        [SerializeField] private TextView _capacityValueText;
        [SerializeField] private TMPro.TMP_Text _incomeCostText;
        [SerializeField] private TextView _incomeValueText;
        [SerializeField] private ViewPanel _incomeFingerImage;
        [SerializeField] private TMPro.TMP_Text _resourceCostText;
        [SerializeField] private TextView _resourceValueText;
        [SerializeField] private ViewPanel _resourceFingerImage;
        [SerializeField] private string _textPrefix = "";
        [SerializeField] private string _textSuffix = "$";
        [SerializeField] private string _maxText = "MAX";
        [SerializeField] private string _1kText = "K";

        public event Action TakeOffButtonClicked;
        public event Action UpgradeCapacityButtonClicked;
        public event Action UpgradeIncomeButtonClicked;
        public event Action UpgradeResourceButtonClicked;

        private void Awake()
        {
            DisableView();
        }

        private void OnEnable()
        {
            _takeOffButton.onClick.AddListener(OnTakeOffButtonClicked);
            _upgradeCapacityButton.onClick.AddListener(OnUpgradeCapacityButtonClicked);
            _upgradeIncomeButton.onClick.AddListener(OnUpgradeIncomeButtonClicked);
            _upgradeResourceButton.onClick.AddListener(OnUpgradeResourceButtonClicked);
        }

        private void OnDisable()
        {
            _takeOffButton.onClick.RemoveListener(OnTakeOffButtonClicked);
            _upgradeCapacityButton.onClick.RemoveListener(OnUpgradeCapacityButtonClicked);
            _upgradeIncomeButton.onClick.RemoveListener(OnUpgradeIncomeButtonClicked);
            _upgradeResourceButton.onClick.RemoveListener(OnUpgradeResourceButtonClicked);
        }

        public void EnableView(bool needToEnableAnimation)
        {
            _upgradeView.EnableView();
            _upgradeViewPanel.EnableView();
            _relativeCanvases.ForEach(canvas => canvas.gameObject.SetActive(true));

            if (_takeOffButton.TryGetComponent(out Animator animator))
            {
                animator.enabled = needToEnableAnimation;
            }
        }

        public void DisableView()
        {
            _upgradeView.DisableView();
            _upgradeViewPanel.DisableView();
            _relativeCanvases.ForEach(canvas => canvas.gameObject.SetActive(false));
        }

        public void SetUIState(IInteractor interactor)
        {
            SetButtonState(interactor, interactor.InteractionResource, _upgradeCapacityButton, _capacityCostText, _capacityFingerImage);
            SetTextState(interactor.InteractionResource, _capacityValueText);
            SetButtonState(interactor, interactor.UpgradeResource, _upgradeIncomeButton, _incomeCostText, _incomeFingerImage);
            SetTextState(interactor.UpgradeResource, _incomeValueText);
            SetButtonState(interactor, interactor.ResourcePerTickUpgradable, _upgradeResourceButton, _resourceCostText, _resourceFingerImage);
            SetTextState(interactor.ResourcePerTickUpgradable, _resourceValueText);
        }

        private void SetButtonState(IInteractor interactor, IUpgradable upgradable, Button button, TMPro.TMP_Text text, ViewPanel image)
        {
            var currentUpgradeCost = upgradable.UpgradeData.GetCurrentUpgradeCost();
            var isAbleToUpgrade = upgradable.UpgradeData.IsAbleToUpgrade && interactor.UpgradeResource.IsAbleToDecreaseResourceAmount(currentUpgradeCost);
            button.interactable = isAbleToUpgrade;

            if (upgradable.UpgradeData.IsAbleToUpgrade)
            {
                text.text = _textPrefix + currentUpgradeCost.ToString() + _textSuffix;
            }
            else
            {
                text.text = _maxText;

                var textAnchored = text.rectTransform.anchoredPosition;
                textAnchored.x = 0;
                text.rectTransform.anchoredPosition = textAnchored;

                image.DisableView();
            }
        }

        private void SetTextState(IUpgradable upgradable, TextView textView)
        {
            textView.SetTextFormated((upgradable.UpgradeData.CurrentUpgradeLevel + 1).ToString());
        }

        private void OnTakeOffButtonClicked()
        {
            if (_takeOffButton.TryGetComponent(out Animator animator))
            {
                animator.enabled = false;
            }

            TakeOffButtonClicked?.Invoke();
        }

        private void OnUpgradeCapacityButtonClicked()
        {
            if (_takeOffButton.TryGetComponent(out Animator animator))
            {
                animator.enabled = false;
            }
            UpgradeCapacityButtonClicked?.Invoke();
        }

        private void OnUpgradeIncomeButtonClicked()
        {
            UpgradeIncomeButtonClicked?.Invoke();
        }

        private void OnUpgradeResourceButtonClicked()
        {
            UpgradeResourceButtonClicked?.Invoke();
        }
    }
}
