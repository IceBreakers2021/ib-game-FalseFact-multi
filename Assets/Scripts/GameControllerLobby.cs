using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerLobby : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Join_Game(string playerName, int avatarId);
    
    private GameManager gameManager;
    public GameObject buttonPlay;
    // Variables
    private string gameID;
    
    void Awake()
    {
        Debug.Log("Lobby waking up...");
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
        //TODO : Would like to have the channel name a the top of the page.
        //TODO: Have some text that tells them what to do, i.e. that they should have facts about themselves.
    }
    
    //TODO : Handle when someone else has joined as first player, save them as activePlayer
    
    public void OnClick_Play()
    {
        //TODO, make this button not interactable until they have filled in the facts, 
        // Have feedback when they try, to remind them they need to fill in the facts.
        if (gameManager.myFacts.Any(fact => fact == ""))
        {
            Debug.Log("Not all facts entered, gameManager is null =="+(gameManager==null));
            return; // If any fact is not filled, don't allow th play button to work.
        }
        // Sends along all necessary info for other clients to add this player to their game
        //TODO: Get the current teller, then join game
        Join_Game(gameManager.mainPlayer.name, gameManager.mainPlayer.spriteNumber);
        SceneManager.LoadScene(sceneName: "MainGame");
    }
    
    public void OnEndEdit_True1(string value)
    {
        gameManager.myFacts[2] = value;
    }
    public void OnEndEdit_True2(string value)
    {
        gameManager.myFacts[1] = value;
    }
    public void OnEndEdit_False1(string value)
    {
        gameManager.myFacts[0] = value;
    }
}