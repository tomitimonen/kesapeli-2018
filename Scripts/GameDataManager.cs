using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameDataManager : MonoBehaviour {

    string savePath;
    GameData gameData;
    public GameData GameData
    {
        get
        {
            if (gameData == null)
            {
                LoadGame();
            }
            return gameData;
        }
    }

    static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance != null) return instance;

            GameObject singletons = GameObject.Find("Singletons");
            if (singletons == null)
            {
                singletons = new GameObject("Singletons");
            }
            instance = singletons.AddComponent<GameDataManager>();

            return instance;
        }
    }

    private void Awake()
    {
        LoadGame();
        DontDestroyOnLoad(this);
        savePath = Path.Combine(Application.persistentDataPath, "savegame.dat");
        Debug.Log("GameDataManager - Save game path is " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "savegame.dat")))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "savegame.dat"), FileMode.Open);
            gameData = (GameData)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            CreateNewGame();
        }
    }

    public void SaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Path.Combine(Application.persistentDataPath, "savegame.dat"), FileMode.OpenOrCreate);
        if (gameData == null) Debug.LogError("GameDataManager - SaveGame - gameData is null");
        bf.Serialize(file, gameData);
        file.Close();
    }

    public void CreateNewGame()
    {
        gameData = new GameData();
        SaveGame();
    }


}
