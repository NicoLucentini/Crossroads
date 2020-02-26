using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile; 

public class GameServicesMgr : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameServices.ManagedInit();
    }

    private void OnEnable()
    {
        GameServices.UserLoginSucceeded += OnUserLoginSucceeded;
        GameServices.UserLoginFailed += OnUserLoginFailed;
    }
    private void OnDisable()
    {
        GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        GameServices.UserLoginFailed -= OnUserLoginFailed;
    }
    // Event handlers
    void OnUserLoginSucceeded()
    {
        Debug.Log("User logged in successfully.");
    }

    void OnUserLoginFailed()
    {
        Debug.Log("User login failed.");
    }
    public bool Initialized() {
        return GameServices.IsInitialized();
    }

    public void ShowLeaderboardUi() {
        // Check for initialization before showing leaderboard UI
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI("Transit");
        }
        else
        {
#if UNITY_ANDROID
            GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
    Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void ReportScore(int score) {
        GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_Transit);
    }
}
