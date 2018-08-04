using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]


public class PlayerScore : MonoBehaviour {

    int lastKills = 0;
    int lastDeaths = 0;

    Player player;

	// Use this for initialization
	void Start () {

        player = GetComponent<Player>();

	}

    private void OnDestroy()
    {
        if (player != null)
        SyncNow();
    }

    IEnumerator  SyncScoreLoop ()
    {

        while (true)
        {

            yield return new WaitForSeconds(5f);

            SyncNow();

        
        }
    }



    void SyncNow ()
    {
        if (UserAccountManager.instance = null)
            Debug.Log ("Pas de UserAccountManager boulay");
        if (UserAccountManager.isLoggedIn)
            UserAccountManager.instance.GetData(OnDataReceived);
    }

	void OnDataReceived (string data)
    {

        Debug.Log("data receieve " + data);

        if (player.kills <= lastKills && player.deaths <= lastDeaths)
            return;

        int killsSinceLast = player.kills - lastKills;
        int deathsSinceLast = player.deaths - lastDeaths;

        if (killsSinceLast == 0 && deathsSinceLast == 0)
            return;
        int kills = DataTranslator.DataToKills(data);
        int deaths = DataTranslator.DataToDeaths(data);

        int newKills = killsSinceLast + kills;
        int newDeaths = deathsSinceLast + deaths;

        string newData = DataTranslator.ValuesToData(newKills, newDeaths);

        lastKills = player.kills;
        lastDeaths = player.deaths;

        Debug.Log("New data sent " + newData);
        UserAccountManager.instance.SendData(newData);

    }
}
