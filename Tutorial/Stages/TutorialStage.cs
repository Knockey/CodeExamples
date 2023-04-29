using Interactions;
using System;
using UnityEngine;

namespace Tutorial
{
    public abstract class TutorialStage : MonoBehaviour
    {
        private const string StagePref = "_STAGE_COMPLETE";
        private const string StageCompletePref = "COMPLETED";

        [SerializeField, Min(0)] private int _stageIndex;
        [SerializeField] private TutorialStageType _stageName;

        public int StageIndex => _stageIndex;
        public string StageName => _stageName.ToString() + _stageIndex;

        public event Action<TutorialStage> StageComplete;

        public virtual void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            if (IsStageComplete(stageIndex))
                throw new TutorialException(_stageName, nameof(IsStageComplete));
        }

        public virtual bool IsStageComplete(int stageIndex)
        {
            return PlayerPrefs.HasKey((_stageName + "_" + stageIndex).ToString().ToUpper() + StagePref);
        }

        public void SaveStageCompletion(int stageIndex)
        {
            PlayerPrefs.SetString((_stageName + "_" + stageIndex).ToString().ToUpper() + StagePref, StageCompletePref);
        }

        protected virtual void EndStage(TutorialStage tutorialStage)
        {
            StageComplete?.Invoke(tutorialStage);
        }
    }
}
