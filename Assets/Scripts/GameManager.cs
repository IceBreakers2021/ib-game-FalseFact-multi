using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Make this object persistent over scenes
    private static GameManager s_Instance = null;

    // Variables
    public Sprite[] playerSprites;
    private string gameID;

    public Player mainPlayer;

    //First fact is always the false one.
    public string[] myFacts = new string[3];
    public string[] currentFacts = new string[3];

    public int falseFactPosition = -1;

    //Who is revealing their facts.
    public string currentTeller = "";

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

    public Sprite[] Get_playerSprites()
    {
        return playerSprites;
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

    public bool isTelling()
    {
        return currentTeller == mainPlayer.webId;
    }

    public Player getNextTeller()
    {
        foreach (KeyValuePair<string, Player> keyVal in players)
        {
            var player = keyVal.Value;
            if (player.hastTold == false)
            {
                return player;
            }
        }

        return mainPlayer;
    }

    public void setCurrentTeller(string inputParams)
    {
        if (isTelling())
        {
            Debug.Log("-----------Current teller ignoring setCurrentTeller call");
            return;
        }

        Debug.Log("-------Setting up the current teller");

        string[] parameters = inputParams.Split('|');
        currentTeller = parameters[0];
        falseFactPosition = int.Parse(parameters[1]);
        //Had an error, might be nicer way to do this, but am tired.
        currentFacts[0] = parameters[2];
        currentFacts[1] = parameters[3];
        currentFacts[2] = parameters[4];
    }
}
