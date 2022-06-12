using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Resources.Scripts.Storage
{
    public class Storage
    {
        private readonly string _filePath;
        private readonly BinaryFormatter _formatter;
        
        public Storage()
        {
            _formatter = new BinaryFormatter();
            var directory = Application.persistentDataPath + "/saves";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            _filePath = Application.persistentDataPath + "/saves/settings.save";
        }

        public object Load(object saveDataByDefault)
        {
            if (!File.Exists(_filePath))
            {
                if (saveDataByDefault != null)
                    Save(saveDataByDefault);
                return saveDataByDefault;
            }

            var file = File.Open(_filePath, FileMode.Open);
            var savedData = _formatter.Deserialize(file);
            file.Close();
            return savedData;
        }

        public void Save(object saveData)
        {
            var file = File.Create(_filePath);
            _formatter.Serialize(file, saveData);
            file.Close();
        }
    }
}
