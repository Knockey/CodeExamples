using Extensions;
using Interactions;
using LoadingScreenView;
using System.Collections;
using UnityEngine;

namespace Tutorial
{
    public class MovementTutorialStage : TutorialStage
    {
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _mesh;
        [SerializeField] private Transform _playerPoint;
        [SerializeField] private ViewPanel _movementViewPanel;
        [SerializeField] private StageArrow _stageArrow;
        [SerializeField] private NeedToBuildDirectionView _needToBuildDirectionView;
        [SerializeField, Min(0f)] private float _uiTime = 3f;

        private UpgradeZone _upgradeZone;

        private void Awake()
        {
            _movementViewPanel.DisableView();
            _stageArrow.DisableArrow();
        }

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);

            _player.position = _playerPoint.position;
            _mesh.position = _player.position.With(y: _mesh.position.y);

            _needToBuildDirectionView.Enable();
            _movementViewPanel.EnableView();
            _stageArrow.EnableArrow();

            _upgradeZone = upgradeZone;
            _upgradeZone.transform.position = new Vector3(-1000f, -1000f, -1000f);

            StartCoroutine(WaitForTap());
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            base.EndStage(tutorialStage);
            _movementViewPanel.DisableView();
            _stageArrow.DisableArrow();
        }

        private IEnumerator WaitForTap()
        {
            while (Input.GetMouseButton(0) == false)
            {
                yield return null;
            }

            yield return new WaitForSeconds(_uiTime);
            EndStage(this);
        }
    }
}
