using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ambulance : Car
{
    public float maxWaitTime = 8;

    private UISituation ui;
    public Sprite icon;

    public Behaviour halo;

    
    public override void Init()
    {
        base.Init();

        SoundManager.instance.Add(GetComponent<AudioSource>());
        
        ui = GuiManager.instance.InstantiateUISituation();

        ui.Show(false, false, true);
        ui.SetMain(icon);
        ui.SetMask(icon);
        ui.target = transform;
        //ui.SetOffset(new Vector3(0, 3, 0));
        StartCoroutine(CTWaitTime(maxWaitTime));
        StartCoroutine(CTHaloAnim());
    }

    IEnumerator CTWaitTime(float waitTime)
    {
        float wt = waitTime;
        float maxWt = waitTime;
        while (wt > 0)
        {
            wt -= Time.deltaTime;
            ui.SetFill( wt / maxWt);
            ui.SetText(wt.ToString("n1"));
            yield return new WaitForEndOfFrame();
        }

        GuiManager.instance.ChangeEndGameText(carType + " DIDNT ARRIVED ON TIME!!!");
        GameManager.instance.OnCarCollision();
    }

    IEnumerator CTHaloAnim()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            halo.enabled = !halo.enabled;
        }
    }

    protected override void OnGameEnd()
    {
        base.OnGameEnd();


        SoundManager.instance.Remove(GetComponent<AudioSource>());
        if (ui != null)
            Destroy(ui.gameObject);
    }
    public override void OnDie()
    {
        SoundManager.instance.Remove(GetComponent<AudioSource>());
        if (ui != null)
            Destroy(ui.gameObject);
        StopAllCoroutines();
        base.OnDie();
    }
  
}
