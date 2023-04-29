using UnityEngine;

namespace Airplane
{
    public class MoverFullControl : MonoBehaviour, IMover
    {
        [SerializeField] private MoveSpeed _moveSpeed;
        [SerializeField] private AirplaneFlyAnimation _airplaneFlyAnimation;

        private Borders _borders;
        private float _movementSpeed => _moveSpeed.CurrentSpeed;

        public void Init(Borders borders)
        {
            _borders = borders;
        }

        public void Move(Vector3 direction, Joystick stick)
        {
            _airplaneFlyAnimation.UpdateMeshTransform(_movementSpeed, stick);

            if (stick.Direction == Vector2.zero)
            {
                return;
            }

            var movementSpeed = _movementSpeed * Time.deltaTime;
            direction = new Vector3(stick.Horizontal, 0f, stick.Vertical);
            direction = direction.normalized;

            transform.Translate(direction * movementSpeed);

            var newPosition = transform.position;
            newPosition.x = Mathf.Clamp(newPosition.x, _borders.LeftBorder, _borders.RightBorder);
            newPosition.z = Mathf.Clamp(newPosition.z, _borders.BottomBorder, _borders.TopBorder);
            transform.position = newPosition;
        }

        public void Rotate(Vector3 direction, Joystick stick) { }
    }
}
