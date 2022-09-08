using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CardData", menuName = "ScriptableObjects/CardData", order = 1)]
public class CardData : ScriptableObject
{
    [Header("Card")]
    public string CardName;
    public string CardDescription;
    public Sprite cardImageSprite;
    
    [Header("Values")]
    public int DesignValue;
    public int ArtValue;
    public int ProgrammingValue;
}
