using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxiUser : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    bool canMove;
	void Start ()
    {
        GameManager.onGameEnd += OnGameEnd;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (canMove)
            transform.position += direction * speed * Time.deltaTime;
	}
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
        transform.LookAt(transform.position + dir * 100);
        canMove = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layers.BOUNDARIES)
        {
            Destroy(gameObject);
        }
    }

    void OnGameEnd()
    {
     
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        GameManager.onGameEnd -= OnGameEnd;
    }
}
