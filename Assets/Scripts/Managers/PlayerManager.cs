using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    const string urlLeaderBoardPost = "https://heroku-demo-lucentini.herokuapp.com/usuarios/create";
    const string urlLeaderBoardPut = "https://heroku-demo-lucentini.herokuapp.com/usuarios/";

    

    public int CreatePlayer(string playerNombre)
    {
        LeaderBoardDataItem request = new LeaderBoardDataItem() { nombre = playerNombre, score = "0" };

        string jsonRequest = JsonUtility.ToJson(request);
        string response = WebRequestHelper.DoWebRequest(urlLeaderBoardPost, "POST", jsonRequest);
        Debug.Log("@CreatePlayer: " + response);
        return JsonUtility.FromJson<LeaderBoardDataItem>(response).id;
    }

    public void UpdatePlayer(int playerId, int playerScore)
    {

        LeaderBoardDataItem request = new LeaderBoardDataItem() { id = playerId, score = playerScore.ToString() };
        string jsonRequest = JsonUtility.ToJson(request);
        string response = WebRequestHelper.DoWebRequest($"{urlLeaderBoardPut}/{playerId}", "PUT", jsonRequest);
        Debug.Log("@UpdatePlayer: " + response);
    }
}
