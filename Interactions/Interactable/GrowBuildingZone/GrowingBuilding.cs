using UnityEngine;

namespace Interactions
{
    public class GrowingBuilding : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _growEffect;

        private void Awake()
        {
            transform.gameObject.SetActive(false);
        }

        public void Grow()
        {
            Instantiate(_growEffect, transform.position, Quaternion.identity, transform);
            transform.gameObject.SetActive(true);
        }
    }
}
