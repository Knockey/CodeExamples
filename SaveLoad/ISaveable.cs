using System;

namespace SaveLoad
{
    public interface ISaveable
    {
        public string Name { get; }

        public event Action NeedToSave;

        public int GetSaveValue();
        public void LoadSaveData(SaveData saveData);
    }
}
