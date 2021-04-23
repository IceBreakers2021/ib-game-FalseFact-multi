using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Make this object persistent over scenes
    private static GameManager s_Instance = null;

    // Variables
    private int numberPlayers;
    private string gameID;
    public Player mainPlayer = new Player();

    void Awake()
    {
        // Persistence over scenes handling
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialization code
            numberPlayers = 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Set_numberPlayers(int value)
    {
        numberPlayers = value;
    }
    public int Get_numberPlayers()
    {
        return numberPlayers;
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

