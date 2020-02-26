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

public class GameReward
{
    public GameReward(float xp, float coins)
    {
        this.xp = xp;this.coins = coins;
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


    Dictionary<GameMode, GameReward> rewards = new Dictionary<GameMode, GameReward>();

    [Header("Game References")]
    public List<Barrier> barriers;

    [Header("Status")]
    [ReadOnly]public bool isPaused;
    [ReadOnly]public bool gameIsRunning = false;
    public float cdTap = .5f;
    [ReadOnly]public bool tapEnabled = true;
    public static float globalCarSpeed;
    public GameMode mode;
    public GameStatus status = GameStatus.MODE_SELECTION;

   
    [Header("Stats")]
    [ReadOnly]public int ambulanceCount;
    [ReadOnly]public int taxiCount;
    [ReadOnly]public int firefighterCount;
    [ReadOnly]public int policeCount;
    [ReadOnly]public int oldmanCount;
    [ReadOnly]public int carPassed;
    [ReadOnly]public int normalCarsCount;
    [ReadOnly]public int score;
    [ReadOnly]public int carFixedCount;
    //ui
    [Header("UI")]

    [Header("Ui Screens")]
    public GameObject signIn;
    public GameObject modeSelect;
    public GameObject tapAndScore;
    public GameObject game;
    public GameObject endGame;
    public GameObject shopGo;
    

    [Header("Ui Others")]
    public GameObject gameLights;
    public GameObject current;
    public GameObject adGo;
    public Image pauseImage;
    public Text pauseText;
    public Button freeCoinsBut;
    public Text freeCoinsTex;
    public Text pointsText;
    public Text achievmentRewardText;
    public Text achievmentLevelText;

    [Header("XpTable")]
    public List<int> xpTable;
    public int profileMaxLevel = 25;

    
    [Header("Ad")]
    public int gameCount;
    public int gamesPlayedForAd = 3;
    public int freeCoinsCDInMinutes = 60;
    public bool adView = false;

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
       
        profile.playerName = "Player " + Random.Range(0, 100);

        //realTimeManager.onDateLoaded += UpdateFreeCoinsButton;
        onLoadComplete += UpdateAfterLoadData;

        rewards.Add(GameMode.CAREER, new GameReward(0.5f, .5f));
        rewards.Add(GameMode.TRANSIT, new GameReward(0.5f, .5f));
        rewards.Add(GameMode.NO_BRAKES, new GameReward(.75f, .75f));

       
    }
    
    public void Start()
    {
        LoadData();

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
        print("UpdateAfterLoadData" + (profile == null ));
       if(profile != null)
            UpdateProfileUI();     
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
        score += reward;

        growScore += reward;

        if (growScore > growValue)
        {
            growScore -= 100;
            if(onScore!=null)
                onScore();
        }
        

        guiManager.ChangeScore(score.ToString());
    }
    public void OnCarPassed(int rewards, CarType cType)
    {
        AddReward(rewards);
        carPassed++;

       
        if (cType == CarType.AMBULANCE)
            ambulanceCount++;
        else if (cType == CarType.COPS)
            policeCount++;
        else if (cType == CarType.BOMBERS)
            firefighterCount++;
        else if (cType == CarType.TAXI)
            taxiCount++;
        else
            normalCarsCount++;

    }
   
    public void OnOldManPassed(int reward)
    {
        score += reward;
        oldmanCount++;

        guiManager.ChangeScore(score.ToString());
       
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
      //uiProfile.ChangeLoginText("Welcome " + profile.playerName);
    }

    bool CheckAndUpdateScore()
    {
      
        Debug.Log("Score " + score + ", profile record " + profile.maxScoreTransit);
        if (score > profile.maxScoreTransit)
        {

            Debug.Log("recoooooord Score " + score + ", profile record " + profile.maxScoreTransit);
            profile.maxScoreTransit = score;
            uiProfile.ChangeRecordTransit("RECORD " + profile.maxScoreTransit);
            return true;
        }
        return false;
    }

    void CheckAdCount()
    {
        if (gameCount % gamesPlayedForAd == 0)
            AdManager.instance.AdShow(() => { print("Ad View"); });
    }

    void ResetCounts()
    {
        oldmanCount = 0;
        taxiCount = 0;
        ambulanceCount = 0;
        policeCount = 0;
        firefighterCount = 0;
        normalCarsCount = 0;
        score = 0;
        carPassed = 0;
    }
    #endregion


    #region SAVE AND LOAD
    
    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Create);

        bf.Serialize(fs,profile);
        fs.Close();
    }
    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/player.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(Application.persistentDataPath + "/player.sav", FileMode.Open);

            profile = bf.Deserialize(fs) as Profile;
            fs.Close();
          
        }
        onLoadComplete?.Invoke();
    }

    #endregion

    #region UI handlers

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
        current.SetActive(false);
        status = s;
        switch (s)
        {
            case GameStatus.MODE_SELECTION: current = modeSelect; break;
            case GameStatus.TAP_TO_PLAY: current = tapAndScore; break;
            case GameStatus.IN_GAME: current = game; break;
            case GameStatus.END_GAME: current = endGame; break;
            case GameStatus.SHOP: current = shopGo;break;
            case GameStatus.SIGN_IN: current = signIn; break;
        }

        current.SetActive(true);
    }
    #endregion

    #region GAME EVENTS

    public void OnGameStart()
    {
        print("OnGameStart");

        if (onGameStart != null)
            onGameStart();

        Time.timeScale = 1;

        gameCount++;
        CheckAdCount();
        ResetCounts();

        guiManager.ChangeTapToStart(false);
        guiManager.ChangeScoreActive(true);
        guiManager.ChangeScore(score.ToString());
        spawner.OnReset();
        spawner.enabled = true;
        controller.enabled = true;
        controller.SetOnTap();
        adView = false;
        gameIsRunning = true;
        FindObjectOfType<OldMan>().Init();
        skyboxSwitcher.SetSkybox();
        SwitchScreen(GameStatus.IN_GAME);
        guiManager.ShowRestartAndContinue(true, true);

    }
    
    public void OnEndGame(bool won)
    {

        bool newRecord = CheckAndUpdateScore();
        Debug.Log("New Record ? " + newRecord);
       

        guiManager.ChangeNewRecord(newRecord);

        SwitchScreen(GameStatus.END_GAME);
        if(!won)
            adGo.SetActive(!adView);
        Time.timeScale = 0;
        gameIsRunning = false;
        spawner.enabled = false;
        controller.enabled = false;

        onGameEnd.Invoke();
        SaveData();

    }
    
    public void OnResumeGame()
    {
        SwitchScreen(GameStatus.IN_GAME);
        adView = true;
        Time.timeScale = 1;
        spawner.OnReset();
        gameIsRunning = true;
        spawner.enabled = true;
        controller.enabled = true;
    }

    #endregion
}
