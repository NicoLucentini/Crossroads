using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaxiStop : MonoBehaviour
{
    public static List<TaxiStop> taxiStops = new List<TaxiStop>();
    public Directions dir;
    private void Awake()
    {
        taxiStops.Add(this);
    }

    public static TaxiStop GetStop(Directions dir) =>
        taxiStops.FirstOrDefault(x => x.dir == dir);

}
