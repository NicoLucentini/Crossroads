using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScreenManager : MonoBehaviour
{
    public GameObject current;

    [SerializeField] private List<ScreenItem> screens;
    
    public void SwitchScreen(GameStatus status) {

        current?.SetActive(false);
        current = screens.Find(x => x.status == status).screen;
        current?.SetActive(true);
    }
    public GameObject GetCurrentScreen() {
        return current;
    }
    
}

[System.Serializable]
public class ScreenItem {
    public GameStatus status;
    public GameObject screen;
}
