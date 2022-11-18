using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taxi : Car
{
    public static bool taxiFirstTime;
    public TaxiStop myStop;
    public bool onTaxiStop = false;
    public bool hasStopped = false;
    public float taxiWaitTime = 5;
    public float taxiWaitTimer = 5;


    UIButton taxiButton;

    UISituation taxiInstruction;
    public GameObject taxiUser;

    public override void Start()
    {
        base.Start();
        
    }
    public override void Init()
    {
        base.Init();
        myStop = TaxiStop.GetStop(dir);
        hasStopped = false;
        onTaxiStop = false;
        taxiWaitTimer = taxiWaitTime;
    }
  


    IEnumerator CTInstruction()
    {
        taxiFirstTime = true;
        //decir que toque el boton
        taxiInstruction = GuiManager.instance.InstantiateUIInstructionEmpty();
        taxiInstruction.SetText("TAP TO LEAVE STOP");
        taxiInstruction.Show(false, false, true);
        taxiInstruction.target = transform;
        taxiInstruction.offset = new Vector3(0, 172, 0);

        GameManager.instance.ManageTime(0);
        yield return new WaitForSecondsRealtime(5);

        CleanCt();

        //sacar el boton;

    }

    void CleanCt()
    {
        if (taxiInstruction != null)
        {
            if (taxiInstruction.gameObject != null)
                Destroy(taxiInstruction.gameObject);
            GameManager.instance.ManageTime(1);
        }

    }

    public void OnTaxiStop()
    {
        hasStopped = true;
        onTaxiStop = true;
        speed = 0;
        breakOn = true;

        taxiButton = GuiManager.instance.InstantiateUIButton();
        taxiButton.target = transform;
        taxiButton.SetOnClick(OnClickButton);
        taxiButton.SetOffset(new Vector3(0, 24, 0));

        if (!taxiFirstTime)
            StartCoroutine(CTInstruction());
    }

    public void OnEndTaxiStop()
    {
        onTaxiStop = false;
        speed = maxSpeed;
        breakOn = false;
        GameObject user = GameObject.Instantiate(taxiUser, myStop.transform.position, Quaternion.identity);
        user.GetComponent<TaxiUser>().SetDirection(-direction);
        if (taxiButton != null)
            Destroy(taxiButton.gameObject);
    
    }

    public void OnClickButton()
    {
        //esto puede ser super rustico..

        //taxiWaitTimer -= 0.5f;
        taxiWaitTimer -= 5;
        CleanCt();

    }

    public override void Move()
    {
        if(!onTaxiStop)
            base.Move();
    }

    public override void Update ()
    {
       
        if (!hasStopped)
        {
            if (Vector3.Distance(transform.position, myStop.transform.position) < 3)
            {
                OnTaxiStop();
            }
        }

        if (onTaxiStop)
        {
            taxiWaitTimer -= Time.deltaTime;

            if (taxiButton != null)
                taxiButton.SetText(taxiWaitTimer.ToString("n1"));

            if (taxiWaitTimer <= 0)
            {
                OnEndTaxiStop();
            }
            
        }
    }

   

    public override void SetDirection(Vector3 direction, Directions dir)
    {
        base.SetDirection(direction, dir);
        myStop = TaxiStop.GetStop(dir);
    }

    protected override void OnGameEnd()
    {
        base.OnGameEnd();
      

        if (taxiButton != null)
            Destroy(taxiButton.gameObject);
    }
    public override void OnDie()
    {
        if (taxiButton != null)
            Destroy(taxiButton.gameObject);
     
        base.OnDie();
    }

   
}
