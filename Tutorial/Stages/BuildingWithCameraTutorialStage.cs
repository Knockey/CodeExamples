using Interactions;
using System.Collections;
using UnityEngine;

namespace Tutorial
{
    public class BuildingWithCameraTutorialStage : BuildTutorialStage
    {
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _camera;
        [SerializeField, Min(0f)] private float _cameraTime = 2f;

        public override void StartStage(int stageIndex, UpgradeZone upgradeZone)
        {
            base.StartStage(stageIndex, upgradeZone);
            StartCoroutine(WaitForCamera());
        }

        private IEnumerator WaitForCamera()
        {
            _camera.Priority = 100;
            yield return new WaitForSeconds(_cameraTime);
            _camera.Priority = 1;
        }
    }
}
