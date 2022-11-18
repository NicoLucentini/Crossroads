using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineService 
{
    public static bool isOnline = false;
    const string urlLeaderBoard = "https://heroku-demo-lucentini.herokuapp.com/usuarios/getAllUsers";
    const string urlLeaderBoardPost = "https://heroku-demo-lucentini.herokuapp.com/usuarios/create";
    const string urlLeaderBoardPut = "https://heroku-demo-lucentini.herokuapp.com/usuarios/";

    public static LeaderBoardDataItem GetPlayer(int id) {
        return WebRequestHelper.Get($"{urlLeaderBoardPut}/{id.ToString()}").ToObject<LeaderBoardDataItem>();
    }

    public static int CreatePlayer(string playerName, string tScore = "0") {

        LeaderBoardDataItem request = new LeaderBoardDataItem() { nombre = playerName, score = tScore };

        string response = WebRequestHelper.Post(urlLeaderBoardPost, request.ToJson());
        Debug.Log("@CreatePlayer: " + response);
        try
        {
            return JsonUtility.FromJson<LeaderBoardDataItem>(response).id;
        }
        catch (System.Exception e)
        {
            Debug.Log($"CreatePlayerFailed {e.Message}");
            return -1;
        }
    }

    public static  void UpdatePlayer(int playerId, int playerScore)
    {
        LeaderBoardDataItem request = new LeaderBoardDataItem() { id = playerId, score = playerScore.ToString() };
        string jsonRequest = request.ToJson();
        string response = WebRequestHelper.Put($"{urlLeaderBoardPut}/{playerId}", jsonRequest);
        Debug.Log("@UpdatePlayer: " + response);
    }

    public static LeaderBoardData GetAllPlayers() {
        string response = WebRequestHelper.Get(urlLeaderBoard);
        Debug.Log($"#MyLeaderBoard @LoadData: {urlLeaderBoard} \n Response: {response}");
        return response.ToObject<LeaderBoardData>();
    }
}
