using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions.Humans
{
    public class HumanEnableList : MonoBehaviour
    {
        [SerializeField] private List<HumanEnable> _humanEnableList;

        public void EnableObjects()
        {
            if (_humanEnableList != null && _humanEnableList.Count > 0)
            {
                _humanEnableList.ForEach(humanEnable => humanEnable.EnableHumans());
            }
        }

        public void EnableObjectsByTimeWithParticles(float time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException($"{nameof(time)} can't be less, than 0! It equals {time} now!");

            StartCoroutine(Spawn(time));
        }

        public void DisableObjects()
        {
            if (_humanEnableList != null && _humanEnableList.Count > 0)
            {
                _humanEnableList.ForEach(humanEnable => humanEnable.DisableHumans());
            }
        }

        private IEnumerator Spawn(float time)
        {
            var tickTime = new WaitForSeconds((time * 0.8f) / _humanEnableList.Count);
            var objectIndex = 0;

            while (objectIndex < _humanEnableList.Count)
            {
                if (_humanEnableList[objectIndex].IsEmpty)
                {
                    yield return null;
                    objectIndex += 1;
                    continue;
                }

                _humanEnableList[objectIndex].EnableObjectsByTimeWithParticles(time * 0.8f);

                objectIndex += 1;
                yield return tickTime;
            }
        }

#if UNITY_EDITOR
        public void FillHumanEnableList()
        {
            _humanEnableList = GetComponentsInChildren<HumanEnable>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
