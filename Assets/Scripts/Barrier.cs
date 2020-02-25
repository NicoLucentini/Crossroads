using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Directions direction;

    public Renderer rend;
    public Collider col;

    public void EnableRenderer(bool on)
    {
        rend.enabled = on;
    }
    public void EnableColliders(bool on)
    {
        col.isTrigger = !on;
    }
}
