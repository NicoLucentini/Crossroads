using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarClaxon : MonoBehaviour
{
    public AudioSource audiosource; 

    public void Play()
    {
        audiosource.Play();
    }

}
