using UnityEngine;

namespace Airplane
{
    public interface IMover 
    {
        public void Move(Vector3 direction, Joystick stick);
        public void Rotate(Vector3 direction, Joystick stick);
    }
}
