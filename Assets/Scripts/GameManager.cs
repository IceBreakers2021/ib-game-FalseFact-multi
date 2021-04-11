using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Make this object persistent over scenes
    private static GameManager s_Instance = null;

    // Variables
    private int numberPlayers;

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

    public void SetNumberPlayers(int value)
    {
        numberPlayers = value;
    }
    public int GetNumberPlayers()
    {
        return numberPlayers;
    }
}

