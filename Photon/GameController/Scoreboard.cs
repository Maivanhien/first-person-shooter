using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] GameObject playerScoreBoardItem;
    [SerializeField] Transform playerList;
    void OnEnable()
    {
        //get an array of players
        PlayerSetup[] players = GameManager.gameManager.GetAllPlayers();
        //Loop through and set up a list item for each one
        foreach(PlayerSetup player in players)
        {
            GameObject itemGO = Instantiate(playerScoreBoardItem, playerList);
            itemGO.GetComponent<PlayerScoreBoardItem>().Setup(player.gameObject.name, player.kills, player.deaths);
        }
    }

    void OnDisable()
    {
        //Clean up our list of items
        foreach(Transform child in playerList)
        {
            Destroy(child.gameObject);
        }
    }
}
