using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerMenu : MonoBehaviour
{
    // Object for handling variables across scenes
    public GameManager gameManager;

    public GameObject buttonCreate;
    public GameObject buttonJoin;
    public GameObject inputFieldGameID;

    // Variables
    private int maxNumberPlayers;
    private string gameID_input;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize GameManager variables in Start() instead of Awake() so that GameManager has time to initialize.
        maxNumberPlayers = 5;
        gameManager.Set_numberPlayers(maxNumberPlayers);
        // Ask for number of players in the game.
    }

    // Update is called once per frame
    void Update()
    {
        return;
    }

    void Awake()
    {
        // Initializations
        gameID_input = "";
    }

    void Go_To_Lobby()
    {
        if (gameID_input == "") return;
        //TODO : Feedback to user that they need a game id. I.e. set field colour to red.
        gameManager.Set_gameID(gameID_input); // Sets the gameID for Unity
        Set_Lobby(gameID_input); // Sets the gameID for the websocket channel
        SceneManager.LoadScene(sceneName: "Lobby");
    }

    public void OnClick_Create()
    {
        Go_To_Lobby(); //TODO: Could create a new id.
    }

    public void OnClick_Join()
    {
        Go_To_Lobby();
    }

    public void OnEndEdit_GameID(string value)
    {
        gameID_input = value;
    }

    [DllImport("__Internal")]
    private static extern void Set_Lobby(string _channelName);
}
