using System;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


[System.Serializable]
public class FactColors
{
    public Color neutralColor;
    public Color trueColor;
    public Color falseColor;
}


public class GameControllerMain : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Change_Choice(string playerWebId, int choice);

    [DllImport("__Internal")]
    private static extern void Tell_Next_Round(string playerWebId);

    [DllImport("__Internal")]
    private static extern void RequestChannelPlayers();

    [DllImport("__Internal")]
    private static extern void End_Round();

    [DllImport("__Internal")]
    private static extern void Join_Game(string playerName, int avatarId, int hasTold);

    [DllImport("__Internal")]
    private static extern void Reply_Current_Teller(string webid, int falsePosition, string false1, string true1,
        string true2);

    public GameObject buttonConfirm;
    public GameObject playerIndicatorPrefab;
    public float indicatorMoveSpeed;
    public float indicatorMarginStart;
    public float indicatorMargin;
    public float revealedWrongAlpha;
    public GameObject buttonFact1;
    public GameObject buttonFact2;
    public GameObject buttonFact3;
    public GameObject confirmButton;
    public FactColors factColors;
    public GameObject imagePlayerIconTeller;
    public GameObject textPlayerNameTeller;
    public GameObject buttonHelp;
    public string[] instructionTexts;
    public GameObject textTitle;

    enum State
    {
        Guess,
        Result
    }

    State state;
    private Sprite[] playerSprites;
    private string gameID;
    private GameManager gameManager; // Object for handling variables across scenes
    private const double TOLERANCE = 0.00001; //Floating point comparison

    void Awake()
    {
        Debug.Log("Main game waking up...");
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
        }
        else
        {
            Debug.Log("---------------Game manager is null");
        }

        // Initialize all players as disabled

        // Other initializations
        textTitle.GetComponent<Text>().text = gameID;

#if (UNITY_WEBGL == true && UNITY_EDITOR == false)
        RequestChannelPlayers();
#elif UNITY_EDITOR == true
        Debug.Log("TODO: Fake players for testing.");
