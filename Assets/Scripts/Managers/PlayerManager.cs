using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    const string urlLeaderBoardPost = "https://heroku-demo-lucentini.herokuapp.com/usuarios/create";
    const string urlLeaderBoardPut = "https://heroku-demo-lucentini.herokuapp.com/usuarios/";


    public static System.Action<Profile> onLoadComplete;

    [Header("UI")]
    [SerializeField] private InputField registerInputfield;
    [SerializeField] private Text hiText;
    [SerializeField] private Text maxScoreTransit;


    public Profile profile;
    

    public void Register()
    {
        profile = new Profile();

        profile.registered = "Registered";
        profile.playerName = registerInputfield.text;
        hiText.text = "Hi, " + profile.playerName;

        int id = CreatePlayer(profile.playerName);

        profile.idUser = id;

        SaveLocalData();
        GameManager.instance.SwitchScreen(GameStatus.MODE_SELECTION);
    }

    public void ChangeRecordTransit(string msg)
    {
        maxScoreTransit.text = msg;
    }

    #region create online

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
    #endregion

    #region save and load local
    public void SaveLocalData()
    {
        Debug.Log("#PlayerManager @SaveLocalData");
        SaveManager.SaveData(Application.persistentDataPath + "/player.sav", profile);
    }
    public void LoadLocalData()
    {   
        profile = SaveManager.LoadData<Profile>(Application.persistentDataPath + "/player.sav");
        Debug.Log(profile.ToStringFull());
        this.Log($"@LoadLocalData {profile.ToStringFull()}");
        onLoadComplete?.Invoke(profile);
    }
    #endregion
}
