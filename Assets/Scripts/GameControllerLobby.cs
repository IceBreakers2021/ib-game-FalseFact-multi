using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerLobby : MonoBehaviour
{
    private GameManager gameManager;

    public GameObject buttonPlay;

    // Variables
    private string gameID;
    
    void Awake()
    {
        // Look for and get the GameManager from previous scene
        gameManager = FindObjectOfType<GameManager>();
        // Define default values (in case GameManager does not exist, e.g. when scene is launched without previous scene)
        gameID = "?";
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            gameID = gameManager.Get_gameID();
        }
        // Other initializations
    }

    //TODO: Handle facts, saving them in game manager, etc. 
    
    public void OnClick_Play()
    {
        // TODO: Sprite / Color selection and propagation to other people.
        // Sends along all necessary info for other clients to add this player to their game
        Join_Game(gameManager.mainPlayer.name, gameManager.mainPlayer.spriteNumber);
        SceneManager.LoadScene(sceneName: "MainGame");
    }
    
    [DllImport("__Internal")]
    private static extern void Join_Game(string playerName, int avatarId);
}