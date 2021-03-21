using System.Collections;
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
        string res = $"{obj.GetType().FullName} [ ";
        foreach (var field in obj.GetType().GetFields().Where(x=>x.IsPublic)) {
                res += $" {field.Name}: {field.GetValue(obj)} ";
        }
        res+= " ]";
        return res;
    }
}
