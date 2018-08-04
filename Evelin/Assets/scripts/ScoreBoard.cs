using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour {

    [SerializeField]
    GameObject playerScoreboardItem;

    [SerializeField]
    Transform playerList;

	void OnEnable()
    {
        Player[] players = GameManager.GetAllPlayers();

        foreach (Player player in players)
        {
            GameObject itemGO = (GameObject)Instantiate(playerScoreboardItem, playerList);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();

            if (item != null)
            {
                item.Setup(player.username, player.kills, player.deaths);
            }

        }
    }

    private void OnDisable()
    {
        foreach (Transform child in playerList)
        {
            Destroy (child.gameObject);
        }
    }


}
