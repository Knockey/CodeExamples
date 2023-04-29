using Interactions;
using Interactions.Resources;
using LoadingScreenView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tutorial
{
    public class TapTutorialStage : TutorialStage
    {
        [SerializeField] private TapInput _tapInput;
        [SerializeField] private InteractionResource _interactionResource;
        [SerializeField] private ViewPanel _tapView;
        [SerializeField] private List<Button> _buttonsToDisable;
        [SerializeField, Min(0f)] private float _minTimeToTap = 5f;
        [SerializeField] private Animator _takeOffButton;

        private Coroutine _takeOffButtonDisable;

        private void Awake()
        {
            _tapView.DisableView();
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);
            _tapView.EnableView();
            _buttonsToDisable.ForEach(button => button.interactable = false);
            StartCoroutine(WaitForResourceMaxed());
            _takeOffButtonDisable = StartCoroutine(TakeOffButtonDisable());
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            _tapView.DisableView();
            _buttonsToDisable.ForEach(button => button.interactable = true);

            if (_takeOffButtonDisable != null)
            {
                StopCoroutine(_takeOffButtonDisable);
            }

            base.EndStage(tutorialStage);
        }

        private IEnumerator WaitForResourceMaxed()
        {
            var time = 0f;

            while (/*_interactionResource.IsResourceMaxed == false ||*/ time <= _minTimeToTap || _tapInput.IsTaping == false)
            {
                yield return null;
                time += Time.deltaTime;
            }

            EndStage(this);
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
