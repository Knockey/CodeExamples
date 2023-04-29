using Analytics;
using Interactions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        private const string TutorialPref = "TUTORIAL_COMPLETE";
        private const string CompletePref = "COMPLETED";

        [SerializeField] private UpgradeZone _upgradeZone;

        [SerializeField] private List<TutorialStage> _tutorialStages;

        private int _currentStageIndex = 0;

        private void Start()
        {
            if (PlayerPrefs.GetString(TutorialPref, "") == CompletePref || PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0) > 0)
            {
                Destroy(gameObject);
                return;
            }

            LoadTutorialStages();
            TryStartNextTutorialStage();
        }

        private void LoadTutorialStages()
        {
            _tutorialStages = FindObjectsOfType<TutorialStage>().OrderBy(stage => stage.StageIndex).ToList();
        }

        private void TryStartNextTutorialStage()
        {
            if (_tutorialStages.Count == 0)
                throw new TutorialException();

            var currentStage = _tutorialStages[_currentStageIndex];

            if (currentStage == null)
                throw new TutorialException();

            if (IsStageComplete(currentStage))
                return;

            currentStage.StartStage(_currentStageIndex, _upgradeZone);
            currentStage.StageComplete += OnStageComplete;

            if (currentStage.StageIndex == 0)
            {
                FreeplayAnalytics.Instance.SendLevelStartEvent();
            }
        }

        private bool IsStageComplete(TutorialStage currentStage)
        {
            if (currentStage.IsStageComplete(_currentStageIndex))
            {
                _currentStageIndex += 1;

                if (_currentStageIndex < _tutorialStages.Count)
                {
                    TryStartNextTutorialStage();
                    return true;
                }

                PlayerPrefs.SetString(TutorialPref, CompletePref);
                return true;
            }

            return false;
        }

        private void OnStageComplete(TutorialStage tutorialStage)
        {
            tutorialStage.StageComplete -= OnStageComplete;
            tutorialStage.SaveStageCompletion(_currentStageIndex);
            FreeplayAnalytics.Instance.SendTutorialStageCompleteEvent(tutorialStage.StageName);

            TryStartNextTutorialStage();
        }
    }
}
