using Airplane;
using Analytics;
using BarUI;
using Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace GameState
{
    public class LevelProgressionHandler : MonoBehaviour, IValueChanger
    {
        private const int MaxCompetePercent = 1;

        [SerializeField] private CityOverviewCamera _cityOverviewCamera;
        [SerializeField, Min(0f)] private float _delayTime = 7.5f;
        [Header("Interactable objects")]
        [SerializeField] private List<MonoBehaviour> _interactableMonoBehaviours;
        [Header("Upgrade resource generators")]
        [SerializeField] private List<UpgradeResourceGenerator> _upgradeResourceGenerators;

        private bool _isPlayerWon = false;
        private List<IInteractable> _interactables = new();
        private CompositeDisposable _disposables;
        private PlayerMovement _playerMovement;

        public List<IInteractable> Interactables => _interactables;

        public event Action<float, float> ValueInited;
        public event Action<float, float> ValueChanged;
        public event Action PlayerWon;

        private void OnValidate()
        {
            if (_interactables.Count > 0)
            {
                _interactableMonoBehaviours.ForEach(obj =>
                {
                    if (obj == null)
                        return;

                    if (obj is IInteractable)
                        return;

                    Debug.LogWarning(obj.name + " needs to implement " + nameof(IInteractable));
                    obj = null;
                });
            }
        }

        private void Awake()
        {
            _interactableMonoBehaviours.ForEach(obj =>
            {
                _interactables.Add(obj as IInteractable);
            });

            _disposables = new CompositeDisposable();
        }

        private void OnDisable()
        {
            _interactables.ForEach(interactable => interactable.InteractionPercentUpdated -= OnInteractionPercentUpdated);
            _disposables?.Dispose();
        }

        private void Start()
        {
            var currentProgressPercent = GetCurrentProgressPercent();
            ValueInited?.Invoke(currentProgressPercent, 1f);
        }

        public void Init(PlayerMovement playerMovement)
        {
            _playerMovement = playerMovement;
            _interactables.ForEach(interactable => interactable.InteractionPercentUpdated += OnInteractionPercentUpdated);
        }

        private void OnInteractionPercentUpdated()
        {
            var currentProgressPercent = GetCurrentProgressPercent();
            ValueChanged?.Invoke(currentProgressPercent, 1f);

            CheckIfPlayerWon(currentProgressPercent);
        }

        private float GetCurrentProgressPercent()
        {
            var interactionPercentSum = 0f;
            _interactables.ForEach(interactable =>
            {
                interactionPercentSum += interactable.CurrentCompletionPercent;
            });

            return interactionPercentSum / _interactables.Count;
        }

        private void CheckIfPlayerWon(float currentProgressPercent)
        {
            if (currentProgressPercent >= MaxCompetePercent && _isPlayerWon == false)
            {
                _isPlayerWon = true;

                FreeplayAnalytics.Instance.SendLevelCompleteEvent();
                UpdateCompletedLevelsCount();
                EnableOverviewDelayed();
            }
        }

        private void UpdateCompletedLevelsCount()
        {
            var completedLevelsCount = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0);
            completedLevelsCount += 1;
            PlayerPrefs.SetInt(LevelPrefs.CompletedLevels, completedLevelsCount);
        }

        private void EnableOverviewDelayed()
        {
            Observable.Timer(TimeSpan.FromSeconds(_delayTime))
                .Subscribe(_ =>
                {
                    _upgradeResourceGenerators.ForEach(generator => generator.DisableView());
                    _cityOverviewCamera.EnableOverview();
                    _playerMovement.DisableStick();
                    PlayerWon?.Invoke();
                })
                .AddTo(_disposables);
        }

#if UNITY_EDITOR
        public void FillInteractablesList()
        {
            var saveableList = GetComponentsInChildren<MonoBehaviour>().ToList();
            _interactableMonoBehaviours.Clear();

            saveableList.ForEach(obj =>
            {
                if (obj is IInteractable)
                {
                    _interactableMonoBehaviours.Add(obj);
                }
            });

            Save();
        }

        public void FillGeneratorsLists()
        {
            _upgradeResourceGenerators = GetComponentsInChildren<UpgradeResourceGenerator>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
