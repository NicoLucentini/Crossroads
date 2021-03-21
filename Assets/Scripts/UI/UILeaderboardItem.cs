using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILeaderboardItem : MonoBehaviour
{
    [SerializeField]private Text playerText;
    [SerializeField]private Text scoreText;

    public void Set(string playerValue, string scoreValue, Color color) {
        playerText.text = playerValue;
        scoreText.text = scoreValue;
        playerText.color = scoreText.color = color;
    }

}
