using Analytics;
using LoadingScreenView;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneLoad
{
    [RequireComponent(typeof(GameSceneInit))]
    public class GameSceneLoad : MonoBehaviour
    {
        [SerializeField] private ViewPanel _loadingScreenView;
        [SerializeField, Min(0f)] private float _loadingScreenTime = 1f;
        [Header("Next scene load")]
        [SerializeField] private List<InteractionObjectsData> _levelPrefabs;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Button _nextLevelButton;

        private GameSceneInit _gameSceneInit;
        private CompositeDisposable _disposables;

        private void Awake()
        {
            _gameSceneInit = GetComponent<GameSceneInit>();
            _disposables = new CompositeDisposable();

            ShowLoadingScreen();
            LoadLevel();
        }

        private void OnEnable()
        {
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClick);
        }

        private void OnDisable()
        {
            _disposables.Dispose();
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClick);
        }

        private void ShowLoadingScreen()
        {
            _loadingScreenView.EnableView();

            Observable.Timer(System.TimeSpan.FromSeconds(_loadingScreenTime)).Subscribe(_ =>
            {
                _loadingScreenView.DisableView();
            }).AddTo(_disposables);
        }

        private void LoadLevel()
        {
            var currentLevel = GetLevelToLoad();
            var levelInstance = Instantiate(currentLevel, _spawnPoint);
            _gameSceneInit.InitScene(levelInstance);
        }

        private InteractionObjectsData GetLevelToLoad()
        {
            var completedLevelsCount = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0);
            var levelIndex = completedLevelsCount % _levelPrefabs.Count;

            return _levelPrefabs[levelIndex];
        }

        private void OnNextLevelButtonClick()
        {
            _loadingScreenView.EnableView();
            FreeplayAnalytics.Instance.SendLevelStartEvent();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
