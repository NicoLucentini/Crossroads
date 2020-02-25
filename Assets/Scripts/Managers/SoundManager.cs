using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public List<AudioSource> sources = new List<AudioSource>();
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        GameManager.onGameEnd += OnGameEnd;
    }
    public void OnGameEnd() {
        PauseOnOff(true);
    }
    public void Add(AudioSource a)
    {
        sources.Add(a);
    }
    public void Remove(AudioSource a)
    {
        if (sources.Contains(a))
            sources.Remove(a);
    }

    public void PauseAll()
    {
        foreach (var a in sources)
        {
            a.Pause();
        }
    }
    public void PauseOnOff(bool on)
    {
        if (on) PauseAll();
        else UnPauseAll();
    }

    public void UnPauseAll()
    {
        foreach (var a in sources)
        {
            a.UnPause();
        }
    }
}
