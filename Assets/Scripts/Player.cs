using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    //TODO : Need to show player indicator // color.
    public Image indicator;
    public string name;
    public int spriteNumber;
    public string webId;
    //public List facts (false, true, true)

    [System.NonSerialized]
    public int selectedFact = -1;
    [System.NonSerialized]
    public Vector2 indicatorPosition;

    public Player(string name, int spriteNumber,  string webId)
    {
        this.name = name;
        this.spriteNumber = spriteNumber;
        this.webId = webId;
    }

    public Player(){}
}
