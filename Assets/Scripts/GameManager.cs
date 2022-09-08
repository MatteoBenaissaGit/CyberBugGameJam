using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Data list")]
    //card list
    public List<CardData> CardDataList = new List<CardData>();
    //character list
    public List<CharacterData> CharacterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    private List<CharacterData> _charactersDesignerList = new List<CharacterData>();
    private List<CharacterData> _charactersArtistList = new List<CharacterData>();
    private List<CharacterData> _charactersProgrammerList = new List<CharacterData>();

    [Header("References")]
    //references
    [SerializeField] private TextMeshProUGUI _designerListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _artistListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _programmerListTextMeshPro;

    
    private void AddCharacterToList(CharacterData characterData, Role role)
    {
        switch (role)
        {
            case Role.Designer:
                _charactersDesignerList.Add(characterData);
                UpdateListsVisual(_charactersDesignerList, _designerListTextMeshPro);
                break;
            case Role.Artist:
                _charactersArtistList.Add(characterData);
                UpdateListsVisual(_charactersArtistList, _artistListTextMeshPro);
                break;
            case Role.Programmer:
                _charactersProgrammerList.Add(characterData);
                UpdateListsVisual(_charactersProgrammerList, _programmerListTextMeshPro);
                break;
        }
    }

    private void UpdateListsVisual(List<CharacterData> characterList, TextMeshProUGUI textMesh)
    {
        string listText = string.Empty;
        foreach (var character in characterList)
        {
            listText += $"\n- {character.CharacterName}";
        }
        textMesh.text = listText;
    }
}

