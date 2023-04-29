using Interactions;
using LoadingScreenView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class UpgradeTutorialStage : TutorialStage
    {
        [SerializeField] private Button _upgradeAirplaneButton;
        [SerializeField] private ViewPanel _upgradeAirplaneView;
        [SerializeField] private Button _spawnerUpgradeButton;
        [SerializeField] private ViewPanel _upgradeConveyorView;
        [SerializeField] private List<Button> _buttonsToDisable;
        [SerializeField] private Animator _takeOffButton;

        private Coroutine _buttonsDisable;
        private Coroutine _takeOffButtonDisable;

        private void Awake()
        {
            _upgradeAirplaneView.DisableView();
            _upgradeConveyorView.DisableView();
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);

            _buttonsDisable = StartCoroutine(DisableButtons());
            _takeOffButtonDisable = StartCoroutine(TakeOffButtonDisable());

            _upgradeAirplaneButton.onClick.AddListener(OnUpgradeButtonClick); 
            _upgradeAirplaneView.EnableView();
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            _upgradeConveyorView.DisableView();
            _spawnerUpgradeButton.onClick.RemoveListener(OnSpawnerUpgradeButtonClick);

            _buttonsToDisable.ForEach(button => button.interactable = true);
            _upgradeAirplaneButton.interactable = true;

            if (_takeOffButtonDisable != null)
            {
                StopCoroutine(_takeOffButtonDisable);
            }

            base.EndStage(tutorialStage);
        }

        private void OnUpgradeButtonClick()
        {
            _upgradeAirplaneButton.onClick.RemoveListener(OnUpgradeButtonClick);
            _upgradeAirplaneButton.interactable = false;
            _upgradeAirplaneView.DisableView();
            _buttonsToDisable.ForEach(button => button.interactable = false);

            if (_buttonsDisable != null)
            {
                StopCoroutine(_buttonsDisable);
            }

            _upgradeConveyorView.EnableView();
            _spawnerUpgradeButton.interactable = true;
            _spawnerUpgradeButton.onClick.AddListener(OnSpawnerUpgradeButtonClick);
        }

        private void OnSpawnerUpgradeButtonClick()
        {
            EndStage(this);
        }

        private IEnumerator DisableButtons()
        {
            while (enabled)
            {
                _buttonsToDisable.ForEach(button => button.interactable = false);
                _spawnerUpgradeButton.interactable = false;

                yield return null;
            }
        }

        private IEnumerator TakeOffButtonDisable()
        {
            while (enabled)
            {
                _takeOffButton.enabled = false;
                yield return null;
            }
        }
    }
}
