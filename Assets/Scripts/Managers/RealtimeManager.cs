using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class RealtimeManager : MonoBehaviour
{

    // Use this for initialization
    public string result;
    public string[] date;

    public string time;
    public string oldTime;

    public string sec;
    public string min;
    public string hr;
    public string month;
    public string day;
    public string year;

    public DateTime dateTime;
    public DateTime lastDateTime;

    public TimeSpan difference;

    
    public Action onDateLoaded;
    private void Awake()
    {
        LoadData();

        InternetConnectionManager.onInternetConnected += () => GetDate(false);
    }
    void Start ()
    {
        if(GameManager.isOnline)
        {
            GetDate(false);
        }
    }

   

    public void GetDate(bool saveData)
    {
        StartCoroutine(GetInternetTime2(saveData));
    }
    public IEnumerator GetInternetTime2(bool saveData)
    {
        UnityWebRequest myHttpWebRequest = UnityWebRequest.Get("http://www.microsoft.com");

        // UnityWebRequestAsyncOperation async = myHttpWebRequest.SendWebRequest();

        //yield return async;
        //yield return myHttpWebRequest.Send();
         yield return myHttpWebRequest.SendWebRequest();
        //quizas haya que cambiarlo por esto
        //string netTime = async.webRequest.GetResponseHeader("date");

        string netTime = myHttpWebRequest.GetResponseHeader("date");
        DateResolve(netTime, saveData);
        

    }
    void DateResolve(string res, bool saveData)
    {
        date = res.Split(' ');
        time = date[4];

        day = date[1];
        month = ToMonth(date[2]).ToString();
        year = date[3];


        var timeSplit = time.Split(':');
        hr = timeSplit[0];
        min = timeSplit[1];
        sec = timeSplit[2];
       

        dateTime = new DateTime(int.Parse(year),
            int.Parse(month),
            int.Parse(day),
            int.Parse(hr),
            int.Parse(min),
            int.Parse(sec)
            );


        Debug.Log("Current " + dateTime);

        if (saveData)
        {
            SaveData();
            lastDateTime = dateTime;
        }
        difference = dateTime.Subtract(lastDateTime);
        //
        Debug.Log("Diff " + difference);

        if(onDateLoaded != null)
            onDateLoaded.Invoke();
    }

    public int ToMonth(string val)
    {
        switch (val)
        {
            case "Jan": return 1;
            case "Feb": return 2;
            case "Mar": return 3;
            case "Apr": return 4;
            case "May": return 5;
            case "Jun": return 6;
            case "Jul": return 7;
            case "Aug": return 8;
            case "Sep": return 9;
            case "Oct": return 10;
            case "Nov": return 11;
            case "Dec": return 12;
        }

        return 1;
    }

    public void SaveData()
    {
       
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/time.sav", FileMode.Create);

        bf.Serialize(fs, dateTime);
        print("Save Time Data" + dateTime.ToString());
        fs.Close();
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/time.sav"))
        {
            
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/time.sav", FileMode.Open);

            lastDateTime = (DateTime)bf.Deserialize(fs);
            print("Load Time Data " + lastDateTime.ToString());
            fs.Close();
        }
    }

  

  
}


