using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveGame
{
    public static void SaveGameData (GameSession gameSession)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/score.txt";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(gameSession);

        formatter.Serialize(stream, data);
        stream.Close();
    }



    public static SaveData LoadGameData()
    {
        string path = Application.persistentDataPath + "/score.txt";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
