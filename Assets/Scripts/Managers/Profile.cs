using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Profile 
{
    public int maxScoreNoBrakes = 0;
    public int maxScoreTransit = 0;
    public int points = 0;
    public string playerName = "";
    public int experience = 0;
    public int level = 1;
    public int achievmentLevel = 1;
    //public bool[] blockedData ;
    public List<bool> blockedData;
    public int idUser = -1;
    public string registered;
}
