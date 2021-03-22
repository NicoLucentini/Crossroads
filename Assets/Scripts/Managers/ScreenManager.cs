using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScreenManager : MonoBehaviour
{
    [ReadOnly] private GameObject current;
    [ReadOnly] private GameStatus status;
    #pragma warning disable 0649
    [SerializeField] private List<ScreenItem> screens;
    #pragma warning restore 0649

    private void Awake()
    { 
        PlayerManager.onLoadFailed += () => SwitchScreen(GameStatus.SIGN_IN);
        PlayerManager.onLoadSuccesfull += (x) => SwitchScreen(GameStatus.MAIN_MENU);
        GameManager.onBackToMenu += ()=> SwitchScreen(GameStatus.MAIN_MENU);
        GameManager.onClickPlay += () => SwitchScreen(GameStatus.TAP_TO_PLAY);
        GameManager.onGameStart += () => SwitchScreen(GameStatus.IN_GAME);
        GameManager.onGameResume += () => SwitchScreen(GameStatus.IN_GAME);
        GameManager.onGameEnd += () => SwitchScreen(GameStatus.END_GAME);

        SwitchScreen(GameStatus.SIGN_IN);
    }

    public void SwitchScreen(GameStatus status) {
        this.status = status;
        current?.SetActive(false);
        current = screens.Find(x => x.status == status).screen;
        current?.SetActive(true);
    }
    public GameObject GetCurrentScreen() {
        return current;
    }
    public GameStatus GetStatus() {
        return status;
    }
}

[System.Serializable]
public class ScreenItem {
    public GameStatus status;
    public GameObject screen;
}
