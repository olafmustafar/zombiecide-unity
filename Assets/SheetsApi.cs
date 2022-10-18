using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

struct TokenResponse
{
    public string access_token { get; set; }
}

struct SheetInfo
{
    public string sheet_id { get; set; }
    public string client_email { get; set; }
}

public class SheetsApi
{
    string token;
    SheetInfo sheetInfo;

    public SheetsApi()
    {
        TextAsset sheetInfoFile = Resources.Load("GoogleApi/SheetInfo") as TextAsset;
        sheetInfo = JsonConvert.DeserializeObject<SheetInfo>(sheetInfoFile.ToString());
        Debug.Log(sheetInfo.client_email);
        Debug.Log(sheetInfo.sheet_id);
    }

    public static IEnumerator SendDataSheet(String[] dataSheet)
    {
        SheetsApi sapi = new SheetsApi();
        yield return sapi.RequestToken();
        yield return sapi.Send(dataSheet);
    }


    IEnumerator Send(String[] dataSheet)
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(new
        {
            values = new List<String[]>() { dataSheet }
        });

        using (UnityWebRequest www = UnityWebRequest.Put($"https://sheets.googleapis.com/v4/spreadsheets/{sheetInfo.sheet_id}/values/Data!A1:append?valueInputOption=USER_ENTERED&insertDataOption=INSERT_ROWS", json))
        {
            www.method = "POST";
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                Debug.Log(www.error);
            }
        }
    }

    IEnumerator RequestToken()
    {
        WWWForm form = new WWWForm();
        form.AddField("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer");
        form.AddField("assertion", GetGoogleJWT());

        using (UnityWebRequest www = UnityWebRequest.Post("https://oauth2.googleapis.com/token", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                TokenResponse response = JsonConvert.DeserializeObject<TokenResponse>(www.downloadHandler.text);
                token = response.access_token;
            }
        }
    }

    string GetGoogleJWT()
    {
        var iat = DateTimeOffset.Now.ToUnixTimeSeconds();
        var exp = DateTimeOffset.Now.ToUnixTimeSeconds() + 3600;

        var header = new
        {
            alg = "RS256",
            typ = "JWT"
        };

        var payload = new
        {
            iss = sheetInfo.client_email,
            scope = "https://www.googleapis.com/auth/spreadsheets",
            aud = "https://accounts.google.com/o/oauth2/token",
            exp = exp,
            iat = iat
        };

        string header_payload_encoded =
            Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header)))
            + "."
            + Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));

        string signature_encoded;
        using (var rsaCSP = new RSACryptoServiceProvider())
        {
            TextAsset key = Resources.Load<TextAsset>("GoogleApi/key");
            rsaCSP.FromXmlString(key.text);

            byte[] signature = rsaCSP.SignData(Encoding.UTF8.GetBytes(header_payload_encoded), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            signature_encoded = Convert.ToBase64String(signature);
        }

        return header_payload_encoded + "." + signature_encoded;
    }
}