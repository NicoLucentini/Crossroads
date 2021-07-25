using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnectionManager : MonoBehaviour
{
    public static System.Action onInternetConnected;
    public static System.Action onInternetDisconnected;
    public float checkFrequency = 15f;
    public static bool isOnline = true;

    [SerializeField] private bool simulateOffline;

    private void Awake()
    {
        if (simulateOffline)
            isOnline = false;
        else
            CheckForInternetConnection();
    }

    public void CheckForInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (isOnline)
            {
                onInternetDisconnected?.Invoke();
            }

            isOnline = false;
        }
        else
        {
            if (!isOnline)
            {
                isOnline = true;
                onInternetConnected?.Invoke();
            }
        }
        Invoke("CheckForInternetConnection", checkFrequency);
    }
}
