using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private int _numberOfCardsRequiredPerCharacter = 3;
    private int _currentNumberOfCardSelected;
    
    [Header("Data list")]
    public List<CardData> CardDataList = new List<CardData>();
    public List<CharacterData> CharacterDataList = new List<CharacterData>();
    
    //role listing (Empty lists that are completed throughout the game when character's roles are decided)
    private List<CharacterData> _charactersDesignerList = new List<CharacterData>();
    private List<CharacterData> _charactersArtistList = new List<CharacterData>();
    private List<CharacterData> _charactersProgrammerList = new List<CharacterData>();

    [Header("TextMesh References")]
    [SerializeField] private TextMeshProUGUI _designerListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _artistListTextMeshPro;
    [SerializeField] private TextMeshProUGUI _programmerListTextMeshPro;

    [Header("Gauges References")]
    [SerializeField] private Image _designGauge;
    [SerializeField] private Image _artGauge;
    [SerializeField] private Image _programmingGauge;
    [SerializeField] private Image _designBackTempGauge;
    [SerializeField] private Image _artBackTempGauge;
    [SerializeField] private Image _programmingBackTempGauge;

    [Header("Debug")] 
    [SerializeField] private GameState _gameState;
    [SerializeField] private CharacterData _currentCharacter;

    private bool _cardIsCurrentlySelected = false;

    private void Start()
    {
        UIReset();
        ChangeState(GameState.Start);
    }

    private void UIReset()
    {
        //gauges
        _designGauge.fillAmount = 0;
        _artGauge.fillAmount = 0;
        _programmingGauge.fillAmount = 0;
        _designBackTempGauge.fillAmount = 0;
        _artBackTempGauge.fillAmount = 0;
        _programmingBackTempGauge.fillAmount = 0;
        
        //text
        _designerListTextMeshPro.text = "";
        _artistListTextMeshPro.text = "";
        _programmerListTextMeshPro.text = "";
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
        CardSelectionAndActivation();
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
        
        //Anim character

        CardSpawn();
    }
    
    private void CardSpawn()
    {
        print("Card Spawning");
        
        //card spawning animation (other script)
        
        //temporary ! step to the next state directly instead of waiting for the card spawn
        ChangeState(GameState.WaitingForInput);
    }

    private void CardSelectionAndActivation()
    {
        Card cardComponent = null;
        
        //card reveal if mouse over card
        RaycastHit selectionHit;
        Ray selectionRay = Camera.main.ScreenPointToRay(Input.mousePosition); 
        if ( Physics.Raycast (selectionRay,out selectionHit,100.0f)) {
            print("You clicked the " + selectionHit.transform.name);
            //get the card component and check if it isn't null
            if (selectionHit.collider.GetComponent<Card>() != null)
            {
                CardSelection(cardComponent);
                //card activation if click
                if (Input.GetMouseButtonDown(0))
                {
                    cardComponent = selectionHit.collider.GetComponent<Card>();
                    CardActivation(cardComponent);
                }
            }
        }
    }

    private void CardSelection(Card cardComponent)
    {
        //show the card description with animation
    }

    private void CardActivation(Card cardComponent)
    {
        _currentNumberOfCardSelected++;
        
        //gauges modification
        _currentCharacter.AddValueToGauges(cardComponent.gD,cardComponent.gA,cardComponent.gP);
        GaugeAnimation();
        
        //check if the good amount of cards are selected
        if (_currentNumberOfCardSelected >= _numberOfCardsRequiredPerCharacter)
            ChangeState(GameState.CharacterRoleAttribution);
    }

    private void GaugeAnimation()
    {
        //white instant effect behind gauges
        _designBackTempGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Designer] / 100;
        _artBackTempGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Artist] / 100;
        _programmingBackTempGauge.fillAmount = (float)_currentCharacter.RolesGaugesDictionary[Role.Programmer] / 100;
        
        //DOTween effect with real gauge right after
        const float fillAmountTime = .5f;
        _designGauge.DOFillAmount(_designBackTempGauge.fillAmount, fillAmountTime);
        _artGauge.DOFillAmount(_artBackTempGauge.fillAmount, fillAmountTime);
        _programmingGauge.DOFillAmount(_programmingBackTempGauge.fillAmount, fillAmountTime);
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

