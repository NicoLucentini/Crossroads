using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboard : MonoBehaviour
{
    public Text rank;
    public Text username;
    public Text score;
    public Color otherColor;
    public Color meColor;
    public void Set(string r, string u, string s, bool me)
    {
        rank.text ="#"+ r;
        username.text = u;
        score.text = s;
        if (me)
            ChangeColors(meColor);
        else
            ChangeColors(otherColor);
    }
    void ChangeColors(Color col)
    {
        rank.color = col;
        username.color = col;
        score.color = col;
    }
}
