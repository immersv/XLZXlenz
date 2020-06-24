using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Networking;

public class OpenFileExplorer : MonoBehaviour
{
    string path;
    public RawImage image;
    // Start is called before the first frame update
   public void OpenExplorer()
    {
        path = EditorUtility.OpenFilePanel("Jpg files", "", "jpg");
        GetImage();
    }
    void GetImage()
    {
        if (path != null)
        {
            StartCoroutine(UpdateImage());
        }
    }
    IEnumerator UpdateImage()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture("file://"+path))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                image.texture = DownloadHandlerTexture.GetContent(uwr);
            }
        }
    }
}
