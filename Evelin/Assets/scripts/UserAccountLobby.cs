using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
	

public class UserAccountLobby : MonoBehaviour {

    public Text usernameText;

	// Use this for initialization
	void Start () {


        if (UserAccountManager.isLoggedIn)
        usernameText.text = "Logged In As:" + UserAccountManager.playerUsername;
        //
        Debug.Log("Object = " + UserAccountManager.playerUsername);

    }
	
	public void LogOut ()
    {
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.LogOut();
    }
}
