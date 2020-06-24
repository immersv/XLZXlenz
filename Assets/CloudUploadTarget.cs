using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Configuration;
using UnityEngine.Networking;

public class PostNewTrackableRequest
{
    public string name;
    public float width;
    public string image;
    public string application_metadata;
}

public class CloudUploadTarget : MonoBehaviour
{
    public CloudCheckSimilarTargets similarTargets;
    public Texture2D texture;

    private string access_key = "c14a587f5accc3deeb13ae244b59705545e4a436";
    private string secret_key = "85c97149c6894c1bb9ae8cae317774ff914731a6";
    private string url = @"https://vws.vuforia.com";//@"<a href="https://vws.vuforia.com";//">https://vws.vuforia.com";</a>
    private string targetName = "anil17"; // must change when upload another Image Target, avoid same as exist Image on cloud

    private byte[] requestBytesArray;

    public void CallPostTarget()
    {
        StartCoroutine(PostNewTarget());
    }

    IEnumerator PostNewTarget()
    {

        string requestPath = "/targets";
        string serviceURI = url + requestPath;
        string httpAction = "POST";
        string contentType = "application/json";
        string date = string.Format("{0:r}", DateTime.Now.ToUniversalTime());

        Debug.Log(date);

        // if your texture2d has RGb24 type, don't need to redraw new texture2d
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        tex.SetPixels(texture.GetPixels());
        tex.Apply();
        byte[] image = tex.EncodeToPNG();

        string metadataStr = "Metadata regarding the target to store and call when image target is detected ";//May use for key,name...in game
        byte[] metadata = System.Text.ASCIIEncoding.ASCII.GetBytes(metadataStr);
        PostNewTrackableRequest model = new PostNewTrackableRequest();
        model.name = targetName;
        model.width = 64.0f; // don't need same as width of texture
        model.image = System.Convert.ToBase64String(image);

        model.application_metadata = System.Convert.ToBase64String(metadata);
        //string requestBody = JsonWriter.Serialize(model);
        string requestBody = JsonUtility.ToJson(model);

        WWWForm form = new WWWForm();

        var headers = form.headers;
        byte[] rawData = form.data;
        headers["host"] = url;
        headers["date"] = date;
        headers["Content-Type"] = contentType;

        HttpWebRequest httpWReq = (HttpWebRequest)HttpWebRequest.Create(serviceURI);

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

        headers["Authorization"] = string.Format("VWS {0}:{1}", access_key, signature);

        Debug.Log("<color=green>Signature: " + signature + "</color>");

        WWW request = new WWW(serviceURI, System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(model)), headers);
        yield return request;

        if (request.error != null)
        {
            Debug.Log("request error: " + request.error+ request.text);
           
           
        }
        else
        {
            Debug.Log("request success");
            Debug.Log("returned data" + request.text);
            CreateList(request.text);

        }
    }
    public void CreateList(string jsonString)
    {
        if (jsonString != null)
        {
            Root root = new Root();
            Newtonsoft.Json.JsonConvert.PopulateObject(jsonString, root);
            Debug.Log(root.target_id);
            similarTargets.CheckDuplicates(root.target_id);
        }
        

    }
}
public class Root
{
    public string transaction_id { get; set; }
    public string result_code { get; set; }
    public string target_id { get; set; }

}