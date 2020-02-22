using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingPaths : MonoBehaviour
{
    public List<WalkingPath> paths = new List<WalkingPath>();
    public List<WalkingPoint> walkingPoints = new List<WalkingPoint>();

    public WalkingPath GetRandom()
    {
        return paths[Random.Range(0, paths.Count)];
    }

    public WalkingPoint GetRandomWPoint()
    {
        return walkingPoints[Random.Range(0, walkingPoints.Count)];
    }
}
[System.Serializable]
public class WalkingPath
{
    public Transform start;
    public Transform end;
    public Directions dir;
}

[System.Serializable]
public class WalkingPoint
{
    public List<Transform> wp;
}
