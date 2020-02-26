using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public float gameTimer;
    public float fireEventInterval;

    private Coroutine countingCT;

    public static System.Action onTimePassed;

    public UnityEngine.UI.Text timerText;
    bool pause = false;
    private void Start()
    {   
        //GameManager.onGameStart += DeactivateTimer;
        GameManager.onGameStart += ActivateTimer;
        GameManager.onGameEnd += () => Pause(true);
        GameManager.onGameResume += () => Pause(false);
    }


    private void ActivateTimer() {

        /*if (countingCT != null)
            StopCoroutine(countingCT);
            */
        Pause(false);
        DeactivateTimer();

        countingCT = StartCoroutine(StartCounting());
        InvokeRepeating("FireEvent", fireEventInterval, fireEventInterval);
    }
    private void DeactivateTimer() {
        if (countingCT != null)
            StopCoroutine(countingCT);

        CancelInvoke("FireEvent");
    }
    public void Pause(bool pause)
    {
        this.pause = pause;
    }
    void FireEvent() {
        if (onTimePassed != null)
            onTimePassed(); 
    }
    IEnumerator StartCounting() {
        gameTimer = 0;
        timerText.text = "Time: " + gameTimer.ToString("n2");
        while (true)
        {
            if (!pause)
            {
                gameTimer += Time.deltaTime;
                timerText.text = "Time: " + gameTimer.ToString("n2");
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
