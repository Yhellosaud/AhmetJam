using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Managers
{
    public class DataPersistenceManager : MonoBehaviour
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName = "load.game";
        [SerializeField] private GameSettings gameSettings;
        public GameData gameData;
        private List<IDataPersistence> dataPersistenceObjects;
        private FileDataHandler dataHandler;

        public void Start()
        {
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        public void LoadGame()
        {
            Debug.Log("Reloaded!");
            this.gameData = dataHandler.Load();

            if (this.gameData == null)
            {
                NewGame();
            }
            gameData.SecondsForWaitSession = gameSettings.SecondsForWaitSession;
            gameData.SecondsForPlaySession = gameSettings.SecondsForPlaySession;
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }

        }
        public void SaveGame()
        {
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.SaveData(gameData);
            }

            dataHandler.Save(gameData);
        }
        public void NewGame()
        {
            this.gameData = new GameData(gameSettings);
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
                SaveGame();
        }
        private void OnApplicationQuit()
        {
            SaveGame();
        }
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
            return new List<IDataPersistence>(dataPersistenceObjects);
        }
    }
}
