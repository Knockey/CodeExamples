using UniRx;
using UnityEngine;

namespace City
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public class Bus : Car
    {
        [Header("Bus")]
        [SerializeField, Min(0f)] private float _stopTime = 1f;

        private CompositeDisposable _disposables;

        protected override void Awake()
        {
            base.Awake();
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;

            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }

        private void OnEnable()
        {
            _disposables = new CompositeDisposable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _disposables?.Dispose();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BusStop busStop))
            {
                CarMovement.StopMovement();
                Observable.Timer(System.TimeSpan.FromSeconds(_stopTime)).Subscribe(_ => CarMovement.StartMovement()).AddTo(_disposables);
            }
        }
    }
}
