using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class GameControllerMenu : MonoBehaviour
{
    // Object for handling variables across scenes
    public GameManager gameManager;

    public GameObject buttonJoin;
    public GameObject inputFieldGameID;
    public GameObject imagePlayerIcon;
    public GameObject inputFieldPlayerName;
    public Text textInstructions;
    public string[] instructionTexts;

    // Variables
    //private int maxNumberPlayers;
    private Sprite[] playerSprites;
    private string gameID_input;
    private int spriteNumber;
    private string playerName;
    private string webId;


    // Start is called before the first frame update
    void Start()
    {
        // Initialize GameManager variables in Start() instead of Awake() so that GameManager has time to initialize.
        //playerSprites[0] = Resources.Load<Sprite>("Sprites/PlayerIcons/default-none");
        playerSprites = gameManager.Get_playerSprites();

        // Select random starting player icon
        spriteNumber = Random.Range(0, playerSprites.Length);
        imagePlayerIcon.GetComponent<Image>().sprite = playerSprites[spriteNumber];

        // Setup help text
        textInstructions.text = instructionTexts[0];
        textInstructions.enabled = false;
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
        playerName = "";
    }

    public void OnClick_Join()
    {
        if (playerName == "")
        {
            //TODO : Feedback to user that they need a player name.
            Debug.Log("No player name");
            //Web_Log("No player name");
            return;
        }

        if (gameID_input == "")
        {
            //TODO : Feedback to user that they need a game id. I.e. set field colour to red.
            Debug.Log("No game id");
            //Web_Log("No game id");
            return;
        }

        gameManager.Set_gameID(gameID_input); // Sets the gameID for Unity
#if (UNITY_WEBGL == true && UNITY_EDITOR == false)
        Set_Lobby(gameID_input); // Sets the channel for the websocket channel
        Get_Web_Id(); // May have to change order of operations here.
#elif UNITY_EDITOR == true
        Set_Web_Id("testID");
#endif
    }

    //A little ugly, but we will join after getting the web id
    public void Set_Web_Id(string web_id)
    {
        webId = web_id;
        gameManager.mainPlayer = new Player(playerName, spriteNumber, webId, false);
        gameManager.players.Add(webId, gameManager.mainPlayer);
        SceneManager.LoadScene(sceneName: "MakeFacts");
    }

    public void OnClick_PlayerIcon()
    {
        // Get current player icon number
        int sprite_nr_current = -1; // -1 if no known icon
        for (int i = 0; i < playerSprites.Length; i++)
        {
            if (imagePlayerIcon.GetComponent<Image>().sprite == playerSprites[i])
            {
                sprite_nr_current = i;
                break;
            }
        }

        // Get next free player icon number
        int sprite_nr = 0;
        if (sprite_nr_current < playerSprites.Length - 1)
        {
            sprite_nr = sprite_nr_current + 1;
        }

        // sprite_nr = Random.Range(0, playerSprites.Length);
        // Change player icon
        spriteNumber = sprite_nr;
        imagePlayerIcon.GetComponent<Image>().sprite = playerSprites[sprite_nr];
    }


    public void OnClick_Help()
    {
        textInstructions.enabled = !textInstructions.enabled;
    }

    public void OnEndEdit_GameID(string value)
    {
        gameID_input = value;
    }

    public void OnEndEdit_PlayerName(string value)
    {
        playerName = value;
    }

    [DllImport("__Internal")]
    private static extern void Set_Lobby(string _channelName);

    [DllImport("__Internal")]
    private static extern void Get_Web_Id();

    [DllImport("__Internal")]
    private static extern void Web_Log(string line);
}
