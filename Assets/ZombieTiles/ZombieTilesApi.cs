using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections;

public class ZombieTilesApi 
{
    private const string API_ADDRESS = @"http://127.0.0.1:3000";

    public Dungeon dungeon { get; set; }
    string result;
    
    public IEnumerator LoadDungeon(bool isGenerated, int levelId)
    {
        yield return Request( isGenerated ?  "generated": "manual", $"level{levelId + 1}" );

        if (result.Length > 0)
        {
            Debug.Log("done");
            dungeon = ParseResponse(result);
        }
        else
        {
            dungeon = null;
        }
    }

    IEnumerator Request(string type, string level)
    {
        Debug.Log("one");
        MonoBehaviour.print($"{API_ADDRESS}?type={type}&id={level}");
        UnityWebRequest www = UnityWebRequest.Get($"{API_ADDRESS}?type={type}&id={level}");
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
    }