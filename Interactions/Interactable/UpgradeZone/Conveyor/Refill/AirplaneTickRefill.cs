using Interactions.Resources;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class AirplaneTickRefill : AirplaneRefill
    {
        private Coroutine _refill;

        private float RefillPerTick
        {
            get
            {
                var modifiersSum = 0f;
                ResourceSpawners.ForEach(spawner =>
                {
                    if (spawner.IsEnabled)
                    {
                        modifiersSum += spawner.CurrentRefillPerTick;
                    }
                });

                return modifiersSum;
            }
        }

        public override void StartRefill(InteractionResource interactionResource)
        {
            StopRefill(interactionResource);
            _refill = StartCoroutine(Refill(interactionResource));
        }

        public override void StopRefill(InteractionResource interactionResource)
        {
            if (_refill != null)
            {
                StopCoroutine(_refill);
                _refill = null;
            }

            _refill = StartCoroutine(RefillForInstant(interactionResource.MaxResourceAmount));
        }

        private IEnumerator Refill(InteractionResource interactionResource)
        {
            while (interactionResource.IsAbleToIncreaseAmount())
            {
                interactionResource.IncreaseResourceAmount(RefillPerTick);
                yield return null;
            }
        }

        private IEnumerator RefillForInstant(float maxAmount)
        {
            var amount = maxAmount * InstantRefillPercent;

            while (InstantRefillPercent <= amount)
            {
                InstantRefillResourceAmount += RefillPerTick;
                yield return null;
            }
        }

    }
}
