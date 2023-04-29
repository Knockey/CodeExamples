using FreeplaySDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Analytics
{
    public class FreeplayAnalytics : Singleton<FreeplayAnalytics>
    {
        private const int OneMinuteConst = 60;
        private const string TUTORIAL_STAGE_COMPLETE = "tutorial_stage_complete";
        private const string AIRPLANE_UPGRADE = "airplane_upgrade";
        private const string CONVEYOR_UPGRADE = "conveyor_upgrade";
        private const string DISTRICT_OPEN = "district_open";
        private const string TIME_SPENT = "time_spent";

        [SerializeField, Min(0)] private float _oneMinuteBorder = 20;
        [SerializeField, Min(0)] private int _timeToSendEventsAfter = 5;

        private int _timeSpent = 0;
        private Coroutine _timer;

        public void StartAnalyticsTimer()
        {
            _timer = StartCoroutine(StartTimer());
        }

        public void SendLevelStartEvent()
        {
            var levelIndex = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0);
            Freeplay.NotifyLevelStart(levelIndex);
        }

        public void SendLevelCompleteEvent()
        {
            var levelIndex = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0);
            Freeplay.NotifyLevelCompleted(levelIndex, true);
        }

        public void SendTutorialStageCompleteEvent(string stageName)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "stage_name", stageName }
            };

            SendAppMetricaEvent(TUTORIAL_STAGE_COMPLETE, properties);
        }

        public void SendDistrictOpenEvent(string districtName)
        {
            var levelIndex = PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0).ToString();

            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "level", levelIndex },
                { "district", districtName }
            };

            SendAppMetricaEvent(DISTRICT_OPEN, properties);
        }

        public void SendConveyorUpgradeEvent(string conveyorName, ConveyorUpgradeType conveyorUpgradeType, int levelIndex)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "name", conveyorName },
                { "type", conveyorUpgradeType },
                { "level", levelIndex }
            };

            SendAppMetricaEvent(CONVEYOR_UPGRADE, properties);
        }

        public void SendAirplaneUpgradeEvent(AirplaneUpgradeType airplaneUpgradeType, int levelIndex)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "type", airplaneUpgradeType },
                { "level", levelIndex }
            };

            SendAppMetricaEvent(AIRPLANE_UPGRADE, properties);
        }

        private IEnumerator StartTimer()
        {
            _timeSpent = 0;
            var tick = new WaitForSecondsRealtime(1f);

            while (enabled)
            {
                yield return tick;
                _timeSpent += 1;

                if ((_timeSpent <= _oneMinuteBorder * OneMinuteConst && _timeSpent % OneMinuteConst == 0)
                    || (_timeSpent > _oneMinuteBorder * OneMinuteConst && _timeSpent % (OneMinuteConst * _timeToSendEventsAfter) == 0))
                {
                    var timeSpentCurrent = _timeSpent / OneMinuteConst;
                    SendTimeSpentEvent(timeSpentCurrent);
                }
            }
        }

        private void SendTimeSpentEvent(int time)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>
            {
                { "time", time },
            };

            SendAppMetricaEvent(TIME_SPENT, properties);
        }

        private static void SendAppMetricaEvent(string eventName, Dictionary<string, object> properties)
        {
#if FREEPLAY_APPMETRICA_ENABLED
            var debugString = "";

            foreach (var property in properties)
            {
                debugString += property.Key.ToString() + " " + property.Value.ToString() + " ";
            }

            AppMetrica.Instance.ReportEvent(eventName, properties);
            Freeplay.LogInfo("Send Event AppMetrica " + eventName + " " + debugString);
#endif
        }
    }
}
