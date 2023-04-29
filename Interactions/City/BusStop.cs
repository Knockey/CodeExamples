using UnityEngine;

namespace City
{
    [RequireComponent(typeof(Collider))]
    public class BusStop : MonoBehaviour
    {
        private void Awake()
        {
            var collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }
    }
}
