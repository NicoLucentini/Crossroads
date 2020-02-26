using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public static GuiManager instance;

    [Header("End game")]
    public Text scoreEndGame;
    //public Text coinsEndGame;
    //public Text xpEndGame;
    public GameObject newRecordGo;
    public Text endgameText;
    public GameObject restartButton;
    public GameObject continueButton;

    [Header("In game")]
    public Text scoreText;
    public Text rushHourText;
    public Text trafficLightTimer;
   

    [Header("Tap to Start")]
    public Text tapToStart;
    


   [Header("Prefabs and Other")]
    public GameObject uiSituationPrefab;
    public GameObject uiInstructionPrefab;
    public GameObject uiButtonPrefab;
    public GameObject uiButtonEmptyPrefab;
    public GameObject uiAchivementPrefab;
    public Transform gameCanvas;
    public Transform achievmentCanvas;
    public Transform achievmentMenuCanvas;

    public List<GameObject> uiAchievment;
    public List<GameObject> uiAchievmentMenu;



    public void ChangeCoinsAndXp(string c, string xp)
    {
     //   coinsEndGame.text = "+ " + c;
    //xpEndGame.text = "+ " + xp;
    }

    public void ShowRestartAndContinue(bool restart, bool contin)
    {
        restartButton.SetActive(restart);
        continueButton.SetActive(contin);
    }

    public UISituation InstantiateUISituation()
    {   
        GameObject uiGO = GameObject.Instantiate(uiSituationPrefab, gameCanvas);
        return uiGO.GetComponent<UISituation>();
    }
    public void ClearAchievment()
    {
        foreach (var a in uiAchievment)
        {
            Destroy(a);
        }
        uiAchievment.Clear();
    }
    public void ClearAchievmentMenu()
    {
        foreach (var a in uiAchievmentMenu)
        {
            Destroy(a);
        }
        uiAchievmentMenu.Clear();
    }
    public UISituation InstantiateUIAchievment()
    {
        GameObject uiGO = GameObject.Instantiate(uiAchivementPrefab, achievmentCanvas);
        uiAchievment.Add(uiGO);
        return uiGO.GetComponent<UISituation>();
    }
    public UISituation InstantiateUIAchievmentMenu()
    {
        GameObject uiGO = GameObject.Instantiate(uiAchivementPrefab, achievmentMenuCanvas );
        uiAchievmentMenu.Add(uiGO);
        return uiGO.GetComponent<UISituation>();
    }
    public UIButton InstantiateUIButton()
    {
        GameObject uiGO = GameObject.Instantiate(uiButtonPrefab, gameCanvas);
        return uiGO.GetComponent<UIButton>();
    }
    public UIButton InstantiateUIButtonEmpty()
    {
        GameObject uiGO = GameObject.Instantiate(uiButtonEmptyPrefab, gameCanvas);
        return uiGO.GetComponent<UIButton>();
    }
    public UISituation InstantiateUIInstructionEmpty()
    {
        GameObject uiGO = GameObject.Instantiate(uiInstructionPrefab, gameCanvas);
        return uiGO.GetComponent<UISituation>();
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void ChangeTrafficLightTimer(string msg)
    {
        trafficLightTimer.text = msg;
    }
    public void ChangeRushHour(bool on)
    {
        rushHourText.gameObject.SetActive(on);
    }
    public void ChangeScore(string score)
    {
        scoreText.text = "SCORE: " + score;
        scoreEndGame.text = "SCORE: " + score;
    }
    public void ChangeScoreActive(bool on)
    {
        scoreText.gameObject.SetActive(on);
    }
    public void ChangeScoreEndActive(bool on)
    {
        scoreEndGame.gameObject.SetActive(on);
    }
    public void ChangeNewRecord(bool on)
    {
        newRecordGo.gameObject.SetActive(on);
    }
    public void ChangeTapToStart(bool on)
    {
        tapToStart.gameObject.SetActive(on);
    }
    public void ChangeEndGameText(string msg)
    {
        endgameText.text = msg;
    }
}
