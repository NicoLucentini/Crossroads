using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public static int carCount = 0;

    [Header("Status")]
    [ReadOnly]public float timeAlive;
    [ReadOnly]public float speed;
    [ReadOnly]public float timeBreak;
    [ReadOnly]public Directions dir;
    [ReadOnly]public float instantiationTime;
    [ReadOnly]public Vector3 direction;
    [ReadOnly]public CarPool pool;

    [ReadOnly] public bool crashAgainstCar = false;
    [Header("Move settings")]
    public float baseSpeed;
    public float maxSpeed;
    public bool startsWithMaxSpeed = true;  
    public float acel;
    public float desacel;
    [ReadOnly]public float breakDistance = 2;
    public bool breakInstant = false;
    [ReadOnly]public bool breakOn = true;
    public static bool brokenMessage = false;
    UISituation brokenInstruction;
    [Header("BREAK")]
    public bool canBreak;
    public int breakChance = 3;
    public Sprite breakSprite;
    [ReadOnly]public bool isBroken = false;
    [Header("Reaction")]
    private float reactionTimeWithCars = .05f;
    private float reactionTimeWithLights = .2f;
    
    [Header("Rewards")]
    public int reward;
    [ReadOnly]
    public bool rewardSended = false;


    [Header("References")]
    public CarSpawner spawner;
    public Renderer mainRenderer;
    public bool changeColor = true;
    public CarType carType;
    public GameObject particleFire;


    bool canCollide = true;

    public virtual void Start()
    {
        GameManager.onGameEnd += OnGameEnd;
        canCollide = true;
        carCount++;
        gameObject.name = "Car " + carCount;
    }
    public virtual void Init()
    {
        
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.eulerAngles = Vector3.zero;
        maxSpeed = GameManager.globalCarSpeed;
        rewardSended = false;
        acel = maxSpeed * 3;
        speed = startsWithMaxSpeed ? maxSpeed : 0;
        timeBreak = 0;
        instantiationTime = Time.time;
        timeAlive = 0;
        StartCoroutine(CTTimeAlive());
        isBroken = false;
               
        if (canBreak)
        {
            breakChance = 2;
            int r = Random.Range(0, 100);
            if (r <= breakChance)
            {
                StartCoroutine(BreakCar());
            }
        }
    }

    UIButton brokenButton;
    GameObject firePart;
    IEnumerator  BreakCar()
    {
        int time = Random.Range(3, 6);
        yield return new WaitForSeconds(time);

        breakOn = true;
        speed = 0;
        brokenButton = GuiManager.instance.InstantiateUIButton();
        brokenButton.SetOnClick(UnbreakCar);
        brokenButton.SetOffset(new Vector3(0, 24, 0));
       // brokenButton.SetOffset(new Vector3(0, 128, 0));
        brokenButton.SetImage(breakSprite);
        brokenButton.ShowText(false);
        brokenButton.target = transform;
        isBroken = true;
        firePart = GameObject.Instantiate(particleFire, transform.position, Quaternion.identity);

        if (!brokenMessage)
        {
            brokenMessage = true;
            brokenInstruction = GuiManager.instance.InstantiateUIInstructionEmpty();
            brokenInstruction.SetText("TAP TO FIX CAR");
            brokenInstruction.Show(false, false, true);
            brokenInstruction.target = transform;
            brokenInstruction.SetOffset(new Vector3(0, 172, 0));

            GameManager.instance.ManageTime(0);
            yield return new WaitForSecondsRealtime(5);
            GameManager.instance.ManageTime(1);
        }

        Debug.Log("eNTRO A ROMPERESE");
    }

    private void UnbreakCar()
    {
        breakOn = false;
        isBroken = false;
        speed = maxSpeed;
        GameManager.instance.ManageTime(1);
        if (brokenInstruction!= null && brokenInstruction.gameObject != null)
            Destroy(brokenInstruction.gameObject);

        Destroy(firePart);
        GameManager.instance.stats.carFixedCount++;

        if (brokenButton != null)
            Destroy(brokenButton.gameObject);
    }
    IEnumerator CTTimeAlive()
    {
        while (true)
        {
            timeAlive += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    
    #region SET
    public void SetColor(Color c)
    {
        if(changeColor)
            mainRenderer.material.SetColor("_Color", c);
    }
    public virtual void SetDirection(Vector3 direction, Directions dir)
    {
        this.direction = direction;
        this.dir = dir;
        transform.LookAt(transform.position + direction);
    }
    #endregion

    public bool claxonOn = false;
    public virtual void Update()
    {
        if (breakOn)
        {
            timeBreak += Time.deltaTime;
            if (timeBreak > 3.5f)
            {
                if (!claxonOn)
                {
                    FindObjectOfType<CarClaxon>().Play();
                    claxonOn = true;
                }
            }
            
        }
    }

    protected virtual void Move()
    {
        if (!breakOn)
        {
            speed += acel * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
        }
        transform.position += direction * speed * Time.deltaTime;
    }

    public void FixedUpdate()
    {
        AutoBrake();

        if (!isBroken)
        {
            Move();
        }

    }

    #region BREAK
    public void BreakOnOff()
    {
        breakOn = !breakOn;
        if (breakOn) Break();
        else UnBrake();
    }
    public void UnBrake()
    {
        breakOn = false;
    }
    private void Break()
    {
        if (breakInstant)
            speed = 0;
        breakOn = true;
        CancelInvoke("UnBrake");
    }

    private void AutoBrake()
    {
        int mask = 1 << Layers.CARS | 1 << Layers.BARRIER;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, breakDistance, mask))
        {
            lastRay = hit.collider.gameObject.layer;

            if (lastRay == Layers.BARRIER)
            {
                if (hit.collider.gameObject.GetComponent<Barrier>().direction != dir)
                {
                    return;
                }
            }

            if (lastRay == Layers.CARS)
            {
                Car car = hit.collider.GetComponent<Car>();

                if (car.direction != direction)
                    return;
                
            }

            if (!breakInstant)
            {
                speed -= desacel * Time.deltaTime;
                speed = Mathf.Clamp(speed, 0, maxSpeed);
            }
            Break();

        }
        else
        {
            if (breakOn)
            {
                Invoke("UnBrake", lastRay == Layers.CARS ? reactionTimeWithCars : reactionTimeWithLights);
            }
        }
    }
    #endregion



    #region COLLISION/TRIGGER
    public int lastRay;    

    public  void OnCollisionEnter(Collision collision)
    {
        if (!canCollide) return;

        if (collision.gameObject.layer == Layers.BARRIER)
        {
            canCollide = false;
            if (collision.gameObject.GetComponent<Barrier>().direction == dir)
            {
            }
            else
            {
                //como hago para ignorar esta fucking collision...
                GetComponent<Collider>().isTrigger = true;
            }
        }

        if (collision.gameObject.layer == Layers.CARS )
        {
            Debug.Log("@OnCarCollision");
            canCollide = false;
            GameManager.instance.OnCarCollision();
            if (timeAlive > 2)
                GuiManager.instance.ChangeEndGameText("DONT LET CARS CRASH!!!");
            else {
                Debug.Log("Car " + gameObject.name + " crashed " + gameObject.name);
                GuiManager.instance.ChangeEndGameText("DONT LET CARS STACK!!!");
            }
        }
        if (collision.gameObject.layer == Layers.PEOPLE)
        {
            canCollide = false;
            GameManager.instance.OnCarCollision();
            GuiManager.instance.ChangeEndGameText("DONT CRASH  PEOPLE!!!");
        }

    }

    protected  void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.BOUNDARIES)
        {
            OnEndArrive();
        }
    }

    private void OnEndArrive()
    {
        if (!rewardSended)
        {
            rewardSended = true;

            float rewardMult = Mathf.Clamp(5 - timeBreak, 0, 5);
            int r = (int)(reward * rewardMult);
            UISituation endReward = GuiManager.instance.InstantiateUISituation();
            endReward.Show(false, false, true);
            endReward.SetText("+" + r);
            endReward.SetOffset(-direction);
            endReward.SetPos(transform.position + new Vector3(0, 2, 0));
            Color col;
            if (rewardMult <= 1)
                col = Color.red;
            else if (rewardMult > 1 && rewardMult < 3.5f)
                col = Color.yellow;
            else
                col = Color.green;

            endReward.SetTextColor(col);
            endReward.Anim();
            Destroy(endReward.gameObject, 1);
            GameManager.instance.OnCarPassed(r, carType);

        }

        spawner.currentCars.Remove(gameObject);

        OnDie();
    }

    protected virtual void OnGameEnd()
    {
        StopAllCoroutines();
        CancelInvoke();

        if (brokenButton != null)
            Destroy(brokenButton.gameObject);
        if (firePart != null)
            Destroy(firePart);

        if (brokenInstruction != null && brokenInstruction.gameObject != null)
            Destroy(brokenInstruction.gameObject);
    }
    public virtual void OnDie()
    {
        StopAllCoroutines();
        CancelInvoke();

        if (brokenButton != null)
            Destroy(brokenButton.gameObject);
        if(firePart!=null)
            Destroy(firePart);

        if (brokenInstruction != null && brokenInstruction.gameObject != null)
            Destroy(brokenInstruction.gameObject);

        if (spawner.usesPool)
            pool.DropCar(this);
        else
            Destroy(gameObject);
      
    }
    #endregion

    protected  void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * breakDistance, Color.red);
    }
}
