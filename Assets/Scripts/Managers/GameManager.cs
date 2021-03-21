using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum GameMode
{
   NO_BRAKES,
   TRANSIT,
   DRIVER,
   CAREER,
}
public enum GameStatus
{
    SIGN_IN,
    MODE_SELECTION, //REPLACES IN FUTURE WITH MAIN MENU
    SHOP,//SHOP
    TAP_TO_PLAY, // TAP TO PLAY
    IN_GAME, // DURING GAME
    END_GAME, //AFTER LOSING
}
[System.Serializable]
public class GameStats {
     [ReadOnly] public int ambulanceCount;
    [ReadOnly] public int taxiCount;
    [ReadOnly] public int firefighterCount;
    [ReadOnly] public int policeCount;
    [ReadOnly] public int oldmanCount;
    [ReadOnly] public int carPassed;
    [ReadOnly] public int normalCarsCount;
    [ReadOnly] public int score;
    [ReadOnly] public int carFixedCount;
    [ReadOnly] public int timer;

}
public class GameReward
{
    public GameReward(float xp, float coins)
    {
        this.xp = xp;
        this.coins = coins;
    }
    public float xp;
    public float coins;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    #region EVENTS
    public static System.Action onGameEnd;
    public static System.Action onGameStart;
    public static System.Action onGameResume;
    public static System.Action onLoadComplete;
    public static System.Action onScore;
    #endregion


    [Header("Manager References")]
    public Profile profile;
    public GameController controller;
    public CarSpawner spawner;
    public RealtimeManager realTimeManager;
    public BuildingsManager buildingManager;
    public AchievmentManager achievmentManager;
    public Shop shop;
    public SkyboxSwitcher skyboxSwitcher;
    public UIProfile uiProfile;
    public TimerManager timerManager;
    public GuiManager guiManager;
    public GameServicesMgr gameServicesManager;
    public ScreenManager screenManager;
    public PlayerManager playerManager;
    public MyLeaderBoard myLeaderboard;

    Dictionary<GameMode, GameReward> rewards = new Dictionary<GameMode, GameReward>();

    [Header("Game References")]
    public List<Barrier> barriers;

    [Header("Status")]
    [ReadOnly] public bool isPaused;
    [ReadOnly] public bool gameIsRunning = false;
    public float cdTap = .5f;
    [ReadOnly] public bool tapEnabled = true;
    public static float globalCarSpeed;
    public GameMode mode;
    public GameStatus status = GameStatus.MODE_SELECTION;


    [Header("Stats")]
   

    [ReadOnly] public GameStats stats;

    [Header("UI")]

    [Header("Ui Others")]
    public GameObject gameLights;
    public GameObject adGo;
    public Image pauseImage;
    public Text pauseText;
    public Button freeCoinsBut;
    public Text freeCoinsTex;
    public Text pointsText;
    public Text achievmentRewardText;
    public Text achievmentLevelText;
    public InputField registerInputfield;
    public Text hiText;
    [Header("XpTable")]
    public List<int> xpTable;
    public int profileMaxLevel = 25;


    public int gameCount;


    [Header("Game settings")]
    [Space(10)]
    [Header("Transit")]
    public float transit_StartFreq;
    public float transit_MinFreq;
    public float transit_RushHour;
    public float transit_Incr;
    public float transit_Speed;
    [Header("Breaks")]
    public float brakes_StartFreq;
    public float brakes_MinFreq;
    public float brakes_RushHour;
    public float brakes_Incr;
    public float brakes_Speed;

    public static bool isOnline;

    private void Awake()
    {
        if (instance == null) instance = this;

        CacheManagers();

        profile.playerName = "Player " + Random.Range(0, 100);

        //realTimeManager.onDateLoaded += UpdateFreeCoinsButton;
        onLoadComplete += UpdateAfterLoadData;
        UIProfile.onLoginSuccesfull += OnLoginSuccesful;

        rewards.Add(GameMode.CAREER, new GameReward(0.5f, .5f));
        rewards.Add(GameMode.TRANSIT, new GameReward(0.5f, .5f));
        rewards.Add(GameMode.NO_BRAKES, new GameReward(.75f, .75f));
    }
    void OnLoginSuccesful() {
        SaveData();
        SwitchScreen(GameStatus.MODE_SELECTION);
    }
    void CacheManagers() {
        if (guiManager == null) guiManager = FindObjectOfType<GuiManager>();
        if (controller == null) controller = FindObjectOfType<GameController>();
        if (realTimeManager == null) realTimeManager = FindObjectOfType<RealtimeManager>();
        if (buildingManager == null) buildingManager = FindObjectOfType<BuildingsManager>();
        if (timerManager == null) timerManager = FindObjectOfType<TimerManager>();
        if (achievmentManager == null) achievmentManager = FindObjectOfType<AchievmentManager>();
        if (shop == null) shop = FindObjectOfType<Shop>();
        if (skyboxSwitcher == null) skyboxSwitcher = FindObjectOfType<SkyboxSwitcher>();
        if (uiProfile == null) uiProfile = FindObjectOfType<UIProfile>();
        if (spawner == null) spawner = FindObjectOfType<CarSpawner>();
        if (gameServicesManager == null) gameServicesManager = FindObjectOfType<GameServicesMgr>();
        if (screenManager == null) screenManager = FindObjectOfType<ScreenManager>();
        if (playerManager == null) playerManager = FindObjectOfType<PlayerManager>();
        if (myLeaderboard == null) myLeaderboard = FindObjectOfType<MyLeaderBoard>();
    }

