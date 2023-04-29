using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Upgrades;

namespace SaveLoad
{
    public class DataLoad : MonoBehaviour
    {

        [SerializeField] private string _fileName = "SaveData";
        [SerializeField] private bool _isSavingLevel = false;
        [SerializeField] private List<MonoBehaviour> _objectsToSave;

        private List<ISaveable> _saveables = new();

        private void OnValidate()
        {
            if (_objectsToSave != null && _objectsToSave.Count > 0)
            {
                _objectsToSave.ForEach(obj =>
                {
                    if (obj == null)
                        return;

                    if (obj is IUpgradable)
                        return;

                    if (obj is ISaveable)
                        return;

                    Debug.LogWarning(obj.name + " needs to implement " + nameof(IUpgradable));
                    _objectsToSave[_objectsToSave.IndexOf(obj)] = null;
                });

                _objectsToSave.RemoveAll(obj => obj == null);
            }
        }

        private void Awake()
        {
            FillISaveableList();
            LoadData();
        }

        private void OnEnable()
        {
            _saveables.ForEach(obj => obj.NeedToSave += OnNeedToSave);
        }

        private void OnDisable()
        {
            _saveables.ForEach(obj => obj.NeedToSave -= OnNeedToSave);
        }

        public void LoadData()
        {
            var saveDataList = LoadSaveDataList();

            for (int saveableIndex = 0; saveableIndex < _saveables.Count; saveableIndex += 1)
            {
                var saveData = saveDataList.GetSaveData(_saveables[saveableIndex].Name + saveableIndex);
                _saveables[saveableIndex].LoadSaveData(saveData);
            }
        }

        private void FillISaveableList()
        {
            _objectsToSave.ForEach(obj =>
            {
                if (obj is IUpgradable upgradable)
                {
                    _saveables.Add(upgradable.UpgradeData);
                }

                if (obj is ISaveable saveable)
                {
                    _saveables.Add(saveable);
                }

                if (obj is ISaveable == false && obj is IUpgradable == false)
                {
                    throw new System.InvalidCastException($"{nameof(obj)} should implement {nameof(IUpgradable)} or {nameof(ISaveable)}!");
                }
            });
        }

        private SaveDataList LoadSaveDataList()
        {
            var additionalPath = _isSavingLevel ? PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0).ToString() : "";
            SaveDataList saveData;
            var path = Application.persistentDataPath + "/" + _fileName + additionalPath + ".json";

            if (File.Exists(path))
            {
                string loadedJson = File.ReadAllText(path);
                saveData = JsonUtility.FromJson<SaveDataList>(loadedJson);
                Debug.Log("Game data loaded from: " + path);
            }
            else
            {
                saveData = new SaveDataList();
                Debug.Log("Default game data loaded!");
            }

            return saveData;
        }

        private void OnNeedToSave()
        {
            var saveDataList = new SaveDataList();

            for (int saveableIndex = 0; saveableIndex < _saveables.Count; saveableIndex += 1)
            {
                var objectName = _saveables[saveableIndex].Name + saveableIndex;
                var valueToSave = _saveables[saveableIndex].GetSaveValue();

                var saveData = new SaveData(objectName, valueToSave);
                saveDataList.AddSaveData(saveData);
            }

            var additionalPath = _isSavingLevel ? PlayerPrefs.GetInt(LevelPrefs.CompletedLevels, 0).ToString() : "";
            var path = Application.persistentDataPath + "/" + _fileName + additionalPath + ".json";
            string jsonToSave = JsonUtility.ToJson(saveDataList, true);

            File.WriteAllText(path, jsonToSave);
            Debug.Log("Save completed to: " + path);
        }

#if UNITY_EDITOR
        public void FillBuildingsList()
        {
            var saveableList = GetComponentsInChildren<MonoBehaviour>().ToList();
            _objectsToSave.Clear();

            saveableList.ForEach(obj =>
            {
                if (obj is IUpgradable _ || obj is ISaveable __)
                {
                    _objectsToSave.Add(obj);
                }
            });

            Save();
        }

        private void Save() => UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
