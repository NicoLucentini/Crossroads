using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsManager : MonoBehaviour
{

    public List<Transform> spawnPoints;
    public List<GameObject> prefabs;
    public AnimationClip clip;


    public void Start()
    {
        GameManager.onScore += OnGrow;


    }

    public void OnGrow()
    {
        foreach (var sp in spawnPoints)
        {
            if (sp.childCount == 0)
            {
                CreateBuilding(sp);
                break;
            }
        }
    }
    public void CreateBuilding(Transform spawn) {

        GameObject go = GameObject.Instantiate(prefabs[Random.Range(0, prefabs.Count)], spawn.position, Quaternion.identity);

        go.transform.eulerAngles = spawn.transform.eulerAngles * -1;
        go.transform.SetParent(spawn);
        if (go.GetComponent<Animation>() == null)
        {
            Animation anim = go.AddComponent<Animation>();
            anim.AddClip(clip, "Grow");
            
            anim.clip = clip;
        }

        go.GetComponent<Animation>().Play();
       
    }
}
