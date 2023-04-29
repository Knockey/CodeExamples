using UnityEngine;

namespace Airplane
{
    public class Borders : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _distanceX = 10;
        [SerializeField, Min(0f)] private float _distanceZ = 10;

        public float LeftBorder => transform.position.x - _distanceX;
        public float RightBorder => transform.position.x + _distanceX;
        public float TopBorder => transform.position.z + _distanceZ;
        public float BottomBorder => transform.position.z - _distanceZ;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(_distanceX * 2, 5f, _distanceZ * 2));
        }
#endif
    }
}
