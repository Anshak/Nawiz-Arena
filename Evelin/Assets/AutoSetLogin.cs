using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSetLogin : MonoBehaviour {

    [SerializeField] InputField loginText;
    [SerializeField] InputField passwordText;
    private string storedLogin;
    private string storedPassword;

	// Use this for initialization
	void Start () {

        storedLogin = PlayerPrefsManager.GetPlayerLogin();
              
        loginText.text = storedLogin;
        passwordText.text = storedPassword;


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
