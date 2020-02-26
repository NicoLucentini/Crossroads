using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{

    public List<BuildingSpawnPoint> spawnPoints;
    public List<GameObject> prefabs;

    public List<GameObject> prefabst1;
    public List<GameObject> prefabst2;
    public List<GameObject> prefabst3;

    public AnimationClip clip;

    public int level = 1;
    

    public void Start()
    {
        TimerManager.onTimePassed += OnGrow;
        GameManager.onGameStart += ResetAll;
    }
    public void ResetAll() {
        foreach (var sp in spawnPoints)
            sp.ResetSpawn();
    }
    public void OnGrow()
    {
        //Elegir uno que este vacio...si no esta vacio eligo cualquiera

        bool emptySpawns = spawnPoints.Exists(x => !x.HasChilds());
        if (emptySpawns)
        {
            foreach (var sp in spawnPoints)
            {
                if (!sp.HasChilds())
                {
                    CreateBuilding(sp, prefabst1[Random.Range(0, prefabst1.Count)]);
                    break;
                }
            }
        }
        else
        {
            int minLevel =  CheckAllLevels();

            BuildingSpawnPoint sp = spawnPoints[Random.Range(0, spawnPoints.Count)];
            CreateBuilding(sp, minLevel == 2 ? prefabst2[Random.Range(0, prefabst2.Count)] : prefabst3[Random.Range(0, prefabst3.Count)]);
        }

       
    }
    public void CreateBuilding(BuildingSpawnPoint spawn, GameObject prefab) {

        if (spawn.GetChild() != null)
            Destroy(spawn.GetChild());

        GameObject go = GameObject.Instantiate(prefab, spawn.transform.position, Quaternion.identity);

        go.transform.eulerAngles = spawn.transform.eulerAngles * -1;
        go.transform.SetParent(spawn.transform);
        if (go.GetComponent<Animation>() == null)
        {
            Animation anim = go.AddComponent<Animation>();
            anim.AddClip(clip, "Grow");
            
            anim.clip = clip;
        }

        spawn.AddLevel();

        go.GetComponent<Animation>().Play();
       
    }

    public int CheckAllLevels() {
        int minValue = int.MaxValue;
        foreach (var sp in spawnPoints) {
            if (sp.level < minValue)
                minValue = sp.level;
        }

        return minValue;
    }
}
