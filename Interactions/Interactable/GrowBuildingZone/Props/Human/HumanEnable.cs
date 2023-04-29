using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions.Humans
{
    public class HumanEnable : MonoBehaviour
    {
        [SerializeField] private List<Human> _humans;

        public bool IsEmpty => _humans.Count <= 0;

        public void EnableHumans()
        {
            if (_humans != null && _humans.Count > 0)
            {
                _humans.ForEach(human => human.Enable());
            }
        }

        public void EnableObjectsByTimeWithParticles(float time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException($"{nameof(time)} can't be less, than 0! It equals {time} now!");

            StartCoroutine(SpawnHuman(time));
        }

        public void DisableHumans()
        {
            if (_humans != null && _humans.Count > 0)
            {
                _humans.ForEach(human => human.Disable());
            }
        }

        public void SpawnParticles(float particleTime)
        {
            if (_humans != null && _humans.Count > 0)
            {
                _humans.ForEach(human => human.SpawnParticle(particleTime));
            }
        }

        private IEnumerator SpawnHuman(float time)
        {
            var tickTime = new WaitForSeconds((time * 0.8f) / _humans.Count);
            var objectIndex = 0;

            while (objectIndex < _humans.Count)
            {
                _humans[objectIndex].Enable();
                _humans[objectIndex].SpawnParticle(time * 0.8f);

                objectIndex += 1;
                yield return tickTime;
            }
        }

#if UNITY_EDITOR
        public void FillHumansList()
        {
            _humans = GetComponentsInChildren<Human>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
