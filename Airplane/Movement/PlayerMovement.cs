using Extensions;
using System.Collections;
using UnityEngine;

namespace Airplane
{
    public class PlayerMovement : MonoBehaviour, IMovement
    {
        [SerializeField] private Transform _camera;
        [Header("Movement settings")]
        [SerializeField] private Joystick _joystick;
        [SerializeField] private MonoBehaviour _moverBehaviour;

        private Coroutine _movement;
        private IMover _mover => (IMover)_moverBehaviour;

        private void OnValidate()
        {
            GenericInterfaceInjection.TrySetObject<IMover>(ref _moverBehaviour);
        }

        private void Start()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.With(y: _camera.rotation.eulerAngles.y));
            StartMovement();
        }

        public void StartMovement()
        {
            _joystick.Enable();
            _movement = StartCoroutine(MovePerFrame());
        }

        public void EnableStick()
        {
            _joystick.Enable();
        }

        public void DisableStick()
        {
            _joystick.Disable();
        }

        public void StopMovement()
        {
            _joystick.Disable();
            if (_movement != null)
            {
                StopCoroutine(_movement);
            }
        }

        private IEnumerator MovePerFrame()
        {
            var movementDirection = transform.forward;
            var rotationDirection = transform.up;

            while (enabled)
            {
                _mover.Move(movementDirection, _joystick);
                _mover.Rotate(rotationDirection, _joystick);
                yield return null;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 10f);
        }
#endif
    }
}
