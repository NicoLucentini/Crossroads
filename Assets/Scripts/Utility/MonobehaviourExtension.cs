using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MonobehaviourExtension 
{
    public static void Log(this MonoBehaviour ob, string msg)
    {
        Debug.Log($"#{ob.GetType().FullName} {msg}");
    }
    public static string ToStringFull(this object obj )
    {
        if (obj == null) return null;
                 
        string res = $"{obj.GetType().FullName} [ ";
        foreach (var field in obj.GetType().GetFields().Where(x=>x.IsPublic)) {
                res += $" {field.Name}: {field.GetValue(obj)} ";
        }
        res+= " ]";
        return res;
    }

    public static string ListFull<T>(this List<T> list) {

        string res = "";
        foreach (var item in list) {
            res += item.ToStringFull();
        }
        return res;
    }

    public static string ToJson(this object obj) {
        return JsonUtility.ToJson(obj);
    }
    public static T ToObject<T>(this string json)
    {
        return JsonUtility.FromJson<T>(json);
    }
}
