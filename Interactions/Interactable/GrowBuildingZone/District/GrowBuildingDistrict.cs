using Airplane;
using Analytics;
using BarUI;
using Extensions;
using Interactions.Resources;
using LoadingScreenView;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Interactions
{
    public class GrowBuildingDistrict : MonoBehaviour, IValueChanger
    {
        [SerializeField] private GrowBuildingDistrict _nextDistrict;
        [SerializeField] private ViewPanel _disabledDistrictCanvas;
        [SerializeField, Min(0)] private int _resourceForDistrictComplete = 50;
        [Header("Props enable")]
        [SerializeField, Min(0f)] private float _propsDropDelay = 1f;
        [SerializeField] private DistrictDoneObjects _districtDoneObjectsParent;
        [SerializeField] private Humans.HumanEnableList _humanEnableList;
        [Header("Confetti particles")]
        [SerializeField] private List<Transform> _particlePoints;
        [SerializeField] private ParticleSystem _confettiParticles;
        [SerializeField, Min(0f)] private float _betweenParticlesDelay = 0.25f;
        [Header("District complete UI")]
        [SerializeField, Min(0f)] private float _uiTime = 2.5f;
        [Header("Camera switch")]
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _districtCamera;
        [SerializeField, Min(0f)] private float _cameraOnDistrictTime = 2f;
        [SerializeField, Min(0f)] private float _districtCompleteCameraTime = 5f;
        [Header("Interactable objects")]
        [SerializeField] private bool _isActiveFromStart = false;
        [SerializeField] private List<MonoBehaviour> _interactableMonoBehaviours;
        [Header("Transparent objects")]
        [SerializeField] private List<GrowBuildingBlinkList> _growBuildingBlinkLists;
        [Header("Upgrade resource generators")]
        [SerializeField] private List<UpgradeResourceGenerator> _upgradeResourceGenerators;

        private PlayerMovement _playerMovement;
        private UpgradeResource _upgradeResource;
        private DistrictCompleteView _districtCompleteView;
        private ViewPanel _lowResourceView;
        private ViewPanel _needToBuildDirectionView;
        private List<IInteractable> _interactables = new();
        private CompositeDisposable _disposables;
        private bool _isLoading = true;

        public Cinemachine.CinemachineVirtualCamera DistrictCamera => _districtCamera;
        public float CurrentCompletionPercent
        {
            get
            {
                float sum = 0;
                _interactables.ForEach(interactable => sum += interactable.CurrentCompletionPercent);
                return sum / _interactables.Count;
            }
        }

        public event Action DisctictAnimationComplete;
        public event Action<float, float> ValueInited;
        public event Action<float, float> ValueChanged;

        private void OnValidate()
        {
            if (_interactables.Count > 0)
            {
                _interactableMonoBehaviours.ForEach(obj =>
                {
                    GenericInterfaceInjection.TrySetObject<IInteractable>(ref obj);
                });
            }
        }

        private void Awake()
        {
            _interactableMonoBehaviours.ForEach(obj =>
            {
                _interactables.Add(obj as IInteractable);
            });
        }

        private void OnEnable()
        {
            _interactables.ForEach(interactable => interactable.InteractionPercentUpdated += OnInteractionPercentUpdated);
            _disposables = new CompositeDisposable();
        }

        private void OnDisable()
        {
            _interactables.ForEach(interactable => interactable.InteractionPercentUpdated -= OnInteractionPercentUpdated);
            _disposables?.Dispose();
        }

        private void Start()
        {
            Observable.Timer(System.TimeSpan.FromSeconds(_cameraOnDistrictTime)).Subscribe(_ => _isLoading = false).AddTo(_disposables);
            ValueInited?.Invoke(CurrentCompletionPercent, 1f);
        }

        public void Init(PlayerMovement playerMovement, UpgradeResource upgradeResource, DistrictCompleteView districtCompleteView, 
            ViewPanel lowResourceView, ViewPanel needToBuildDirectionView, ViewPanel airplaneUpgradeUI)
        {
            _playerMovement = playerMovement;
            _upgradeResource = upgradeResource;
            _districtCompleteView = districtCompleteView;
            _lowResourceView = lowResourceView;
            _needToBuildDirectionView = needToBuildDirectionView;

            InitObjects(airplaneUpgradeUI);
        }

        public void EnableAllTransparent()
        {
            _growBuildingBlinkLists.ForEach(growBuildingBlinkList => growBuildingBlinkList.EnableAll());
            _disabledDistrictCanvas.DisableView();
        }

        public void DisableAllTransparent()
        {
            _growBuildingBlinkLists.ForEach(growBuildingBlinkList => growBuildingBlinkList.DisableAll());
        }

        public void EnableInteractables()
        {
            _interactables.ForEach(interactable => interactable.TryEnableInteractable());
            _disabledDistrictCanvas.DisableView();
        }

        public void DisableInteractables()
        {
            _interactables.ForEach(interactable => interactable.DisableInteractable());
        }

        private void InitObjects(ViewPanel airplaneUpgradeUI)
        {
            if (_isActiveFromStart)
            {
                _disabledDistrictCanvas.DisableView();

                var currentLevel = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0);
                if (currentLevel > 0 && CurrentCompletionPercent == 0)
                {
                    ShowEnabledDistrict(airplaneUpgradeUI);
                }
            }
            else
            {
                _interactables.ForEach(interactable => interactable.DisableInteractable());
                _disabledDistrictCanvas.EnableView();
            }

            _districtDoneObjectsParent?.InitObjects();
            _humanEnableList.DisableObjects();
            _districtCompleteView.DisableView();
        }

        private void ShowEnabledDistrict(ViewPanel airplaneUpgradeUI)
        {
            DistrictCamera.Priority = 1001;

            Observable.Timer(System.TimeSpan.FromSeconds(0.5f))
                .Subscribe(_ =>
                {
                    DisableInteractables();
                    EnableAllTransparent();
                    airplaneUpgradeUI.DisableView();
                }).AddTo(_disposables);

            Observable.Timer(System.TimeSpan.FromSeconds(_cameraOnDistrictTime + _uiTime))
                .Subscribe(_ =>
                {
                    DistrictCamera.Priority = 1;
                    DisableAllTransparent();
                    EnableInteractables();
                    airplaneUpgradeUI.EnableView();
                    _upgradeResourceGenerators.ForEach(generator => generator.TryEnableView());
                }).AddTo(_disposables);
        }

        private void OnInteractionPercentUpdated()
        {
            var currentProgressPercent = GetCurrentProgressPercent();
            TryEnableNextDistrict(currentProgressPercent);
            ValueChanged?.Invoke(CurrentCompletionPercent, 1f);
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

        private void TryEnableNextDistrict(float currentProgressionPercent)
        {
            if (currentProgressionPercent < 1f)
            {
                return;
            }

            if (_isLoading)
            {
                LoadProps();
                return;
            }

            _nextDistrict?.DisableInteractables();

            DisableStickForEndscreen();
            SwitchCameraOnCurrentDistrictDelayed();
            SpawnPropsDelayed();
            EnableCompleteDistrictViewDelayed();
        }

        private void LoadProps()
        {
            _districtDoneObjectsParent?.EnableObjects();
            _humanEnableList.EnableObjects();
            _nextDistrict?.DisableAllTransparent();
            _nextDistrict?.EnableInteractables();
        }

        private void DisableStickForEndscreen()
        {
            _playerMovement.DisableStick();
            _lowResourceView.DisableView();
            _needToBuildDirectionView.DisableView();
            _upgradeResourceGenerators.ForEach(generator => generator.DisableView());

            var delayTime = _nextDistrict != null ? _districtCompleteCameraTime + _uiTime + _cameraOnDistrictTime : _districtCompleteCameraTime + _uiTime;

            Observable.Timer(System.TimeSpan.FromSeconds(delayTime))
                .Subscribe(_ =>
                {
                    _playerMovement.EnableStick();
                })
                .AddTo(_disposables);
        }

        private void SwitchCameraOnCurrentDistrictDelayed()
        {
            _districtCamera.Priority = 1000;

            Observable.Timer(System.TimeSpan.FromSeconds(_districtCompleteCameraTime + _uiTime))
                .Subscribe(_ =>
                {
                    _districtCamera.Priority = 1;
                })
                .AddTo(_disposables);
        }

        private void SpawnPropsDelayed()
        {
            Observable.Timer(System.TimeSpan.FromSeconds(_propsDropDelay)).Subscribe(_ =>
            {
                _upgradeResourceGenerators.ForEach(generator => generator.DisableView());
                _districtDoneObjectsParent?.EnableObjectsByTime(_districtCompleteCameraTime - _propsDropDelay);
                _humanEnableList.EnableObjectsByTimeWithParticles(_districtCompleteCameraTime - _propsDropDelay);
            }).AddTo(_disposables);
        }

        private void EnableCompleteDistrictViewDelayed()
        {
            Observable.Timer(System.TimeSpan.FromSeconds(_districtCompleteCameraTime)).Subscribe(_ =>
            {
                _upgradeResource.IncreaceUpgradeResourceAmount(_resourceForDistrictComplete);
                _districtCompleteView.Enable(_resourceForDistrictComplete);
                StartCoroutine(ParticlesSpawn());

                Observable.Timer(System.TimeSpan.FromSeconds(_uiTime)).Subscribe(_ =>
                {
                    _districtCompleteView.DisableView();

                    if (_nextDistrict == null)
                    {
                        return;
                    }

                    SwitchToNextDistrict();
                }).AddTo(_disposables);
            }).AddTo(_disposables);
        }

        private IEnumerator ParticlesSpawn()
        {
            var tick = new WaitForSeconds(_betweenParticlesDelay);

            for (int particlePointIndex = 0; particlePointIndex < _particlePoints.Count; particlePointIndex += 1)
            {
                var particle = Instantiate(_confettiParticles, _particlePoints[particlePointIndex]);
                var main = particle.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                main.loop = false;

                yield return tick;
            }
        }

        private void SwitchToNextDistrict()
        {
            _nextDistrict.DistrictCamera.Priority = 1001;
            _nextDistrict.DisableInteractables();
            _nextDistrict.EnableAllTransparent();

            FreeplayAnalytics.Instance.SendDistrictOpenEvent(_nextDistrict.gameObject.name);

            Observable.Timer(System.TimeSpan.FromSeconds(_cameraOnDistrictTime))
                .Subscribe(_ =>
                {
                    _nextDistrict.DistrictCamera.Priority = 1;
                    _nextDistrict.DisableAllTransparent();
                    _nextDistrict.EnableInteractables();
                    _lowResourceView.EnableView();
                    _needToBuildDirectionView.EnableView();
                    _upgradeResourceGenerators.ForEach(generator => generator.TryEnableView());

                    DisctictAnimationComplete?.Invoke();
                }).AddTo(_disposables);
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

        public void FillTransparentObjectsLists()
        {
            _growBuildingBlinkLists = GetComponentsInChildren<GrowBuildingBlinkList>().ToList();
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
