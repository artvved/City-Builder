using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Game
{
    public class SaveLoadController : MonoBehaviour
    {
        [SerializeField] private string savePath;
        [SerializeField] private string saveFileName = "data.json";


        public void Save(MapModel model)
        {
            string json = JsonConvert.SerializeObject(model,Formatting.Indented);
            File.WriteAllText(savePath,json);
        }

        public MapModel Load()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                return JsonConvert.DeserializeObject<MapModel>(json);
            }

            return null;
        }


        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
#else
            savePath = Path.Combine(Application.dataPath, saveFileName);
#endif
        }
    }
}