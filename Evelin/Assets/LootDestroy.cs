using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootDestroy : MonoBehaviour {

    public float destroyTime = 8f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, destroyTime);	
	}
	
	
}
