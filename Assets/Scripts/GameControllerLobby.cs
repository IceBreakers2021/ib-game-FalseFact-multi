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
    public GameObject imagePlayerIcon;
    public GameObject inputFieldPlayerName;

    // Variables
    public Sprite[] playerSprites;
    private string playerName;
    private int spriteNumber;
    private string webId;
    
    void Awake()
    {
        // Look for and get the GameManager from previous scene
        gameManager = FindObjectOfType<GameManager>();
        // Define default values (in case GameManager does not exist, e.g. when scene is launched without previous scene)
        // ...
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            // ...
        }
        // Other initializations
        playerName = "";
        Get_Web_Id(); // Aks react to call Set_Web_Id
    }

    public void Set_Web_Id(string web_id)
    {
        
        webId = web_id;
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

    public void OnEndEdit_PlayerName(string value)
    {
        playerName = value;
    }

    public void OnClick_Play()
    {
        // TODO: Sprite / Color selection and propagation to other people.
        gameManager.mainPlayer = new Player(playerName, Color.red, webId);
        // Sends along all necessary info for other clients to add this player to their game
        Join_Game(playerName, spriteNumber);
        SceneManager.LoadScene(sceneName:"MainGame");
    }
    
    [DllImport("__Internal")]
    private static extern void Join_Game(string playerName, int avatarId);
    
    [DllImport("__Internal")]
    private static extern void Get_Web_Id();
    
    //TODO : Lobby Needs to ask for other players on this channel.
    //TODO : Lobby needs to handle new players joining while creating their character.
}
