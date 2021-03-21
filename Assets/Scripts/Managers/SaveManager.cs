using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveManager 
{
    public static T LoadData<T>(string path){
        T data = default(T);

        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            data = (T)bf.Deserialize(fs);
            fs.Close();
        }

        return data;
    }
    public static void SaveData(string path, object obj) {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(path, FileMode.Create);

        bf.Serialize(fs, obj);
        fs.Close();
    }
}
