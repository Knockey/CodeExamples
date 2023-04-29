using LoadingScreenView;
using UnityEngine;
using UnityEngine.UI;

namespace Analytics
{
    public class AnalyticsDebugger : MonoBehaviour
    {
        [SerializeField] private Button _logButton;
        [SerializeField] private TMPro.TMP_Text _logButtonText;
        [SerializeField] private ViewPanel _logViewPanel;
        [SerializeField] private TMPro.TMP_Text _logText;

        private int _logIndex = 1;

        private void Awake()
        {
#if UNITY_EDITOR == false && DEVELOPMENT_BUILD == false
            Destroy(gameObject);
#else
            _logViewPanel.EnableView();
            _logText.text = "";
            _logButtonText.text = "X";
#endif
        }

        private void OnEnable()
        {
            _logButton.onClick.AddListener(OnLogButtonClick);
        }

        private void OnDisable()
        {
            _logButton.onClick.RemoveListener(OnLogButtonClick);
        }

        public void UpdateLogText(string text)
        {
            Debug.Log(text);
            _logText.text += _logIndex.ToString() + ". " + text + "\n";
            _logIndex += 1;
        }

        private void OnLogButtonClick()
        {
            if (_logViewPanel.IsEnabled)
            {
                _logViewPanel.DisableView();
                _logButtonText.text = "0";
                return;
            }

            _logViewPanel.EnableView();
            _logButtonText.text = "X";
        }
    }
}
