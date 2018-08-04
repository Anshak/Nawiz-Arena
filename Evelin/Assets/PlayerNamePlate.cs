using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNamePlate : MonoBehaviour {

    [SerializeField]
    private Text usernameText;

    [SerializeField]
    private Player player;

    [SerializeField]
    private RectTransform healthBarFill;
	
	// Update is called once per frame
	void LateUpdate () {
        usernameText.text = player.username;
        healthBarFill.localScale = new Vector3(player.GetHealPct(), 1f, 1f);


        Camera cam =  Camera.main;
            
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up);
    
}
}
