using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions.Humans
{
    public class Human : MonoBehaviour
    {
        [SerializeField] private List<ParticleSystem> _emojiParticles;
        [SerializeField] private Transform _particleSpawnPoint;

        private bool _isEnabled = false;

        public void Enable()
        {
            gameObject.SetActive(true);
            _isEnabled = true;
        }

        public void Disable()
        {
            if (_isEnabled == false)
            {
                gameObject.SetActive(false);
            }
        }

        public void SpawnParticle(float particleTime)
        {
            if (particleTime <= 0f)
                throw new ArgumentOutOfRangeException($"{nameof(particleTime)} can't be less or equal 0! Now it equals {particleTime}!");

            var particleIndex = UnityEngine.Random.Range(0, _emojiParticles.Count);

            var particle = Instantiate(_emojiParticles[particleIndex], _particleSpawnPoint);
            Destroy(particle.gameObject, particleTime);
        }

    }
}
