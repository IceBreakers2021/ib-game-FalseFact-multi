using System.Collections;
using System.Collections.Generic;
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


    void Awake()
    {
        // Look for and get the GameManager from previous scene
        gameManager = GameObject.FindObjectOfType<GameManager>();
        // Define default values (in case GameManager does not exist, e.g. when scene is launched without previous scene)
        // ...
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            // ...
        }

        // Other initializations
        playerName = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        imagePlayerIcon.GetComponent<Image>().sprite = playerSprites[sprite_nr];
    }

    public void OnEndEdit_PlayerName(string value)
    {
        playerName = value;
    }

    public void OnClick_Play()
    {
        // TODO: set things up
        SceneManager.LoadScene(sceneName:"MainGame");
    }
}
