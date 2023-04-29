using System;
using System.Collections;
using UniRx;
using UnityEngine;

namespace Interactions
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(InteractableObjectsDependency))]
    public class UpgradeResourceGenerator : MonoBehaviour
    {
        [SerializeField, Min(0)] private int _upgradeResourceIncrement = 1;
        [SerializeField, Min(0)] private int _upgradeResourceCap = 100;
        [SerializeField, Min(0f)] private float _tickTime = 1f;
        [SerializeField, Min(0f)] private float _delayBeforeStartAgain = 3f;
        [Header("UI")]
        [SerializeField] private UpgradeResourceGeneratorView _generatorView;
        [Header("Particles")]
        [SerializeField] private ParticleSystem _pickUpParticle;
        [SerializeField] private Transform _particlePoint;

        private InteractableObjectsDependency _interactableObjectsDependency;
        private int _upgradeResourceAmount = 0;
        private bool _isAbleToTake = false;
        private CompositeDisposable _disposables;

        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;

            _interactableObjectsDependency = GetComponent<InteractableObjectsDependency>();
            _generatorView.DisableView();
            _disposables = new CompositeDisposable();
        }

        private void OnEnable()
        {
            _interactableObjectsDependency.IsReadyToUnlock += OnReadyToUnlock;
        }

        private void OnDisable()
        {
            _interactableObjectsDependency.IsReadyToUnlock -= OnReadyToUnlock;
            _disposables?.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_isAbleToTake && other.TryGetComponent(out IInteractor interactor))
            {
                GiveUpgradeResource(interactor);
            }
        }

        public void TryEnableView()
        {
            if (_interactableObjectsDependency.IsReady)
            {
                _generatorView.EnableView();
            }
        }

        public void DisableView()
        {
            _generatorView.DisableView();
        }

        private void OnReadyToUnlock()
        {
            _generatorView.EnableView();
            StartCoroutine(GenerateUpgradeResource());
            EnableTakeAbilityDelayed();
        }

        private IEnumerator GenerateUpgradeResource()
        {
            var tick = new WaitForSeconds(_tickTime);

            while (enabled)
            {
                yield return tick;

                _upgradeResourceAmount += _upgradeResourceIncrement;
                _upgradeResourceAmount = Mathf.Clamp(_upgradeResourceAmount, 0, _upgradeResourceCap);
                _generatorView.UpdateView(_upgradeResourceAmount);
            }
        }

        private void GiveUpgradeResource(IInteractor interactor)
        {
            interactor.UpgradeResource.IncreaceUpgradeResourceAmount(_upgradeResourceAmount);
            _upgradeResourceAmount = 0;
            _generatorView.UpdateView(_upgradeResourceAmount);

            SpawnParticle();
            EnableTakeAbilityDelayed();
        }

        private void SpawnParticle()
        {
            var particle = Instantiate(_pickUpParticle, _particlePoint.transform.position, Quaternion.identity, _particlePoint);
            var main = particle.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
            main.loop = false;
        }

        private void EnableTakeAbilityDelayed()
        {
            _isAbleToTake = false;

            Observable.Timer(TimeSpan.FromSeconds(_delayBeforeStartAgain)).Subscribe(_ =>
            {
                _isAbleToTake = true;
            }).AddTo(_disposables);
        }
    }
}
