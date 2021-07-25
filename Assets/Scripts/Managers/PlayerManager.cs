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


    #region UI Handler

    public void Register()
    {
        string pName = GuiManager.instance.GetRegisterInputFieldValue();

        profile = new Profile
        {
            playerName = pName,
            idUser = -1
        };

        if(InternetConnectionManager.isOnline)
            profile.idUser = OnlineService.CreatePlayer(profile.playerName);

        profile.registered = profile.idUser == -1 ? "Local" : "Online";
           
        onLoadSuccesfull?.Invoke(profile);
    }

    #endregion

    void UpdatePlayerUI() {
        GuiManager.instance.ChangeHiText(profile.playerName);
        GuiManager.instance.ChangeRecordTransit(profile.maxScoreTransit.ToString());
    }

   public void UpdatePlayer(int playerId, int playerScore) {
        OnlineService.UpdatePlayer(playerId, playerScore);
        UpdatePlayerUI();
    }

    #region save and load local
    public void SaveLocalData()
    {
        Debug.Log("#PlayerManager @SaveLocalData");
        SaveManager.SaveData(Application.persistentDataPath + "/player.sav", profile);
    }
    void OnLoadSuccesfull(Profile profile) {
        CreatePlayerWhenRegistrationOccuredOffline(profile);
        onLoadSuccesfull?.Invoke(profile);
    }
    void CreatePlayerWhenRegistrationOccuredOffline(Profile profile) {
        if (profile.idUser == -1 && InternetConnectionManager.isOnline)
            profile.idUser = OnlineService.CreatePlayer(profile.playerName, profile.maxScoreTransit.ToString());
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

