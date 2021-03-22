using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScreenManager : MonoBehaviour
{
    [ReadOnly]private GameObject current;

    #pragma warning disable 0649
    [SerializeField] private List<ScreenItem> screens;
    #pragma warning restore 0649

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
