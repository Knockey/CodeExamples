using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SaveLoad
{
    [Serializable]
    public class SaveDataList
    {
        [SerializeField] private List<SaveData> _saveDatas;

        public SaveDataList()
        {
            _saveDatas = new List<SaveData>();
        }

        public void AddSaveData(SaveData saveData) => _saveDatas.Add(saveData);

        public SaveData GetSaveData(string hashCode)
        {
            var saveData = _saveDatas.FirstOrDefault(saveData => saveData.ObjectID == hashCode);

            if (saveData == null)
                Debug.LogWarning($"No save data with this {nameof(hashCode)}. Creating new one.");

            return saveData == null ? new SaveData(hashCode) : saveData;
        }
    }
}
