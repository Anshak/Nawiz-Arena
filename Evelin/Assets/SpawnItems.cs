using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour {

    public Transform[] SpawnPoints;
    public float spawnTime = 8f;

    public GameObject m4LootPrefab;

	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnItem", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SpawnItem ()
    {
        int spawnIndex = Random.Range(0, SpawnPoints.Length);
        Instantiate(m4LootPrefab, SpawnPoints[spawnIndex].position, SpawnPoints[spawnIndex].rotation);
    }
}
