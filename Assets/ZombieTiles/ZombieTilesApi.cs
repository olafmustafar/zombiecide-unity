using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

public class ZombieTilesApi
{
    public Dungeon dungeon { get; set; }
    public bool useDoubledEnemies = true;
    string result;

    public void LoadDungeon(bool isGenerated, int levelId)
    {
        string generatedLevels = "generated";
        if (useDoubledEnemies)
        {
            generatedLevels = "generated-doubled-enemies";
        }

        TextAsset textFile = Resources.Load("Levels/" + (isGenerated ? generatedLevels : "manual") + $"/level{levelId + 1}") as TextAsset;
        dungeon = ParseResponse(textFile.ToString());
    }

    static string ReadFromFile(string path)
    {
        System.IO.StreamReader reader = new System.IO.StreamReader(path);
        string result = reader.ReadToEnd();
        Debug.Log(result);
        reader.Close();
        return result;
    }

    IEnumerator Request(string type, string level)
    {
        Debug.Log("one");
        string address = GetApiAddress();
        MonoBehaviour.print($"{address}?type={type}&id={level}");
        UnityWebRequest www = UnityWebRequest.Get($"{address}?type={type}&id={level}");
        yield return www.SendWebRequest();


        if (www.result == UnityWebRequest.Result.ConnectionError
        || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(www.error);
            result = "";
        }
        else
        {
            result = www.downloadHandler.text;
        }
    }

    static Dungeon ParseResponse(string json)
    {
        return JsonConvert.DeserializeObject<Dungeon>(json);
    }

    string GetApiAddress()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            string address = Application.absoluteURL;
            address = address.Remove(address.LastIndexOf(':'));
            return $"{address}:3000";
        }
        else
        {
            return "http://127.0.0.1:3000";
        }
    }
}
