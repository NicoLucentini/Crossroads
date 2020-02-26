using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternetConnectionManager : MonoBehaviour
{
    public static System.Action onInternetConnected;
    public static System.Action onInternetDisconnected;
    public float checkFrequency = 15f;
    public static bool isOnline;

    private void Awake()
    {
        CheckForInternetConnection();
    }

    public void CheckForInternetConnection()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (isOnline)
            {
                if (onInternetDisconnected != null)
                    onInternetDisconnected();
            }

            isOnline = false;
        }
        else
        {
            if (!isOnline)
            {
                isOnline = true;

                if (onInternetConnected != null)
                    onInternetConnected();
            }
        }
        Invoke("CheckForInternetConnection", checkFrequency);
    }
}
