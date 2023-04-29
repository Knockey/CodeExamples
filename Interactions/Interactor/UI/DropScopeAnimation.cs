using UnityEngine;

namespace Interactions
{
    public class DropScopeAnimation : MonoBehaviour
    {
        private Quaternion _defaultRotation;

        private void Awake()
        {
            _defaultRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation = _defaultRotation;
        }
    }
}
