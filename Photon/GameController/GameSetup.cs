using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup gameSetup;
    public int nextPlayersTeam;
    public int localPlayerTeam;
    public Transform[] spawnPointsTeamOne;
    public Transform[] spawnpointsTeamTwo;

    private void OnEnable()
    {
        if(GameSetup.gameSetup == null)
        {
            GameSetup.gameSetup = this;
        }
    }

    public void UpdateTeam()
    {
        if(nextPlayersTeam == 1)
        {
            nextPlayersTeam = 2;
        }
        else if(nextPlayersTeam == 2)
        {
            nextPlayersTeam = 1;
        }
    }
}
