using UnityEngine;

namespace Airplane
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class SpeedChangeTrigger : MonoBehaviour
    {
        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;

            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out MoveSpeed moveSpeed))
                moveSpeed.SwitchSpeed();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out MoveSpeed moveSpeed))
                moveSpeed.SwitchSpeed();
        }
    }
}
