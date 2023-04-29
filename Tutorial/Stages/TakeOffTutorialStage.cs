using Interactions;
using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TakeOffTutorialStage : TutorialStage
    {
        [SerializeField] private Button _takeOffButton;
        [SerializeField] private ViewPanel _takeOffView;
        [SerializeField] private List<Button> _buttonsToDisable;

        private Coroutine _viewDisable;

        private void Awake()
        {
            _takeOffView.DisableView();
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);
            _takeOffButton.interactable = true;
            _takeOffButton.onClick.AddListener(OnTakeOffButtonClick);
            _takeOffButton.GetComponent<Animator>().enabled = true;
            _takeOffView.EnableView();
            _buttonsToDisable.ForEach(button => button.interactable = false);
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            base.EndStage(tutorialStage);
            _takeOffButton.onClick.RemoveListener(OnTakeOffButtonClick);
            _takeOffView.DisableView();

            if (_viewDisable != null)
            {
                StopCoroutine(_viewDisable);
            }
        }

        private void OnTakeOffButtonClick()
        {
            EndStage(this);
        }
    }
}
