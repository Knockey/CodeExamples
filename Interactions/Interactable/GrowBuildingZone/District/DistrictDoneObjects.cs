using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactions
{
    public class DistrictDoneObjects : MonoBehaviour
    {
        private List<Animator> _objectsToEnableWhenDistrictDone;
        private bool _hasObjectsToEnable => _objectsToEnableWhenDistrictDone != null && _objectsToEnableWhenDistrictDone.Count > 0;

        public void InitObjects()
        {
            var districtDoneObjects = GetComponentsInChildren<Animator>();
            _objectsToEnableWhenDistrictDone = new List<Animator>(districtDoneObjects);
            _objectsToEnableWhenDistrictDone.ForEach(obj => obj.gameObject.SetActive(false));
        }

        public void EnableObjects()
        {
            if (_hasObjectsToEnable)
            {
                _objectsToEnableWhenDistrictDone.ForEach(obj => obj.gameObject.SetActive(true));
            }
        }

        public void EnableObjectsByTime(float time)
        {
            if (time < 0)
                throw new ArgumentOutOfRangeException($"{nameof(time)} can't be less, than 0! It equals {time} now!");

            StartCoroutine(SpawnObjects(time));
        }

        private IEnumerator SpawnObjects(float time)
        {
            var tickTime = new WaitForSeconds((time * 0.8f) / _objectsToEnableWhenDistrictDone.Count);
            var objectIndex = 0;

            while (objectIndex < _objectsToEnableWhenDistrictDone.Count)
            {
                _objectsToEnableWhenDistrictDone[objectIndex].gameObject.SetActive(true);
                objectIndex += 1;
                yield return tickTime;
            }
        }
    }
}
