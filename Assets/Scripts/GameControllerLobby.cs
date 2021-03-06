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
    private static extern void Join_Game(string playerName, int avatarId, int hasTold, int selectedFact);

    [DllImport("__Internal")]
    private static extern void Ask_For_Current_Teller();

    [DllImport("__Internal")]
    private static extern void Reply_Current_Teller(string webid, int falsePosition, string false1, string true1,
        string true2);

    private GameManager gameManager;

    public GameObject buttonPlay;
    public GameObject textTitle;
    public GameObject imagePlayerIcon;
    public GameObject textPlayerName;
    public GameObject buttonHelp;
    public string[] instructionTexts;

    // Variables
    private Sprite[] playerSprites;
    private string gameID;

    void Awake()
    {
#if (UNITY_WEBGL == true && UNITY_EDITOR == false)
        Ask_For_Current_Teller();
#endif
        // Look for and get the GameManager from previous scene
        gameManager = FindObjectOfType<GameManager>();
        // Define default values (in case GameManager does not exist, e.g. when scene is launched without previous scene)
        gameID = "?";
        playerSprites = null;
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            gameID = gameManager.Get_gameID();
            playerSprites = gameManager.Get_playerSprites();

            imagePlayerIcon.GetComponent<Image>().sprite = playerSprites[gameManager.mainPlayer.spriteNumber];
            textPlayerName.GetComponent<Text>().text = gameManager.mainPlayer.name;
        }

        // Set the help text
        buttonHelp.GetComponent<HelpInstructions>().SetText(instructionTexts[0]);

        // Other initializations
        textTitle.GetComponent<Text>().text = gameID;

        //TODO: Have some text that tells them what to do, i.e. that they should have facts about themselves.
    }

    public void OnClick_Play()
    {
        //TODO, make this button not interactable until they have filled in the facts, 
        // Have feedback when they try, to remind them they need to fill in the facts.
        if (gameManager.myFacts.Any(fact => fact == ""))
        {
            Debug.Log("Not all facts entered, gameManager is null ==" + (gameManager == null));
            return; // If any fact is not filled, don't allow th play button to work.
        }
        // Sends along all necessary info for other clients to add this player to their game
#if (UNITY_WEBGL == true && UNITY_EDITOR == false)
        Join_Game(gameManager.mainPlayer.name, gameManager.mainPlayer.spriteNumber, gameManager.mainPlayer.hasToldInt(), gameManager.mainPlayer.selectedFact);
#endif
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

    //Parameters should be : webid, falseFactPosition, facts
    public void SetCurrentTeller(string inputParams)
    {
        gameManager.setCurrentTeller(inputParams);
    }

    public void OnRequest_CurrentTeller()
    {
        if (!gameManager.isTelling()) return;
        //A little ugly, unsure if the plugin can do it in a nicer way.
        Debug.Log("We should not have gotten here...");
        Reply_Current_Teller(gameManager.mainPlayer.webId, gameManager.falseFactPosition, gameManager.currentFacts[0],
            gameManager.currentFacts[1], gameManager.currentFacts[2]);
    }

    //Empty methods kept for easier javascript code

    public void OnPlayer_Joins(string input)
    {
        //Ignore for now, instead add from main after asking for it...
    }

    public void OnRequest_Players()
    {
        //Ignore for now, well send this message when we are ready.
    }

    public void OnToldEndOfRound()
    {
        //Ignore for now, new players should not care
    }

    public void OnToldNextRound(string webId)
    {
        //Ignore for now, making facts does not care.
    }
    public void ToldPlayerLeft(string webId)
    {
        //Ignore for now, not in lobby
    }

    public void EndOfGame()
    {
        //Mmmm... should probably do something more clever here.
    }
}
