using UnityEngine;

namespace Interactions.UpgradesIsland
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] private Transform _endPoint;

        public Transform EndPoint => _endPoint;

        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;

            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
        }
    }
}
