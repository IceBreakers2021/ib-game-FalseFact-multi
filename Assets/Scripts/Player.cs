using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    //TODO : Need to show player indicator // color.
    public GameObject indicator;
    public RectTransform rectTransform;
    public string name;
    public int spriteNumber;
    public string webId;
    public bool hastTold;
    //public List facts (false, true, true)

    public int selectedFact = -1;
    [System.NonSerialized]
    public Vector2 indicatorPosition;

    public Player(string name, int spriteNumber,  string webId, bool hastTold)
    {
        this.name = name;
        this.spriteNumber = spriteNumber;
        this.webId = webId;
        this.hastTold = hastTold;
    }

    public void setIndicator(GameObject _indicator)
    {
        indicator = _indicator;
        rectTransform = indicator.GetComponent<RectTransform>();
    }

    public int hasToldInt()
    {
        return hastTold ? 1 : 0;
    }
    // public Player(){}
}
