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
    public float minFreq;
    public float startFreq;
    public float freqIncrement;
    public float tempFreq;
    public bool useCurve;

    public AnimationCurve freqAnimation;
    public AnimationCurve otherLaneFreq;
    public float otherFreq;
    public float timer;

    
    public Directions nextDir;
    public Directions dir;
    [Header("Rush Hourd")]
    public float rushHourCd;
    public float rushHourDuration;
    public bool onRushHour = false;
    public float rushHourFreqMult;

    [Header("CurrentCars")]
    public List<GameObject> currentCars;

    [Header("Queue Spawn")]
    public GameObject queuePrefab;
    public Directions queueDirection;
    public CarType queueType;
    public bool hasQueue = false;

    [Header("Pool")]
    public bool usesPool;
    public CarPool pool;


    //Dispatch an event every Time.deltaTime
    public static System.Action onTimePassed;

    void Start ()
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

    void StartRushHour()
    {
        onRushHour = true;

        Invoke("EndRushHour", rushHourDuration);

        tempFreq = frequency;
        frequency *= rushHourFreqMult;
        frequency = Mathf.Clamp(frequency, minFreq, startFreq);

        
        GuiManager.instance.ChangeRushHour(true);

        /*
        CancelInvoke("Spawn");
        InvokeRepeating("Spawn", tempFreq, frequency);
        */
    }
    void EndRushHour()
    {
        onRushHour = false;

        frequency = tempFreq;
        frequency -= freqIncrement;
        frequency = Mathf.Clamp(frequency, minFreq, startFreq);
        Invoke("StartRushHour", rushHourCd );
        
        GuiManager.instance.ChangeRushHour(false);

        /*
        CancelInvoke("Spawn");
        InvokeRepeating("Spawn", tempFreq , frequency);
        */

    }

    public void QueueSpawn(GameObject prefab, Directions dir, CarType type)
    {
        queuePrefab = prefab;
        queueDirection = dir;
        queueType = type;
        hasQueue = true;
    }

    void Spawn()
    {
        GameObject prefab;
        dir = Directions.NONE;
        CarSpawn carSpawn = GetCar();
        CarType ct = carSpawn.type;

        if (hasQueue)
        {
            prefab = queuePrefab;
            dir = queueDirection;
            hasQueue = false;
            ct = queueType;
        }
        else
        {
        
            prefab = carSpawn.prefab;

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
        car.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        car.GetComponent<Rigidbody>().velocity = Vector3.zero;
        car.transform.eulerAngles = Vector3.zero;
        
        car.transform.position = spawner.transform.position;
        car.SetDirection(carDir, dir);
        car.spawner = this;

        Color c = possibleColors[Random.Range(0, possibleColors.Count)];
        car.SetColor(c);

        currentCars.Add(car.gameObject);

        CalculateFrequency();

        Invoke("Spawn", frequency);
    }

   

    public void CalculateFrequency()
    {
        otherFreq = 0;
        if (useCurve)
        {
            var tRes = timer % 25;
            var t = tRes / 25;
            var freq = freqAnimation.Evaluate(t);
            otherFreq = otherLaneFreq.Evaluate(t);
            frequency = freq;
        }

        if (GameManager.instance.mode == GameMode.TRANSIT || GameManager.instance.mode == GameMode.DRIVER)
        {
            if (dir != nextDir)
                frequency *= otherFreq;
        }

        if (GameManager.instance.mode == GameMode.NO_BRAKES)
            frequency *= 2;
    }

    public void OnReset()
    {
        OnFinish();

        frequency = startFreq;
        tempFreq = startFreq;
        timer = 0;
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
  
    public void OnFinish()
    {
        StopAllCoroutines();
        ClearCars();
        onRushHour = false;
        GuiManager.instance.ChangeRushHour(false);
        CancelInvoke("EndRushHour");
        CancelInvoke("StartRushHour");
        CancelInvoke("Spawn");
    }

    public void ClearCars()
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
    public Transform GetSpawner(Directions dir)
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
    public Vector3 GetDirection(Directions dir)
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
    public CarSpawn GetCar()
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
