using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interactions
{
    public class GrowingBuildingsList : MonoBehaviour
    {
        [SerializeField] private List<GrowingBuilding> _growingBuildingList;

        public int BuildingsCount => _growingBuildingList.Count;

        private void Awake()
        {
            if (_growingBuildingList == null || _growingBuildingList.Count == 0)
                Debug.LogWarning($"Fill buildings list on {gameObject.name}");
        }

        public void GrowBuilding(int buildingIndex)
        {
            TryThrowException(buildingIndex);

            _growingBuildingList[buildingIndex].Grow();
        }

        private void TryThrowException(int buildingIndex)
        {
            if (buildingIndex < 0)
                throw new System.ArgumentOutOfRangeException($"{nameof(buildingIndex)} can't be less, than 0! It euqals {buildingIndex} now!");

            if (buildingIndex >= _growingBuildingList.Count)
                throw new System.ArgumentOutOfRangeException($"{nameof(buildingIndex)} can't be more, than {BuildingsCount}! It euqals {buildingIndex} now!" +
                    $"Call {nameof(BuildingsCount)} property first!");

            if (_growingBuildingList == null || _growingBuildingList.Count == 0)
                Debug.LogWarning($"{nameof(_growingBuildingList)} on component {gameObject} is empty!");
        }

#if UNITY_EDITOR
        public void FillBuildingsList()
        {
            _growingBuildingList = GetComponentsInChildren<GrowingBuilding>().ToList();
            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