    public void Start()
    {
        LoadData();
        ResetStats();
    
    }

    public void ManageTime(float timeValue)
    {
        if (gameIsRunning)
        {
            Time.timeScale = timeValue;
        }
    }

    void UpdateAfterLoadData()
    {
       Debug.Log("UpdateAfterLoadData" + (profile == null ));

        if (profile != null)
        {
            UpdateProfileUI();
            myLeaderboard.SetPlayerId(profile.idUser);
        }
        else
        {
            SwitchScreen(GameStatus.SIGN_IN);
        }
        
    }


    public Animation testAnim;
    public void Update()
    {
        //hack building
        if (Input.GetKeyDown(KeyCode.Space))
            testAnim.Play();


        if (!tapEnabled) return;


        if (Input.GetMouseButton(0))
        {
            if (status == GameStatus.TAP_TO_PLAY)
                OnGameStart();
        }
    
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (status == GameStatus.SHOP)
                SwitchScreen(GameStatus.MODE_SELECTION);
            else if (status == GameStatus.MODE_SELECTION)
                QuitGame();
        }
       
      
    }

    


    #region click cd
    IEnumerator ResetTapCt()
    {
        yield return new WaitForSecondsRealtime(cdTap);
        tapEnabled = true;

    }
    public void ResetTap()
    {
        tapEnabled = true;
    }
    #endregion

   

    #region Game Situations



    public void OnCarCollision()
    {
        Debug.Log("@OnCarCollision");
        //PERDISTE
        if (gameIsRunning)
        {
            Debug.Log("You Lose");
            OnEndGame(false);
        }
    }

    public int growScore = 0;
    public int growValue = 100;
    public void AddReward(int reward)
    {
        stats.score += reward;
        growScore += reward;

        if (growScore > growValue)
        {
            growScore -= 100;
            if(onScore!=null)
                onScore();
        }
        

        guiManager.ChangeScore(stats.score.ToString());
    }
    public void OnCarPassed(int rewards, CarType cType)
    {
        AddReward(rewards);
        stats.carPassed++;

        switch (cType)
        {
            case CarType.AMBULANCE:
                stats.ambulanceCount++;break;
            case CarType.COPS:
                stats.policeCount++; break;
            case CarType.BOMBERS:
                stats.firefighterCount++; break;
            case CarType.TAXI:
                stats.taxiCount++; ; break;
            default:
                stats.normalCarsCount++;break;

        }
    }
   
    public void OnOldManPassed(int reward)
    {
        stats.score += reward;
        stats.oldmanCount++;

        guiManager.ChangeScore(stats.score.ToString());
       
    }

    public void AddXp(int xp)
    {
        profile.experience += xp;
        if (profile.experience >= xpTable[profile.level-1])
        {
            if (profileMaxLevel > profile.level)
            {
                profile.level++;
                profile.experience -= xpTable[profile.level - 1];
            }
        }

        UpdateProfileUI();

    }

    void UpdateProfileUI()
    {
        SwitchScreen(GameStatus.MODE_SELECTION);
        uiProfile.ChangeRecordTransit("Record " +  profile.maxScoreTransit);
        hiText.text = "Hi, " + profile.playerName;
      //uiProfile.ChangeLoginText("Welcome " + profile.playerName);
    }

    bool IsRecord(int newScore, int oldScore)
    {
        return newScore > oldScore;
    }

    void CheckAdCount()
    {
        if (gameCount % AdManager.instance.gamesPlayedForAd == 0)
            AdManager.instance.AdShow(() => { print("Ad View"); });
    }

    void ResetStats()
    {
        stats = new GameStats
        {
            oldmanCount = 0,
            taxiCount = 0,
            ambulanceCount = 0,
            policeCount = 0,
            firefighterCount = 0,
            normalCarsCount = 0,
            score = 0,
            carPassed = 0
        };
    }
    #endregion


    #region SAVE AND LOAD
    
    public void SaveData()
    {
        SaveManager.SaveData(Application.persistentDataPath + "/player.sav", profile);
    }
    public void LoadData()
    {
        profile = SaveManager.LoadData<Profile>(Application.persistentDataPath + "/player.sav");
        onLoadComplete?.Invoke();
    }

    #endregion

    #region UI handlers

    public void Register() {

        profile = new Profile();
        
        profile.registered = "Registered";        
        profile.playerName = registerInputfield.text;
        hiText.text = "Hi, " + profile.playerName;

        int id = playerManager.CreatePlayer(profile.playerName);
        profile.idUser = id;


        SaveData();
        SwitchScreen(GameStatus.MODE_SELECTION);
    }



    //QUIT GAME SE LLAMA CON EL BOTON O CON LA TECLA DE ATRAS

    public void MainMenuFromShop()
    {
        SwitchScreen(GameStatus.MODE_SELECTION);
    }
    public void QuitGame()
    {
        //save data (?
        Application.Quit();
    }

    //CLICKEA EL BOTON DE SHOP
    public void OnClickShop()
    {
        SwitchScreen(GameStatus.SHOP);
    }

    //CLICKEA EL BOTON DE PAUSA EN LA PARTIDA
    public void PauseOnOff()
    {

        isPaused = !isPaused;
        SoundManager.instance.PauseOnOff(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
        pauseImage.color = isPaused ? new Color(1, 1, 1) : new Color(41 / 255f, 41 / 255f, 41 / 255f);

    }

    //CLICKEA EL BOTON DE CONTINUAR JUGANDO, MUESTRA UN AD
    public void OnContinuePlaying()
    {
        AdManager.instance.AdShow(OnResumeGame);
    }

    //CLIKEA EL BOTON DE GOTO MENU CUANDO TERMINA LA PARTIDA
    public void OnGoToMenuFromGame()
    {
        guiManager.ChangeTapToStart(true);
        guiManager.ChangeRushHour(false);

        gameIsRunning = false;
        spawner.enabled = false;
        controller.enabled = false;
        tapEnabled = false;
        
        
        StartCoroutine(ResetTapCt());

        SaveData();
        SwitchScreen(GameStatus.MODE_SELECTION);
    }

    public void OnClickRestart()
    {
        if (mode == GameMode.TRANSIT)
            StartTransitMode();
      

        OnGameStart();

    }

    //CLICKEA EL BOTON DE MODO TRANSIT
    public void StartTransitMode()
    {
        mode = GameMode.TRANSIT;
        /*
        spawner.startFreq = transit_StartFreq;
        spawner.minFreq = transit_MinFreq;
        spawner.rushHourFreqMult = transit_RushHour;
        spawner.freqIncrement = transit_Incr;
    */
        globalCarSpeed = transit_Speed;

        foreach (var b in barriers)
        {
            b.EnableRenderer(false);
            b.EnableColliders(false);
        }
        gameLights.SetActive(true);

        SwitchScreen(GameStatus.TAP_TO_PLAY);
    }
   
    //CAMBIA LA PANTALLA DEL MENU
    public void SwitchScreen(GameStatus s)
    {
        status = s;
        screenManager.SwitchScreen(s);
    }
    #endregion

    #region GAME EVENTS

    public void OnGameStart()
    {
        print("OnGameStart");

        onGameStart?.Invoke();

        Time.timeScale = 1;

        gameCount++;
        CheckAdCount();
        ResetStats();

        guiManager.ChangeTapToStart(false);
        guiManager.ChangeScoreActive(true);
        guiManager.ChangeScore(stats.score.ToString());
        spawner.OnReset();
        spawner.enabled = true;
        controller.enabled = true;
        controller.SetOnTap();
        AdManager.instance.adView = false;
        gameIsRunning = true;
        FindObjectOfType<OldMan>().Init();
        skyboxSwitcher.SetSkybox();
        SwitchScreen(GameStatus.IN_GAME);
        guiManager.ShowRestartAndContinue(true, true);

    }
    
    public void OnEndGame(bool won)
    {
        Time.timeScale = 0;
        gameIsRunning = false;
        Debug.Log($"@OnEndGame {won}");

        bool newRecord = IsRecord(stats.score, profile.maxScoreTransit);

        if (newRecord)
        { 
            profile.maxScoreTransit = stats.score;
            playerManager.UpdatePlayer(profile.idUser, stats.score);
            uiProfile.ChangeRecordTransit("RECORD " + profile.maxScoreTransit);
        }


        //gameServicesManager.ReportScore(score);

        SwitchScreen(GameStatus.END_GAME);

        guiManager.ChangeNewRecord(newRecord);

        /*
        if (!won)
            adGo.SetActive(!adView);
            */
        Debug.Log($"@OnEndGame Pre Time Scale");
     
        spawner.enabled = false;
        controller.enabled = false;
        Debug.Log($"@OnEndGame Post Time Scale");
        onGameEnd?.Invoke();
       // Time.timeScale = 0;
        Debug.Log($"@OnEndGame Post Invoke");
        SaveData();
        Debug.Log($"@OnEndGame Post Save");

    }
    
    public void OnResumeGame()
    {
        SwitchScreen(GameStatus.IN_GAME);
        AdManager.instance.adView = true;
        Time.timeScale = 1;
        spawner.OnReset();
        gameIsRunning = true;
        spawner.enabled = true;
        controller.enabled = true;
        onGameResume?.Invoke();
    }

    #endregion
}
