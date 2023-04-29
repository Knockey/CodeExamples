using Extensions;
using LoadingScreenView;
using SaveLoad;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Upgrades;

namespace Interactions.UpgradesIsland
{
    public class ConveyorObjectSpawner : MonoBehaviour, IUpgradable, ISaveable
    {
        private const string OpenAnimation = "IsOpen";

        [SerializeField] private ConveyorObject _objectPrefab;
        [SerializeField] private ConveyorSpeedModifier _speedModifier;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField, Min(0f)] private float _timeBetweenPacks = 0.15f;
        [SerializeField] private Animator _moneyMakerAnimator;
        [Header("Buy spawner settings")]
        [SerializeField] private bool _isBoughtFromStart = false;
        [SerializeField] private Transform _mesh;
        [SerializeField, Min(0)] private int _spawnerBuyCost = 10;
        [SerializeField] private ViewPanel _buySpawnerViewPanel;
        [SerializeField] private Button _spawnerBuyButton;
        [SerializeField] private TextView _costText;
        [Header("Upgrades")]
        [SerializeField, Min(0f)] private float _refillAmount = 8000f;
        [SerializeField, Min(1)] private int _packsCount = 1;
        [SerializeField] private UpgradeData _upgradeData;
        [SerializeField] private ConveyorUpgradeView _conveyorUpgradeView;

        private Coroutine _spawnCoroutine;
        private float _currentSpawnTime => _upgradeData.GetModifiedUpgradeValue() / _speedModifier.CurrentSpawnSpeedModifier;

        public bool IsEnabled { get; private set; } = false;
        public int SpawnerCost => _spawnerBuyCost;
        public UpgradeData UpgradeData => _upgradeData;
        public float CurrentRefillPerTick => _refillAmount;
        public string Name => gameObject.name;
        public int CurrentLevel => _upgradeData.CurrentUpgradeLevel;

        public event Action<ConveyorObjectSpawner> TryBuySpawner;
        public event Action<ConveyorObjectSpawner> TryUpgradeSpawner;
        public event Action NeedToSave;

        private void Awake()
        {
            DisableObject();
            _costText.SetTextFormated(_spawnerBuyCost.ToString());
        }

        private void OnEnable()
        {
            _spawnerBuyButton.onClick.AddListener(OnSpawnerBuyButtonClicked);
            _conveyorUpgradeView.UpgradeButtonClicked += OnSpawnerUpgradeButtonClicked;
        }

        private void OnDisable()
        {
            _spawnerBuyButton.onClick.RemoveListener(OnSpawnerBuyButtonClicked);
            _conveyorUpgradeView.UpgradeButtonClicked -= OnSpawnerUpgradeButtonClicked;
        }

        public void EnableUpgradeView()
        {
            if (IsEnabled)
                _conveyorUpgradeView.EnableView();
        }

        public void DisableUpgradeView()
        {
            if (IsEnabled)
                _conveyorUpgradeView.DisableView();
        }

        public void EnableObject()
        {
            IsEnabled = true;

            _mesh.gameObject.SetActive(true);
            _buySpawnerViewPanel.DisableView();
            _conveyorUpgradeView.EnableView();

            _spawnCoroutine = StartCoroutine(SpawnObjects());
            NeedToSave?.Invoke();
        }

        public void UpdateButtonsState(IInteractor interactor)
        {
            _spawnerBuyButton.interactable = interactor.UpgradeResource.IsAbleToDecreaseResourceAmount(SpawnerCost);

            var currentCost = _upgradeData.GetCurrentUpgradeCost();
            var isInteractable = _upgradeData.IsAbleToUpgrade && interactor.UpgradeResource.IsAbleToDecreaseResourceAmount(currentCost);
            var incomePerSecond = (_refillAmount * _packsCount) / (_upgradeData.GetModifiedUpgradeValue() + (_timeBetweenPacks * _packsCount));

            _conveyorUpgradeView.UpdateView(incomePerSecond, isInteractable, UpgradeData.IsAbleToUpgrade, _upgradeData.CurrentUpgradeLevel, currentCost);
        }

        public void UpgradeSpawner()
        {
            var upgradeLevel = _upgradeData.CurrentUpgradeLevel + 1;
            _upgradeData.SetUpgradeLevel(upgradeLevel);
        }

        public int GetSaveValue()
        {
            return IsEnabled ? 1 : 0;
        }

        public void LoadSaveData(SaveData saveData)
        {
            if (saveData.ValueToSave == 1 || (saveData.ValueToSave == 0 && _isBoughtFromStart))
                EnableObject();
        }

        private void DisableObject()
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }

            IsEnabled = false;
            _mesh.gameObject.SetActive(false);
            _buySpawnerViewPanel.EnableView();
            _conveyorUpgradeView.DisableView();
        }

        private IEnumerator SpawnObjects()
        {
            while (enabled)
            {
                yield return new WaitForSeconds(_currentSpawnTime);

                _moneyMakerAnimator.SetBool(OpenAnimation, true);

                for (int index = 0; index < _packsCount; index += 1)
                {
                    Spawn();
                    yield return new WaitForSeconds(_timeBetweenPacks);
                }

                _moneyMakerAnimator.SetBool(OpenAnimation, false);
            }
        }

        private void Spawn()
        {
            var obj = Instantiate(_objectPrefab, _spawnPoint.position, _objectPrefab.transform.rotation, _spawnPoint.parent);
            obj.Init(_endPoint, CurrentRefillPerTick, _speedModifier);
        }

        private void OnSpawnerBuyButtonClicked()
        {
            TryBuySpawner?.Invoke(this);
        }

        private void OnSpawnerUpgradeButtonClicked()
        {
            TryUpgradeSpawner?.Invoke(this);
        }
    }
}
