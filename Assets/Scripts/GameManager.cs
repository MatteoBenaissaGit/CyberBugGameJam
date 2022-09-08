using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //card list
    [SerializeField] private List<CardData> _cardDataList = new List<CardData>();
    
    //character list
    [SerializeField] private List<CharacterData> _characterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    private List<CharacterData> _charactersDesignerList = new List<CharacterData>();
    private List<CharacterData> _charactersArtistList = new List<CharacterData>();
    private List<CharacterData> _charactersProgrammerList = new List<CharacterData>();
    
    
}

