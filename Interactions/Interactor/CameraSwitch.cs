using Cinemachine;
using UnityEngine;

namespace Interactions
{
    public class CameraSwitch : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera _flyCamera;
        [SerializeField] private CinemachineVirtualCamera _landedCamera;

        public void SwitchToLandedCamera()
        {
            _flyCamera.Priority = 0;
            _landedCamera.Priority = 10;
        }

        public void SwitchToFlyCamera()
        {
            _landedCamera.Priority = 0;
            _flyCamera.Priority = 10;
        }
    }
}
