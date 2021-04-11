using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControllerMenu : MonoBehaviour
{
    // Object for handling variables across scenes
    public GameManager gameManager;

    public Slider sliderPlayerNumber;
    public Text textPlayerNumberIndicator;

    // Variables
    public int numberPlayers;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize GameManager variables in Start() instead of Awake() so that GameManager has time to initialize.
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
        sliderPlayerNumber.value = numberPlayers;
        textPlayerNumberIndicator.text = numberPlayers.ToString();
    }

    public void PlayerNumberChange(float value)
    {
        numberPlayers = (int)value;
        textPlayerNumberIndicator.text = numberPlayers.ToString();
        gameManager.SetNumberPlayers(numberPlayers);
    }

    public void PressPlay()
    {
        SceneManager.LoadScene(sceneName:"MainGame");
    }
}
