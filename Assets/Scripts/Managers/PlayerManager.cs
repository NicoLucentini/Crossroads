using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public static event System.Action<Profile> onLoadSuccesfull;
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

       
           
        onLoadSuccesfull?.Invoke(profile);
    }

    void RegisterOnline() {
        if (InternetConnectionManager.isOnline)
            profile.idUser = OnlineService.CreatePlayer(profile.playerName);

        profile.registered = profile.idUser == -1 ? "Local" : "Online";
    }
    #endregion

    void UpdatePlayerUI() {
        GuiManager.instance.ChangeHiText(profile.playerName);
        GuiManager.instance.ChangeRecordTransit(profile.maxScoreTransit.ToString());
    }

   public void UpdatePlayer(int playerId, int playerScore) {
       if(OnlineService.isOnline)
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
        if (OnlineService.isOnline)
        {
            CreatePlayerWhenRegistrationOccuredOffline(profile);
            CreatePlayerWhenPlayerIsNotRegisterdOnline(profile);
        }

        onLoadSuccesfull?.Invoke(profile);
    }
    void CreatePlayerWhenRegistrationOccuredOffline(Profile profile) {
        if (profile.idUser == -1)
            RegisterOnline();
    }
    void CreatePlayerWhenPlayerIsNotRegisterdOnline(Profile profile) {
        if (profile.idUser != -1 && OnlineService.GetPlayer(profile.idUser) == null) {
            RegisterOnline();
        }
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

