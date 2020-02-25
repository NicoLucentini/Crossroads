using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingSpawnPoint : MonoBehaviour
{
    public int level = 1;
    
    public void AddLevel() {
        level++;    
    }
    public bool HasChilds() {
        return transform.childCount != 0;
    }
    public GameObject GetChild() {
        if (HasChilds())
            return transform.GetChild(0).gameObject;
        else return null;
    }
    public void ResetSpawn() {
        level = 1;
        if (GetChild() != null)
            Destroy(GetChild());
    }
}
