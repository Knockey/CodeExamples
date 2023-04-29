using Extensions;
using System.Collections;
using UnityEngine;

namespace Interactions
{
    public class CityOverviewCamera : MonoBehaviour
    {
        [SerializeField] private Cinemachine.CinemachineVirtualCamera _overviewCamera;
        [SerializeField] private Transform _rotationPoint;
        [SerializeField, Min(0f)] private float _cameraSpeed = 15f;

        public void EnableOverview()
        {
            _overviewCamera.Priority = 10000;
            StartCoroutine(RotateCamera());
        }

        private IEnumerator RotateCamera()
        {
            while (enabled)
            {
                var angles = _rotationPoint.rotation.eulerAngles.With(y: _rotationPoint.rotation.eulerAngles.y + _cameraSpeed * Time.deltaTime);
                _rotationPoint.rotation = Quaternion.Euler(angles);

                yield return null;
            }
        }
    }
}
