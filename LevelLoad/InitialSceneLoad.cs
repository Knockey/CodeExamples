using Analytics;
using FreeplaySDK;
using IJunior.TypedScenes;
using System.Collections;
using UnityEngine;

namespace SceneLoad
{
    public class InitialSceneLoad : MonoBehaviour
    {
        private const string GDPR_KEY = "FreeplayGDPR";

        private void Start()
        {
            StartLoad();
        }

        private void StartLoad()
        {
            if (Freeplay.Settings.PrivacyPolicy && PlayerPrefs.GetInt(GDPR_KEY) == 0)
            {
                StartCoroutine(WaitForPrivacyPolicyAccept());
                return;
            }

            LoadGameScene();
        }

        private IEnumerator WaitForPrivacyPolicyAccept()
        {
            yield return new WaitUntil(() => PlayerPrefs.GetInt(GDPR_KEY) != 0);
            LoadGameScene();
        }

        private static void LoadGameScene()
        {
            FreeplayAnalytics.Instance.StartAnalyticsTimer();
            GameScene.Load();
        }
    }
}
