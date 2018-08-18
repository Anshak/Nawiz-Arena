using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    private Transform player;
    private GameObject play;

	// Use this for initialization
	void Start () {
       
    }

    public void SetPlayer(Transform _player)
    {
        player = _player;
        
    }

    // Update is called once per frame
    void LateUpdate () {

        if (player) { 
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        //Debug.Log("Minimap position = " + transform.position);
        //Debug.Log("Player position = " + newPosition);

        transform.position = newPosition;
        }
    }
}
