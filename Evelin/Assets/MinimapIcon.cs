using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Player))]
public class MinimapIcon : MonoBehaviour {

    private Player player;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
       
        
        
	}
	
	// Update is called once per frame
	void Update () {
        Player player = gameObject.GetComponentInParent<Player>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //Debug.Log("test " + spriteRenderer.gameObject);
        //Debug.Log("test 2 " + player.isLocalPlayer);
        if (player.isLocalPlayer)
        {
            spriteRenderer.color = new Color(1, 0, 0, 1);
        }
    }
}
