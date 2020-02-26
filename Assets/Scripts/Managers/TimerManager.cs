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

    private void Start()
    {
        GameManager.onGameEnd += DeactivateTimer;
        GameManager.onGameStart += ActivateTimer;
    }


    private void ActivateTimer() {
        if (countingCT != null)
            StopCoroutine(countingCT);

        countingCT = StartCoroutine(StartCounting());
        InvokeRepeating("FireEvent", fireEventInterval, fireEventInterval);
    }
    private void DeactivateTimer() {
        if (countingCT != null)
            StopCoroutine(countingCT);

        CancelInvoke("FireEvent");
    }

    void FireEvent() {
        if (onTimePassed != null)
            onTimePassed(); 
    }
    IEnumerator StartCounting() {
        gameTimer = 0;
        while (true)
        {
            gameTimer += Time.deltaTime;
            timerText.text = "Time: " + gameTimer.ToString("n2");
            yield return new WaitForEndOfFrame();
        }
    }
}
