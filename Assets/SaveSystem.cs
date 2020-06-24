using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveSystem 
{
   public static void SaveXidData(XIDScan scan)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/XidData.xlenz";
        FileStream stream = new FileStream(path, FileMode.Create);
        XidData data = new XidData(scan);
        formatter.Serialize(stream,data);
        stream.Close();
        Debug.Log("Succesfully Saved Xid Data in the file");

    } 
    public static XidData LoadXidData()
    {
        string path = Application.persistentDataPath + "/XidData.xlenz";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            XidData data=formatter.Deserialize(stream)as XidData;
            stream.Close();
            return data;

        }
        else
        {
            Debug.Log("Save File not found " + path);
            return null;
        }
       
    }
}
