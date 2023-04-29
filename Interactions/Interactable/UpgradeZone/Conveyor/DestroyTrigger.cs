using System;
using UnityEngine;

namespace Interactions.UpgradesIsland
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class DestroyTrigger : MonoBehaviour
    {
        [Header("Income view")]
        [SerializeField] private Transform _viewSpawnPoint;
        [SerializeField] private ResourceIncomeView _resourceIncomeView;

        private bool _isViewEnabled = false;

        public event Action<IRefillResource> Collided;

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
            if (other.TryGetComponent(out IRefillResource refillResource))
            {
                TrySpawnView(refillResource);

                Collided?.Invoke(refillResource);
            }
        }

        private void TrySpawnView(IRefillResource refillResource)
        {
            if (_isViewEnabled == false)
                return;

            var incomeView = Instantiate(_resourceIncomeView, _viewSpawnPoint.position, _resourceIncomeView.transform.rotation, _viewSpawnPoint);
            incomeView.Init(refillResource.ResourceAmount);
        }

        public void EnableView()
        {
            _isViewEnabled = true;
        }

        public void DisableView()
        {
            _isViewEnabled = false;
        }
    }
}
