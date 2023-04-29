using UnityEngine;

namespace Interactions.Resources
{
    [RequireComponent(typeof(Collider))]
    public class ResourceCollisionCollider : MonoBehaviour
    {
        [SerializeField] private float _speedMoveOutCollider = 2.5f;

        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent(out IConveyorCollidable _))
            {
                MoveOutFromCollider(other);
            }
        }

        private void MoveOutFromCollider(Collider collider)
        {
            Vector3 closestPoint = _collider.ClosestPoint(collider.transform.position);
            Vector3 direction = new Vector3(closestPoint.x - transform.position.x, 0f, closestPoint.z - transform.position.z);
            collider.transform.Translate(direction * _speedMoveOutCollider * Time.fixedDeltaTime);
        }
    }
}
