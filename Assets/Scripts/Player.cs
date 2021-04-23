using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Button button;
    //TODO : Need to show player indicator // color.
    public Image indicator;
    public Color color;
    public string name;
    public int spriteNumber;
    public string webId;

    [System.NonSerialized]
    public int selectedFact;
    [System.NonSerialized]
    public Vector2 indicatorPosition;

    public Player(string name, int spriteNumber, Color color, string webId)
    {
        this.name = name;
        this.spriteNumber = spriteNumber;
        this.color = color;
        this.webId = webId;
    }

    public Player(){}
}
