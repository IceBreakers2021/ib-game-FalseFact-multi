using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Make this object persistent over scenes
    private static GameManager s_Instance = null;

    // Variables
    private string gameID;
    public Player mainPlayer = new Player();
    public bool isTelling = false;
    //Sorted list of players, with sorting should have the same order for all players.
    public SortedDictionary<string, Player> players = new SortedDictionary<string, Player>();
    // public int maxPlayers = 1;

    void Awake()
    {
        // Persistence over scenes handling
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Adds a new Player iff we dont already have them.
    public void Add_PLayer(Player newPlayer)
    {
        if (!players.ContainsKey(newPlayer.webId))
        {
            players.Add(newPlayer.webId, newPlayer);
            //Can log here that we got a new player
        }
    }
    public int Get_numberPlayers()
    {
        return players.Count;
    }
    public void Set_gameID(string value)
    {
        gameID = value;
    }
    public string Get_gameID()
    {
        return gameID;
    }
}

