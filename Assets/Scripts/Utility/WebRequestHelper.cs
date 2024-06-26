﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class WebRequestHelper : MonoBehaviour
{

    private static string DoWebRequest(string url, string method, string body = null, System.Action<string> callback = null)
    {
        byte[] byteArray = null;
        string jsonResponse;

        if (body != null)
            byteArray = Encoding.UTF8.GetBytes(body);

        HttpWebRequest request =
       (HttpWebRequest)WebRequest.Create(url);

        request.Method = method;
        request.ContentType = "application/json";
        if(byteArray != null)
            request.GetRequestStream().Write(byteArray, 0, byteArray.Length);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        jsonResponse = reader.ReadToEnd();
        return jsonResponse;
    }

    public static string Post(string url, string body) {
        return DoWebRequest(url, "POST", body);
    }
    public static string Get(string url, string body = null){
        return DoWebRequest(url, "GET", body);
    }
    public static string Put(string url, string body){
        return DoWebRequest(url, "PUT", body);
    }
}
