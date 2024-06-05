using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum Directions
{
    TOP,
    BOT,
    RIGHT,
    LEFT,
    NONE,
}
public class CarSpawner : MonoBehaviour
{
    [Header("Spawn References")]
    public List<GameObject> carPrefabs;
    public List<Color> possibleColors;
    public List<CarSpawn> carSpawns;

    [Header("Spawn Points")]
    public Transform topSpawner;
    public Transform botSpawner;
    public Transform rightSpawner;
    public Transform leftSpawner;


    [Header("Spawn Frequencies")]
    public float frequency;

    public AnimationCurve freqAnimation;

    [Header("CurrentCars")]
    public List<GameObject> currentCars;

    [Header("Pool")]
    public bool usesPool;
    public CarPool pool;



    private int carsSpawned = 0;


    //Dispatch an event every Time.deltaTime
    public static System.Action onTimePassed;

    void Start()
    {
        Shop.onCarBuyed += CarChances;

        OnReset();
        CarChances();
    }

    void CarChances()
    {
        var realSpawns = carSpawns.Where(x => !x.blocked).ToList();

        int count = realSpawns.Count - 4;

        float total = 80 / count;

        foreach (var rs in realSpawns)
        {
            rs.chance = (int)total;
        }


        //ambu
        carSpawns[10].chance = (int)(0.03f * 100);
        //fire truck
        carSpawns[11].chance = (int)(0.03f * 100);
        //police
        carSpawns[12].chance = (int)(0.03f * 100);
        //taxi
        carSpawns[13].chance = (int)(0.03f * 100);

    }


    void Spawn()
    {

        dir = Directions.NONE;
        carsSpawned++;
        if (carsSpawned % 5 == 0 && carSpawns.Exists(x => x.blocked))
        {
            carSpawns.FirstOrDefault(x => x.blocked).blocked = false;
        }

        CarSpawn carSpawn = GetCar();
        CarType ct = carSpawn.type;


        var prefab = carSpawn.prefab;

        if (nextDir == Directions.NONE)
        {
            int pos = Random.Range(0, 4);
            dir = (Directions)pos;
            nextDir = dir;
        }
        else
        {
            dir = nextDir;
            int pos = Random.Range(0, 4);
            nextDir = (Directions)pos;

        }

        Transform spawner = GetSpawner(dir);
        Vector3 carDir = GetDirection(dir);

        Car car = null;
        if (usesPool)
            car = pool.GetCar(ct);
        else
        {
            GameObject carGo = GameObject.Instantiate(prefab, spawner.transform.position, Quaternion.identity);
            car = carGo.GetComponent<Car>();
        }

        car.Init();

        car.transform.position = spawner.transform.position;
        car.SetDirection(carDir, dir);
        car.spawner = this;

        Color c = possibleColors[Random.Range(0, possibleColors.Count)];
        car.SetColor(c);

        currentCars.Add(car.gameObject);

        //CalculateFrequency();
        RefactorFrequency();

        Invoke("Spawn", frequency);
    }


    public float frequencyTimeInSeconds = 60;
    private Directions dir;
    private Directions nextDir;
    private float timer;

    private void RefactorFrequency() {

        var freqPoint = (timer % frequencyTimeInSeconds) / frequencyTimeInSeconds;

        Debug.Log( "Timer "+ timer + ", Freq point " + freqPoint);

        var freqValue = freqAnimation.Evaluate(freqPoint);

        frequency = freqValue;
    }

    public void OnReset()
    {
        OnFinish();

        timer = 0;
        carsSpawned = 0;
        //InvokeRepeating("Spawn", 2, frequency);

        Invoke("Spawn", 2);
        StopCoroutine("CTTimer");
        StartCoroutine(CTTimer());
       // Invoke("StartRushHour", rushHourCd);
      
    }

    IEnumerator CTTimer()
    {
        while (true)
        {
            timer += Time.deltaTime;
            if (onTimePassed != null)
                onTimePassed();
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnFinish()
    {
        StopAllCoroutines();
        ClearCars();
        GuiManager.instance.ChangeRushHour(false);
        CancelInvoke("EndRushHour");
        CancelInvoke("StartRushHour");
        CancelInvoke("Spawn");
    }

    private void ClearCars()
    {

        GameObject temp = null;
        for (int i = 0; i < currentCars.Count; i++)
        {
            temp = currentCars[i];
            temp.GetComponent<Car>().OnDie();
        }
        currentCars.Clear();
    }

    public int RouletteSelection(List<int> values)
    {
        int winner = 0;
        int sum = 0;

        for (int i = 0; i < values.Count; i++)
        {
            sum += values[i];
        }

        int rand = Random.Range(0, sum);
        sum = 0;

        for (int i = 0; i < values.Count; i++)
        {
            if (rand < sum + values[i])
                return i;
            sum += values[i];
        }
        return winner;
    }
    private Transform GetSpawner(Directions dir)
    {
        switch (dir)
        {
            case Directions.TOP: return topSpawner;
            case Directions.BOT: return botSpawner;
            case Directions.RIGHT: return rightSpawner;
            case Directions.LEFT: return leftSpawner;
        }
        return GetSpawner(Directions.TOP);
    }
    private Vector3 GetDirection(Directions dir)
    {
        switch (dir)
        {
            case Directions.TOP: return new Vector3(0, 0, 1);
            case Directions.BOT: return new Vector3(0, 0, -1);
            case Directions.RIGHT: return new Vector3(1, 0, 0);
            case Directions.LEFT: return new Vector3(-1, 0, 0);
        }
        return GetDirection(Directions.TOP);

    }
    private CarSpawn GetCar()
    {
        var unblockedCars = carSpawns.Where(x => !x.blocked).ToList();
        List<int> chances = new List<int>();
        for (int i = 0; i < unblockedCars.Count; i++)
        {
            chances.Add(unblockedCars[i].chance);
        }
        int winner = RouletteSelection(chances);

        return unblockedCars[winner];

    }
}


[System.Serializable]
public class CarSpawn
{
    public int id;
    public GameObject prefab;
    public CarType type;
    public int chance;
    public bool blocked;
    public int unblockCost;
    public Sprite carSprite;

}
public enum CarType
{
    NORMAL,
    HATCHBACK,
    COUPE,
    VAN,
    MICRO,
    MICROTRANSPORT,
    MICROCARGO,
    MPV,
    STATION,
    PICKUP,
    BUS,
    AMBULANCE,
    TAXI,
    COPS,
    BOMBERS,
   
}
