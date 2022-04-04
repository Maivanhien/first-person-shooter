using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameObject sceneCamera;
    public static GameManager gameManager;
    private Dictionary<string, PlayerSetup> players;

    public delegate void OnPlayerKilledCallback(string player, string source);
    public OnPlayerKilledCallback onPlayerKilledCallback;

    private void Awake()
    {
        gameManager = this;
    }

    void Start()
    {
        players = new Dictionary<string, PlayerSetup>();
    }

    public void SetupPlayer(string name, PlayerSetup playerSetup)
    {
        players.Add(name, playerSetup);
    }

    public void RemovePlayer(string name)
    {
        players.Remove(name);
    }

    public PlayerSetup[] GetAllPlayers()
    {
        return players.Values.ToArray();
    }
}
