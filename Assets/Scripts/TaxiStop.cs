using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiStop : MonoBehaviour
{
    public static List<TaxiStop> taxiStops = new List<TaxiStop>();
    public Directions dir;
    private void Awake()
    {
        taxiStops.Add(this);
    }

    public static TaxiStop GetStop(Directions dir)
    {
        foreach (var stop in taxiStops)
        {
            if (stop.dir == dir)
                return stop;
        }

        return null;
    }

    
}
