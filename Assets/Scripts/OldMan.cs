using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldMan : MonoBehaviour
{
    public static bool firstTime;
    public WalkingPath wp;
    public float speed = 2;
    public float baseSpeed = 2;
    public Vector3 dir;
    public bool canMove;
    UIButton ui;
    
    UISituation uiInstruction;
    public Sprite icon;
    WalkingPaths paths;
    
    private void Start()
    {
        speed = baseSpeed;
        paths = FindObjectOfType<WalkingPaths>();
        GameManager.onGameStart += Init;
    }

    public void Init()
    {
        if (GameManager.instance.mode == GameMode.TRANSIT || GameManager.instance.mode == GameMode.CAREER)
        {
          
            wp = paths.GetRandom();
            transform.eulerAngles = Vector3.zero;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = wp.start.position;
            dir = (wp.end.position - wp.start.position).normalized;
            transform.LookAt(transform.position + new Vector3(dir.x, 0, dir.z));
            canMove = true;
        }
    }

    public void WalkFaster()
    {
        speed++;

        if (uiInstruction != null)
        {
            GameManager.instance.ManageTime(1);
            StopCoroutine("CTInstruction");
            if ( uiInstruction.gameObject != null)
                Destroy(uiInstruction.gameObject);
        }
    }

    private void Update()
    {
        if (wp != null && canMove)
        {

            transform.position += dir * speed * Time.deltaTime;
            

            if (Vector3.Distance(transform.position, wp.end.position) < 1)
            {
                EndWalk();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.WALK_CROSS)
        {
            GameManager.instance.controller.enabled = false;

            if(!firstTime)
            StartCoroutine(CTInstruction());

            if (wp.dir == Directions.TOP || wp.dir == Directions.BOT)
            {
                GameManager.instance.controller.ModifyBarriers(true);
            }
            else
            {
                GameManager.instance.controller.ModifyBarriers(false);
            }
      
            //esta es la opcion con el botton...sino seria en el update de la persona
            ui = GuiManager.instance.InstantiateUIButton();
            ui.target = transform;
            ui.SetOnClick(WalkFaster);
            ui.SetImage(icon);
            ui.SetText("faster");
            ui.ShowText(true);
            ui.SetOffset(new Vector3(0,1,0));
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == Layers.WALK_CROSS)
        {
            speed = baseSpeed;
            GameManager.instance.controller.enabled = true;
            GameManager.instance.OnOldManPassed(250);
            
            Destroy(ui.gameObject);
          
        }
    }


    IEnumerator CTInstruction()
    {
        firstTime = true;
        //decir que toque el boton
        uiInstruction = GuiManager.instance.InstantiateUIInstructionEmpty();
        uiInstruction.SetText("TAP TO CROSS FASTER");
        uiInstruction.Show(false, false, true);
        uiInstruction.target = transform;
        uiInstruction.offset = new Vector3(0, 8, 0);

        GameManager.instance.ManageTime(0);
        yield return new WaitForSecondsRealtime(5);

        GameManager.instance.ManageTime(1);
        if (uiInstruction != null && uiInstruction.gameObject != null)
            Destroy(uiInstruction.gameObject);
        //sacar el boton;

    }

  

    public void EndWalk()
    {
      
        canMove = false;
        transform.position = new Vector3(0, -20, 0);
        Init();
    }
}
