using UnityEngine;

namespace Airplane
{
    public interface ILander
    {
        public Transform Transform { get; }
        public float AnimationTime { get; }

        public void Land(Transform landingPoint);
        public void TakeOff();
    }
}
