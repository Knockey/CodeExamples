using Analytics;
using Extensions;
using Interactions;
using UnityEngine;

namespace Tutorial
{
    public class ParkBuildTutorialStage : TutorialStage
    {
        [SerializeField] private StageArrow _stageArrow;
        [SerializeField] private GrowBuildingDistrict _buildingDistrict;
        [SerializeField] private MonoBehaviour _interactableBehaviour;

        private IInteractable _interactableZone => (IInteractable)_interactableBehaviour;

        private void OnValidate()
        {
            GenericInterfaceInjection.TrySetObject<IInteractable>(ref _interactableBehaviour);
        }

        private void Awake()
        {
            _stageArrow.DisableArrow();
        }

        public override bool IsStageComplete(int stageIndex)
        {
            if (base.IsStageComplete(stageIndex) == false && _interactableZone.IsCompleted)
            {
                FreeplayAnalytics.Instance.SendTutorialStageCompleteEvent(StageName);
                SaveStageCompletion(stageIndex);
            }

            return base.IsStageComplete(stageIndex);
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);

            if (_buildingDistrict.CurrentCompletionPercent < 1)
            {
                _buildingDistrict.DisctictAnimationComplete += OnDisctictAnimationComplete;
            }
            else
            {
                _interactableZone.InteractionPercentUpdated += OnInteractionPercentUpdated;
                _stageArrow.EnableArrow();
            }
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            base.EndStage(tutorialStage);
            _interactableZone.InteractionPercentUpdated -= OnInteractionPercentUpdated;
            _stageArrow.DisableArrow();
        }

        private void OnDisctictAnimationComplete()
        {
            _buildingDistrict.DisctictAnimationComplete -= OnDisctictAnimationComplete;

            _interactableZone.InteractionPercentUpdated += OnInteractionPercentUpdated;
            _stageArrow.EnableArrow();
        }

        private void OnInteractionPercentUpdated()
        {
            if (_interactableZone.IsCompleted)
            {
                EndStage(this);
            }
        }
    }
}
