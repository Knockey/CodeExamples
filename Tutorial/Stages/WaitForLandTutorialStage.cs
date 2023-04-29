using Interactions;
using Interactions.UpgradesIsland;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Tutorial
{
    public class WaitForLandTutorialStage : TutorialStage
    {
        [SerializeField] private Transform _airportPoint;
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;
        [SerializeField, Min(0f)] private float _cameraTime = 2f;

        private UpgradeZone _upgradeZone;

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);

            var conveyorObjects = FindObjectsOfType<ConveyorObject>().ToList();
            conveyorObjects.ForEach(conveyorObject => Destroy(conveyorObject.gameObject));

            StartCoroutine(WaitForCamera());

            _upgradeZone = upgradeZone;
            _upgradeZone.transform.position = _airportPoint.transform.position;
            _upgradeZone.Landed += OnLanded;
        }

        protected override void EndStage(TutorialStage tutorialStage)
        {
            base.EndStage(tutorialStage);
            _upgradeZone.Landed -= OnLanded;
        }

        private void OnLanded()
        {
            EndStage(this);
        }

        private IEnumerator WaitForCamera()
        {
            _camera.Priority = 100;
            yield return new WaitForSeconds(_cameraTime);
            _camera.Priority = 1;
        }
    }
}
