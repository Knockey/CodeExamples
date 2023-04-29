using UnityEngine;

namespace Airplane
{
    public class MoverConstantForward : MonoBehaviour, IMover
    {
        [Header("Movement settings")]
        [SerializeField] private MoveSpeed _moveSpeed;
        [SerializeField, Min(0f)] private float _rotationSpeed = 150f;
        [Header("Mesh rotation")]
        [SerializeField] private MeshRotation _meshRotation;

        private float _movementSpeed => _moveSpeed.CurrentSpeed;

        public void Move(Vector3 direction, Joystick stick)
        {
            var movementSpeed = _movementSpeed * Time.deltaTime;
            transform.Translate(direction * movementSpeed);
        }

        public void Rotate(Vector3 direction, Joystick stick)
        {
            var angle = _rotationSpeed * stick.Horizontal * Time.deltaTime * direction;
            _meshRotation.RotateMesh(stick.Horizontal);
            transform.Rotate(angle);
        }
    }
}
