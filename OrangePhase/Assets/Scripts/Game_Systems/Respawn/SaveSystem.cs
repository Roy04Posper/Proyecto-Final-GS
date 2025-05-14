using UnityEngine;
using System.IO;


[System.Serializable]
public class SaveSystem : MonoBehaviour
{
    private static SaveData saveData = new SaveData();
    [System.Serializable]
    public struct SaveData 
    {
        public PlayerSaveData PlayerData;
    }
    public static string SaveFileName() 
    {
        string saveFile = Application.persistentDataPath + "/save" + ".save";
        return saveFile;
    }
    public static void Save()
    {
        HandleSaveData();
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }
    public static void HandleSaveData()
    {
        GameManager.Instance.Player.Save(ref saveData.PlayerData);
    }
    public static void Load()
    {
        string saveContent = File.ReadAllText(SaveFileName());
        saveData = JsonUtility.FromJson<SaveData>(saveContent);
    }
    private static void HandleLoadData()
    {
        GameManager.Instance.Player.Load(saveData.PlayerData);
    }
}
