using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    public static GuiManager instance;

    #pragma warning disable 0649

    [Header("Register")]

    [SerializeField] private InputField registerInputfield;

    [Header("Menu")]
   
    [SerializeField] private Text hiText;
    [SerializeField] private Text maxScoreTransit;

    [Header("End game")]
    [SerializeField] private Text scoreEndGame;
    //public Text coinsEndGame;
    //public Text xpEndGame;
    [SerializeField] private GameObject newRecordGo;
    [SerializeField] private Text endgameText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject continueButton;

    [Header("In game")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text rushHourText;
    [SerializeField] private Text trafficLightTimer;
   

    [Header("Tap to Start")]
    [SerializeField] private Text tapToStart;

    [Header("Prefabs and Other")]
    [SerializeField] private GameObject uiSituationPrefab;
    [SerializeField] private GameObject uiInstructionPrefab;
    [SerializeField] private GameObject uiButtonPrefab;
    [SerializeField] private GameObject uiButtonEmptyPrefab;
    [SerializeField] private GameObject uiAchivementPrefab;
    [SerializeField] private Transform gameCanvas;
    [SerializeField] private Transform achievmentCanvas;
    [SerializeField] private Transform achievmentMenuCanvas;

    private List<GameObject> uiAchievment;
    private List<GameObject> uiAchievmentMenu;

    #pragma warning restore 0649


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetRegisterInputFieldValue() => registerInputfield.text;
    public void ShowRestartAndContinue(bool restart, bool contin)
    {
        restartButton.SetActive(restart);
        continueButton.SetActive(contin);
    }

    public void ChangeRecordTransit(string msg) =>
        maxScoreTransit.text = $"Record {msg} ";
    public void ChangeHiText(string msg) =>
        hiText.text = $"Hi, {msg}"; 
    
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
    public UIButton InstantiateUIButton()=> Instantiate(uiButtonPrefab, gameCanvas).GetComponent<UIButton>();
    public UIButton InstantiateUIButtonEmpty()=> Instantiate(uiButtonEmptyPrefab, gameCanvas).GetComponent<UIButton>();
    public UISituation InstantiateUIInstructionEmpty() =>Instantiate(uiInstructionPrefab, gameCanvas).GetComponent<UISituation>();
    public UISituation InstantiateUISituation() =>Instantiate(uiSituationPrefab, gameCanvas).GetComponent<UISituation>();


    public void ChangeTrafficLightTimer(string msg) =>
        trafficLightTimer.text = msg;
    
    public void ChangeRushHour(bool on) =>
        rushHourText.gameObject.SetActive(on);
    public void ChangeScore(string score)
    {
        scoreText.text = "SCORE: " + score;
        scoreEndGame.text = "SCORE: " + score;
    }
    public void ChangeScoreActive(bool on) =>
        scoreText.gameObject.SetActive(on);

    public void ChangeScoreEndActive(bool on) =>
        scoreEndGame.gameObject.SetActive(on);

    public void ChangeNewRecord(bool on) =>
        newRecordGo.gameObject.SetActive(on);

    public void ChangeTapToStart(bool on) =>
        tapToStart.gameObject.SetActive(on);

    public void ChangeEndGameText(string msg) =>
        endgameText.text = msg;
}
