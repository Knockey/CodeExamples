using Airplane;
using Analytics;
using Interactions.Resources;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrades;

namespace Interactions
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(InteractionResource))]
    [RequireComponent(typeof(UpgradeResource))]
    public class Interactor : MonoBehaviour, IInteractor
    {
        [SerializeField] private CameraSwitch _cameraSwitch;
        [SerializeField] private MonoBehaviour _movementBehaviour;
        [SerializeField] private MonoBehaviour _landerBehaviour;
        [Header("Resource particle")]
        [SerializeField] private List<ParticleSystem> _moneyParticles;
        [SerializeField] private Transform _particlePoint;
        [Header("Upgrade particle")]
        [SerializeField] private ParticleSystem _capacityUpgradeParticle;
        [SerializeField] private ParticleSystem _incomeUpgradeParticle;
        [SerializeField] private ParticleSystem _resourceUpgradeParticle;
        [Header("UI")]
        [SerializeField] private NeedToBuildDirectionView _needToBuildDirectionView;

        private int _particlesCount = 1;
        private InteractionResource _interactionResource;
        private UpgradeResource _upgradeResource;
        private Coroutine _interaction;
        private IInteractable _previousInteractable;

        public bool IsResourceMaxed => _interactionResource.IsResourceMaxed;
        public float ResourcePerTick => _interactionResource.ResourcePerTick;
        public IMovement Movement => (IMovement)_movementBehaviour;
        public ILander Lander => (ILander)_landerBehaviour;
        public IResource UpgradeResource => _upgradeResource;
        public InteractionResource InteractionResource => _interactionResource;
        public IUpgradable ResourcePerTickUpgradable => _interactionResource.ResourcePerTickUpgradable;

        private void OnValidate()
        {
            TrySetObject<IMovement>(ref _movementBehaviour);
            TrySetObject<ILander>(ref _landerBehaviour);
        }

        private void Awake()
        {
            SetupComponents();
            DisableMoneyParticle();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable) && interactable.IsAbleToInteract && _previousInteractable != interactable)
            {
                if (_previousInteractable != null)
                {
                    StopInteraction(_previousInteractable);
                }

                _previousInteractable = interactable;
                InteractWith(interactable);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (_interaction == null && other.TryGetComponent(out GrowBuildingsZone zone))
            {
                var interactable = zone as IInteractable;

                if (interactable.IsAbleToInteract && _previousInteractable != interactable)
                {
                    if (_previousInteractable != null)
                    {
                        StopInteraction(_previousInteractable);
                    }

                    _previousInteractable = interactable;
                    InteractWith(interactable);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out IInteractable interactable) && _interaction != null && _previousInteractable == interactable)
                StopInteraction(interactable);
        }

        private void InteractWith(IInteractable interactable)
        {
            switch (interactable)
            {
                case UpgradeZone upgradeZone:
                    InteractWithUpgradeZone(upgradeZone);
                    break;
                default:
                    if (interactable.IsAbleToInteract)
                    {
                        _interaction = StartCoroutine(Interact(interactable));
                    }
                    break;
            }
        }

        private IEnumerator Interact(IInteractable interactable)
        {
            EnableMoneyParticle();
            interactable.StartInteraction(this);

            while (interactable.IsAbleToInteract && _interactionResource.TryDecreaseResourceAmount())
            {
                interactable.Interact(this);
                _upgradeResource.IncreaseSpentResourceAmount(ResourcePerTick);

                yield return null;
            }

            DisableMoneyParticle();
            interactable.FinishInteraction(this);
        }

        private void InteractWithUpgradeZone(UpgradeZone upgradeZone)
        {
            _cameraSwitch.SwitchToLandedCamera();
            _needToBuildDirectionView.Disable();

            upgradeZone.TakeOffButtonClicked += StopInteraction;
            upgradeZone.TriedToUpgradeCapacity += OnTriedToUpgradeCapacity;
            upgradeZone.TriedToUpgradeIncome += OnTriedToUpgradeIncome;
            upgradeZone.TriedToUpgradeResource += OnTriedToUpgradeResource;

            upgradeZone.SetUpgradeButtonState(this);
            upgradeZone.StartInteraction(this);
        }

        private void EnableMoneyParticle()
        {
            for (int particleIndex = 0; particleIndex < _particlesCount; particleIndex += 1)
            {
                _moneyParticles[particleIndex].Play();
            }
        }

        private void DisableMoneyParticle()
        {
            for (int particleIndex = 0; particleIndex < _moneyParticles.Count; particleIndex += 1)
            {
                _moneyParticles[particleIndex].Stop();
            }
        }

        private void StopInteraction(IInteractable interactable)
        {
            if (interactable is UpgradeZone upgradeZone)
            {
                upgradeZone.TakeOffButtonClicked -= StopInteraction;
                upgradeZone.TriedToUpgradeCapacity -= OnTriedToUpgradeCapacity;
                upgradeZone.TriedToUpgradeIncome -= OnTriedToUpgradeIncome;
                upgradeZone.TriedToUpgradeResource -= OnTriedToUpgradeResource;

                _particlesCount = upgradeZone.MaxResourceLevelValue;
                _cameraSwitch.SwitchToFlyCamera();
                _needToBuildDirectionView.Enable();
            }

            if (_interaction != null)
            {
                DisableMoneyParticle();
                interactable.FinishInteraction(this);
                StopCoroutine(_interaction);
                _interaction = null;
            }

            _previousInteractable = null;
        }

        private void OnTriedToUpgradeCapacity(UpgradeZone upgradeZone)
        {
            TryToUpgrade(upgradeZone, _interactionResource, AirplaneUpgradeType.Capacity);
            upgradeZone.RestartRefill(this);
            InteractionResource.IncreaseResourceAmount(1f);
        }

        private void OnTriedToUpgradeIncome(UpgradeZone upgradeZone)
        {
            TryToUpgrade(upgradeZone, _upgradeResource, AirplaneUpgradeType.Income);
        }

        private void OnTriedToUpgradeResource(UpgradeZone upgradeZone)
        {
            TryToUpgrade(upgradeZone, ResourcePerTickUpgradable, AirplaneUpgradeType.ResourseDropSpeed);
        }

        private void TryToUpgrade(UpgradeZone upgradeZone, IUpgradable upgradable, AirplaneUpgradeType airplaneUpgradeType)
        {
            var currentUpgradeCost = upgradable.UpgradeData.GetCurrentUpgradeCost();

            if (upgradable.UpgradeData.IsAbleToUpgrade && _upgradeResource.IsAbleToDecreaseResourceAmount(currentUpgradeCost))
            {
                _upgradeResource.DecreaseResourceAmount(currentUpgradeCost);

                var resourceAmountLevel = upgradable.UpgradeData.CurrentUpgradeLevel;
                upgradable.UpgradeData.SetUpgradeLevel(resourceAmountLevel + 1);
                upgradeZone.RestartRefill(this);

                PlayParticle(airplaneUpgradeType);

                FreeplayAnalytics.Instance.SendAirplaneUpgradeEvent(airplaneUpgradeType, upgradable.UpgradeData.CurrentUpgradeLevel);

                upgradeZone.SetUpgradeButtonState(this);
            }
        }

        private void PlayParticle(AirplaneUpgradeType upgradeType)
        {
            switch (upgradeType)
            {
                case AirplaneUpgradeType.Capacity:
                    _capacityUpgradeParticle.Play();
                    break;
                case AirplaneUpgradeType.Income:
                    _incomeUpgradeParticle.Play();
                    break;
                case AirplaneUpgradeType.ResourseDropSpeed:
                    _resourceUpgradeParticle.Play();
                    break;
                default: 
                    throw new System.ArgumentOutOfRangeException($"{upgradeType} particle not implemented!");
            }
        }

        private void SetupComponents()
        {
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;

            var collider = GetComponent<Collider>();
            collider.isTrigger = true;

            _interactionResource = GetComponent<InteractionResource>();
            _upgradeResource = GetComponent<UpgradeResource>();
        }

        private static void TrySetObject<T>(ref MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null)
                return;

            if (monoBehaviour is T)
                return;

            Debug.LogWarning(monoBehaviour.name + " needs to implement " + typeof(T));
            monoBehaviour = null;
        }
    }
}
