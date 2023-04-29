using LoadingScreenView;
using System.Collections.Generic;
using UnityEngine;

namespace GameState
{
    public class EndScreenView : MonoBehaviour
    {
        [SerializeField] private ViewPanel _endScreenPanel;
        [SerializeField] private List<ViewPanel> _panelsToDisable;

        private LevelProgressionHandler _levelProgressionHandler;

        private void Awake()
        {
            _endScreenPanel.DisableView();
        }

        private void OnEnable()
        {
            _levelProgressionHandler.PlayerWon += OnPlayerWon;
        }

        private void OnDisable()
        {
            _levelProgressionHandler.PlayerWon -= OnPlayerWon;
        }

        public void Init(LevelProgressionHandler levelProgressionHandler)
        {
            _levelProgressionHandler = levelProgressionHandler;
        }

        private void OnPlayerWon()
        {
            _endScreenPanel.EnableView();
            _panelsToDisable.ForEach(viewPanel => viewPanel.DisableView());
        }
    }
}
