using Interactions.Resources;
using Interactions.UpgradesIsland;
using UnityEngine;

namespace Interactions
{
    public class AirplaneCollisionRefill : AirplaneRefill
    {
        [SerializeField] private DestroyTrigger _destroyTrigger;
        [SerializeField] private Animator _takeOffButtonAnimator;

        private InteractionResource _interactionResource;

        protected override void OnEnable()
        {
            base.OnEnable();
            _destroyTrigger.Collided += OnCollided;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _destroyTrigger.Collided -= OnCollided;
        }

        public override void StartRefill(InteractionResource interactionResource)
        {
            _interactionResource = interactionResource;
        }

        public override void StopRefill(InteractionResource interactionResource)
        {
            _interactionResource = null;
        }

        private void OnCollided(IRefillResource refillResource)
        {
            if (_interactionResource != null && _interactionResource.IsAbleToIncreaseAmount())
            {
                _interactionResource.IncreaseResourceAmount(refillResource.ResourceAmount);

                if (_interactionResource.IsResourceMaxed)
                {
                    _takeOffButtonAnimator.enabled = true;
                }

                return;
            }

            InstantRefillResourceAmount += refillResource.ResourceAmount;
        }
    }
}