#endif

        SetupSpriteForPlayer(gameManager.mainPlayer);
        //TODO : Any player initialization should be done when players join.
        // for (int i = 0; i < player.Length; i++)
        // {
        //     player[i].indicator.enabled = false;
        //     player[i].selectedFact = 0;
        //     player[i].indicatorPosition = player[i].indicator.rectTransform.anchoredPosition;
        // }
        //If there is currently no teller, then we set ourselves as the current teller.
        if (gameManager.currentTeller == "")
        {
            SetupAsCurrentTeller();
            OnRequest_CurrentTeller(); // Manually sending it here to others who are already making their facts.
            Debug.Log("Done setting up as teller");
        }
        else
        {
            SetupAsPlayer();
            Debug.Log("Done setting up as player");
        }

        GotoState_Guess();
    }

    void SetupAsCurrentTeller()
    {
        gameManager.currentTeller = gameManager.mainPlayer.webId;
        gameManager.falseFactPosition = Random.Range(1, 4);
        gameManager.myFacts.CopyTo(gameManager.currentFacts, 0);
        confirmButton.SetActive(true);
    }

    void SetupAsPlayer()
    {
        //TODO: Should have gotten falseFactPosition, current facts, at the same time as currentTeller
    }

    void Update()
    {
        MoveIndicators();
    }

    void GotoState_Guess()
    {
        foreach (var player in gameManager.players.Values)
        {
            player.indicator.SetActive(false);
            player.selectedFact = -1; //resetting for next round
        }

        Debug.Log("Going into gotoStateGuess");
        state = State.Guess;

        SetButtonInteractable_ButtonFacts(true);
        SetImageColor_ButtonFacts_revealed(false);
        SetImageAlpha_PlayerIndicatorStatus(false);

        // Organizes buttons based on falseFactPosition.
        SetUpButtonText();
        // Set the help text
        buttonHelp.GetComponent<HelpInstructions>().SetText(instructionTexts[0]);
        // Display the current teller name and icon
        var teller = gameManager.players[gameManager.currentTeller];
        textPlayerNameTeller.GetComponent<Text>().text = teller.name;
        imagePlayerIconTeller.GetComponent<Image>().sprite = playerSprites[teller.spriteNumber];

        Debug.Log("Done with gotoState");
    }

    void GotoState_Result()
    {
        state = State.Result;

        SetButtonInteractable_ButtonFacts(false);
        SetImageColor_ButtonFacts_revealed(true);
        SetImageAlpha_PlayerIndicatorStatus(true);

        buttonHelp.GetComponent<HelpInstructions>().SetText(instructionTexts[1]);
    }


    void SetButtonInteractable_ButtonFacts(bool toggle)
    {
        buttonFact1.GetComponentInParent<Button>().interactable = toggle;
        buttonFact2.GetComponentInParent<Button>().interactable = toggle;
        buttonFact3.GetComponentInParent<Button>().interactable = toggle;
    }

    void SetUpButtonText()
    {
        //Current facts, first will always be the false fact.
        switch (gameManager.falseFactPosition)
        {
            case 1:
                buttonFact1.GetComponentInChildren<Text>().text = gameManager.currentFacts[0];
                buttonFact2.GetComponentInChildren<Text>().text = gameManager.currentFacts[1];
                buttonFact3.GetComponentInChildren<Text>().text = gameManager.currentFacts[2];
                break;
            case 2:
                buttonFact1.GetComponentInChildren<Text>().text = gameManager.currentFacts[1];
                buttonFact2.GetComponentInChildren<Text>().text = gameManager.currentFacts[0];
                buttonFact3.GetComponentInChildren<Text>().text = gameManager.currentFacts[2];
                break;
            case 3:
                buttonFact1.GetComponentInChildren<Text>().text = gameManager.currentFacts[1];
                buttonFact2.GetComponentInChildren<Text>().text = gameManager.currentFacts[2];
                buttonFact3.GetComponentInChildren<Text>().text = gameManager.currentFacts[0];
                break;
            default:
                Debug.Log("#### Unknown false fact position!");
                break;
        }
    }

    void SetImageColor_ButtonFacts_revealed(bool toggle)
    {
        if (toggle)
        {
            Color c = factColors.trueColor;
            c.a = revealedWrongAlpha;
            buttonFact1.GetComponentInParent<Image>().color = c;
            buttonFact2.GetComponentInParent<Image>().color = c;
            buttonFact3.GetComponentInParent<Image>().color = c;
            c = factColors.falseColor;
            c.a = 1.0f;
            switch (gameManager.falseFactPosition)
            {
                case 1:
                    buttonFact1.GetComponentInParent<Image>().color = c;
                    break;
                case 2:
                    buttonFact2.GetComponentInParent<Image>().color = c;
                    break;
                case 3:
                    buttonFact3.GetComponentInParent<Image>().color = c;
                    break;
                default:
                    break;
            }
        }
        else
        {
            buttonFact1.GetComponentInParent<Image>().color = factColors.neutralColor;
            buttonFact2.GetComponentInParent<Image>().color = factColors.neutralColor;
            buttonFact3.GetComponentInParent<Image>().color = factColors.neutralColor;
        }
    }

    void SetImageAlpha_PlayerIndicatorStatus(bool toggle)
    {
        if (toggle)
        {
            float falseFactY = 0.0f;
            switch (gameManager.falseFactPosition)
            {
                case 1:
                    falseFactY = buttonFact1.GetComponentInParent<RectTransform>().anchoredPosition.y;
                    break;
                case 2:
                    falseFactY = buttonFact2.GetComponentInParent<RectTransform>().anchoredPosition.y;
                    break;
                case 3:
                    falseFactY = buttonFact3.GetComponentInParent<RectTransform>().anchoredPosition.y;
                    break;
                default:
                    break;
            }
            //TODO: Adjust playerIcon GameObject to show they were wrong.
            // foreach (var player in gameManager.players.Values)
            // {
            //     Color c = player.indicator.color;
            //     c.a = Math.Abs(player.indicatorPosition.y - falseFactY) < TOLERANCE ? 1.0f : revealedWrongAlpha;
            //     player.indicator.color = c;
            // }
        }
        else
        {
            // foreach (var player in gameManager.players.Values)
            // {
            //     Color c = player.indicator.color;
            //     c.a = 1.0f;
            //     player.indicator.color = c;
            // }
        }
    }

    void SelectFact(int fact_nr, Player player)
    {
        if (player == gameManager.mainPlayer)
        {
#if (UNITY_WEBGL == true && UNITY_EDITOR == false)
            Change_Choice(player.webId, fact_nr);
#elif UNITY_EDITOR == true
            Debug.Log("Would have called change_choice");
#endif
        }

        player.selectedFact = fact_nr;

        //// Handle indicator positions
        // Get old position
        Vector2 targetPos = player.indicatorPosition;
        // Calculate new X positions of other indicators
        for (float xx = (targetPos.x + indicatorMargin);
            xx < (targetPos.x + gameManager.Get_numberPlayers() * indicatorMargin);
            xx += indicatorMargin)
        {
            foreach (var playersValue in gameManager.players.Values)
            {
                if (!playersValue.indicator.activeSelf) continue;
                if (Math.Abs(playersValue.indicatorPosition.x - xx) < TOLERANCE &&
                    Math.Abs(playersValue.indicatorPosition.y - targetPos.y) < TOLERANCE)
                {
                    playersValue.indicatorPosition.x -= indicatorMargin;
                }
            }
        }

        // Calculate new Y position of current indicator
        switch (fact_nr)
        {
            case 1:
                targetPos.y = buttonFact1.GetComponentInParent<RectTransform>().anchoredPosition.y;
                break;
            case 2:
                targetPos.y = buttonFact2.GetComponentInParent<RectTransform>().anchoredPosition.y;
                break;
            case 3:
                targetPos.y = buttonFact3.GetComponentInParent<RectTransform>().anchoredPosition.y;
                break;
            default:
                // stay at current Y
                break;
        }

        // Calculate new X position of current indicator
        for (float xx = indicatorMarginStart;
            xx < (indicatorMarginStart + gameManager.Get_numberPlayers() * indicatorMargin);
            xx += indicatorMargin)
        {
            bool ok = true;
            foreach (var playersValue in gameManager.players.Values)
            {
                if (playersValue == gameManager.mainPlayer || !playersValue.indicator.activeSelf) continue;
                if (!(Math.Abs(playersValue.indicatorPosition.x - xx) < TOLERANCE) ||
                    !(Math.Abs(playersValue.indicatorPosition.y - targetPos.y) < TOLERANCE)) continue;
                ok = false;
                break;
            }

            if (!ok) continue;
            targetPos.x = xx;
            break;
        }

        // Apply
        player.indicatorPosition = targetPos;

        if (player.indicator.activeSelf) return;
        // Set initial indicator position
        player.rectTransform.anchoredPosition = player.indicatorPosition;
        player.indicator.SetActive(true);
    }

    void MoveIndicators()
    {
        foreach (var player in gameManager.players.Values)
        {
            if (player.rectTransform.anchoredPosition == player.indicatorPosition) continue;
            var distance = player.indicatorPosition - player.rectTransform.anchoredPosition;
            if (distance.magnitude <= indicatorMoveSpeed)
            {
                player.rectTransform.anchoredPosition = player.indicatorPosition;
            }
            else
            {
                Vector2 move = distance;
                move.Normalize();
                move *= indicatorMoveSpeed;
                player.rectTransform.anchoredPosition += move;
            }
        }
    }

    public void OnClick_Confirm()
    {
        switch (state)
        {
            case State.Guess:
                // Check if all have chosen a fact
                var ok = gameManager.players.Values.All(player =>
                    player.selectedFact != -1 || gameManager.mainPlayer == player);
                if (ok)
                {
                    End_Round(); // Sends message to everyone to go to result stage.
                    gameManager.mainPlayer.hastTold = true;
                    GotoState_Result();
                }

                break;
            case State.Result:
                //The current teller
                var nextTeller = gameManager.getNextTeller();
                confirmButton.SetActive(false);
                if (nextTeller == gameManager.mainPlayer)
                {
                    Debug.Log("End of game!");
                }
                else
                {
                    Tell_Next_Round(nextTeller.webId);
                    gameManager.currentTeller = nextTeller.webId;
                    GotoState_Guess();
                }

                break;
            default:
                Debug.Log("#### Unknown game state!");
                break;
        }
    }


    public void OnClick_Fact1()
    {
        if (gameManager.isTelling()) return;
        SelectFact(1, gameManager.mainPlayer);
    }

    public void OnClick_Fact2()
    {
        if (gameManager.isTelling()) return;
        SelectFact(2, gameManager.mainPlayer);
    }

    public void OnClick_Fact3()
    {
        if (gameManager.isTelling()) return;
        SelectFact(3, gameManager.mainPlayer);
    }

    /**
     * Occurs when we get data from another client about a choice they made.
     */
    public void OnOtherPlayerSelect(string twoInts)
    {
        //Will get choice (int) and webId (string)
        string[] parameters = twoInts.Split('|');
        //Can only take 1 parameter from Javascript, having to separate it into integers here.
        //Json may be more useful for more complicated inputs
        SelectFact(int.Parse(parameters[0]), gameManager.players[parameters[1]]);
    }

    //From javascript, adds new player to the dictionary of known players, according to their web id.
    public void OnPlayer_Joins(string input)
    {
        //parameters, split to get 4 values, name (string), sprite (string),  hasTold (int), webid (string),
        Debug.Log("New Player joined");
        string[] parameters = input.Split('|');
        bool hasTold = int.Parse(parameters[2]) == 1;
        Player newPlayer = new Player(parameters[0], int.Parse(parameters[1]), parameters[3], hasTold);
        SetupSpriteForPlayer(newPlayer);
        gameManager.Add_PLayer(newPlayer);
    }

    //Adds an indicator to the scene and to this player, based on player details.
    public void SetupSpriteForPlayer(Player player)
    {
        //TODO: Display player name somehow on the canvas, maybe another prefab object. Could include score.
        GameObject playerIcon =
            Instantiate(playerIndicatorPrefab, new Vector3(indicatorMarginStart + indicatorMargin, 0, 0),
                Quaternion.identity);
        playerIcon.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        playerIcon.GetComponent<Image>().sprite = playerSprites[player.spriteNumber];
        player.setIndicator(playerIcon);
    }

    //Another client has joined the channel, and wants to know the list of players in the world.
    public void OnRequest_Players()
    {
        //Easiest is to just resend the Join_Game message
        Join_Game(gameManager.mainPlayer.name, gameManager.mainPlayer.spriteNumber,
            gameManager.mainPlayer.hasToldInt());
    }

    public void SetCurrentTeller(string inputParams)
    {
        gameManager.setCurrentTeller(inputParams);
        SetUpButtonText();
    }

    public void OnRequest_CurrentTeller()
    {
        if (!gameManager.isTelling()) return;
        //A little ugly, unsure if the plugin can do it in a nicer way.
        Debug.Log("I am current teller and replying to request for tellers");
        Reply_Current_Teller(gameManager.mainPlayer.webId, gameManager.falseFactPosition, gameManager.currentFacts[0],
            gameManager.currentFacts[1], gameManager.currentFacts[2]);
    }

    public void OnToldEndOfRound()
    {
        gameManager.players[gameManager.currentTeller].hastTold = true;
        GotoState_Result();
        //This is for everyone who is not the current teller
        //if you are next in line, you get to click next button, which should trigger your facts to be sent to
        //everyone.
        //SetCurrentTeller
    }

    //Told by others that the next round is beginning, given webID by the last currentTeller
    public void OnToldNextRound(string webId)
    {
        if (gameManager.mainPlayer.webId == webId)
        {
            SetupAsCurrentTeller();
            OnRequest_CurrentTeller(); // Manually sending it here to others who are already making their facts.
        }
        else
        {
            SetupAsPlayer();
        }

        GotoState_Guess();
    }
    public void ToldPlayerLeft(string webId)
    {
        //Ignore for now, making facts does not care.
        var player = gameManager.players[webId];
        if(player == null) return;
        gameManager.players.Remove(webId);
        Destroy(player.indicator);
    }
    //TODO: Handle on End Round
    //TODO: Teller can't click confirm if there are no other people
    //TODO: Teller can't click confirm if not everyone who has joined has chosen an option
    //TODO: Each Player should have a field for if they have delivered their choices, and send on request for players.
    //TODO: Players should hold state of their choice, and send this along to the other players when they join
    //TODO: Method of moving on to another person, either buttons for teller to click or next person in sorted dictionary.
    //TODO: Pass scene state to react, to avoid calling methods that don't exist.
}
