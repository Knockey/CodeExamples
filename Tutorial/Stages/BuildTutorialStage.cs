using Extensions;
using Interactions;
using System.Collections.Generic;
using UnityEngine;

namespace Tutorial
{
    public class BuildTutorialStage : TutorialStage
    {
        [SerializeField] private List<MonoBehaviour> _interactableBehaviours;
        [SerializeField] private bool _needToDisableAirport = false;
        [Header("Arrow")]
        [SerializeField] private StageArrow _stageArrow;
        [SerializeField] private MonoBehaviour _behaviourToDisableArrow;
        [SerializeField, Min(0f)] private float _arrowDisablePercent = 0.25f;

        private IInteractable _interactableToDisableArrow => (IInteractable)_behaviourToDisableArrow;
        private List<IInteractable> _interactableZones;

        private void OnValidate()
        {
            if (_interactableBehaviours != null && _interactableBehaviours.Count > 0)
            {
                _interactableBehaviours.ForEach(behaviour =>
                {
                    GenericInterfaceInjection.TrySetObject<IInteractable>(ref behaviour);
                });
            }

            GenericInterfaceInjection.TrySetObject<IInteractable>(ref _behaviourToDisableArrow);
        }

        private void Awake()
        {
            _stageArrow.DisableArrow();
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);

            _interactableZones = new List<IInteractable>();
            _interactableBehaviours.ForEach(obj =>
            {
                _interactableZones.Add(obj as IInteractable);
            });

            _interactableZones.ForEach(zone => zone.InteractionPercentUpdated += OnInteractionPercentUpdated);

            if (_interactableToDisableArrow.CurrentCompletionPercent < _arrowDisablePercent)
            {
                _stageArrow.EnableArrow();
                _interactableToDisableArrow.InteractionPercentUpdated += OnInteractionPercentUpdatedOnArrowDisabler;
            }

            if (_needToDisableAirport && upgradeZone)
            {
                upgradeZone.transform.position = new Vector3(-1000f, -1000f, -1000f);
            }
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            base.EndStage(tutorialStage);
            _interactableZones.ForEach(zone => zone.InteractionPercentUpdated -= OnInteractionPercentUpdated);
            _stageArrow.DisableArrow();
        }

        private void OnInteractionPercentUpdatedOnArrowDisabler()
        {
            if (_interactableToDisableArrow.CurrentCompletionPercent > _arrowDisablePercent)
            {
                _stageArrow.DisableArrow();
                _interactableToDisableArrow.InteractionPercentUpdated -= OnInteractionPercentUpdatedOnArrowDisabler;
            }
        }

        private void OnInteractionPercentUpdated()
        {
            bool isCompleted = true;
            _interactableZones.ForEach(zone =>
            {
                isCompleted = isCompleted && zone.IsCompleted;
            });

            if (isCompleted)
            {
                EndStage(this);
            }
        }
    }
}
