using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardItem : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField]private Text playerText;
    [SerializeField]private Text scoreText;
    #pragma warning restore 0649

    public void Set(string playerValue, string scoreValue, Color color) {
        playerText.text = playerValue;
        scoreText.text = scoreValue;
        playerText.color = scoreText.color = color;
    }

}
