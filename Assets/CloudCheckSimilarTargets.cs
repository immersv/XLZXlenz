using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using UnityEngine.Networking;
using System.Net;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json;


public class CloudCheckSimilarTargets : MonoBehaviour
{
    
    private string access_key = "c14a587f5accc3deeb13ae244b59705545e4a436";
    private string secret_key = "85c97149c6894c1bb9ae8cae317774ff914731a6";
    //Address of Vuforia's server
    private string url = @"https://vws.vuforia.com";

    public void Starts()
    {
        //CheckDuplicates();
        Debug.Log("In Start");
    }
   
    public string CheckDuplicates(string tid)
    {
        string targetID = tid;
        string requestPath = "/duplicates/" + targetID;
        string serviceURI = url + requestPath;
        string httpAction = "GET";
        string contentType = "";
        string requestBody = "";
        Debug.Log("In function");
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(serviceURI);
        return VuforiaRequest(requestPath, httpAction, contentType, requestBody, unityWebRequest);
    }
    public string VuforiaRequest(string requestPath, string httpAction, string contentType, string requestBody, UnityWebRequest unityWebRequest)
    {
        string serviceURI = url + requestPath;
        string date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());

        unityWebRequest.SetRequestHeader("host", url);
        unityWebRequest.SetRequestHeader("date", date);
        unityWebRequest.SetRequestHeader("content-type", contentType);

        MD5 md5 = MD5.Create();
        var contentMD5bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(requestBody));
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < contentMD5bytes.Length; i++)
        {
            sb.Append(contentMD5bytes[i].ToString("x2"));
        }

        string contentMD5 = sb.ToString();

        string stringToSign = string.Format("{0}\n{1}\n{2}\n{3}\n{4}", httpAction, contentMD5, contentType, date, requestPath);

        HMACSHA1 sha1 = new HMACSHA1(System.Text.Encoding.ASCII.GetBytes(secret_key));
        byte[] sha1Bytes = System.Text.Encoding.ASCII.GetBytes(stringToSign);
        MemoryStream stream = new MemoryStream(sha1Bytes);
        byte[] sha1Hash = sha1.ComputeHash(stream);
        string signature = System.Convert.ToBase64String(sha1Hash);

        unityWebRequest.SetRequestHeader("authorization", string.Format("VWS {0}:{1}", access_key, signature));


        unityWebRequest.SendWebRequest();
        while (!unityWebRequest.isDone && !unityWebRequest.isNetworkError)
        {
            //If request error, return fail            
        }
        if (unityWebRequest.error != null)
        {
            Debug.Log("requestError: " + unityWebRequest.error);

            return "fail";
        }
        else
        {
            if (httpAction == "DELETE")
            {
                return "Deleted";
            }
            Debug.Log(unityWebRequest.downloadHandler.text);

        }
        Debug.Log(unityWebRequest.downloadHandler.text);
        // CreateSimilarList(unityWebRequest.downloadHandler.text);
        return unityWebRequest.downloadHandler.text;

    }
    public void CreateSimilarList(string jsonSimilar)
    {
        if (jsonSimilar!=null)
        {
            SRoot rootSimalar = new SRoot();
            Newtonsoft.Json.JsonConvert.PopulateObject(jsonSimilar, rootSimalar);
            Debug.Log(rootSimalar.similar_targets.Count);
            if (rootSimalar.similar_targets.Count != null)
            {
                for(int i = 0; i < rootSimalar.similar_targets.Count; i++)
                {
                    Debug.Log(rootSimalar.similar_targets[i]);
                }
            }
        }
        
    }
}

public class SRoot
{
    public string transaction_id { get; set; }
    public List<string> similar_targets { get; set; }
    public string result_code { get; set; }

}

