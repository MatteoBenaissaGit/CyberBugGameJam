using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private int _numberOfCardsRequiredPerCharacter;
    private int _currentNumberOfCardSelected;
    
    [Header("Data list")]
    public List<CardData> CardDataList = new List<CardData>();
    public List<CharacterData> CharacterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    private List<CharacterData> _charactersDesignerList = new List<CharacterData>();
    private List<CharacterData> _charactersArtistList = new List<CharacterData>();
    private List<CharacterData> _charactersProgrammerList = new List<CharacterData>();

    [Header("Events")] 
    public UnityEvent OnCardChosen = new UnityEvent();

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _designerListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _artistListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _programmerListTextMeshPro;

    [Header("Debug")] 
    [SerializeField] private GameState _gameState;
    [SerializeField] private CharacterData _currentCharacter;

    private void Start()
    {
        ChangeState(GameState.Start);
    }

    private void ChangeState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Start:
                GameStart();
                break;
            case GameState.CharacterSpawn:
                CharacterSpawn();
                break;
            case GameState.CardSpawn:
                CardSpawn();
                break;
            case GameState.WaitingForInput:
                break;
            case GameState.CharacterRoleAttribution:
                CharacterRoleAttribution();
                break;
            case GameState.CharacterExit:
                CharacterExit();
                break;
        }

        _gameState = gameState;
    }

    private void Update()
    {
        if (_gameState != GameState.WaitingForInput) return;
        CardSelectionAndAddingToCharacterGauges();
    }

    private void GameStart()
    {
        print("Game Start");
        //starting animation or text ?

        const float secondsToStartCharacterSpawn = 2f;
        StartCoroutine(StartToFirstCharacterSpawn(secondsToStartCharacterSpawn));
    }

    private IEnumerator StartToFirstCharacterSpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ChangeState(GameState.CharacterSpawn);
    }

    private void CharacterSpawn()
    {
        print("Character Spawn");
        //get a random character and make it spawn
        CharacterDataList[0] = _currentCharacter;

        CardSpawn();
    }
    
    private void CardSpawn()
    {
        print("Card Spawning");
        //card spawning
        
        //temporary ! step to the next state directy instead of waiting for the card spawn
        ChangeState(GameState.WaitingForInput);
    }

    private void CardSelectionAndAddingToCharacterGauges()
    {
        _currentNumberOfCardSelected++;
        
        if (Input.GetMouseButtonDown(0)){ 
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if ( Physics.Raycast (ray,out hit,100.0f)) {
                print("You clicked the " + hit.transform.name);
                //get the card component and check if it isnt null
                
                //take the card values
            }
        }
        
        //when a card is selected (how?)

        _currentCharacter.AddValueToGauges(0,0,0);
        
        if (_currentNumberOfCardSelected >= _numberOfCardsRequiredPerCharacter)
            ChangeState(GameState.CharacterRoleAttribution);
    }
    
    private void CharacterRoleAttribution()
    {
        //variable to know if character has been greatly associated with his role
        bool doesCharacterHasPredefinedRole = _currentCharacter.CharacterPredefinedRole == _currentCharacter.CharacterRoleGiver();

        AddCharacterToList(_currentCharacter, _currentCharacter.CharacterRoleGiver());
        ChangeState(GameState.CharacterExit);
    }

    private void CharacterExit()
    {
        //character exit animation
        
        CharacterDataList.Remove(_currentCharacter);
        
        ChangeState(GameState.CharacterSpawn);
    }

    
    private void AddCharacterToList(CharacterData characterData, Role? role)
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

public enum GameState
{
    Start,
    CharacterSpawn,
    CardSpawn,
    WaitingForInput,
    CharacterRoleAttribution,
    CharacterExit
}

