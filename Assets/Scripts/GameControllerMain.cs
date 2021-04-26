using System;
using System.Collections;
using System.Collections.Generic;
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

[System.Serializable]
public class InstructionTexts
{
    public string input;
    public string guess;
    public string result;
}

public class GameControllerMain : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Change_Choice(string playerWebId, int choice);

    [DllImport("__Internal")]
    private static extern void Create_List(string factTrue1, string factTrue2, string falseFact, int falsePosition);

    [DllImport("__Internal")]
    private static extern void RequestChannelPlayers();

    [DllImport("__Internal")]
    private static extern void End_Round();

    [DllImport("__Internal")]
    private static extern void Join_Game(string playerName, int avatarId);


    public GameObject buttonConfirm;
    public float indicatorMoveSpeed;
    public float indicatorMarginStart;
    public float indicatorMargin;
    public float revealedWrongAlpha;
    public GameObject buttonFact1;
    public GameObject buttonFact2;
    public GameObject buttonFact3;
    public FactColors factColors;
    public Text textInstructions;
    public InstructionTexts instructionTexts;

    enum State
    {
        Guess,
        Result
    }

    State state;
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
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            gameID = gameManager.Get_gameID();
        }
        else
        {
            Debug.Log("---------------Game manager is null");
        }

        // Initialize all players as disabled

        // Disable help text
        textInstructions.enabled = false;
        //TODO : Lobby Needs to ask for other players on this channel.
        RequestChannelPlayers();
        //TODO : Any player initialization should be done when players join.
        // for (int i = 0; i < player.Length; i++)
        // {
        //     player[i].indicator.enabled = false;
        //     player[i].selectedFact = 0;
        //     player[i].indicatorPosition = player[i].indicator.rectTransform.anchoredPosition;
        // }
        //If there is currently no teller, then we set ourselves as the current teller.
        // TODO: Current teller needs to tell people who join the game they are the teller.
        if (gameManager.currentTeller == "")
        {
            SetupAsCurrentTeller();
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
        gameManager.myFacts.CopyTo(gameManager.currentFacts,0);
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
        Debug.Log("Going into gotoStateGuess");
        state = State.Guess;
        SetActive_ButtonFacts(true);
        SetButtonInteractable_ButtonFacts(true);
        SetImageColor_ButtonFacts_revealed(false);
        SetImageAlpha_PlayerIndicatorStatus(false);

        // Organizes buttons based on falseFactPosition.
        SetUpButtonText();

        textInstructions.text = instructionTexts.guess;
        Debug.Log("Done with gotoState");
    }

    void GotoState_Result()
    {
        state = State.Result;
        SetActive_ButtonFacts(true);
        SetButtonInteractable_ButtonFacts(false);
        SetImageColor_ButtonFacts_revealed(true);
        SetImageAlpha_PlayerIndicatorStatus(true);

        textInstructions.text = instructionTexts.result;
    }

    void SetActive_ButtonFacts(bool toggle)
    {
        // Attention: GameObjects have to be active in order to be modified!
        buttonFact1.SetActive(toggle);
        buttonFact2.SetActive(toggle);
        buttonFact3.SetActive(toggle);
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

            foreach (var player in gameManager.players.Values)
            {
                Color c = player.indicator.color;
                c.a = Math.Abs(player.indicatorPosition.y - falseFactY) < TOLERANCE ? 1.0f : revealedWrongAlpha;
                player.indicator.color = c;
            }
        }
        else
        {
            foreach (var player in gameManager.players.Values)
            {
                Color c = player.indicator.color;
                c.a = 1.0f;
                player.indicator.color = c;
            }
        }
    }

    void SelectFact(int fact_nr, Player player)
    {
        if (player == gameManager.mainPlayer)
        {
            Change_Choice(player.webId, fact_nr);
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
                if (!playersValue.indicator.enabled) continue;
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
                if (playersValue == gameManager.mainPlayer || !playersValue.indicator.enabled) continue;
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

        if (player.indicator.enabled) return;
        // Set initial indicator position
        player.indicator.rectTransform.anchoredPosition = player.indicatorPosition;
        player.indicator.enabled = true;
    }

    void MoveIndicators()
    {
        foreach (var player in gameManager.players.Values)
        {
            if (player.indicator.rectTransform.anchoredPosition == player.indicatorPosition) continue;
            var distance = player.indicatorPosition - player.indicator.rectTransform.anchoredPosition;
            if (distance.magnitude <= indicatorMoveSpeed)
            {
                player.indicator.rectTransform.anchoredPosition = player.indicatorPosition;
            }
            else
            {
                Vector2 move = distance;
                move.Normalize();
                move *= indicatorMoveSpeed;
                player.indicator.rectTransform.anchoredPosition += move;
            }
        }
    }

    public void OnClick_Confirm()
    {
        switch (state)
        {
            case State.Guess:
                // Check if all have chosen a fact
                var ok = gameManager.players.Values.All(player => player.selectedFact != -1);
                if (ok)
                {
                    End_Round(); // Sends message to everyone to go to result stage.
                    GotoState_Result();
                }

                break;
            case State.Result:
                //TODO: change the activePlayer (current Teller) and mark that the current player has told their stuff.
                GotoState_Guess();
                break;
            default:
                Debug.Log("#### Unknown game state!");
                break;
        }
    }


    public void OnClick_Fact1()
    {
        SelectFact(1, gameManager.mainPlayer);
    }

    public void OnClick_Fact2()
    {
        SelectFact(2, gameManager.mainPlayer);
    }

    public void OnClick_Fact3()
    {
        SelectFact(3, gameManager.mainPlayer);
    }

    /**
     * Occurs when we get data from another client about a choice they made.
     */
    public void OnOtherPlayerSelect(string twoInts)
    {
        //Will get choice (int) and webId (string)
        string[] parameters = twoInts.Split(',');
        //Can only take 1 parameter from Javascript, having to separate it into integers here.
        //Json may be more useful for more complicated inputs
        SelectFact(int.Parse(parameters[0]), gameManager.players[parameters[1]]);
    }

    //From javascript, adds new player to the dictionary of known players, according to their web id.
    public void OnPlayer_Joins(string input)
    {
        //parameters, split to get 3 values, name, sprite, webid
        string[] parameters = input.Split(',');
        Player newPlayer = new Player(parameters[0], int.Parse(parameters[1]), parameters[2]);
        gameManager.Add_PLayer(newPlayer);
    }

    //Another client has joined the channel, and wants to know the list of players in the world.
    public void OnRequest_Players()
    {
        //Easiest is to just resend the Join_Game message
        Join_Game(gameManager.mainPlayer.name, gameManager.mainPlayer.spriteNumber);
    }

    public void OnClick_Help()
    {
        textInstructions.enabled = !textInstructions.enabled;
    }
}