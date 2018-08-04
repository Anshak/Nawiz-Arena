using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScoreBoardItem : MonoBehaviour {

    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killText;

    [SerializeField]
    Text deathText;

    public void Setup(string username, int kills, int deaths)
    {
        usernameText.text = username;
        killText.text = "Kills: " + kills;
        deathText.text = "Deaths: " + deaths;

    }

}
