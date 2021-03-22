using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{

    const string urlLeaderBoardPost = "https://heroku-demo-lucentini.herokuapp.com/usuarios/create";
    const string urlLeaderBoardPut = "https://heroku-demo-lucentini.herokuapp.com/usuarios/";


    public static System.Action<Profile> onLoadSuccesfull;
    public static System.Action onLoadFailed;

    public Profile profile;


    private void Awake()
    {
        GameManager.onGameEnd += SaveLocalData;
        GameManager.onBackToMenu += SaveLocalData;
        
        onLoadSuccesfull += (x) => UpdatePlayerUI();
        onLoadSuccesfull += (x) => SaveLocalData();
    }

    public void Register()
    {
        string pName = GuiManager.instance.GetRegisterInputFieldValue();

        int id = CreatePlayer(pName);

        profile = new Profile
        {
            registered = "Registered",
            playerName = pName,
            idUser = id == -1 ? -1 : id
        };

        onLoadSuccesfull?.Invoke(profile);
    }
    public void UpdatePlayerUI() {
        GuiManager.instance.ChangeHiText(profile.playerName);
        GuiManager.instance.ChangeRecordTransit(profile.maxScoreTransit.ToString());
    }

    #region create online

    public int CreatePlayer(string playerNombre)
    {
        LeaderBoardDataItem request = new LeaderBoardDataItem() { nombre = playerNombre, score = "0" };

        string jsonRequest = JsonUtility.ToJson(request);
        string response = WebRequestHelper.DoWebRequest(urlLeaderBoardPost, "POST", jsonRequest);
        Debug.Log("@CreatePlayer: " + response);
        try
        {
            return JsonUtility.FromJson<LeaderBoardDataItem>(response).id;
        }
        catch (System.Exception e) {
            Debug.Log($"CreatePlayerFailed {e.Message}");
            return -1;
        }
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
    void OnLoadSuccesfull(Profile profile) {
        if (profile.idUser == -1 && InternetConnectionManager.isOnline) {
            CreatePlayer(profile.playerName);
        }

        UpdatePlayerUI();
        SaveLocalData();
        onLoadSuccesfull?.Invoke(profile);
    }
    public void LoadLocalData()
    {   
        profile = SaveManager.LoadData<Profile>(Application.persistentDataPath + "/player.sav");
        Debug.Log(profile.ToStringFull());
        this.Log($"@LoadLocalData {profile.ToStringFull()}");
        if (profile != null)
        {
            OnLoadSuccesfull(profile);
        }
        else
        {
            onLoadFailed?.Invoke();
        }
    }
    #endregion
}

