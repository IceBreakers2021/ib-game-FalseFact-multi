using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Button button;
    public Image indicator;
    public Color color;

    [System.NonSerialized]
    public int selectedFact;
    [System.NonSerialized]
    public Vector2 indicatorPosition;
}

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
    private GameManager gameManager; // Object for handling variables across scenes
    public GameObject buttonConfirm;
    public Player[] player;
    public float indicatorMoveSpeed;
    public float indicatorMarginStart;
    public float indicatorMargin;
    public float revealedWrongAlpha;
    public GameObject inputFieldTrue1;
    public GameObject inputFieldTrue2;
    public GameObject inputFieldFalse1;
    public GameObject buttonFact1;
    public GameObject buttonFact2;
    public GameObject buttonFact3;
    public FactColors factColors;
    public Text textInstructions;
    public InstructionTexts instructionTexts;

    enum State
    {
        Input,
        Guess,
        Result
    }
    State state;

    string factTrue1;
    string factTrue2;
    string factFalse1;
    int falseFactPosition;
    int activePlayer;

    void Awake()
    {
        // Look for and get the GameManager from previous scene
        gameManager = GameObject.FindObjectOfType<GameManager>();
        // Define default values (in case GameManager does not exist, e.g. when scene is launched without previous scene)
        int playerNumber = player.Length;
        // Set variables to inherited values from GameManager
        if (gameManager != null)
        {
            playerNumber = gameManager.GetNumberPlayers() - 1;
        }

        // Initialize all players as disabled
        for (int i = 0; i < player.Length; i++)
        {
            player[i].button.interactable = false;
            player[i].indicator.enabled = false;
            player[i].selectedFact = 0;
            player[i].indicatorPosition = player[i].indicator.rectTransform.anchoredPosition;
        }
        // Set player number
        if (playerNumber < player.Length)
        {
            System.Array.Resize(ref player, playerNumber);
        }

        // Disable help text
        textInstructions.enabled = false;

        // Go into first state
        GotoState_Input();
    }

    void Update()
    {
        MoveIndicators();
    }

    void GotoState_Input()
    {
        state = State.Input;

        factTrue1 = "";
        factTrue2 = "";
        factFalse1 = "";
        falseFactPosition = 0;
        activePlayer = 0;

        for (int i = 0; i < player.Length; i++)
        {
            player[i].button.interactable = false;
            player[i].indicator.enabled = false;
            player[i].selectedFact = 0;
            player[i].indicatorPosition = player[i].indicator.rectTransform.anchoredPosition;
        }
        SetActive_InputFields(true);
        ClearInputField_InputFields();
        SetActive_ButtonFacts(false);
        SetImageAlpha_PlayerIndicatorStatus(false);

        textInstructions.text = instructionTexts.input;
    }

    void GotoState_Guess()
    {
        state = State.Guess;

        for (int i = 0; i < player.Length; i++)
        {
            player[i].button.interactable = true;
            player[i].selectedFact = 0;
        }
        SetActive_InputFields(false);
        SetActive_ButtonFacts(true);
        SetButtonInteractable_ButtonFacts(true);
        SetImageColor_ButtonFacts_revealed(false);
        SetImageAlpha_PlayerIndicatorStatus(false);

        // Randomize false fact position and write to buttons
        SetRandomFalseFactPositionAndButtonText();

        textInstructions.text = instructionTexts.guess;
    }

    void GotoState_Result()
    {
        state = State.Result;

        for (int i = 0; i < player.Length; i++)
        {
            player[i].button.interactable = false;
            player[i].indicator.enabled = true;
        }
        SetActive_InputFields(false);
        SetActive_ButtonFacts(true);
        SetButtonInteractable_ButtonFacts(false);
        SetImageColor_ButtonFacts_revealed(true);
        SetImageAlpha_PlayerIndicatorStatus(true);

        textInstructions.text = instructionTexts.result;
    }

    void SetActive_InputFields(bool toggle)
    {
        // Attention: GameObjects have to be active in order to be modified!
        inputFieldTrue1.SetActive(toggle);
        inputFieldTrue2.SetActive(toggle);
        inputFieldFalse1.SetActive(toggle);
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

    void ClearInputField_InputFields()
    {
        inputFieldTrue1.GetComponentInParent<InputField>().text = "";
        inputFieldTrue2.GetComponentInParent<InputField>().text = "";
        inputFieldFalse1.GetComponentInParent<InputField>().text = "";
    }

    void SetRandomFalseFactPositionAndButtonText()
    {
        falseFactPosition = Random.Range(1, 4);
        switch (falseFactPosition)
        {
            case 1:
                buttonFact1.GetComponentInChildren<Text>().text = factFalse1;
                buttonFact2.GetComponentInChildren<Text>().text = factTrue1;
                buttonFact3.GetComponentInChildren<Text>().text = factTrue2;
                break;
            case 2:
                buttonFact1.GetComponentInChildren<Text>().text = factTrue1;
                buttonFact2.GetComponentInChildren<Text>().text = factFalse1;
                buttonFact3.GetComponentInChildren<Text>().text = factTrue2;
                break;
            case 3:
                buttonFact1.GetComponentInChildren<Text>().text = factTrue1;
                buttonFact2.GetComponentInChildren<Text>().text = factTrue2;
                buttonFact3.GetComponentInChildren<Text>().text = factFalse1;
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
            switch (falseFactPosition)
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
            switch (falseFactPosition)
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
            for (int i = 0; i < player.Length; i++)
            {
                Color c = player[i].indicator.color;
                if (player[i].indicatorPosition.y == falseFactY)
                {
                    c.a = 1.0f;
                }
                else
                {
                    c.a = revealedWrongAlpha;
                }
                player[i].indicator.color = c;
            }
        }
        else
        {
            for (int i = 0; i < player.Length; i++)
            {
                Color c = player[i].indicator.color;
                c.a = 1.0f;
                player[i].indicator.color = c;
            }
        }
    }

    void SelectFact(int fact_nr, int player_nr)
    {
        if (player_nr > 0)
        {
            int p = player_nr - 1;
            player[p].selectedFact = fact_nr;

            //// Handle indicator positions
            // Get old position
            Vector2 targetPos = player[p].indicatorPosition;
            // Calculate new X positions of other indicators
            for (float xx = (targetPos.x + indicatorMargin); xx < (targetPos.x + player.Length*indicatorMargin); xx += indicatorMargin)
            {
                for (int i = 0; i < player.Length; i++)
                {
                    if (!player[i].indicator.enabled) continue;
                    if (player[i].indicatorPosition.x == xx && player[i].indicatorPosition.y == targetPos.y)
                    {
                        player[i].indicatorPosition.x -= indicatorMargin;
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
            for (float xx = indicatorMarginStart; xx < (indicatorMarginStart + player.Length*indicatorMargin); xx += indicatorMargin)
            {
                bool ok = true;
                for (int i = 0; i < player.Length; i++)
                {
                    if (p == i || !player[i].indicator.enabled) continue;
                    if (player[i].indicatorPosition.x == xx && player[i].indicatorPosition.y == targetPos.y)
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    targetPos.x = xx;
                    break;
                }
            }
            // Apply
            player[p].indicatorPosition = targetPos;

            if (!player[p].indicator.enabled)
            {
                // Set initial indicator position
                player[p].indicator.rectTransform.anchoredPosition = player[p].indicatorPosition;
                player[p].indicator.enabled = true;
            }
        }
    }

    void SelectPlayer(int player_nr)
    {
        activePlayer = player_nr;
        for (int i = 0; i < player.Length; i++)
        {
            player[i].button.interactable = true;
        }
        if (activePlayer > 0)
        {
            player[activePlayer-1].button.interactable = false;
        }
    }

    void MoveIndicators()
    {
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].indicator.rectTransform.anchoredPosition != player[i].indicatorPosition)
            {
                Vector2 distance = player[i].indicatorPosition - player[i].indicator.rectTransform.anchoredPosition;
                if (distance.magnitude <= indicatorMoveSpeed)
                {
                    player[i].indicator.rectTransform.anchoredPosition = player[i].indicatorPosition;
                }
                else
                {
                    Vector2 move = distance;
                    move.Normalize();
                    move *= indicatorMoveSpeed;
                    player[i].indicator.rectTransform.anchoredPosition += move;
                }
            }
        }
    }

    public void OnClick_Confirm()
    {
        switch (state)
        {
            case State.Input:
                if (factTrue1 != "" && factTrue2 != "" && factFalse1 != "")
                {
                    GotoState_Guess();
                }
                break;
            case State.Guess:
                // Check if all have chosen a fact
                bool ok = true;
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i].selectedFact <= 0)
                    {
                        ok = false;
                    }
                }
                if (ok)
                {
                    GotoState_Result();
                }
                break;
            case State.Result:
                GotoState_Input();
                break;
            default:
                Debug.Log("#### Unknown game state!");
                break;
        }
    }

    public void OnClick_Player1()
    {
        SelectPlayer(1);
    }
    public void OnClick_Player2()
    {
        SelectPlayer(2);
    }
    public void OnClick_Player3()
    {
        SelectPlayer(3);
    }
    public void OnClick_Player4()
    {
        SelectPlayer(4);
    }

    public void OnEndEdit_True1(string value)
    {
        factTrue1 = value;
    }
    public void OnEndEdit_True2(string value)
    {
        factTrue2 = value;
    }
    public void OnEndEdit_False1(string value)
    {
        factFalse1 = value;
    }

    public void OnClick_Fact1()
    {
        SelectFact(1, activePlayer);
    }
    public void OnClick_Fact2()
    {
        SelectFact(2, activePlayer);
    }
    public void OnClick_Fact3()
    {
        SelectFact(3, activePlayer);
    }

    public void OnClick_Help()
    {
        textInstructions.enabled = !textInstructions.enabled;
    }
}
