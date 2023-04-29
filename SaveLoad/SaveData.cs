using System;

namespace SaveLoad
{
    [Serializable]
    public class SaveData
    {
        public string ObjectID;
        public int ValueToSave;

        public SaveData(string objectName, int valueToSave = 0)
        {
            ObjectID = objectName;
            ValueToSave = valueToSave;
        }
    }
}
