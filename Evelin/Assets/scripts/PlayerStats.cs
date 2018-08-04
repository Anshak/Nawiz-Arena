using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour {

    public Text deathCount;
    public Text killCount;

	// Use this for initialization
	void Start () {
        if (UserAccountManager.isLoggedIn)
        UserAccountManager.instance.GetData(OnReceiveData);
	}
	

    void OnReceiveData(string data)
    {
        Debug.Log("Les data sont " + data);
        if (killCount == null || deathCount == null)
            return;
        killCount.text = DataTranslator.DataToKills(data).ToString() + " KILLS";
        deathCount.text = DataTranslator.DataToDeaths(data).ToString() + " DEATHS";
    }

	// Update is called once per frame
	void Update () {
		
	}
}
