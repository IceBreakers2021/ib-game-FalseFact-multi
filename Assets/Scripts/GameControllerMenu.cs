using System.Collections;
using System.Collections.Generic;
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
    private int numberPlayers;
    private string gameID_input;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize GameManager variables in Start() instead of Awake() so that GameManager has time to initialize.
        numberPlayers = 5;
        gameManager.SetNumberPlayers(numberPlayers);
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


    void Goto_Loppy()
    {
        // TODO: save GameID in GameManager
        SceneManager.LoadScene(sceneName:"Lobby");
    }

    public void OnClick_Create()
    {
        Goto_Loppy();
    }

    public void OnClick_Join()
    {
        if (gameID_input == "")
        {
            return;
        }
        else
        {
            Goto_Loppy();
        }
    }

    public void OnEndEdit_GameID(string value)
    {
        gameID_input = value;
    }
}
